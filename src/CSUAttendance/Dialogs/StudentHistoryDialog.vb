Imports System.Drawing
Imports System.Windows.Forms

''' <summary>Read-only view of one student's dated attendance in one class, with the absence-hours bar.</summary>
Public Class StudentHistoryDialog
    Inherits SheetForm

    Private ReadOnly _classId As Integer
    Private ReadOnly _studentId As Integer

    Public Sub New()
        Me.New(0, 0)
    End Sub

    Public Sub New(classId As Integer, studentId As Integer)
        _classId = classId
        _studentId = studentId
        FormBorderStyle = FormBorderStyle.None
        StartPosition = FormStartPosition.CenterParent
        ShowInTaskbar = False
        BackColor = Theme.Surface
        Font = Theme.FontBase
        ClientSize = New Size(Theme.Dpi(940), Theme.Dpi(600))
        Text = "Attendance history"
    End Sub

    Private Sub StudentHistoryDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim left = Theme.Dpi(RailWidth + 28)
        Dim w = ClientSize.Width - left - Theme.Dpi(28)
        Dim s = Db.Query("SELECT * FROM v_attendance_status WHERE class_id = @c AND student_id = @s", Db.P("@c", _classId), Db.P("@s", _studentId))
        If s.Rows.Count = 0 Then Return
        Dim r = s.Rows(0)
        SetRail(Icons.Records, "Attendance history", $"{r("student_no")}{vbCrLf}{r("subject_code")} · {r("section_name")}{vbCrLf}{vbCrLf}Hours are counted per meeting. Excused hours count until the requirement is complied.")

        Dim title = Ui.Caption(Db.ToStr(r("student_name")), Theme.FontHeading)
        title.Location = New Point(left, Theme.Dpi(24))
        Dim subtitle = Ui.Muted($"{r("subject_code")} - {r("subject_description")}  ·  {r("section_name")}")
        subtitle.Location = New Point(left, Theme.Dpi(60))
        Dim badge As New StatusBadge With {.Text = Db.ToStr(r("attendance_status"))}
        badge.Location = New Point(left + w - badge.Width - Theme.Dpi(36), Theme.Dpi(30))

        Dim pct = Db.ToDec(r("percent_absent"))
        Dim bar As New HoursBar With {.PercentAbsent = pct, .Warning1Pct = AppSettings.Warning1Pct, .Warning2Pct = AppSettings.Warning2Pct, .DropPct = AppSettings.DropPct}
        bar.SetBounds(left, Theme.Dpi(96), w, Theme.Dpi(12))
        Dim barText = Ui.Caption($"{Db.ToDec(r("hours_absent")):0.0#} of {Db.ToDec(r("allowed_hours")):0.0#} allowed absence hours used  ({pct:0.0}% of {Db.ToDec(r("total_hours")):0.#} class hours)", Theme.FontBold)
        barText.Location = New Point(left, Theme.Dpi(116))

        Dim chips As New CountChips()
        chips.SetItems({New KeyValuePair(Of String, String)("Present", Db.ToStr(r("present_count"))),
                        New KeyValuePair(Of String, String)("Late", Db.ToStr(r("late_count"))),
                        New KeyValuePair(Of String, String)("Absent", Db.ToStr(r("absent_count"))),
                        New KeyValuePair(Of String, String)("Excused", Db.ToStr(r("excused_count")))})
        chips.Location = New Point(left, Theme.Dpi(148))

        Dim grid = Ui.Grid()
        grid.Dock = DockStyle.None
        grid.SetBounds(left, Theme.Dpi(196), w, ClientSize.Height - Theme.Dpi(196) - Theme.Dpi(80))
        grid.DataSource = Db.Query(
            "SELECT s.session_date, DATE_FORMAT(s.session_date, '%a') AS day_name, ar.status, ar.time_in, ar.hours_absent, " &
            "       CONCAT_WS(' · ', NULLIF(ar.remarks, ''), NULLIF(CONCAT(ar.excuse_requirement, IF(ar.excuse_status IS NULL, '', CONCAT(' (', ar.excuse_status, ')'))), '')) AS notes " &
            "FROM attendance_records ar JOIN attendance_sessions s ON s.session_id = ar.session_id " &
            "WHERE s.class_id = @c AND ar.student_id = @s ORDER BY s.session_date DESC", Db.P("@c", _classId), Db.P("@s", _studentId))
        Ui.Col(grid, "session_date", "Date", 16, "ddd, MMM d")
        Ui.Col(grid, "day_name", "Day", 8)
        Ui.Col(grid, "status", "Status", 14)
        Ui.Col(grid, "time_in", "Time in", 12)
        Ui.Col(grid, "hours_absent", "Hours", 10, "0.0#")
        Ui.Col(grid, "notes", "Remarks / requirement", 40)
        Ui.Hide(grid, "day_name")
        AddHandler grid.CellPainting, Sub(sender2, ev) If ev.ColumnIndex >= 0 AndAlso grid.Columns(ev.ColumnIndex).Name = "status" Then Theme.PaintBadgeCell(ev)
        AddHandler grid.CellFormatting, Sub(sender2, ev)
                                            If ev.ColumnIndex >= 0 AndAlso grid.Columns(ev.ColumnIndex).Name = "time_in" AndAlso TypeOf ev.Value Is TimeSpan Then
                                                ev.Value = Date.Today.Add(DirectCast(ev.Value, TimeSpan)).ToString("h:mm tt")
                                                ev.FormattingApplied = True
                                            End If
                                        End Sub

        Dim btnClose = Ui.Action("Close", "secondary", Nothing, 110)
        btnClose.Location = New Point(ClientSize.Width - btnClose.Width - Theme.Dpi(28), ClientSize.Height - btnClose.Height - Theme.Dpi(24))
        btnClose.DialogResult = DialogResult.Cancel
        CancelButton = btnClose

        Controls.AddRange({title, subtitle, badge, bar, barText, chips, grid, btnClose})
    End Sub

End Class
