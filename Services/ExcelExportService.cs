using ClosedXML.Excel;
using MvcCurrency.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCurrency.Services
{
    public class ExcelExportService : IExcelExportService
    {
        public byte[] GetFileContent(List<Rate> rates)
        {
            using (var workbook = new XLWorkbook())
            {
                var sheetName = "ExchangeRate";
                var worksheet = workbook.Worksheets.Add(sheetName);

                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "No";
                worksheet.Cell(currentRow, 2).Value = "EffectiveDate";
                worksheet.Cell(currentRow, 3).Value = "Mid";

                foreach (var rate in rates)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = rate.No;
                    worksheet.Cell(currentRow, 2).Value = rate.EffectiveDate;
                    worksheet.Cell(currentRow, 3).Value = rate.Mid;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }
    }
}
