Imports System.Text.RegularExpressions
Imports System.Windows.Forms

''' <summary>Add/edit a school term. id = 0 → add. The first term ever added becomes the active term.</summary>
Public Class TermDialog

    Private ReadOnly _id As Integer

    Public Sub New()
        Me.New(0)
    End Sub

    Public Sub New(termId As Integer)
        InitializeComponent()
        _id = termId
    End Sub

    Private Sub TermDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Theme.StylePrimary(btnSave)
        Theme.StyleSecondary(btnCancel)
        Theme.StyleCombo(cboSemester)

        If _id = 0 Then
            ' Sensible defaults: the school year that contains today, 1st semester
            Dim y = If(Date.Today.Month >= 6, Date.Today.Year, Date.Today.Year - 1)
            txtYear.Text = $"{y}-{y + 1}"
            cboSemester.SelectedIndex = 0
            dtpStart.Value = Date.Today
            dtpEnd.Value = Date.Today.AddMonths(4)
            Return
        End If

        lblTitle.Text = "Edit school term"
        Dim dt = Db.Query("SELECT school_year, semester, start_date, end_date FROM school_terms WHERE term_id = @id", Db.P("@id", _id))
        If dt.Rows.Count = 0 Then
            DialogResult = DialogResult.Cancel
            Return
        End If
        Dim r = dt.Rows(0)
        txtYear.Text = Db.ToStr(r("school_year"))
        cboSemester.SelectedItem = Db.ToStr(r("semester"))
        dtpStart.Value = Db.ToDate(r("start_date"))
        dtpEnd.Value = Db.ToDate(r("end_date"))
    End Sub

    Private Function ValidateInputs() As Boolean
        errProvider.Clear()
        Dim ok = True
        Dim m = Regex.Match(txtYear.Text.Trim(), "^(\d{4})-(\d{4})$")
        If Not m.Success OrElse CInt(m.Groups(2).Value) <> CInt(m.Groups(1).Value) + 1 Then
            errProvider.SetError(txtYear, "Use the format 2026-2027 (second year = first year + 1).")
            ok = False
        End If
        If cboSemester.SelectedIndex < 0 Then
            errProvider.SetError(cboSemester, "Choose a semester.")
            ok = False
        End If
        If dtpEnd.Value.Date <= dtpStart.Value.Date Then
            errProvider.SetError(dtpEnd, "End date must be after the start date.")
            ok = False
        End If
        Return ok
    End Function

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Not ValidateInputs() Then Return
        Dim year = txtYear.Text.Trim()
        Dim semester = CStr(cboSemester.SelectedItem)
        Dim label = $"{semester} {year}"
        Try
            If _id = 0 Then
                ' If no term is active yet (fresh system), make this one active.
                Dim hasActive = Db.ToInt(Db.Scalar("SELECT COUNT(*) FROM school_terms WHERE is_active = 1")) > 0
                Dim newId = Db.Insert("INSERT INTO school_terms (school_year, semester, start_date, end_date, is_active) VALUES (@y, @s, @a, @b, @act)",
                                      Db.P("@y", year), Db.P("@s", semester), Db.P("@a", dtpStart.Value.Date),
                                      Db.P("@b", dtpEnd.Value.Date), Db.P("@act", If(hasActive, 0, 1)))
                Audit.Log("CREATE", "Term", newId, label)
            Else
                Db.Execute("UPDATE school_terms SET school_year = @y, semester = @s, start_date = @a, end_date = @b WHERE term_id = @id",
                           Db.P("@y", year), Db.P("@s", semester), Db.P("@a", dtpStart.Value.Date),
                           Db.P("@b", dtpEnd.Value.Date), Db.P("@id", _id))
                Audit.Log("UPDATE", "Term", _id, label)
            End If
            DialogResult = DialogResult.OK
        Catch ex As Exception When Db.IsDuplicate(ex)
            errProvider.SetError(cboSemester, $"{label} already exists.")
        Catch ex As Exception
            MessageBox.Show(Me, "Could not save the term." & Environment.NewLine & Environment.NewLine & ex.Message,
                            "Something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class
