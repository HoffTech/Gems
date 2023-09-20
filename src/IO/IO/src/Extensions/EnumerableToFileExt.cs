// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using FastMember;

using Gems.IO.Attributes;

namespace Gems.IO.Extensions
{
    /// <summary>
    /// Класс для расширения IEnumerable.
    /// </summary>
    public static class EnumerableToFileExt
    {
        /// <summary>
        /// Сохранить коллекцию в файл.
        /// </summary>
        /// <param name="collection">сохраняемая коллекция.</param>
        /// <param name="fileName">полное имя файла.</param>
        /// <returns>признак успешности записи.</returns>
        public static async Task<bool> SaveToFile<T>(this IEnumerable<T> collection, string fileName) where T : class
        {
            if (collection == null || !collection.Any() || fileName == string.Empty)
            {
                return false;
            }

            var table = new DataTable();
            await using (var reader = ObjectReader.Create(collection))
            {
                table.Load(reader);
            }

            using var document = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook);

            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            worksheetPart.Worksheet = new Worksheet(sheetData);

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());
            var sheetArray = new OpenXmlElement[]
            {
                new Sheet
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Sheet1"
                }
            };

            sheets.Append(sheetArray);

            var headerRow = new Row();
            var columns = new List<string>();

            var myFields = typeof(T).GetProperties();

            foreach (var field in myFields)
            {
                if (!(field.GetCustomAttributes(typeof(ColumnHeaderNameAttribute), false) is
                    ColumnHeaderNameAttribute[] attributes))
                {
                    continue;
                }

                columns.Add(field.Name);

                foreach (var attribute in attributes)
                {
                    var curCell = new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(attribute.HeaderName)
                    };

                    headerRow.AppendChild(curCell);
                }
            }

            sheetData.AppendChild(headerRow);

            foreach (DataRow dsRow in table.Rows)
            {
                var newRow = new Row();
                foreach (var cell in columns.Select(col => new Cell
                {
                    DataType = CellValues.String,
                    CellValue = new CellValue(dsRow[col].ToString() ?? string.Empty)
                }))
                {
                    newRow.AppendChild(cell);
                }

                sheetData.AppendChild(newRow);
            }

            workbookPart.Workbook.Save();

            return true;
        }
    }
}
