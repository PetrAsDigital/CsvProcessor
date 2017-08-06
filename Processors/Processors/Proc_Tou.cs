using LumenWorks.Framework.IO.Csv;
using Processors.Entities;
using Processors.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Processors.Processors
{
    public class Proc_Tou: Processor_base, IProcessor
    {
        private int energy_Index = -1;
        private int dateTime_Index = -1;
        private double energy_Sum;
        private double energy_Median;
        private double energy_Top;
        private double energy_Bottom;

        public bool CanProcessAgain { get { return true; } }

        public Proc_Tou(string fileName)
            :base(fileName)
        { }

        public override void Initialize(CsvReader csvReader)
        {
            base.Initialize(csvReader);

            energy_Index = Common.GetFieldIndex(Headers, Common.ENERGY);
            dateTime_Index = Common.GetFieldIndex(Headers, Common.DATETIME);
        }

        public void ProcessRow(CsvReader csvReader, int row)
        {
            energy_Sum += Common.GetFloatValue(csvReader[energy_Index], row);
        }

        public void ProcessSummary(int row)
        {
            var dataRowsNum = row - 1;  // minus header row
            CheckEmptyFile(dataRowsNum);

            energy_Median = energy_Sum / dataRowsNum;
            energy_Top = energy_Median * 1.2;
            energy_Bottom = energy_Median * 0.8;

            Processor_Result.Description = "Pls. see abnormal energy values as follows:";
        }
        public void ProcessRowAgain(CsvReader csvReader, int row)
        {
            var energy_str = csvReader[energy_Index];
            var energy = Common.GetFloatValue(energy_str, row);
            if (energy > energy_Top || energy < energy_Bottom)
            {
                var formattedResult = GetFormattedRecord(FileName, csvReader[dateTime_Index], energy_str, energy_Median);
                Processor_Result.Result.Add(formattedResult);
            }
        }

        private static string GetFormattedRecord(string fileName, string dateTime, string energy_str, double median)
        {
            StringBuilder result = new StringBuilder();
            result.Append("{" + fileName + "}");
            result.Append("{" + dateTime + "}");
            result.Append("{" + energy_str + "}");
            result.Append("{" + median + "}");

            return result.ToString();
        }

        public void ProcessSummaryAgain(int row)
        {
            if (Processor_Result.Result.Count == 0)
                Processor_Result.Description = "Sorry, no abnormal rows to show...";
        }


        /// <summary>
        /// This method is used for unit testing only
        /// </summary>
        /// <param name="fileNameWithPath"></param>
        /// <returns></returns>
        public static List<string> Get_Data_Directly(string fileNameWithPath)
        {
            var result = new List<string>();
            List<Ent_Tou> data = new List<Ent_Tou>();

            using (CsvReader csvReader = new CsvReader(new StreamReader(fileNameWithPath), true))
            {
                var headers = csvReader.GetFieldHeaders();
                var energy_Index = Common.GetFieldIndex(headers, Common.ENERGY);
                var dateTime_Index = Common.GetFieldIndex(headers, Common.DATETIME);
                int row = 1;

                while (csvReader.ReadNextRecord())
                {
                    row++;
                    var ent_Tou = new Ent_Tou() { Energy_String = csvReader[energy_Index], Energy = Common.GetFloatValue(csvReader[energy_Index], row), DateTime = csvReader[dateTime_Index] };
                    data.Add(ent_Tou);
                }
            }

            var median_value = data.Select(a => a.Energy).Average();
            var top_value = median_value * 1.2;
            var bottom_value = median_value * 0.8;

            var filteredData = data.Where(a => a.Energy > top_value || a.Energy < bottom_value).ToList();
            var fileName = Path.GetFileName(fileNameWithPath);

            foreach (var item in filteredData)
            {
                var formattedResult = GetFormattedRecord(fileName, item.DateTime, item.Energy_String, median_value);
                result.Add(formattedResult);
            }

            return result;
        }
    }
}
