Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>
''' Student dashboard card for one subject: a colored status edge on the left, subject code + name,
''' teacher and schedule, the absence-hours bar, "x of y allowed hours" and P/L/A/E counts. Click = open history.
''' </summary>
Public Class SubjectTicket
    Inherits Control

    Public Property SubjectCode As String = ""
    Public Property Description As String = ""
    Public Property Detail As String = ""
    Public Property Status As String = "Good"
    Public Property HoursAbsent As Decimal
    Public Property AllowedHours As Decimal
    Public Property PercentAbsent As Decimal
    Public Property Counts As String = ""
    Private _hover As Boolean

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
        Height = Theme.Dpi(176)
        Cursor = Cursors.Hand
    End Sub

    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        MyBase.OnMouseEnter(e)
        _hover = True
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        MyBase.OnMouseLeave(e)
        _hover = False
        Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g = e.Graphics
        g.Clear(Theme.BackOf(If(Parent, CType(Me, Control))))
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim colors = Theme.StatusColors(Status)
        Dim card As New Rectangle(0, 0, Width - 1, Height - 1)
        Using path = Theme.RoundedRect(card, Theme.Dpi(10)), br As New SolidBrush(Theme.Surface), pen As New Pen(If(_hover, colors.Fore, Theme.Border))
            g.FillPath(br, path)
            g.SetClip(path)
            Using edge As New SolidBrush(colors.Fore)
                g.FillRectangle(edge, 0, 0, Theme.Dpi(6), Height)
            End Using
            g.ResetClip()
            g.DrawPath(pen, path)
        End Using

        Dim x = Theme.Dpi(24), w = Width - Theme.Dpi(44)
        ' Status pill (top-right)
        Dim pillW = TextRenderer.MeasureText(Status, Theme.FontSmallBold).Width + Theme.Dpi(20)
        Dim pill As New Rectangle(Width - pillW - Theme.Dpi(16), Theme.Dpi(16), pillW, Theme.Dpi(24))
        Using path = Theme.RoundedRect(pill, pill.Height \ 2), br As New SolidBrush(colors.Back)
            g.FillPath(br, path)
        End Using
        TextRenderer.DrawText(g, Status, Theme.FontSmallBold, pill, colors.Fore, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)

        Dim flags = TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPrefix
        TextRenderer.DrawText(g, SubjectCode, Theme.FontHeading, New Rectangle(x, Theme.Dpi(12), pill.X - x - Theme.Dpi(8), Theme.Dpi(30)), Theme.TextPrimary, flags)
        TextRenderer.DrawText(g, Description, Theme.FontBase, New Rectangle(x, Theme.Dpi(42), w, Theme.Dpi(22)), Theme.TextPrimary, flags)
        TextRenderer.DrawText(g, Detail, Theme.FontSmall, New Rectangle(x, Theme.Dpi(64), w, Theme.Dpi(20)), Theme.TextMuted, flags)

        ' Hours bar against the drop limit
        Dim limit = If(AppSettings.DropPct <= 0D, 20D, AppSettings.DropPct)
        Dim track As New Rectangle(x, Theme.Dpi(98), w, Theme.Dpi(10))
        Using path = Theme.RoundedRect(track, track.Height \ 2), br As New SolidBrush(Theme.NeutralSoft)
            g.FillPath(br, path)
        End Using
        Dim ratio = CSng(Math.Min(1D, PercentAbsent / limit))
        If ratio > 0 Then
            Dim fill As New Rectangle(track.X, track.Y, Math.Max(track.Height, CInt(track.Width * ratio)), track.Height)
            Using path = Theme.RoundedRect(fill, track.Height \ 2), br As New SolidBrush(colors.Fore)
                g.FillPath(br, path)
            End Using
        End If
        Using pen As New Pen(Color.FromArgb(140, Theme.TextMuted))
            For Each t In {AppSettings.Warning1Pct, AppSettings.Warning2Pct}
                Dim tx = track.X + CInt(track.Width * CSng(t / limit))
                g.DrawLine(pen, tx, track.Y - Theme.Dpi(3), tx, track.Bottom + Theme.Dpi(3))
            Next
        End Using

        TextRenderer.DrawText(g, $"{HoursAbsent:0.0#} of {AllowedHours:0.0#} allowed absence hours", Theme.FontBold,
                              New Rectangle(x, Theme.Dpi(116), w, Theme.Dpi(22)), Theme.TextPrimary, flags)
        TextRenderer.DrawText(g, Counts, Theme.FontSmall, New Rectangle(x, Theme.Dpi(140), w, Theme.Dpi(20)), Theme.TextMuted, flags)
    End Sub

End Class
