using System;

namespace Intersystech.CommonIF
{
    /// <summary>
    /// トレースログ
    /// </summary>
    [Serializable]
    public class Log
    {
        /// <summary>
        /// ログ内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// スタックトレース
        /// </summary>
        public string StackTrace { get; set; }

    }
}
