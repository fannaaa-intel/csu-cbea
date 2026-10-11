Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>One student's mark in the roster being edited.</summary>
Public Class RosterEntry
    Public Property StudentId As Integer
    Public Property StudentNo As String = ""
    Public Property Name As String = ""
    Public Property Standing As String = "Good"
    Public Property Status As String = "Present"
    Public Property TimeIn As TimeSpan?
    Public Property Remarks As String = ""
    Public Property Requirement As String = ""
    Public Property ExcuseStatus As String = ""
End Class

''' <summary>
''' Take Attendance: pick class + date, mark each student P / L / A / E, save in one transaction.
''' Keyboard: Up/Down move, P L A E set the status, F2 edits remarks, Ctrl+S saves.
''' Teachers can edit a saved session for edit_window_days; Admin always (AttendanceRules.CanEditSession).
''' </summary>
Public Class TakeAttendancePage
    Inherits PageBase

    Private Shared ReadOnly Letters As String() = {"P", "L", "A", "E"}

    Private ReadOnly cboClass As ComboBox = Ui.Combo(360)
    Private ReadOnly dtpDate As DateTimePicker = Ui.DatePicker(170)
    Private ReadOnly btnAllPresent As Button = Ui.Action("Mark all present", "secondary", Icons.CheckList, 150)
    Private ReadOnly btnSave As Button = Ui.Action("Save attendance", "primary", Icons.Save, 160)
    Private ReadOnly banner As New Banner()
    Private ReadOnly chips As New CountChips()
    Private ReadOnly lblSession As Label = Ui.Muted("")
    Private ReadOnly txtCheckIn As TextField = Ui.Search("Type or scan student no. + Enter", 260)
    Private ReadOnly lblCheckIn As Label = Ui.Muted("")
    Private ReadOnly grid As DataGridView = Ui.Grid()
    Private ReadOnly flashTimer As New Timer With {.Interval = 1400}
    Private lblRosterNote As Label
    Private emptyRoster As Label

    Private ReadOnly _roster As New List(Of RosterEntry)
    Private _classRow As DataRow
    Private _sessionExists As Boolean
    Private _locked As Boolean
    Private _dirty As Boolean
    Private _loading As Boolean
    Private _flashRow As Integer = -1
    Private _shownClassId As Integer
    Private _shownDate As Date

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Take Attendance"
        End Get
    End Property

    Public Sub New()
        BuildLayout()
    End Sub

    ' =====================================================================
    '  Layout
    ' =====================================================================
    Private Sub BuildLayout()
        ' Roster card
        Dim card = Ui.Card()
        card.Controls.Add(grid)
        card.Controls.Add(Ui.CardHeader("Class roster", lblRosterNote))
        emptyRoster = Ui.EmptyState(grid, "")
        BuildGridColumns()

        ' Summary strip: counters on the left, quick check-in on the right
        Dim strip As New Panel With {.Dock = DockStyle.Top, .Height = Theme.Dpi(56), .BackColor = Theme.Background}
        Dim leftRow = Ui.Row()
        chips.Margin = New Padding(0, Theme.Dpi(4), Theme.Dpi(16), 0)
        lblSession.Margin = New Padding(0, Theme.Dpi(12), 0, 0)
        leftRow.Controls.AddRange({chips, lblSession})
        Dim rightRow = Ui.Row(DockStyle.Right)
        txtCheckIn.Margin = New Padding(0, Theme.Dpi(2), 0, 0)
        lblCheckIn.Margin = New Padding(0, Theme.Dpi(12), Theme.Dpi(12), 0)
        rightRow.Controls.AddRange({txtCheckIn, lblCheckIn})
        strip.Controls.Add(leftRow)
        strip.Controls.Add(rightRow)

        ' Toolbar: class + date on the left, actions on the right
        Dim bar = Ui.Toolbar()
        Dim filters = Ui.Row()
        filters.Controls.AddRange({cboClass, dtpDate})
        Dim actions = Ui.Row(DockStyle.Right)
        actions.Controls.AddRange({btnSave, btnAllPresent})
        bar.Controls.Add(filters)
        bar.Controls.Add(actions)

        Controls.Add(card)
        Controls.Add(strip)
        Controls.Add(banner)
        Controls.Add(bar)

        dtpDate.MaxDate = Date.Today
        AddHandler cboClass.SelectedIndexChanged, Sub() If Not _loading Then SelectionChanged()
        AddHandler dtpDate.ValueChanged, Sub() If Not _loading Then SelectionChanged()
        AddHandler btnSave.Click, Sub() SaveAttendance()
        AddHandler btnAllPresent.Click, Sub() MarkAllPresent()
        AddHandler txtCheckIn.KeyDown, AddressOf CheckIn_KeyDown
        AddHandler grid.CellPainting, AddressOf Grid_CellPainting
        AddHandler grid.CellClick, AddressOf Grid_CellClick
        AddHandler grid.KeyDown, AddressOf Grid_KeyDown
        AddHandler grid.CellEndEdit, AddressOf Grid_CellEndEdit
        AddHandler flashTimer.Tick, Sub()
                                        flashTimer.Stop()
                                        _flashRow = -1
                                        grid.Invalidate()
                                    End Sub
    End Sub

    Private Sub BuildGridColumns()
        grid.ReadOnly = False
        grid.EditMode = DataGridViewEditMode.EditProgrammatically
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        FixedWidth(AddColumn("no", "#", 4), 52)
        FixedWidth(AddColumn("student_no", "Student no.", 10), 120)
        AddColumn("name", "Name", 45)
        FixedWidth(AddColumn("standing", "Standing", 12), 120)
        For Each l In Letters
            Dim c = AddColumn(l, l, 5)
            FixedWidth(c, 60)
            c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
        Next
        FixedWidth(AddColumn("time_in", "Time in", 9), 100)
        Dim remarks = AddColumn("remarks", "Remarks", 40)
        remarks.ReadOnly = False
        remarks.DefaultCellStyle.ForeColor = Theme.TextMuted
    End Sub

    Private Shared Sub FixedWidth(c As DataGridViewColumn, width As Integer)
        c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        c.Width = Theme.Dpi(width)
    End Sub

    Private Function AddColumn(name As String, header As String, weight As Integer) As DataGridViewColumn
        Dim c As New DataGridViewTextBoxColumn With {.Name = name, .HeaderText = header, .FillWeight = weight, .ReadOnly = True,
                                                     .SortMode = DataGridViewColumnSortMode.NotSortable}
        grid.Columns.Add(c)
        Return c
    End Function

    ' =====================================================================
    '  Loading
    ' =====================================================================
    Public Overrides Sub OnPageShown()
        _loading = True
        Lookups.BindClasses(cboClass, Lookups.ActiveTermId())
        ' Opened from Records / dashboard with (classId, date)
        Dim target = TryCast(NavigationArg, Tuple(Of Integer, Date))
        If target IsNot Nothing Then
            cboClass.SelectedValue = target.Item1
            dtpDate.Value = If(target.Item2 > Date.Today, Date.Today, target.Item2)
        ElseIf cboClass.Items.Count > 0 Then
            cboClass.SelectedIndex = DefaultClassIndex()
        End If
        _loading = False

        If cboClass.Items.Count = 0 Then
            banner.ShowMessage(If(Session.IsTeacher, "You have no classes in the active term. Ask the Admin to assign your classes.",
                                  "There are no classes in the active term yet. Add them on the Classes page."), "Info")
            UpdateUi()
            Return
        End If
        LoadRoster()
        grid.Focus()
    End Sub

    ''' <summary>Pre-selects the class that meets today and is closest to the current time.</summary>
    Private Function DefaultClassIndex() As Integer
        Dim dt = DirectCast(cboClass.DataSource, DataTable)
        Dim best = 0, bestGap = Double.MaxValue
        For i = 0 To dt.Rows.Count - 1
            If Not AttendanceRules.MeetsOn(Db.ToStr(dt.Rows(i)("days")), Date.Today) Then Continue For
            Dim gap = Math.Abs((Db.ToTime(dt.Rows(i)("start_time")) - Date.Now.TimeOfDay).TotalMinutes)
            If gap < bestGap Then
                bestGap = gap
                best = i
            End If
        Next
        Return best
    End Function

    ''' <summary>Class or date changed: confirm discarding unsaved marks, then reload.</summary>
    Private Sub SelectionChanged()
        If _dirty AndAlso Not Confirm("You have unsaved attendance. Discard it and load the other class/date?", "Unsaved attendance", "Discard", True) Then
            _loading = True                    ' put the pickers back on the roster being edited
            cboClass.SelectedValue = _shownClassId
            dtpDate.Value = _shownDate
            _loading = False
            Return
        End If
        LoadRoster()
    End Sub

    Private ReadOnly Property ClassId As Integer
        Get
            Return Lookups.SelectedId(cboClass)
        End Get
    End Property

    Private Sub LoadRoster()
        _classRow = Lookups.SelectedRow(cboClass)
        _shownClassId = ClassId
        _shownDate = dtpDate.Value.Date
        _roster.Clear()
        _dirty = False
        If _classRow Is Nothing Then
            UpdateUi()
            Return
        End If
        Try
            Dim day = dtpDate.Value.Date
            Dim session = Db.Query(
                "SELECT s.session_id, s.updated_at, u.full_name FROM attendance_sessions s LEFT JOIN users u ON u.user_id = s.recorded_by " &
                "WHERE s.class_id = @c AND s.session_date = @d", Db.P("@c", ClassId), Db.P("@d", day))
            _sessionExists = session.Rows.Count > 0
            Dim sessionId = If(_sessionExists, Db.ToInt(session.Rows(0)("session_id")), 0)
            _locked = _sessionExists AndAlso Not AttendanceRules.CanEditSession(day)

            Dim dt = Db.Query(
                "SELECT s.student_id, s.student_no, " & TextUtil.SortableNameSql("s") & " AS student_name, " &
                "       COALESCE(v.attendance_status, 'Good') AS standing, v.late_count, " &
                "       ar.status, ar.time_in, ar.remarks, ar.excuse_requirement, ar.excuse_status " &
                "FROM class_enrollments e JOIN students s ON s.student_id = e.student_id " &
                "LEFT JOIN attendance_records ar ON ar.session_id = @sid AND ar.student_id = s.student_id " &
                "LEFT JOIN v_attendance_status v ON v.class_id = e.class_id AND v.student_id = e.student_id " &
                "WHERE e.class_id = @c ORDER BY s.last_name, s.first_name",
                Db.P("@sid", sessionId), Db.P("@c", ClassId))
            Dim tardy = AppSettings.TardyAlertCount
            For Each r As DataRow In dt.Rows
                Dim standing = Db.ToStr(r("standing"))
                If standing = "Good" AndAlso tardy > 0 AndAlso Db.ToInt(r("late_count")) >= tardy Then standing = "Tardiness"
                _roster.Add(New RosterEntry With {
                    .StudentId = Db.ToInt(r("student_id")), .StudentNo = Db.ToStr(r("student_no")), .Name = Db.ToStr(r("student_name")),
                    .Standing = standing, .Status = If(Db.ToStr(r("status")) = "", "Present", Db.ToStr(r("status"))),
                    .TimeIn = If(r("time_in") Is DBNull.Value, CType(Nothing, TimeSpan?), Db.ToTime(r("time_in"))),
                    .Remarks = Db.ToStr(r("remarks")), .Requirement = Db.ToStr(r("excuse_requirement")), .ExcuseStatus = Db.ToStr(r("excuse_status"))
                })
            Next

            ' Banner: locked > not a meeting day > editing a saved session
            If _locked Then
                banner.ShowMessage($"Locked: teachers can edit a saved session for {AppSettings.EditWindowDays} days. Ask the Admin to change this one.", "Danger", Icons.Lock)
            ElseIf Not AttendanceRules.MeetsOn(Db.ToStr(_classRow("days")), day) Then
                banner.ShowMessage($"This class does not normally meet on {day:dddd}s ({Db.ToStr(_classRow("days")).Replace(",", ", ")}). You can still record a make-up session.", "Warning")
            ElseIf _sessionExists Then
                banner.ShowMessage($"Editing saved attendance - recorded by {Db.ToStr(session.Rows(0)("full_name"))}, last updated {Db.ToDate(session.Rows(0)("updated_at")):MMM d, h:mm tt}.", "Info")
            Else
                banner.ShowMessage("")
            End If
        Catch ex As Exception
            ShowError(ex, "load the class roster")
        End Try
        FillGrid()
        UpdateUi()
    End Sub

    Private Sub FillGrid()
        grid.Rows.Clear()
        For i = 0 To _roster.Count - 1
            Dim en = _roster(i)
            Dim idx = grid.Rows.Add(i + 1, en.StudentNo, en.Name, en.Standing, "", "", "", "", TimeText(en), en.Remarks)
            grid.Rows(idx).Tag = en
        Next
        emptyRoster.Text = "No students are enrolled in this class yet. Enroll them on the Enrollment page."
        emptyRoster.Visible = _roster.Count = 0
    End Sub

    Private Shared Function TimeText(en As RosterEntry) As String
        If Not en.TimeIn.HasValue Then Return ""
        Return Date.Today.Add(en.TimeIn.Value).ToString("h:mm tt")
    End Function

    ' =====================================================================
    '  Marking
    ' =====================================================================
    Private Sub SetStatus(rowIndex As Integer, status As String)
        If _locked OrElse rowIndex < 0 OrElse rowIndex >= _roster.Count Then Return
        Dim en = _roster(rowIndex)
        If status = "Excused" Then
            Using f As New ExcuseDialog(en.Name, en.Requirement)
                If f.ShowDialog(FindForm()) <> DialogResult.OK Then Return
                en.Requirement = f.Requirement
                If en.ExcuseStatus = "" Then en.ExcuseStatus = "Pending"
            End Using
        Else
            en.Requirement = ""
            en.ExcuseStatus = ""
        End If
        If status = "Absent" OrElse status = "Excused" Then en.TimeIn = Nothing
        en.Status = status
        grid.Rows(rowIndex).Cells("time_in").Value = TimeText(en)
        MarkDirty()
        grid.InvalidateRow(rowIndex)
    End Sub

    Private Sub MarkAllPresent()
        If _locked OrElse _roster.Count = 0 Then Return
        For Each en In _roster.Where(Function(x) x.Status <> "Present")
            en.Status = "Present"
            en.Requirement = ""
            en.ExcuseStatus = ""
        Next
        MarkDirty()
        grid.Invalidate()
    End Sub

    Private Sub MarkDirty()
        _dirty = True
        UpdateUi()
    End Sub

    ''' <summary>Quick check-in: Enter on a student no. marks Present/Late from the current time (today only).</summary>
    Private Sub CheckIn_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode <> Keys.Enter Then Return
        e.SuppressKeyPress = True
        Dim no = txtCheckIn.Text.Trim()
        If no = "" OrElse _locked Then Return
        Dim index = _roster.FindIndex(Function(x) x.StudentNo.Equals(no, StringComparison.OrdinalIgnoreCase))
        If index < 0 Then
            lblCheckIn.ForeColor = Theme.Danger
            lblCheckIn.Text = $"{no} is not in this class"
            txtCheckIn.SelectAllText()
            Return
        End If
        Dim en = _roster(index)
        If dtpDate.Value.Date = Date.Today Then
            en.TimeIn = Date.Now.TimeOfDay
            en.Status = AttendanceRules.StatusFromTimeIn(Db.ToTime(_classRow("start_time")), en.TimeIn.Value)
        Else
            en.Status = "Present"
        End If
        en.Requirement = ""
        en.ExcuseStatus = ""
        grid.Rows(index).Cells("time_in").Value = TimeText(en)
        lblCheckIn.ForeColor = If(en.Status = "Late", Theme.Warning, Theme.Success)
        lblCheckIn.Text = $"{en.Name.Split(","c)(0)} · {en.Status}"
        txtCheckIn.Text = ""
        grid.CurrentCell = grid.Rows(index).Cells("name")
        _flashRow = index
        flashTimer.Stop()
        flashTimer.Start()
        MarkDirty()
        grid.Invalidate()
    End Sub

    Private Sub Grid_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        Dim name = grid.Columns(e.ColumnIndex).Name
        Dim i = Array.IndexOf(Letters, name)
        If i >= 0 Then
            SetStatus(e.RowIndex, AttendanceRules.Statuses(i))
        ElseIf name = "remarks" AndAlso Not _locked Then
            grid.BeginEdit(True)
        End If
    End Sub

    Private Sub Grid_KeyDown(sender As Object, e As KeyEventArgs)
        If grid.IsCurrentCellInEditMode OrElse grid.CurrentRow Is Nothing Then Return
        Dim i = Array.IndexOf(Letters, e.KeyCode.ToString())
        If i >= 0 AndAlso Not e.Control Then
            SetStatus(grid.CurrentRow.Index, AttendanceRules.Statuses(i))
            e.Handled = True
        ElseIf e.KeyCode = Keys.F2 AndAlso Not _locked Then
            grid.CurrentCell = grid.CurrentRow.Cells("remarks")
            grid.BeginEdit(True)
            e.Handled = True
        End If
    End Sub

    Private Sub Grid_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs)
        If grid.Columns(e.ColumnIndex).Name <> "remarks" Then Return
        Dim en = _roster(e.RowIndex)
        Dim text = Db.ToStr(grid.Rows(e.RowIndex).Cells("remarks").Value).Trim()
        If text.Length > 255 Then text = text.Substring(0, 255)
        If text <> en.Remarks Then
            en.Remarks = text
            MarkDirty()
        End If
    End Sub

    ''' <summary>Ctrl+S saves from anywhere on the page.</summary>
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = (Keys.Control Or Keys.S) Then
            If grid.IsCurrentCellInEditMode Then grid.EndEdit()
            SaveAttendance()
            Return True
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    ' =====================================================================
    '  Painting: P / L / A / E chips, standing badge, flash highlight
    ' =====================================================================
    Private Sub Grid_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        Dim name = grid.Columns(e.ColumnIndex).Name
        If name = "standing" Then
            Theme.PaintBadgeCell(e)
            Return
        End If
        Dim letterIndex = Array.IndexOf(Letters, name)
        If letterIndex < 0 AndAlso e.RowIndex <> _flashRow Then Return

        Dim selected = (e.State And DataGridViewElementStates.Selected) <> 0
        If e.RowIndex = _flashRow Then
            Using br As New SolidBrush(Theme.SuccessSoft)
                e.Graphics.FillRectangle(br, e.CellBounds)
            End Using
            If letterIndex < 0 Then
                e.Paint(e.CellBounds, DataGridViewPaintParts.ContentForeground)
                e.Handled = True
                Return
            End If
        Else
            e.PaintBackground(e.CellBounds, selected)
        End If

        Dim en = _roster(e.RowIndex)
        Dim status = AttendanceRules.Statuses(letterIndex)
        Dim isOn = en.Status = status
        Dim colors = Theme.StatusColors(status)
        Dim d = Math.Min(Theme.Dpi(30), e.CellBounds.Height - Theme.Dpi(8))
        Dim r As New Rectangle(e.CellBounds.X + (e.CellBounds.Width - d) \ 2, e.CellBounds.Y + (e.CellBounds.Height - d) \ 2, d, d)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        If isOn Then
            Using br As New SolidBrush(colors.Fore)
                e.Graphics.FillEllipse(br, r)
            End Using
        Else
            Using br As New SolidBrush(Theme.Surface), pen As New Pen(If(_locked, Theme.Border, Color.FromArgb(150, colors.Fore)), Theme.Dpi(1))
                e.Graphics.FillEllipse(br, r)
                e.Graphics.DrawEllipse(pen, r)
            End Using
        End If
        TextRenderer.DrawText(e.Graphics, Letters(letterIndex), Theme.FontSmallBold, r,
                              If(isOn, Color.White, If(_locked, Theme.Border, colors.Fore)),
                              TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        e.Handled = True
    End Sub

    ' =====================================================================
    '  Counters + buttons
    ' =====================================================================
    Private Sub UpdateUi()
        Dim counts = AttendanceRules.Statuses.Select(Function(s) New KeyValuePair(Of String, String)(s, _roster.Where(Function(x) x.Status = s).Count().ToString()))
        chips.SetItems(counts)
        If _classRow IsNot Nothing Then
            Dim hours = AttendanceRules.SessionHours(Db.ToTime(_classRow("start_time")), Db.ToTime(_classRow("end_time")))
            lblSession.Text = $"{Date.Today.Add(Db.ToTime(_classRow("start_time"))):h:mm} – {Date.Today.Add(Db.ToTime(_classRow("end_time"))):h:mm tt}  ·  {hours:0.0#} hrs per meeting"
        Else
            lblSession.Text = ""
        End If
        lblRosterNote.Text = If(_roster.Count = 0, "", $"{Ui.Plural(_roster.Count, "student")}{If(_dirty, "  ·  unsaved changes", "  ·  keys: P L A E mark · F2 remarks · Ctrl+S save")}")
        lblRosterNote.ForeColor = If(_dirty, Theme.Warning, Theme.TextMuted)
        Dim canEdit = Not _locked AndAlso _roster.Count > 0
        btnSave.Enabled = canEdit AndAlso (_dirty OrElse Not _sessionExists)
        btnAllPresent.Enabled = canEdit
        txtCheckIn.Enabled = canEdit
        grid.Columns("remarks").ReadOnly = Not canEdit
    End Sub

    ' =====================================================================
    '  Save
    ' =====================================================================
    Private Sub SaveAttendance()
        If Not btnSave.Enabled Then Return
        Dim classId = Me.ClassId
        Dim day = dtpDate.Value.Date
        Dim hours = AttendanceRules.SessionHours(Db.ToTime(_classRow("start_time")), Db.ToTime(_classRow("end_time")))
        Dim sessionId = 0
        Try
            Cursor = Cursors.WaitCursor
            Db.InTransaction(
                Sub(cn, tx)
                    Db.Execute(cn, tx,
                        "INSERT INTO attendance_sessions (class_id, session_date, hours, recorded_by) VALUES (@c, @d, @h, @u) " &
                        "ON DUPLICATE KEY UPDATE hours = VALUES(hours), recorded_by = VALUES(recorded_by), updated_at = NOW()",
                        Db.P("@c", classId), Db.P("@d", day), Db.P("@h", hours), Db.P("@u", Session.UserId))
                    sessionId = Db.ToInt(Db.Scalar(cn, tx, "SELECT session_id FROM attendance_sessions WHERE class_id = @c AND session_date = @d",
                                                   Db.P("@c", classId), Db.P("@d", day)))
                    For Each en In _roster
                        Dim excused = en.Status = "Excused"
                        Db.Execute(cn, tx,
                            "INSERT INTO attendance_records (session_id, student_id, status, time_in, hours_absent, remarks, excuse_requirement, excuse_status, updated_by) " &
                            "VALUES (@s, @st, @status, @t, @ha, @rem, @req, @es, @u) " &
                            "ON DUPLICATE KEY UPDATE status = VALUES(status), time_in = VALUES(time_in), hours_absent = VALUES(hours_absent), " &
                            "remarks = VALUES(remarks), excuse_requirement = VALUES(excuse_requirement), excuse_status = VALUES(excuse_status), updated_by = VALUES(updated_by)",
                            Db.P("@s", sessionId), Db.P("@st", en.StudentId), Db.P("@status", en.Status),
                            Db.P("@t", If(en.TimeIn.HasValue, CObj(en.TimeIn.Value), Nothing)),
                            Db.P("@ha", AttendanceRules.HoursAbsentFor(en.Status, hours)), Db.P("@rem", TextUtil.OrNull(en.Remarks)),
                            Db.P("@req", If(excused, TextUtil.OrNull(en.Requirement), Nothing)),
                            Db.P("@es", If(excused, TextUtil.OrNull(If(en.ExcuseStatus = "", "Pending", en.ExcuseStatus)), Nothing)),
                            Db.P("@u", Session.UserId))
                    Next
                End Sub)

            Dim counts = String.Join(" ", AttendanceRules.Statuses.Select(Function(s) $"{s.Substring(0, 1)}{_roster.Where(Function(x) x.Status = s).Count()}"))
            Audit.Log("SAVE", "Attendance", sessionId, $"{cboClass.Text.Replace("  ", " ")} {day:yyyy-MM-dd}: {counts}")
            Dim issued = AttendanceRules.EvaluateClass(classId)
            Host.RefreshBell()
            _dirty = False
            LoadRoster()
            ShowToast(SavedMessage(issued), If(issued.Count > 0, "Warning", "Success"))
        Catch ex As Exception
            ShowError(ex, "save the attendance")
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>"Attendance saved" or "Attendance saved · 2 students reached Warning 1".</summary>
    Private Shared Function SavedMessage(issued As List(Of IssuedWarning)) As String
        If issued.Count = 0 Then Return "Attendance saved"
        Dim parts = issued.GroupBy(Function(w) w.Level).Select(Function(g) $"{Ui.Plural(g.Count(), "student")} reached {g.Key}")
        Return "Attendance saved  ·  " & String.Join(", ", parts)
    End Function

    Public Overrides Function CanLeave() As Boolean
        If Not _dirty Then Return True
        Return Confirm("You have unsaved attendance for this class. Leave without saving?", "Unsaved attendance", "Leave without saving", True)
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then flashTimer.Dispose()
        MyBase.Dispose(disposing)
    End Sub

End Class
