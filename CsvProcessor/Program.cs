using Parser;
using Processors;
using Processors.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsvProcessor
{
    class Program
    {
        /// <summary>
        /// This project reads csv files and processes its records according to the particular specifications.
        /// The main program (see below) can process big files because it doesn't store any values in collections.
        /// The only disadvantage here is that it has to process each file in two steps (cycles).
        /// The first cycle reads the data and calculates the average values or something else at the end of the first cycle.
        /// The second cycle iterates through the file again and picks up the records which pass some criteria.
        /// 
        /// If we need to implement a new processor then we just have to add a new processor to the Processors project,
        /// e.g. class "Proc_Lp" shows how to implement the whole functionality. Btw. this class also contains static method
        /// Get_Data_Directly which is used only for unit testing and this approach shows the other scenario - instead of
        /// reading the csv file twice we read it only once, store all necessary values into a collection and then process
        /// the data at the end.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), Common.FILESDIR);

            var allFiles = Directory.GetFiles(path, "*.csv");
            var allFilesDictionary = allFiles.Select((s, i) => new { s, i }).ToDictionary(x => x.i + 1, x => x.s);

            var continueProcess = true;
            while (continueProcess)
            {
                Console.WriteLine("0: exit");
                foreach (var file in allFilesDictionary)
                {
                    Console.WriteLine($"{file.Key}: {Path.GetFileName(file.Value)}");
                }

                Console.WriteLine();
                Console.WriteLine("Choose a file from above by a number or zero for exit...");


                string sChosen = Console.ReadLine();
                int nChosen;
                while (!int.TryParse(sChosen, out nChosen) || !IsCorrectNumber(allFilesDictionary, nChosen))
                {
                    Console.WriteLine("Wrong number, try again...");
                    sChosen = Console.ReadLine();
                }

                if (nChosen == 0)
                    continueProcess = false;
                else
                {
                    Console.WriteLine();
                    Console.WriteLine($"Processing file {Path.GetFileName(allFilesDictionary[nChosen])}");
                    Console.WriteLine();

                    // main process...
                    ProcessFile(allFilesDictionary[nChosen]);

                    Console.WriteLine();
                }
            }
        }

        static bool IsCorrectNumber(Dictionary<int,string> allFilesDictionary, int nChosen)
        {
            return nChosen == 0 || allFilesDictionary.ContainsKey(nChosen);
        }

        static void ProcessFile(string fileNameWithPath)
        {
            try
            {
                Reader reader = new Reader();
                reader.ProcessFile(fileNameWithPath);
            }
            catch (ProcessorException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General exception {ex.Message}");
            }
        }
    }
}
