using LumenWorks.Framework.IO.Csv;
using Processors.Interfaces;
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
                StringBuilder sb = new StringBuilder();
                sb.Append("{" + FileName + "}");
                sb.Append("{" + csvReader[dateTime_Index] + "}");
                sb.Append("{" + dataValue_str + "}");
                sb.Append("{" + dataValue_Median + "}");

                Processor_Result.Result.Add(sb.ToString());
            }
        }

        public void ProcessSummaryAgain(int row)
        {
            if (Processor_Result.Result.Count == 0)
                Processor_Result.Description = "No abnormal values are present in the file";
        }
    }
}
