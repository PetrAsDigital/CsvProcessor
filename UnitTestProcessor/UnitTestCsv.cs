using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parser;
using Processors;
using Processors.Processors;
using System;
using System.IO;
using System.Linq;

namespace UnitTestProcessor
{
    [TestClass]
    public class UnitTestCsv
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), Common.FILESDIR);

        [TestMethod]
        public void TestMethod_LP_Test()
        {
            Check_Lp_File("LP_test.csv");
        }

        [TestMethod]
        public void TestMethod_Tou_Test()
        {
            Check_Tou_File("TOU_test.csv");
        }

        /// <summary>
        /// Testing all files within a specific folder
        /// </summary>
        [TestMethod]
        public void TestMethod_Test_All()
        {
            var allFiles = Directory.GetFiles(path, "*.csv");
            foreach (var item in allFiles)
            {
                var fileName = Path.GetFileName(item);

                if (fileName.StartsWith("LP"))
                    Check_Lp_File(fileName);
                else if (fileName.StartsWith("TOU"))
                    Check_Tou_File(fileName);
                else
                    throw new Exception($"Type of a file {fileName} not recognised");
            }
        }

        private void Check_Lp_File(string fileName)
        {
            string fileNameWithPath = Path.Combine(path, fileName);

            Reader reader = new Reader();
            ProcessorResult processorResult = reader.ProcessFile(fileNameWithPath);

            var lp_Data = Proc_Lp.Get_Data_Directly(fileNameWithPath);

            Assert.IsTrue(processorResult.Result.SequenceEqual(lp_Data));
        }

        private void Check_Tou_File(string fileName)
        {
            string fileNameWithPath = Path.Combine(path, fileName);

            Reader reader = new Reader();
            ProcessorResult processorResult = reader.ProcessFile(fileNameWithPath);

            var tou_Data = Proc_Tou.Get_Data_Directly(fileNameWithPath);

            Assert.IsTrue(processorResult.Result.SequenceEqual(tou_Data));
        }
    }
}
