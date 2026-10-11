Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>Minimal vertical bar chart drawn with GDI+ (no chart library). Call SetData(labels, values).</summary>
Public Class SimpleBarChart
    Inherits Control

    Private _labels As New List(Of String)
    Private _values As New List(Of Decimal)
    Public Property BarColor As Color = Theme.Accent
    Public Property ValueFormat As String = "0.#"

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
        BackColor = Theme.Surface
        Font = Theme.FontSmall
        Size = New Size(Theme.Dpi(400), Theme.Dpi(220))
    End Sub

    Public Sub SetData(labels As IEnumerable(Of String), values As IEnumerable(Of Decimal))
        _labels = labels.ToList()
        _values = values.ToList()
        Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g = e.Graphics
        g.Clear(BackColor)
        g.SmoothingMode = SmoothingMode.AntiAlias
        If _values.Count = 0 Then
            TextRenderer.DrawText(g, "No data yet", Font, ClientRectangle, Theme.TextMuted,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            Return
        End If

        Dim top = Theme.Dpi(24), bottom = Theme.Dpi(30)
        Dim plotH = Height - top - bottom
        Dim maxV = Math.Max(1D, _values.Max())
        Dim slot = CSng(Width) / _values.Count
        Dim barW = Math.Min(Theme.Dpi(44), slot * 0.5F)

        Using baseline As New Pen(Theme.Border)
            g.DrawLine(baseline, 0, top + plotH, Width, top + plotH)
        End Using

        For i = 0 To _values.Count - 1
            Dim h = CSng(_values(i) / maxV) * plotH
            Dim x = slot * i + (slot - barW) / 2
            Dim rect As New RectangleF(x, top + plotH - h, barW, Math.Max(h, 1))
            Using path = Theme.RoundedRect(Rectangle.Round(rect), Theme.Dpi(5)), br As New SolidBrush(BarColor)
                g.FillPath(br, path)
            End Using
            TextRenderer.DrawText(g, _values(i).ToString(ValueFormat), Font,
                                  New Rectangle(CInt(slot * i), CInt(top + plotH - h - Theme.Dpi(20)), CInt(slot), Theme.Dpi(18)), Theme.TextPrimary,
                                  TextFormatFlags.HorizontalCenter)
            TextRenderer.DrawText(g, If(i < _labels.Count, _labels(i), ""), Font,
                                  New Rectangle(CInt(slot * i), top + plotH + Theme.Dpi(6), CInt(slot), Theme.Dpi(22)), Theme.TextMuted,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.EndEllipsis)
        Next
    End Sub

End Class
