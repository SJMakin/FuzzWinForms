Option Strict On

Imports System.Reflection
Imports System.Xml.Serialization
Imports System.Threading
Imports System.IO

Public Enum TestWindowExitCode
    NoError = 0
    Timeout = 1
    ProcessExit = 2
    StrayOffScreen = 3
    FatalError = 4
    Interrupt = 5
End Enum

Public Class TestWindow

    'Ins
    Public Property startTimeOut As Integer = 5000
    Public Property iterationTimeOut As Integer = 1024
    Public Property delayBetweenActions As Integer = 100
    Public Property recordImages As Boolean = False
    Public Property targetFileName As String
    Public Property windowTitleSize As Integer = 35
    Public Property windowEdges As Integer = 9

    Public Property preTestActions As New List(Of String)
    Public Property postTestActions As New List(Of String)

    Public Property eventLogFilterInclude As New List(Of String)
    Public Property eventLogFilterExclude As New List(Of String)
    Public Property eventLogMatchDistance As Integer = 5

    'Outs
    <XmlIgnoreAttribute()> Public Property eventLogs As String = String.Empty
    <XmlIgnoreAttribute()> Public Property replayLog As String = String.Empty
    <XmlIgnoreAttribute()> Public Property exitCode As TestWindowExitCode

    'chances
    Public Property rMouseMove As Integer = 20
    Public Property rMouseClickLeft As Integer = 30
    Public Property rMouseClickRight As Integer = 40
    Public Property rMouseClickMiddle As Integer = 41
    Public Property rMouseClickAndDrag As Integer = 50
    Public Property rKeyBoardSpam As Integer = 96
    Public Property rNaughtyString As Integer = 98
    Public Property rKeyBoardAction As Integer = 100

    Public Property rKeyBoardFunctionKey As Integer = 101

    Public Property keyboardModifierPercent As Integer = 10

    'Locals
    Private winHelper As New WindowHelperNativeMethods
    Private windowSize As WindowHelperNativeMethods.RECT
    Private hWnd As New IntPtr
    Private currentHwnd As New IntPtr
    Private CurrentWindowText As String = String.Empty
    Private pID As New Integer
    Private iteration As Integer = 0
    Private Shared rN As New Random
    Private WithEvents evtApp As New EventLog("Application")

    Private ActionKeys() As Short = {CShort(WindowHelperNativeMethods.VirtualKeys.VK_TAB), CShort(WindowHelperNativeMethods.VirtualKeys.VK_ESCAPE), CShort(WindowHelperNativeMethods.VirtualKeys.VK_RETURN)}
    Private FunctionKeys() As Short = {CShort(WindowHelperNativeMethods.VirtualKeys.VK_F1), CShort(WindowHelperNativeMethods.VirtualKeys.VK_F2), CShort(WindowHelperNativeMethods.VirtualKeys.VK_F3), CShort(WindowHelperNativeMethods.VirtualKeys.VK_F4), CShort(WindowHelperNativeMethods.VirtualKeys.VK_F5), CShort(WindowHelperNativeMethods.VirtualKeys.VK_F6), CShort(WindowHelperNativeMethods.VirtualKeys.VK_F7), CShort(WindowHelperNativeMethods.VirtualKeys.VK_F8), CShort(WindowHelperNativeMethods.VirtualKeys.VK_F9), CShort(WindowHelperNativeMethods.VirtualKeys.VK_F10), CShort(WindowHelperNativeMethods.VirtualKeys.VK_F11), CShort(WindowHelperNativeMethods.VirtualKeys.VK_F12)}
    Private ModifierKeys() As Short = {CShort(WindowHelperNativeMethods.VirtualKeys.VK_CONTROL), CShort(WindowHelperNativeMethods.VirtualKeys.VK_SHIFT), CShort(WindowHelperNativeMethods.VirtualKeys.VK_MENU)}

    Private NaughtyStrings As New List(Of String)

    'methods

    Public Sub New()
        evtApp.EnableRaisingEvents = True

        Dim AllNaughtyStrings As String = My.Resources.NaughtyStrings
        NaughtyStrings = AllNaughtyStrings.Split(vbNewLine.ToCharArray, StringSplitOptions.RemoveEmptyEntries).ToList
        Dim comments As New List(Of String)
        For Each s As String In NaughtyStrings
            If s.StartsWith("#") Then comments.Add(s)
        Next

        For Each s As String In comments
            NaughtyStrings.Remove(s)
        Next

    End Sub

    Public Sub ResetTest()
        eventLogs = String.Empty

        replayLog = String.Empty

        exitCode = TestWindowExitCode.NoError

        hWnd = IntPtr.Zero
        currentHwnd = IntPtr.Zero
        pID = 0

        iteration = 0
    End Sub

    Private Sub RunActions(ByVal actions As List(Of String))
        evtApp.EnableRaisingEvents = False

        For Each action In actions
            Try
                Shell(action, AppWinStyle.NormalNoFocus, True, -1)
            Catch ex As Exception
                MsgBox(action & " failed." & vbNewLine & ex.ToString)
                Throw
            End Try

        Next

        evtApp.EnableRaisingEvents = True
    End Sub

    Private Sub evtApp_EntryWritten(ByVal sender As Object, ByVal e As System.Diagnostics.EntryWrittenEventArgs) Handles evtApp.EntryWritten
        Dim addEntry As Boolean = eventLogFilterInclude.Count = 0 'If no include filter default to yes

        For Each s As String In eventLogFilterInclude
            If e.Entry.Message.Contains(s) Then
                addEntry = True
            End If
        Next

        For Each s As String In eventLogFilterExclude
            If e.Entry.Message.Contains(s) Then
                addEntry = False
            End If
        Next

        If addEntry Then eventLogs += e.Entry.Message & vbNewLine & vbNewLine

    End Sub

    Public Shared Function newRandomNumber(ByVal minValue As Integer, ByVal maxValue As Integer) As Integer
        If minValue > maxValue Then Return minValue

        Return rN.Next(minValue, maxValue)
    End Function

    Public Event TestComplete As EventHandler
    Public Event ReplayComplete As EventHandler

    Private Function StartProcess(ByRef p As Process, ByVal filename As String) As IntPtr
        Dim pStartInfo As New ProcessStartInfo
        pStartInfo.FileName = filename
        pStartInfo.WindowStyle = ProcessWindowStyle.Maximized
        p.StartInfo = pStartInfo ' pStartInfo.UseShellExecute = False
        p.Start()

        While p.MainWindowHandle = IntPtr.Zero
            Thread.Sleep(10)
        End While

        p.WaitForInputIdle()

        pID = p.Id

        While WindowHelperNativeMethods.GetHandleByProcessID(pID) = IntPtr.Zero
            Thread.Sleep(10)
        End While

        hWnd = WindowHelperNativeMethods.GetHandleByProcessID(pID)

        winHelper.SetWindowActive(hWnd, True)

    End Function

    Public Sub StartTest()
        Try
            exitCode = TestWindowExitCode.NoError

            RunActions(preTestActions)

            Using p As New Process

                StartProcess(p, targetFileName)

                '
                'pID = winHelper.hWnd2pID(hWnd).ToInt32
                Thread.Sleep(startTimeOut)

                replayLog = "STARTING TARGET:" & targetFileName & vbNewLine
                replayLog += "DATE: " & Now & vbNewLine
                windowSize = winHelper.updateWindowSize(hWnd)
                replayLog += "START WINDOW SIZE: " & windowSize.Top & " " & windowSize.Bottom & " " & windowSize.Left & " " & windowSize.Right & " " & vbNewLine
                Dim currentMousePos As Point = winHelper.GetCursor
                replayLog += "START MOUSE POSITION: " & currentMousePos.ToString & vbNewLine
                currentHwnd = hWnd

                While Not ProblemsFound()
                    iteration += 1
                    windowSize = winHelper.updateWindowSize(currentHwnd)
                    sendRandomInput()
                    Thread.Sleep(delayBetweenActions)
                End While

                If Not p.HasExited Then p.Kill()
            End Using

            RunActions(postTestActions)

        Catch ex As Exception
            If Not TypeOf (ex) Is ThreadAbortException Then
                eventLogs += ex.ToString & vbNewLine
                exitCode = TestWindowExitCode.FatalError
            End If
        Finally
            RaiseEvent TestComplete(Me, New EventArgs())
        End Try
    End Sub

    Public Sub ReplayTest(ByVal testSteps() As String)
        Try
            RunActions(preTestActions)

            exitCode = TestWindowExitCode.NoError

            Using p As New Process

                StartProcess(p, testSteps(0).Replace("STARTING TARGET:", ""))

                Thread.Sleep(CInt(startTimeOut / 2))

                ' winHelper.SetWindowActive(hWnd)

                currentHwnd = hWnd

                'Need to move to config
                'Set Window as per replay.
                '   Dim startWindowSizes() As String = testSteps(2).Replace("START WINDOW SIZE: ", "").Split(" ".ToCharArray, StringSplitOptions.RemoveEmptyEntries)

                '                Dim rct As New WindowHelper.RECT
                '               rct.Top = CInt(startWindowSizes(0))
                '              rct.Bottom = CInt(startWindowSizes(1))
                '             rct.Left = CInt(startWindowSizes(2))
                '            rct.Right = CInt(startWindowSizes(3))

                'winHelper.SetWindowPosition(hWnd, rct)

                'Set Mouse as per replay.
                Dim mousepos As String = testSteps(3).Replace("START MOUSE POSITION: ", "")
                Dim x As String = New String(mousepos.Split(",".ToCharArray)(0).Where(Function(c As Char) [Char].IsDigit(c)).ToArray())
                Dim y As String = New String(mousepos.Split(",".ToCharArray)(1).Where(Function(c As Char) [Char].IsDigit(c)).ToArray())
                Dim startmousepostion As Point = New Point(Integer.Parse(x), Integer.Parse(y))
                winHelper.SetCusor(startmousepostion.X, startmousepostion.Y)

                Thread.Sleep(CInt(startTimeOut / 2))

                replayLog = "STARTING TARGET:" & targetFileName & vbNewLine
                replayLog += "DATE: " & Now & vbNewLine
                windowSize = winHelper.updateWindowSize(hWnd)
                replayLog += "START WINDOW SIZE: " & windowSize.Top & " " & windowSize.Bottom & " " & windowSize.Left & " " & windowSize.Right & " " & vbNewLine
                Dim currentMousePos As Point = winHelper.GetCursor
                replayLog += "START MOUSE POSITION: " & currentMousePos.ToString & vbNewLine
                currentHwnd = hWnd

                'Get going.
                While iteration <= testSteps.Length - 1 And Not ProblemsFound()
                    If sendReplayInput(testSteps(iteration)) Then Thread.Sleep(delayBetweenActions) 'Message sent to app
                    iteration += 1
                End While

                'Done.
                If Not p.HasExited Then p.Kill()
            End Using

            RunActions(postTestActions)

            Using sr As New StreamWriter("C:\temp\FUZZ\replaylog.txt")
                sr.Write(replayLog)
            End Using
        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            RaiseEvent ReplayComplete(Me, New EventArgs())
        End Try

    End Sub

    Function sendReplayInput(ByVal input As String) As Boolean
        Try
            If input.StartsWith("MOUSEMOVE") Then
                Dim splitInput() As String = input.Split(" ".ToCharArray)
                Dim x As Integer = Integer.Parse(splitInput(1))
                Dim y As Integer = Integer.Parse(splitInput(2))
                winHelper.SetCusor(x, y)

            ElseIf input.StartsWith("CLICKLEFT") Then
                winHelper.ClickLeft()
            ElseIf input.StartsWith("CLICKRIGHT") Then
                winHelper.ClickRight()
            ElseIf input.StartsWith("CLICKMIDDLE") Then
                winHelper.ClickMiddle()
            ElseIf input.StartsWith("CLICKANDDRAG") Then
                Dim splitInput() As String = input.Split(" ".ToCharArray)
                Dim x As Integer = Integer.Parse(splitInput(1))
                Dim y As Integer = Integer.Parse(splitInput(2))
                winHelper.ClickAndDrag(x, y)
            ElseIf input.StartsWith("SEND") Then
                Dim splitInput() As String = input.Split(" ".ToCharArray)
                Dim sAscii As Short = Short.Parse(splitInput(1))
                Dim m As Short = 0
                If splitInput.Length = 3 AndAlso Short.TryParse(splitInput(2), m) Then
                    winHelper.SendKey(sAscii, m)
                Else
                    winHelper.SendKey(sAscii)
                End If
            ElseIf input.StartsWith("NAUGHTYSTRING") Then
                Dim splitInput() As String = input.Split(" ".ToCharArray, 2)
                Dim naughtystr = splitInput(1)
                winHelper.SendKey(naughtystr)
            ElseIf input.StartsWith("WAIT")
                'Do nothing, but return true.  MouseIsInActiveWindow was false
            Else
                Return False
            End If

        Catch ex As Exception

            MsgBox(input & vbNewLine & ex.ToString)
            Thread.CurrentThread.Abort()

        End Try

        replayLog += input & vbNewLine

        Return True

    End Function

    Private Function MouseIsInActiveWindow(ByVal MousePosition As Point, ByVal WindowSize As WindowHelperNativeMethods.RECT) As Boolean
        If MousePosition.X > WindowSize.Left AndAlso MousePosition.X < WindowSize.Right Then
            If MousePosition.Y > WindowSize.Top AndAlso MousePosition.X < WindowSize.Bottom Then
                Return True
            End If
        End If

        Return False

    End Function

    Sub sendRandomInput()
        Dim r As Integer = newRandomNumber(1, 100)

        If rMouseMove > r Then
            Dim x As Integer = newRandomNumber(windowSize.Left + windowEdges, windowSize.Right - windowEdges)
            Dim y As Integer = newRandomNumber(windowSize.Top + windowTitleSize, windowSize.Bottom - windowEdges)
            winHelper.SetCusor(x, y)
            replayLog += "MOUSEMOVE " & x & " " & y & vbNewLine
        ElseIf rMouseClickLeft > r Then
            If MouseIsInActiveWindow(winHelper.GetCursor, windowSize) Then
                winHelper.ClickLeft()
                replayLog += "CLICKLEFT" & vbNewLine
            Else
                replayLog += "WAIT" & vbNewLine
            End If
        ElseIf rMouseClickRight > r Then
            If MouseIsInActiveWindow(winHelper.GetCursor, windowSize) Then
                winHelper.ClickRight()
                replayLog += "CLICKRIGHT" & vbNewLine
            Else
                replayLog += "WAIT" & vbNewLine
            End If
        ElseIf rMouseClickMiddle > r Then
            If MouseIsInActiveWindow(winHelper.GetCursor, windowSize) Then
                winHelper.ClickMiddle()
                replayLog += "CLICKMIDDLE" & vbNewLine
            Else
                replayLog += "WAIT" & vbNewLine
            End If
        ElseIf rMouseClickAndDrag > r Then
            If MouseIsInActiveWindow(winHelper.GetCursor, windowSize) Then
                Dim x As Integer = newRandomNumber(windowSize.Left + windowEdges, windowSize.Right - windowEdges)
                Dim y As Integer = newRandomNumber(windowSize.Top + windowTitleSize, windowSize.Bottom - windowEdges)
                winHelper.ClickAndDrag(x, y)
                replayLog += "CLICKANDDRAG " & x & " " & y & vbNewLine
            Else
                replayLog += "WAIT" & vbNewLine
            End If

        ElseIf rKeyBoardSpam > r Then
            'Key codes:
            'https://msdn.microsoft.com/en-us/library/dd375731%28VS.85%29.aspx
            Dim randomAscii As Short = CShort(newRandomNumber(32, 82))
            If newRandomNumber(1, 100) < keyboardModifierPercent Then
                Dim modifier As Short = ModifierKeys(newRandomNumber(1, ModifierKeys.Length))
                winHelper.SendKey(randomAscii, modifier)
                replayLog += "SENDKEY " & randomAscii & " " & modifier & " " & vbNewLine
            Else
                winHelper.SendKey(randomAscii)
                replayLog += "SENDKEY " & randomAscii & vbNewLine
            End If
        ElseIf rNaughtyString > r Then
            Dim naughtystr As String = NaughtyStrings(newRandomNumber(0, NaughtyStrings.Count - 1))
            winHelper.SendKey(naughtystr)
            replayLog += "NAUGHTYSTRING " & naughtystr & vbNewLine
        ElseIf rKeyBoardAction > r Then
            Dim randomAction As Integer = newRandomNumber(1, ActionKeys.Length) - 1
            winHelper.SendKey(ActionKeys(randomAction))
            replayLog += "SENDACTIONKEY " & ActionKeys(randomAction) & vbNewLine
        ElseIf rKeyBoardFunctionKey > r Then
            Dim randomFunction As Integer = newRandomNumber(1, FunctionKeys.Length) - 1
            winHelper.SendKey(FunctionKeys(randomFunction))
            replayLog += "SENDKEY " & FunctionKeys(randomFunction) & vbNewLine
        End If
    End Sub

    Function ProblemsFound() As Boolean
        Dim lastinputtime As Integer = winHelper.GetLastInputTime
        If lastinputtime <= delayBetweenActions / 2 Then
            ' MsgBox("Interrupt")
            replayLog += "Interrupt"
            exitCode = TestWindowExitCode.Interrupt
            Return True
        End If
        Dim newCurrentWindowText = winHelper.fGetWindowText(currentHwnd)
        If CurrentWindowText <> newCurrentWindowText Then
            CurrentWindowText = newCurrentWindowText
            replayLog += "NEW WINDOW FOCUS: " & newCurrentWindowText & vbNewLine
        End If

        'Process stopped?!
        Try
            Dim p As Process = Process.GetProcessById(pID)
        Catch ex As ArgumentException
            replayLog += "Process exit"
            exitCode = TestWindowExitCode.ProcessExit
            Return True
        End Try

        'Target somehow become unactive
        Dim currentWindow As IntPtr = winHelper.CurrentWindow
        ' If currentWindow = IntPtr.Zero Then
        ' winHelper.SetWindowActive(currentHwnd)
        ' End If

        'We have opened a new sub window or have strayed of our target
        If currentWindow <> currentHwnd Then
            If winHelper.windowIsRelated(currentWindow, hWnd, pID) OrElse currentWindow = hWnd Then
                currentHwnd = currentWindow
            Else
                exitCode = TestWindowExitCode.StrayOffScreen
                Return True
            End If
        End If

        'Done too much
        If iteration > iterationTimeOut Then
            exitCode = TestWindowExitCode.Timeout
            Return True
        End If

        Return False
    End Function

End Class