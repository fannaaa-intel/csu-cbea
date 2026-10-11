''' <summary>A warning that was newly issued by EvaluateClass.</summary>
Public Class IssuedWarning
    Public Property StudentId As Integer
    Public Property StudentName As String = ""
    Public Property Level As String = ""        ' Warning 1 | Warning 2 | Dropped | Tardiness
    Public Property Percent As Decimal
End Class

''' <summary>
''' CSU attendance rules (Student Manual, Revised 2019):
'''   s.II.B.4   absences allowed up to 20% of total class hours; dropped after two (2) warnings
'''   s.II.C.2.1 dropped when accumulated absence reaches at least 20% of total class hours
''' The percentage/status math lives in the database view v_attendance_status (single source of truth).
''' This module computes hours, decides Present/Late, and issues warnings + notifications.
''' </summary>
Public Module AttendanceRules

    Public ReadOnly Statuses As String() = {"Present", "Late", "Absent", "Excused"}

    ''' <summary>Length of one class meeting in hours (e.g. 08:00–09:30 = 1.5).</summary>
    Public Function SessionHours(startTime As TimeSpan, endTime As TimeSpan) As Decimal
        Return Math.Round(CDec((endTime - startTime).TotalHours), 2)
    End Function

    ''' <summary>
    ''' Total class hours in the semester = number of meeting days between term start and end × hours per meeting.
    ''' days uses the classes.days SET format, e.g. "Mon,Wed".
    ''' </summary>
    Public Function ComputeTotalHours(termStart As Date, termEnd As Date, days As String, startTime As TimeSpan, endTime As TimeSpan) As Decimal
        Dim meetDays = days.Split(","c).Select(Function(d) d.Trim()).ToList()
        Dim count = 0
        Dim d0 = termStart.Date
        While d0 <= termEnd.Date
            If meetDays.Contains(d0.ToString("ddd", Globalization.CultureInfo.InvariantCulture)) Then count += 1
            d0 = d0.AddDays(1)
        End While
        Return count * SessionHours(startTime, endTime)
    End Function

    ''' <summary>True if the class meets on the given date.</summary>
    Public Function MeetsOn(days As String, day As Date) As Boolean
        Dim name = day.ToString("ddd", Globalization.CultureInfo.InvariantCulture)
        Return days.Split(","c).Any(Function(d) d.Trim() = name)
    End Function

    ''' <summary>Present or Late based on the time the student arrived (late_minutes setting).</summary>
    Public Function StatusFromTimeIn(classStart As TimeSpan, timeIn As TimeSpan) As String
        Return If(timeIn > classStart.Add(TimeSpan.FromMinutes(AppSettings.LateMinutes)), "Late", "Present")
    End Function

    ''' <summary>hours_absent value to store for a record.</summary>
    Public Function HoursAbsentFor(status As String, sessionHrs As Decimal) As Decimal
        Return If(status = "Absent" OrElse status = "Excused", sessionHrs, 0D)
    End Function

    ''' <summary>Teachers may edit a saved session for edit_window_days; Admin always can.</summary>
    Public Function CanEditSession(sessionDate As Date) As Boolean
        If Session.IsAdmin Then Return True
        Return (Date.Today - sessionDate.Date).TotalDays <= AppSettings.EditWindowDays
    End Function

    ''' <summary>Current status rows for one class (from v_attendance_status).</summary>
    Public Function GetClassSummary(classId As Integer) As DataTable
        Return Db.Query("SELECT * FROM v_attendance_status WHERE class_id = @c ORDER BY student_name", Db.P("@c", classId))
    End Function

    ''' <summary>Current status rows for one student in the active term.</summary>
    Public Function GetStudentSummary(studentId As Integer) As DataTable
        Return Db.Query("SELECT v.* FROM v_attendance_status v JOIN school_terms t ON t.term_id = v.term_id AND t.is_active = 1 " &
                        "WHERE v.student_id = @s ORDER BY v.subject_code", Db.P("@s", studentId))
    End Function

    ''' <summary>
    ''' Call after saving attendance for a class. Issues any warnings the students have newly reached
    ''' (Warning 1 → Warning 2 → Dropped, plus Tardiness) and notifies student, teacher and (for Dropped) admins.
    ''' Warnings already issued are never duplicated (unique key on class+student+level).
    ''' </summary>
    Public Function EvaluateClass(classId As Integer) As List(Of IssuedWarning)
        Dim issued As New List(Of IssuedWarning)
        Dim tardyCount = AppSettings.TardyAlertCount
        Dim rows = Db.Query("SELECT v.student_id, v.student_name, v.subject_code, v.percent_absent, v.hours_absent, v.late_count, " &
                            "       v.attendance_status, st.user_id AS student_user_id, t.user_id AS teacher_user_id " &
                            "FROM v_attendance_status v " &
                            "JOIN students st ON st.student_id = v.student_id " &
                            "JOIN teachers t  ON t.teacher_id  = v.teacher_id " &
                            "WHERE v.class_id = @c", Db.P("@c", classId))

        For Each r As DataRow In rows.Rows
            Dim levels As New List(Of String)
            Select Case Db.ToStr(r("attendance_status"))
                Case "Dropped" : levels.AddRange({"Warning 1", "Warning 2", "Dropped"})
                Case "Warning 2" : levels.AddRange({"Warning 1", "Warning 2"})
                Case "Warning 1" : levels.Add("Warning 1")
            End Select
            Dim lates = Db.ToInt(r("late_count"))
            If tardyCount > 0 AndAlso lates >= tardyCount Then levels.Add("Tardiness")

            For Each level In levels
                Dim pct = Db.ToDec(r("percent_absent"))
                Dim added = Db.Execute("INSERT IGNORE INTO attendance_warnings (class_id, student_id, level, hours_absent, percent, late_count, issued_by) " &
                                       "VALUES (@c, @s, @l, @h, @p, @lc, @by)",
                                       Db.P("@c", classId), Db.P("@s", r("student_id")), Db.P("@l", level),
                                       Db.P("@h", r("hours_absent")), Db.P("@p", pct), Db.P("@lc", lates),
                                       Db.P("@by", If(Session.UserId = 0, CObj(Nothing), Session.UserId)))
                If added = 0 Then Continue For

                Dim w As New IssuedWarning With {
                    .StudentId = Db.ToInt(r("student_id")),
                    .StudentName = Db.ToStr(r("student_name")),
                    .Level = level,
                    .Percent = pct
                }
                issued.Add(w)
                NotifyWarning(w, Db.ToStr(r("subject_code")), lates, Db.ToInt(r("student_user_id")), Db.ToInt(r("teacher_user_id")))
            Next
        Next
        Return issued
    End Function

    Private Sub NotifyWarning(w As IssuedWarning, subject As String, lates As Integer, studentUserId As Integer, teacherUserId As Integer)
        Dim pctText = w.Percent.ToString("0.#") & "%"
        Select Case w.Level
            Case "Tardiness"
                Notifier.Send(studentUserId, "Frequent tardiness",
                              $"You have been late {lates} times in {subject}. Please see your instructor.", "Warning")
                Notifier.Send(teacherUserId, "Frequent tardiness",
                              $"{w.StudentName} has been late {lates} times in {subject} (guidance referral suggested).", "Warning")
            Case "Dropped"
                Notifier.Send(studentUserId, $"Dropped - {subject}",
                              $"Your absences in {subject} reached {pctText} of total class hours. You are subject to dropping (CSU Student Manual s.II.C.2.1).", "Danger")
                Notifier.Send(teacherUserId, $"Dropped - {subject}",
                              $"{w.StudentName} reached the absence limit in {subject} ({pctText}).", "Danger")
                Notifier.SendToAdmins($"Dropped - {subject}",
                                      $"{w.StudentName} reached the absence limit in {subject} ({pctText}).", "Danger")
            Case Else
                Notifier.Send(studentUserId, $"{w.Level} - {subject}",
                              $"Your absences in {subject} reached {pctText} of total class hours. Please attend regularly to avoid being dropped.", "Warning")
                Notifier.Send(teacherUserId, $"{w.Level} - {subject}",
                              $"{w.StudentName} reached {w.Level} in {subject} ({pctText} absent).", "Warning")
        End Select
    End Sub

End Module
