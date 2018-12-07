using System;
using System.Data;
using System.Data.SqlClient;
using Intersystech.CommonIF;
using Intersystech.DataModel;
using Intersystech.ExceptionLibrary;
using Intersystech.Utility;

namespace Intersystech.ConnectionFactory
{
    /// <summary>
    /// SqlServerDbConnectionFactoryクラス
    /// </summary>
    public sealed class SqlServerDbConnectionFactory : DbConnectionFactory
    {
        private SqlConnection sqlConnection;
        private SqlCommand sqlCommand;
        private SqlTransaction sqlTransaction;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SqlServerDbConnectionFactory()
        {
        }

        /// <summary>
        /// データベースへ接続します。
        /// </summary>
        protected override void CreateConnection()
        {
            if (sqlConnection == null)
            {
                sqlConnection = new SqlConnection(this.DbConnectionString);
                sqlCommand = sqlConnection.CreateCommand();
            }
        }

        /// <summary>
        /// 指定するSQLコマンドを実行します。
        /// </summary>
        /// <param name="command">SQLコマンド</param>
        /// <param name="commandTimeout">タイムアウト時間</param>
        /// <returns>Dtoオブジェクト</returns>
        public override ResponseDto ExecuteQuery(string command, int commandTimeout = 0)
        {
            try
            {
                //string message = "SQLコマンド実行";
                //TraceLogger.Information(message);
                //TraceLogger.Information(command);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command, sqlConnection);
                ResponseDto dto = new ResponseDto();
                dto.AffectedResultSet.Clear();

                sqlDataAdapter.SelectCommand.Transaction = sqlTransaction;
                sqlDataAdapter.SelectCommand.CommandTimeout = commandTimeout;
                DateTime startTime = DateTime.Now;
                sqlDataAdapter.Fill(dto.AffectedResultSet);
                DateTime stopTime = DateTime.Now;

                dto.AffectedCount = dto.AffectedResultSet.Tables[0].Rows.Count;

                //message = "SQLコマンド実行時間：" + stopTime.Subtract(startTime) + ", ";
                //message += "実行結果：" + dto.AffectedCount;
                //TraceLogger.Information(message);

                return dto;
            }
            catch (Exception ex)
            {
                DatabaseCustomExceptionMapping.Resolve(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 指定するSQLコマンドを実行します。
        /// </summary>
        /// <param name="command">SQLコマンド</param>
        /// <param name="commandTimeout">タイムアウト時間</param>
        /// <returns>Dtoオブジェクト</returns>
        public override ResponseDto ExecuteScalar(string command, int commandTimeout = 0)
        {
            try
            {
                //string message = "SQLコマンド実行";
                //TraceLogger.Information(message);
                //TraceLogger.Information(command);
                sqlCommand.Transaction = sqlTransaction;
                sqlCommand.CommandTimeout = commandTimeout;
                sqlCommand.CommandText = command;

                ResponseDto dto = new ResponseDto();
                object resultObject = null;
                DateTime startTime = DateTime.Now;
                if (command.StartsWith("DELETE", StringComparison.CurrentCultureIgnoreCase) ||
                    command.StartsWith("INSERT", StringComparison.CurrentCultureIgnoreCase) ||
                    command.StartsWith("UPDATE", StringComparison.CurrentCultureIgnoreCase))
                {
                    resultObject = sqlCommand.ExecuteNonQuery();
                    dto.AffectedCount = Convert.ToInt32(resultObject);
                }
                else if (command.StartsWith("SELECT", StringComparison.CurrentCultureIgnoreCase))
                {
                    resultObject = sqlCommand.ExecuteScalar();
                    dto.AffectedCount = resultObject == null ? 0 : 1;
                }
                DateTime stopTime = DateTime.Now;
                dto.AffectedObject = resultObject;

                //message = "SQLコマンド実行時間：" + stopTime.Subtract(startTime) + ", ";
                //message += "実行結果：" + dto.AffectedObject;
                //TraceLogger.Information(message);

                return dto;
            }
            catch (Exception ex)
            {
                DatabaseCustomExceptionMapping.Resolve(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 指定するSQLコマンドを実行します。
        /// </summary>
        /// <param name="command">SQLコマンド</param>
        /// <param name="commandTimeout">タイムアウト時間</param>
        /// <returns>Dtoオブジェクト</returns>
        public override ResponseDto ExecuteUpdate(string command, int commandTimeout = 0)
        {
            try
            {
                // 行ロック
                UpdateLockIfNeed(command);

                //string message = "SQLコマンド実行";
                //TraceLogger.Information(message);
                //TraceLogger.Information(command);
                sqlCommand.Transaction = sqlTransaction;
                sqlCommand.CommandTimeout = commandTimeout;
                sqlCommand.CommandText = command;

                ResponseDto dto = new ResponseDto();
                DateTime startTime = DateTime.Now;
                dto.AffectedCount = sqlCommand.ExecuteNonQuery();
                DateTime stopTime = DateTime.Now;

                //message = "SQLコマンド実行時間：" + stopTime.Subtract(startTime) + ", ";
                //message += "実行結果：" + dto.AffectedCount;
                //TraceLogger.Information(message);

                return dto;
            }
            catch (Exception ex)
            {
                DatabaseCustomExceptionMapping.Resolve(ex);
                throw ex;
            }
        }

        /// <summary>
        /// データベース接続をOpenします。
        /// </summary>
        public override void Open()
        {
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }
            }
            catch (Exception ex)
            {
                DatabaseCustomExceptionMapping.Resolve(ex);
                throw ex;
            }
        }

        /// <summary>
        /// データベース接続をCloseします。
        /// </summary>
        public override void Close()
        {
            try
            {
                if (sqlConnection.State != ConnectionState.Closed)
                {
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                DatabaseCustomExceptionMapping.Resolve(ex);
                throw ex;
            }
        }

        /// <summary>
        /// トランザクション開始関数
        /// トランザクションの開始を行う。
        /// </summary>
        public override void BeginTransaction()
        {
            try
            {
                sqlTransaction = sqlConnection.BeginTransaction();
            }
            catch (Exception ex)
            {
                DatabaseCustomExceptionMapping.Resolve(ex);
                throw ex;
            }
        }

        /// <summary>
        /// コミット関数
        /// トランザクションのコミットを行う。
        /// </summary>
        public override void Commit()
        {
            try
            {
                if (sqlTransaction != null)
                {
                    sqlTransaction.Commit();
                    sqlTransaction = null;
                }
            }
            catch (Exception ex)
            {
                DatabaseCustomExceptionMapping.Resolve(ex);
                throw ex;
            }
        }

        /// <summary>
        /// ロールバック関数
        /// トランザクションのロールバックを行う。
        /// </summary>
        public override void Rollback()
        {
            try
            {
                if (sqlTransaction != null)
                {
                    sqlTransaction.Rollback();
                    sqlTransaction = null;
                }
            }
            catch (Exception ex)
            {
                DatabaseCustomExceptionMapping.Resolve(ex);
                throw ex;
            }
        }

        /// <summary>
        /// リソースをリリースします。
        /// </summary>
        public override void Dispose()
        {
            this.Close();
            this.sqlCommand = null;
            this.sqlConnection = null;
            this.sqlTransaction = null;
        }
    }
}
