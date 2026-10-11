Imports System.Windows.Forms

''' <summary>
''' Add/edit a student. id = 0 → add: creates the users row (role Student, username = password = student no.,
''' must change at first login) and the students row in ONE transaction. Edit keeps the account in sync.
''' </summary>
Public Class StudentDialog

    Private ReadOnly _id As Integer
    Private ReadOnly _defaultSectionId As Integer
    Private _userId As Integer

    Public Sub New()
        Me.New(0)
    End Sub

    ''' <param name="defaultSectionId">Section to preselect when adding (e.g. the page's current filter).</param>
    Public Sub New(studentId As Integer, Optional defaultSectionId As Integer = 0)
        InitializeComponent()
        _id = studentId
        _defaultSectionId = defaultSectionId
    End Sub

    Private Sub StudentDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Theme.StylePrimary(btnSave)
        Theme.StyleSecondary(btnCancel)
        Theme.StyleCombo(cboStatus)
        Theme.StyleCombo(cboSex)
        Theme.StyleCombo(cboSection)

        cboSection.DisplayMember = "section_name"
        cboSection.ValueMember = "section_id"
        cboSection.DataSource = Db.Query("SELECT section_id, section_name FROM sections ORDER BY section_name")
        cboSection.SelectedIndex = -1

        If _id = 0 Then
            cboStatus.SelectedItem = "Active"
            If _defaultSectionId > 0 Then cboSection.SelectedValue = _defaultSectionId
            Return
        End If

        lblTitle.Text = "Edit student"
        lblHint.Text = "The login username always follows the student no."
        Dim dt = Db.Query("SELECT * FROM students WHERE student_id = @id", Db.P("@id", _id))
        If dt.Rows.Count = 0 Then
            DialogResult = DialogResult.Cancel
            Return
        End If
        Dim r = dt.Rows(0)
        _userId = Db.ToInt(r("user_id"))
        txtStudentNo.Text = Db.ToStr(r("student_no"))
        cboStatus.SelectedItem = Db.ToStr(r("status"))
        txtFirst.Text = Db.ToStr(r("first_name"))
        txtMiddle.Text = Db.ToStr(r("middle_name"))
        txtLast.Text = Db.ToStr(r("last_name"))
        cboSex.SelectedItem = Db.ToStr(r("sex"))
        If Db.ToInt(r("section_id")) > 0 Then cboSection.SelectedValue = Db.ToInt(r("section_id"))
        txtContact.Text = Db.ToStr(r("contact_no"))
        txtGuardian.Text = Db.ToStr(r("guardian_name"))
        txtGuardianContact.Text = Db.ToStr(r("guardian_contact"))
    End Sub

    Private Function ValidateInputs() As Boolean
        errProvider.Clear()
        Dim ok = True
        Dim studentNo = txtStudentNo.Text.Trim()

        If Not TextUtil.IsStudentNo(studentNo) Then
            errProvider.SetError(txtStudentNo, "Use the format 24-1234 (2 digits, dash, 4 digits).")
            ok = False
        ElseIf Db.ToInt(Db.Scalar("SELECT COUNT(*) FROM students WHERE student_no = @n AND student_id <> @id",
                                  Db.P("@n", studentNo), Db.P("@id", _id))) > 0 Then
            errProvider.SetError(txtStudentNo, $"Student no. {studentNo} already exists.")
            ok = False
        ElseIf AuthService.UsernameExists(studentNo, _userId) Then
            errProvider.SetError(txtStudentNo, $"A user account named {studentNo} already exists.")
            ok = False
        End If

        If txtFirst.Text.Trim() = "" Then
            errProvider.SetError(txtFirst, "First name is required.")
            ok = False
        End If
        If txtLast.Text.Trim() = "" Then
            errProvider.SetError(txtLast, "Last name is required.")
            ok = False
        End If
        If cboSex.SelectedIndex < 0 Then
            errProvider.SetError(cboSex, "Choose Male or Female.")
            ok = False
        End If
        If cboSection.SelectedIndex < 0 Then
            errProvider.SetError(cboSection, If(cboSection.Items.Count = 0, "Add a section first (Sections page).", "Choose a section."))
            ok = False
        End If
        If cboStatus.SelectedIndex < 0 Then
            errProvider.SetError(cboStatus, "Choose a status.")
            ok = False
        End If
        Return ok
    End Function

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Not ValidateInputs() Then Return
        Dim studentNo = txtStudentNo.Text.Trim()
        Dim fullName = TextUtil.FullName(txtFirst.Text, txtMiddle.Text, txtLast.Text)
        Try
            If _id = 0 Then
                AddStudent(studentNo, fullName)
                MessageBox.Show(Me, $"Account created for {fullName}." & Environment.NewLine & Environment.NewLine &
                                    $"Username:  {studentNo}" & Environment.NewLine &
                                    $"Password:  {studentNo}" & Environment.NewLine & Environment.NewLine &
                                    "Give this to the student; they must change it at first login.",
                                "Student account", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                UpdateStudent(studentNo, fullName)
            End If
            DialogResult = DialogResult.OK
        Catch ex As Exception When Db.IsDuplicate(ex)
            errProvider.SetError(txtStudentNo, $"Student no. {studentNo} already exists.")
        Catch ex As Exception
            MessageBox.Show(Me, "Could not save the student." & Environment.NewLine & Environment.NewLine & ex.Message,
                            "Something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>The student's column values, shared by INSERT and UPDATE.</summary>
    Private Function StudentParams() As MySqlConnector.MySqlParameter()
        Return {
            Db.P("@no", txtStudentNo.Text.Trim()), Db.P("@f", txtFirst.Text.Trim()), Db.P("@m", TextUtil.OrNull(txtMiddle.Text)),
            Db.P("@l", txtLast.Text.Trim()), Db.P("@sex", CStr(cboSex.SelectedItem)), Db.P("@sec", Db.ToInt(cboSection.SelectedValue)),
            Db.P("@c", TextUtil.OrNull(txtContact.Text)), Db.P("@g", TextUtil.OrNull(txtGuardian.Text)),
            Db.P("@gc", TextUtil.OrNull(txtGuardianContact.Text)), Db.P("@st", CStr(cboStatus.SelectedItem))
        }
    End Function

    ''' <summary>Creates the login account, then the student linked to it.</summary>
    Private Sub AddStudent(studentNo As String, fullName As String)
        Dim newId = 0
        Db.InTransaction(Sub(cn, tx)
                             Dim userId = Db.Insert(cn, tx,
                                 "INSERT INTO users (username, password_hash, role, full_name, must_change_password) " &
                                 "VALUES (@u, @h, 'Student', @n, 1)",
                                 Db.P("@u", studentNo), Db.P("@h", PasswordHasher.HashPassword(studentNo)), Db.P("@n", fullName))
                             Dim ps = StudentParams().ToList()
                             ps.Add(Db.P("@uid", userId))
                             newId = Db.Insert(cn, tx,
                                 "INSERT INTO students (user_id, student_no, first_name, middle_name, last_name, sex, section_id, " &
                                 "contact_no, guardian_name, guardian_contact, status) " &
                                 "VALUES (@uid, @no, @f, @m, @l, @sex, @sec, @c, @g, @gc, @st)", ps.ToArray())
                         End Sub)
        Audit.Log("CREATE", "Student", newId, $"{studentNo} {fullName}")
    End Sub

    ''' <summary>Updates the student and keeps users.username / full_name in sync.</summary>
    Private Sub UpdateStudent(studentNo As String, fullName As String)
        Db.InTransaction(Sub(cn, tx)
                             Dim ps = StudentParams().ToList()
                             ps.Add(Db.P("@id", _id))
                             Db.Execute(cn, tx,
                                 "UPDATE students SET student_no = @no, first_name = @f, middle_name = @m, last_name = @l, sex = @sex, " &
                                 "section_id = @sec, contact_no = @c, guardian_name = @g, guardian_contact = @gc, status = @st " &
                                 "WHERE student_id = @id", ps.ToArray())
                             If _userId > 0 Then
                                 Db.Execute(cn, tx, "UPDATE users SET username = @u, full_name = @n WHERE user_id = @uid",
                                            Db.P("@u", studentNo), Db.P("@n", fullName), Db.P("@uid", _userId))
                             End If
                         End Sub)
        Audit.Log("UPDATE", "Student", _id, $"{studentNo} {fullName}")
    End Sub

End Class
