using LumenWorks.Framework.IO.Csv;
using Processors.Exceptions;

namespace Processors.Processors
{
    public abstract class Processor_base
    {
        private string _fileName;
        private string[] _headers;
        private int _fieldCount;

        
        public string FileName { get { return _fileName; } }
        public int FieldCount { get { return _fieldCount; } }
        public string[] Headers { get { return _headers; } }

        public Processor_base(string fileName)
        {
            _fileName = fileName;
        }

        public virtual void Initialize(CsvReader csvReader)
        {
            _fieldCount = csvReader.FieldCount;
            if (_fieldCount < 1)
                throw new ProcessorException("Selected file contains no data!");

            _headers = csvReader.GetFieldHeaders();
        }

        protected void CheckEmptyFile(int rowNum)
        {
            if (rowNum < 1)
                throw new ProcessorException("Selected file contains no rows!");
        }
    }
}
