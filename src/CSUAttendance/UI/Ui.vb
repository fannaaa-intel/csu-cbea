Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Small builders for pages that are laid out in code (Take Attendance, dashboards, Enrollment...).
''' Every size here is DPI-scaled, so code-built screens match the designer-built ones at 125%/150%.
''' Docking rule: add the Fill control first, then the Top/Bottom ones.
''' </summary>
Public Module Ui

    ''' <summary>Toolbar strip (Dock Top, 52 px) with its own left-to-right flow for filters.</summary>
    Public Function Toolbar(Optional height As Integer = 52) As Panel
        Return New Panel With {.Dock = DockStyle.Top, .Height = Theme.Dpi(height), .BackColor = Theme.Background}
    End Function

    ''' <summary>Horizontal row of controls (no wrapping) - for toolbars and card headers.</summary>
    Public Function Row(Optional dock As DockStyle = DockStyle.Left) As FlowLayoutPanel
        Return New FlowLayoutPanel With {.Dock = dock, .AutoSize = True, .AutoSizeMode = AutoSizeMode.GrowAndShrink,
                                         .WrapContents = False, .FlowDirection = If(dock = DockStyle.Right, FlowDirection.RightToLeft, FlowDirection.LeftToRight),
                                         .BackColor = Color.Transparent, .Margin = Padding.Empty, .Padding = Padding.Empty}
    End Function

    Public Function Card(Optional dock As DockStyle = DockStyle.Fill, Optional pad As Integer = 16) As CardPanel
        Dim c As New CardPanel With {.Dock = dock}
        c.Padding = New Padding(Theme.Dpi(pad))
        Return c
    End Function

    ''' <summary>Transparent spacer used between docked cards.</summary>
    Public Function Gap(dock As DockStyle, Optional size As Integer = 16) As Panel
        Dim p As New Panel With {.Dock = dock, .BackColor = Color.Transparent}
        If dock = DockStyle.Left OrElse dock = DockStyle.Right Then p.Width = Theme.Dpi(size) Else p.Height = Theme.Dpi(size)
        Return p
    End Function

    Public Function Caption(value As String, Optional font As Font = Nothing, Optional color As Color = Nothing) As Label
        Return New Label With {.Text = value, .AutoSize = True, .Font = If(font, Theme.FontBase),
                               .ForeColor = If(color.IsEmpty, Theme.TextPrimary, color), .BackColor = Color.Transparent,
                               .UseMnemonic = False, .Margin = New Padding(0, 0, Theme.Dpi(12), 0)}
    End Function

    Public Function Muted(value As String) As Label
        Return Caption(value, Theme.FontSmall, Theme.TextMuted)
    End Function

    ''' <summary>Card title row: bold title on the left, muted note on the right.</summary>
    Public Function CardHeader(title As String, ByRef note As Label, Optional height As Integer = 40) As Panel
        Dim p As New Panel With {.Dock = DockStyle.Top, .Height = Theme.Dpi(height), .BackColor = Color.Transparent}
        Dim t = Caption(title, Theme.FontSubtitle)
        t.AutoSize = True
        t.Dock = DockStyle.Left
        note = Muted("")
        note.AutoSize = False
        note.TextAlign = ContentAlignment.MiddleRight
        note.Dock = DockStyle.Fill
        p.Controls.Add(note)                 ' Fill first, then the docked title
        p.Controls.Add(t)
        Return p
    End Function

    Public Function Combo(width As Integer) As ComboBox
        Dim c As New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList, .Width = Theme.Dpi(width),
                                    .Margin = New Padding(0, Theme.Dpi(5), Theme.Dpi(12), 0)}
        Theme.StyleCombo(c)
        Return c
    End Function

    Public Function Search(placeholder As String, width As Integer) As TextField
        Return New TextField With {.Placeholder = placeholder, .Width = Theme.Dpi(width), .Height = Theme.Dpi(36),
                                   .Margin = New Padding(0, 0, Theme.Dpi(12), 0)}
    End Function

    Public Function DatePicker(width As Integer) As DateTimePicker
        Return New DateTimePicker With {.Format = DateTimePickerFormat.Custom, .CustomFormat = "MMM d, yyyy", .Width = Theme.Dpi(width),
                                        .Font = Theme.FontBase, .Margin = New Padding(0, Theme.Dpi(5), Theme.Dpi(12), 0)}
    End Function

    ''' <summary>A styled button. kind: primary | secondary | danger | ghost.</summary>
    Public Function Action(caption As String, kind As String, Optional glyph As String = Nothing, Optional minWidth As Integer = 96) As Button
        Dim b As New Button With {.Text = caption, .Margin = New Padding(Theme.Dpi(8), 0, 0, 0)}
        Select Case kind
            Case "primary" : Theme.StylePrimary(b, glyph)
            Case "danger" : Theme.StyleDanger(b, glyph)
            Case "ghost" : Theme.StyleGhost(b, glyph)
            Case Else : Theme.StyleSecondary(b, glyph)
        End Select
        Theme.FitWidth(b, minWidth)
        Return b
    End Function

    ''' <summary>A styled read-only grid docked to fill its card.</summary>
    Public Function Grid(Optional multiSelect As Boolean = False) As DataGridView
        Dim g As New DataGridView With {.Dock = DockStyle.Fill}
        Theme.StyleGrid(g, multiSelect)
        Return g
    End Function

    ''' <summary>Centered muted message shown inside an empty grid, under its column headers (toggle .Visible).</summary>
    Public Function EmptyState(grid As DataGridView, message As String) As Label
        Dim l As New Label With {.Text = message, .AutoSize = False, .TextAlign = ContentAlignment.MiddleCenter, .Font = Theme.FontBase,
                                 .ForeColor = Theme.TextMuted, .BackColor = Theme.Surface, .Visible = False, .UseMnemonic = False}
        grid.Controls.Add(l)
        Dim place = Sub() l.SetBounds(0, grid.ColumnHeadersHeight + Theme.Dpi(24), grid.ClientSize.Width, Theme.Dpi(72))
        AddHandler grid.Resize, Sub() place()
        AddHandler l.VisibleChanged, Sub() place()
        place()
        Return l
    End Function

    ''' <summary>Sets header text + relative width for a grid column (if it exists).</summary>
    Public Sub Col(g As DataGridView, name As String, header As String, weight As Integer, Optional format As String = Nothing)
        If Not g.Columns.Contains(name) Then Return
        g.Columns(name).HeaderText = header
        g.Columns(name).FillWeight = weight
        If format IsNot Nothing Then g.Columns(name).DefaultCellStyle.Format = format
    End Sub

    Public Sub Hide(g As DataGridView, ParamArray names As String())
        For Each n In names
            If g.Columns.Contains(n) Then g.Columns(n).Visible = False
        Next
    End Sub

    ''' <summary>Dashboard greeting: "Good morning, Maria" + a muted line (date · term). Plain type, no banner.</summary>
    Public Function Greeting(firstName As String, subtitle As String) As Panel
        Dim p As New Panel With {.Dock = DockStyle.Top, .Height = Theme.Dpi(76), .BackColor = Theme.Background}
        Dim hour = Date.Now.Hour
        Dim hello = If(hour < 12, "Good morning", If(hour < 18, "Good afternoon", "Good evening"))
        Dim t = Caption($"{hello}, {firstName}", Theme.FontTitle)
        t.Location = New Point(0, 0)
        Dim s = Muted(subtitle)
        s.Font = Theme.FontBase
        s.Location = New Point(Theme.Dpi(2), Theme.Dpi(40))
        p.Controls.Add(t)
        p.Controls.Add(s)
        p.Tag = s                           ' the subtitle label, so pages can fill it in later
        Return p
    End Function

    ''' <summary>Equal-width row of controls (e.g. stat cards) with a gap between them.</summary>
    Public Function EqualRow(height As Integer, ParamArray items As Control()) As TableLayoutPanel
        Dim t As New TableLayoutPanel With {.Dock = DockStyle.Top, .Height = Theme.Dpi(height), .ColumnCount = items.Length, .RowCount = 1,
                                            .BackColor = Theme.Background, .Margin = Padding.Empty, .Padding = Padding.Empty}
        For i = 0 To items.Length - 1
            t.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F / items.Length))
            items(i).Dock = DockStyle.Fill
            items(i).Margin = New Padding(If(i = 0, 0, Theme.Dpi(8)), 0, If(i = items.Length - 1, 0, Theme.Dpi(8)), 0)
            t.Controls.Add(items(i), i, 0)
        Next
        Return t
    End Function

    ''' <summary>A compact list grid (no column headers) for dashboard cards.</summary>
    Public Function ListGrid() As DataGridView
        Dim g = Grid()
        g.ColumnHeadersVisible = False
        g.RowTemplate.Height = Theme.Dpi(38)
        g.DefaultCellStyle.SelectionBackColor = Theme.Surface
        g.AlternatingRowsDefaultCellStyle.BackColor = Theme.Surface
        g.TabStop = False
        Return g
    End Function

    ''' <summary>First name from "Maria Elena R. Bautista" → "Maria".</summary>
    Public Function FirstName(fullName As String) As String
        Dim parts = fullName.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
        Return If(parts.Length = 0, "", parts(0))
    End Function

    ''' <summary>"1 student" / "3 students".</summary>
    Public Function Plural(n As Integer, word As String, Optional pluralWord As String = Nothing) As String
        Return $"{n} {If(n = 1, word, If(pluralWord, word & "s"))}"
    End Function

End Module
