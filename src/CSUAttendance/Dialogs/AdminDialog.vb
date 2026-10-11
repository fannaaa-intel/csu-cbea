Imports System.Windows.Forms

''' <summary>Add an Admin account (id = 0) or edit an admin's username / full name (id > 0, no password fields).</summary>
Public Class AdminDialog

    Private ReadOnly _id As Integer

    Public Sub New()
        Me.New(0)
    End Sub

    Public Sub New(userId As Integer)
        InitializeComponent()
        _id = userId
    End Sub

    Private Sub AdminDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Theme.StylePrimary(btnSave)
        Theme.StyleSecondary(btnCancel)
        SetRail(Icons.Users, "Administrator", "Admins manage every record, the settings and the audit log. Keep the number of admin accounts small.")
        If _id = 0 Then Return

        lblTitle.Text = "Edit admin"
        For Each c As Control In New Control() {lblPassword, txtPassword, lblConfirm, txtConfirm}
            c.Visible = False
        Next
        lblInfo.Text = "To change this admin's password, use Reset password on the User Accounts page."
        lblInfo.Top = lblPassword.Top
        Dim dt = Db.Query("SELECT username, full_name FROM users WHERE user_id = @id AND role = 'Admin'", Db.P("@id", _id))
        If dt.Rows.Count = 0 Then
            DialogResult = DialogResult.Cancel
            Return
        End If
        txtUsername.Text = Db.ToStr(dt.Rows(0)("username"))
        txtFullName.Text = Db.ToStr(dt.Rows(0)("full_name"))
    End Sub

    Private Function ValidateInputs() As Boolean
        errProvider.Clear()
        Dim ok = True
        Dim username = txtUsername.Text.Trim()
        If username = "" OrElse username.Contains(" ") Then
            errProvider.SetError(txtUsername, "Username is required and cannot contain spaces.")
            ok = False
        ElseIf AuthService.UsernameExists(username, _id) Then
            errProvider.SetError(txtUsername, $"Username {username} is already taken.")
            ok = False
        End If
        If txtFullName.Text.Trim() = "" Then
            errProvider.SetError(txtFullName, "Full name is required.")
            ok = False
        End If
        If _id = 0 Then
            Dim reason = ""
            If Not PasswordHasher.IsStrongEnough(txtPassword.Text, reason) Then
                errProvider.SetError(txtPassword, reason)
                ok = False
            ElseIf txtConfirm.Text <> txtPassword.Text Then
                errProvider.SetError(txtConfirm, "Passwords do not match.")
                ok = False
            End If
        End If
        Return ok
    End Function

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Not ValidateInputs() Then Return
        Dim username = txtUsername.Text.Trim()
        Try
            If _id = 0 Then
                Dim newId = Db.Insert("INSERT INTO users (username, password_hash, role, full_name, must_change_password) VALUES (@u, @h, 'Admin', @n, 1)",
                                      Db.P("@u", username), Db.P("@h", PasswordHasher.HashPassword(txtPassword.Text)), Db.P("@n", txtFullName.Text.Trim()))
                Audit.Log("CREATE", "User", newId, $"Admin {username}")
            Else
                Db.Execute("UPDATE users SET username = @u, full_name = @n WHERE user_id = @id",
                           Db.P("@u", username), Db.P("@n", txtFullName.Text.Trim()), Db.P("@id", _id))
                Audit.Log("UPDATE", "User", _id, $"Admin {username}")
            End If
            DialogResult = DialogResult.OK
        Catch ex As Exception When Db.IsDuplicate(ex)
            errProvider.SetError(txtUsername, $"Username {username} is already taken.")
        Catch ex As Exception
            Notice.Fail(Me, "Could not save the account", "Please check your connection and the details, then try again.", ex.Message)
        End Try
    End Sub

End Class
