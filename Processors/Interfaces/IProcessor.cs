using LumenWorks.Framework.IO.Csv;

namespace Processors.Interfaces
{
    public interface IProcessor
    {
        bool CanProcessAgain { get; }

        void Initialize(CsvReader csvReader);
        void ProcessRow(CsvReader csvReader, int row);
        void ProcessSummary(int row);
        void ProcessRowAgain(CsvReader csvReader, int row);
        void ProcessSummaryAgain(int row);
    }
}
