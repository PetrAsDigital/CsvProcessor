using LumenWorks.Framework.IO.Csv;
using Processors;
using Processors.Interfaces;
using System.IO;

namespace Parser
{
    public class Reader
    {
        public ProcessorResult ProcessFile(string fileNameWithPath)
        {
            // find the right processor by the file name
            Common common = new Common();
            IProcessor processor = common.GetProcessor(fileNameWithPath); 
                       

            // read the file for the first time and calculate what's needed
            using (CsvReader csvReader = new CsvReader(new StreamReader(fileNameWithPath), true))
            {
                int row = 1;    // header has row = 1, data start with row = 2
                processor.Initialize(csvReader);

                while (csvReader.ReadNextRecord())
                {
                    row++;
                    processor.ProcessRow(csvReader, row);
                }

                processor.ProcessSummary(row);
            }

            // process the file again if needed...
            if (processor.CanProcessAgain)
            {
                using (CsvReader csv = new CsvReader(new StreamReader(fileNameWithPath), true))
                {
                    int row = 1;    // header has row = 1, data start with row = 

                    while (csv.ReadNextRecord())
                    {
                        row++;
                        processor.ProcessRowAgain(csv, row);
                    }

                    processor.ProcessSummaryAgain(row);
                }
            }

            processor.PrintResult();

            return processor.GetResult();
        }
    }
}
