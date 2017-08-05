using LumenWorks.Framework.IO.Csv;
using Processors.Exceptions;
using Processors.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Processors.Processors
{
    public class Proc_Lp : Processor_base, IProcessor
    {
        private int dataValue_Index = -1;
        private int dateTime_Index = -1;
        private double dataValue_Median;
        private double dataValue_Top;
        private double dataValue_Bottom;
        private Dictionary<int, double> dataValues = new Dictionary<int, double>();

        public bool CanProcessAgain { get { return true; } }

        public Proc_Lp(string fileName)
            :base(fileName)
        {}

        public override void Initialize(CsvReader csvReader)
        {
            base.Initialize(csvReader);

            for (int i = 0; i < FieldCount; i++)
            {
                switch (Headers[i])
                {
                    case "Data Value":
                        dataValue_Index = i;
                        break;
                    case "Date/Time":
                        dateTime_Index = i;
                        break;
                }
            }

            if (dataValue_Index < 0)
                throw new ProcessorException("Selected file does not have 'Data Value' column!");

            if (dateTime_Index < 0)
                throw new ProcessorException("Selected file does not have 'Date/Time' column!");
        }

        public void ProcessRow(CsvReader csvReader, int row)
        {
            dataValues.Add(row, Common.GetFloatValue(csvReader[dataValue_Index], row));
        }

        public void ProcessSummary(int row)
        {
            var dataRowsNum = row - 1;  // minus header row
            CheckEmptyFile(dataRowsNum);

            dataValue_Median = dataValues.Average(a => a.Value);
            dataValue_Top = dataValue_Median * 1.2;
            dataValue_Bottom = dataValue_Median * 0.8;

            var num = dataValues.Count(a => a.Value > dataValue_Top || a.Value < dataValue_Bottom);

            Console.WriteLine();
            Console.WriteLine($"Abnormal values are present in the following {num} rows:");
        }

        public void ProcessRowAgain(CsvReader csvReader, int row)
        {
            var dataValue = Common.GetFloatValue(csvReader[dataValue_Index], row);
            if (dataValue > dataValue_Top || dataValue < dataValue_Bottom)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{" + FileName + "}");
                sb.Append("{" + csvReader[dateTime_Index] + "}");
                sb.Append("{" + csvReader[dataValue_Index] + "}");
                sb.Append("{" + dataValue_Median + "}");

                Console.WriteLine(sb);
            }
        }

        public void ProcessSummaryAgain(int row)
        {
            // not necessary for this processor
        }
    }
}
