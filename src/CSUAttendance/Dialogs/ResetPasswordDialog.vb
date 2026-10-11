Imports System.Windows.Forms

''' <summary>Admin types a temporary password for another admin account (forced change at next sign-in).</summary>
Public Class ResetPasswordDialog

    Private ReadOnly _userId As Integer
    Private ReadOnly _username As String

    Public Sub New()
        Me.New(0, "")
    End Sub

    Public Sub New(userId As Integer, username As String)
        InitializeComponent()
        _userId = userId
        _username = username
    End Sub

    Private Sub ResetPasswordDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Theme.StylePrimary(btnSave)
        Theme.StyleSecondary(btnCancel)
        btnSave.Text = "Reset"
        SetRail(Icons.Key, "Reset password", "The old password stops working right away. Tell the user the temporary password in person.")
        lblTitle.Text = $"Reset password · {_username}"
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        errProvider.Clear()
        Dim reason = ""
        If Not PasswordHasher.IsStrongEnough(txtPassword.Text, reason) Then
            errProvider.SetError(txtPassword, reason)
            Return
        End If
        If txtConfirm.Text <> txtPassword.Text Then
            errProvider.SetError(txtConfirm, "Passwords do not match.")
            Return
        End If
        Try
            AuthService.ResetPassword(_userId, txtPassword.Text)
            DialogResult = DialogResult.OK
        Catch ex As Exception
            Notice.Fail(Me, "Could not reset the password", "Please check your connection, then try again.", ex.Message)
        End Try
    End Sub

End Class
