using LumenWorks.Framework.IO.Csv;
using Processors.Interfaces;

namespace Processors.Processors
{
    public class Proc_Tou: Processor_base, IProcessor
    {
        public bool CanProcessAgain { get { return true; } }

        public Proc_Tou(string fileName)
            :base(fileName)
        { }

        public override void Initialize(CsvReader csvReader)
        {
            base.Initialize(csvReader);
        }

        public void ProcessRow(CsvReader csvReader, int row)
        {

        }

        public void ProcessSummary(int row)
        {

        }
        public void ProcessRowAgain(CsvReader csvReader, int row)
        {

        }

        public void ProcessSummaryAgain(int row)
        {
            // no summary
        }
    }
}
