Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>Sidebar item: icon + label. Active item gets a soft-pink pill and rose text.</summary>
Public Class NavButton
    Inherits Control

    Private _glyph As String = Icons.Dashboard
    Private _active As Boolean
    Private _hover As Boolean

    ''' <summary>Page key passed to MainForm.Navigate (e.g. "dashboard").</summary>
    <Browsable(False)>
    Public Property PageKey As String = ""

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
        Size = New Size(Theme.Dpi(200), Theme.Dpi(34))
        Font = Theme.FontBase
        BackColor = Theme.Surface
        Cursor = Cursors.Hand
        Margin = New Padding(0, 0, 0, Theme.Dpi(2))
    End Sub

    Public Property Glyph As String
        Get
            Return _glyph
        End Get
        Set(value As String)
            _glyph = value
            Invalidate()
        End Set
    End Property

    <DefaultValue(False)>
    Public Property IsActive As Boolean
        Get
            Return _active
        End Get
        Set(value As Boolean)
            _active = value
            Invalidate()
        End Set
    End Property

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
        g.Clear(BackColor)
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim r As New Rectangle(0, 0, Width - 1, Height - 1)
        If _active OrElse _hover Then
            Using path = Theme.RoundedRect(r, Theme.Dpi(8)), br As New SolidBrush(If(_active, Theme.AccentSoft, Theme.Hover))
                g.FillPath(br, path)
            End Using
        End If
        If _active Then
            ' short rose marker on the left edge of the active item
            Dim bar As New Rectangle(0, Height \ 2 - Theme.Dpi(10), Theme.Dpi(4), Theme.Dpi(20))
            Using path = Theme.RoundedRect(bar, Theme.Dpi(2)), br As New SolidBrush(Theme.Accent)
                g.FillPath(br, path)
            End Using
        End If
        Dim fore = If(_active, Theme.Accent, Theme.TextPrimary)
        Dim iconFore = If(_active, Theme.Accent, Theme.TextMuted)
        TextRenderer.DrawText(g, _glyph, Theme.FontIcon, New Rectangle(Theme.Dpi(14), 0, Theme.Dpi(24), Height), iconFore,
                              TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        TextRenderer.DrawText(g, Text, If(_active, Theme.FontBold, Theme.FontBase), New Rectangle(Theme.Dpi(50), 0, Width - Theme.Dpi(54), Height), fore,
                              TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
    End Sub

End Class
