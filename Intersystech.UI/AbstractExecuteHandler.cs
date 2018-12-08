using System;
using System.Linq;
using System.Transactions;
using Intersystech.BusinessFacade;
using Intersystech.CommonIF;
using Intersystech.ConnectionFactory;
using Intersystech.DataModel;
using Intersystech.ExceptionLibrary;
using Intersystech.Extension;
using Intersystech.Utility;

namespace Intersystech.UI
{
    /// <summary>
    /// ExecuteHandler抽象クラス
    /// <remarks>BusinessFacadeメソッドの実行機能を提供します。</remarks>
    /// </summary>
    public abstract class AbstractExecuteHandler
    {
        ///// <summary>
        ///// トレースログインスタンス
        ///// </summary>
        //public TraceLogger TraceLogger = new TraceLogger();

        ///// <summary>
        ///// トレースログ
        ///// </summary>
        //public Log TraceLog = Singleton<Log>.Instance;

        /// <summary>
        /// 接続文字列インデックス
        /// </summary>
        protected int connectionStringIndex = 0;

        /// <summary>
        /// データベース種類
        /// </summary>
        protected DatabaseType databaseType = DatabaseType.None;

        /// <summary>
        /// ページロード日時
        /// </summary>
        protected const string PageLoadTime = "PageLoadTime";

        /// <summary>
        /// 指定したBusinessFacadeクラスのインスタンスを取得します。
        /// </summary>
        /// <typeparam name="T">BusinessFacadeRepository型</typeparam>
        /// <returns>BusinessFacadeのインスタンス</returns>
        protected T GetBusinessFacade<T>() where T : BusinessFacadeRepository
        {
            return Singleton<T>.Instance;
        }

        /// <summary>
        /// BusinessFacadeメソッドを実行します。
        /// </summary>
        /// <typeparam name="TParam">メソッドのパラメータ型</typeparam>
        /// <param name="action">メソッド名</param>
        /// <param name="param">メソッドパラメータ</param>
        protected void Execute<TParam>(Action<TParam> action, TParam param)
        {
            string message = string.Concat(action.Target, ".", action.Method.Name);
            //TraceLogger.Start(message);

            var customAttribute = action.Method.GetCustomAttributes(typeof(TransactionMode), true).FirstOrDefault();

            TransactionMode transactionMode = customAttribute as TransactionMode;

            if (transactionMode.TransactionType == TransactionType.Query)
            {
                // トランザクションモード属性が「検索」の場合、トランザクション制御しない
                using (DbConnectionFactory factory = DbConnectionFactory.CreateFactory(databaseType, connectionStringIndex))
                {
                    try
                    {
                        factory.Open();
                        action(param);
                    }
                    finally
                    {
                        // 接続先データベースをデフォルト値に戻す
                        ChangeConnection();
                    }
                }
            }
            else
            {
                // トランザクションモード属性が「更新」の場合、トランザクション制御する
                using (DbConnectionFactory factory = DbConnectionFactory.CreateFactory(databaseType, connectionStringIndex))
                {
                    try
                    {
                        factory.Open();
                        factory.BeginTransaction();
                        action(param);
                        factory.Commit();
                        //if (transactionMode.TransactionType == TransactionType.Update &&
                        //    (factory.SqlCommandQueue.Contains(SqlCommandType.Insert) ||
                        //    factory.SqlCommandQueue.Contains(SqlCommandType.Update) ||
                        //    factory.SqlCommandQueue.Contains(SqlCommandType.Delete)))
                        //{
                        //    // ページロード時刻再設定
                        //    SessionHelper.Set(PageLoadTime, DateTime.Now);
                        //}
                    }
                    catch (AlertCustomSystemException acsex)
                    {
                        if (Convert.ToBoolean(acsex.Data["RollBackTransaction"]) == false)
                        {
                            factory.Commit();
                        }
                        else
                        {
                            factory.Rollback();
                        }
                        throw acsex;
                    }
                    catch (Exception)
                    {
                        factory.Rollback();
                        throw;
                    }
                    finally
                    {
                        // 接続先データベースをデフォルト値に戻す
                        ChangeConnection();
                    }
                }
            }

            //TraceLogger.Stop(message);
        }

        /// <summary>
        /// 接続先データベースを変更します。
        /// </summary>
        /// <param name="databaseType">データベース種類</param>
        /// <param name="connectionStringIndex">接続文字列インデックス</param>
        public void ChangeConnection(DatabaseType databaseType = DatabaseType.None, int connectionStringIndex = 0)
        {
            this.databaseType = databaseType;
            this.connectionStringIndex = connectionStringIndex;
        }
    }
}