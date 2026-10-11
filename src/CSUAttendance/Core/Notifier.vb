''' <summary>In-app notifications (bell icon). Types: Info | Success | Warning | Danger</summary>
Public Module Notifier

    Public Sub Send(userId As Integer, title As String, message As String, Optional type As String = "Info")
        If userId = 0 Then Return
        Db.Execute("INSERT INTO notifications (user_id, title, message, type) VALUES (@u, @t, @m, @ty)",
                   Db.P("@u", userId), Db.P("@t", title), Db.P("@m", message), Db.P("@ty", type))
    End Sub

    Public Sub SendToAdmins(title As String, message As String, Optional type As String = "Info")
        Db.Execute("INSERT INTO notifications (user_id, title, message, type) " &
                   "SELECT user_id, @t, @m, @ty FROM users WHERE role = 'Admin' AND is_active = 1",
                   Db.P("@t", title), Db.P("@m", message), Db.P("@ty", type))
    End Sub

    Public Function UnreadCount(userId As Integer) As Integer
        Return Db.ToInt(Db.Scalar("SELECT COUNT(*) FROM notifications WHERE user_id = @u AND is_read = 0", Db.P("@u", userId)))
    End Function

    Public Function GetForUser(userId As Integer, Optional limit As Integer = 100) As DataTable
        Return Db.Query("SELECT notification_id, title, message, type, is_read, created_at FROM notifications " &
                        "WHERE user_id = @u ORDER BY created_at DESC, notification_id DESC LIMIT @lim",
                        Db.P("@u", userId), Db.P("@lim", limit))
    End Function

    Public Sub MarkAllRead(userId As Integer)
        Db.Execute("UPDATE notifications SET is_read = 1 WHERE user_id = @u AND is_read = 0", Db.P("@u", userId))
    End Sub

End Module
