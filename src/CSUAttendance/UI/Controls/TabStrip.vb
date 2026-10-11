Imports System.Drawing
Imports System.Windows.Forms

''' <summary>Text tabs with a rose underline under the active one (e.g. "Sessions | Pending excuses").</summary>
Public Class TabStrip
    Inherits Control

    Private ReadOnly _tabs As New List(Of String)
    Private ReadOnly _bounds As New List(Of Rectangle)
    Private _selected As Integer
    Private _hover As Integer = -1

    Public Event SelectedChanged As EventHandler

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
        Font = Theme.FontBold
        Height = Theme.Dpi(40)
        Cursor = Cursors.Hand
    End Sub

    Public Sub SetTabs(ParamArray names As String())
        _tabs.Clear()
        _tabs.AddRange(names)
        Width = names.Sum(Function(n) TextRenderer.MeasureText(n, Font).Width + Theme.Dpi(32))
        Invalidate()
    End Sub

    ''' <summary>Changes the text of one tab (e.g. to show a count).</summary>
    Public Sub SetTabText(index As Integer, text As String)
        If index < 0 OrElse index >= _tabs.Count Then Return
        _tabs(index) = text
        Width = _tabs.Sum(Function(n) TextRenderer.MeasureText(n, Font).Width + Theme.Dpi(32))
        Invalidate()
    End Sub

    Public Property SelectedIndex As Integer
        Get
            Return _selected
        End Get
        Set(value As Integer)
            If value = _selected Then Return
            _selected = value
            Invalidate()
            RaiseEvent SelectedChanged(Me, EventArgs.Empty)
        End Set
    End Property

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        Dim h = _bounds.FindIndex(Function(b) b.Contains(e.Location))
        If h <> _hover Then
            _hover = h
            Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        MyBase.OnMouseLeave(e)
        _hover = -1
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseClick(e As MouseEventArgs)
        MyBase.OnMouseClick(e)
        Dim i = _bounds.FindIndex(Function(b) b.Contains(e.Location))
        If i >= 0 Then SelectedIndex = i
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g = e.Graphics
        g.Clear(Theme.BackOf(If(Parent, CType(Me, Control))))
        _bounds.Clear()
        Dim x = 0
        For i = 0 To _tabs.Count - 1
            Dim w = TextRenderer.MeasureText(_tabs(i), Font).Width + Theme.Dpi(32)
            Dim r As New Rectangle(x, 0, w, Height)
            _bounds.Add(r)
            Dim active = i = _selected
            Dim fore = If(active, Theme.Accent, If(i = _hover, Theme.TextPrimary, Theme.TextMuted))
            TextRenderer.DrawText(g, _tabs(i), Font, New Rectangle(x, 0, w, Height - Theme.Dpi(4)), fore,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            If active Then
                Using br As New SolidBrush(Theme.Accent)
                    g.FillRectangle(br, x + Theme.Dpi(12), Height - Theme.Dpi(3), w - Theme.Dpi(24), Theme.Dpi(3))
                End Using
            End If
            x += w
        Next
    End Sub

End Class
