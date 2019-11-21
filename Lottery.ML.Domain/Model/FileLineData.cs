using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.ML.Domain.Model
{
    [DelimitedRecord("")]
    public class FileLineData
    {
        public FileLineData() { }
        public FileLineData(string data)
        {
            this.data = data;
        }
        public string data;
    }
    
}
