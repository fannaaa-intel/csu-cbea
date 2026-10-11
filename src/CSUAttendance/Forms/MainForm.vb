Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' App shell: sidebar (menu per role) + top bar (page title, date, bell) + content area.
''' Pages are UserControls inheriting PageBase; open them with Navigate("key").
''' </summary>
Public Class MainForm

    ''' <summary>True when the user clicked Log out (Program shows the login screen again).</summary>
    Public Property LoggedOut As Boolean

    Private _current As PageBase
    Private _currentKey As String = ""
    Private ReadOnly _navButtons As New List(Of NavButton)
    Private WithEvents _bellTimer As New Timer With {.Interval = 30000}

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim u = Session.User
        Text = $"CSU Attendance  -  {u.FullName} ({u.Role})"
        lblUserName.Text = ShortName(u.FullName)
        lblUserName.Width = btnLogout.Left - lblUserName.Left - Theme.Dpi(2)
        If TextRenderer.MeasureText(lblUserName.Text, lblUserName.Font).Width > lblUserName.Width Then lblUserName.Font = Theme.FontSmallBold
        lblUserRole.Text = u.Role
        lblAvatar.Text = Initials(u.FullName)
        lblDate.Text = Date.Today.ToString("dddd, MMMM d, yyyy")
        lblLogo.Text = "CSU"
        lblLogo.Font = Theme.FontSmallBold
        lblAppSub.AutoSize = False
        lblAppSub.AutoEllipsis = True
        lblAppSub.Width = pnlBrand.ClientSize.Width - lblAppSub.Left - Theme.Dpi(12)
        Try
            lblAppSub.Text = Lookups.ActiveTermLabel()
        Catch ex As Exception
            lblAppSub.Text = "Attendance Management"
        End Try
        btnBell.Text = Icons.Bell
        btnLogout.Text = Icons.Logout
        Dim tip As New ToolTip()
        tip.SetToolTip(btnBell, "Notifications")
        tip.SetToolTip(btnLogout, "Log out")

        MakeRound(lblAvatar)
        MakeRound(lblLogo)                       ' circular CSU seal
        btnBell.FlatAppearance.BorderColor = pnlTopbar.BackColor   ' no dark focus frame around the bell
        AddHandler btnBell.LocationChanged, Sub() PlaceBellBadge()

        BuildMenu()
        Navigate("dashboard")
        RefreshBell()
        _bellTimer.Start()
    End Sub

    ' ------------------------------------------------------------------
    ' Navigation
    ' ------------------------------------------------------------------
    Private Sub BuildMenu()
        flpNav.SuspendLayout()
        flpNav.Controls.Clear()
        _navButtons.Clear()
        Dim lastGroup = ""
        For Each item In PageRegistry.MenuFor(Session.User.Role)
            If item.Group <> "" AndAlso item.Group <> lastGroup Then flpNav.Controls.Add(GroupCaption(item.Group))
            lastGroup = item.Group
            Dim b As New NavButton With {
                .Text = item.Text,
                .Glyph = item.Glyph,
                .PageKey = item.Key,
                .Width = NavButtonWidth()
            }
            AddHandler b.Click, AddressOf NavButton_Click
            flpNav.Controls.Add(b)
            _navButtons.Add(b)
        Next
        flpNav.ResumeLayout()
    End Sub

    ''' <summary>Small uppercase caption above a group of menu items (e.g. "RECORDS").</summary>
    Private Function GroupCaption(text As String) As Label
        Return New Label With {
            .Text = text.ToUpper(), .Font = Theme.FontSmallBold, .ForeColor = Theme.TextMuted, .BackColor = Theme.Surface,
            .AutoSize = False, .Size = New Size(NavButtonWidth(), Theme.Dpi(24)), .TextAlign = ContentAlignment.BottomLeft,
            .Padding = New Padding(Theme.Dpi(14), 0, 0, Theme.Dpi(3)), .Margin = New Padding(0, Theme.Dpi(2), 0, 0)
        }
    End Function

    Private Sub NavButton_Click(sender As Object, e As EventArgs)
        Navigate(DirectCast(sender, NavButton).PageKey)
    End Sub

    Private Function NavButtonWidth() As Integer
        Return flpNav.ClientSize.Width - flpNav.Padding.Horizontal - 2
    End Function

    ''' <summary>Opens a page. arg is handed to the page as NavigationArg (e.g. a class_id).</summary>
    Public Sub Navigate(key As String, Optional arg As Object = Nothing)
        If _current IsNot Nothing AndAlso Not _current.CanLeave() Then Return

        Dim page As PageBase
        Try
            page = PageRegistry.Create(key)
        Catch ex As Exception
            Notice.Fail(Me, "Could not open this page", "Something went wrong while opening the page.", ex.Message)
            Return
        End Try

        page.Host = Me
        page.NavigationArg = arg
        page.Dock = DockStyle.Fill

        pnlContent.SuspendLayout()
        If _current IsNot Nothing Then
            pnlContent.Controls.Remove(_current)
            _current.Dispose()
        End If
        pnlContent.Controls.Add(page)
        pnlContent.ResumeLayout()

        _current = page
        _currentKey = key
        lblPageTitle.Text = page.PageTitle
        For Each b In _navButtons
            b.IsActive = (b.PageKey = key)
        Next

        Try
            page.OnPageShown()
            page.TidyToolbars()
        Catch ex As Exception
            Notice.Fail(Me, "Could not load this page", "Please check that MySQL is running in XAMPP, then open the page again.", ex.Message)
        End Try
    End Sub

    ''' <summary>Reloads the current page (e.g. after settings change).</summary>
    Public Sub ReloadCurrent()
        Dim key = _currentKey
        Dim arg = If(_current IsNot Nothing, _current.NavigationArg, Nothing)
        _current = Nothing
        For Each c In pnlContent.Controls.OfType(Of PageBase)().ToList()
            pnlContent.Controls.Remove(c)
            c.Dispose()
        Next
        Navigate(key, arg)
    End Sub

    Private Sub flpNav_Resize(sender As Object, e As EventArgs) Handles flpNav.Resize
        Dim w = NavButtonWidth()
        For Each c As Control In flpNav.Controls
            c.Width = w
        Next
    End Sub

    ' ------------------------------------------------------------------
    ' Notifications bell
    ' ------------------------------------------------------------------
    Public Sub RefreshBell()
        Try
            Dim n = Notifier.UnreadCount(Session.UserId)
            lblBellCount.Text = If(n > 99, "99+", n.ToString())
            lblBellCount.Visible = n > 0
            PlaceBellBadge()
        Catch ex As Exception
            Debug.WriteLine("Bell refresh failed: " & ex.Message)
        End Try
    End Sub

    ''' <summary>Small rose pill on the bell's top-right corner, sized to its number.</summary>
    Private Sub PlaceBellBadge()
        Dim h = Theme.Dpi(18)
        lblBellCount.Size = New Size(Math.Max(h, TextRenderer.MeasureText(lblBellCount.Text, lblBellCount.Font).Width + Theme.Dpi(8)), h)
        lblBellCount.Location = New Point(btnBell.Right - lblBellCount.Width + Theme.Dpi(4), btnBell.Top + Theme.Dpi(2))
        MakeRound(lblBellCount, h \ 2)
        lblBellCount.BringToFront()
    End Sub

    Private Sub _bellTimer_Tick(sender As Object, e As EventArgs) Handles _bellTimer.Tick
        RefreshBell()
    End Sub

    Private Sub btnBell_Click(sender As Object, e As EventArgs) Handles btnBell.Click
        Navigate("notifications")
    End Sub

    ' ------------------------------------------------------------------
    ' Logout / closing
    ' ------------------------------------------------------------------
    Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        If _current IsNot Nothing AndAlso Not _current.CanLeave() Then Return
        If Not Notice.Confirm(Me, "Log out", "You will return to the sign-in screen.", "Log out", "Stay") Then Return
        Audit.Log("LOGOUT", "User", Session.UserId)
        LoggedOut = True
        _current = Nothing
        Close()
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not LoggedOut AndAlso _current IsNot Nothing AndAlso Not _current.CanLeave() Then
            e.Cancel = True
            Return
        End If
        _bellTimer.Stop()
    End Sub

    ' ------------------------------------------------------------------
    ' Look & feel
    ' ------------------------------------------------------------------
    Private Sub pnlSidebar_Paint(sender As Object, e As PaintEventArgs) Handles pnlSidebar.Paint
        Using pen As New Pen(Theme.Border)
            e.Graphics.DrawLine(pen, pnlSidebar.Width - 1, 0, pnlSidebar.Width - 1, pnlSidebar.Height)
        End Using
    End Sub

    ''' <summary>Thin rose ring around the circular CSU seal.</summary>
    Private Sub pnlBrand_Paint(sender As Object, e As PaintEventArgs) Handles pnlBrand.Paint
        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        Dim r = lblLogo.Bounds
        r.Inflate(Theme.Dpi(3), Theme.Dpi(3))
        Using pen As New Pen(Theme.AccentSoft, Theme.Dpi(2))
            e.Graphics.DrawEllipse(pen, r)
        End Using
    End Sub

    Private Sub pnlTopbar_Paint(sender As Object, e As PaintEventArgs) Handles pnlTopbar.Paint
        Using pen As New Pen(Theme.Border)
            e.Graphics.DrawLine(pen, 0, pnlTopbar.Height - 1, pnlTopbar.Width, pnlTopbar.Height - 1)
        End Using
    End Sub

    Private Sub pnlUser_Paint(sender As Object, e As PaintEventArgs) Handles pnlUser.Paint
        Using pen As New Pen(Theme.Border)
            e.Graphics.DrawLine(pen, 16, 0, pnlUser.Width - 16, 0)
        End Using
    End Sub

    Private Shared Sub MakeRound(c As Control, Optional radius As Integer = -1)
        Dim r = If(radius < 0, c.Height \ 2, radius)
        c.Region = New Region(Theme.RoundedRect(New Rectangle(0, 0, c.Width, c.Height), r))
    End Sub

    ''' <summary>"Maria Elena R. Bautista" → "Maria Bautista" (fits the sidebar without cutting).</summary>
    Private Shared Function ShortName(fullName As String) As String
        Dim parts = fullName.Split({" "c}, StringSplitOptions.RemoveEmptyEntries).Where(Function(p) Not p.EndsWith(".")).ToList()
        Return If(parts.Count <= 2, String.Join(" ", parts), parts.First() & " " & parts.Last())
    End Function

    Private Shared Function Initials(fullName As String) As String
        Dim parts = fullName.Split({" "c}, StringSplitOptions.RemoveEmptyEntries).
                             Where(Function(p) Not p.EndsWith(".")).ToList()
        If parts.Count = 0 Then Return "?"
        If parts.Count = 1 Then Return parts(0).Substring(0, 1).ToUpper()
        Return (parts.First().Substring(0, 1) & parts.Last().Substring(0, 1)).ToUpper()
    End Function

End Class
