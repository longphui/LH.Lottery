using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.ML.Domain.Service
{
    public interface ITrainingGroundService
    {
        void BuildTrainingGround(string rootPath);
    }
}
