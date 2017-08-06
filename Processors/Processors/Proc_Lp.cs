﻿using LumenWorks.Framework.IO.Csv;
using Processors.Entities;
using Processors.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Processors.Processors
{
    public class Proc_Lp : Processor_base, IProcessor
    {
        private int dataValue_Index = -1;
        private int dateTime_Index = -1;
        private double dataValue_Sum;
        private double dataValue_Median;
        private double dataValue_Top;
        private double dataValue_Bottom;

        public bool CanProcessAgain { get { return true; } }

        public Proc_Lp(string fileName)
            : base(fileName)
        { }

        public override void Initialize(CsvReader csvReader)
        {
            base.Initialize(csvReader);

            dataValue_Index = Common.GetFieldIndex(Headers, Common.DATAVALUE);
            dateTime_Index = Common.GetFieldIndex(Headers, Common.DATETIME);
        }

        public void ProcessRow(CsvReader csvReader, int row)
        {
            dataValue_Sum += Common.GetFloatValue(csvReader[dataValue_Index], row);
        }

        public void ProcessSummary(int row)
        {
            var dataRowsNum = row - 1;  // minus header row
            CheckEmptyFile(dataRowsNum);

            dataValue_Median = dataValue_Sum / dataRowsNum;
            dataValue_Top = dataValue_Median * 1.2;
            dataValue_Bottom = dataValue_Median * 0.8;

            Processor_Result.Description = "Abnormal values are present in the following rows:";
        }

        public void ProcessRowAgain(CsvReader csvReader, int row)
        {
            var dataValue_str = csvReader[dataValue_Index];
            var dataValue = Common.GetFloatValue(dataValue_str, row);
            if (dataValue > dataValue_Top || dataValue < dataValue_Bottom)
            {
                var formattedResult = GetFormattedRecord(FileName, csvReader[dateTime_Index], dataValue_str, dataValue_Median);
                Processor_Result.Result.Add(formattedResult);
            }
        }

        private static string GetFormattedRecord(string fileName, string dateTime, string dataValue_str, double median)
        {
            StringBuilder result = new StringBuilder();
            result.Append("{" + fileName + "}");
            result.Append("{" + dateTime + "}");
            result.Append("{" + dataValue_str + "}");
            result.Append("{" + median + "}");

            return result.ToString();
        }

        public void ProcessSummaryAgain(int row)
        {
            if (Processor_Result.Result.Count == 0)
                Processor_Result.Description = "No abnormal values are present in the file";
        }


        /// <summary>
        /// This method is used for unit testing only
        /// </summary>
        /// <param name="fileNameWithPath"></param>
        /// <returns></returns>
        public static List<string> Get_Data_Directly(string fileNameWithPath)
        {
            var result = new List<string>();
            List<Ent_Lp> data = new List<Ent_Lp>();

            using (CsvReader csvReader = new CsvReader(new StreamReader(fileNameWithPath), true))
            {
                var headers = csvReader.GetFieldHeaders();
                var dataValue_Index = Common.GetFieldIndex(headers, Common.DATAVALUE);
                var dateTime_Index = Common.GetFieldIndex(headers, Common.DATETIME);
                int row = 1;

                while (csvReader.ReadNextRecord())
                {
                    row++;
                    var ent_Lp = new Ent_Lp() { DataValue_String = csvReader[dataValue_Index], DataValue = Common.GetFloatValue(csvReader[dataValue_Index], row), DateTime = csvReader[dateTime_Index] };
                    data.Add(ent_Lp);
                }
            }

            var median_value = data.Select(a => a.DataValue).Average();
            var top_value = median_value * 1.2;
            var bottom_value = median_value * 0.8;

            var filteredData = data.Where(a => a.DataValue > top_value || a.DataValue < bottom_value).ToList();
            var fileName = Path.GetFileName(fileNameWithPath);

            foreach (var item in filteredData)
            {
                var formattedResult = GetFormattedRecord(fileName, item.DateTime, item.DataValue_String, median_value);
                result.Add(formattedResult);
            }

            return result;
        }
    }
}
