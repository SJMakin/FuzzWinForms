Option Strict On
Imports System.IO
Imports System.Xml.Serialization

Public Class Lithium

    'settings
    Dim finalChunkSize As Integer = 1

    'bits we never want minimising
    Dim before As String()
    Dim after As String()

    'bits to minimize
    Dim parts As New List(Of String)

    'set during init
    Dim tw As New TestWindow
    Dim InterestingError As String = String.Empty
    Dim testcaseDirectory As String = String.Empty
    Dim testcaseFileName As String = "test"

    'working variables
    Dim chunkStart As Integer = 0
    Dim chunkEnd As Integer = 0

    'stats
    Dim testCount As Integer = 0
    Dim testTotal As Integer = 0

    Sub Mimimize()
        Try
            Dim anyChunksRemoved As Boolean = False
            Dim origNumParts As Integer = parts.Count
            Dim chunkSize As Integer = largestPowerOfTwoSmallerThan(parts.Count)

            While True
                If chunkStart >= parts.Count Then 'finished loop
                    Dim last As Boolean = chunkSize = finalChunkSize
                    Dim empty As Boolean = parts.Count = 0

                    If Not empty And anyChunksRemoved And last Then
                        chunkStart = 0
                        Log("Starting another round of chunk size " & chunkSize.ToString)
                    ElseIf empty Or last
                        Log("Lithium result: succeeded, reduced to: " & parts.Count.ToString)
                        Exit While
                    Else
                        chunkStart = 0
                        chunkSize = CInt(chunkSize / 2)
                        Log("Halving chunk size to " & chunkSize.ToString)

                    End If
                    anyChunksRemoved = False
                End If

                Dim description As String = "Removing a chunk of size " + chunkSize.ToString + " starting at " + chunkStart.ToString + " of " + parts.Count.ToString

                If interesting(chunkStart, chunkSize, True) Then
                    Log(description + " was a successful reduction :)")
                    anyChunksRemoved = True
                    'leave chunkStart the same
                Else
                    Log(description + " made the file 'uninteresting'.")
                    chunkStart += chunkSize
                End If

            End While

            writeTestcase(testcaseFileName)

            Log("=== LITHIUM SUMMARY ===")
            If chunkSize = 1 And Not anyChunksRemoved Then
                Log("  Removing any single step from the final file makes it uninteresting!")
            End If

            Log("  Initial size: " & origNumParts.ToString)
            Log("  Final size: " & parts.Count.ToString)
            Log("  Tests performed: " & testCount.ToString)
            Log("  Test total: " & testTotal.ToString)

            MsgBox("Done!", MsgBoxStyle.Exclamation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

    Function interesting(ByVal ichunkStart As Integer, ByVal chunkSize As Integer, Optional ByVal writeIt As Boolean = True) As Boolean
        chunkSize = Math.Min(chunkSize, parts.Count - ichunkStart)
        Dim oldParts As New List(Of String)
        oldParts.AddRange(parts)  ' would rather be less side-effecty about this, And be passing partsSuggestion around
        parts.RemoveRange(ichunkStart, chunkSize)

        If writeIt Then writeTestcase(testcaseFileName)

        testCount += 1
        testTotal += parts.Count

        Dim inter As Boolean = CheckReplay()

        ' Save an extra copy Of the file inside the temp directory.
        ' This Is useful If you're reducing an assertion and encounter a crash:
        ' it gives you a way To Try To reproduce the crash.
        'If String.IsNullOrEmpty(tempDir) Then

        '    Dim tempFileTag As String = IIf(inter, "interesting", "boring")
        '    writeTestcaseTemp(tempFileTag, True)
        'End If

        If Not inter Then
            parts.Clear()
            parts.AddRange(oldParts)
        End If

        Return inter

    End Function

    'Sub writeTestcaseTemp(ByVal partialFilename As String, ByVal useNumber As Boolean)
    '    If useNumber Then
    '        partialFilename = Str(tempFileCount) + "-" + partialFilename
    '        tempFileCount += 1
    '        writeTestcase(Path.Combine(tempDir, partialFilename & testcaseExtension))
    '    End If
    'End Sub

    Sub writeTestcase(filename As String)
        Using SR As New StreamWriter(filename)
            SR.WriteLine(before)

            Dim tempParts As New List(Of String)
            tempParts.AddRange(before)
            tempParts.AddRange(parts)

            For Each str As String In tempParts
                SR.WriteLine(str)
            Next

            SR.WriteLine(after)
        End Using
    End Sub

    Sub Log(s As String)
        Debug.Write(s)

        Using sr As New StreamWriter(testcaseDirectory & "\LithiumLog.txt", True)
            sr.WriteLine(s)
        End Using

    End Sub

    Function largestPowerOfTwoSmallerThan(ByVal n As Integer) As Integer
        Dim i As Integer = 1

        While i * 2 < n
            i *= 2
        End While

        Return i
    End Function

    Public Sub InitialiseReplay(filename As String)

        Dim replaypath As String = Path.GetDirectoryName(filename)
        testcaseDirectory = replaypath
        testcaseFileName = replaypath & "\MinimisedSteps.txt"

        Using sr As New StreamReader(replaypath & "\EventLog.txt")
            InterestingError = sr.ReadToEnd
        End Using

        Dim xSer As New XmlSerializer(GetType(TestWindow))
        Using sr2 As New StreamReader(replaypath & "\Settings.xml")
            tw = DirectCast(xSer.Deserialize(sr2), TestWindow)
        End Using

        Dim allTestSteps As String
        Using sr3 As New StreamReader(filename)
            allTestSteps = sr3.ReadToEnd
        End Using

        parts = allTestSteps.Split(vbNewLine.ToCharArray, StringSplitOptions.RemoveEmptyEntries).ToList

        Array.Resize(before, 4)

        parts.CopyTo(0, before, 0, 4)
        parts.RemoveRange(0, 4)
    End Sub

    Function CheckReplay() As Boolean
        tw.ResetTest()

        Dim tempParts As New List(Of String)
        tempParts.AddRange(before)
        tempParts.AddRange(parts)
        ' tempParts.AddRange(after)

        tw.ReplayTest(tempParts.ToArray)

        If LevenshteinDistance.Compute(tw.eventLogs.Trim, InterestingError.Trim) <= tw.eventLogMatchDistance Then Return True

        Return False

    End Function

End Class