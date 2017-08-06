using LumenWorks.Framework.IO.Csv;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parser;
using Processors;
using Processors.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnitTestProcessor
{
    [TestClass]
    public class UnitTestCsv
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), Common.FILESDIR);

        [TestMethod]
        public void TestMethod_LP_Test()
        {
            CheckLpFile("LP_test.csv");
        }

        [TestMethod]
        public void TestMethod_Tou_Test()
        {
            CheckTouFile("TOU_test.csv");
        }

        [TestMethod]
        public void TestMethod_Test_All()
        {
            var allFiles = Directory.GetFiles(path, "*.csv");
            foreach (var item in allFiles)
            {
                var fileName = Path.GetFileName(item);

                if (fileName.StartsWith("LP"))
                    CheckLpFile(fileName);
                else if (fileName.StartsWith("TOU"))
                    CheckTouFile(fileName);
                else
                    throw new Exception($"Type of a file {fileName} not recognised");
            }
        }

        private void CheckLpFile(string fileName)
        {
            string fileNameWithPath = Path.Combine(path, fileName);

            Reader reader = new Reader();
            ProcessorResult processorResult = reader.ProcessFile(fileNameWithPath);

            var lp_Data = Get_Lp_Data(fileNameWithPath);

            Assert.IsTrue(processorResult.Result.SequenceEqual(lp_Data));
        }

        private void CheckTouFile(string fileName)
        {
            string fileNameWithPath = Path.Combine(path, @"TOU_test.csv");

            Reader reader = new Reader();
            ProcessorResult processorResult = reader.ProcessFile(fileNameWithPath);

            var tou_Data = Get_Tou_Data(fileNameWithPath);

            Assert.IsTrue(processorResult.Result.SequenceEqual(tou_Data));
        }

        private List<string> Get_Lp_Data(string fileNameWithPath)
        {
            var result = new List<string>();
            List<Ent_Lp> data = new List<Ent_Lp>();

            using (CsvReader csvReader = new CsvReader(new StreamReader(fileNameWithPath), true))
            {
                var headers = csvReader.GetFieldHeaders();
                var dataValue_Index = Common.GetFieldIndex(headers, Common.DATAVALUE);
                var dateTime_Index = Common.GetFieldIndex(headers, Common.DATETIME);
                int row = 1;

                while (csvReader.ReadNextRecord())
                {
                    row++;
                    var ent_Lp = new Ent_Lp() { DataValue_String = csvReader[dataValue_Index], DataValue = Common.GetFloatValue(csvReader[dataValue_Index], row), DateTime = csvReader[dateTime_Index] };
                    data.Add(ent_Lp);
                }
            }

            var average_value = data.Select(a => a.DataValue).Average();
            var top_value = average_value * 1.2;
            var bottom_value = average_value * 0.8;

            var filteredData = data.Where(a => a.DataValue > top_value || a.DataValue < bottom_value).ToList();
            var fileName = Path.GetFileName(fileNameWithPath);

            foreach (var item in filteredData)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{" + fileName + "}");
                sb.Append("{" + item.DateTime + "}");
                sb.Append("{" + item.DataValue_String + "}");
                sb.Append("{" + average_value + "}");

                result.Add(sb.ToString());
            }

            return result;
        }

        private List<string> Get_Tou_Data(string fileNameWithPath)
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

            var average_value = data.Select(a => a.Energy).Average();
            var top_value = average_value * 1.2;
            var bottom_value = average_value * 0.8;

            var filteredData = data.Where(a => a.Energy > top_value || a.Energy < bottom_value).ToList();
            var fileName = Path.GetFileName(fileNameWithPath);

            foreach (var item in filteredData)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{" + fileName + "}");
                sb.Append("{" + item.DateTime + "}");
                sb.Append("{" + item.Energy_String + "}");
                sb.Append("{" + average_value + "}");

                result.Add(sb.ToString());
            }

            return result;
        }
    }
}
