using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersystech.ConnectionFactory
{
    /// <summary>
    /// SQLコマンド種類
    /// </summary>
    public enum SqlCommandType
    {
        /// <summary>
        /// 検索
        /// </summary>
        Select = 1,
        /// <summary>
        /// 追加
        /// </summary>
        Insert = 2,
        /// <summary>
        /// 更新
        /// </summary>
        Update = 3,
        /// <summary>
        /// 削除
        /// </summary>
        Delete = 4
    }
}
