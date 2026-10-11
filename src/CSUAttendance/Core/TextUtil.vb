Imports System.Text.RegularExpressions

''' <summary>Small text helpers shared by the add/edit dialogs (name formatting, blank → NULL).</summary>
Public Module TextUtil

    ''' <summary>"Maria Elena", "Reyes", "Bautista" → "Maria Elena R. Bautista" (stored in users.full_name).</summary>
    Public Function FullName(first As String, middle As String, last As String) As String
        Dim mi = If(middle.Trim() = "", "", " " & middle.Trim().Substring(0, 1).ToUpper() & ".")
        Return $"{first.Trim()}{mi} {last.Trim()}"
    End Function

    ''' <summary>Trimmed text, or Nothing (saved as NULL) when blank.</summary>
    Public Function OrNull(text As String) As Object
        Dim t = If(text, "").Trim()
        Return If(t = "", Nothing, t)
    End Function

    ''' <summary>True for a student number like 24-1234.</summary>
    Public Function IsStudentNo(text As String) As Boolean
        Return Regex.IsMatch(text.Trim(), "^\d{2}-\d{4}$")
    End Function

    ''' <summary>SQL expression for "Last, First M." given a table alias t (code, never user input).</summary>
    Public Function SortableNameSql(t As String) As String
        Return $"CONCAT({t}.last_name, ', ', {t}.first_name, " &
               $"IF({t}.middle_name IS NULL OR {t}.middle_name = '', '', CONCAT(' ', LEFT({t}.middle_name, 1), '.')))"
    End Function

End Module
