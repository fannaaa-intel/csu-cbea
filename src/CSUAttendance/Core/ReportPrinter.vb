Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Printing
Imports System.Windows.Forms

''' <summary>
''' Prints a table (the visible columns of a DataGridView) as a formal university report:
''' centered letterhead with a double rule, report title, a label/value information block,
''' a fully ruled table with a "No." column, "Nothing follows", Prepared by / Noted by signatures,
''' and a footer with the print date and "Page X of Y" on every page. Black and gray only.
''' Show it with PrintPreviewDialog (choose "Microsoft Print to PDF" to save a PDF). No report library needed.
''' </summary>
Public Class ReportPrinter

    Private Const CollegeName As String = "College of Business, Entrepreneurship and Accountancy"
    Private Const SystemName As String = "CSU Student Attendance Management System"

    Private ReadOnly _title As String
    Private ReadOnly _info As New List(Of (Label As String, Value As String))
    Private ReadOnly _summary As List(Of String)
    Private ReadOnly _headers As New List(Of String)
    Private ReadOnly _rows As New List(Of String())
    Private ReadOnly _doc As New PrintDocument()
    Private ReadOnly _printedAt As Date = Date.Now

    ' Serif letterhead like printed university forms; a plain sans-serif for the table
    Private ReadOnly fRepublic As New Font("Times New Roman", 10.0F)
    Private ReadOnly fSchool As New Font("Times New Roman", 16.0F, FontStyle.Bold)
    Private ReadOnly fCollege As New Font("Times New Roman", 10.5F, FontStyle.Italic)
    Private ReadOnly fTitle As New Font("Times New Roman", 13.0F, FontStyle.Bold)
    Private ReadOnly fLabel As New Font("Arial", 9.0F, FontStyle.Bold)
    Private ReadOnly fText As New Font("Arial", 9.0F)
    Private ReadOnly fHeader As New Font("Arial", 8.5F, FontStyle.Bold)
    Private ReadOnly fSmall As New Font("Arial", 7.5F)
    Private ReadOnly fSmallItalic As New Font("Arial", 8.0F, FontStyle.Italic)

    Private _widths As Single()
    Private _rowH As Single
    Private _pages As List(Of (First As Integer, Count As Integer))
    Private _page As Integer

    Public Sub New(title As String, filterLine As String, grid As DataGridView, Optional summaryLines As IEnumerable(Of String) = Nothing)
        _title = title
        ' "Class: ACC 313    Term: 1st Semester" -> [(Class, ACC 313), (Term, 1st Semester)]
        For Each part In filterLine.Split({"    "}, StringSplitOptions.RemoveEmptyEntries)
            Dim colon = part.IndexOf(":"c)
            If colon > 0 Then _info.Add((part.Substring(0, colon).Trim(), part.Substring(colon + 1).Trim())) Else _info.Add(("", part.Trim()))
        Next
        _info.Add(("Date printed", _printedAt.ToString("MMMM d, yyyy")))
        _summary = If(summaryLines Is Nothing, New List(Of String), summaryLines.ToList())

        Dim cols = grid.Columns.Cast(Of DataGridViewColumn)().Where(Function(c) c.Visible).OrderBy(Function(c) c.DisplayIndex).ToList()
        _headers.Add("No.")
        _headers.AddRange(cols.Select(Function(c) c.HeaderText))
        Dim n = 0
        For Each r As DataGridViewRow In grid.Rows
            n += 1
            Dim cells = New List(Of String) From {n.ToString()}
            cells.AddRange(cols.Select(Function(c) Convert.ToString(r.Cells(c.Index).FormattedValue)))
            _rows.Add(cells.ToArray())
        Next
        _doc.DocumentName = title
        _doc.DefaultPageSettings.Margins = New Margins(60, 60, 50, 50)     ' hundredths of an inch
        AddHandler _doc.BeginPrint, AddressOf BeginPrint
        AddHandler _doc.PrintPage, AddressOf PrintPage
    End Sub

    ''' <summary>Opens the print preview window (maximized). The printer icon prints or saves as PDF.</summary>
    Public Sub ShowPreview(owner As IWin32Window)
        ChooseOrientation()
        Using dlg As New PrintPreviewDialog With {.Document = _doc, .WindowState = FormWindowState.Maximized, .ShowIcon = False,
                                                  .Text = $"Print preview - {_title}"}
            dlg.PrintPreviewControl.Zoom = 1.0
            dlg.ShowDialog(owner)
        End Using
    End Sub

    ''' <summary>Landscape when the natural column widths do not fit a portrait page.</summary>
    Private Sub ChooseOrientation()
        Using g = Graphics.FromHwnd(IntPtr.Zero)
            Dim natural = NaturalWidths(g).Sum() * 100.0F / g.DpiX          ' in hundredths of an inch
            _doc.DefaultPageSettings.Landscape = natural > 730                 ' 8.5" page - margins
        End Using
    End Sub

    Private Function NaturalWidths(g As Graphics) As Single()
        Dim w(_headers.Count - 1) As Single
        For i = 0 To _headers.Count - 1
            w(i) = g.MeasureString(_headers(i), fHeader).Width
            For Each r In _rows.Take(300)
                w(i) = Math.Max(w(i), g.MeasureString(r(i), fText).Width)
            Next
            w(i) = Math.Min(w(i), 300) + 14
        Next
        Return w
    End Function

    Private Sub BeginPrint(sender As Object, e As PrintEventArgs)
        _page = 0
        _pages = Nothing
    End Sub

    ' ------------------------------------------------------------------
    ' Page layout
    ' ------------------------------------------------------------------
    Private ReadOnly Property FooterHeight As Single
        Get
            Return 26
        End Get
    End Property

    Private ReadOnly Property ClosingHeight As Single     ' "Nothing follows" + total + signatures
        Get
            Return 100
        End Get
    End Property

    ''' <summary>Splits the rows into pages. The closing block (signatures) always fits on the last page.</summary>
    Private Sub Paginate(g As Graphics, area As RectangleF)
        _pages = New List(Of (Integer, Integer))
        Dim firstRoom = area.Height - FirstHeader(g, area, False) - _rowH - FooterHeight
        Dim otherRoom = area.Height - ContinuedHeader(g, area, False) - _rowH - FooterHeight
        Dim index = 0
        Dim room = firstRoom
        Do
            Dim fit = Math.Max(0, CInt(Math.Floor(room / _rowH)))
            Dim take = Math.Min(fit, _rows.Count - index)
            Dim left = room - take * _rowH
            If index + take >= _rows.Count Then
                If left >= ClosingHeight OrElse take = 0 Then
                    _pages.Add((index, take))
                Else
                    ' signatures do not fit: carry the last rows over so they never stand alone on a page
                    Dim carry = Math.Min(3, take)
                    If take - carry > 0 Then _pages.Add((index, take - carry))
                    _pages.Add((index + take - carry, carry))
                End If
                Exit Do
            End If
            _pages.Add((index, take))
            index += take
            room = otherRoom
        Loop
    End Sub

    Private Sub PrintPage(sender As Object, e As PrintPageEventArgs)
        Dim g = e.Graphics
        g.SmoothingMode = SmoothingMode.None
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit
        Dim area As RectangleF = e.MarginBounds
        If _pages Is Nothing Then
            Dim natural = NaturalWidths(g)
            Dim scale = area.Width / Math.Max(1.0F, natural.Sum())
            _widths = natural.Select(Function(w) w * scale).ToArray()
            _rowH = fText.GetHeight(g) + 7
            Paginate(g, area)
        End If
        _page += 1
        Dim pg = _pages(_page - 1)
        Dim isLast = _page = _pages.Count

        Dim y = area.Top + If(_page = 1, FirstHeader(g, area, True), ContinuedHeader(g, area, True))
        If pg.Count > 0 OrElse (_page = 1) Then
            y = DrawTable(g, area, y, pg.First, pg.Count)
        End If
        If isLast Then DrawClosing(g, area, y)
        DrawFooter(g, area)
        e.HasMorePages = Not isLast
    End Sub

    ''' <summary>Letterhead, double rule, title and the information block. Returns its height (draws only when draw = True).</summary>
    Private Function FirstHeader(g As Graphics, area As RectangleF, draw As Boolean) As Single
        Dim y = area.Top
        Using center As New StringFormat With {.Alignment = StringAlignment.Center}
            If draw Then g.DrawString("Republic of the Philippines", fRepublic, Brushes.Black, New RectangleF(area.Left, y, area.Width, 20), center)
            y += fRepublic.GetHeight(g) + 1
            If draw Then g.DrawString(AppSettings.SchoolName.ToUpper(), fSchool, Brushes.Black, New RectangleF(area.Left, y, area.Width, 30), center)
            y += fSchool.GetHeight(g) + 1
            If draw Then g.DrawString(CollegeName, fCollege, Brushes.Black, New RectangleF(area.Left, y, area.Width, 20), center)
            y += fCollege.GetHeight(g) + 8
            If draw Then
                Using thick As New Pen(Color.Black, 2.0F), thin As New Pen(Color.Black, 0.6F)
                    g.DrawLine(thick, area.Left, y, area.Right, y)
                    g.DrawLine(thin, area.Left, y + 4, area.Right, y + 4)
                End Using
            End If
            y += 18
            If draw Then g.DrawString(_title.ToUpper(), fTitle, Brushes.Black, New RectangleF(area.Left, y, area.Width, 26), center)
            y += fTitle.GetHeight(g) + 12
        End Using

        ' Information block: two label/value pairs per line
        Dim half = area.Width / 2
        Dim labelW = 0F
        For Each item In _info
            labelW = Math.Max(labelW, g.MeasureString(item.Label & ":", fLabel).Width)
        Next
        Dim lineH = fText.GetHeight(g) + 5
        For i = 0 To _info.Count - 1 Step 2
            For j = 0 To 1
                If i + j >= _info.Count Then Exit For
                Dim item = _info(i + j)
                Dim x = area.Left + j * half
                If draw Then
                    If item.Label <> "" Then g.DrawString(item.Label & ":", fLabel, Brushes.Black, x, y)
                    Using fmt As New StringFormat With {.Trimming = StringTrimming.EllipsisCharacter, .FormatFlags = StringFormatFlags.NoWrap}
                        g.DrawString(item.Value, fText, Brushes.Black, New RectangleF(x + labelW + 6, y, half - labelW - 16, lineH), fmt)
                    End Using
                End If
            Next
            y += lineH
        Next
        If _summary.Count > 0 Then
            y += 4
            For Each line In _summary
                If draw Then
                    If line Is _summary(0) Then g.DrawString("Summary:", fLabel, Brushes.Black, area.Left, y)
                    g.DrawString(line.Replace("    ", "   |   "), fText, Brushes.Black, area.Left + labelW + 6, y)
                End If
                y += lineH
            Next
        End If
        Return y + 12 - area.Top
    End Function

    Private Function ContinuedHeader(g As Graphics, area As RectangleF, draw As Boolean) As Single
        If draw Then
            g.DrawString($"{AppSettings.SchoolName.ToUpper()}  -  {_title}", fLabel, Brushes.Black, area.Left, area.Top)
            Dim cont = "(continued)"
            g.DrawString(cont, fSmallItalic, Brushes.Black, area.Right - g.MeasureString(cont, fSmallItalic).Width, area.Top + 1)
            Using pen As New Pen(Color.Black, 0.6F)
                g.DrawLine(pen, area.Left, area.Top + 20, area.Right, area.Top + 20)
            End Using
        End If
        Return 34
    End Function

    ''' <summary>Ruled table: gray header row, thin black grid, numbers centered. Returns the y below it.</summary>
    Private Function DrawTable(g As Graphics, area As RectangleF, top As Single, first As Integer, count As Integer) As Single
        Dim y = top
        Using headBrush As New SolidBrush(Color.FromArgb(225, 225, 225))
            g.FillRectangle(headBrush, area.Left, y, area.Width, _rowH)
        End Using
        DrawCells(g, _headers.ToArray(), area.Left, y, fHeader, True)
        y += _rowH
        For i = first To first + count - 1
            DrawCells(g, _rows(i), area.Left, y, fText, False)
            y += _rowH
        Next
        If _rows.Count = 0 Then
            Using center As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                g.DrawString("No records for the selected filters.", fSmallItalic, Brushes.Black, New RectangleF(area.Left, y, area.Width, _rowH), center)
            End Using
            y += _rowH
        End If

        ' Grid lines: outer box (heavier) + every row and column
        Using thin As New Pen(Color.Black, 0.5F), outer As New Pen(Color.Black, 1.0F)
            Dim x = area.Left
            For Each w In _widths.Take(_widths.Length - 1)
                x += w
                g.DrawLine(thin, x, top, x, y)
            Next
            For ry = top + _rowH To y - 1 Step _rowH
                g.DrawLine(thin, area.Left, ry, area.Right, ry)
            Next
            g.DrawRectangle(outer, area.Left, top, area.Width, y - top)
            g.DrawLine(outer, area.Left, top + _rowH, area.Right, top + _rowH)
        End Using
        Return y
    End Function

    Private Sub DrawCells(g As Graphics, cells As String(), left As Single, top As Single, font As Font, isHeader As Boolean)
        Dim x = left
        For i = 0 To cells.Length - 1
            Dim number = i = 0 OrElse IsNumber(cells(i))
            Using fmt As New StringFormat With {.Trimming = StringTrimming.EllipsisCharacter, .FormatFlags = StringFormatFlags.NoWrap,
                                                .LineAlignment = StringAlignment.Center,
                                                .Alignment = If(isHeader OrElse number, StringAlignment.Center, StringAlignment.Near)}
                g.DrawString(cells(i), font, Brushes.Black, New RectangleF(x + 4, top, _widths(i) - 8, _rowH), fmt)
            End Using
            x += _widths(i)
        Next
    End Sub

    Private Shared Function IsNumber(value As String) As Boolean
        Dim d As Decimal
        Return value <> "" AndAlso Decimal.TryParse(value.TrimEnd("%"c), d)
    End Function

    ''' <summary>"Nothing follows", total, and the Prepared by / Noted by signature blocks.</summary>
    Private Sub DrawClosing(g As Graphics, area As RectangleF, top As Single)
        Dim y = top + 8
        Using center As New StringFormat With {.Alignment = StringAlignment.Center}
            g.DrawString("*** Nothing follows ***", fSmallItalic, Brushes.Black, New RectangleF(area.Left, y, area.Width, 16), center)
        End Using
        Dim total = $"Total records: {_rows.Count}"
        g.DrawString(total, fText, Brushes.Black, area.Right - g.MeasureString(total, fText).Width, y)
        y += 22

        Dim blockW = Math.Min(260.0F, area.Width / 2 - 40)
        Dim preparedRole = If(Session.IsAdmin, "System Administrator", If(Session.IsTeacher, "Instructor", Session.User.Role))
        DrawSignature(g, "Prepared by:", Session.User.FullName.ToUpper(), preparedRole, area.Left, y, blockW)
        DrawSignature(g, "Noted by:", "", "Department Chairperson", area.Right - blockW, y, blockW)
    End Sub

    Private Sub DrawSignature(g As Graphics, caption As String, name As String, role As String, x As Single, y As Single, width As Single)
        g.DrawString(caption, fText, Brushes.Black, x, y)
        Dim lineY = y + 34
        Using pen As New Pen(Color.Black, 0.6F), center As New StringFormat With {.Alignment = StringAlignment.Center}
            If name <> "" Then g.DrawString(name, fLabel, Brushes.Black, New RectangleF(x, lineY - fLabel.GetHeight(g) - 1, width, 18), center)
            g.DrawLine(pen, x, lineY, x + width, lineY)
            g.DrawString(role, fText, Brushes.Black, New RectangleF(x, lineY + 3, width, 16), center)
            g.DrawString("Signature over printed name", fSmall, Brushes.DimGray, New RectangleF(x, lineY + 18, width, 14), center)
        End Using
    End Sub

    Private Sub DrawFooter(g As Graphics, area As RectangleF)
        Dim y = area.Bottom - 16
        Using pen As New Pen(Color.Gray, 0.5F)
            g.DrawLine(pen, area.Left, y - 4, area.Right, y - 4)
        End Using
        g.DrawString($"{SystemName}  |  Printed {_printedAt:MMMM d, yyyy h:mm tt} by {Session.User.FullName}", fSmall, Brushes.DimGray, area.Left, y)
        Dim pageText = $"Page {_page} of {_pages.Count}"
        g.DrawString(pageText, fSmall, Brushes.DimGray, area.Right - g.MeasureString(pageText, fSmall).Width, y)
    End Sub

End Class
