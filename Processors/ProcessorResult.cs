using System.Collections.Generic;

namespace Processors
{
    public class ProcessorResult
    {
        public string Description { get; set; }
        public List<string> Result { get; set; }

        public ProcessorResult()
        {
            Result = new List<string>();
        }
    }
}
