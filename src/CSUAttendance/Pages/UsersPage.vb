Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' All login accounts. Admins are added here; teacher/student accounts come from the Teachers/Students pages.
''' Reset password and Activate/Deactivate work for every role.
''' </summary>
Public Class UsersPage

    Private _loading As Boolean

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "User Accounts"
        End Get
    End Property

    Public Overrides Sub OnPageShown()
        Theme.StylePrimary(btnAdd, Icons.Add)
        Theme.StyleSecondary(btnReset, Icons.Key)
        Theme.StyleSecondary(btnToggle, Icons.Lock)
        Theme.StyleCombo(cboRole)
        Theme.StyleGrid(dgv)
        AddHint()

        _loading = True
        cboRole.Items.AddRange({"All roles", "Admin", "Teacher", "Student"})
        cboRole.SelectedIndex = 0
        _loading = False
        LoadData()
    End Sub

    ''' <summary>Muted line at the bottom of the card explaining where teacher/student accounts come from.</summary>
    Private Sub AddHint()
        Dim hint As New Label With {
            .Dock = DockStyle.Bottom, .Height = Theme.Dpi(34), .BackColor = Theme.Surface,
            .Font = Theme.FontSmall, .ForeColor = Theme.TextMuted, .TextAlign = ContentAlignment.MiddleLeft,
            .Padding = New Padding(Theme.Dpi(10), 0, 0, 0),
            .Text = "Teacher and student accounts are created automatically on the Teachers and Students pages."
        }
        cardList.Controls.Add(hint)
    End Sub

    Private Sub LoadData()
        Dim q = txtSearch.Text.Trim()
        Dim role = If(cboRole.SelectedIndex <= 0, "", CStr(cboRole.SelectedItem))
        Dim dt = Db.Query(
            "SELECT u.user_id, u.username, u.full_name, u.role, IF(u.is_active = 1, 'Active', 'Inactive') AS status, " &
            "       u.last_login, t.employee_no, s.student_no " &
            "FROM users u LEFT JOIN teachers t ON t.user_id = u.user_id LEFT JOIN students s ON s.user_id = u.user_id " &
            "WHERE (@q = '' OR u.username LIKE @like OR u.full_name LIKE @like) AND (@r = '' OR u.role = @r) " &
            "ORDER BY FIELD(u.role, 'Admin', 'Teacher', 'Student'), u.full_name",
            Db.P("@q", q), Db.P("@like", "%" & q & "%"), Db.P("@r", role))

        dgv.DataSource = dt
        dgv.Columns("user_id").Visible = False
        dgv.Columns("employee_no").Visible = False
        dgv.Columns("student_no").Visible = False
        dgv.Columns("username").HeaderText = "Username"
        dgv.Columns("username").FillWeight = 18
        dgv.Columns("full_name").HeaderText = "Full name"
        dgv.Columns("full_name").FillWeight = 34
        dgv.Columns("role").HeaderText = "Role"
        dgv.Columns("role").FillWeight = 14
        dgv.Columns("status").HeaderText = "Status"
        dgv.Columns("status").FillWeight = 14
        dgv.Columns("last_login").HeaderText = "Last sign-in"
        dgv.Columns("last_login").FillWeight = 20
        dgv.Columns("last_login").DefaultCellStyle.Format = "MMM d, yyyy  h:mm tt"
        dgv.Columns("last_login").DefaultCellStyle.NullValue = "Never"

        lblCount.Text = $"{dt.Rows.Count} account{If(dt.Rows.Count = 1, "", "s")}"
        lblEmpty.Visible = dt.Rows.Count = 0
        lblEmpty.Text = "No accounts match your search or role filter."
        lblEmpty.BringToFront()
        UpdateButtons()
    End Sub

    Private Function Selected(column As String) As String
        If dgv.CurrentRow Is Nothing Then Return ""
        Return Db.ToStr(dgv.CurrentRow.Cells(column).Value)
    End Function

    Private Function SelectedId() As Integer
        If dgv.CurrentRow Is Nothing Then Return 0
        Return Db.ToInt(dgv.CurrentRow.Cells("user_id").Value)
    End Function

    Private Sub UpdateButtons()
        Dim has = SelectedId() > 0
        btnReset.Enabled = has
        btnToggle.Enabled = has
        btnToggle.Text = If(Selected("status") = "Inactive", "Activate", "Deactivate")
    End Sub

    ' ---------------- events ----------------
    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        LoadData()
    End Sub

    Private Sub cboRole_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboRole.SelectedIndexChanged
        If Not _loading Then LoadData()
    End Sub

    Private Sub dgv_SelectionChanged(sender As Object, e As EventArgs) Handles dgv.SelectionChanged
        UpdateButtons()
    End Sub

    Private Sub dgv_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgv.CellPainting
        If e.ColumnIndex < 0 Then Return
        Dim name = dgv.Columns(e.ColumnIndex).Name
        If name = "status" OrElse name = "role" Then Theme.PaintBadgeCell(e)
    End Sub

    ''' <summary>Double-click: admins open their edit dialog; teachers/students open their own page filtered to them.</summary>
    Private Sub dgv_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgv.CellDoubleClick
        If e.RowIndex < 0 Then Return
        Select Case Selected("role")
            Case "Teacher" : Host.Navigate("teachers", Selected("employee_no"))
            Case "Student" : Host.Navigate("students", Selected("student_no"))
            Case Else
                Using f As New AdminDialog(SelectedId())
                    If f.ShowDialog(FindForm()) = DialogResult.OK Then
                        LoadData()
                        ShowToast("Account updated")
                    End If
                End Using
        End Select
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Using f As New AdminDialog(0)
            If f.ShowDialog(FindForm()) = DialogResult.OK Then
                LoadData()
                ShowToast("Admin account added")
            End If
        End Using
    End Sub

    ''' <summary>Students → student no., teachers → employee no., admins → a typed temporary password.</summary>
    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        Dim id = SelectedId()
        If id = 0 Then Return
        Dim username = Selected("username")
        Dim role = Selected("role")
        Try
            If role = "Admin" Then
                Using f As New ResetPasswordDialog(id, username)
                    If f.ShowDialog(FindForm()) = DialogResult.OK Then ShowToast($"Password reset for {username}")
                End Using
                Return
            End If

            Dim newPassword = If(role = "Student", Selected("student_no"), Selected("employee_no"))
            Dim what = If(role = "Student", "their student no.", "their employee no.")
            If Not Confirm($"Reset the password of {username} to {what} ({newPassword})? They must change it at their next sign-in.",
                           "Reset password", "Reset password") Then Return
            AuthService.ResetPassword(id, newPassword)
            ShowToast($"Password reset for {username}")
        Catch ex As Exception
            ShowError(ex, "reset the password")
        End Try
    End Sub

    Private Sub btnToggle_Click(sender As Object, e As EventArgs) Handles btnToggle.Click
        Dim id = SelectedId()
        If id = 0 Then Return
        Dim username = Selected("username")
        Dim activate = Selected("status") = "Inactive"
        Try
            If Not activate Then
                If id = Session.UserId Then
                    Notice.Info(Me, "Not allowed", "You cannot deactivate your own account while signed in.")
                    Return
                End If
                If Selected("role") = "Admin" AndAlso
                   Db.ToInt(Db.Scalar("SELECT COUNT(*) FROM users WHERE role = 'Admin' AND is_active = 1")) <= 1 Then
                    Notice.Info(Me, "Last administrator", "At least one active Admin account must remain.")
                    Return
                End If
                If Not Confirm($"Deactivate {username}? They will not be able to sign in until reactivated.",
                               "Deactivate account", "Deactivate", True) Then Return
            End If
            Db.Execute("UPDATE users SET is_active = @a WHERE user_id = @id", Db.P("@a", If(activate, 1, 0)), Db.P("@id", id))
            Audit.Log(If(activate, "ACTIVATE", "DEACTIVATE"), "User", id, username)
            LoadData()
            ShowToast($"{username} {If(activate, "activated", "deactivated")}")
        Catch ex As Exception
            ShowError(ex, "change the account status")
        End Try
    End Sub

End Class
