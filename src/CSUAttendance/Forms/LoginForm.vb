Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>Full-screen sign-in: brand panel on the left, centered sign-in card on the right. Returns OK when Session.User is set.</summary>
Public Class LoginForm

    Private Const DbDownMessage As String = "Cannot connect to the database. Start MySQL in the XAMPP Control Panel, then try again."
    Private _schoolName As String = "CAGAYAN STATE UNIVERSITY"
    Private _policyPct As Decimal() = {10D, 15D, 20D}      ' replaced by the Settings values when the database is up

    Private Sub LoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Theme.EnableDoubleBuffer(pnlBrand)
        CheckDatabase()
    End Sub

    Private Sub LoginForm_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        txtUsername.Focus()
    End Sub

    ''' <summary>Reads the school name; when the database is down, shows the error under the sign-in fields.</summary>
    Private Sub CheckDatabase()
        Dim err As String = ""
        If Db.TestConnection(err) Then
            Try
                _schoolName = AppSettings.SchoolName.ToUpper()
                _policyPct = {AppSettings.Warning1Pct, AppSettings.Warning2Pct, AppSettings.DropPct}
                pnlBrand.Invalidate()
            Catch ex As Exception
                ' settings table missing: keep the default name
            End Try
        Else
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

    ''' <summary>
    ''' Paints the brand side: deep rose gradient with stripes, a CSU seal header, a headline,
    ''' and a "policy card" that draws the 10% / 15% / 20% thresholds as a meter.
    ''' </summary>
    Private Sub pnlBrand_Paint(sender As Object, e As PaintEventArgs) Handles pnlBrand.Paint
        Dim g = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim r = pnlBrand.ClientRectangle
        If r.Width = 0 OrElse r.Height = 0 Then Return
        Using br As New LinearGradientBrush(r, Theme.AccentDeep, Theme.Accent, 115.0F)
            g.FillRectangle(br, r)
        End Using
        Theme.DrawStripes(g, New Rectangle(r.Right - Theme.Dpi(260), 0, Theme.Dpi(260), Theme.Dpi(180)), 22, 18)

        Dim left = Math.Max(Theme.Dpi(48), r.Width \ 10)
        Dim w = r.Width - left * 2
        Dim soft = Color.FromArgb(215, Color.White)

        ' Header: CSU seal + school name
        Dim seal As New Rectangle(left, Theme.Dpi(40), Theme.Dpi(46), Theme.Dpi(46))
        Using br As New SolidBrush(Color.FromArgb(40, Color.White)), pen As New Pen(Color.FromArgb(200, Color.White), Theme.Dpi(2))
            g.FillEllipse(br, seal)
            g.DrawEllipse(pen, seal)
        End Using
        TextRenderer.DrawText(g, "CSU", Theme.FontSmallBold, seal, Color.White, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        TextRenderer.DrawText(g, _schoolName, Theme.FontBold, New Rectangle(seal.Right + Theme.Dpi(14), seal.Top + Theme.Dpi(2), w, Theme.Dpi(22)),
                              Color.White, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)
        TextRenderer.DrawText(g, "Student Attendance Management System", Theme.FontSmall, New Rectangle(seal.Right + Theme.Dpi(14), seal.Top + Theme.Dpi(24), w, Theme.Dpi(20)),
                              soft, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)

        ' Headline block + policy card, centered vertically in the remaining space
        Dim blockH = Theme.Dpi(470)
        Dim top = Math.Max(seal.Bottom + Theme.Dpi(40), (r.Height - blockH) \ 2)
        Using titleFont As New Font("Segoe UI Semibold", 28.0F)
            TextRenderer.DrawText(g, "Every class hour," & vbCrLf & "accounted for.", titleFont,
                                  New Rectangle(left - Theme.Dpi(3), top, w, Theme.Dpi(116)), Color.White, TextFormatFlags.Left Or TextFormatFlags.WordBreak)
        End Using
        TextRenderer.DrawText(g, "Take attendance in seconds, see who is at risk, and print reports for the Registrar.",
                              Theme.FontBase, New Rectangle(left, top + Theme.Dpi(124), Math.Min(w, Theme.Dpi(440)), Theme.Dpi(48)),
                              soft, TextFormatFlags.Left Or TextFormatFlags.WordBreak)

        ' Policy card: a white "ladder" with the three steps and what they mean for a 54-hour class
        Dim card As New Rectangle(left, top + Theme.Dpi(196), Math.Min(w, Theme.Dpi(500)), Theme.Dpi(256))
        DrawPolicyCard(g, card)

        TextRenderer.DrawText(g, "BSAIS 3B  ·  System Project  ·  2026", Theme.FontSmall,
                              New Rectangle(left, r.Bottom - Theme.Dpi(48), w, Theme.Dpi(20)), Color.FromArgb(170, Color.White), TextFormatFlags.Left)
    End Sub

    ''' <summary>White card: header, then one row per step (badge with the %, meaning, hours in a 54-hour class).</summary>
    Private Sub DrawPolicyCard(g As Graphics, card As Rectangle)
        Using shadow = Theme.RoundedRect(New Rectangle(card.X, card.Y + Theme.Dpi(6), card.Width, card.Height), Theme.Dpi(14)),
              br As New SolidBrush(Color.FromArgb(45, 60, 10, 25))
            g.FillPath(br, shadow)
        End Using
        Using path = Theme.RoundedRect(card, Theme.Dpi(14)), br As New SolidBrush(Theme.Surface)
            g.FillPath(br, path)
        End Using
        Dim cx = card.X + Theme.Dpi(24), cw = card.Width - Theme.Dpi(48)
        TextRenderer.DrawText(g, "BEFORE A STUDENT IS DROPPED", Theme.FontSmallBold, New Rectangle(cx, card.Y + Theme.Dpi(20), cw, Theme.Dpi(20)),
                              Theme.Accent, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)
        TextRenderer.DrawText(g, "CSU Student Manual 2019  ·  counted in class hours", Theme.FontSmall, New Rectangle(cx, card.Y + Theme.Dpi(40), cw, Theme.Dpi(20)),
                              Theme.TextMuted, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)
        TextRenderer.DrawText(g, "54-hr class", Theme.FontSmall, New Rectangle(cx, card.Y + Theme.Dpi(40), cw, Theme.Dpi(20)),
                              Theme.TextMuted, TextFormatFlags.Right Or TextFormatFlags.VerticalCenter)

        Dim steps = {(_policyPct(0), "Warning 1", "First warning from the instructor"),
                     (_policyPct(1), "Warning 2", "Second and final warning"),
                     (_policyPct(2), "Dropped", "Subject to dropping from the class")}
        Dim rowH = Theme.Dpi(56)
        Dim y = card.Y + Theme.Dpi(72)
        For Each st In steps
            Using pen As New Pen(Theme.Border)
                g.DrawLine(pen, cx, y, cx + cw, y)
            End Using
            Dim colors = Theme.StatusColors(st.Item2)
            Dim pill As New Rectangle(cx, y + (rowH - Theme.Dpi(28)) \ 2, Theme.Dpi(60), Theme.Dpi(28))
            Using path = Theme.RoundedRect(pill, pill.Height \ 2), br As New SolidBrush(colors.Back)
                g.FillPath(br, path)
            End Using
            TextRenderer.DrawText(g, $"{st.Item1:0.#}%", Theme.FontBold, pill, colors.Fore, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            Dim tx = pill.Right + Theme.Dpi(16)
            TextRenderer.DrawText(g, st.Item2, Theme.FontBold, New Rectangle(tx, y + Theme.Dpi(8), cw - tx + cx, Theme.Dpi(22)),
                                  Theme.TextPrimary, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)
            TextRenderer.DrawText(g, st.Item3, Theme.FontSmall, New Rectangle(tx, y + Theme.Dpi(28), cw - tx + cx - Theme.Dpi(70), Theme.Dpi(20)),
                                  Theme.TextMuted, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
            TextRenderer.DrawText(g, $"{54D * st.Item1 / 100D:0.0#} h", Theme.FontBold, New Rectangle(cx, y, cw, rowH),
                                  Theme.TextPrimary, TextFormatFlags.Right Or TextFormatFlags.VerticalCenter)
            y += rowH
        Next
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
