Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>
''' Smooth, fully rounded button (anti-aliased) used in Notice dialogs and on the login screen.
''' Works as AcceptButton/CancelButton (IButtonControl). Filled = solid color; otherwise outline.
''' </summary>
Public Class PillButton
    Inherits Control
    Implements IButtonControl

    Private _hover As Boolean
    Private _pressed As Boolean
    Private _isDefault As Boolean

    Public Property FillColor As Color = Theme.Accent
    Public Property Filled As Boolean = True

    <DefaultValue(DialogResult.None)>
    Public Property DialogResult As DialogResult Implements IButtonControl.DialogResult

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or
                 ControlStyles.UserPaint Or ControlStyles.Selectable Or ControlStyles.StandardClick, True)
        Font = Theme.FontBold
        Cursor = Cursors.Hand
        Size = New Size(Theme.Dpi(120), Theme.Dpi(42))
        BackColor = Theme.Surface
        TabStop = True
    End Sub

    Public Sub NotifyDefault(value As Boolean) Implements IButtonControl.NotifyDefault
        _isDefault = value
        Invalidate()
    End Sub

    Public Sub PerformClick() Implements IButtonControl.PerformClick
        If CanSelect Then OnClick(EventArgs.Empty)
    End Sub

    Protected Overrides Sub OnClick(e As EventArgs)
        Dim f = FindForm()
        If f IsNot Nothing AndAlso DialogResult <> DialogResult.None Then f.DialogResult = DialogResult
        MyBase.OnClick(e)
    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)
        If e.KeyCode = Keys.Space OrElse e.KeyCode = Keys.Enter Then
            PerformClick()
            e.Handled = True
        End If
    End Sub

    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        MyBase.OnMouseEnter(e)
        _hover = True
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        MyBase.OnMouseLeave(e)
        _hover = False
        _pressed = False
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        _pressed = True
        Focus()
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        _pressed = False
        Invalidate()
    End Sub

    Protected Overrides Sub OnGotFocus(e As EventArgs)
        MyBase.OnGotFocus(e)
        Invalidate()
    End Sub

    Protected Overrides Sub OnLostFocus(e As EventArgs)
        MyBase.OnLostFocus(e)
        Invalidate()
    End Sub

    Protected Overrides Sub OnEnabledChanged(e As EventArgs)
        MyBase.OnEnabledChanged(e)
        Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g = e.Graphics
        g.Clear(Theme.BackOf(If(Parent, CType(Me, Control))))
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim r As New Rectangle(1, 1, Width - 3, Height - 3)
        Dim shade = If(_pressed, 0.2F, If(_hover, 0.1F, 0F))
        Dim baseColor = If(Enabled, FillColor, Theme.Border)

        Using path = Theme.RoundedRect(r, r.Height \ 2)
            If Filled Then
                Using br As New SolidBrush(Theme.Darken(baseColor, shade))
                    g.FillPath(br, path)
                End Using
            Else
                Using br As New SolidBrush(If(_hover OrElse _pressed, Theme.Hover, Theme.Surface)), pen As New Pen(Theme.Border, Theme.Dpi(1))
                    g.FillPath(br, path)
                    g.DrawPath(pen, path)
                End Using
            End If
            If Focused AndAlso ShowFocusCues Then
                Using pen As New Pen(Color.FromArgb(110, If(Filled, Theme.Ink, Theme.Accent)), Theme.Dpi(2))
                    g.DrawPath(pen, path)
                End Using
            End If
        End Using

        Dim fore = If(Filled, Color.White, If(Enabled, Theme.TextPrimary, Theme.TextMuted))
        TextRenderer.DrawText(g, Text, Font, r, fore,
                              TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.NoPrefix Or TextFormatFlags.EndEllipsis)
    End Sub

End Class
