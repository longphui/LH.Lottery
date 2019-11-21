using Dapper;
using Lottery.ML.Domain.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.ML.Domain.Infrastructure
{
    public class SqliteLotteryResultRepository: LotteryResultRepository
    {
        public SqliteLotteryResultRepository(IConfiguration config) : base(config)
        {
            this.DBType = "SQLite";
        }

        public override IList<LotteryResult> GetLast100Rows()
        {
            string sql = @" 
        select * from(Select * from LotteryResult order by  id desc limit 99)a order by Id
        ";
            return GetList(sql);
        }

        public override IList<LotteryResult> GetLast100Rows(string id)
        {
            string sql = @" select * from( select * from LotteryResult where Id<@Id
            order by Id desc limit 99)a order by Id";
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("Id", id);
            return GetList(sql, dynamicParams);
        }

        public override LotteryResult GetLastRow()
        {
            string sql = @" Select * from LotteryResult order by  id desc limit 1";
            return Get(sql);
        }

        public override LotteryPredictionLog GetLastSuccessLog()
        {
            string sql = @" select * from LotteryPredictionLog where LotteryCode!='19082' and IsSuccess=1 order by Id desc limit 1";
            return Get<LotteryPredictionLog>(sql);
        }

        public override LotteryResult GetNextLotteryCode(string lotteryCode)
        {
            string sql = @" select * from LotteryResult where Id!='19082' and Id>@Id order by Id limit 1";
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("Id", lotteryCode);
            return Get(sql, dynamicParams);
        }
    }
}
