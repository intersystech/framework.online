using Intersystech.CommonIF;
using Intersystech.DataAccess;
using Intersystech.Utility;
using System;

namespace Intersystech.BusinessFacade
{
    /// <summary>
    /// BusinessFacadeRepositoryクラス
    /// </summary>
    public abstract class BusinessFacadeRepository
    {
        ///// <summary>
        ///// ログ記録インターフェース
        ///// </summary>
        //protected ILogger TraceLogger = Singleton<Logger>.Instance;
        ///// <summary>
        ///// トレースログ
        ///// </summary>
        //protected Log TraceLog = Singleton<Log>.Instance;

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
        /// DataAccessRepositoryを継承するクラスのインスタンスを取得します。
        /// </summary>
        /// <typeparam name="T">DataAccessRepository型</typeparam>
        /// <returns>T型のインスタンス</returns>
        protected T GetDataAccess<T>() where T : DataAccessRepository
        {
            T t = Singleton<T>.Instance;
            return t;
        }
    }
}
