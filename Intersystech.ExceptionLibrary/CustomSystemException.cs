using System;

namespace Intersystech.ExceptionLibrary
{
    /// <summary>
    /// CustomSystemExceptionクラス
    /// </summary>
    public class CustomSystemException : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">Exception内容</param>
        public CustomSystemException(string message)
            : base(message)
        {
        }
    }
}
