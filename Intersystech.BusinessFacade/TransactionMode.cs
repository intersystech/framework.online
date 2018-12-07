using System;

namespace Intersystech.BusinessFacade
{
    /// <summary>
    /// トランザクションタイプ
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// 検索
        /// </summary>
        Query,
        /// <summary>
        /// 更新
        /// </summary>
        Update,
        /// <summary>
        /// 独立
        /// </summary>
        Single
    }

    /// <summary>
    /// トランザクションモードプロパティ
    /// </summary>
    public class TransactionMode : Attribute
    {
        private TransactionType transactionType;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="transactionType">トランザクション種類</param>
        public TransactionMode(TransactionType transactionType)
        {
            this.transactionType = transactionType;
        }

        /// <summary>
        /// トランザクション種類
        /// </summary>
        public TransactionType TransactionType
        {
            get
            {
                return this.transactionType;
            }
        }
    }
}
