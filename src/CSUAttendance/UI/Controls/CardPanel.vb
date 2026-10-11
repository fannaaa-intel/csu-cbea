Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>White rounded card with a 1px border. Put content inside; default padding 16.</summary>
Public Class CardPanel
    Inherits Panel

    Private _radius As Integer = Theme.Radius
    Private _borderColor As Color = Theme.Border
    Private _cardColor As Color = Theme.Surface

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
        Padding = New Padding(Theme.Dpi(16))
        MyBase.BackColor = Theme.Surface
    End Sub

    <Category("Appearance"), DefaultValue(8)>
    Public Property CornerRadius As Integer
        Get
            Return _radius
        End Get
        Set(value As Integer)
            _radius = value
            Invalidate()
        End Set
    End Property

    <Category("Appearance")>
    Public Property BorderColor As Color
        Get
            Return _borderColor
        End Get
        Set(value As Color)
            _borderColor = value
            Invalidate()
        End Set
    End Property

    ''' <summary>Fill color of the card (BackColor stays the card color for child controls).</summary>
    <Category("Appearance")>
    Public Property CardColor As Color
        Get
            Return _cardColor
        End Get
        Set(value As Color)
            _cardColor = value
            MyBase.BackColor = value
            Invalidate()
        End Set
    End Property

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        ' Paint the parent's color in the corners, then the rounded card on top.
        Dim parentColor = If(Parent IsNot Nothing, Parent.BackColor, Theme.Background)
        e.Graphics.Clear(parentColor)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Dim r As New Rectangle(0, 0, Width - 1, Height - 1)
        Using path = Theme.RoundedRect(r, Theme.Dpi(_radius)), br As New SolidBrush(_cardColor), pen As New Pen(_borderColor)
            e.Graphics.FillPath(br, path)
            e.Graphics.DrawPath(pen, path)
        End Using
    End Sub

End Class
