Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>Full-screen sign-in: brand panel on the left, centered sign-in card on the right. Returns OK when Session.User is set.</summary>
Public Class LoginForm

    Private Const DbDownMessage As String = "Cannot connect to the database. Start MySQL in the XAMPP Control Panel, then try again."
    Private _schoolName As String = "CAGAYAN STATE UNIVERSITY"
    Private ReadOnly _checkFont As New Font("Segoe MDL2 Assets", 8.0F)

    Private Sub LoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Theme.EnableDoubleBuffer(pnlBrand)
        CheckDatabase()
    End Sub

    Private Sub LoginForm_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        txtUsername.Focus()
    End Sub

    ''' <summary>Shows a green/red database status line and reads the school name.</summary>
    Private Sub CheckDatabase()
        Dim err As String = ""
        If Db.TestConnection(err) Then
            lblDb.Text = ChrW(&H25CF) & "  Database connected"
            lblDb.ForeColor = Theme.Success
            Try
                _schoolName = AppSettings.SchoolName.ToUpper()
                pnlBrand.Invalidate()
            Catch ex As Exception
                ' settings table missing: keep the default name
            End Try
        Else
            lblDb.Text = ChrW(&H25CF) & "  Database offline - start MySQL in XAMPP"
            lblDb.ForeColor = Theme.Danger
            ShowError(DbDownMessage)
        End If
    End Sub

    ' ---------------- layout ----------------
    ''' <summary>Brand panel takes ~45% of the width; the sign-in card stays centered on the right.</summary>
    Private Sub LoginForm_Layout(sender As Object, e As LayoutEventArgs) Handles Me.Layout
        pnlBrand.Width = CInt(ClientSize.Width * 0.45)
    End Sub

    Private Sub pnlForm_Layout(sender As Object, e As LayoutEventArgs) Handles pnlForm.Layout
        pnlCard.Location = New Point(Math.Max(0, (pnlForm.ClientSize.Width - pnlCard.Width) \ 2),
                                     Math.Max(0, (pnlForm.ClientSize.Height - pnlCard.Height) \ 2))
    End Sub

    ''' <summary>Paints the brand side: rose gradient, soft rings, school name, title, three feature lines.</summary>
    Private Sub pnlBrand_Paint(sender As Object, e As PaintEventArgs) Handles pnlBrand.Paint
        Dim g = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim r = pnlBrand.ClientRectangle
        If r.Width = 0 OrElse r.Height = 0 Then Return
        Using br As New LinearGradientBrush(r, Theme.Accent, Theme.AccentDeep, 60.0F)
            g.FillRectangle(br, r)
        End Using
        Theme.DrawStripes(g, New Rectangle(r.Right - Theme.Dpi(220), 0, Theme.Dpi(220), Theme.Dpi(220)), 26, 18)
        Theme.DrawStripes(g, New Rectangle(0, r.Bottom - Theme.Dpi(160), Theme.Dpi(160), Theme.Dpi(160)), 20, 18)
        Using dot As New SolidBrush(Color.FromArgb(30, Color.White))
            For y = Theme.Dpi(40) To Theme.Dpi(140) Step Theme.Dpi(16)
                For x = r.Right - Theme.Dpi(150) To r.Right - Theme.Dpi(40) Step Theme.Dpi(16)
                    g.FillEllipse(dot, x, y, Theme.Dpi(3), Theme.Dpi(3))
                Next
            Next
        End Using

        ' Content block, vertically centered
        Dim left = Math.Max(Theme.Dpi(48), r.Width \ 9)
        Dim w = r.Width - left * 2
        Dim top = Math.Max(Theme.Dpi(40), (r.Height - Theme.Dpi(470)) \ 2)

        Dim tile As New Rectangle(left, top, Theme.Dpi(64), Theme.Dpi(64))
        Using path = Theme.RoundedRect(tile, Theme.Dpi(18)), br As New SolidBrush(Color.FromArgb(50, Color.White))
            g.FillPath(br, path)
        End Using
        Using f As New Font("Segoe MDL2 Assets", 22.0F)
            TextRenderer.DrawText(g, Icons.Classes, f, tile, Color.White, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        End Using

        Dim y0 = tile.Bottom + Theme.Dpi(28)
        TextRenderer.DrawText(g, _schoolName, Theme.FontSmallBold, New Rectangle(left, y0, w, Theme.Dpi(20)), Color.FromArgb(230, Color.White), TextFormatFlags.Left)
        Using titleFont As New Font("Segoe UI Semibold", 26.0F)
            TextRenderer.DrawText(g, "Student Attendance" & vbCrLf & "Management System", titleFont,
                                  New Rectangle(left - Theme.Dpi(3), y0 + Theme.Dpi(26), w, Theme.Dpi(110)), Color.White, TextFormatFlags.Left Or TextFormatFlags.WordBreak)
        End Using
        TextRenderer.DrawText(g, "Record, monitor and report class attendance following the CSU Student Manual attendance policy.",
                              Theme.FontBase, New Rectangle(left, y0 + Theme.Dpi(142), Math.Min(w, Theme.Dpi(420)), Theme.Dpi(48)),
                              Color.FromArgb(225, Color.White), TextFormatFlags.Left Or TextFormatFlags.WordBreak)

        Dim features = {"Absences counted in class hours, not days",
                        "Automatic warnings at 10% and 15%, drop at 20%",
                        "Reports ready to print, save as PDF or export"}
        Dim fy = y0 + Theme.Dpi(214)
        For Each feature In features
            Dim chip As New Rectangle(left, fy, Theme.Dpi(26), Theme.Dpi(26))
            Using br As New SolidBrush(Color.FromArgb(55, Color.White))
                g.FillEllipse(br, chip)
            End Using
            TextRenderer.DrawText(g, Icons.Check, _checkFont, chip, Color.White, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            TextRenderer.DrawText(g, feature, Theme.FontBase, New Rectangle(chip.Right + Theme.Dpi(12), fy, w - Theme.Dpi(40), Theme.Dpi(26)),
                                  Color.White, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
            fy += Theme.Dpi(40)
        Next

        TextRenderer.DrawText(g, "BSAIS 3B  ·  System Project  ·  2026", Theme.FontSmall,
                              New Rectangle(left, r.Bottom - Theme.Dpi(48), w, Theme.Dpi(20)), Color.FromArgb(180, Color.White), TextFormatFlags.Left)
    End Sub

    Private Sub pnlBrand_Resize(sender As Object, e As EventArgs) Handles pnlBrand.Resize
        pnlBrand.Invalidate()
    End Sub

    ' ---------------- sign in ----------------
    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        lblError.Text = ""
        If txtUsername.Text.Trim() = "" Then
            ShowError("Enter your username.")
            txtUsername.Focus()
            Return
        End If
        If txtPassword.Text = "" Then
            ShowError("Enter your password.")
            txtPassword.Focus()
            Return
        End If

        Cursor = Cursors.WaitCursor
        btnLogin.Enabled = False
        btnLogin.Text = "Signing in..."
        btnLogin.Refresh()
        Try
            Dim res = AuthService.Login(txtUsername.Text, txtPassword.Text)
            If Not res.Success Then
                ShowError(res.Message)
                txtPassword.SelectAllText()
                Return
            End If

            If res.MustChangePassword Then
                Using f As New ChangePasswordForm(True)
                    If f.ShowDialog(Me) <> DialogResult.OK Then
                        Session.Clear()
                        ShowError("You need to set a new password before continuing.")
                        Return
                    End If
                End Using
            End If

            AppSettings.Reload()
            DialogResult = DialogResult.OK
        Catch ex As Exception
            ShowError(DbDownMessage)
            Debug.WriteLine(ex.ToString())
        Finally
            Cursor = Cursors.Default
            btnLogin.Enabled = True
            btnLogin.Text = "Sign in"
        End Try
    End Sub

    Private Sub chkShowPassword_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowPassword.CheckedChanged
        txtPassword.UseSystemPasswordChar = Not chkShowPassword.Checked
    End Sub

    Private Sub ShowError(message As String)
        lblError.Text = message
    End Sub

End Class
