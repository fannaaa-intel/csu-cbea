Imports System.IO
Imports System.Text
Imports System.Windows.Forms

''' <summary>
''' Saves the visible columns of a grid as CSV that opens correctly in Excel:
''' UTF-8 with BOM (so ñ and · show properly), every field quoted, quotes doubled.
''' </summary>
Public Module CsvExporter

    ''' <summary>Asks where to save, writes the file and returns the path ("" when cancelled).</summary>
    Public Function Export(grid As DataGridView, suggestedName As String, owner As IWin32Window) As String
        Dim safeName = String.Join("_", suggestedName.Split(Path.GetInvalidFileNameChars()))
        Using dlg As New SaveFileDialog With {.Filter = "CSV file (*.csv)|*.csv", .FileName = $"{safeName} {Date.Today:yyyy-MM-dd}.csv",
                                              .Title = "Export to CSV", .OverwritePrompt = True}
            If dlg.ShowDialog(owner) <> DialogResult.OK Then Return ""
            Dim cols = grid.Columns.Cast(Of DataGridViewColumn)().Where(Function(c) c.Visible).OrderBy(Function(c) c.DisplayIndex).ToList()
            Dim sb As New StringBuilder()
            sb.AppendLine(String.Join(",", cols.Select(Function(c) Quote(c.HeaderText))))
            For Each r As DataGridViewRow In grid.Rows
                sb.AppendLine(String.Join(",", cols.Select(Function(c) Quote(Convert.ToString(r.Cells(c.Index).FormattedValue)))))
            Next
            File.WriteAllText(dlg.FileName, sb.ToString(), New UTF8Encoding(True))
            Return dlg.FileName
        End Using
    End Function

    Private Function Quote(value As String) As String
        Return """" & If(value, "").Replace("""", """""") & """"
    End Function

End Module
