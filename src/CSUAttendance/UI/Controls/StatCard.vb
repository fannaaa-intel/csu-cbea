Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

''' <summary>Dashboard stat tile: small icon chip, big number, caption underneath.</summary>
Public Class StatCard
    Inherits CardPanel

    Private _value As String = "0"
    Private _caption As String = "Caption"
    Private _glyph As String = Icons.Dashboard
    Private _accentColor As Color = Theme.Accent
    Private _accentSoft As Color = Theme.AccentSoft

    Public Sub New()
        Size = New Size(220, 108)
    End Sub

    Public Property Value As String
        Get
            Return _value
        End Get
        Set(v As String)
            _value = v
            Invalidate()
        End Set
    End Property

    Public Property Caption As String
        Get
            Return _caption
        End Get
        Set(v As String)
            _caption = v
            Invalidate()
        End Set
    End Property

    Public Property Glyph As String
        Get
            Return _glyph
        End Get
        Set(v As String)
            _glyph = v
            Invalidate()
        End Set
    End Property

    ''' <summary>Icon color; the chip behind it uses the matching soft color.</summary>
    Public Sub SetAccent(fore As Color, soft As Color)
        _accentColor = fore
        _accentSoft = soft
        Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        Dim g = e.Graphics
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        Dim chip As New Rectangle(Width - 56, 16, 40, 40)
        Using path = Theme.RoundedRect(chip, 10), br As New SolidBrush(_accentSoft)
            g.FillPath(br, path)
        End Using
        TextRenderer.DrawText(g, _glyph, Theme.FontIconLarge, chip, _accentColor, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        TextRenderer.DrawText(g, _value, Theme.FontStat, New Rectangle(16, 14, Width - 80, 44), Theme.TextPrimary,
                              TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
        TextRenderer.DrawText(g, _caption, Theme.FontSmall, New Rectangle(16, 62, Width - 32, 30), Theme.TextMuted,
                              TextFormatFlags.Left Or TextFormatFlags.Top Or TextFormatFlags.WordBreak)
    End Sub

End Class
