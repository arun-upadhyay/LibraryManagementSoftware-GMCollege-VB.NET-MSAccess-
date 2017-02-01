Imports System.Data.OleDb
Imports System.IO
Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Security.Cryptography
Imports System.Text
Public Class BookSerialEntry
    Dim rdr As OleDbDataReader = Nothing
    Dim dtable As DataTable
    Dim con As OleDbConnection = Nothing
    Dim adp As OleDbDataAdapter
    Dim ds As DataSet
    Dim cmd As OleDbCommand = Nothing
    Dim dt As New DataTable
    Dim cs As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\LMS_DB.accdb;Persist Security Info=False;"


    Private Sub BookSerialEntry_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        DataGridView2.Visible = False
    End Sub
  
    Public Sub loadGrid()

        Try
            con = New OleDbConnection(cs)
            con.Open()
            Dim sqlq = "SELECT AccessionNo, BookTitle, Author, Subject, Department from Book where BookTitle like '" & txtBookTitle.Text & "%'"
            cmd = New OleDbCommand(sqlq, con)
            Dim myDA As OleDbDataAdapter = New OleDbDataAdapter(cmd)
            Dim myDataSet As DataSet = New DataSet()
            myDA.Fill(myDataSet, "BookSearch")
            DataGridView2.DataSource = myDataSet.Tables("BookSearch").DefaultView
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub cmbAccNo_SelectedIndexChanged(sender As System.Object, e As System.EventArgs)
        loadGrid()
    End Sub

    

    Private Sub txtBookTitle_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles txtBookTitle.KeyUp
        If ((e.KeyCode = Keys.Enter) Or (e.KeyCode = Keys.Return)) And txtBookTitle.Text <> "" Then

            DataGridView2.Focus()

        End If

    End Sub

    Private Sub txtBookTitle_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtBookTitle.TextChanged
        DataGridView2.Visible = True
        loadGrid()
    End Sub

    Private Sub DataGridView2_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles DataGridView2.KeyDown
        If e.KeyCode = Keys.Enter Then

            e.SuppressKeyPress = True
            add2Grid1()

        End If
    End Sub

    Private Sub DataGridView2_RowHeaderMouseClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles DataGridView2.RowHeaderMouseClick
        add2Grid1()


    End Sub
    Public Sub add2Grid1()
        Try
            txtAccessionNo.Text = DataGridView2.SelectedRows(0).Cells("AccessionNo").Value.ToString()
            txtBookTitle.Text = DataGridView2.SelectedRows(0).Cells("BookTitle").Value.ToString()
            DataGridView2.Hide()
            btnNew.Enabled = True
            btnSave.Enabled = True
            txtBookCode.Text = ""
            lblBookID.Text = ""

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub txtAccessionNo_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtAccessionNo.TextChanged
        Dim sqlq = "SELECT BID as [Book ID], Code as [Book Code] from BookCode where AccessionNo='" & Trim(txtAccessionNo.Text) & "'"
        loadGrid1(sqlq)
    End Sub
    Public Sub loadGrid1(sqlq As String)
        Try
            con = New OleDbConnection(cs)
            con.Open()
            cmd = New OleDbCommand(sqlq, con)
            Dim myDA As OleDbDataAdapter = New OleDbDataAdapter(cmd)
            Dim myDataSet As DataSet = New DataSet()
            myDA.Fill(myDataSet, "BookCode")
            DataGridView1.DataSource = myDataSet.Tables("BookCode").DefaultView
            findNoB()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Sub findNoB()

        con = New OleDbConnection(cs)
        con.Open()
        cmd = New OleDbCommand("SELECT NoOfBooks  from Book where AccessionNo='" & Trim(txtAccessionNo.Text) & "'", con)
        rdr = cmd.ExecuteReader()
        rdr.Read()
        txtNOBA.Text = rdr("NoOfBooks").ToString()
        con.Close()
        con = New OleDbConnection(cs)
        con.Open()
        cmd = New OleDbCommand("SELECT Code  from BookCode where AccessionNo='" & Trim(txtAccessionNo.Text) & "'", con)
        rdr = cmd.ExecuteReader()
        Dim countnb = 0
        While rdr.Read()
            countnb = countnb + 1
        End While
        txtNOB.Text = countnb
        con.Close()



    End Sub

    Public Shared Function GetUniqueKey(ByVal maxSize As Integer) As String
        Dim chars As Char() = New Char(61) {}
        chars = "123456789".ToCharArray()
        Dim data As Byte() = New Byte(0) {}
        Dim crypto As New RNGCryptoServiceProvider()
        crypto.GetNonZeroBytes(data)
        data = New Byte(maxSize - 1) {}
        crypto.GetNonZeroBytes(data)
        Dim result As New StringBuilder(maxSize)
        For Each b As Byte In data
            result.Append(chars(b Mod (chars.Length)))
        Next
        Return result.ToString()
    End Function

    Private Sub btnSave_Click(sender As System.Object, e As System.EventArgs) Handles btnSave.Click
        Try
            If txtBookCode.Text = "" Then
                MsgBox("Blank Book Code")
                txtBookCode.Focus()
                Exit Sub
            End If
            If MsgBox("Save Changes?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "WARNING") = MsgBoxResult.Yes Then
                Dim BID As String = "BID" + GetUniqueKey(5)
                lblBookID.Text = BID
                con = New OleDbConnection(cs)
                con.Open()
                Dim cb As String = "insert into BookCode(AccessionNo, BookTitle, Code, BID) VALUES ('" & txtAccessionNo.Text & "','" & txtBookTitle.Text & "','" & txtBookCode.Text & "', '" & lblBookID.Text & "')"
                cmd = New OleDbCommand(cb)
                cmd.Connection = con
                cmd.ExecuteReader()
                con.Close()
                con = New OleDbConnection(cs)
                con.Open()
                Dim cb1 As String = "Update book set NoOfBooks = NoOfBooks+1 where AccessionNo='" & txtAccessionNo.Text & "'"
                cmd = New OleDbCommand(cb1)
                cmd.Connection = con
                cmd.ExecuteNonQuery()
                con.Close()
                Dim sqlq = "SELECT BID as [Book ID], Code as [Book Code] from BookCode where AccessionNo='" & Trim(txtAccessionNo.Text) & "'"
                loadGrid1(sqlq)
                MsgBox("Record Successfully Added on Database!")
                btnSave.Enabled = False
                btnDelete.Enabled = True
                btnUpdate.Enabled = True
            Else
                Me.Close()

            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub btnNew_Click(sender As System.Object, e As System.EventArgs) Handles btnNew.Click
        txtBookCode.Text = ""
        lblBookID.Text = ""
        txtBookCode.Focus()
        btnDelete.Enabled = False
        btnUpdate.Enabled = False
        btnSave.Enabled = True




    End Sub

    Private Sub btnDelete_Click(sender As System.Object, e As System.EventArgs) Handles btnDelete.Click
        If lblBookID.Text = "" And txtBookCode.Text = "" Then
            MsgBox("Select Record to Delete")
            Exit Sub
            DataGridView1.Focus()

        End If
        Try
            If MessageBox.Show("Do you really want to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = Windows.Forms.DialogResult.Yes Then
                DeleteRecord()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Sub DeleteRecord()
        Dim RowsAffected As Integer = 0
        con = New OleDbConnection(cs)
        con.Open()
        Dim cq As String = "delete from BookCode where Code= '" & Trim(txtBookCode.Text) & "'"
        cmd = New OleDbCommand(cq)
        cmd.Connection = con
        RowsAffected = cmd.ExecuteNonQuery()
        If RowsAffected > 0 Then
            MessageBox.Show("Successfully deleted", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information)
            con = New OleDbConnection(cs)
            con.Open()
            Dim cb1 As String = "Update book set NoOfBooks = NoOfBooks-1 where AccessionNo='" & txtAccessionNo.Text & "'"
            cmd = New OleDbCommand(cb1)
            cmd.Connection = con
            cmd.ExecuteNonQuery()
            con.Close()
            Dim sqlq = "SELECT BID as [Book ID], Code as [Book Code] from BookCode where AccessionNo='" & Trim(txtAccessionNo.Text) & "'"
            loadGrid1(sqlq)

        Else
            MessageBox.Show("No record found", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Information)

        End If

    End Sub

    Private Sub btnUpdate_Click(sender As System.Object, e As System.EventArgs) Handles btnUpdate.Click
        If lblBookID.Text = "" And txtBookCode.Text = "" Then
            MsgBox("Select Record to Update")
            Exit Sub
            DataGridView1.Focus()
        End If
        Try
            If MessageBox.Show("Do you really want to Update this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = Windows.Forms.DialogResult.Yes Then
                UpdateRecord()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Sub UpdateRecord()
        con = New OleDbConnection(cs)
        con.Open()
        Dim cb As String = "update BookCode set Code='" & Trim(txtBookCode.Text) & "' where BID='" & lblBookID.Text & "'"
        cmd = New OleDbCommand(cb)
        cmd.Connection = con
        cmd.ExecuteNonQuery()
        con.Close()
        MessageBox.Show("Successfully updated", "Book Record", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Dim sqlq = "SELECT BID as [Book ID], Code as [Book Code] from BookCode where AccessionNo='" & Trim(txtAccessionNo.Text) & "'"
        loadGrid1(sqlq)
    End Sub



    Private Sub DataGridView1_SelectionChanged(sender As Object, e As System.EventArgs) Handles DataGridView1.SelectionChanged
        Try
            txtBookCode.Text = DataGridView1.CurrentRow.Cells(1).Value
            lblBookID.Text = DataGridView1.CurrentRow.Cells(0).Value
            btnSave.Enabled = False
            btnUpdate.Enabled = True
            btnDelete.Enabled = True
        Catch ex As Exception

        End Try



    End Sub

    Private Sub DataGridView1_RowHeaderMouseClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles DataGridView1.RowHeaderMouseClick

        Dim dr As DataGridViewRow = DataGridView1.SelectedRows(0)
        txtBookCode.Text = dr.Cells(1).Value.ToString()
        lblBookID.Text = dr.Cells(0).Value.ToString()
        btnSave.Enabled = False
        btnUpdate.Enabled = True
        btnDelete.Enabled = True
    End Sub

    Private Sub txtSearch_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtSearch.TextChanged
        txtBookCode.Text = ""
        Dim sqlq = "SELECT BID, Code as BookCode from BookCode where code like '%" & txtSearch.Text & "%' and AccessionNo='" & txtAccessionNo.Text & "'"
        loadGrid1(sqlq)
    End Sub
End Class