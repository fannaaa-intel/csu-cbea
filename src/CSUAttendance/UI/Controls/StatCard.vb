Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

''' <summary>Dashboard stat tile: big number, caption, icon chip, and a thin accent stripe at the top.</summary>
Public Class StatCard
    Inherits CardPanel

    Private _value As String = "0"
    Private _caption As String = "Caption"
    Private _note As String = ""
    Private _glyph As String = Icons.Dashboard
    Private _accentColor As Color = Theme.Accent
    Private _accentSoft As Color = Theme.AccentSoft

    Public Sub New()
        Size = New Size(Theme.Dpi(220), Theme.Dpi(116))
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

    ''' <summary>Optional small line under the caption (e.g. "of 40 enrolled").</summary>
    Public Property Note As String
        Get
            Return _note
        End Get
        Set(v As String)
            _note = If(v, "")
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
        Dim p = Theme.Dpi(18)

        ' accent stripe along the top-left
        Using path = Theme.RoundedRect(New Rectangle(p, 0, Theme.Dpi(36), Theme.Dpi(4)), Theme.Dpi(2)), br As New SolidBrush(_accentColor)
            g.FillPath(br, path)
        End Using

        Dim chipSize = Theme.Dpi(40)
        Dim chip As New Rectangle(Width - p - chipSize, p, chipSize, chipSize)
        Using path = Theme.RoundedRect(chip, Theme.Dpi(12)), br As New SolidBrush(_accentSoft)
            g.FillPath(br, path)
        End Using
        TextRenderer.DrawText(g, _glyph, Theme.FontIconLarge, chip, _accentColor, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)

        Dim textW = Width - p * 2 - chipSize - Theme.Dpi(8)
        TextRenderer.DrawText(g, _value, Theme.FontStat, New Rectangle(p, Theme.Dpi(14), textW, Theme.Dpi(46)), Theme.TextPrimary,
                              TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
        TextRenderer.DrawText(g, _caption, Theme.FontBold, New Rectangle(p, Theme.Dpi(62), Width - p * 2, Theme.Dpi(22)), Theme.TextPrimary,
                              TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
        If _note <> "" Then
            TextRenderer.DrawText(g, _note, Theme.FontSmall, New Rectangle(p, Theme.Dpi(84), Width - p * 2, Theme.Dpi(20)), Theme.TextMuted,
                                  TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
        End If
    End Sub

End Class
