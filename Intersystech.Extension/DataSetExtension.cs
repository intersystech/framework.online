using System;
using System.Data;
using Intersystech.Utility;

namespace Intersystech.Extension
{
    /// <summary>
    /// DataSetの拡張クラス
    /// </summary>
    public static class DataSetExtension
    {
        /// <summary>
        /// TypedDataSetを生成します。
        /// </summary>
        /// <typeparam name="T">TypedDataSet</typeparam>
        /// <param name="dataSet">DataSet</param>
        /// <returns>TypedDataSet</returns>
        public static T CreateDataSet<T>(this DataSet dataSet) where T : DataSet
        {
            try
            {
                T typedDataSet = Activator.CreateInstance<T>();
                typedDataSet.Tables[0].Clear();

                foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                {
                    foreach (DataColumn dataCol in dataSet.Tables[0].Columns)
                    {
                        // 読み取り専用を解除する
                        if (dataCol.ReadOnly == true)
                        {
                            dataCol.ReadOnly = false;
                        }
                        // 列のMaxLengthを無制限にする
                        dataCol.MaxLength = -1;
                        // DBNullの場合、ThrowExceptionを回避する
                        if (dataRow.IsNull(dataCol))
                        {
                            if (dataCol.DataType == typeof(String))
                            {
                                dataRow[dataCol] = string.Empty;
                            }
                            else if (dataCol.DataType == typeof(DateTime))
                            {
                                dataRow[dataCol] = DateTime.MinValue;
                            }
                        }
                    }
                    typedDataSet.Tables[0].ImportRow(dataRow);
                }

                return typedDataSet;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// TypedDataSetをコピーします。
        /// </summary>
        /// <typeparam name="T">TypedDataSet</typeparam>
        /// <param name="dataSet">DataSet</param>
        /// <returns>TypedDataSet</returns>
        public static T Copy<T>(this DataSet dataSet) where T : DataSet
        {
            return dataSet.Copy() as T;
        }

        /// <summary>
        /// TypedDataSetをクローンします。
        /// </summary>
        /// <typeparam name="T">TypedDataSet</typeparam>
        /// <param name="dataSet">DataSet</param>
        /// <returns>TypedDataSet</returns>
        public static T Clone<T>(this DataSet dataSet) where T : DataSet
        {
            return dataSet.Clone() as T;
        }
    }
}
