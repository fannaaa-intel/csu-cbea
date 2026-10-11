Imports System.Data
Imports MySqlConnector

''' <summary>
''' Thin data-access helper around MySqlConnector.
''' RULE: always pass values as parameters with Db.P("@name", value) — never concatenate user input into SQL.
''' Type notes (MySqlConnector): DATE/DATETIME -> Date, TIME -> TimeSpan, DECIMAL -> Decimal,
''' TINYINT(1) -> Boolean, COUNT()/SUM() -> Long/Decimal, SET -> String ("Mon,Wed").
''' </summary>
Public Module Db

    Public Property ConnectionString As String = ""

    Public Function GetConnection() As MySqlConnection
        Dim cn As New MySqlConnection(ConnectionString)
        cn.Open()
        Return cn
    End Function

    ''' <summary>Creates a parameter; Nothing becomes DBNull.</summary>
    Public Function P(name As String, value As Object) As MySqlParameter
        Return New MySqlParameter(name, If(value, DBNull.Value))
    End Function

    Public Function TestConnection(ByRef errorMessage As String) As Boolean
        Try
            Using cn = GetConnection()
                Return True
            End Using
        Catch ex As Exception
            errorMessage = ex.Message
            Return False
        End Try
    End Function

    ' ------------------------------------------------------------------
    ' Simple calls (each opens and closes its own connection)
    ' ------------------------------------------------------------------
    Public Function Query(sql As String, ParamArray ps As MySqlParameter()) As DataTable
        Using cn = GetConnection()
            Return Query(cn, Nothing, sql, ps)
        End Using
    End Function

    Public Function Execute(sql As String, ParamArray ps As MySqlParameter()) As Integer
        Using cn = GetConnection()
            Return Execute(cn, Nothing, sql, ps)
        End Using
    End Function

    ''' <summary>Returns the first column of the first row, or Nothing when there is no row / NULL.</summary>
    Public Function Scalar(sql As String, ParamArray ps As MySqlParameter()) As Object
        Using cn = GetConnection()
            Return Scalar(cn, Nothing, sql, ps)
        End Using
    End Function

    ''' <summary>Runs an INSERT and returns the new AUTO_INCREMENT id.</summary>
    Public Function Insert(sql As String, ParamArray ps As MySqlParameter()) As Integer
        Using cn = GetConnection()
            Return Insert(cn, Nothing, sql, ps)
        End Using
    End Function

    ' ------------------------------------------------------------------
    ' Transaction-aware overloads (use inside Db.InTransaction)
    ' ------------------------------------------------------------------
    Public Function Query(cn As MySqlConnection, tx As MySqlTransaction, sql As String, ParamArray ps As MySqlParameter()) As DataTable
        Using cmd = BuildCommand(cn, tx, sql, ps)
            Using da As New MySqlDataAdapter(cmd)
                Dim dt As New DataTable()
                da.Fill(dt)
                Return dt
            End Using
        End Using
    End Function

    Public Function Execute(cn As MySqlConnection, tx As MySqlTransaction, sql As String, ParamArray ps As MySqlParameter()) As Integer
        Using cmd = BuildCommand(cn, tx, sql, ps)
            Return cmd.ExecuteNonQuery()
        End Using
    End Function

    Public Function Scalar(cn As MySqlConnection, tx As MySqlTransaction, sql As String, ParamArray ps As MySqlParameter()) As Object
        Using cmd = BuildCommand(cn, tx, sql, ps)
            Dim result = cmd.ExecuteScalar()
            If result Is Nothing OrElse TypeOf result Is DBNull Then Return Nothing
            Return result
        End Using
    End Function

    Public Function Insert(cn As MySqlConnection, tx As MySqlTransaction, sql As String, ParamArray ps As MySqlParameter()) As Integer
        Using cmd = BuildCommand(cn, tx, sql, ps)
            cmd.ExecuteNonQuery()
            Return CInt(cmd.LastInsertedId)
        End Using
    End Function

    ''' <summary>Runs work inside one transaction; commits on success, rolls back and rethrows on error.</summary>
    Public Sub InTransaction(work As Action(Of MySqlConnection, MySqlTransaction))
        Using cn = GetConnection()
            Using tx = cn.BeginTransaction()
                Try
                    work(cn, tx)
                    tx.Commit()
                Catch
                    tx.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Sub

    Private Function BuildCommand(cn As MySqlConnection, tx As MySqlTransaction, sql As String, ps As MySqlParameter()) As MySqlCommand
        Dim cmd As New MySqlCommand(sql, cn, tx)
        If ps IsNot Nothing Then
            For Each prm In ps
                cmd.Parameters.Add(prm)
            Next
        End If
        Return cmd
    End Function

    ' ------------------------------------------------------------------
    ' Safe conversions for DataRow values (handle DBNull)
    ' ------------------------------------------------------------------
    Public Function ToInt(value As Object) As Integer
        If value Is Nothing OrElse TypeOf value Is DBNull Then Return 0
        Return Convert.ToInt32(value)
    End Function

    Public Function ToDec(value As Object) As Decimal
        If value Is Nothing OrElse TypeOf value Is DBNull Then Return 0D
        Return Convert.ToDecimal(value)
    End Function

    Public Function ToStr(value As Object) As String
        If value Is Nothing OrElse TypeOf value Is DBNull Then Return ""
        Return Convert.ToString(value)
    End Function

    Public Function ToBool(value As Object) As Boolean
        If value Is Nothing OrElse TypeOf value Is DBNull Then Return False
        Return Convert.ToBoolean(value)
    End Function

    Public Function ToDate(value As Object) As Date
        If value Is Nothing OrElse TypeOf value Is DBNull Then Return Date.MinValue
        Return Convert.ToDateTime(value)
    End Function

    Public Function ToTime(value As Object) As TimeSpan
        If value Is Nothing OrElse TypeOf value Is DBNull Then Return TimeSpan.Zero
        If TypeOf value Is TimeSpan Then Return DirectCast(value, TimeSpan)
        Return TimeSpan.Parse(Convert.ToString(value))
    End Function

    ''' <summary>True when the exception is a MySQL duplicate-key error (1062).</summary>
    Public Function IsDuplicate(ex As Exception) As Boolean
        Dim my = TryCast(ex, MySqlException)
        Return my IsNot Nothing AndAlso my.Number = 1062
    End Function

    ''' <summary>True when a row cannot be deleted because other rows reference it (1451).</summary>
    Public Function IsInUse(ex As Exception) As Boolean
        Dim my = TryCast(ex, MySqlException)
        Return my IsNot Nothing AndAlso my.Number = 1451
    End Function

End Module
