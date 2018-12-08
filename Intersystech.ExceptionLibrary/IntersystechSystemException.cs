using System;

namespace Intersystech.ExceptionLibrary
{
    /// <summary>
    /// IntersystechSystemException
    /// </summary>
    public class IntersystechSystemException : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">Exception内容</param>
        public IntersystechSystemException(string message)
            : base(message)
        {
        }
    }
}
