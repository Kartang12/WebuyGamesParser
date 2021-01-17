using Ganss.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WebuyParser
{
    class ExcelWriter
    {
        private ExcelWriter() 
        {
            mapper = new ExcelMapper();
        }
        private ExcelMapper mapper {get;set;}

        private static ExcelWriter _instance;

        public static ExcelWriter GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ExcelWriter();
            }
            return _instance;
        }

        public void SaveFile(string filename, List<Game> list, string platform)
        {
            _instance.mapper.Save(filename, list, platform, true);
        }
    }
}
