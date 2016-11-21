﻿/*
    版权所有:杭州火图科技有限公司
    地址:浙江省杭州市滨江区西兴街道阡陌路智慧E谷B幢4楼在地图中查看
    (c) Copyright Hangzhou Hot Technology Co., Ltd.
    Floor 4,Block B,Wisdom E Valley,Qianmo Road,Binjiang District
    2013-216. All rights reserved.
**/


using BAMENG.CONFIG;
using BAMENG.MODEL;
using HotCoreUtils.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BAMENG.DAL
{
    public class AbstractDAL : IDisposable
    {



        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~AbstractDAL() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// 设置手机号码部分隐藏
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public string DoMoblieView(string mobile)
        {
            if (mobile.Length != 11) return mobile;
            string result = "";
            for (var i = 0; i < 11; i++)
            {
                result += (i < 3 || i > 8) ? mobile.Substring(i, 1) : "*";
            }
            return result;
        }

        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        ///<param name="PageSize"></param>
        ///<param name="PageIndex"></param>
        /// <param name="strSql"></param>
        ///<param name="orderbyField">排序字段</param>
        /// <param name="commandParameters"></param>
        /// <param name="callback">回调处理函数</param>
        /// <returns></returns>
        public ResultPageModel getPageData<T>(int PageSize, int PageIndex, string strSql, string orderbyField, SqlParameter[] commandParameters, Action<List<T>> callback, bool orderby = false) where T : new()
        {
            PageSize = PageSize > 0 ? PageSize : 20;
            string querySql = DbHelperSQLP.buildPageSql(PageIndex, PageSize, strSql, orderbyField, orderby);
            string recordCountSql = DbHelperSQLP.buildRecordCountSql(strSql);

            ResultPageModel result = new ResultPageModel();
            int recordCount = Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, recordCountSql, commandParameters));
            List<T> lst = new List<T>();
            using (SqlDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, querySql, commandParameters))
            {
                lst = DbHelperSQLP.GetEntityList<T>(dr);
                callback?.Invoke(lst);
                //callback.Invoke(lst);
            }

            int pageCount = recordCount / PageSize;
            if (recordCount % PageSize != 0)
            {
                ++pageCount;
            }
            result.PageIndex = PageIndex;
            result.PageSize = PageSize;
            result.Total = recordCount;
            result.PageCount = pageCount;
            result.Rows = lst;
            return result;
        }

        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="strSql"></param>
        /// <param name="orderbyField">排序字段</param>
        /// <param name="orderby"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public ResultPageModel getPageData<T>(int PageSize, int PageIndex, string strSql, string orderbyField, bool orderby, params SqlParameter[] commandParameters) where T : new()
        {
            PageSize = PageSize > 0 ? PageSize : 20;
            string querySql = DbHelperSQLP.buildPageSql(PageIndex, PageSize, strSql, orderbyField, orderby);
            string recordCountSql = DbHelperSQLP.buildRecordCountSql(strSql);

            ResultPageModel result = new ResultPageModel();
            int recordCount = Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, recordCountSql, commandParameters));
            List<T> lst = new List<T>();
            using (SqlDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, querySql, commandParameters))
            {
                lst = DbHelperSQLP.GetEntityList<T>(dr);
            }

            int pageCount = recordCount / PageSize;
            if (recordCount % PageSize != 0)
            {
                ++pageCount;
            }
            result.PageIndex = PageIndex;
            result.PageSize = PageSize;
            result.Total = recordCount;
            result.PageCount = pageCount;
            result.Rows = lst;
            return result;
        }


        public ResultPageModel getPageData<T>(int PageSize, int PageIndex, string strSql, string orderbyField,params SqlParameter[] commandParameters) where T : new()
        {
            PageSize = PageSize > 0 ? PageSize : 20;
            string querySql = DbHelperSQLP.buildPageSql(PageIndex, PageSize, strSql, orderbyField);
            string recordCountSql = DbHelperSQLP.buildRecordCountSql(strSql);

            ResultPageModel result = new ResultPageModel();
            int recordCount = Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, recordCountSql, commandParameters));
            List<T> lst = new List<T>();
            using (SqlDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, querySql, commandParameters))
            {
                lst = DbHelperSQLP.GetEntityList<T>(dr);
            }

            int pageCount = recordCount / PageSize;
            if (recordCount % PageSize != 0)
            {
                ++pageCount;
            }
            result.PageIndex = PageIndex;
            result.PageSize = PageSize;
            result.Total = recordCount;
            result.PageCount = pageCount;
            result.Rows = lst;
            return result;
        }

        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="strSql"></param>
        /// <param name="orderbyField">排序字段</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public ResultPageModel getPageData<T>(int PageSize, int PageIndex, string strSql, string orderbyField, Action<List<T>> callback, bool orderby = false) where T : new()
        {
            PageSize = PageSize > 0 ? PageSize : 20;
            string querySql = DbHelperSQLP.buildPageSql(PageIndex, PageSize, strSql, orderbyField, orderby);
            string recordCountSql = DbHelperSQLP.buildRecordCountSql(strSql);

            ResultPageModel result = new ResultPageModel();
            int recordCount = Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, recordCountSql));
            List<T> lst = new List<T>();
            using (SqlDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, querySql))
            {
                lst = DbHelperSQLP.GetEntityList<T>(dr);
                callback?.Invoke(lst);
                //callback.Invoke(lst);
            }

            int pageCount = recordCount / PageSize;
            if (recordCount % PageSize != 0)
            {
                ++pageCount;
            }
            result.PageIndex = PageIndex;
            result.PageSize = PageSize;
            result.Total = recordCount;
            result.PageCount = pageCount;
            result.Rows = lst;
            return result;
        }

        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="strSql"></param>
        /// <param name="orderbyField">排序字段</param>
        /// <returns></returns>
        public ResultPageModel getPageData<T>(int PageSize, int PageIndex, string strSql, string orderbyField, bool orderby = false) where T : new()
        {
            PageSize = PageSize > 0 ? PageSize : 20;
            string querySql = DbHelperSQLP.buildPageSql(PageIndex, PageSize, strSql, orderbyField, orderby);
            string recordCountSql = DbHelperSQLP.buildRecordCountSql(strSql);

            ResultPageModel result = new ResultPageModel();
            int recordCount = Convert.ToInt32(DbHelperSQLP.ExecuteScalar(WebConfig.getConnectionString(), CommandType.Text, recordCountSql));
            List<T> lst = new List<T>();
            using (SqlDataReader dr = DbHelperSQLP.ExecuteReader(WebConfig.getConnectionString(), CommandType.Text, querySql))
            {
                lst = DbHelperSQLP.GetEntityList<T>(dr);
            }

            int pageCount = recordCount / PageSize;
            if (recordCount % PageSize != 0)
            {
                ++pageCount;
            }
            result.PageIndex = PageIndex;
            result.PageSize = PageSize;
            result.Total = recordCount;
            result.PageCount = pageCount;
            result.Rows = lst;
            return result;
        }

        /// <summary>   
        /// 根据需要得实体类信息   
        /// </summary>   
        /// <typeparam name="T">需要一个对象有一个无参数的实例化方法</typeparam>   
        /// <param name="dr">table数据源</param>   
        /// <returns>返回整理好了集合</returns>           
        public static List<T> GetEntityList<T>(IDataReader dr) where T : new()
        {
            List<T> entityList = new List<T>();

            int fieldCount = -1;

            while (dr.Read())
            {
                if (-1 == fieldCount)
                    fieldCount = dr.FieldCount;

                // 得到实体类对象   
                T t = (T)Activator.CreateInstance(typeof(T));

                for (int i = 0; i < fieldCount; i++)
                {
                    PropertyInfo prop = t.GetType().GetProperty(dr.GetName(i),
                        BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

                    if (null != prop)
                    {
                        // 为了能用在默认为null的值上   
                        // 如 DateTime? tt = null;
                        if (null == dr[i] || Convert.IsDBNull(dr[i]))
                        {
                            if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Date")
                                prop.SetValue(t, DateTime.Parse("1975-1-1"), null);
                            else if (prop.PropertyType.Name == "String")
                                prop.SetValue(t, "", null);
                            else
                                prop.SetValue(t, null, null);
                        }
                        else
                            prop.SetValue(t, dr[i], null);
                    }
                }

                entityList.Add(t);
            }
            dr.Close();
            return entityList;
        }

        public T GetEntity<T>(IDataReader dr) where T : new()
        {
            T entity = default(T);

            int fieldCount = -1;

            if (dr.Read())
            {
                if (-1 == fieldCount)
                    fieldCount = dr.FieldCount;

                // 得到实体类对象   
                T t = (T)Activator.CreateInstance(typeof(T));

                for (int i = 0; i < fieldCount; i++)
                {
                    PropertyInfo prop = t.GetType().GetProperty(dr.GetName(i),
                        BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

                    if (null != prop)
                    {
                        // 为了能用在默认为null的值上   
                        // 如 DateTime? tt = null;   
                        if (null == dr[i] || Convert.IsDBNull(dr[i]))
                            if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Date")
                                prop.SetValue(t, DateTime.Parse("1975-1-1"), null);
                            else if (prop.PropertyType.Name == "String")
                                prop.SetValue(t, "", null);
                            else
                                prop.SetValue(t, null, null);
                        else
                            prop.SetValue(t, dr[i], null);
                    }
                }
                entity = t;
            }
            dr.Close();
            return entity;
        }

    }

}
