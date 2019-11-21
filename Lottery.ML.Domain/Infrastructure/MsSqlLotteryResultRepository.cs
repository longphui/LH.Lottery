using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.ML.Domain.Infrastructure
{
    public class MsSqlLotteryResultRepository: LotteryResultRepository
    {
        public MsSqlLotteryResultRepository(IConfiguration config) : base(config)
        {
            this.DBType = "MSSQL";
        }
    }
}
