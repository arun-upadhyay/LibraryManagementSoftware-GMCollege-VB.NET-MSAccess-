Imports System.IO
Imports System.Data.OleDb

Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Security.Cryptography
Imports System.Text

Public Class frmMain
    Dim rdr As OleDbDataReader = Nothing
    Dim dtable As DataTable
    Dim con As OleDbConnection = Nothing
    Dim adp As OleDbDataAdapter
    Dim ds As DataSet
    Dim cmd As OleDbCommand = Nothing
    Dim dt As New DataTable
    Dim cs As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\LMS_DB.accdb;Persist Security Info=False;"
    Dim ci, cr As Integer
    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        frmAbout.Show()
    End Sub
    Private Sub DataGridView1_RowPostPaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPostPaintEventArgs) Handles DataGridView1.RowPostPaint
        Dim strRowNumber As String = (e.RowIndex + 1).ToString()
        Dim size As SizeF = e.Graphics.MeasureString(strRowNumber, Me.Font)
        If DataGridView1.RowHeadersWidth < Convert.ToInt32((size.Width + 20)) Then
            DataGridView1.RowHeadersWidth = Convert.ToInt32((size.Width + 20))
        End If
        Dim b As Brush = SystemBrushes.ControlText
        e.Graphics.DrawString(strRowNumber, Me.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2))

    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        lblDateTime.Text = Now
        con = New OleDbConnection(cs)
        con.Open()
        cmd = New OleDbCommand("SELECT NoOfBooks  from Book", con)
        rdr = cmd.ExecuteReader()
        Dim countnb = 0
        While rdr.Read()
            countnb = countnb + CInt(rdr(0).ToString)
        End While
        con.Close()
        con = New OleDbConnection(cs)
        con.Open()
        cmd = New OleDbCommand("SELECT * from BookIssue_Student where Status='Issued'", con)
        rdr = cmd.ExecuteReader()
        Dim count1 = 0
        While rdr.Read()
            count1 = count1 + 1
        End While

        con.Close()
        con = New OleDbConnection(cs)
        con.Open()
        cmd = New OleDbCommand("SELECT * from BookIssue_Staff where Status='Issued'", con)
        rdr = cmd.ExecuteReader()
        While rdr.Read()
            count1 = count1 + 1
        End While
        Me.Text = "Library Management System    Current Library Status: " + "Available Books=" + CStr(countnb) + " Books Issued= " + CStr(count1)
        lblAvailableBooks.Text = CStr(countnb)
        lblIssuedBooks.Text = CStr(count1)
        lblTotalBooks.Text = CStr(CInt(countnb) + CStr(count1))
        con.Close()
        ''Loading Book Information
        Dim sqlb As String = "Select AccessionNo as [Accession No], BookTitle as [Book Title], Author, NoOfBooks as [Total Books], Price from Book"
        loadBook(sqlb)
        ''Loading Student and Staff Information
        Dim sqls As String = "Select StudentID as [Student ID], StudentName as [Student Name], Course, Stu_Session as [Session], MobileNo as [Contact No] from Student"
        loadStudents(sqls)
        ''Loading Book Issue information for Student and Staff
        DataGridView3.Rows.Clear()
        ci = 0
        Dim sqli As String = "Select StudentID , AccessionNo, BCode ,IssueDate, DueDate from BookIssue_Student where Status='Issued' order by IssueDate Desc"
        loadBookIssue(sqli)
        sqli = "Select StaffID , AccessionNo , BCode  ,IssueDate, DueDate from BookIssue_Staff where Status='Issued' order by IssueDate Desc"
        loadBookIssue(sqli)
        ''Loading Book Return information for Student and Staff
        cr = 0
        DataGridView4.Rows.Clear()
        Dim sqlr As String = "Select StudentID as [Student ID], AccessionNo as [Accession No], BCode  as [Book Code],IssueDate, ReturnDate, Fine from BookIssue_Student, Return_Student where BookIssue_Student.TransactionID=Return_Student.TransactionID order by ReturnDate DESC"
        loadBookReturn(sqlr)
        sqlr = "Select StaffID , AccessionNo , BCode  ,IssueDate, ReturnDate, Fine from BookIssue_Staff, Return_Staff where BookIssue_Staff.TransactionID=Return_Staff.TransactionID order by ReturnDate DESC"
        loadBookReturn(sqlr)
    End Sub
    Public Sub loadStudents(sqls As String)
        Try
            con = New OleDbConnection(cs)
            con.Open()
            cmd = New OleDbCommand(sqls, con)
            Dim myDA As OleDbDataAdapter = New OleDbDataAdapter(cmd)
            Dim myDataSet As DataSet = New DataSet()
            myDA.Fill(myDataSet, "Studentsload")
            DataGridView2.DataSource = myDataSet.Tables("Studentsload").DefaultView
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Sub loadBook(sqlb As String)
        Try
            con = New OleDbConnection(cs)
            con.Open()
            cmd = New OleDbCommand(sqlb, con)
            rdr = cmd.ExecuteReader()
            Dim c As Integer = 0
            DataGridView1.Rows.Clear()
            If rdr.HasRows Then
                While rdr.Read()
                    DataGridView1.Rows.Add()
                    DataGridView1.Item(0, c).Value = rdr(0).ToString
                    DataGridView1.Item(1, c).Value = rdr(1).ToString
                    DataGridView1.Item(2, c).Value = rdr(2).ToString
                    DataGridView1.Item(3, c).Value = rdr(3).ToString
                    DataGridView1.Item(4, c).Value = rdr(4).ToString
                    c = c + 1
                End While
            End If
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Sub loadBookIssue(sqli As String)
        Try
            con = New OleDbConnection(cs)
            con.Open()
            cmd = New OleDbCommand(sqli, con)
            rdr = cmd.ExecuteReader()
            If rdr.HasRows Then
                While rdr.Read()
                    DataGridView3.Rows.Add()
                    DataGridView3.Item(0, ci).Value = rdr(0).ToString
                    DataGridView3.Item(1, ci).Value = rdr(1).ToString
                    DataGridView3.Item(2, ci).Value = rdr(2).ToString
                    DataGridView3.Item(3, ci).Value = rdr(3).ToString
                    DataGridView3.Item(4, ci).Value = rdr(4).ToString
                    ci = ci + 1
                End While
            End If
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Sub loadBookReturn(sqlr As String)
        Try
            con = New OleDbConnection(cs)
            con.Open()
            cmd = New OleDbCommand(sqlr, con)
            rdr = cmd.ExecuteReader()
            If rdr.HasRows Then
                While rdr.Read()
                    DataGridView4.Rows.Add()
                    DataGridView4.Item(0, cr).Value = rdr(0).ToString
                    DataGridView4.Item(1, cr).Value = rdr(1).ToString
                    DataGridView4.Item(2, cr).Value = rdr(2).ToString
                    DataGridView4.Item(3, cr).Value = rdr(3).ToString
                    DataGridView4.Item(4, cr).Value = rdr(4).ToString
                    DataGridView4.Item(5, cr).Value = rdr(5).ToString
                    cr = cr + 1
                End While
            End If
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub CalculatorToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CalculatorToolStripMenuItem.Click
        System.Diagnostics.Process.Start("Calc.exe")
    End Sub

    Private Sub NotepadToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NotepadToolStripMenuItem.Click
        System.Diagnostics.Process.Start("Notepad.exe")
    End Sub

    Private Sub TaskManagerToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TaskManagerToolStripMenuItem.Click
        System.Diagnostics.Process.Start("TaskMgr.exe")
    End Sub

    Private Sub MSWordToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MSWordToolStripMenuItem.Click
        System.Diagnostics.Process.Start("WinWord.exe")
    End Sub

    Private Sub WordpadToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WordpadToolStripMenuItem.Click
        System.Diagnostics.Process.Start("Wordpad.exe")
    End Sub

    Private Sub SystemInfoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SystemInfoToolStripMenuItem.Click
        frmSystemInfo.Show()
    End Sub

    Private Sub StudentsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StudentsToolStripMenuItem.Click
        frmStudent.Show()
    End Sub

    Private Sub LogoutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LogoutToolStripMenuItem.Click
        Me.Hide()
        frmAbout.Hide()
        frmSystemInfo.Hide()
        frmStudent.Hide()
        frmBookIssue.Hide()
        frmBookReturn.Hide()
        frmBookEntry.Hide()
        frmStudent.Hide()
        frmBookReturn.Hide()
        frmCourse.Hide()
        frmStaff.Hide()
        frmSupplier.Hide()
        frmRegistration.Hide()
        frmLoginDetails.Hide()
        frmBookRecord.Hide()
        frmDepartment.Hide()
        frmYears.Hide()
        frmJournalsMagzinesRecord1.Hide()
        frmStudentRecord1.Hide()
        frmStaffRecord1.Hide()
        frmNewsPaper.Hide()
        frmNewsPaperRecord1.Hide()
        frmProjectRecord1.Hide()
        frmBookIssueRecord1.Hide()
        frmBookIssueRecord_Staff1.Hide()
        frmSupplierRecord.Hide()
        frmStudentList.Hide()
        frmStudentListRecord1.Hide()
        frmBookIssueRecord1.Hide()
        frmBookIssueRecord_Staff1.Hide()
        frmBookReturnRecord_Student1.Hide()
        frmBookReturnRecord_Staff1.Hide()
        frmCards.Hide()
        frmStudentsCardRecord.Hide()
        frmStaffCardRecord.Hide()
        frmNoDues.Hide()
        frmStudentsNoDuesdRecord.Hide()
        frmStaffsNoDuesRecord.Hide()
        Frmlogin.Show()
        Frmlogin.cmbUserType.Text = ""
        Frmlogin.txtUsername.Text = ""
        Frmlogin.txtPassword.Text = ""
        Frmlogin.ProgressBar1.Visible = False
        Frmlogin.cmbUserType.Focus()
    End Sub

    Private Sub IssueToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IssueToolStripMenuItem.Click
        frmBookIssue.Show()
    End Sub

    Private Sub BookReturnToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BookReturnToolStripMenuItem.Click
        frmBookReturn.Show()
    End Sub

    Private Sub BooksToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BooksToolStripMenuItem.Click
        frmBookEntry.Show()
    End Sub

    Private Sub BooksToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BooksToolStripMenuItem1.Click
        frmBookEntry.Show()
    End Sub

    Private Sub StudentsToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StudentsToolStripMenuItem1.Click
        frmStudent.Show()
    End Sub

    Private Sub BooksReturnToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BooksReturnToolStripMenuItem.Click
        frmBookReturn.Show()
    End Sub

    Private Sub BooksIssueToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BooksIssueToolStripMenuItem.Click
        frmBookIssue.Show()
    End Sub

    Private Sub CourseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CourseToolStripMenuItem.Click
        frmCourse.Show()
    End Sub

    Private Sub FacultiesToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FacultiesToolStripMenuItem1.Click
        frmStaff.Show()
    End Sub
    Private Sub SuppliersToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SuppliersToolStripMenuItem.Click
        frmSupplier.Show()
    End Sub

    Private Sub RegistrationToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RegistrationToolStripMenuItem1.Click
        frmRegistration.Show()
    End Sub

    Private Sub LoginDetailsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoginDetailsToolStripMenuItem.Click
        frmLoginDetails.Show()
    End Sub

    Private Sub RegistrationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RegistrationToolStripMenuItem.Click
        frmRegistration.Show()
    End Sub

    Private Sub FacultiesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FacultiesToolStripMenuItem.Click
        frmStaff.Show()
    End Sub

    Private Sub SearchToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchToolStripMenuItem1.Click
        frmBookRecord.Show()

    End Sub

    Private Sub frmMain_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        End
    End Sub

    Private Sub DepartmentToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DepartmentToolStripMenuItem.Click
        frmDepartment.Show()
    End Sub

    Private Sub MasterEntryToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MasterEntryToolStripMenuItem2.Click
        frmBookRecord.Show()
    End Sub

    Private Sub YearsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles YearsToolStripMenuItem.Click
        frmYears.Show()
    End Sub

    Private Sub JournalsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles JournalsToolStripMenuItem.Click
        frmJournalsAndMagzines.Show()
    End Sub

    Private Sub JournalsAndMagzinesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles JournalsAndMagzinesToolStripMenuItem.Click
        frmJournalsMagzinesRecord1.Show()
    End Sub

    Private Sub StudentsToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StudentsToolStripMenuItem3.Click
        frmStudentRecord1.Show()
    End Sub

    Private Sub FacultiesToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FacultiesToolStripMenuItem3.Click
        frmStaffRecord1.Show()
    End Sub

    Private Sub NewPapersToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewPapersToolStripMenuItem.Click
        frmNewsPaper.Show()
    End Sub

    Private Sub BooksToolStripMenuItem4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BooksToolStripMenuItem4.Click
        frmNewsPaperRecord1.Show()
        frmNewsPaperRecord1.Reset()
    End Sub

    Private Sub SubscriptionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SubscriptionToolStripMenuItem.Click
        frmProject.Show()
    End Sub

    Private Sub ProjectsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProjectsToolStripMenuItem.Click
        frmProjectRecord1.Show()
        frmProjectRecord1.Reset()
    End Sub

    Private Sub StudentsToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StudentsToolStripMenuItem2.Click
        frmBookIssueRecord1.Show()
        frmBookIssueRecord1.Reset()
    End Sub

    Private Sub StaffToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StaffToolStripMenuItem.Click
        frmBookIssueRecord_Staff1.Show()
        frmBookIssueRecord_Staff1.Reset()
    End Sub

    Private Sub SuppliersToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SuppliersToolStripMenuItem1.Click
        frmSupplierRecord.Show()
    End Sub

    Private Sub StudentListToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StudentListToolStripMenuItem.Click
        frmStudentList.Show()
    End Sub

    Private Sub StudentsListToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StudentsListToolStripMenuItem.Click
        frmStudentListRecord1.Show()
        frmStudentListRecord1.Reset()
    End Sub

    Private Sub StudentsToolStripMenuItem5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StudentsToolStripMenuItem5.Click
        frmBookIssueRecord1.Show()
        frmBookIssueRecord1.Reset()
    End Sub

    Private Sub StaffsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StaffsToolStripMenuItem.Click
        frmBookIssueRecord_Staff1.Show()
        frmBookIssueRecord_Staff1.Reset()
    End Sub

    Private Sub StudentsToolStripMenuItem6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StudentsToolStripMenuItem6.Click
        frmBookReturnRecord_Student1.Show()
        frmBookReturnRecord_Student1.Reset()
    End Sub

    Private Sub StaffsToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StaffsToolStripMenuItem1.Click
        frmBookReturnRecord_Staff1.Show()
        frmBookReturnRecord_Staff1.Reset()
    End Sub

    Private Sub CardsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CardsToolStripMenuItem.Click
        frmCards.Show()
    End Sub

    Private Sub StudentsToolStripMenuItem7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StudentsToolStripMenuItem7.Click
        frmStudentsCardRecord.Show()
    End Sub

    Private Sub StaffToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StaffToolStripMenuItem2.Click
        frmStaffCardRecord.Show()
    End Sub

    Private Sub NoDuesDocToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NoDuesDocToolStripMenuItem.Click
        frmNoDues.Show()
    End Sub

    Private Sub StudentsToolStripMenuItem8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StudentsToolStripMenuItem8.Click
        frmStudentsNoDuesdRecord.Show()
    End Sub

    Private Sub StaffToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StaffToolStripMenuItem3.Click
        frmStaffsNoDuesRecord.Show()
    End Sub

    Private Sub BackupToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles BackupToolStripMenuItem.Click
       
        backUP()

    End Sub
    Public Sub backUP()
        Try
            Dim dbFileName As String = "LMS_DB.accdb"
            Dim CurrentDatabasePath As String = Path.Combine(Environment.CurrentDirectory, dbFileName)
            Dim backTimeStamp As String = DateTime.Now.ToString("dd-MMM-yy-HH-mm-ss") + "_" + Path.GetFileNameWithoutExtension(dbFileName)
            Dim destFileName As String = backTimeStamp + dbFileName
            Dim fbd As New FolderBrowserDialog
            If fbd.ShowDialog = DialogResult.OK Then
                Dim PathtobackUp As String = fbd.SelectedPath.ToString()
                destFileName = Path.Combine(PathtobackUp, destFileName)
                File.Copy(CurrentDatabasePath, destFileName, True)
                MessageBox.Show("Successful Backup! ")
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
    Private Sub RestoreToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles RestoreToolStripMenuItem.Click
        restoreDB()

    End Sub
    Public Sub restoreDB()
        Try
            Dim dbFileName As String = "LMS_DB.accdb"
            Dim pathBackup As String = ""
            Dim ext As String = ""
            Dim CurrentDatabasePath As String = Path.Combine(Application.StartupPath, dbFileName)
            Dim ODialog As New OpenFileDialog
            If ODialog.ShowDialog() = DialogResult.OK Then

                pathBackup = ODialog.FileName
                ext = Path.GetExtension(pathBackup)
            End If
            If ext = ".accdb" Then

                File.Copy(pathBackup, CurrentDatabasePath, True)
                MessageBox.Show("Successfully Restored!")
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub
    Private Sub BookCodeToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles BookCodeToolStripMenuItem.Click
        BookSerialEntry.Show()

    End Sub

    Private Sub BackupToolStripMenuItem1_Click(sender As System.Object, e As System.EventArgs) Handles BackupToolStripMenuItem1.Click
        backUP()

    End Sub

    Private Sub RestoreToolStripMenuItem1_Click(sender As System.Object, e As System.EventArgs) Handles RestoreToolStripMenuItem1.Click
        restoreDB()

    End Sub
    Private Sub TextBox1_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBox1.TextChanged
        Try
            Timer1.Enabled = False
            Dim sqlb = "Select AccessionNo as [Accession No], BookTitle as [Book Title], Author, NoOfBooks as [Total Books], Price from Book where BookTitle Like '" & TextBox1.Text & "%' or AccessionNo Like '" & TextBox1.Text & "%' "
            loadBook(sqlb)

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

   
    Private Sub frmMain_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        ''Timer1.Enabled = True

    End Sub

    Private Sub frmMain_LostFocus(sender As Object, e As System.EventArgs) Handles Me.LostFocus
        Timer1.Enabled = True
    End Sub

    Private Sub RefreshToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles RefreshToolStripMenuItem.Click
        Timer1.Enabled = True
        TextBox1.Text = ""
        TextBox2.Text = ""
        TextBox3.Text = ""
        TextBox4.Text = ""

    End Sub

    Private Sub TextBox2_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBox2.TextChanged
        Try
            Timer1.Enabled = False
            Dim sqls As String = "Select StudentID as [Student ID], StudentName as [Student Name], Course, Stu_Session as [Session], MobileNo as [Contact No] from Student where StudentID LIKE '" & TextBox2.Text & "%' OR StudentName LIKE '" & TextBox2.Text & "%'"
            loadStudents(sqls)

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    
    Private Sub TextBox3_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBox3.TextChanged
        Try
            Timer1.Enabled = False
            DataGridView3.Rows.Clear()
            ci = 0
            Dim sqli As String = "Select StudentID , AccessionNo, BCode ,IssueDate, DueDate from BookIssue_Student where StudentID like '" & TextBox3.Text & "%' and Status='Issued' order by IssueDate Desc"
            loadBookIssue(sqli)
            sqli = "Select StaffID, AccessionNo , BCode  ,IssueDate, DueDate from BookIssue_Staff where Status='Issued' and  StaffID like '" & TextBox3.Text & "%' order by IssueDate Desc"
            loadBookIssue(sqli)
        Catch ex As Exception

        End Try
    End Sub
    Private Sub TextBox4_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBox4.TextChanged
        Try
            Timer1.Enabled = False
            cr = 0
            DataGridView4.Rows.Clear()
            Dim sqlr As String = "Select StudentID as [Student ID], AccessionNo as [Accession No], BCode  as [Book Code],IssueDate, ReturnDate, Fine from BookIssue_Student, Return_Student where BookIssue_Student.TransactionID=Return_Student.TransactionID and  StudentID like '" & TextBox4.Text & "%'  order by ReturnDate DESC"
            loadBookReturn(sqlr)
            sqlr = "Select StaffID , AccessionNo , BCode  ,IssueDate, ReturnDate, Fine from BookIssue_Staff, Return_Staff where BookIssue_Staff.TransactionID=Return_Staff.TransactionID and StaffID like '" & TextBox4.Text & "%' order by ReturnDate DESC"
            loadBookReturn(sqlr)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub DataGridView2_RowPostPaint(sender As System.Object, e As System.Windows.Forms.DataGridViewRowPostPaintEventArgs) Handles DataGridView2.RowPostPaint
        Dim strRowNumber As String = (e.RowIndex + 1).ToString()
        Dim size As SizeF = e.Graphics.MeasureString(strRowNumber, Me.Font)
        If DataGridView2.RowHeadersWidth < Convert.ToInt32((size.Width + 20)) Then
            DataGridView2.RowHeadersWidth = Convert.ToInt32((size.Width + 20))
        End If
        Dim b As Brush = SystemBrushes.ControlText
        e.Graphics.DrawString(strRowNumber, Me.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2))
    End Sub

    Private Sub DataGridView3_RowPostPaint(sender As System.Object, e As System.Windows.Forms.DataGridViewRowPostPaintEventArgs) Handles DataGridView3.RowPostPaint
        Dim strRowNumber As String = (e.RowIndex + 1).ToString()
        Dim size As SizeF = e.Graphics.MeasureString(strRowNumber, Me.Font)
        If DataGridView3.RowHeadersWidth < Convert.ToInt32((size.Width + 20)) Then
            DataGridView3.RowHeadersWidth = Convert.ToInt32((size.Width + 20))
        End If
        Dim b As Brush = SystemBrushes.ControlText
        e.Graphics.DrawString(strRowNumber, Me.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2))
    End Sub

    Private Sub DataGridView4_RowPostPaint(sender As System.Object, e As System.Windows.Forms.DataGridViewRowPostPaintEventArgs) Handles DataGridView4.RowPostPaint
        Dim strRowNumber As String = (e.RowIndex + 1).ToString()
        Dim size As SizeF = e.Graphics.MeasureString(strRowNumber, Me.Font)
        If DataGridView4.RowHeadersWidth < Convert.ToInt32((size.Width + 20)) Then
            DataGridView4.RowHeadersWidth = Convert.ToInt32((size.Width + 20))
        End If
        Dim b As Brush = SystemBrushes.ControlText
        e.Graphics.DrawString(strRowNumber, Me.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2))
    End Sub
End Class
