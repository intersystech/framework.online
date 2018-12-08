using System;

namespace Intersystech.Utility
{
    /// <summary>
    /// Singletonクラス
    /// </summary>
    /// <typeparam name="T">class型</typeparam>
    public sealed class Singleton<T> where T : class
    {
        private static volatile T instance;
        private static object syncRoot = new Object();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private Singleton()
        {
        }

        /// <summary>
        /// インスタンス
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = Activator.CreateInstance<T>();
                        }
                    }
                }
                return instance;
            }
        }
    }
}
