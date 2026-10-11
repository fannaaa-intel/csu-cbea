Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>
''' Soft colored message strip with an icon (e.g. "Locked: sessions can be edited for 3 days").
''' kind follows Theme.StatusColors: Info | Warning | Danger | Success. Hidden when Message is empty.
''' </summary>
Public Class Banner
    Inherits Control

    Private _kind As String = "Info"
    Private _glyph As String

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
        Dock = DockStyle.Top
        Height = Theme.Dpi(48)
        Font = Theme.FontBase
        Visible = False
    End Sub

    ''' <summary>Shows the banner with a message, or hides it when message is "".</summary>
    Public Sub ShowMessage(message As String, Optional kind As String = "Info", Optional glyph As String = Nothing)
        _kind = kind
        _glyph = glyph
        Text = message
        FitHeight()
        Visible = message <> ""
        Invalidate()
    End Sub

    ''' <summary>Height = text lines + padding; re-measured when the width changes (e.g. after docking).</summary>
    Private Sub FitHeight()
        Dim textHeight = TextRenderer.MeasureText(Text, Theme.FontBold, New Size(Math.Max(200, Width - Theme.Dpi(68)), 0),
                                                  TextFormatFlags.WordBreak Or TextFormatFlags.NoPrefix).Height
        Dim h = Math.Max(Theme.Dpi(52), textHeight + Theme.Dpi(36))
        If Height <> h Then Height = h
    End Sub

    Protected Overrides Sub OnSizeChanged(e As EventArgs)
        MyBase.OnSizeChanged(e)
        If Text <> "" Then FitHeight()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g = e.Graphics
        g.Clear(Theme.BackOf(If(Parent, CType(Me, Control))))
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim colors = Theme.StatusColors(_kind)
        Dim r As New Rectangle(0, 0, Width - 1, Height - Theme.Dpi(8) - 1)
        Using path = Theme.RoundedRect(r, Theme.Dpi(8)), br As New SolidBrush(colors.Back)
            g.FillPath(br, path)
        End Using
        Using br As New SolidBrush(colors.Fore)
            g.FillRectangle(br, 0, Theme.Dpi(8), Theme.Dpi(4), r.Height - Theme.Dpi(16))
        End Using
        Dim glyph = If(_glyph IsNot Nothing, _glyph, If(_kind = "Danger", Icons.ErrorBadge, If(_kind = "Warning", Icons.WarningSign, If(_kind = "Success", Icons.Check, Icons.Info))))
        TextRenderer.DrawText(g, glyph, Theme.FontIcon, New Rectangle(Theme.Dpi(16), 0, Theme.Dpi(24), r.Height), colors.Fore,
                              TextFormatFlags.VerticalCenter Or TextFormatFlags.HorizontalCenter)
        TextRenderer.DrawText(g, Text, Theme.FontBold, New Rectangle(Theme.Dpi(52), 0, Width - Theme.Dpi(68), r.Height), Theme.TextPrimary,
                              TextFormatFlags.VerticalCenter Or TextFormatFlags.WordBreak Or TextFormatFlags.NoPrefix)
    End Sub

End Class
