using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows
namespace faceAnnotate_api
{
    public class XLFileWriter
    {
        


        public static bool CreateExcelDocument(DataSet ds, string excelFilename)
        {
            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(excelFilename, SpreadsheetDocumentType.Workbook))
                {
                    WriteExcelFile(ds, document);
                }
                Trace.WriteLine("Successfully created: " + excelFilename);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                return false;
            }
        }

        private static void WriteExcelFile(DataSet ds, SpreadsheetDocument spreadsheet)
        {
            //  Create the Excel file contents.  This function is used when creating an Excel file either writing 
            //  to a file, or writing to a MemoryStream.
            spreadsheet.AddWorkbookPart();
            spreadsheet.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

            //  My thanks to James Miera for the following line of code (which prevents crashes in Excel 2010)
            spreadsheet.WorkbookPart.Workbook.Append(new BookViews(new WorkbookView()));

            //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
            WorkbookStylesPart workbookStylesPart = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");
            Stylesheet stylesheet = new Stylesheet();
            workbookStylesPart.Stylesheet = stylesheet;

            //  Loop through each of the DataTables in our DataSet, and create a new Excel Worksheet for each.
            uint worksheetNumber = 1;
            Sheets sheets = spreadsheet.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
            foreach (DataTable dt in ds.Tables)
            {
                //  For each worksheet you want to create
                string worksheetName = dt.TableName;

                //  Create worksheet part, and add it to the sheets collection in workbook
                WorksheetPart newWorksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
                Sheet sheet = new Sheet() { Id = spreadsheet.WorkbookPart.GetIdOfPart(newWorksheetPart), SheetId = worksheetNumber, Name = worksheetName };
                sheets.Append(sheet);

                //  Append this worksheet's data to our Workbook, using OpenXmlWriter, to prevent memory problems
                WriteDataTableToExcelWorksheet(dt, newWorksheetPart);

                worksheetNumber++;
            }

            spreadsheet.WorkbookPart.Workbook.Save();
        }

        private static void WriteDataTableToExcelWorksheet(DataTable dt, WorksheetPart worksheetPart)
        {
            OpenXmlWriter writer = OpenXmlWriter.Create(worksheetPart);
            writer.WriteStartElement(new Worksheet());
            writer.WriteStartElement(new SheetData());

            string cellValue = "";

            //  Create a Header Row in our Excel file, containing one header for each Column of data in our DataTable.
            //
            //  We'll also create an array, showing which type each column of data is (Text or Numeric), so when we come to write the actual
            //  cells of data, we'll know if to write Text values or Numeric cell values.
            int numberOfColumns = dt.Columns.Count;
            bool[] IsNumericColumn = new bool[numberOfColumns];

            string[] excelColumnNames = new string[numberOfColumns];
            for (int n = 0; n < numberOfColumns; n++)
                excelColumnNames[n] = GetExcelColumnName(n);

            //
            //  Create the Header row in our Excel Worksheet
            //
            uint rowIndex = 1;

            writer.WriteStartElement(new Row { RowIndex = rowIndex });
            for (int colInx = 0; colInx < numberOfColumns; colInx++)
            {
                DataColumn col = dt.Columns[colInx];
                AppendTextCell(excelColumnNames[colInx] + "1", col.ColumnName, ref writer);
                IsNumericColumn[colInx] = (col.DataType.FullName == "System.Decimal") || (col.DataType.FullName == "System.Int32") || (col.DataType.FullName == "System.Double") || (col.DataType.FullName == "System.Single");
            }
            writer.WriteEndElement();   //  End of header "Row"

            //
            //  Now, step through each row of data in our DataTable...
            //
            double cellNumericValue = 0;
            foreach (DataRow dr in dt.Rows)
            {
                // ...create a new row, and append a set of this row's data to it.
                ++rowIndex;

                writer.WriteStartElement(new Row { RowIndex = rowIndex });

                for (int colInx = 0; colInx < numberOfColumns; colInx++)
                {
                    cellValue = dr.ItemArray[colInx].ToString();

                    // Create cell with data
                    if (IsNumericColumn[colInx])
                    {
                        //  For numeric cells, make sure our input data IS a number, then write it out to the Excel file.
                        //  If this numeric value is NULL, then don't write anything to the Excel file.
                        cellNumericValue = 0;
                        if (double.TryParse(cellValue, out cellNumericValue))
                        {
                            cellValue = cellNumericValue.ToString();
                            AppendNumericCell(excelColumnNames[colInx] + rowIndex.ToString(), cellValue, ref writer);
                        }
                    }
                    else
                    {
                        //  For text cells, just write the input data straight out to the Excel file.
                        AppendTextCell(excelColumnNames[colInx] + rowIndex.ToString(), cellValue, ref writer);
                    }
                }
                writer.WriteEndElement(); //  End of Row
            }
            writer.WriteEndElement(); //  End of SheetData
            writer.WriteEndElement(); //  End of worksheet

            writer.Close();
        }

        private static void AppendTextCell(string cellReference, string cellStringValue, ref OpenXmlWriter writer)
        {
            //  Add a new Excel Cell to our Row 
            writer.WriteElement(new Cell { CellValue = new CellValue(cellStringValue), CellReference = cellReference, DataType = CellValues.String });
        }

        private static void AppendNumericCell(string cellReference, string cellStringValue, ref OpenXmlWriter writer)
        {
            //  Add a new Excel Cell to our Row 
            writer.WriteElement(new Cell { CellValue = new CellValue(cellStringValue), CellReference = cellReference, DataType = CellValues.Number });
        }

        private static string GetExcelColumnName(int columnIndex)
        {
            //  Convert a zero-based column index into an Excel column reference  (A, B, C.. Y, Y, AA, AB, AC... AY, AZ, B1, B2..)
            //
            //  eg  GetExcelColumnName(0) should return "A"
            //      GetExcelColumnName(1) should return "B"
            //      GetExcelColumnName(25) should return "Z"
            //      GetExcelColumnName(26) should return "AA"
            //      GetExcelColumnName(27) should return "AB"
            //      ..etc..
            //
            if (columnIndex < 26)
                return ((char)('A' + columnIndex)).ToString();

            char firstChar = (char)('A' + (columnIndex / 26) - 1);
            char secondChar = (char)('A' + (columnIndex % 26));

            return string.Format("{0}{1}", firstChar, secondChar);
        }

        public static DataSet CreateSampleData()
        {
            //  Create a sample DataSet, containing three DataTables.
            //  (Later, this will save to Excel as three Excel worksheets.)
            //
            DataSet ds = new DataSet();

            //  Create the first table of sample data
            DataTable dt1 = new DataTable("Drivers");
            dt1.Columns.Add("UserID", Type.GetType("System.Decimal"));
            dt1.Columns.Add("Surname", Type.GetType("System.String"));
            dt1.Columns.Add("Forename", Type.GetType("System.String"));
            dt1.Columns.Add("Sex", Type.GetType("System.String"));
            dt1.Columns.Add("Date of Birth", Type.GetType("System.DateTime"));

            dt1.Rows.Add(new object[] { 1, "James", "Brown", "M", new DateTime(1962,3,19) });
            dt1.Rows.Add(new object[] { 2, "Edward", "Jones", "M", new DateTime(1939,7,12) });
            dt1.Rows.Add(new object[] { 3, "Janet", "Spender", "F", new DateTime(1996,1,7) });
            dt1.Rows.Add(new object[] { 4, "Maria", "Percy", "F", new DateTime(1991,10,24) });
            dt1.Rows.Add(new object[] { 5, "Malcolm", "Marvelous", "M", new DateTime(1973,5,7) });
            ds.Tables.Add(dt1);


            //  Create the second table of sample data
            DataTable dt2 = new DataTable("Vehicles");
            dt2.Columns.Add("Vehicle ID", Type.GetType("System.Decimal"));
            dt2.Columns.Add("Make", Type.GetType("System.String"));
            dt2.Columns.Add("Model", Type.GetType("System.String"));

            dt2.Rows.Add(new object[] { 1001, "Ford", "Banana" });
            dt2.Rows.Add(new object[] { 1002, "GM", "Thunderbird" });
            dt2.Rows.Add(new object[] { 1003, "Porsche", "Rocket" });
            dt2.Rows.Add(new object[] { 1004, "Toyota", "Gas guzzler" });
            dt2.Rows.Add(new object[] { 1005, "Fiat", "Spangly" });
            dt2.Rows.Add(new object[] { 1006, "Peugeot", "Lawnmower" });
            dt2.Rows.Add(new object[] { 1007, "Jaguar", "Freeloader" });
            dt2.Rows.Add(new object[] { 1008, "Aston Martin", "Caravanette" });
            dt2.Rows.Add(new object[] { 1009, "Mercedes-Benz", "Hitchhiker" });
            dt2.Rows.Add(new object[] { 1010, "Renault", "Sausage" });
            dt2.Rows.Add(new object[] { 1011, "Saab", "Chickennuggetmobile" });
            ds.Tables.Add(dt2);


            //  Create the third table of sample data
            DataTable dt3 = new DataTable("Vehicle owners");
            dt3.Columns.Add("User ID", Type.GetType("System.Decimal"));
            dt3.Columns.Add("Vehicle_ID", Type.GetType("System.Decimal"));

            dt3.Rows.Add(new object[] { 1, 1002 });
            dt3.Rows.Add(new object[] { 2, 1000 });
            dt3.Rows.Add(new object[] { 3, 1010 });
            dt3.Rows.Add(new object[] { 5, 1006 });
            dt3.Rows.Add(new object[] { 6, 1007 });
            ds.Tables.Add(dt3);

            return ds;
        }
    }
}
