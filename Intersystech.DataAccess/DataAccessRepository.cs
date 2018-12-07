using Intersystech.CommonIF;
using Intersystech.ConnectionFactory;
using Intersystech.DataModel;
using Intersystech.ExceptionLibrary;
using Intersystech.Utility;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Intersystech.DataAccess
{
    /// <summary>
    /// データアクセスリポジトリ
    /// </summary>
    public abstract class DataAccessRepository
    {
        ///// <summary>
        ///// ログ記録インターフェース
        ///// </summary>
        //protected TraceLogger TraceLogger = new TraceLogger();

        private DbConnectionFactory Factory
        {
            get
            {
                return DbConnectionFactory.GetFactory();
            }
        }

        private string MethodName
        {
            get
            {
                StackTrace st = new StackTrace(true);

                string result = string.Empty;
                foreach (StackFrame frame in st.GetFrames())
                {
                    MethodBase method = frame.GetMethod();
                    Type type = method.DeclaringType;
                    if (type.IsSubclassOf(typeof(DataAccessRepository)))
                    {
                        // メソッド名の取得
                        string className = frame.GetMethod().DeclaringType.FullName;
                        string methodName = frame.GetMethod().Name;
                        result = string.Concat(className, ".", methodName);
                        break;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// SQLコマンドを実行します。
        /// </summary>
        /// <param name="command">SQLコマンド</param>
        /// <param name="commandTimeout">SQLコマンドタイムアウト値(秒単位)</param>
        /// <returns>ユーザー定義型Dtoオブジェクト</returns>
        protected ResponseDto Execute(string command, int commandTimeout = 30)
        {
            //TraceLogger.Start(MethodName);

            ResponseDto dto = null;

            SqlCommandType sqlCommandType = GetSqlCommandType(command);
            if (sqlCommandType == SqlCommandType.Select)
            {
                Factory.SqlCommandQueue.Enqueue(SqlCommandType.Select);
                dto = Factory.ExecuteQuery(command, commandTimeout);
            }
            else if (sqlCommandType == SqlCommandType.Insert)
            {
                Factory.SqlCommandQueue.Enqueue(SqlCommandType.Insert);
                dto = Factory.ExecuteUpdate(command, commandTimeout);
            }
            else if (sqlCommandType == SqlCommandType.Update)
            {
                Factory.SqlCommandQueue.Enqueue(SqlCommandType.Update);
                dto = Factory.ExecuteUpdate(command, commandTimeout);
            }
            else
            {
                Factory.SqlCommandQueue.Enqueue(SqlCommandType.Delete);
                dto = Factory.ExecuteUpdate(command, commandTimeout);
            }

            //TraceLogger.Stop(MethodName);

            return dto;
        }

        /// <summary>
        /// SQLコマンドを実行します。
        /// </summary>
        /// <param name="command">SQLコマンド</param>
        /// <param name="commandTimeout">SQLコマンドタイムアウト値(秒単位)</param>
        /// <returns>ユーザー定義型Dtoオブジェクト</returns>
        protected ResponseDto ExecuteScalar(string command, int commandTimeout = 30)
        {
            //TraceLogger.Start(MethodName);
            ResponseDto dto = Factory.ExecuteScalar(command, commandTimeout);

            SqlCommandType sqlCommandType = GetSqlCommandType(command);
            if (sqlCommandType == SqlCommandType.Select)
            {
                Factory.SqlCommandQueue.Enqueue(SqlCommandType.Select);
            }
            else if (sqlCommandType == SqlCommandType.Insert)
            {
                Factory.SqlCommandQueue.Enqueue(SqlCommandType.Insert);
            }
            else if (sqlCommandType == SqlCommandType.Update)
            {
                Factory.SqlCommandQueue.Enqueue(SqlCommandType.Update);
            }
            else
            {
                Factory.SqlCommandQueue.Enqueue(SqlCommandType.Delete);
            }

            //TraceLogger.Stop(MethodName);
            return dto;
        }

        private SqlCommandType GetSqlCommandType(string command)
        {
            string sqlType = command.Substring(0, 6).ToUpper();
            if (sqlType == "SELECT")
            {
                return SqlCommandType.Select;
            }
            else if (sqlType == "INSERT")
            {
                return SqlCommandType.Insert;
            }
            else if (sqlType == "UPDATE")
            {
                return SqlCommandType.Update;
            }
            else if (sqlType == "DELETE")
            {
                return SqlCommandType.Delete;
            }
            else
            {
                throw new CustomSystemException("SELECT、UPDATE、INSERT、DELETE以外のSQLコマンドはサポートしません。");
            }
        }
    }
}
