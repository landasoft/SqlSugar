﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：Queryable扩展函数
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static SqlSugar.Queryable<T> Where<T>(this SqlSugar.Queryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            var type = queryable.Type;
            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression);
            queryable.Params.AddRange(re.Paras);
            queryable.Where.Add(re.SqlWhere);
            return queryable;
        }

        /// <summary>
        /// 条件筛选
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="whereString"></param>
        /// <returns></returns>
        public static SqlSugar.Queryable<T> Where<T>(this SqlSugar.Queryable<T> queryable, string whereString, object whereObj = null)
        {
            var type = queryable.Type;
            string whereStr = string.Format(" AND {0} ", whereString);
            queryable.Where.Add(whereStr);
            if (whereObj != null)
                queryable.Params.AddRange(SqlSugarTool.GetParameters(whereObj));
            return queryable;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="orderFileds">如：id asc,name desc </param>
        /// <returns></returns>
        public static SqlSugar.Queryable<T> OrderBy<T>(this SqlSugar.Queryable<T> queryable, string orderFileds)
        {
            queryable.OrderBy = orderFileds.ToSuperSqlFilter();
            return queryable;
        }
        /// <summary>
        /// 分组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="groupFileds">如：id,name </param>
        /// <returns></returns>
        public static SqlSugar.Queryable<T> GroupBy<T>(this SqlSugar.Queryable<T> queryable, string groupFileds)
        {
            queryable.GroupBy = groupFileds.ToSuperSqlFilter();
            return queryable;
        }

        /// <summary>
        ///  跳过序列中指定数量的元素，然后返回剩余的元素。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static SqlSugar.Queryable<T> Skip<T>(this SqlSugar.Queryable<T> queryable, int index)
        {
            if (queryable.OrderBy.IsNullOrEmpty())
            {
                throw new Exception(".Skip必需使用.Order排序");
            }
            queryable.Skip = index;
            return queryable;
        }

        /// <summary>
        /// 从起始点向后取指定条数的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static SqlSugar.Queryable<T> Take<T>(this SqlSugar.Queryable<T> queryable, int num)
        {
            if (queryable.OrderBy.IsNullOrEmpty())
            {
                throw new Exception(".Take必需使用.OrderBy排序");
            }
            queryable.Take = num;
            return queryable;
        }

        /// <summary>
        ///  返回序列的唯一元素；如果该序列并非恰好包含一个元素，则会引发异常。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static T Single<T>(this  SqlSugar.Queryable<T> queryable)
        {
            return queryable.ToList().Single();
        }

        /// <summary>
        ///  返回序列的唯一元素；如果该序列并非恰好包含一个元素，则会引发异常。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool Any<T>(this  SqlSugar.Queryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            var type = queryable.Type;
            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression);
            queryable.Where.Add(re.SqlWhere);
            queryable.Params.AddRange(re.Paras);
            return queryable.Count() > 0;
        }



        /// <summary>
        ///  确定序列是否包含任何元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static bool Any<T>(this  SqlSugar.Queryable<T> queryable)
        {
            return queryable.Count() > 0;
        }

        /// <summary>
        ///  返回序列的唯一元素；如果该序列并非恰好包含一个元素，则会引发异常。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T Single<T>(this  SqlSugar.Queryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            var type = queryable.Type;
            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression);
            queryable.Where.Add(re.SqlWhere);
            queryable.Params.AddRange(re.Paras);
            return queryable.ToList().Single();
        }

        /// <summary>
        /// 将源数据对象转换到新对象中
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static SqlSugar.Queryable<TResult> Select<TSource, TResult>(this SqlSugar.Queryable<TSource> queryable, Expression<Func<TSource, TResult>> expression)
        {
            var type = typeof(TSource);
            SqlSugar.Queryable<TResult> reval = new Queryable<TResult>()
            {
                DB = queryable.DB,
                OrderBy = queryable.OrderBy,
                Params = queryable.Params,
                Skip = queryable.Skip,
                Take = queryable.Take,
                Where = queryable.Where,
                TableName = type.Name,
                GroupBy = queryable.GroupBy,
                Select = Regex.Match(expression.ToString(), @"(?<=\{).*?(?=\})").Value
            };
            reval.Select = Regex.Replace(reval.Select, @"(?<=\=).*?\.", "");
            return reval;
        }
        /// <summary>
        /// 将源数据对象转换到新对象中
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static SqlSugar.Queryable<TResult> Select<TSource, TResult>(this SqlSugar.Queryable<TSource> queryable, string select)
        {
            var type = typeof(TSource);
            SqlSugar.Queryable<TResult> reval = new Queryable<TResult>()
            {
                DB = queryable.DB,
                OrderBy = queryable.OrderBy,
                Params = queryable.Params,
                Skip = queryable.Skip,
                Take = queryable.Take,
                Where = queryable.Where,
                TableName = type.Name,
                GroupBy = queryable.GroupBy,
                Select = select
            };
            return reval;
        }


        /// <summary>
        /// 获取序列总记录数
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static int Count<T>(this SqlSugar.Queryable<T> queryable)
        {
            StringBuilder sbSql = new StringBuilder();
            string withNoLock = queryable.DB.IsNoLock ? "WITH(NOLOCK)" : null;
            sbSql.AppendFormat("SELECT COUNT({3})  FROM {0} {1} WHERE 1=1 {2} {4} ", queryable.TName, withNoLock, string.Join("", queryable.Where), queryable.Select.GetSelectFiles(), queryable.GroupBy.GetGroupBy());
            var count = queryable.DB.GetInt(sbSql.ToString(), queryable.Params.ToArray());
            return count;
        }


        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="maxField">列</param>
        /// <returns></returns>
        public static TResult Max<TSource, TResult>(this SqlSugar.Queryable<TSource> queryable, string maxField)
        {
            StringBuilder sbSql = new StringBuilder();
            string withNoLock = queryable.DB.IsNoLock ? "WITH(NOLOCK)" : null;
            sbSql.AppendFormat("SELECT MAX({3})  FROM {0} {1} WHERE 1=1 {2} {4} ", queryable.TName, withNoLock, string.Join("", queryable.Where), maxField, queryable.GroupBy.GetGroupBy());
            var objValue = queryable.DB.GetScalar(sbSql.ToString(), queryable.Params.ToArray());
            var reval = Convert.ChangeType(objValue, typeof(TResult));
            return (TResult)reval;
        }

        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="minField">列</param>
        /// <returns></returns>
        public static TResult Min<TSource, TResult>(this SqlSugar.Queryable<TSource> queryable, string minField)
        {
            StringBuilder sbSql = new StringBuilder();
            string withNoLock = queryable.DB.IsNoLock ? "WITH(NOLOCK)" : null;
            sbSql.AppendFormat("SELECT MIN({3})  FROM {0} {1} WHERE 1=1 {2} {4} ", queryable.TName, withNoLock, string.Join("", queryable.Where), minField, queryable.GroupBy.GetGroupBy());
            var objValue = queryable.DB.GetScalar(sbSql.ToString(), queryable.Params.ToArray());
            var reval = Convert.ChangeType(objValue, typeof(TResult));
            return (TResult)reval;
        }

        /// <summary>
        /// 将Queryable转换为List《T》集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this SqlSugar.Queryable<T> queryable)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                string withNoLock = queryable.DB.IsNoLock ? "WITH(NOLOCK)" : null;
                var order = queryable.OrderBy.IsValuable() ? (",row_index=ROW_NUMBER() OVER(ORDER BY " + queryable.OrderBy + " )") : null;

                sbSql.AppendFormat("SELECT " + queryable.Select.GetSelectFiles() + " {1} FROM {0} {2} WHERE 1=1 {3} {4} ", queryable.TableName.IsNullOrEmpty() ? queryable.TName : queryable.TableName, order, withNoLock, string.Join("", queryable.Where), queryable.GroupBy.GetGroupBy());
                if (queryable.Skip == null && queryable.Take != null)
                {
                    sbSql.Insert(0, "SELECT " + queryable.Select.GetSelectFiles() + " FROM ( ");
                    sbSql.Append(") t WHERE t.row_index<=" + queryable.Take);
                }
                else if (queryable.Skip != null && queryable.Take == null)
                {
                    sbSql.Insert(0, "SELECT " + queryable.Select.GetSelectFiles() + " FROM ( ");
                    sbSql.Append(") t WHERE t.row_index>" + (queryable.Skip));
                }
                else if (queryable.Skip != null && queryable.Take != null)
                {
                    sbSql.Insert(0, "SELECT " + queryable.Select.GetSelectFiles() + " FROM ( ");
                    sbSql.Append(") t WHERE t.row_index BETWEEN " + (queryable.Skip + 1) + " AND " + (queryable.Skip + queryable.Take));
                }

                var reader = queryable.DB.GetReader(sbSql.ToString(), queryable.Params.ToArray());
                var reval = SqlSugarTool.DataReaderToList<T>(typeof(T), reader, queryable.Select.GetSelectFiles());
                queryable = null;
                return reval;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("sql:{0}\r\n message:{1}", ex.Message));
            }
            finally
            {
                sbSql = null;
                queryable = null;
            }

        }

        /// <summary>
        /// 将Queryable转换为分页后的List《T》集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <returns></returns>
        public static List<T> ToPageList<T>(this SqlSugar.Queryable<T> queryable, int pageIndex, int pageSize)
        {
            if (queryable.OrderBy.IsNullOrEmpty())
            {
                throw new Exception("分页必需使用.Order排序");
            }
            queryable.Skip = (pageIndex - 1) * pageSize;
            queryable.Take = pageSize;
            return queryable.ToList();
        }


    }
}
