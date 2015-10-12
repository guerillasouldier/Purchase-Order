Imports System.Security.Permissions

Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Activated

        Me.DateTimePicker1.Enabled = False
        Me.DateTimePicker2.Enabled = False
        Me.Button1.Enabled = False

        Call DataGridUpdate()


    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles DataGridView1.MouseUp
        If e.Button = Windows.Forms.MouseButtons.Right Then
            Me.ContextMenuStrip1.Show()

        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Call DataGridUpdate()

    End Sub

    Sub DataGridUpdate()

        Dim con As New OleDb.OleDbConnection
        con.ConnectionString = "Provider=SQLOLEDB;Server=de08w2971;Database=oem_imaint;UID=imaint"
        con.Open()

        Dim Cmd As New OleDb.OleDbCommand
        With Cmd
            .CommandType = CommandType.Text
            If Me.CheckBox1.Checked = True Then

                Dim Date1 As Date = Me.DateTimePicker1.Text
                Dim Date2 As Date = Me.DateTimePicker2.Text

                .CommandText = "SELECT imrpt_po_part_orders.po_num AS [PO Number], imrpt_po_part_orders.status_id AS Status, imrpt_po_part_orders.date_ordered AS [Date Ordered], 
                      imrpt_po_part_orders.order_quantity AS [Quantity Ordered], imrpt_po_part_orders.part_id As [Part ID], imvw_part.description As [Part Description], 
                      imrpt_po_part_orders.supplier AS Supplier
From imrpt_po_part_orders INNER Join
                      imvw_part On imrpt_po_part_orders.part_id = imvw_part.id
Where (imrpt_po_part_orders.date_ordered > Convert(DateTime, '" & Date1 & "', 102) AND imrpt_po_part_orders.date_ordered < Convert(DateTime, '" & Date2 & "', 102))"
            Else
                .CommandText = "SELECT imrpt_po_part_orders.po_num AS [PO Number], imrpt_po_part_orders.status_id AS Status, imrpt_po_part_orders.date_ordered AS [Date Ordered], 
                      imrpt_po_part_orders.order_quantity AS [Quantity Ordered], imrpt_po_part_orders.part_id As [Part ID], imvw_part.description As [Part Description], 
                      imrpt_po_part_orders.supplier AS Supplier
From imrpt_po_part_orders INNER Join
                      imvw_part On imrpt_po_part_orders.part_id = imvw_part.id
Where (imrpt_po_part_orders.status_id Like 'Ordered' OR imrpt_po_part_orders.status_id Like 'Awaiting Approval')"

            End If
            .Connection = con
        End With

        Dim reader As OleDb.OleDbDataReader = Cmd.ExecuteReader()
        reader.Read()

        Dim dt = New DataTable
        dt.Load(reader)
        Try

            With DataGridView1
                .AutoGenerateColumns = True
                .DataSource = dt
                .Refresh()
            End With

        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            con.Close()
        End Try

        reader.Close()

    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If Me.CheckBox1.Checked = True Then
            Me.DateTimePicker1.Enabled = True
            Me.DateTimePicker2.Enabled = True
            Me.Button1.Enabled = True
        Else
            Me.DateTimePicker1.Enabled = False
            Me.DateTimePicker2.Enabled = False
            Me.Button1.Enabled = False
            Call DataGridUpdate()
        End If
    End Sub

    Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click

        My.Computer.Clipboard.SetDataObject(DataGridView1.GetClipboardContent())

    End Sub

    Private Sub PrintToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PrintToolStripMenuItem.Click, PrintToolStripMenuItem1.Click

        Me.PrintDialog1().ShowDialog()
        Me.PrintDialog1().AllowSelection = True

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click


        'Declare new instance of the application for program use
        Dim excelApp As New Microsoft.Office.Interop.Excel.Application
        excelApp.Visible = True
        Dim misValue As Object = System.Reflection.Missing.Value
        Dim oBook As Microsoft.Office.Interop.Excel.Workbook = excelApp.Workbooks.Add
        Dim oSheet As Microsoft.Office.Interop.Excel.Worksheet = CType(excelApp.Worksheets.Add, Microsoft.Office.Interop.Excel.Worksheet)

        If DataGridView1.SelectedCells.Count = 0 Then
            MsgBox("Please select a range of cells before attempting to export.", 1, "No Range Selected!")
        End If

        Dim columns As Integer = DataGridView1.ColumnCount
        Dim rows As Integer = DataGridView1.RowCount

        'Create an array
        Dim DataArray(rows, columns) As Object
        Dim r As Integer
        Dim x As Integer

        For x = 0 To columns - 1
            For r = 0 To rows - 1
                DataArray(r, x) = DataGridView1(x, r).Value
            Next
        Next

        'Add headers to the worksheet on row 1.
        oSheet = oBook.Worksheets(1)
        oSheet.Range("A1").Value = "Part Order Number"
        oSheet.Range("B1").Value = "Status"
        oSheet.Range("C1").Value = "Date Ordered"
        oSheet.Range("D1").Value = "Quantity Ordered"
        oSheet.Range("E1").Value = "Part ID"
        oSheet.Range("F1").Value = "Part Description"
        oSheet.Range("G1").Value = "Supplier"

        'Transfer the array to the worksheet starting at cell A2.
        oSheet.Range("A2").Resize(rows, columns).Value = DataArray
        oSheet.Rows.RowHeight = 22
        oSheet.Range("A1:G1").Font.Bold = True
        oSheet.Range("A1:G1").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter
        oSheet.Columns.AutoFit()
        'Save the Workbook and quit Excel.
        oSheet = Nothing
        oBook = Nothing

    End Sub
End Class

