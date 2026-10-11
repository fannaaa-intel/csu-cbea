Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Enroll students into a class: "Available students" (left) → buttons (middle) → "Enrolled" (right).
''' Both grids allow multi-select. Adding uses INSERT IGNORE so re-adding is harmless.
''' </summary>
Public Class EnrollmentPage
    Inherits PageBase

    Private ReadOnly cboClass As ComboBox = Ui.Combo(380)
    Private ReadOnly lblClassInfo As Label = Ui.Muted("")
    Private ReadOnly cboSection As ComboBox = Ui.Combo(170)
    Private ReadOnly txtSearch As TextField = Ui.Search("Search student no. or name", 240)
    Private ReadOnly gridAvailable As DataGridView = Ui.Grid(True)
    Private ReadOnly gridEnrolled As DataGridView = Ui.Grid(True)
    Private ReadOnly btnAdd As Button = Ui.Action("Add selected", "primary", Icons.ArrowRight, 170)
    Private ReadOnly btnAddSection As Button = Ui.Action("Add whole section", "secondary", Icons.People, 170)
    Private ReadOnly btnRemove As Button = Ui.Action("Remove selected", "danger", Icons.ArrowLeft, 170)
    Private lblAvailableNote As Label
    Private lblEnrolledNote As Label
    Private emptyAvailable As Label
    Private emptyEnrolled As Label
    Private _loading As Boolean

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Enrollment"
        End Get
    End Property

    Public Sub New()
        BuildLayout()
    End Sub

    Private Sub BuildLayout()
        ' Body: [Available card] [buttons] [Enrolled card]
        Dim body As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 3, .RowCount = 1, .BackColor = Theme.Background}
        body.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
        body.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, Theme.Dpi(244)))
        body.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
        body.RowStyles.Add(New RowStyle(SizeType.Percent, 100))

        Dim left = Ui.Card()
        left.Margin = Padding.Empty
        Dim filters = Ui.Row(DockStyle.Top)
        filters.Padding = New Padding(0, 0, 0, Theme.Dpi(10))
        filters.Controls.AddRange({cboSection, txtSearch})
        left.Controls.Add(gridAvailable)
        left.Controls.Add(filters)
        left.Controls.Add(Ui.CardHeader("Available students", lblAvailableNote))
        emptyAvailable = Ui.EmptyState(gridAvailable, "")

        Dim right = Ui.Card()
        right.Margin = Padding.Empty
        right.Controls.Add(gridEnrolled)
        right.Controls.Add(Ui.CardHeader("Enrolled in this class", lblEnrolledNote))
        emptyEnrolled = Ui.EmptyState(gridEnrolled, "No students enrolled yet. Pick students on the left and click ""Add selected"".")

        Dim middle As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False,
                                                 .Padding = New Padding(Theme.Dpi(20), Theme.Dpi(140), 0, 0), .BackColor = Theme.Background}
        For Each b In {btnAdd, btnAddSection, btnRemove}
            b.Width = Theme.Dpi(204)
            b.Margin = New Padding(0, 0, 0, Theme.Dpi(12))
            middle.Controls.Add(b)
        Next
        middle.Controls.Add(Ui.Muted("Tip: hold Ctrl or Shift to" & vbCrLf & "select several students."))

        body.Controls.Add(left, 0, 0)
        body.Controls.Add(middle, 1, 0)
        body.Controls.Add(right, 2, 0)

        Dim bar = Ui.Toolbar()
        Dim row = Ui.Row()
        lblClassInfo.Margin = New Padding(Theme.Dpi(4), Theme.Dpi(10), 0, 0)
        row.Controls.AddRange({cboClass, lblClassInfo})
        bar.Controls.Add(row)

        Controls.Add(body)
        Controls.Add(bar)

        AddHandler cboClass.SelectedIndexChanged, Sub() If Not _loading Then ClassChanged()
        AddHandler cboSection.SelectedIndexChanged, Sub() If Not _loading Then LoadAvailable()
        AddHandler txtSearch.TextChanged, Sub() LoadAvailable()
        AddHandler btnAdd.Click, AddressOf AddSelected
        AddHandler btnAddSection.Click, AddressOf AddWholeSection
        AddHandler btnRemove.Click, AddressOf RemoveSelected
        AddHandler gridAvailable.CellDoubleClick, Sub(s, e) If e.RowIndex >= 0 Then AddSelected(Nothing, EventArgs.Empty)
        AddHandler gridAvailable.SelectionChanged, Sub() UpdateButtons()
        AddHandler gridEnrolled.SelectionChanged, Sub() UpdateButtons()
    End Sub

    Public Overrides Sub OnPageShown()
        _loading = True
        Lookups.BindClasses(cboClass, Lookups.ActiveTermId())
        Lookups.BindSections(cboSection)
        _loading = False
        If cboClass.Items.Count > 0 Then
            cboClass.SelectedIndex = 0
            ClassChanged()
        Else
            lblClassInfo.Text = "No classes in the active term yet. Add one on the Classes page."
            UpdateButtons()
        End If
    End Sub

    Private ReadOnly Property ClassId As Integer
        Get
            Return Lookups.SelectedId(cboClass)
        End Get
    End Property

    ''' <summary>New class picked: default the section filter to the class's own section, reload both lists.</summary>
    Private Sub ClassChanged()
        Dim r = Lookups.SelectedRow(cboClass)
        If r Is Nothing Then Return
        _loading = True
        cboSection.SelectedValue = Db.ToInt(r("section_id"))
        _loading = False
        lblClassInfo.Text = $"{Db.ToStr(r("days")).Replace(",", " · ")}   {Date.Today.Add(Db.ToTime(r("start_time"))):h:mm}–{Date.Today.Add(Db.ToTime(r("end_time"))):h:mm tt}   ·   {Db.ToDec(r("total_hours")):0.#} class hours"
        LoadEnrolled()
        LoadAvailable()
    End Sub

    Private Sub LoadEnrolled()
        Dim dt = Db.Query(
            "SELECT s.student_id, s.student_no, " & TextUtil.SortableNameSql("s") & " AS student_name, sec.section_name " &
            "FROM class_enrollments e JOIN students s ON s.student_id = e.student_id LEFT JOIN sections sec ON sec.section_id = s.section_id " &
            "WHERE e.class_id = @c ORDER BY s.last_name, s.first_name", Db.P("@c", ClassId))
        gridEnrolled.DataSource = dt
        FormatGrid(gridEnrolled)
        lblEnrolledNote.Text = Ui.Plural(dt.Rows.Count, "student")
        emptyEnrolled.Visible = dt.Rows.Count = 0
        UpdateButtons()
    End Sub

    ''' <summary>Active students not yet in this class, filtered by section and search.</summary>
    Private Sub LoadAvailable()
        If ClassId = 0 Then Return
        Dim q = txtSearch.Text.Trim()
        Dim dt = Db.Query(
            "SELECT s.student_id, s.student_no, " & TextUtil.SortableNameSql("s") & " AS student_name, sec.section_name " &
            "FROM students s LEFT JOIN sections sec ON sec.section_id = s.section_id " &
            "WHERE s.status = 'Active' AND (@sec = 0 OR s.section_id = @sec) " &
            "  AND s.student_id NOT IN (SELECT student_id FROM class_enrollments WHERE class_id = @c) " &
            "  AND (@q = '' OR s.student_no LIKE @like OR s.first_name LIKE @like OR s.last_name LIKE @like) " &
            "ORDER BY s.last_name, s.first_name",
            Db.P("@sec", Lookups.SelectedId(cboSection)), Db.P("@c", ClassId), Db.P("@q", q), Db.P("@like", "%" & q & "%"))
        gridAvailable.DataSource = dt
        FormatGrid(gridAvailable)
        gridAvailable.ClearSelection()
        lblAvailableNote.Text = Ui.Plural(dt.Rows.Count, "student")
        emptyAvailable.Text = If(q = "", "Everyone in this section is already enrolled.", "No students match your search.")
        emptyAvailable.Visible = dt.Rows.Count = 0
        UpdateButtons()
    End Sub

    Private Shared Sub FormatGrid(g As DataGridView)
        Ui.Hide(g, "student_id")
        Ui.Col(g, "student_no", "Student no.", 26)
        Ui.Col(g, "student_name", "Name", 48)
        Ui.Col(g, "section_name", "Section", 26)
    End Sub

    Private Sub UpdateButtons()
        btnAdd.Enabled = ClassId > 0 AndAlso gridAvailable.SelectedRows.Count > 0
        btnAddSection.Enabled = ClassId > 0 AndAlso Lookups.SelectedId(cboSection) > 0
        btnRemove.Enabled = ClassId > 0 AndAlso gridEnrolled.SelectedRows.Count > 0 AndAlso gridEnrolled.Rows.Count > 0
    End Sub

    Private Shared Function SelectedStudentIds(g As DataGridView) As List(Of Integer)
        Return g.SelectedRows.Cast(Of DataGridViewRow)().Select(Function(r) Db.ToInt(r.Cells("student_id").Value)).ToList()
    End Function

    ' ---------------- actions ----------------
    Private Sub AddSelected(sender As Object, e As EventArgs)
        Dim ids = SelectedStudentIds(gridAvailable)
        If ids.Count = 0 Then Return
        Enroll(ids)
    End Sub

    Private Sub AddWholeSection(sender As Object, e As EventArgs)
        Dim ids = gridAvailable.Rows.Cast(Of DataGridViewRow)().Select(Function(r) Db.ToInt(r.Cells("student_id").Value)).ToList()
        If ids.Count = 0 Then
            Notice.Info(Me, "Nothing to add", "Everyone in this section is already enrolled in the class.")
            Return
        End If
        If Not Confirm($"Enroll all {ids.Count} listed student(s) of {cboSection.Text} into {cboClass.Text}?", "Add whole section", "Enroll all") Then Return
        Enroll(ids)
    End Sub

    Private Sub Enroll(ids As List(Of Integer))
        Try
            Dim added = 0
            Dim classId = Me.ClassId
            Db.InTransaction(Sub(cn, tx)
                                 For Each id In ids
                                     added += Db.Execute(cn, tx, "INSERT IGNORE INTO class_enrollments (class_id, student_id) VALUES (@c, @s)",
                                                         Db.P("@c", classId), Db.P("@s", id))
                                 Next
                             End Sub)
            Audit.Log("ENROLL", "Class", classId, $"{added} student(s) added to {cboClass.Text}")
            LoadEnrolled()
            LoadAvailable()
            ShowToast($"{Ui.Plural(added, "student")} enrolled")
        Catch ex As Exception
            ShowError(ex, "enroll the students")
        End Try
    End Sub

    Private Sub RemoveSelected(sender As Object, e As EventArgs)
        Dim ids = SelectedStudentIds(gridEnrolled)
        If ids.Count = 0 Then Return
        Try
            Dim classId = Me.ClassId
            Dim withAttendance = Db.ToInt(Db.Scalar(
                "SELECT COUNT(DISTINCT ar.student_id) FROM attendance_records ar JOIN attendance_sessions s ON s.session_id = ar.session_id " &
                "WHERE s.class_id = @c AND FIND_IN_SET(ar.student_id, @ids)", Db.P("@c", classId), Db.P("@ids", String.Join(",", ids))))
            Dim warning = If(withAttendance > 0,
                             $" {Ui.Plural(withAttendance, "of them has", "of them have")} attendance in this class - those records will stop showing in summaries.", "")
            If Not Confirm($"Remove {Ui.Plural(ids.Count, "student")} from {cboClass.Text}?{warning}", "Remove from class", "Remove", True) Then Return

            Db.InTransaction(Sub(cn, tx)
                                 For Each id In ids
                                     Db.Execute(cn, tx, "DELETE FROM class_enrollments WHERE class_id = @c AND student_id = @s", Db.P("@c", classId), Db.P("@s", id))
                                 Next
                             End Sub)
            Audit.Log("UNENROLL", "Class", classId, $"{ids.Count} student(s) removed from {cboClass.Text}")
            LoadEnrolled()
            LoadAvailable()
            ShowToast($"{Ui.Plural(ids.Count, "student")} removed")
        Catch ex As Exception
            ShowError(ex, "remove the students")
        End Try
    End Sub

End Class
