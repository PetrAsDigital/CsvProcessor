using LumenWorks.Framework.IO.Csv;
using Processors;
using Processors.Interfaces;
using System.IO;

namespace Parser
{
    public class Reader
    {
        /// <summary>
        /// If we want to avoid using too much memory while reading big files (1GB and more) by storing the necessary values
        /// into a collection then we have to process the csv file in two steps.
        /// First we calculate all we need in the first cycle, then we pick up the right records in the second cycle.
        /// </summary>
        /// <param name="fileNameWithPath"></param>
        /// <returns></returns>
        public ProcessorResult ProcessFile(string fileNameWithPath)
        {
            // find the right processor by the file name
            IProcessor processor = Common.GetProcessor(fileNameWithPath); 
                       

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

            // process the file again and pick up the right records (some processors may not need this)...
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

            return processor.GetResult();   // we need the result for unit testing in UnitTestProcessor project
        }
    }
}
