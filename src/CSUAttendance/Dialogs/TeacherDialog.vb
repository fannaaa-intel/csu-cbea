Imports System.Windows.Forms

''' <summary>
''' Add/edit a teacher. id = 0 → add: creates the users row (role Teacher, password = employee no.,
''' must change at first login) and the teachers row in ONE transaction.
''' </summary>
Public Class TeacherDialog

    Private ReadOnly _id As Integer
    Private _userId As Integer

    Public Sub New()
        Me.New(0)
    End Sub

    Public Sub New(teacherId As Integer)
        InitializeComponent()
        _id = teacherId
    End Sub

    Private Sub TeacherDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Theme.StylePrimary(btnSave)
        SetRail(Icons.Teachers, "Teacher", "Adding a teacher also creates their login account. They sign in with their username and set their own password.")
        Theme.StyleSecondary(btnCancel)
        If _id = 0 Then Return

        lblTitle.Text = "Edit teacher"
        lblHint.Text = "Changing the username takes effect at the teacher's next login."
        Dim dt = Db.Query("SELECT t.user_id, t.employee_no, t.first_name, t.middle_name, t.last_name, t.department, t.contact_no, u.username " &
                          "FROM teachers t JOIN users u ON u.user_id = t.user_id WHERE t.teacher_id = @id", Db.P("@id", _id))
        If dt.Rows.Count = 0 Then
            DialogResult = DialogResult.Cancel
            Return
        End If
        Dim r = dt.Rows(0)
        _userId = Db.ToInt(r("user_id"))
        txtEmpNo.Text = Db.ToStr(r("employee_no"))
        txtUsername.Text = Db.ToStr(r("username"))
        txtFirst.Text = Db.ToStr(r("first_name"))
        txtMiddle.Text = Db.ToStr(r("middle_name"))
        txtLast.Text = Db.ToStr(r("last_name"))
        txtDepartment.Text = Db.ToStr(r("department"))
        txtContact.Text = Db.ToStr(r("contact_no"))
    End Sub

    ''' <summary>Suggest a username like "mbautista" (first initial + last name) when adding.</summary>
    Private Sub txtLast_Leave(sender As Object, e As EventArgs) Handles txtLast.Leave
        If _id > 0 OrElse txtUsername.Text.Trim() <> "" Then Return
        If txtFirst.Text.Trim() = "" OrElse txtLast.Text.Trim() = "" Then Return
        txtUsername.Text = (txtFirst.Text.Trim().Substring(0, 1) & txtLast.Text.Trim()).Replace(" ", "").ToLower()
    End Sub

    Private Function ValidateInputs() As Boolean
        errProvider.Clear()
        Dim ok = True
        Dim empNo = txtEmpNo.Text.Trim().ToUpper()
        Dim username = txtUsername.Text.Trim()

        If empNo = "" Then
            errProvider.SetError(txtEmpNo, "Employee no. is required.")
            ok = False
        ElseIf Db.ToInt(Db.Scalar("SELECT COUNT(*) FROM teachers WHERE employee_no = @e AND teacher_id <> @id",
                                  Db.P("@e", empNo), Db.P("@id", _id))) > 0 Then
            errProvider.SetError(txtEmpNo, $"Employee no. {empNo} already exists.")
            ok = False
        End If

        If username = "" Then
            errProvider.SetError(txtUsername, "Username is required.")
            ok = False
        ElseIf username.Contains(" ") Then
            errProvider.SetError(txtUsername, "Username cannot contain spaces.")
            ok = False
        ElseIf AuthService.UsernameExists(username, _userId) Then
            errProvider.SetError(txtUsername, $"Username {username} is already taken.")
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
        Return ok
    End Function

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Not ValidateInputs() Then Return
        Dim empNo = txtEmpNo.Text.Trim().ToUpper()
        Dim username = txtUsername.Text.Trim()
        Dim fullName = TextUtil.FullName(txtFirst.Text, txtMiddle.Text, txtLast.Text)
        Try
            If _id = 0 Then
                AddTeacher(empNo, username, fullName)
                Notice.Success(Me, "Teacher account created", $"Account created for {fullName}." & Environment.NewLine & Environment.NewLine &
                                    $"Username:  {username}" & Environment.NewLine &
                                    $"Password:  {empNo}" & Environment.NewLine & Environment.NewLine &
                                    "Give this to the teacher; they must change it at first login.")
            Else
                UpdateTeacher(empNo, username, fullName)
            End If
            DialogResult = DialogResult.OK
        Catch ex As Exception When Db.IsDuplicate(ex)
            errProvider.SetError(txtEmpNo, "Employee no. or username already exists.")
        Catch ex As Exception
            Notice.Fail(Me, "Could not save the teacher", "Please check your connection and the details, then try again.", ex.Message)
        End Try
    End Sub

    ''' <summary>Creates the login account, then the teacher linked to it.</summary>
    Private Sub AddTeacher(empNo As String, username As String, fullName As String)
        Dim newId = 0
        Db.InTransaction(Sub(cn, tx)
                             Dim userId = Db.Insert(cn, tx,
                                 "INSERT INTO users (username, password_hash, role, full_name, must_change_password) " &
                                 "VALUES (@u, @h, 'Teacher', @n, 1)",
                                 Db.P("@u", username), Db.P("@h", PasswordHasher.HashPassword(empNo)), Db.P("@n", fullName))
                             newId = Db.Insert(cn, tx,
                                 "INSERT INTO teachers (user_id, employee_no, first_name, middle_name, last_name, department, contact_no) " &
                                 "VALUES (@uid, @e, @f, @m, @l, @d, @c)",
                                 Db.P("@uid", userId), Db.P("@e", empNo), Db.P("@f", txtFirst.Text.Trim()),
                                 Db.P("@m", TextUtil.OrNull(txtMiddle.Text)), Db.P("@l", txtLast.Text.Trim()),
                                 Db.P("@d", TextUtil.OrNull(txtDepartment.Text)), Db.P("@c", TextUtil.OrNull(txtContact.Text)))
                         End Sub)
        Audit.Log("CREATE", "Teacher", newId, $"{empNo} {fullName} (user {username})")
    End Sub

    ''' <summary>Updates the teacher and keeps the account's username and full name in sync.</summary>
    Private Sub UpdateTeacher(empNo As String, username As String, fullName As String)
        Db.InTransaction(Sub(cn, tx)
                             Db.Execute(cn, tx,
                                 "UPDATE teachers SET employee_no = @e, first_name = @f, middle_name = @m, last_name = @l, " &
                                 "department = @d, contact_no = @c WHERE teacher_id = @id",
                                 Db.P("@e", empNo), Db.P("@f", txtFirst.Text.Trim()), Db.P("@m", TextUtil.OrNull(txtMiddle.Text)),
                                 Db.P("@l", txtLast.Text.Trim()), Db.P("@d", TextUtil.OrNull(txtDepartment.Text)),
                                 Db.P("@c", TextUtil.OrNull(txtContact.Text)), Db.P("@id", _id))
                             Db.Execute(cn, tx, "UPDATE users SET username = @u, full_name = @n WHERE user_id = @uid",
                                        Db.P("@u", username), Db.P("@n", fullName), Db.P("@uid", _userId))
                         End Sub)
        Audit.Log("UPDATE", "Teacher", _id, $"{empNo} {fullName}")
    End Sub

End Class
