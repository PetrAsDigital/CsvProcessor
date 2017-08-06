using LumenWorks.Framework.IO.Csv;
using Processors.Interfaces;
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
                StringBuilder sb = new StringBuilder();
                sb.Append("{" + FileName + "}");
                sb.Append("{" + csvReader[dateTime_Index] + "}");
                sb.Append("{" + energy_str + "}");
                sb.Append("{" + energy_Median + "}");

                Processor_Result.Result.Add(sb.ToString());
            }
        }

        public void ProcessSummaryAgain(int row)
        {
            if (Processor_Result.Result.Count == 0)
                Processor_Result.Description = "Sorry, no abnormal rows to show...";
        }
    }
}
