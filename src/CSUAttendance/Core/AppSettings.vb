''' <summary>
''' Reads/writes the `settings` table (cached). Keys:
''' school_name, warning1_pct, warning2_pct, drop_pct, late_minutes,
''' tardy_alert_count, lates_per_absence, edit_window_days
''' </summary>
Public Module AppSettings

    Private _cache As Dictionary(Of String, String)

    Public Sub Reload()
        _cache = New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        For Each r As DataRow In Db.Query("SELECT setting_key, setting_value FROM settings").Rows
            _cache(Db.ToStr(r("setting_key"))) = Db.ToStr(r("setting_value"))
        Next
    End Sub

    Public Function GetString(key As String, Optional defaultValue As String = "") As String
        If _cache Is Nothing Then Reload()
        Dim v As String = Nothing
        Return If(_cache.TryGetValue(key, v), v, defaultValue)
    End Function

    Public Function GetDecimal(key As String, defaultValue As Decimal) As Decimal
        Dim d As Decimal
        Return If(Decimal.TryParse(GetString(key), d), d, defaultValue)
    End Function

    Public Function GetInt(key As String, defaultValue As Integer) As Integer
        Dim i As Integer
        Return If(Integer.TryParse(GetString(key), i), i, defaultValue)
    End Function

    Public Sub SetValue(key As String, value As String)
        Db.Execute("UPDATE settings SET setting_value = @v WHERE setting_key = @k", Db.P("@v", value), Db.P("@k", key))
        Reload()
    End Sub

    ' Convenience accessors used by the rules engine
    Public ReadOnly Property SchoolName As String
        Get
            Return GetString("school_name", "Cagayan State University")
        End Get
    End Property
    Public ReadOnly Property Warning1Pct As Decimal
        Get
            Return GetDecimal("warning1_pct", 10D)
        End Get
    End Property
    Public ReadOnly Property Warning2Pct As Decimal
        Get
            Return GetDecimal("warning2_pct", 15D)
        End Get
    End Property
    Public ReadOnly Property DropPct As Decimal
        Get
            Return GetDecimal("drop_pct", 20D)
        End Get
    End Property
    Public ReadOnly Property LateMinutes As Integer
        Get
            Return GetInt("late_minutes", 15)
        End Get
    End Property
    Public ReadOnly Property TardyAlertCount As Integer
        Get
            Return GetInt("tardy_alert_count", 3)
        End Get
    End Property
    Public ReadOnly Property EditWindowDays As Integer
        Get
            Return GetInt("edit_window_days", 3)
        End Get
    End Property

End Module
