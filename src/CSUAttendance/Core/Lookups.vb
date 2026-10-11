Imports System.Windows.Forms

''' <summary>Shared combo-box data: terms, sections and classes (teachers only get their own classes).</summary>
Public Module Lookups

    Public Function ActiveTermId() As Integer
        Return Db.ToInt(Db.Scalar("SELECT term_id FROM school_terms WHERE is_active = 1 LIMIT 1"))
    End Function

    ''' <summary>"1st Semester 2026-2027" for the active term, or "No active term".</summary>
    Public Function ActiveTermLabel() As String
        Dim s = Db.ToStr(Db.Scalar("SELECT CONCAT(semester, ' ', school_year) FROM school_terms WHERE is_active = 1 LIMIT 1"))
        Return If(s = "", "No active term", s)
    End Function

    ''' <summary>Fills a combo with all terms (newest first) and selects the active one.</summary>
    Public Sub BindTerms(cbo As ComboBox)
        Dim dt = Db.Query("SELECT term_id, CONCAT(semester, ' ', school_year, IF(is_active = 1, '  (active)', '')) AS label " &
                          "FROM school_terms ORDER BY start_date DESC")
        Bind(cbo, dt, "term_id", "label")
        Dim active = ActiveTermId()
        If active > 0 Then cbo.SelectedValue = active Else If dt.Rows.Count > 0 Then cbo.SelectedIndex = 0
    End Sub

    ''' <summary>Fills a combo with sections; when withAll, the first item is "All sections" (value 0).</summary>
    Public Sub BindSections(cbo As ComboBox, Optional withAll As Boolean = True)
        Dim dt = Db.Query("SELECT section_id, section_name FROM sections ORDER BY section_name")
        If withAll Then
            Dim all = dt.NewRow()
            all("section_id") = 0
            all("section_name") = "All sections"
            dt.Rows.InsertAt(all, 0)
        End If
        Bind(cbo, dt, "section_id", "section_name")
        If dt.Rows.Count > 0 Then cbo.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' Classes of a term as "AIS 311 · BSAIS 3B · Bautista". Teachers only see their own classes.
    ''' Extra columns: start_time, end_time, days, section_id, teacher_id, total_hours.
    ''' </summary>
    Public Function ClassesFor(termId As Integer) As DataTable
        Return Db.Query(
            "SELECT c.class_id, CONCAT(sub.subject_code, '  ·  ', sec.section_name, '  ·  ', t.last_name) AS label, " &
            "       c.start_time, c.end_time, c.days, c.section_id, c.teacher_id, c.total_hours, sub.subject_code, sec.section_name " &
            "FROM classes c JOIN subjects sub ON sub.subject_id = c.subject_id " &
            "JOIN sections sec ON sec.section_id = c.section_id JOIN teachers t ON t.teacher_id = c.teacher_id " &
            "WHERE c.term_id = @term AND c.is_active = 1 AND (@tch = 0 OR c.teacher_id = @tch) " &
            "ORDER BY sub.subject_code, sec.section_name",
            Db.P("@term", termId), Db.P("@tch", If(Session.IsTeacher, Session.User.TeacherId, 0)))
    End Function

    Public Sub BindClasses(cbo As ComboBox, termId As Integer)
        Bind(cbo, ClassesFor(termId), "class_id", "label")
    End Sub

    Public Sub Bind(cbo As ComboBox, dt As DataTable, valueMember As String, displayMember As String)
        If cbo.BindingContext Is Nothing Then cbo.BindingContext = New BindingContext()   ' combos not placed on a form yet
        cbo.DataSource = Nothing
        cbo.DisplayMember = displayMember
        cbo.ValueMember = valueMember
        cbo.DataSource = dt
        cbo.SelectedIndex = -1
    End Sub

    ''' <summary>The selected value as Integer (0 when nothing is selected).</summary>
    Public Function SelectedId(cbo As ComboBox) As Integer
        If cbo.SelectedIndex < 0 OrElse cbo.SelectedValue Is Nothing Then Return 0
        Return Db.ToInt(cbo.SelectedValue)
    End Function

    ''' <summary>The selected row of a data-bound combo (Nothing when nothing is selected).</summary>
    Public Function SelectedRow(cbo As ComboBox) As DataRow
        Dim v = TryCast(cbo.SelectedItem, DataRowView)
        Return If(v Is Nothing, Nothing, v.Row)
    End Function

End Module
