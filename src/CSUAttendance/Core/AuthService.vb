Public Class LoginResult
    Public Property Success As Boolean
    Public Property Message As String = ""
    Public Property MustChangePassword As Boolean
End Class

''' <summary>Login, password change and account creation helpers.</summary>
Public Module AuthService

    Public Function Login(username As String, password As String) As LoginResult
        Dim res As New LoginResult()
        Dim dt = Db.Query("SELECT u.user_id, u.username, u.password_hash, u.role, u.full_name, u.is_active, u.must_change_password, " &
                          "       t.teacher_id, s.student_id " &
                          "FROM users u " &
                          "LEFT JOIN teachers t ON t.user_id = u.user_id " &
                          "LEFT JOIN students s ON s.user_id = u.user_id " &
                          "WHERE u.username = @u LIMIT 1",
                          Db.P("@u", username.Trim()))

        If dt.Rows.Count = 0 OrElse Not PasswordHasher.VerifyPassword(password, Db.ToStr(dt.Rows(0)("password_hash"))) Then
            res.Message = "Incorrect username or password."
            Return res
        End If

        Dim r = dt.Rows(0)
        If Not Db.ToBool(r("is_active")) Then
            res.Message = "This account is deactivated. Please contact the administrator."
            Return res
        End If

        Session.User = New CurrentUser With {
            .UserId = Db.ToInt(r("user_id")),
            .Username = Db.ToStr(r("username")),
            .FullName = Db.ToStr(r("full_name")),
            .Role = Db.ToStr(r("role")),
            .TeacherId = Db.ToInt(r("teacher_id")),
            .StudentId = Db.ToInt(r("student_id"))
        }

        Db.Execute("UPDATE users SET last_login = NOW() WHERE user_id = @id", Db.P("@id", Session.UserId))
        Audit.Log("LOGIN", "User", Session.UserId)

        res.Success = True
        res.MustChangePassword = Db.ToBool(r("must_change_password"))
        Return res
    End Function

    Public Function ChangePassword(userId As Integer, currentPassword As String, newPassword As String, ByRef message As String) As Boolean
        Dim stored = Db.ToStr(Db.Scalar("SELECT password_hash FROM users WHERE user_id = @id", Db.P("@id", userId)))
        If Not PasswordHasher.VerifyPassword(currentPassword, stored) Then
            message = "Current password is incorrect."
            Return False
        End If
        If Not PasswordHasher.IsStrongEnough(newPassword, message) Then Return False
        If currentPassword = newPassword Then
            message = "New password must be different from the current password."
            Return False
        End If
        Db.Execute("UPDATE users SET password_hash = @h, must_change_password = 0 WHERE user_id = @id",
                   Db.P("@h", PasswordHasher.HashPassword(newPassword)), Db.P("@id", userId))
        Audit.Log("CHANGE_PASSWORD", "User", userId)
        message = "Password updated."
        Return True
    End Function

    ''' <summary>Admin reset: sets a new password and forces a change at next login.</summary>
    Public Sub ResetPassword(userId As Integer, newPassword As String)
        Db.Execute("UPDATE users SET password_hash = @h, must_change_password = 1 WHERE user_id = @id",
                   Db.P("@h", PasswordHasher.HashPassword(newPassword)), Db.P("@id", userId))
        Audit.Log("RESET_PASSWORD", "User", userId)
    End Sub

    Public Function UsernameExists(username As String, Optional exceptUserId As Integer = 0) As Boolean
        Return Db.ToInt(Db.Scalar("SELECT COUNT(*) FROM users WHERE username = @u AND user_id <> @id",
                                  Db.P("@u", username.Trim()), Db.P("@id", exceptUserId))) > 0
    End Function

End Module
