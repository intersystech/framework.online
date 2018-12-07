using System;
using System.Configuration;
using System.Web.Configuration;
using Intersystech.CommonIF;
using Intersystech.DataModel;
using Intersystech.Utility;
using System.Collections;
using Intersystech.ExceptionLibrary;

namespace Intersystech.ConnectionFactory
{
    /// <summary>
    /// DbConnectionFactoryクラス
    /// </summary>
    public abstract class DbConnectionFactory : IDisposable
    {
        #region 変数

        /// <summary>
        /// DbConnectionFactoryオブジェクト
        /// </summary>
        [ThreadStatic]
        private static DbConnectionFactory factory = null;

        ///// <summary>
        ///// ログ作成インタフェース
        ///// </summary>
        //public TraceLogger TraceLogger = new TraceLogger();

        ///// <summary>
        ///// ログオブジェクト
        ///// </summary>
        //public Log TraceLog = Singleton<Log>.Instance;

        /// <summary>
        /// トランザクション単位で実行されるSQLコマンド種類のコレクション
        /// </summary>
        public Queue SqlCommandQueue = new Queue();

        /// <summary>
        /// データベースカスタム例外マッピングインターフェース
        /// </summary>
        public IDatabaseCustomExceptionMapping DatabaseCustomExceptionMapping = Singleton<DatabaseCustomExceptionMapping>.Instance;

        #endregion

        #region 定数
        /// <summary>
        /// ページロード日時
        /// </summary>
        protected const string PageLoadTime = "PageLoadTime";

        /// <summary>
        /// 最終更新日時
        /// </summary>
        protected const string LastUpdatedDatetime = "LastUpdatedDatetime";

        #endregion

        #region プロパティ

        /// <summary>
        /// 接続文字列インデックス
        /// </summary>
        private static int ConnectionStringIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 接続文字列インデックス
        /// </summary>
        private static DatabaseType databaseType;
        /// <summary>
        /// データベース種類
        /// </summary>
        private static DatabaseType DatabaseType
        {
            get
            {
                if (databaseType != DatabaseType.None)
                    return databaseType;

                string databaseTypeStr = ConfigHelper.GetAppSettingsValue("DatabaseType");
                return (DatabaseType)Enum.Parse(typeof(DatabaseType), databaseTypeStr);
            }
            set
            {
                databaseType = value;
            }
        }

        /// <summary>
        /// データベース接続文字列
        /// </summary>
        protected string DbConnectionString
        {
            get
            {
                ConnectionStringSettingsCollection connectionStrings = WebConfigurationManager.ConnectionStrings;
                if (connectionStrings.Count == 0)
                    return string.Empty;

                return connectionStrings[ConnectionStringIndex].ToString();
            }
        }

        /// <summary>
        /// 排他チェック用列名
        /// </summary>
        protected string ExclusiveColumn
        {
            get
            {
                return ConfigHelper.GetAppSettingsValue("ExclusiveColumn");
            }
        }

        /// <summary>
        /// 更新時に行単位のロックを行うか
        /// </summary>
        protected bool DoUpdateLock
        {
            get
            {
                return ConfigHelper.GetAppSettingsValue("DoUpdateLock") == "1";
            }
        }
        #endregion

        #region メソッド

        /// <summary>
        /// データベース種類に応じて、接続します。
        /// </summary>
        /// <param name="databaseType">データベース種類</param>
        /// <param name="connectionStringIndex">接続文字列インデックス</param>
        /// <returns>DbConnectionFactoryオブジェクト</returns>
        public static DbConnectionFactory CreateFactory(DatabaseType databaseType, int connectionStringIndex)
        {
            DatabaseType = databaseType;
            ConnectionStringIndex = connectionStringIndex;

            //if (DatabaseType == DatabaseType.MySQL)
            //{
            //    factory = new MySqlDbConnectionFactory();
            //    factory.CreateConnection();
            //}
            //else if (DatabaseType == DatabaseType.Oracle)
            //{
            //    factory = new OracleDbConnectionFactory();
            //    factory.CreateConnection();
            //}
            if (DatabaseType == DatabaseType.SQLServer)
            {
                factory = new SqlServerDbConnectionFactory();
                factory.CreateConnection();
            }
            else if (DatabaseType == DatabaseType.Access)
            {
                factory = new OleDbConnectionFactory();
                factory.CreateConnection();
            }
            return factory;
        }

        /// <summary>
        /// データベースファクトリを取得します。
        /// </summary>
        /// <returns>DbConnectionFactoryオブジェクト</returns>
        public static DbConnectionFactory GetFactory()
        {
            return factory;
        }

        /// <summary>
        /// データベースへ接続します。
        /// </summary>
        protected abstract void CreateConnection();

        /// <summary>
        /// 検索用SQLコマンドを実行します。
        /// </summary>
        /// <param name="command">SQLコマンド</param>
        /// <param name="commandTimeout">タイムアウト時間</param>
        /// <returns>Dtoオブジェクト</returns>
        public abstract ResponseDto ExecuteQuery(string command, int commandTimeout = 0);

        /// <summary>
        /// 指定するSQLコマンドを実行し、返す結果セットの最初の行にある最初の列を返します。 残りの列または行は無視されます。
        /// </summary>
        /// <param name="command">SQLコマンド</param>
        /// <param name="commandTimeout">タイムアウト時間</param>
        /// <returns>Dtoオブジェクト</returns>
        public abstract ResponseDto ExecuteScalar(string command, int commandTimeout = 0);

        /// <summary>
        /// 更新用SQLコマンドを実行します。
        /// </summary>
        /// <param name="command">SQLコマンド</param>
        /// <param name="commandTimeout">タイムアウト時間</param>
        /// <returns>Dtoオブジェクト</returns>
        public abstract ResponseDto ExecuteUpdate(string command, int commandTimeout = 0);

        /// <summary>
        /// データベース接続を開きます。
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// データベースへの接続を閉じます。
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// トランザクションを開始します。
        /// </summary>
        public abstract void BeginTransaction();

        /// <summary>
        /// トランザクションをコミットします。
        /// </summary>
        public abstract void Commit();

        /// <summary>
        /// トランザクションをロールバックします。
        /// </summary>
        public abstract void Rollback();

        /// <summary>
        /// データが変更されたか
        /// </summary>
        /// <param name="command">実行SQL</param>
        /// <returns>False：変更されていない、True：変更された</returns>
        [Obsolete]
        protected bool IsDataDirty(string command)
        {
            try
            {
                string strSQL = command.Trim();
                if (strSQL.StartsWith("INSERT", StringComparison.CurrentCultureIgnoreCase) ||
                    strSQL.StartsWith("DELETE", StringComparison.CurrentCultureIgnoreCase))
                {
                    // DELETE、INSERTの場合、排他しない
                    return false;
                }

                // SQL文変換
                string strSQLCheck = string.Empty;
                string exclusiveSql = "SELECT MAX(" + this.ExclusiveColumn + ") AS update_datetime";

                // UPDATEの場合
                if (strSQL.StartsWith("UPDATE", StringComparison.CurrentCultureIgnoreCase))
                {
                    int setIndex = strSQL.IndexOf("SET", 0, StringComparison.CurrentCultureIgnoreCase);
                    string tblStr = strSQL.Substring(6, setIndex - 6);
                    int whereIndex = strSQL.LastIndexOf("WHERE", StringComparison.CurrentCultureIgnoreCase);
                    string whereStr = strSQL.Substring(whereIndex);

                    strSQLCheck = string.Concat(exclusiveSql, " FROM ", tblStr, " ", whereStr);
                    if (this.DoUpdateLock)
                    {
                        if (DatabaseType == DatabaseType.MySQL || DatabaseType == DatabaseType.Oracle)
                        {
                            // MySQL、Oracle：FOR UPDATE
                            string strSQLUpdLock = string.Concat("SELECT * FROM ", tblStr, " ", whereStr, " FOR UPDATE");
                            this.ExecuteScalar(strSQLUpdLock);
                        }
                        else if (DatabaseType == DatabaseType.SQLServer)
                        {
                            // SQLServer：WITH (UPDLOCK)
                            string strSQLUpdLock = string.Concat("SELECT * FROM ", tblStr, " WITH (UPDLOCK) ", whereStr);
                            this.ExecuteScalar(strSQLUpdLock);
                        }
                    }
                }

                // 更新日時
                object result = this.ExecuteScalar(strSQLCheck).AffectedObject;
                if (result == DBNull.Value || result == null)
                {
                    // 該当データがない場合、排他しない
                    return false;
                }

                // ページロード日時がシステム日時より大きい場合、falseを返す
                DateTime pageLoadTime = Convert.ToDateTime(SessionHelper.Get(DbConnectionFactory.PageLoadTime));
                return pageLoadTime.CompareTo(result) < 0;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 行ロックの必要な場合かつUPDATEコマンドの場合、行ロックを行います。
        /// </summary>
        /// <param name="command">実行SQL</param>
        /// <returns>false：行ロック不要、true：行ロック成功</returns>
        protected void UpdateLockIfNeed(string command)
        {
            try
            {
                // 行ロック不要な場合
                if (!this.DoUpdateLock)
                    return;
                // Accessの場合
                if (DatabaseType == DatabaseType.Access)
                    return;

                string strSQL = command.Trim();
                // DELETE、INSERTの場合
                if (strSQL.StartsWith("INSERT", StringComparison.CurrentCultureIgnoreCase) ||
                    strSQL.StartsWith("DELETE", StringComparison.CurrentCultureIgnoreCase))
                    return;

                // SQL文変換
                // UPDATEの場合
                if (strSQL.StartsWith("UPDATE", StringComparison.CurrentCultureIgnoreCase))
                {
                    int setIndex = strSQL.IndexOf("SET", 0, StringComparison.CurrentCultureIgnoreCase);
                    string tblStr = strSQL.Substring(6, setIndex - 6);
                    int whereIndex = strSQL.LastIndexOf("WHERE", StringComparison.CurrentCultureIgnoreCase);
                    string whereStr = strSQL.Substring(whereIndex);

                    if (DatabaseType == DatabaseType.MySQL || DatabaseType == DatabaseType.Oracle)
                    {
                        // MySQL、Oracle：FOR UPDATE
                        string strSQLUpdLock = string.Concat("SELECT * FROM ", tblStr, " ", whereStr, " FOR UPDATE");
                        this.ExecuteScalar(strSQLUpdLock);
                    }
                    else if (DatabaseType == DatabaseType.SQLServer)
                    {
                        // SQLServer：WITH (UPDLOCK)
                        string strSQLUpdLock = string.Concat("SELECT * FROM ", tblStr, " WITH (UPDLOCK) ", whereStr);
                        this.ExecuteScalar(strSQLUpdLock);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 排他チェックコマンドを取得します。
        /// <para>行ロック不要な場合又はSELECT、INSERTの場合、元のコマンドを返す。</para>
        /// <para>DELETE、UPDATEの場合、排他チェックコマンドを返す。</para>
        /// </summary>
        /// <param name="doExclusiveCheck">排他チェックを行うか</param>
        /// <param name="command">SQLコマンド</param>
        /// <returns>行ロック不要な場合又はSELECT、INSERTの場合、元のコマンドを返す。<br/>DELETE、UPDATEの場合、排他チェックコマンドを返す。</returns>
        [Obsolete]
        protected string GetExclusiveCheckCommand(bool doExclusiveCheck, string command)
        {
            // 排他チェック不要な場合
            if (!doExclusiveCheck)
                return command;

            string strSQL = command.Trim();
            // SELECT、INSERTの場合
            if (strSQL.StartsWith("SELECT", StringComparison.CurrentCultureIgnoreCase) ||
                strSQL.StartsWith("INSERT", StringComparison.CurrentCultureIgnoreCase))
                return command;

            string strSQExclusive = string.Format("{0} AND {1}='{2}'", strSQL, ExclusiveColumn, SessionHelper.Get(LastUpdatedDatetime));
            return strSQExclusive;
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public abstract void Dispose();

        #endregion
    }
}
