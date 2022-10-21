using JiraWorkloadReportCreator.Entity;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace JiraWorkloadReportCreator.Helpers
{
    public static class StaticExcelHelper
    {
        public static void CreateExcel(List<Report> report, string resultPath)
        {
            using var spreadsheetDocument = SpreadsheetDocument.Create(resultPath, SpreadsheetDocumentType.Workbook);
            var workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();
            var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            worksheetPart.Worksheet = new Worksheet(sheetData);
            var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(newChild: new Sheets());
            Sheet sheet = new()
            {
                Id = spreadsheetDocument.WorkbookPart.
                GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "test"
            };

            UInt32Value i = 0;
            foreach (var item in report)
            {
                var rowInner = new Row() { RowIndex = ++i };
                rowInner.Append(new Cell() { CellReference = "A" + i, CellValue = new CellValue(item.Author), DataType = CellValues.String });
                sheetData.Append(rowInner);

                foreach (var task in item.Tasks)
                {
                    var rowInnerInner = new Row() { RowIndex = ++i };
                    rowInnerInner.Append(new Cell() { CellReference = "B" + i, CellValue = new CellValue(task.Date.ToString("dd.MM.yyyy")), DataType = CellValues.String });
                    rowInnerInner.Append(new Cell() { CellReference = "C" + i, CellValue = new CellValue(task.TaskId), DataType = CellValues.String });
                    rowInnerInner.Append(new Cell() { CellReference = "D" + i, CellValue = new CellValue(task.WorkTime), DataType = CellValues.Number });
                    rowInnerInner.Append(new Cell() { CellReference = "F" + i, CellValue = new CellValue(task.Comment), DataType = CellValues.String });
                    sheetData.Append(rowInnerInner);
                }

                var rowInner1 = new Row() { RowIndex = ++i };
                rowInner1.Append(new Cell() { CellReference = "D" + i, CellValue = new CellValue(item.Sum), DataType = CellValues.Number });
                sheetData.Append(rowInner1);
            }

            sheets.Append(sheet);
            workbookpart.Workbook.Save();
            spreadsheetDocument.Close();
        }
    }
}
