using Dapper;
using Lottery.ML.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.ML.Domain
{
    public interface IRepository
    {

        string DBType { get; }

        /// <summary>
        /// 获取对象数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        T1 Get<T1>(string sql, DynamicParameters param = null);

        /// <summary>
        /// 获取对象列表数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IList<T1> GetList<T1>(string sql, DynamicParameters param = null);
    }

    public interface IRepository<T> : IRepository where T : class, new()
    {
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
        PageDataView<T> GetPageData(string sqlCount, DynamicParameters paramCount, string sqlItem, DynamicParameters paramItem, int pageIndex, int pageSize, string sortField, string sortAsc);

        /// <summary>
        /// 获取对象列表数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IList<T> GetList(string sql, DynamicParameters param = null);

        /// <summary>
        /// 获取对象列表分页数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="beginRow"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        IList<T> GetList(int beginRow, int rowCount, string sql, DynamicParameters param = null);

        /// <summary>
        /// 获取对象数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        T Get(string sql, DynamicParameters param = null);

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        bool Execute(string sql, Object param = null);
    }
}
