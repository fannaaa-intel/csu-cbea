Imports System.Drawing
Imports System.Windows.Forms

''' <summary>Sign-in screen. Returns DialogResult.OK when Session.User is set.</summary>
Public Class LoginForm

    Private Const DbDownMessage As String = "Cannot connect to the database. Start MySQL in the XAMPP Control Panel, then try again."

    Private Sub LoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Theme.StylePrimary(btnLogin)
        btnLogin.Height = 42
        lblBrandIcon.Text = Icons.Classes
        lblBrandIcon.Region = New Region(Theme.RoundedRect(New Rectangle(0, 0, lblBrandIcon.Width, lblBrandIcon.Height), 12))

        Dim err As String = ""
        If Db.TestConnection(err) Then
            Try
                lblSchool.Text = AppSettings.SchoolName.ToUpper()
            Catch ex As Exception
                ' settings table missing: keep the default label
            End Try
        Else
            ShowError(DbDownMessage)
        End If
    End Sub

    Private Sub LoginForm_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        txtUsername.Focus()
    End Sub

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        lblError.Text = ""
        If txtUsername.Text.Trim() = "" Then
            ShowError("Enter your username.")
            txtUsername.Focus()
            Return
        End If
        If txtPassword.Text = "" Then
            ShowError("Enter your password.")
            txtPassword.Focus()
            Return
        End If

        Cursor = Cursors.WaitCursor
        btnLogin.Enabled = False
        Try
            Dim res = AuthService.Login(txtUsername.Text, txtPassword.Text)
            If Not res.Success Then
                ShowError(res.Message)
                txtPassword.SelectAllText()
                Return
            End If

            If res.MustChangePassword Then
                Using f As New ChangePasswordForm(True)
                    If f.ShowDialog(Me) <> DialogResult.OK Then
                        Session.Clear()
                        ShowError("You need to set a new password before continuing.")
                        Return
                    End If
                End Using
            End If

            AppSettings.Reload()
            DialogResult = DialogResult.OK
        Catch ex As Exception
            ShowError(DbDownMessage)
            Debug.WriteLine(ex.ToString())
        Finally
            Cursor = Cursors.Default
            btnLogin.Enabled = True
        End Try
    End Sub

    Private Sub chkShowPassword_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowPassword.CheckedChanged
        txtPassword.UseSystemPasswordChar = Not chkShowPassword.Checked
    End Sub

    Private Sub ShowError(message As String)
        lblError.Text = message
    End Sub

End Class
