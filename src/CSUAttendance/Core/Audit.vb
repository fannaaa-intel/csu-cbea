''' <summary>Writes to audit_log. Never throws — auditing must not break the main action.</summary>
Public Module Audit

    Public Sub Log(action As String, entity As String, Optional entityId As Integer = 0, Optional details As String = Nothing)
        Try
            Db.Execute("INSERT INTO audit_log (user_id, action, entity, entity_id, details) VALUES (@u, @a, @e, @id, @d)",
                       Db.P("@u", If(Session.UserId = 0, CObj(Nothing), Session.UserId)),
                       Db.P("@a", action),
                       Db.P("@e", entity),
                       Db.P("@id", If(entityId = 0, CObj(Nothing), entityId)),
                       Db.P("@d", details))
        Catch ex As Exception
            Debug.WriteLine("Audit failed: " & ex.Message)
        End Try
    End Sub

End Module
