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
