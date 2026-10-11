Imports System.Configuration
Imports System.Windows.Forms

''' <summary>Entry point: Login → MainForm → (Log out) → Login again.</summary>
Public Module Program

    <STAThread>
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        AddHandler Application.ThreadException, AddressOf OnUnhandledException

        Dim cs = ConfigurationManager.ConnectionStrings("CsuAttendance")
        Db.ConnectionString = If(cs Is Nothing, "", cs.ConnectionString)

        Do
            Session.Clear()
            Using login As New LoginForm()
                If login.ShowDialog() <> DialogResult.OK Then Exit Do
            End Using

            Using main As New MainForm()
                Application.Run(main)
                If Not main.LoggedOut Then Exit Do
            End Using
        Loop
    End Sub

    ''' <summary>Last-resort handler so the app shows a message instead of crashing during a demo.</summary>
    Private Sub OnUnhandledException(sender As Object, e As System.Threading.ThreadExceptionEventArgs)
        Notice.Fail(Nothing, "Unexpected error", "The action could not be completed. Your saved data is safe.", e.Exception.Message)
    End Sub

End Module
