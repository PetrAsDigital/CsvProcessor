﻿using Processors.Exceptions;
using Processors.Interfaces;
using Processors.Processors;
using System.IO;

namespace Processors
{
    public class Common
    {
        public IProcessor GetProcessor(string fileNameWithPath)
        {
            if (string.IsNullOrEmpty(fileNameWithPath))
                throw new ProcessorException("FileName is empty!");

            if (!File.Exists(fileNameWithPath))
                throw new ProcessorException($"File {fileNameWithPath} doesn't exist!");

            var fileName = Path.GetFileName(fileNameWithPath);
            if (string.IsNullOrEmpty(fileName))
                throw new ProcessorException("Unknown fileName!");


            IProcessor processor = null;

            if (fileName.StartsWith("LP"))
                processor = new Proc_Lp(fileName);
            else if (fileName.StartsWith("TOU"))
                processor = new Proc_Tou(fileName);

            if (processor == null)
                throw new ProcessorException("Processor hasn't been assigned due to unknown filename!");

            return processor;
        }

        public static float GetFloatValue(string value, int line)
        {
            float result;
            if (!float.TryParse(value, out result))
                throw new ProcessorException($"Error while trying to parse value {value} at line {line}");

            return result;
        }
    }
}