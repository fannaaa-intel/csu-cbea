Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>One student of a section, with totals over the chosen subject (or all subjects of the section).</summary>
Public Class SectionStudent
    Public Property StudentId As Integer
    Public Property StudentNo As String = ""
    Public Property Name As String = ""
    Public Property Present As Integer
    Public Property Late As Integer
    Public Property Absent As Integer
    Public Property Excused As Integer
    Public Property HoursAbsent As Decimal
    ''' <summary>% absent (from v_attendance_status); with All subjects, the highest % of any subject.</summary>
    Public Property Percent As Decimal
    ''' <summary>Worst standing over the subjects, and the subject where it happened.</summary>
    Public Property Standing As String = "Good"
    Public Property StandingSubject As String = ""
    ''' <summary>Dated attendance records, newest first.</summary>
    Public ReadOnly Property History As New List(Of DataRow)
End Class

''' <summary>A grid row under an expanded student: the column captions (Record = Nothing) or one meeting.</summary>
Public Class HistoryLine
    Public Property Student As SectionStudent
    Public Property Record As DataRow
End Class

''' <summary>
''' Section Attendance: pick a section, then one subject or all subjects of that section.
''' Every student is listed with P / L / A / E counts and standing. The V button opens the student's meetings
''' (status, time in, reason, attachment) right under the row; hovering a name shows the latest meetings.
''' Teachers only see their own classes. Percentages and standings come from v_attendance_status.
''' </summary>
Public Class SectionAttendancePage
    Inherits PageBase

    Private Shared ReadOnly RiskOrder As String() = {"Dropped", "Warning 2", "Warning 1", "Tardiness", "Good"}

    Private ReadOnly cboSection As ComboBox = Ui.Combo(200)
    Private ReadOnly cboSubject As ComboBox = Ui.Combo(360)
    Private ReadOnly txtSearch As TextField = Ui.Search("Search student no. or name", 240)
    Private ReadOnly chips As New CountChips()
    Private ReadOnly btnHistory As Button = Ui.Action("Hours & history", "primary", Icons.View, 150)
    Private ReadOnly grid As DataGridView = Ui.Grid()
    Private lblNote As Label
    Private emptyLabel As Label
    Private _loading As Boolean

    Private _classes As DataTable
    Private _students As New List(Of SectionStudent)
    Private ReadOnly _expanded As New HashSet(Of Integer)

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Section Attendance"
        End Get
    End Property

    Public Sub New()
        Dim card = Ui.Card()
        card.Controls.Add(grid)
        card.Controls.Add(Ui.CardHeader("Students", lblNote))
        emptyLabel = Ui.EmptyState(grid, "")
        BuildColumns()

        Dim strip As New Panel With {.Dock = DockStyle.Top, .Height = Theme.Dpi(52), .BackColor = Theme.Background}
        chips.Location = New Point(0, Theme.Dpi(4))
        strip.Controls.Add(chips)

        Dim bar = Ui.Toolbar()
        Dim filters = Ui.Row()
        filters.Controls.AddRange({cboSection, cboSubject, txtSearch})
        Dim actions = Ui.Row(DockStyle.Right)
        actions.Controls.Add(btnHistory)
        bar.Controls.Add(filters)
        bar.Controls.Add(actions)

        Controls.Add(card)
        Controls.Add(strip)
        Controls.Add(bar)

        AddHandler cboSection.SelectedIndexChanged, Sub() If Not _loading Then SectionChanged()
        AddHandler cboSubject.SelectedIndexChanged, Sub() If Not _loading Then LoadData()
        AddHandler txtSearch.TextChanged, Sub() FillGrid()
        AddHandler grid.CellPainting, AddressOf Grid_CellPainting
        AddHandler grid.RowPostPaint, AddressOf Grid_RowPostPaint
        AddHandler grid.CellFormatting, AddressOf Grid_CellFormatting
        AddHandler grid.CellClick, AddressOf Grid_CellClick
        AddHandler grid.CellDoubleClick, AddressOf Grid_CellDoubleClick
        AddHandler grid.KeyDown, AddressOf Grid_KeyDown
        AddHandler grid.MouseMove, AddressOf Grid_MouseMove
        AddHandler grid.MouseUp, AddressOf Grid_MouseUp
        AddHandler grid.SelectionChanged, Sub() UpdateButtons()
        AddHandler btnHistory.Click, Sub() ShowHistory()
    End Sub

    Private Sub BuildColumns()
        grid.Columns.Add(Column("toggle", "", 0, 56))
        grid.Columns.Add(Column("student_no", "Student no.", 10))
        grid.Columns.Add(Column("name", "Name", 24))
        For Each letter In {"P", "L", "A", "E"}
            Dim c = Column(letter, letter, 0, 56)
            c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            grid.Columns.Add(c)
        Next
        grid.Columns.Add(Column("hours", "Hours absent", 10, 0, "0.0#"))
        grid.Columns.Add(Column("percent", "% absent", 10, 0, "0.0'%'"))
        grid.Columns.Add(Column("standing", "Status", 12))
    End Sub

    ''' <summary>A read-only text column; width > 0 = fixed width, otherwise it fills by weight.</summary>
    Private Shared Function Column(name As String, header As String, weight As Integer, Optional width As Integer = 0,
                                   Optional format As String = Nothing) As DataGridViewColumn
        Dim c As New DataGridViewTextBoxColumn With {.Name = name, .HeaderText = header, .ReadOnly = True,
                                                     .SortMode = DataGridViewColumnSortMode.NotSortable}
        If width > 0 Then
            c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            c.Width = Theme.Dpi(width)
        Else
            c.FillWeight = weight
        End If
        If format IsNot Nothing Then c.DefaultCellStyle.Format = format
        Return c
    End Function

    ' =====================================================================
    '  Section + subject pickers
    ' =====================================================================
    Public Overrides Sub OnPageShown()
        _loading = True
        _classes = Lookups.ClassesFor(Lookups.ActiveTermId())
        Dim sections = _classes.DefaultView.ToTable(True, "section_id", "section_name")
        sections.DefaultView.Sort = "section_name"
        Lookups.Bind(cboSection, sections.DefaultView.ToTable(), "section_id", "section_name")

        ' Opened from Records / the dashboard with a class id: select its section and subject
        Dim target = If(TypeOf NavigationArg Is Integer, CInt(NavigationArg), 0)
        Dim targetRow = _classes.Rows.Cast(Of DataRow)().FirstOrDefault(Function(r) Db.ToInt(r("class_id")) = target)
        If targetRow IsNot Nothing Then
            cboSection.SelectedValue = Db.ToInt(targetRow("section_id"))
        ElseIf cboSection.Items.Count > 0 Then
            cboSection.SelectedIndex = 0
        End If
        BindSubjects()
        If targetRow IsNot Nothing Then cboSubject.SelectedValue = target
        _loading = False
        LoadData()
    End Sub

    Private Sub SectionChanged()
        _loading = True
        BindSubjects()
        _loading = False
        _expanded.Clear()
        LoadData()
    End Sub

    ''' <summary>The subjects (classes) of the selected section, after an "All subjects" item (value 0).</summary>
    Private Sub BindSubjects()
        Dim dt As New DataTable()
        dt.Columns.Add("class_id", GetType(Integer))
        dt.Columns.Add("label", GetType(String))
        dt.Rows.Add(0, "All subjects of this section")
        Dim sectionId = Lookups.SelectedId(cboSection)
        For Each r In _classes.Rows.Cast(Of DataRow)().Where(Function(x) Db.ToInt(x("section_id")) = sectionId)
            dt.Rows.Add(Db.ToInt(r("class_id")), Db.ToStr(r("label")))
        Next
        Lookups.Bind(cboSubject, dt, "class_id", "label")
        cboSubject.SelectedIndex = 0
    End Sub

    Private ReadOnly Property ClassId As Integer
        Get
            Return Lookups.SelectedId(cboSubject)
        End Get
    End Property

    Private ReadOnly Property AllSubjects As Boolean
        Get
            Return ClassId = 0
        End Get
    End Property

    ' =====================================================================
    '  Loading
    ' =====================================================================
    Private Sub LoadData()
        Dim filter As MySqlConnector.MySqlParameter() = {
            Db.P("@sec", Lookups.SelectedId(cboSection)), Db.P("@term", Lookups.ActiveTermId()), Db.P("@cls", ClassId),
            Db.P("@tch", If(Session.IsTeacher, Session.User.TeacherId, 0))}
        Const ClassFilter = "c.section_id = @sec AND c.term_id = @term AND c.is_active = 1 AND (@cls = 0 OR c.class_id = @cls) AND (@tch = 0 OR c.teacher_id = @tch)"
        Try
            Dim summary = Db.Query(
                "SELECT v.* FROM v_attendance_status v JOIN classes c ON c.class_id = v.class_id WHERE " & ClassFilter, filter)
            Dim history = Db.Query(
                "SELECT ar.student_id, s.session_date, sub.subject_code, ar.status, ar.time_in, ar.remarks, ar.attachment_path, " &
                "       ar.excuse_requirement, ar.excuse_status " &
                "FROM attendance_records ar JOIN attendance_sessions s ON s.session_id = ar.session_id " &
                "JOIN classes c ON c.class_id = s.class_id JOIN subjects sub ON sub.subject_id = c.subject_id " &
                "WHERE " & ClassFilter & " ORDER BY s.session_date DESC, sub.subject_code", CloneParams(filter))
            _students = BuildStudents(summary, history)
        Catch ex As Exception
            ShowError(ex, "load the section's attendance")
            _students = New List(Of SectionStudent)
        End Try
        grid.Columns("percent").HeaderText = If(AllSubjects, "Highest %", "% absent")
        UpdateChips()
        FillGrid()
    End Sub

    ''' <summary>A parameter can only belong to one command, so the second query gets its own copies.</summary>
    Private Shared Function CloneParams(ps As MySqlConnector.MySqlParameter()) As MySqlConnector.MySqlParameter()
        Return ps.Select(Function(p) Db.P(p.ParameterName, p.Value)).ToArray()
    End Function

    ''' <summary>One SectionStudent per student: adds the counts of each subject and keeps the worst standing.</summary>
    Private Shared Function BuildStudents(summary As DataTable, history As DataTable) As List(Of SectionStudent)
        Dim tardy = AppSettings.TardyAlertCount
        Dim byId As New Dictionary(Of Integer, SectionStudent)
        For Each r As DataRow In summary.Rows
            Dim id = Db.ToInt(r("student_id"))
            If Not byId.ContainsKey(id) Then
                byId(id) = New SectionStudent With {.StudentId = id, .StudentNo = Db.ToStr(r("student_no")), .Name = Db.ToStr(r("student_name"))}
            End If
            Dim st = byId(id)
            st.Present += Db.ToInt(r("present_count"))
            st.Late += Db.ToInt(r("late_count"))
            st.Absent += Db.ToInt(r("absent_count"))
            st.Excused += Db.ToInt(r("excused_count"))
            st.HoursAbsent += Db.ToDec(r("hours_absent"))
            st.Percent = Math.Max(st.Percent, Db.ToDec(r("percent_absent")))

            Dim standing = Db.ToStr(r("attendance_status"))
            If standing = "Good" AndAlso tardy > 0 AndAlso Db.ToInt(r("late_count")) >= tardy Then standing = "Tardiness"
            If Array.IndexOf(RiskOrder, standing) < Array.IndexOf(RiskOrder, st.Standing) Then
                st.Standing = standing
                st.StandingSubject = Db.ToStr(r("subject_code"))
            End If
        Next
        For Each r As DataRow In history.Rows
            Dim st As SectionStudent = Nothing
            If byId.TryGetValue(Db.ToInt(r("student_id")), st) Then st.History.Add(r)
        Next
        ' At-risk students first (sorted in VB, never on status text in SQL)
        Return byId.Values.OrderBy(Function(s) Array.IndexOf(RiskOrder, s.Standing)).ThenByDescending(Function(s) s.Percent) _
                          .ThenByDescending(Function(s) s.Late).ThenBy(Function(s) s.Name).ToList()
    End Function

    Private Sub UpdateChips()
        Dim count = Function(s As String) _students.Where(Function(x) x.Standing = s).Count().ToString()
        chips.SetItems({"Good", "Warning 1", "Warning 2", "Dropped", "Tardiness"}.Select(Function(s) New KeyValuePair(Of String, String)(s, count(s))))
        Dim atRisk = _students.Where(Function(x) x.Standing <> "Good").Count()
        lblNote.Text = If(_students.Count = 0, "",
            $"{Ui.Plural(_students.Count, "student")} · {atRisk} need attention · click V to see each student's meetings")
    End Sub

    ''' <summary>Shows the students that match the search; expanded students get their meetings underneath.</summary>
    Private Sub FillGrid()
        Dim q = txtSearch.Text.Trim()
        grid.Rows.Clear()
        For Each st In _students.Where(Function(x) q = "" OrElse
                                                   x.StudentNo.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                                                   x.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
            Dim i = grid.Rows.Add("", st.StudentNo, st.Name, st.Present, st.Late, st.Absent, st.Excused, st.HoursAbsent, st.Percent, st.Standing)
            Dim row = grid.Rows(i)
            row.Tag = st
            row.Cells("name").ToolTipText = RecentSummary(st)
            If AllSubjects AndAlso st.StandingSubject <> "" Then row.Cells("standing").ToolTipText = $"{st.Standing} in {st.StandingSubject}"
            If _expanded.Contains(st.StudentId) Then InsertHistory(i)
        Next
        emptyLabel.Text = If(cboSection.Items.Count = 0, If(Session.IsTeacher, "You have no classes in the active term.", "There are no classes in the active term."),
                          If(q = "", "No students are enrolled in this section's classes.", "No students match your search."))
        emptyLabel.Visible = grid.Rows.Count = 0
        UpdateButtons()
    End Sub

    ''' <summary>Hover text: the five latest meetings.</summary>
    Private Function RecentSummary(st As SectionStudent) As String
        If st.History.Count = 0 Then Return "No meetings recorded yet."
        Dim lines = st.History.Take(5).Select(
            Function(r) $"{Db.ToDate(r("session_date")):MMM d}  {If(AllSubjects, Db.ToStr(r("subject_code")) & "  ", "")}{Db.ToStr(r("status"))}" &
                        If(Db.ToStr(r("remarks")) = "", "", $" - {Db.ToStr(r("remarks"))}"))
        Return "Latest meetings:" & vbCrLf & String.Join(vbCrLf, lines) & vbCrLf & $"Click V to see all {st.History.Count}."
    End Function

    ' =====================================================================
    '  V button: show / hide a student's meetings under the row
    ' =====================================================================
    Private Sub ToggleHistory(rowIndex As Integer)
        Dim st = TryCast(grid.Rows(rowIndex).Tag, SectionStudent)
        If st Is Nothing Then Return
        If _expanded.Remove(st.StudentId) Then
            While rowIndex + 1 < grid.Rows.Count AndAlso TypeOf grid.Rows(rowIndex + 1).Tag Is HistoryLine
                grid.Rows.RemoveAt(rowIndex + 1)
            End While
        Else
            _expanded.Add(st.StudentId)
            InsertHistory(rowIndex)
        End If
        grid.InvalidateRow(rowIndex)
    End Sub

    ''' <summary>Inserts the caption line and one line per meeting below the student's row.</summary>
    Private Sub InsertHistory(studentRow As Integer)
        Dim st = DirectCast(grid.Rows(studentRow).Tag, SectionStudent)
        Dim lines As New List(Of DataGridViewRow) From {NewLine(st, Nothing, 30)}
        For Each r In st.History
            lines.Add(NewLine(st, r, 34))
        Next
        grid.Rows.InsertRange(studentRow + 1, lines.ToArray())
    End Sub

    Private Function NewLine(st As SectionStudent, record As DataRow, height As Integer) As DataGridViewRow
        Dim row As New DataGridViewRow With {.Tag = New HistoryLine With {.Student = st, .Record = record}, .Height = Theme.Dpi(height)}
        row.CreateCells(grid)
        Return row
    End Function

    Private Sub Grid_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 AndAlso grid.Columns(e.ColumnIndex).Name = "toggle" Then ToggleHistory(e.RowIndex)
    End Sub

    Private Sub Grid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 AndAlso grid.Columns(e.ColumnIndex).Name <> "toggle" Then ToggleHistory(e.RowIndex)
    End Sub

    ''' <summary>Enter or Space opens / closes the selected student's meetings.</summary>
    Private Sub Grid_KeyDown(sender As Object, e As KeyEventArgs)
        If grid.CurrentRow Is Nothing OrElse (e.KeyCode <> Keys.Enter AndAlso e.KeyCode <> Keys.Space) Then Return
        ToggleHistory(grid.CurrentRow.Index)
        e.Handled = True
    End Sub

    ' =====================================================================
    '  Painting
    ' =====================================================================
    Private Sub Grid_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.RowIndex < 0 OrElse Not TypeOf grid.Rows(e.RowIndex).Tag Is SectionStudent Then Return
        Dim name = grid.Columns(e.ColumnIndex).Name
        Dim tardy = AppSettings.TardyAlertCount
        If name = "L" AndAlso Not AllSubjects AndAlso tardy > 0 AndAlso Db.ToInt(e.Value) >= tardy Then
            e.CellStyle.ForeColor = Theme.Warning
            e.CellStyle.Font = Theme.FontBold
        ElseIf name = "percent" AndAlso Db.ToDec(e.Value) >= AppSettings.Warning1Pct Then
            e.CellStyle.ForeColor = If(Db.ToDec(e.Value) >= AppSettings.DropPct, Theme.Danger, Theme.Orange)
            e.CellStyle.Font = Theme.FontBold
        End If
    End Sub

    Private Sub Grid_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        If TypeOf grid.Rows(e.RowIndex).Tag Is HistoryLine Then
            ' Meeting lines: plain grey background; the text is drawn across the whole row in RowPostPaint
            Using br As New SolidBrush(Theme.Background)
                e.Graphics.FillRectangle(br, e.CellBounds)
            End Using
            e.Handled = True
            Return
        End If
        Dim name = grid.Columns(e.ColumnIndex).Name
        If name = "standing" Then
            Theme.PaintBadgeCell(e)
        ElseIf name = "toggle" Then
            PaintToggle(e)
        End If
    End Sub

    ''' <summary>The V button: outlined chevron, rose and pointing up while the meetings are shown.</summary>
    Private Sub PaintToggle(e As DataGridViewCellPaintingEventArgs)
        Theme.FillCell(e)
        Dim st = DirectCast(grid.Rows(e.RowIndex).Tag, SectionStudent)
        Dim open = _expanded.Contains(st.StudentId)
        Dim d = Theme.Dpi(28)
        Dim r As New Rectangle(e.CellBounds.X + (e.CellBounds.Width - d) \ 2, e.CellBounds.Y + (e.CellBounds.Height - d) \ 2, d, d)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Using path = Theme.RoundedRect(r, Theme.Dpi(6)), br As New SolidBrush(If(open, Theme.AccentSoft, Theme.Surface)),
              pen As New Pen(If(open, Theme.Accent, Theme.Border))
            e.Graphics.FillPath(br, path)
            e.Graphics.DrawPath(pen, path)
        End Using
        TextRenderer.DrawText(e.Graphics, If(open, Icons.ChevronUp, Icons.ChevronDown), Theme.FontSmall, r,
                              If(open, Theme.Accent, Theme.TextMuted), TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        Theme.DrawRowLine(e)
        e.Handled = True
    End Sub

    ''' <summary>Where each part of a meeting line goes (lined up under the student's columns).</summary>
    Private Structure LineLayout
        Public DateBox, StatusBox, TimeBox, SubjectBox, ReasonBox, FileBox As Rectangle
    End Structure

    Private Function LayoutOf(rowBounds As Rectangle) As LineLayout
        Dim x0 = grid.GetColumnDisplayRectangle(grid.Columns("student_no").Index, False).X + Theme.Dpi(10)
        Dim x1 = grid.GetColumnDisplayRectangle(grid.Columns("name").Index, False).X + Theme.Dpi(10)
        Dim top = rowBounds.Y, h = rowBounds.Height
        Dim l As New LineLayout
        l.DateBox = New Rectangle(x0, top, x1 - x0 - Theme.Dpi(8), h)
        l.StatusBox = New Rectangle(x1, top, Theme.Dpi(84), h)
        l.TimeBox = New Rectangle(l.StatusBox.Right + Theme.Dpi(12), top, Theme.Dpi(76), h)
        l.SubjectBox = New Rectangle(l.TimeBox.Right, top, If(AllSubjects, Theme.Dpi(84), 0), h)
        l.FileBox = New Rectangle(rowBounds.Right - Theme.Dpi(170), top, Theme.Dpi(160), h)
        l.ReasonBox = New Rectangle(l.SubjectBox.Right, top, l.FileBox.X - l.SubjectBox.Right - Theme.Dpi(12), h)
        Return l
    End Function

    Private Sub Grid_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs)
        Dim line = TryCast(grid.Rows(e.RowIndex).Tag, HistoryLine)
        If line Is Nothing Then Return
        Dim g = e.Graphics
        Dim l = LayoutOf(e.RowBounds)
        Const Flags = TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPrefix

        ' Rose guide line in the V column ties the meetings to their student
        Dim toggle = grid.GetColumnDisplayRectangle(grid.Columns("toggle").Index, False)
        Using br As New SolidBrush(Theme.Accent)
            g.FillRectangle(br, toggle.X + toggle.Width \ 2 - Theme.Dpi(1), e.RowBounds.Y, Theme.Dpi(2), e.RowBounds.Height)
        End Using

        If line.Record Is Nothing Then
            If line.Student.History.Count = 0 Then
                TextRenderer.DrawText(g, "No meetings recorded yet.", Theme.FontSmall, Rectangle.Union(l.DateBox, l.ReasonBox), Theme.TextMuted, Flags)
                Return
            End If
            For Each head In {Tuple.Create("DATE", l.DateBox), Tuple.Create("STATUS", l.StatusBox), Tuple.Create("TIME IN", l.TimeBox),
                                 Tuple.Create(If(AllSubjects, "SUBJECT", ""), l.SubjectBox), Tuple.Create("REASON / REMARKS", l.ReasonBox),
                                 Tuple.Create("ATTACHMENT", l.FileBox)}
                TextRenderer.DrawText(g, head.Item1, Theme.FontSmallBold, head.Item2, Theme.TextMuted, Flags)
            Next
            Return
        End If

        Dim r = line.Record
        Dim status = Db.ToStr(r("status"))
        TextRenderer.DrawText(g, $"{Db.ToDate(r("session_date")):ddd, MMM d}", Theme.FontSmall, l.DateBox, Theme.TextPrimary, Flags)
        DrawBadge(g, status, l.StatusBox)
        If r("time_in") IsNot DBNull.Value Then
            TextRenderer.DrawText(g, Date.Today.Add(Db.ToTime(r("time_in"))).ToString("h:mm tt"), Theme.FontSmall, l.TimeBox, Theme.TextMuted, Flags)
        End If
        If AllSubjects Then TextRenderer.DrawText(g, Db.ToStr(r("subject_code")), Theme.FontSmallBold, l.SubjectBox, Theme.TextPrimary, Flags)

        Dim reason = ReasonText(r)
        Dim missing = status <> "Present" AndAlso Db.ToStr(r("remarks")) = ""
        TextRenderer.DrawText(g, reason, Theme.FontSmall, l.ReasonBox, If(missing, Theme.Danger, If(status = "Present", Theme.TextMuted, Theme.TextPrimary)), Flags)

        If Db.ToStr(r("attachment_path")) <> "" Then
            TextRenderer.DrawText(g, Icons.Attach, Theme.FontSmall, New Rectangle(l.FileBox.X, l.FileBox.Y, Theme.Dpi(20), l.FileBox.Height), Theme.Accent, Flags)
            TextRenderer.DrawText(g, "View attachment", Theme.FontSmallBold,
                                  New Rectangle(l.FileBox.X + Theme.Dpi(22), l.FileBox.Y, l.FileBox.Width - Theme.Dpi(22), l.FileBox.Height), Theme.Accent, Flags)
        Else
            TextRenderer.DrawText(g, "-", Theme.FontSmall, l.FileBox, Theme.Border, Flags)
        End If
    End Sub

    ''' <summary>The reason, plus the excuse requirement and its status for Excused.</summary>
    Private Shared Function ReasonText(r As DataRow) As String
        Dim status = Db.ToStr(r("status"))
        Dim reason = Db.ToStr(r("remarks"))
        If reason = "" Then reason = If(status = "Present", "-", "No reason recorded")
        If status = "Excused" AndAlso Db.ToStr(r("excuse_requirement")) <> "" Then
            reason &= $"  ·  Requirement: {Db.ToStr(r("excuse_requirement"))} ({Db.ToStr(r("excuse_status"))})"
        End If
        Return reason
    End Function

    ''' <summary>Small status pill (same colors as PaintBadgeCell).</summary>
    Private Shared Sub DrawBadge(g As Graphics, status As String, box As Rectangle)
        Dim colors = Theme.StatusColors(status)
        Dim h = Theme.Dpi(22)
        Dim w = Math.Min(TextRenderer.MeasureText(status, Theme.FontSmallBold).Width + Theme.Dpi(16), box.Width)
        Dim pill As New Rectangle(box.X, box.Y + (box.Height - h) \ 2, w, h)
        g.SmoothingMode = SmoothingMode.AntiAlias
        Using path = Theme.RoundedRect(pill, h \ 2), br As New SolidBrush(colors.Back)
            g.FillPath(br, path)
        End Using
        TextRenderer.DrawText(g, status, Theme.FontSmallBold, pill, colors.Fore, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
    End Sub

    ' =====================================================================
    '  Attachment link
    ' =====================================================================
    ''' <summary>The attachment file name when the mouse is over a "View attachment" link, otherwise "".</summary>
    Private Function AttachmentAt(location As Point) As String
        Dim hit = grid.HitTest(location.X, location.Y)
        If hit.RowIndex < 0 Then Return ""
        Dim line = TryCast(grid.Rows(hit.RowIndex).Tag, HistoryLine)
        If line Is Nothing OrElse line.Record Is Nothing Then Return ""
        Dim file = Db.ToStr(line.Record("attachment_path"))
        If file = "" Then Return ""
        Return If(LayoutOf(grid.GetRowDisplayRectangle(hit.RowIndex, False)).FileBox.Contains(location), file, "")
    End Function

    Private Sub Grid_MouseMove(sender As Object, e As MouseEventArgs)
        grid.Cursor = If(AttachmentAt(e.Location) <> "", Cursors.Hand, Cursors.Default)
    End Sub

    Private Sub Grid_MouseUp(sender As Object, e As MouseEventArgs)
        If e.Button <> MouseButtons.Left Then Return
        Dim file = AttachmentAt(e.Location)
        If file <> "" Then Attachments.Open(Me, file)
    End Sub

    ' =====================================================================
    '  Hours & history dialog (one subject)
    ' =====================================================================
    Private Function SelectedStudent() As SectionStudent
        If grid.CurrentRow Is Nothing Then Return Nothing
        Dim tag = grid.CurrentRow.Tag
        If TypeOf tag Is HistoryLine Then Return DirectCast(tag, HistoryLine).Student
        Return TryCast(tag, SectionStudent)
    End Function

    Private Sub UpdateButtons()
        btnHistory.Enabled = Not AllSubjects AndAlso SelectedStudent() IsNot Nothing
    End Sub

    Private Sub ShowHistory()
        Dim st = SelectedStudent()
        If st Is Nothing OrElse AllSubjects Then Return
        Using f As New StudentHistoryDialog(ClassId, st.StudentId)
            f.ShowDialog(FindForm())
        End Using
    End Sub

End Class
