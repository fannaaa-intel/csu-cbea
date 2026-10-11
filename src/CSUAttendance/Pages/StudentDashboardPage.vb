Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Student home (Session.User.StudentId): greeting + term, a banner when any subject is at risk,
''' and one card per subject with the absence-hours bar. Click a card to see its dated history.
''' </summary>
Public Class StudentDashboardPage
    Inherits PageBase

    Private ReadOnly banner As New Banner()
    Private ReadOnly board As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 3, .BackColor = Theme.Background}
    Private ReadOnly emptyLabel As New Label With {.Dock = DockStyle.Fill, .TextAlign = ContentAlignment.MiddleCenter, .ForeColor = Theme.TextMuted,
                                                   .Visible = False, .Text = "You are not enrolled in any class this term yet."}
    Private greeting As Panel

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "My Attendance"
        End Get
    End Property

    Public Sub New()
        For i = 0 To 2
            board.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.33F))
        Next
        greeting = Ui.Greeting(Ui.FirstName(Session.User.FullName), "")
        Controls.Add(board)
        Controls.Add(emptyLabel)
        Controls.Add(Ui.Gap(DockStyle.Top, 8))
        Controls.Add(banner)
        Controls.Add(greeting)
    End Sub

    Public Overrides Sub OnPageShown()
        DirectCast(greeting.Tag, Label).Text = $"{Date.Today:dddd, MMMM d}  ·  {Lookups.ActiveTermLabel()}  ·  click a subject to see every class meeting"
        Dim dt = Db.Query(
            "SELECT v.*, CONCAT(t.first_name, ' ', t.last_name) AS teacher, c.days, c.start_time, c.end_time " &
            "FROM v_attendance_status v JOIN school_terms tm ON tm.term_id = v.term_id AND tm.is_active = 1 " &
            "JOIN classes c ON c.class_id = v.class_id JOIN teachers t ON t.teacher_id = c.teacher_id " &
            "WHERE v.student_id = @s ORDER BY v.subject_code", Db.P("@s", Session.User.StudentId))
        Dim tardy = AppSettings.TardyAlertCount

        board.SuspendLayout()
        board.Controls.Clear()
        board.RowStyles.Clear()
        Dim rows = Math.Max(1, CInt(Math.Ceiling(dt.Rows.Count / 3.0)))
        board.RowCount = rows + 1
        For i = 0 To rows - 1
            board.RowStyles.Add(New RowStyle(SizeType.Absolute, Theme.Dpi(192)))
        Next
        board.RowStyles.Add(New RowStyle(SizeType.Percent, 100))

        Dim worst = ""
        Dim worstSubject = ""
        For i = 0 To dt.Rows.Count - 1
            Dim r = dt.Rows(i)
            Dim status = Db.ToStr(r("attendance_status"))
            If status = "Good" AndAlso tardy > 0 AndAlso Db.ToInt(r("late_count")) >= tardy Then status = "Tardiness"
            If Rank(status) < Rank(worst) Then
                worst = status
                worstSubject = Db.ToStr(r("subject_code"))
            End If
            Dim ticket As New SubjectTicket With {
                .SubjectCode = Db.ToStr(r("subject_code")), .Description = Db.ToStr(r("subject_description")),
                .Detail = $"{r("teacher")}  ·  {Db.ToStr(r("days")).Replace(",", "/")} {Date.Today.Add(Db.ToTime(r("start_time"))):h:mm}–{Date.Today.Add(Db.ToTime(r("end_time"))):h:mm tt}",
                .Status = status, .HoursAbsent = Db.ToDec(r("hours_absent")), .AllowedHours = Db.ToDec(r("allowed_hours")),
                .PercentAbsent = Db.ToDec(r("percent_absent")),
                .Counts = $"Present {r("present_count")}  ·  Late {r("late_count")}  ·  Absent {r("absent_count")}  ·  Excused {r("excused_count")}",
                .Dock = DockStyle.Fill, .Margin = New Padding(If(i Mod 3 = 0, 0, Theme.Dpi(8)), 0, If(i Mod 3 = 2, 0, Theme.Dpi(8)), Theme.Dpi(16))
            }
            Dim code = ticket.SubjectCode
            AddHandler ticket.Click, Sub() Host.Navigate("history", code)
            board.Controls.Add(ticket, i Mod 3, i \ 3)
        Next
        board.ResumeLayout()
        emptyLabel.Visible = dt.Rows.Count = 0
        board.Visible = dt.Rows.Count > 0
        ShowStandingBanner(worst, worstSubject)
    End Sub

    ''' <summary>Lower = more serious ("" = nothing to report).</summary>
    Private Shared Function Rank(status As String) As Integer
        Dim i = Array.IndexOf({"Dropped", "Warning 2", "Warning 1", "Tardiness"}, status)
        Return If(i < 0, 99, i)
    End Function

    Private Sub ShowStandingBanner(worst As String, subject As String)
        Select Case worst
            Case "Dropped"
                banner.ShowMessage($"You have reached the absence limit in {subject} (20% of class hours) and are subject to dropping - see your instructor (CSU Student Manual s.II.C.2.1).", "Danger")
            Case "Warning 2"
                banner.ShowMessage($"Warning 2 in {subject}: your absences reached {AppSettings.Warning2Pct:0}% of class hours. At {AppSettings.DropPct:0}% you will be dropped (CSU Student Manual s.II.B.4).", "Warning")
            Case "Warning 1"
                banner.ShowMessage($"Warning 1 in {subject}: your absences reached {AppSettings.Warning1Pct:0}% of class hours. Attend regularly to avoid being dropped (CSU Student Manual s.II.B.4).", "Warning")
            Case "Tardiness"
                banner.ShowMessage($"You have been late {AppSettings.TardyAlertCount} or more times in {subject}. Frequent tardiness is referred to the guidance office.", "Warning")
            Case Else
                banner.ShowMessage("You are in good standing in all your subjects. Keep it up!", "Success")
        End Select
    End Sub

End Class
