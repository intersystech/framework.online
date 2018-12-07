using System;
using System.Data;
using System.Data.OleDb;
using Intersystech.CommonIF;
using Intersystech.DataModel;
using Intersystech.ExceptionLibrary;
using Intersystech.Utility;

namespace Intersystech.ConnectionFactory
{
    /// <summary>
    /// OleDbConnectionFactoryクラス
    /// </summary>
    public sealed class OleDbConnectionFactory : DbConnectionFactory
    {
        private OleDbConnection oleDbConnection;
        private OleDbCommand oleDbCommand;
        private OleDbTransaction oleDbTransaction;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OleDbConnectionFactory()
        {
        }

        /// <summary>
        /// データベースへ接続します。
        /// </summary>
        protected override void CreateConnection()
        {
            if (oleDbConnection == null)
            {
                oleDbConnection = new OleDbConnection(this.DbConnectionString);
                oleDbCommand = oleDbConnection.CreateCommand();
            }
        }

        /// <summary>
        /// データベース接続をOpenします。
        /// </summary>
        public override void Open()
        {
            try
            {
                if (oleDbConnection.State == ConnectionState.Closed)
                {
                    oleDbConnection.Open();
                }
            }
            catch (Exception ex)
            {
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
                if (oleDbConnection.State != ConnectionState.Closed)
                {
                    oleDbConnection.Close();
                }
            }
            catch (Exception ex)
            {
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
                oleDbTransaction = oleDbConnection.BeginTransaction();
            }
            catch
            {
                throw;
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
                if (oleDbTransaction != null)
                {
                    oleDbTransaction.Commit();
                    oleDbTransaction = null;
                }
            }
            catch
            {
                throw;
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
                if (oleDbTransaction != null)
                {
                    oleDbTransaction.Rollback();
                    oleDbTransaction = null;
                }
            }
            catch
            {
                throw;
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
                string message = "SQLコマンド実行";
                //TraceLogger.Information(message);
                //TraceLogger.Information(command);

                OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(command, oleDbConnection);
                ResponseDto dto = new ResponseDto();
                dto.AffectedResultSet.Clear();

                oleDbDataAdapter.SelectCommand.Transaction = oleDbTransaction;
                oleDbDataAdapter.SelectCommand.CommandTimeout = commandTimeout;
                DateTime startTime = DateTime.Now;
                oleDbDataAdapter.Fill(dto.AffectedResultSet);
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

                oleDbCommand.Transaction = oleDbTransaction;
                oleDbCommand.CommandTimeout = commandTimeout;
                oleDbCommand.CommandText = command;

                ResponseDto dto = new ResponseDto();
                object resultObject = null;
                DateTime startTime = DateTime.Now;
                if (command.StartsWith("DELETE", StringComparison.CurrentCultureIgnoreCase) ||
                    command.StartsWith("INSERT", StringComparison.CurrentCultureIgnoreCase) ||
                    command.StartsWith("UPDATE", StringComparison.CurrentCultureIgnoreCase))
                {
                    resultObject = oleDbCommand.ExecuteNonQuery();
                    dto.AffectedCount = Convert.ToInt32(resultObject);
                }
                else if (command.StartsWith("SELECT", StringComparison.CurrentCultureIgnoreCase))
                {
                    resultObject = oleDbCommand.ExecuteScalar();
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
                oleDbCommand.Transaction = oleDbTransaction;
                oleDbCommand.CommandTimeout = commandTimeout;
                oleDbCommand.CommandText = command;

                ResponseDto dto = new ResponseDto();
                DateTime startTime = DateTime.Now;
                dto.AffectedCount = oleDbCommand.ExecuteNonQuery();
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
        /// リソースをリリースします。
        /// </summary>
        public override void Dispose()
        {
            this.Close();
            this.oleDbCommand = null;
            this.oleDbConnection = null;
            this.oleDbTransaction = null;
        }
    }
}
