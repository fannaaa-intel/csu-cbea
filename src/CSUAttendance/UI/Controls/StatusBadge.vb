Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>Rounded pill showing a status (Present, Late, Warning 1, Dropped...). Colors come from Theme.StatusColors.</summary>
Public Class StatusBadge
    Inherits Control

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or ControlStyles.UserPaint Or ControlStyles.SupportsTransparentBackColor, True)
        Font = Theme.FontSmall
        Size = New Size(80, 24)
        BackColor = Color.Transparent
        Text = "Good"
    End Sub

    Protected Overrides Sub OnTextChanged(e As EventArgs)
        MyBase.OnTextChanged(e)
        Dim sz = TextRenderer.MeasureText(If(Text, ""), Font)
        Width = sz.Width + 18
        Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim colors = Theme.StatusColors(Text)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Dim r As New Rectangle(0, 0, Width - 1, Height - 1)
        Using path = Theme.RoundedRect(r, Height \ 2), br As New SolidBrush(colors.Back)
            e.Graphics.FillPath(br, path)
        End Using
        TextRenderer.DrawText(e.Graphics, Text, Font, r, colors.Fore, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
    End Sub

End Class
