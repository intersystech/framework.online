using System;

namespace Intersystech.ExceptionLibrary
{
    /// <summary>
    /// AlertCustomSystemExceptionクラス
    /// </summary>
    public class AlertCustomSystemException : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">Exception内容</param>
        public AlertCustomSystemException(string message)
            : base(message)
        {
        }
    }
}
