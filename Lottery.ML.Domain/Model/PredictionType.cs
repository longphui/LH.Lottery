using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.ML.Domain.Model
{
    public class PredictionType
    {
        public static IList<string> TypeList = new List<string>() { "Size",
            "Parity","Prime","SizeAndParity","SizeAndPrime","PrimeAndParity",
            "SizeAndPrimeAndParity","SizeOrParity","SizeOrPrime","PrimeOrParity",
            "SizeOrPrimeOrParity"
        };
    }
}
