<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainWindow
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainWindow))
        Me.dtnStart = New System.Windows.Forms.Button()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnFindFile = New System.Windows.Forms.Button()
        Me.txtFileName = New System.Windows.Forms.TextBox()
        Me.chkKeepGoing = New System.Windows.Forms.CheckBox()
        Me.btnReplay = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnLoad = New System.Windows.Forms.Button()
        Me.btnReduce = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'dtnStart
        '
        Me.dtnStart.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.dtnStart.Location = New System.Drawing.Point(9, 58)
        Me.dtnStart.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.dtnStart.Name = "dtnStart"
        Me.dtnStart.Size = New System.Drawing.Size(65, 47)
        Me.dtnStart.TabIndex = 1
        Me.dtnStart.Text = "Start Test"
        Me.dtnStart.UseVisualStyleBackColor = True
        '
        'btnStop
        '
        Me.btnStop.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnStop.Location = New System.Drawing.Point(78, 59)
        Me.btnStop.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(65, 47)
        Me.btnStop.TabIndex = 2
        Me.btnStop.Text = "Stop Test"
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'btnFindFile
        '
        Me.btnFindFile.Location = New System.Drawing.Point(448, 9)
        Me.btnFindFile.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnFindFile.Name = "btnFindFile"
        Me.btnFindFile.Size = New System.Drawing.Size(22, 18)
        Me.btnFindFile.TabIndex = 3
        Me.btnFindFile.Text = "..."
        Me.btnFindFile.UseVisualStyleBackColor = True
        '
        'txtFileName
        '
        Me.txtFileName.Location = New System.Drawing.Point(9, 10)
        Me.txtFileName.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtFileName.Name = "txtFileName"
        Me.txtFileName.Size = New System.Drawing.Size(434, 20)
        Me.txtFileName.TabIndex = 4
        '
        'chkKeepGoing
        '
        Me.chkKeepGoing.AutoSize = True
        Me.chkKeepGoing.Location = New System.Drawing.Point(9, 32)
        Me.chkKeepGoing.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.chkKeepGoing.Name = "chkKeepGoing"
        Me.chkKeepGoing.Size = New System.Drawing.Size(80, 17)
        Me.chkKeepGoing.TabIndex = 6
        Me.chkKeepGoing.Text = "Keep going"
        Me.chkKeepGoing.UseVisualStyleBackColor = True
        '
        'btnReplay
        '
        Me.btnReplay.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnReplay.Location = New System.Drawing.Point(165, 59)
        Me.btnReplay.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnReplay.Name = "btnReplay"
        Me.btnReplay.Size = New System.Drawing.Size(65, 47)
        Me.btnReplay.TabIndex = 7
        Me.btnReplay.Text = "Replay"
        Me.btnReplay.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSave.Location = New System.Drawing.Point(323, 58)
        Me.btnSave.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(120, 24)
        Me.btnSave.TabIndex = 8
        Me.btnSave.Text = "Save Settings"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnLoad
        '
        Me.btnLoad.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnLoad.Location = New System.Drawing.Point(323, 82)
        Me.btnLoad.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnLoad.Name = "btnLoad"
        Me.btnLoad.Size = New System.Drawing.Size(120, 24)
        Me.btnLoad.TabIndex = 9
        Me.btnLoad.Text = "Load Settings"
        Me.btnLoad.UseVisualStyleBackColor = True
        '
        'btnReduce
        '
        Me.btnReduce.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnReduce.Location = New System.Drawing.Point(234, 59)
        Me.btnReduce.Margin = New System.Windows.Forms.Padding(2)
        Me.btnReduce.Name = "btnReduce"
        Me.btnReduce.Size = New System.Drawing.Size(65, 47)
        Me.btnReduce.TabIndex = 10
        Me.btnReduce.Text = "Reduce"
        Me.btnReduce.UseVisualStyleBackColor = True
        '
        'MainWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(479, 115)
        Me.Controls.Add(Me.btnReduce)
        Me.Controls.Add(Me.btnLoad)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.btnReplay)
        Me.Controls.Add(Me.chkKeepGoing)
        Me.Controls.Add(Me.txtFileName)
        Me.Controls.Add(Me.btnFindFile)
        Me.Controls.Add(Me.btnStop)
        Me.Controls.Add(Me.dtnStart)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Name = "MainWindow"
        Me.Text = "FuzzForms"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents dtnStart As System.Windows.Forms.Button
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents btnFindFile As System.Windows.Forms.Button
    Friend WithEvents txtFileName As System.Windows.Forms.TextBox
    Friend WithEvents chkKeepGoing As System.Windows.Forms.CheckBox
    Friend WithEvents btnReplay As System.Windows.Forms.Button
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnLoad As System.Windows.Forms.Button
    Friend WithEvents btnReduce As Button
End Class
