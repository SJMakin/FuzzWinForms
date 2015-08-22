Option Strict On

Imports System.Threading
Imports System.IO
Imports System.Xml.Serialization

Public Class MainWindow
    Dim winHelper As New WindowHelper
    Dim tw As New TestWindow
    Dim t As New Thread(AddressOf tw.StartTest)
    Dim InterestingError As String


    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles dtnStart.Click
        StartTest()
    End Sub

    Public Sub StartTest()
        Dim target As String = txtFileName.Text
        If Not String.IsNullOrWhiteSpace(target) AndAlso target.EndsWith("exe") AndAlso File.Exists(target) Then
            tw.ResetTest()
            t = New Thread(AddressOf tw.StartTest)
            tw.targetFileName = target
            t.Start()

            AddHandler tw.TestComplete, AddressOf HandleTestComplete

        End If
    End Sub


    Public Sub HandleTestComplete()
        RemoveHandler tw.TestComplete, AddressOf HandleTestComplete

        createReport()

        If chkKeepGoing.Checked = True Then
            Me.Invoke(Sub() StartTest()) 'Start the test on the UI thread 
        End If
    End Sub

    Public Sub HandleReplayComplete()
        RemoveHandler tw.ReplayComplete, AddressOf HandleReplayComplete

        If tw.eventLogs.Trim = InterestingError.Trim Then
            MsgBox("Error reproduced- Expecting error:" & vbNewLine & InterestingError)
        Else
            MsgBox("Error NOT reproduced - Expecting error:" & vbNewLine & InterestingError & vbNewLine & "EventLogs contained: " & tw.eventLogs)
        End If

    End Sub

    Sub createReport()

        Dim folderTime As String = cleanFileName(Now.ToString)
        Dim folder As String = Path.Combine(Application.StartupPath, "\FuzzFormsLogs\" & tw.exitCode.ToString & "\", folderTime)

        If Not Directory.Exists(folder) Then Directory.CreateDirectory(folder)

        Using sr As New StreamWriter(folder & "\Settings.xml")
            Dim xSer As New XmlSerializer(GetType(TestWindow))
            xSer.Serialize(sr, tw)

            If Not String.IsNullOrEmpty(tw.eventLogs) Then
                Using sr2 As New StreamWriter(folder & "\EventLog.txt")
                    sr2.WriteLine(tw.eventLogs & vbNewLine)
                End Using
            End If

            If tw.exitCode = TestWindowExitCode.ProcessExit OrElse tw.exitCode = TestWindowExitCode.StrayOffScreen OrElse Not String.IsNullOrEmpty(tw.eventLogs) Then
                Using sr3 = New StreamWriter(folder & "\ReplayLogging.txt")
                    sr3.WriteLine(tw.replayLog & vbNewLine)
                End Using
            End If
        End Using
    End Sub

    Function cleanFileName(ByVal fileName As String) As String
        Return Path.GetInvalidFileNameChars().Aggregate(fileName, Function(current, c) current.Replace(c.ToString(), String.Empty))
    End Function

    Private Sub btnStop_Click(sender As Object, e As EventArgs) Handles btnStop.Click
        t.Abort()

        MsgBox("Test stopped")
    End Sub

    Private Sub btnFindFile_Click(sender As Object, e As EventArgs) Handles btnFindFile.Click
        Dim ofd As New OpenFileDialog
        ofd.Multiselect = False
        ofd.Filter = "Application files (*.exe)|*.exe|All files (*.*)|*.*"
        ofd.ShowDialog()

        txtFileName.Text = ofd.FileName
    End Sub

    Private Sub btnReplay_Click(sender As Object, e As EventArgs) Handles btnReplay.Click
        Dim ofd As New OpenFileDialog
        ofd.Multiselect = False
        ofd.Filter = "Text (*.txt)|*.txt|All files (*.*)|*.*"

        Dim dr As DialogResult = ofd.ShowDialog()

        If dr = DialogResult.OK Then
            CheckReplay(ofd.FileName)
        End If
    End Sub

    Public Sub CheckReplay(ByVal filename As String)
        Dim replaypath As String = Path.GetDirectoryName(filename)
        Using sr As New StreamReader(replaypath & "\EventLog.txt")
            InterestingError = sr.ReadToEnd
        End Using

        Dim allTestSteps As String = String.Empty

        Using sr As New StreamReader(filename)
            allTestSteps = sr.ReadToEnd
            sr.Close()
        End Using

        Dim testSteps() As String = allTestSteps.Split(vbNewLine.ToCharArray, StringSplitOptions.RemoveEmptyEntries)

        tw.ResetTest()
        t = New Thread(Sub() tw.ReplayTest(testSteps))
        t.Start()

        AddHandler tw.ReplayComplete, AddressOf HandleReplayComplete
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim xSer As New XmlSerializer(GetType(TestWindow))

        Dim sfd As New SaveFileDialog
        sfd.Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*"
        Dim sfdRes As DialogResult = sfd.ShowDialog()

        If sfdRes = DialogResult.OK Then
            Using writer As New StreamWriter(sfd.FileName)
                xSer.Serialize(writer, New TestWindow)
            End Using
        End If
    End Sub

    Private Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        Dim xSer As New XmlSerializer(GetType(TestWindow))

        Dim ofd As New OpenFileDialog
        ofd.Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*"
        Dim ofdRes As DialogResult = ofd.ShowDialog()

        If ofdRes = DialogResult.OK Then
            Using reader As New StreamReader(ofd.FileName)
                tw = DirectCast(xSer.Deserialize(reader), TestWindow)
            End Using
        End If
    End Sub

    Private Sub btnReduce_Click(sender As Object, e As EventArgs) Handles btnReduce.Click

        Dim ofd As New OpenFileDialog
        ofd.Multiselect = False
        ofd.Filter = "Text (*.txt)|*.txt|All files (*.*)|*.*"

        Dim dr As DialogResult = ofd.ShowDialog()

        If dr = DialogResult.OK Then
            Dim l As New Lithium
            l.InitialiseReplay(ofd.FileName)
            t = New Thread(AddressOf l.Mimimize)
            t.Start()
        End If
    End Sub





End Class



