Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Admin settings: one card per attendance rule (value + description + CSU Student Manual basis),
''' the school name, Save, and "Re-check all warnings" (runs EvaluateClass for every class of the active term).
''' </summary>
Public Class SettingsPage
    Inherits PageBase

    ''' <summary>key, title, citation, min, max, unit</summary>
    Private Shared ReadOnly Rules As (Key As String, Title As String, Basis As String, Min As Integer, Max As Integer, Unit As String)() = {
        ("warning1_pct", "Warning 1", "CSU Student Manual s.II.B.4 (1st warning)", 1, 100, "% of class hours"),
        ("warning2_pct", "Warning 2", "CSU Student Manual s.II.B.4 (2nd warning)", 1, 100, "% of class hours"),
        ("drop_pct", "Drop limit", "CSU Student Manual s.II.C.2.1 - dropped at 20%", 1, 100, "% of class hours"),
        ("late_minutes", "Late after", "School default (the manual sets no rule)", 1, 120, "minutes"),
        ("tardy_alert_count", "Tardiness alert", "Guidance referral, CSU Student Manual s.III.B.6.5", 0, 50, "lates (0 = off)"),
        ("lates_per_absence", "Lates per absence", "Off by default - the manual has no conversion rule", 0, 20, "lates (0 = off)"),
        ("edit_window_days", "Teacher edit window", "Admin can always edit; every edit is audited", 0, 60, "days")}

    Private ReadOnly _inputs As New Dictionary(Of String, NumericUpDown)
    Private ReadOnly txtSchool As TextField = Ui.Search("School name", 420)
    Private ReadOnly btnSave As Button = Ui.Action("Save settings", "primary", Icons.Save, 140)
    Private ReadOnly btnRecheck As Button = Ui.Action("Re-check all warnings", "secondary", Icons.Refresh, 190)
    Private ReadOnly board As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 3, .RowCount = 3, .BackColor = Theme.Background}

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Settings"
        End Get
    End Property

    Public Sub New()
        For i = 0 To 2
            board.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.33F))
            board.RowStyles.Add(New RowStyle(SizeType.Absolute, Theme.Dpi(196)))   ' cards sized to their content
        Next
        For i = 0 To Rules.Length - 1
            board.Controls.Add(RuleCard(i), i Mod 3, i \ 3)
        Next
        board.Controls.Add(SchoolCard(), 1, 2)
        board.SetColumnSpan(board.GetControlFromPosition(1, 2), 2)

        Dim bar = Ui.Toolbar()
        Dim note = Ui.Muted("Changes apply right away to every attendance status in the system.")
        note.Margin = New Padding(0, Theme.Dpi(12), 0, 0)
        Dim leftRow = Ui.Row()
        leftRow.Controls.Add(note)
        Dim actions = Ui.Row(DockStyle.Right)
        actions.Controls.AddRange({btnSave, btnRecheck})
        bar.Controls.Add(leftRow)
        bar.Controls.Add(actions)
        Controls.Add(board)
        Controls.Add(bar)

        AddHandler btnSave.Click, Sub() SaveSettings()
        AddHandler btnRecheck.Click, Sub() RecheckWarnings()
    End Sub

    Private Function RuleCard(i As Integer) As CardPanel
        Dim rule = Rules(i)
        Dim c = Ui.Card(DockStyle.Fill, 18)
        c.Margin = New Padding(If(i Mod 3 = 0, 0, Theme.Dpi(8)), If(i < 3, 0, Theme.Dpi(8)), If(i Mod 3 = 2, 0, Theme.Dpi(8)), Theme.Dpi(8))
        Dim title = Ui.Caption(rule.Title, Theme.FontSubtitle)
        title.Location = New Point(c.Padding.Left, c.Padding.Top)
        Dim num As New NumericUpDown With {.Minimum = rule.Min, .Maximum = rule.Max, .Font = Theme.FontHeading, .Width = Theme.Dpi(96),
                                           .Location = New Point(c.Padding.Left, c.Padding.Top + Theme.Dpi(32)), .TextAlign = HorizontalAlignment.Center}
        Dim unit = Ui.Muted(rule.Unit)
        unit.Location = New Point(num.Right + Theme.Dpi(10), num.Top + Theme.Dpi(8))
        Dim desc As New Label With {.Name = "desc", .Font = Theme.FontSmall, .ForeColor = Theme.TextPrimary, .AutoSize = False, .BackColor = Theme.Surface,
                                    .Location = New Point(c.Padding.Left, num.Bottom + Theme.Dpi(12)), .Height = Theme.Dpi(40),
                                    .Anchor = AnchorStyles.Left Or AnchorStyles.Top Or AnchorStyles.Right}
        Dim basis = Ui.Muted(rule.Basis)
        basis.AutoSize = False
        basis.Anchor = AnchorStyles.Left Or AnchorStyles.Top Or AnchorStyles.Right
        basis.Location = New Point(c.Padding.Left, desc.Bottom + Theme.Dpi(6))
        basis.Height = Theme.Dpi(20)
        AddHandler c.Layout, Sub()
                                 desc.Width = c.ClientSize.Width - c.Padding.Horizontal
                                 basis.Width = desc.Width
                             End Sub
        c.Controls.AddRange({title, num, unit, desc, basis})
        _inputs(rule.Key) = num
        Return c
    End Function

    Private Function SchoolCard() As CardPanel
        Dim c = Ui.Card(DockStyle.Fill, 18)
        c.Margin = New Padding(Theme.Dpi(8), Theme.Dpi(8), 0, Theme.Dpi(8))
        Dim title = Ui.Caption("School name", Theme.FontSubtitle)
        title.Location = New Point(c.Padding.Left, c.Padding.Top)
        txtSchool.Location = New Point(c.Padding.Left, c.Padding.Top + Theme.Dpi(34))
        txtSchool.MaxLength = 100
        Dim hint = Ui.Muted("Shown on the sign-in screen and at the top of every printed report.")
        hint.Location = New Point(c.Padding.Left, txtSchool.Bottom + Theme.Dpi(10))
        c.Controls.AddRange({title, txtSchool, hint})
        Return c
    End Function

    Public Overrides Sub OnPageShown()
        Dim dt = Db.Query("SELECT setting_key, setting_value, description FROM settings")
        For Each r As DataRow In dt.Rows
            Dim key = Db.ToStr(r("setting_key"))
            If key = "school_name" Then
                txtSchool.Text = Db.ToStr(r("setting_value"))
            ElseIf _inputs.ContainsKey(key) Then
                Dim num = _inputs(key)
                Dim v As Decimal
                If Decimal.TryParse(Db.ToStr(r("setting_value")), v) Then num.Value = Math.Max(num.Minimum, Math.Min(num.Maximum, v))
                num.Parent.Controls("desc").Text = Db.ToStr(r("description"))
            End If
        Next
    End Sub

    Private Sub SaveSettings()
        Dim w1 = _inputs("warning1_pct").Value, w2 = _inputs("warning2_pct").Value, drop = _inputs("drop_pct").Value
        If Not (w1 < w2 AndAlso w2 < drop) Then
            Notice.Warn(Me, "Check the thresholds", "Warning 1 must be lower than Warning 2, and Warning 2 lower than the drop limit.")
            Return
        End If
        If txtSchool.Text.Trim() = "" Then
            Notice.Warn(Me, "School name is required", "Type the school name shown on the sign-in screen and reports.")
            Return
        End If
        Try
            Dim changes As New List(Of String)
            For Each kv In _inputs
                Dim newValue = CInt(kv.Value.Value).ToString()
                If AppSettings.GetString(kv.Key) <> newValue Then changes.Add($"{kv.Key}={newValue}")
                AppSettings.SetValue(kv.Key, newValue)
            Next
            If AppSettings.SchoolName <> txtSchool.Text.Trim() Then changes.Add("school_name")
            AppSettings.SetValue("school_name", txtSchool.Text.Trim())
            Audit.Log("UPDATE", "Settings", 0, If(changes.Count = 0, "no changes", String.Join(", ", changes)))
            ShowToast(If(changes.Count = 0, "No changes to save", "Settings saved"))
        Catch ex As Exception
            ShowError(ex, "save the settings")
        End Try
    End Sub

    ''' <summary>Issues any warnings students have reached (e.g. after lowering a threshold). Never duplicates.</summary>
    Private Sub RecheckWarnings()
        If Not Confirm("Check every class in the active term and issue any warnings students have newly reached? Notifications are sent for new warnings only.",
                       "Re-check warnings", "Re-check now") Then Return
        Try
            Cursor = Cursors.WaitCursor
            Dim issued = 0
            For Each r As DataRow In Db.Query("SELECT class_id FROM classes WHERE term_id = @t", Db.P("@t", Lookups.ActiveTermId())).Rows
                issued += AttendanceRules.EvaluateClass(Db.ToInt(r("class_id"))).Count
            Next
            Audit.Log("RECHECK", "Warnings", 0, $"{issued} new warning(s)")
            Host.RefreshBell()
            ShowToast(If(issued = 0, "All warnings are up to date", $"{Ui.Plural(issued, "new warning")} issued"), If(issued = 0, "Success", "Warning"))
        Catch ex As Exception
            ShowError(ex, "re-check the warnings")
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

End Class
