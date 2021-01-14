using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace WebuyParser
{
    class ExcelWriter
    {
        public static void Create(List<Game> list)
        {
            //start excel
            ApplicationClass excapp = new ApplicationClass();

            //if you want to make excel visible           
            excapp.Visible = true;

            //create a blank workbook
            var workbook = excapp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);

            //Not done yet. You have to work on a specific sheet - note the cast
            //You may not have any sheets at all. Then you have to add one with NsExcel.Worksheet.Add()
            var sheet = (Worksheet)workbook.Sheets[1]; //indexing starts from 1

            string cellName;
            int counter = 1;

            foreach (var item in list)
            {
                cellName = "A" + counter.ToString();
                var range = sheet.get_Range(cellName, cellName);
                range.Value2 = item.Name;
                ++counter;
            }

            workbook.SaveAs("report.xlsx");
        }


        public static void WriteCSV<T>(IEnumerable<T> items)
        {
            Type itemType = typeof(T);
            var props = itemType.GetProperties();

            using (var writer = new StreamWriter("report.csv"))
            {
                writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));

                foreach (var item in items)
                {
                    writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null))));
                }
            }
        }
    }
}
