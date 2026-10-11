Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>A row of soft pills like "● Present 32   ● Late 3   ● Absent 2". Colors follow the status words.</summary>
Public Class CountChips
    Inherits Control

    Private _items As New List(Of KeyValuePair(Of String, String))

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
        Font = Theme.FontBold
        Height = Theme.Dpi(34)
    End Sub

    ''' <summary>items: (status word, text after it), e.g. ("Present", "32").</summary>
    Public Sub SetItems(items As IEnumerable(Of KeyValuePair(Of String, String)))
        _items = items.ToList()
        Width = _items.Sum(Function(i) ChipWidth(i)) + Theme.Dpi(8) * _items.Count
        Invalidate()
    End Sub

    Private Function ChipWidth(item As KeyValuePair(Of String, String)) As Integer
        Return TextRenderer.MeasureText($"{item.Key}  {item.Value}", Font).Width + Theme.Dpi(36)
    End Function

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g = e.Graphics
        g.Clear(Theme.BackOf(If(Parent, CType(Me, Control))))
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim x = 0
        For Each item In _items
            Dim colors = Theme.StatusColors(item.Key)
            Dim w = ChipWidth(item)
            Dim r As New Rectangle(x, (Height - Theme.Dpi(30)) \ 2, w, Theme.Dpi(30))
            Using path = Theme.RoundedRect(r, r.Height \ 2), br As New SolidBrush(colors.Back)
                g.FillPath(br, path)
            End Using
            Dim d = Theme.Dpi(8)
            Using br As New SolidBrush(colors.Fore)
                g.FillEllipse(br, r.X + Theme.Dpi(12), r.Y + (r.Height - d) \ 2, d, d)
            End Using
            TextRenderer.DrawText(g, $"{item.Key}  {item.Value}", Font, New Rectangle(r.X + Theme.Dpi(26), r.Y, w - Theme.Dpi(30), r.Height),
                                  colors.Fore, TextFormatFlags.VerticalCenter Or TextFormatFlags.Left)
            x += w + Theme.Dpi(8)
        Next
    End Sub

End Class
