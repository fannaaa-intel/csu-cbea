Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Windows.Forms

''' <summary>
''' Prints a table (taken from the visible columns of a DataGridView) with:
''' school header, report title, filter line, "Generated ... by ...", optional summary lines,
''' auto column widths, zebra rows, automatic landscape for wide tables, "Page X of Y",
''' and "Prepared by: ____" on the last page. Show it with PrintPreviewDialog
''' (choose "Microsoft Print to PDF" to save a PDF). No report library needed.
''' </summary>
Public Class ReportPrinter

    Private ReadOnly _title As String
    Private ReadOnly _filter As String
    Private ReadOnly _summary As List(Of String)
    Private ReadOnly _headers As New List(Of String)
    Private ReadOnly _rows As New List(Of String())
    Private ReadOnly _doc As New PrintDocument()

    Private ReadOnly fSchool As New Font("Segoe UI Semibold", 10.0F)
    Private ReadOnly fTitle As New Font("Segoe UI Semibold", 16.0F)
    Private ReadOnly fText As New Font("Segoe UI", 9.0F)
    Private ReadOnly fHeader As New Font("Segoe UI Semibold", 9.0F)
    Private ReadOnly fSmall As New Font("Segoe UI", 8.0F)

    Private _widths As Single()
    Private _rowH As Single
    Private _nextRow As Integer
    Private _page As Integer
    Private _pageCount As Integer

    Public Sub New(title As String, filterLine As String, grid As DataGridView, Optional summaryLines As IEnumerable(Of String) = Nothing)
        _title = title
        _filter = filterLine
        _summary = If(summaryLines Is Nothing, New List(Of String), summaryLines.ToList())
        Dim cols = grid.Columns.Cast(Of DataGridViewColumn)().Where(Function(c) c.Visible).OrderBy(Function(c) c.DisplayIndex).ToList()
        _headers.AddRange(cols.Select(Function(c) c.HeaderText))
        For Each r As DataGridViewRow In grid.Rows
            _rows.Add(cols.Select(Function(c) Convert.ToString(r.Cells(c.Index).FormattedValue)).ToArray())
        Next
        _doc.DocumentName = title
        _doc.DefaultPageSettings.Margins = New Margins(50, 50, 50, 50)     ' hundredths of an inch
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
            _doc.DefaultPageSettings.Landscape = natural > 750                 ' 8.5" page - margins
        End Using
    End Sub

    Private Function NaturalWidths(g As Graphics) As Single()
        Dim w(_headers.Count - 1) As Single
        For i = 0 To _headers.Count - 1
            w(i) = g.MeasureString(_headers(i), fHeader).Width
            For Each r In _rows.Take(300)
                w(i) = Math.Max(w(i), g.MeasureString(r(i), fText).Width)
            Next
            w(i) = Math.Min(w(i), 320) + 14
        Next
        Return w
    End Function

    Private Sub BeginPrint(sender As Object, e As PrintEventArgs)
        _nextRow = 0
        _page = 0
        _pageCount = 0
    End Sub

    Private Function HeaderBlockHeight() As Single
        Return 112 + _summary.Count * 17 + If(_summary.Count > 0, 10, 0)
    End Function

    Private Sub PrintPage(sender As Object, e As PrintPageEventArgs)
        Dim g = e.Graphics
        Dim area As RectangleF = e.MarginBounds
        _page += 1
        Dim y = area.Top

        ' Column widths: natural widths scaled to fill the printable width
        If _widths Is Nothing OrElse _page = 1 Then
            Dim natural = NaturalWidths(g)
            Dim scale = area.Width / Math.Max(1.0F, natural.Sum())
            _widths = natural.Select(Function(w) w * scale).ToArray()
            _rowH = fText.GetHeight(g) + 9
            ' Pages: first page has the big header; every page keeps room for the column header and footer
            Dim footer = 60.0F
            Dim firstRows = CInt(Math.Floor((area.Height - HeaderBlockHeight() - _rowH - footer) / _rowH))
            Dim otherRows = CInt(Math.Floor((area.Height - 30 - _rowH - footer) / _rowH))
            Dim rest = Math.Max(0, _rows.Count - firstRows)
            _pageCount = 1 + CInt(Math.Ceiling(rest / Math.Max(1, otherRows)))
        End If

        If _page = 1 Then
            g.DrawString(AppSettings.SchoolName.ToUpper(), fSchool, Brushes.DimGray, area.Left, y)
            y += 20
            g.DrawString(_title, fTitle, Brushes.Black, area.Left, y)
            y += 34
            g.DrawString(_filter, fText, Brushes.Black, area.Left, y)
            y += 18
            g.DrawString($"Generated {Date.Now:MMMM d, yyyy h:mm tt} by {Session.User.FullName} ({Session.User.Role})", fSmall, Brushes.DimGray, area.Left, y)
            y += 26
            For Each line In _summary
                g.DrawString(line, fText, Brushes.Black, area.Left, y)
                y += 17
            Next
            If _summary.Count > 0 Then y += 10
            y = area.Top + HeaderBlockHeight()
        Else
            g.DrawString($"{_title} (continued)", fHeader, Brushes.DimGray, area.Left, y)
            y += 30
        End If

        ' Column header row
        Using headBrush As New SolidBrush(Theme.AccentSoft), linePen As New Pen(Theme.Accent, 1.5F)
            g.FillRectangle(headBrush, area.Left, y, area.Width, _rowH)
            g.DrawLine(linePen, area.Left, y + _rowH, area.Right, y + _rowH)
        End Using
        DrawRow(g, _headers.ToArray(), area.Left, y, fHeader, Brushes.Black)
        y += _rowH

        ' Data rows with zebra striping
        Dim zebra As New SolidBrush(Color.FromArgb(246, 246, 248))
        While _nextRow < _rows.Count AndAlso y + _rowH <= area.Bottom - 60
            If _nextRow Mod 2 = 1 Then g.FillRectangle(zebra, area.Left, y, area.Width, _rowH)
            DrawRow(g, _rows(_nextRow), area.Left, y, fText, Brushes.Black)
            y += _rowH
            _nextRow += 1
        End While
        zebra.Dispose()
        If _rows.Count = 0 Then
            g.DrawString("No records for the selected filters.", fText, Brushes.DimGray, area.Left + 4, y + 8)
        End If

        ' Footer: page number on every page, signature line on the last page
        e.HasMorePages = _nextRow < _rows.Count
        Dim fy = area.Bottom - 22
        If Not e.HasMorePages Then
            g.DrawString($"Total records: {_rows.Count}", fSmall, Brushes.DimGray, area.Left, fy - 30)
            g.DrawString("Prepared by: ______________________________", fText, Brushes.Black, area.Left, fy)
        End If
        Dim pageText = $"Page {_page} of {Math.Max(_page, _pageCount)}"
        Dim pw = g.MeasureString(pageText, fSmall).Width
        g.DrawString(pageText, fSmall, Brushes.DimGray, area.Right - pw, fy + 2)
    End Sub

    Private Sub DrawRow(g As Graphics, cells As String(), left As Single, top As Single, font As Font, brush As Brush)
        Dim x = left
        Using fmt As New StringFormat With {.Trimming = StringTrimming.EllipsisCharacter, .FormatFlags = StringFormatFlags.NoWrap, .LineAlignment = StringAlignment.Center}
            For i = 0 To cells.Length - 1
                g.DrawString(cells(i), font, brush, New RectangleF(x + 5, top, _widths(i) - 8, _rowH), fmt)
                x += _widths(i)
            Next
        End Using
    End Sub

End Class
