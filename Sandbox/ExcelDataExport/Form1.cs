using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Spire.Xls;
using System.Text.RegularExpressions;

namespace ExcelDataExport
{
    public partial class Form1 : Form
    {
        string datePattern = @"^Calendar\-\d{2}\-\d{2}\-\d{4}$";

        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();

            //workbook.LoadFromFile(@"../../DatatableSample.xlsx");
            workbook.LoadFromFile(@"C:\Personal\Forex\FFCalendar Downloads\Calendar-01-01-2017.xls");

            //Initailize worksheet
            Worksheet sheet = workbook.Worksheets[0];

            var DataTable = sheet.ExportDataTable();

            var numberColumns = 0;
            var rowCounter = 0;

            var dataTableName = DataTable.TableName;

            var extractedDate = "";
            if (Regex.IsMatch(dataTableName,datePattern))
            {
                extractedDate = dataTableName.Replace("Calendar-", "");
            }

            var extractedYear = dataTableName.Substring(dataTableName.LastIndexOf("-")+1, 4);

            MessageBox.Show($"Inferred Date: {extractedDate}, Inferred Year: {extractedYear}");

            foreach (DataRow row in DataTable.Rows)
            {
                rowCounter++;
                if (rowCounter<11)
                {
                    continue;
                }

                var rowData = "";
                numberColumns = row.ItemArray.Length;
                for (int i = 0; i < numberColumns; i++)
                {
                    rowData = String.Concat(rowData, $"{row.ItemArray[i]},");
                }

                if (rowData.EndsWith(","))
                {
                    rowData = rowData.Substring(0, rowData.Length - 1);
                }
                Console.WriteLine(rowData);
            }

            this.dataGrid1.DataSource = DataTable;
        }
    }
}
