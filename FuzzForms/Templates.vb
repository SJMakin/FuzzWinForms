Public Class Templates

    Private Sub btnFindFile_Click(sender As Object, e As EventArgs) Handles btnFindFile.Click
        Dim ofd As New OpenFileDialog
        ofd.Multiselect = False
        ofd.Filter = "Application files (*.exe)|*.exe|All files (*.*)|*.*"
        ofd.ShowDialog()

        txtFileName.Text = ofd.FileName
    End Sub
End Class