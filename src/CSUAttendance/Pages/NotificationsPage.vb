Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>
''' The signed-in user's notifications (opened from the bell). Unread ones are bold with a colored dot;
''' after the page is shown everything is marked read and the bell count is refreshed.
''' </summary>
Public Class NotificationsPage
    Inherits PageBase

    Private ReadOnly grid As DataGridView = Ui.ListGrid()
    Private ReadOnly btnMarkRead As Button = Ui.Action("Mark all as read", "secondary", Icons.Check, 150)
    Private lblNote As Label
    Private emptyLabel As Label
    Private _data As DataTable

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Notifications"
        End Get
    End Property

    Public Sub New()
        Dim card = Ui.Card()
        card.Controls.Add(grid)
        card.Controls.Add(Ui.CardHeader("Your notifications", lblNote))
        emptyLabel = Ui.EmptyState(grid, "You're all caught up - no notifications yet.")
        grid.RowTemplate.Height = Theme.Dpi(76)
        grid.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "item"})

        Dim bar = Ui.Toolbar()
        Dim actions = Ui.Row(DockStyle.Right)
        actions.Controls.Add(btnMarkRead)
        bar.Controls.Add(actions)
        Controls.Add(card)
        Controls.Add(bar)

        AddHandler grid.CellPainting, AddressOf PaintItem
        AddHandler btnMarkRead.Click, Sub()
                                          For Each r As DataRow In _data.Rows
                                              r("is_read") = True
                                          Next
                                          grid.Invalidate()
                                          btnMarkRead.Enabled = False
                                      End Sub
    End Sub

    Public Overrides Sub OnPageShown()
        _data = Notifier.GetForUser(Session.UserId)
        grid.Rows.Clear()
        For i = 0 To _data.Rows.Count - 1
            grid.Rows.Add("")
        Next
        Dim unread = _data.Rows.Cast(Of DataRow)().Count(Function(r) Not Db.ToBool(r("is_read")))
        lblNote.Text = If(unread = 0, Ui.Plural(_data.Rows.Count, "notification"), $"{unread} new")
        btnMarkRead.Enabled = unread > 0
        emptyLabel.Visible = _data.Rows.Count = 0

        ' Seen now: mark read in the database and refresh the bell (they stay bold on screen until you leave)
        Notifier.MarkAllRead(Session.UserId)
        Host.RefreshBell()
    End Sub

    Private Sub PaintItem(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 OrElse e.RowIndex >= _data.Rows.Count Then Return
        Dim r = _data.Rows(e.RowIndex)
        Dim g = e.Graphics
        Dim unread = Not Db.ToBool(r("is_read"))
        Using br As New SolidBrush(If(unread, Theme.RowAlt, Theme.Surface))
            g.FillRectangle(br, e.CellBounds)
        End Using
        Using pen As New Pen(Theme.Border)
            g.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1)
        End Using
        g.SmoothingMode = SmoothingMode.AntiAlias

        Dim colors = Theme.StatusColors(Db.ToStr(r("type")))
        Dim chip As New Rectangle(e.CellBounds.X + Theme.Dpi(12), e.CellBounds.Y + Theme.Dpi(16), Theme.Dpi(40), Theme.Dpi(40))
        Using path = Theme.RoundedRect(chip, Theme.Dpi(12)), br As New SolidBrush(colors.Back)
            g.FillPath(br, path)
        End Using
        Dim glyph = TypeGlyph(Db.ToStr(r("type")))
        TextRenderer.DrawText(g, glyph, Theme.FontIcon, chip, colors.Fore, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)

        Dim x = chip.Right + Theme.Dpi(16)
        Dim timeText = RelativeTime(Db.ToDate(r("created_at")))
        Dim timeW = TextRenderer.MeasureText(timeText, Theme.FontSmall).Width + Theme.Dpi(16)
        Dim w = e.CellBounds.Right - x - timeW - Theme.Dpi(12)
        Dim flags = TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPrefix
        TextRenderer.DrawText(g, Db.ToStr(r("title")), If(unread, Theme.FontBold, Theme.FontBase),
                              New Rectangle(x, e.CellBounds.Y + Theme.Dpi(12), w, Theme.Dpi(24)), Theme.TextPrimary, flags)
        TextRenderer.DrawText(g, Db.ToStr(r("message")), Theme.FontSmall,
                              New Rectangle(x, e.CellBounds.Y + Theme.Dpi(36), w, Theme.Dpi(30)), Theme.TextMuted, flags)
        TextRenderer.DrawText(g, timeText, Theme.FontSmall, New Rectangle(e.CellBounds.Right - timeW - Theme.Dpi(12), e.CellBounds.Y + Theme.Dpi(12), timeW, Theme.Dpi(24)),
                              Theme.TextMuted, TextFormatFlags.Right Or TextFormatFlags.VerticalCenter)
        If unread Then
            Using br As New SolidBrush(Theme.Accent)
                g.FillEllipse(br, e.CellBounds.Right - Theme.Dpi(22), e.CellBounds.Y + Theme.Dpi(44), Theme.Dpi(8), Theme.Dpi(8))
            End Using
        End If
        e.Handled = True
    End Sub

    Private Shared Function TypeGlyph(type As String) As String
        Select Case type
            Case "Danger" : Return Icons.ErrorBadge
            Case "Warning" : Return Icons.WarningSign
            Case "Success" : Return Icons.Check
            Case Else : Return Icons.Bell
        End Select
    End Function

    ''' <summary>"just now", "5 minutes ago", "2 hours ago", "yesterday", "Oct 3".</summary>
    Public Shared Function RelativeTime(t As Date) As String
        Dim span = Date.Now - t
        If span.TotalMinutes < 1 Then Return "just now"
        If span.TotalMinutes < 60 Then Return Ui.Plural(CInt(span.TotalMinutes), "minute") & " ago"
        If span.TotalHours < 24 Then Return Ui.Plural(CInt(span.TotalHours), "hour") & " ago"
        If span.TotalDays < 2 Then Return "yesterday"
        If span.TotalDays < 7 Then Return Ui.Plural(CInt(Math.Floor(span.TotalDays)), "day") & " ago"
        Return t.ToString("MMM d")
    End Function

End Class
