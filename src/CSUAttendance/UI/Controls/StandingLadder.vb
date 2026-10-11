Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>
''' One segmented bar showing how all enrollments split across Good / Warning 1 / Warning 2 / Dropped,
''' with a legend underneath (count + percent per level). Used on the Admin dashboard.
''' </summary>
Public Class StandingLadder
    Inherits Control

    Private Shared ReadOnly Levels As String() = {"Good", "Warning 1", "Warning 2", "Dropped"}
    Private _counts As Integer() = {0, 0, 0, 0}

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
        Font = Theme.FontBase
        Height = Theme.Dpi(120)
    End Sub

    ''' <summary>Counts in the order Good, Warning 1, Warning 2, Dropped.</summary>
    Public Sub SetCounts(good As Integer, warning1 As Integer, warning2 As Integer, dropped As Integer)
        _counts = {good, warning1, warning2, dropped}
        Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g = e.Graphics
        g.Clear(Theme.BackOf(If(Parent, CType(Me, Control))))
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim total = _counts.Sum()
        Dim barH = Theme.Dpi(18)
        Dim bar As New Rectangle(0, Theme.Dpi(6), Width - 1, barH)

        Using path = Theme.RoundedRect(bar, barH \ 2)
            Using br As New SolidBrush(Theme.NeutralSoft)
                g.FillPath(br, path)
            End Using
            If total > 0 Then
                g.SetClip(path)
                Dim x = CSng(bar.X)
                For i = 0 To Levels.Length - 1
                    Dim w = CSng(bar.Width) * _counts(i) / total
                    If w <= 0 Then Continue For
                    Using br As New SolidBrush(Theme.StatusColors(Levels(i)).Fore)
                        g.FillRectangle(br, x, bar.Y, w + 1, bar.Height)
                    End Using
                    x += w
                Next
                g.ResetClip()
            End If
        End Using

        ' Legend: four columns
        Dim colW = Width \ Levels.Length
        For i = 0 To Levels.Length - 1
            Dim cx = colW * i
            Dim colors = Theme.StatusColors(Levels(i))
            Dim top = bar.Bottom + Theme.Dpi(16)
            Using br As New SolidBrush(colors.Fore)
                g.FillEllipse(br, cx, top + Theme.Dpi(5), Theme.Dpi(10), Theme.Dpi(10))
            End Using
            TextRenderer.DrawText(g, Levels(i), Theme.FontSmall, New Rectangle(cx + Theme.Dpi(16), top, colW - Theme.Dpi(16), Theme.Dpi(20)),
                                  Theme.TextMuted, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
            Dim pct = If(total = 0, 0, _counts(i) * 100.0 / total)
            TextRenderer.DrawText(g, _counts(i).ToString(), Theme.FontHeading, New Rectangle(cx, top + Theme.Dpi(22), colW, Theme.Dpi(30)),
                                  Theme.TextPrimary, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)
            Dim numW = TextRenderer.MeasureText(_counts(i).ToString(), Theme.FontHeading).Width
            TextRenderer.DrawText(g, $"{pct:0}%", Theme.FontSmall, New Rectangle(cx + numW, top + Theme.Dpi(28), colW - numW, Theme.Dpi(22)),
                                  Theme.TextMuted, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)
        Next
    End Sub

End Class
