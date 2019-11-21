using Dapper;
using Lottery.ML.Domain.Model;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lottery.ML.Domain.Infrastructure
{
    public class Repository : IRepository
    {
        public string DBType { get; set; }
        readonly string connString;

        public Repository(IConfiguration config) : this(config, "lotteryConn") { }

        public Repository(IConfiguration config, string connStr)
        {
            connString = config.GetConnectionString(connStr);
        }
        protected IDbConnection GetDbConnection()
        {
            switch (this.DBType)
            {
                case "ORACLE":
                    return new OracleConnection(connString);
                case "MSSQL":
                    return new SqlConnection(connString);
                case "SQLite":
                    SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
                    sb.DataSource = AppDomain.CurrentDomain.BaseDirectory + connString; //@"C:\Users\50264\work\project\WayOfLove\data.db";
                    
                    return new SQLiteConnection(sb.ToString());
                default:
                    return new OracleConnection(connString);
            }
        }

        /// <summary>
        /// 获取对象数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T1 Get<T1>(string sql, DynamicParameters param = null)
        {
            using (var DB = GetDbConnection())
            {
                DB.Open();
                T1 data;
                try
                {
                    data = DB.QueryFirstOrDefault<T1>(sql, param);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (DB.State == ConnectionState.Open)
                        DB.Close();
                }
                return data;
            }
        }

        /// <summary>
        /// 获取对象列表数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IList<T1> GetList<T1>(string sql, DynamicParameters param = null)
        {
            using (var DB = GetDbConnection())
            {
                DB.Open();
                IList<T1> data = new List<T1>();
                try
                {
                    data = DB.Query<T1>(sql, param).AsList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (DB.State == ConnectionState.Open)
                        DB.Close();
                }
                return data;
            }
        }

    }

    public class Repository<T> : Repository, IRepository<T> where T : class, new()
    {
        public Repository(IConfiguration config) : base(config) { }

        public Repository(IConfiguration config, string connStr) : base(config, connStr) { }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="sqlCount">获取总行数sql</param>
        /// <param name="paramCount">获取总行数参数</param>
        /// <param name="sqlItem">获取当前页数据sql</param>
        /// <param name="paramItem">获取当前页参数</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页多少行</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortAsc">倒序还是顺序</param>
        /// <returns></returns>
        public PageDataView<T> GetPageData(string sqlCount, DynamicParameters paramCount, string sqlItem, DynamicParameters paramItem, int pageIndex, int pageSize, string sortField, string sortAsc)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                throw new FormatException("分页参数错误");
            }
            if (string.IsNullOrEmpty(sortField))
            {
                throw new FormatException("排序参数错误");
            }
            IList<string> ascList = new List<string> { "asc", "desc" };
            if (!ascList.Contains(sortAsc.ToLower()))
            {
                throw new FormatException("排序参数错误");
            }

            PageDataView<T> pageData = new PageDataView<T>();
            pageData.CurrentPage = pageIndex;
            using (var DB = GetDbConnection())
            {
                DB.Open();
                try
                {
                    pageData.TotalNum = DB.QueryFirstOrDefault<int>(sqlCount, paramCount);
                    pageData.TotalPageCount = pageData.TotalNum / pageSize + (pageData.TotalNum % pageSize > 0 ? 1 : 0);
                    sqlItem = $"select ROW_NUMBER() OVER(Order By {sortField} {sortAsc}) as rowid," + Regex.Replace(sqlItem, "select", "", RegexOptions.IgnoreCase);
                    sqlItem = " select * from (" + sqlItem + ")a where rowid>" + ((pageIndex - 1) * pageSize).ToString() + " and rowid<=" + (pageIndex * pageSize).ToString();
                    pageData.Items = DB.Query<T>(sqlItem, paramItem).AsList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (DB.State == ConnectionState.Open)
                        DB.Close();
                }
                return pageData;
            }
        }

        /// <summary>
        /// 获取对象列表数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IList<T> GetList(string sql, DynamicParameters param = null)
        {
            using (var DB = GetDbConnection())
            {
                DB.Open();
                IList<T> data = new List<T>();
                try
                {
                    data = DB.Query<T>(sql, param).AsList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (DB.State == ConnectionState.Open)
                        DB.Close();
                }
                return data;
            }
        }

        /// <summary>
        /// 获取对象列表分页数据
        /// </summary>
        /// <param name="beginRow"></param>
        /// <param name="rowCount"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IList<T> GetList(int beginRow, int rowCount, string sql, DynamicParameters param = null)
        {
            using (var DB = GetDbConnection())
            {
                DB.Open();
                IList<T> data = new List<T>();
                try
                {
                    data = DB.Query<T>(sql, param).Skip(beginRow).Take(rowCount).AsList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (DB.State == ConnectionState.Open)
                        DB.Close();
                }
                return data;
            }
        }

        /// <summary>
        /// 获取对象数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T Get(string sql, DynamicParameters param = null)
        {
            using (var DB = GetDbConnection())
            {
                DB.Open();
                T data = null;
                try
                {
                    data = DB.QueryFirstOrDefault<T>(sql, param);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (DB.State == ConnectionState.Open)
                        DB.Close();
                }
                return data;
            }
        }

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool Execute(string sql, Object param = null)
        {
            using (var DB = GetDbConnection())
            {
                DB.Open();
                try
                {
                    return DB.Execute(sql, param)>1;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (DB.State == ConnectionState.Open)
                        DB.Close();
                }
            }
        }

    }
}
