using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebuyParser
{
    class ExcelWriter
    {

        public static void WriteCSV<T>(IEnumerable<T> items)
        {
            Type itemType = typeof(T);
            var props = itemType.GetProperties();

            using (var writer = new StreamWriter("report.csv"))
            {
                writer.WriteLine(string.Join("/ ", props.Select(p => p.Name)));

                foreach (var item in items)
                {
                    writer.WriteLine(string.Join("/ ", props.Select(p => p.GetValue(item, null))));
                }
            }
        }
    }
}
