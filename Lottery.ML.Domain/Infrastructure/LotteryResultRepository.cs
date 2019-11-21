using Dapper;
using Lottery.ML.Domain.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.ML.Domain.Infrastructure
{
    public class LotteryResultRepository : Repository<LotteryResult>, ILotteryResultRepository
    {
        public LotteryResultRepository(IConfiguration config) : base(config)
        {
            
        }
        
        public virtual bool Insert(LotteryResult lotteryResult)
        {
            string sql = @" Insert into LotteryResult(Id,LotteryNo,No1,
                    No2,No3,No4,No5,No6,No7,NoSum,LotteryDate)
                    values(@Id,@LotteryNo,@No1,
                    @No2,@No3,@No4,@No5,@No6,@No7,@NoSum,@LotteryDate)
                   ";
            return Execute(sql, lotteryResult);
        }

        public virtual LotteryResult GetById(string Id)
        {
            string sql = @" select * from LotteryResult where Id=@Id ";
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("Id", Id);
            return Get(sql, dynamicParams);
        }

        public virtual IList<LotteryResult> Get100Rows(int beginRow=0,int rowCount=100)
        {
            string sql = @" select * from LotteryResult order by Id ";
            return GetList(beginRow,rowCount,sql);
        }

        public virtual IList<LotteryResult> GetAll()
        {
            string sql = @" select * from LotteryResult order by Id ";
            return GetList(sql);
        }

        public virtual IList<LotteryResult> GetLast100Rows()
        {
            string sql = @" select * from( select top 99 * from LotteryResult order by Id desc)a order by Id";
            return GetList(sql);
        }

        public virtual IList<LotteryResult> GetLast100Rows(string id)
        {
            string sql = @" select * from( select top 99 * from LotteryResult where Id<@Id
            order by Id desc)a order by Id";
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("Id", id);
            return GetList(sql, dynamicParams);
        }

        public virtual LotteryResult GetLastRow()
        {
            string sql = @" select top 1 * from LotteryResult order by Id desc";
            return Get(sql);
        }


        public virtual bool InsertPredictionResult(LotteryPredictionResult predictionResult)
        {
            string sql = @" Insert into LotteryPredictionResult(PredictionType,LotteryCode,PredictionSite,
                    PredictionResult)
                    values(@PredictionType,@LotteryCode,@PredictionSite,
                    @PredictionResult)
                   ";
            return Execute(sql, predictionResult);
        }

        public virtual bool InsertPredictionLog(LotteryPredictionLog predictionLog)
        {
            string sql = @" Insert into LotteryPredictionLog(LotteryCode,PredictionStatus,StartTime)
                    values(@LotteryCode,@PredictionStatus,@StartTime)
                   ";
            return Execute(sql, predictionLog);
        }

        public virtual bool UpdatePredictionLog(LotteryPredictionLog predictionLog)
        {
            string sql = @" update LotteryPredictionLog set PredictionStatus=@PredictionStatus,
            EndTime=@EndTime,IsSuccess=@IsSuccess,ErrMsg=@ErrMsg where LotteryCode=@LotteryCode ";
            return Execute(sql, predictionLog);
        }

        public virtual LotteryPredictionLog GetLotteryPredictionLog(string lotteryCode)
        {
            string sql = @" select * from LotteryPredictionLog where LotteryCode=@LotteryCode and IsSuccess=1";
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("LotteryCode", lotteryCode);
            return Get<LotteryPredictionLog>(sql, dynamicParams);
        }

        public virtual LotteryPredictionLog GetLastSuccessLog()
        {
            string sql = @" select top 1 * from LotteryPredictionLog where LotteryCode!='19082' and IsSuccess=1 order by Id desc";
            return Get<LotteryPredictionLog>(sql);
        }

        public virtual bool DeleteErrorPrediction(string lotteryCode)
        {
            string sql = @" delete from LotteryPredictionResult where LotteryCode!='19082' and LotteryCode>@LotteryCode ";
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("LotteryCode", lotteryCode);
            return Execute(sql, dynamicParams);
        }

        public virtual LotteryResult GetNextLotteryCode(string lotteryCode)
        {
            string sql = @" select top 1 * from LotteryResult where Id!='19082' and Id>@Id order by Id ";
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("Id", lotteryCode);
            return Get(sql, dynamicParams);
        }

        public virtual bool SetLotteryPredictionTotal(string lotteryCode)
        {
//            string sql = @" insert into LotteryPredictionTotal_4
//(LotteryId, LotteryNos,CodeType,PredictionType,NoCount )
//select lotteryId,group_concat(lotteryNo) as lotteryNos,codetype,
//predictionType,count(lotteryNo) as NoCount
//from LotteryPredictionCode_4
// where  lotteryId=@lotteryId and not exists(select lotteryId from LotteryPredictionTotal_4 where lotteryId=@lotteryId )
// group by lotteryId,codetype,predictionType";
            string sql = @"insert into LotteryPredictionTotal_4
(LotteryId, LotteryNos,CodeType,PredictionType,NoCount )
select * from (
select lotteryId,group_concat(lotteryNo) as lotteryNos,codetype,
predictionType,count(lotteryNo) as NoCount
from LotteryPredictionCode_4
 where  lotteryId=@lotteryId
 group by lotteryId,codetype,predictionType
 union 
 select lotteryId,group_concat(lotteryNo) as lotteryNos,codetype,
predictionType,count(lotteryNo) as NoCount
from LotteryPredictionCode_4_1
 where  lotteryId=@lotteryId
 group by lotteryId,codetype,predictionType) where 
 not exists(select lotteryId from LotteryPredictionTotal_4 where lotteryId=@lotteryId )";
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("lotteryId", lotteryCode);
            return Execute(sql, dynamicParams);
        }

        public virtual bool InsertLotteryPredictionCode_4_Temp(string lotteryCode)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"delete from LotteryPredictionCode_4_Temp;
insert into LotteryPredictionCode_4_Temp(LotteryId, LotteryNo,No1,
No2,No3,No4,CodeType,PredictionType )");
            int c = 0;
            foreach (string predictionType in PredictionType.TypeList)
            {
                if (c != 0)
                {
                    sql.Append(" union ");
                }
                sql.Append($@"select s1.lotteryCode,s1.code||s2.code||'XX' as LotteryNo,
s1.code as No1,s2.code as No2,'X' as No3,'X' as No4 ,
'fixedTwo' as codeType,'{predictionType}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
where s1.PredictionSite='first' and s2.PredictionSite='second' and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,s1.code||'X'||s2.code||'X' as LotteryNo,
s1.code as No1,'X' as No2,s2.code as No3,'X' as No4 ,
'fixedTwo' as codeType,'{predictionType}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
where s1.PredictionSite='first' and s2.PredictionSite='third' and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,s1.code||'X'||'X'||s2.code as LotteryNo,
s1.code as No1,'X' as No2,'X' as No3 ,s2.code as No4,
'fixedTwo' as codeType,'{predictionType}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
where s1.PredictionSite='first' and s2.PredictionSite='fourth' and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,'X'||s1.code||s2.code||'X' as LotteryNo,
'X' as No1,s1.code as No2,s2.code as No3,'X' as No4 ,
'fixedTwo' as codeType,'{predictionType}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
where s1.PredictionSite='second' and s2.PredictionSite='third' and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,'X'||s1.code||'X'||s2.code as LotteryNo,
'X' as No1,s1.code as No2,'X' as No3 ,s2.code as No4,
'fixedTwo' as codeType,'{predictionType}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
where s1.PredictionSite='second' and s2.PredictionSite='fourth' and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,'X'||'X'||s1.code||s2.code as LotteryNo,
'X' as No1,'X' as No2 ,s1.code as No3,s2.code as No4,
'fixedTwo' as codeType,'{predictionType}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
where s1.PredictionSite='third' and s2.PredictionSite='fourth' and s1.LotteryCode=@LotteryCode");
                sql.Append(" union ");
                sql.Append($@"select s1.lotteryCode,s1.code||s2.code||s3.code||'X' as LotteryNo,
s1.code as No1,s2.code as No2,s3.code as No3,'X' as No4 ,
'fixedThree' as codeType,'{predictionType}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
inner join V_{predictionType}_LPR s3 on s1.lotteryCode=s3.lotteryCode
where s1.PredictionSite='first' and s2.PredictionSite='second' and s3.PredictionSite='third'
 and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,s1.code||s2.code||'X'||s3.code as LotteryNo,
s1.code as No1,s2.code as No2,'X' as No3 ,s3.code as No4,
'fixedThree' as codeType,'{predictionType}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
inner join V_{predictionType}_LPR s3 on s1.lotteryCode=s3.lotteryCode
where s1.PredictionSite='first' and s2.PredictionSite='second' and s3.PredictionSite='fourth'
 and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,s1.code||'X'||s2.code||s3.code as LotteryNo,
s1.code as No1,'X' as No2 ,s2.code as No3,s3.code as No4,
'fixedThree' as codeType,'{predictionType}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
inner join V_{predictionType}_LPR s3 on s1.lotteryCode=s3.lotteryCode
where s1.PredictionSite='first' and s2.PredictionSite='third' and s3.PredictionSite='fourth'
 and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,'X'||s1.code||s2.code||s3.code as LotteryNo,
'X' as No1 ,s1.code as No2,s2.code as No3,s3.code as No4,
'fixedThree' as codeType,'{predictionType}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
inner join V_{predictionType}_LPR s3 on s1.lotteryCode=s3.lotteryCode
where s1.PredictionSite='second' and s2.PredictionSite='third' and s3.PredictionSite='fourth'
 and s1.LotteryCode=@LotteryCode");
                sql.Append(" union ");
                sql.Append($@"select s1.lotteryCode,s1.code||s2.code||s3.code||s4.code as LotteryNo,
s1.code as No1,s2.code as No2,s3.code as No3,s4.code as No4 ,
'fixedFour' as codeType,'{predictionType}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
inner join V_{predictionType}_LPR s3 on s1.lotteryCode=s3.lotteryCode
inner join V_{predictionType}_LPR s4 on s1.lotteryCode=s4.lotteryCode
where s1.PredictionSite='first' and s2.PredictionSite='second' and s3.PredictionSite='third'
and s4.PredictionSite='fourth' and s1.LotteryCode=@LotteryCode
");
                Dictionary<string, IList<string>> dicOr = new Dictionary<string, IList<string>>();
                dicOr.Add("Size", new List<string> { "SizeOrParity1", "SizeOrPrime1", "SizeOrPrimeOrParity1" });
                dicOr.Add("Parity", new List<string> { "SizeOrParity1", "PrimeOrParity1", "SizeOrPrimeOrParity1" });
                dicOr.Add("Prime", new List<string> { "SizeOrPrime1", "PrimeOrParity1", "SizeOrPrimeOrParity1" });
                if (predictionType == "Size" || predictionType == "Parity" || predictionType == "Prime")
                {
                    foreach (string codtType1 in dicOr[predictionType])
                    {
                        sql.Append(" union ");
                        sql.Append($@"select s1.lotteryCode,s1.code||s2.code||'XX' as LotteryNo,
s1.code as No1,s2.code as No2,'X' as No3,'X' as No4 ,
'fixedTwo' as codeType,'{codtType1}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
where s1.PredictionSite='first' and s2.PredictionSite='second' and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,s1.code||'X'||s2.code||'X' as LotteryNo,
s1.code as No1,'X' as No2,s2.code as No3,'X' as No4 ,
'fixedTwo' as codeType,'{codtType1}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
where s1.PredictionSite='first' and s2.PredictionSite='third' and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,s1.code||'X'||'X'||s2.code as LotteryNo,
s1.code as No1,'X' as No2,'X' as No3 ,s2.code as No4,
'fixedTwo' as codeType,'{codtType1}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
where s1.PredictionSite='first' and s2.PredictionSite='fourth' and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,'X'||s1.code||s2.code||'X' as LotteryNo,
'X' as No1,s1.code as No2,s2.code as No3,'X' as No4 ,
'fixedTwo' as codeType,'{codtType1}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
where s1.PredictionSite='second' and s2.PredictionSite='third' and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,'X'||s1.code||'X'||s2.code as LotteryNo,
'X' as No1,s1.code as No2,'X' as No3 ,s2.code as No4,
'fixedTwo' as codeType,'{codtType1}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
where s1.PredictionSite='second' and s2.PredictionSite='fourth' and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,'X'||'X'||s1.code||s2.code as LotteryNo,
'X' as No1,'X' as No2 ,s1.code as No3,s2.code as No4,
'fixedTwo' as codeType,'{codtType1}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
where s1.PredictionSite='third' and s2.PredictionSite='fourth' and s1.LotteryCode=@LotteryCode");
                        sql.Append(" union ");
                        sql.Append($@"select s1.lotteryCode,s1.code||s2.code||s3.code||'X' as LotteryNo,
s1.code as No1,s2.code as No2,s3.code as No3,'X' as No4 ,
'fixedThree' as codeType,'{codtType1}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
inner join V_{predictionType}_LPR s3 on s1.lotteryCode=s3.lotteryCode
where s1.PredictionSite='first' and s2.PredictionSite='second' and s3.PredictionSite='third'
 and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,s1.code||s2.code||'X'||s3.code as LotteryNo,
s1.code as No1,s2.code as No2,'X' as No3 ,s3.code as No4,
'fixedThree' as codeType,'{codtType1}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
inner join V_{predictionType}_LPR s3 on s1.lotteryCode=s3.lotteryCode
where s1.PredictionSite='first' and s2.PredictionSite='second' and s3.PredictionSite='fourth'
 and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,s1.code||'X'||s2.code||s3.code as LotteryNo,
s1.code as No1,'X' as No2 ,s2.code as No3,s3.code as No4,
'fixedThree' as codeType,'{codtType1}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
inner join V_{predictionType}_LPR s3 on s1.lotteryCode=s3.lotteryCode
where s1.PredictionSite='first' and s2.PredictionSite='third' and s3.PredictionSite='fourth'
 and s1.LotteryCode=@LotteryCode
union
select s1.lotteryCode,'X'||s1.code||s2.code||s3.code as LotteryNo,
'X' as No1 ,s1.code as No2,s2.code as No3,s3.code as No4,
'fixedThree' as codeType,'{codtType1}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
inner join V_{predictionType}_LPR s3 on s1.lotteryCode=s3.lotteryCode
where s1.PredictionSite='second' and s2.PredictionSite='third' and s3.PredictionSite='fourth'
 and s1.LotteryCode=@LotteryCode");
                        sql.Append(" union ");
                        sql.Append($@"select s1.lotteryCode,s1.code||s2.code||s3.code||s4.code as LotteryNo,
s1.code as No1,s2.code as No2,s3.code as No3,s4.code as No4 ,
'fixedFour' as codeType,'{codtType1}' as PredictionType from V_{predictionType}_LPR s1
inner join V_{predictionType}_LPR s2 on s1.lotteryCode=s2.lotteryCode
inner join V_{predictionType}_LPR s3 on s1.lotteryCode=s3.lotteryCode
inner join V_{predictionType}_LPR s4 on s1.lotteryCode=s4.lotteryCode
where s1.PredictionSite='first' and s2.PredictionSite='second' and s3.PredictionSite='third'
and s4.PredictionSite='fourth' and s1.LotteryCode=@LotteryCode
");
                    }
                }
                c++;
            }
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("LotteryCode", lotteryCode);
            return Execute(sql.ToString(), dynamicParams);
        }

        public virtual bool InsertLotteryPredictionTotal_T(string lotteryCode)
        {
            string sql = @" delete  from LotteryPredictionTotal_T where  LotteryId=@LotteryCode;
insert into LotteryPredictionTotal_T
(LotteryId,CodeType,PredictionType, LotteryNos,NoCount,LotteryNos_W,NoCount_W,WinAmount )
select a.Id as LotteryId,a.codeType,a.PredictionType,
case when t4.lotteryId is null then '' else t4.LotteryNos end  as LotteryNos ,
case when t4.lotteryId is null then 0 else t4.NoCount end as NoCount,
case when w.lotteryId is null then '' else w.LotteryNos end  as LotteryNos_W ,
case when w.lotteryId is null then 0 else w.NoCount end as NoCount_W,
case when w.lotteryId is null then 0 else w.NoCount*a.codeRate end as WinAmount
from(
select* from lotteryResult,LotteryPredictionTypeName,lotteryCodeType_4) a left join
(
select lotteryId,group_concat(lotteryNo) as lotteryNos,codetype,
predictionType,count(lotteryNo) as NoCount
from LotteryPredictionCode_4_Temp
 group by lotteryId,codetype,predictionType) t4 on a.Id=t4.LotteryId and a.codeType=t4.codeType
and a.predictionType=t4.predictionType
 left join (
select lotteryId,group_concat(lotteryNo) as lotteryNos,codetype,
predictionType,count(lotteryNo) as NoCount from (
select c.* from LotteryPredictionCode_4_Temp c inner join lotteryResult r
where  c.lotteryId=r.Id and (c.No1=r.No1 or c.No1='X') 
and (c.No2=r.No2 or c.No2='X') 
and (c.No3=r.No3 or c.No3='X') 
and (c.No4=r.No4 or c.No4='X') )group by lotteryId,codetype,predictionType) w on a.Id=w.LotteryId and a.codeType=w.codeType
and a.predictionType=w.predictionType
where a.id=@LotteryCode  and a.codeType!='fixedOne'";
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("LotteryCode", lotteryCode);
            return Execute(sql, dynamicParams);
        }


    }
}
