Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>One class meeting shown on the teacher's "Today" timeline.</summary>
Public Class TimelineEntry
    Public Property ClassId As Integer
    Public Property Start As TimeSpan
    Public Property [End] As TimeSpan
    Public Property Title As String = ""          ' AIS 311 · BSAIS 3B
    Public Property Detail As String = ""         ' Accounting Information Systems · CBEA 201
    Public Property Taken As Boolean
    Public Property Summary As String = ""        ' 32 P · 1 L · 2 A
End Class

''' <summary>
''' Vertical timeline of today's classes: a time column, a rail with a dot per class
''' (filled = attendance taken), the class name, and a button "Take attendance" / "Edit".
''' Raises OpenClass(classId) when the button is clicked.
''' </summary>
Public Class ClassTimeline
    Inherits Panel

    Public Event OpenClass(classId As Integer)

    Public Sub New()
        BackColor = Theme.Surface
        Theme.EnableDoubleBuffer(Me)
    End Sub

    Public Sub SetEntries(entries As IEnumerable(Of TimelineEntry))
        SuspendLayout()
        For Each oldRow In Controls.OfType(Of TimelineRow)().ToList()     ' keep any other child (e.g. an empty-state label)
            Controls.Remove(oldRow)
            oldRow.Dispose()
        Next
        Dim y = 0
        Dim list = entries.ToList()
        For i = 0 To list.Count - 1
            Dim row As New TimelineRow(list(i), i = 0, i = list.Count - 1) With {.Top = y, .Width = ClientSize.Width,
                                                                                  .Anchor = AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top}
            AddHandler row.OpenClicked, Sub(id) RaiseEvent OpenClass(id)
            Controls.Add(row)
            y += row.Height
        Next
        ResumeLayout()
    End Sub

    ''' <summary>One row of the timeline (painted, with a real button on the right).</summary>
    Private Class TimelineRow
        Inherits Control

        Private ReadOnly _e As TimelineEntry
        Private ReadOnly _first As Boolean
        Private ReadOnly _last As Boolean
        Private ReadOnly _button As Button
        Public Event OpenClicked(classId As Integer)

        Public Sub New(entry As TimelineEntry, first As Boolean, last As Boolean)
            _e = entry
            _first = first
            _last = last
            SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
            Height = Theme.Dpi(76)
            BackColor = Theme.Surface
            _button = Ui.Action(If(entry.Taken, "Edit", "Take attendance"), If(entry.Taken, "secondary", "primary"),
                                If(entry.Taken, Icons.Edit, Icons.TakeAttendance), If(entry.Taken, 84, 150))
            _button.Anchor = AnchorStyles.Right Or AnchorStyles.Top
            Controls.Add(_button)
            AddHandler _button.Click, Sub() RaiseEvent OpenClicked(_e.ClassId)
        End Sub

        Protected Overrides Sub OnLayout(e As LayoutEventArgs)
            MyBase.OnLayout(e)
            _button.Location = New Point(Width - _button.Width - Theme.Dpi(4), (Height - _button.Height) \ 2)
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            Dim g = e.Graphics
            g.Clear(Theme.Surface)
            g.SmoothingMode = SmoothingMode.AntiAlias
            Dim timeW = Theme.Dpi(84)
            Dim railX = timeW + Theme.Dpi(14)
            Dim cy = Height \ 2

            TextRenderer.DrawText(g, Date.Today.Add(_e.Start).ToString("h:mm tt"), Theme.FontBold, New Rectangle(0, cy - Theme.Dpi(20), timeW, Theme.Dpi(20)),
                                  Theme.TextPrimary, TextFormatFlags.Right Or TextFormatFlags.VerticalCenter)
            TextRenderer.DrawText(g, Date.Today.Add(_e.End).ToString("h:mm tt"), Theme.FontSmall, New Rectangle(0, cy, timeW, Theme.Dpi(20)),
                                  Theme.TextMuted, TextFormatFlags.Right Or TextFormatFlags.VerticalCenter)

            ' Rail (line through every row) + dot
            Using pen As New Pen(Theme.Border, Theme.Dpi(2))
                g.DrawLine(pen, railX, If(_first, cy, 0), railX, If(_last, cy, Height))
            End Using
            Dim d = Theme.Dpi(16)
            Dim dot As New Rectangle(railX - d \ 2, cy - d \ 2, d, d)
            Dim color = If(_e.Taken, Theme.Success, Theme.Accent)
            Using br As New SolidBrush(If(_e.Taken, color, Theme.Surface)), pen As New Pen(color, Theme.Dpi(3))
                g.FillEllipse(br, dot)
                g.DrawEllipse(pen, dot)
            End Using

            Dim textX = railX + Theme.Dpi(22)
            Dim textW = Width - textX - _button.Width - Theme.Dpi(16)
            TextRenderer.DrawText(g, _e.Title, Theme.FontSubtitle, New Rectangle(textX, cy - Theme.Dpi(24), textW, Theme.Dpi(24)),
                                  Theme.TextPrimary, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
            Dim sub2 = If(_e.Taken, $"Taken  ·  {_e.Summary}", _e.Detail)
            TextRenderer.DrawText(g, sub2, Theme.FontSmall, New Rectangle(textX, cy + Theme.Dpi(2), textW, Theme.Dpi(20)),
                                  If(_e.Taken, Theme.Success, Theme.TextMuted), TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
        End Sub
    End Class

End Class
