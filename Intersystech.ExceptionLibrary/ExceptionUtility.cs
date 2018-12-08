namespace Intersystech.ExceptionLibrary
{
    /// <summary>
    /// ThrowUtilityクラス
    /// </summary>
    public sealed class ExceptionUtility
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private ExceptionUtility()
        {
        }

        /// <summary>
        /// 実行中の処理を中止し、Alertメッセージを画面側に表示するとともに、トランザクションをロールバックします。
        /// ただし、第三引数をFalse指定する場合、トランザクションをコミットします。
        /// </summary>
        /// <param name="message">メッセージ内容</param>
        /// <param name="rollBackTransaction">実行中のトランザクションをロールバックするか</param>
        public static void Alert(string message, bool rollBackTransaction = true)
        {
            Alert(true, message, rollBackTransaction);
        }

        /// <summary>
        /// 指定条件に満たされる場合のみ、実行中の処理を中止し、Alertメッセージを画面側に表示するとともに、トランザクションをロールバックします。
        /// ただし、第三引数をFalse指定する場合、トランザクションをコミットします。
        /// </summary>
        /// <param name="condition">指定条件</param>
        /// <param name="message">メッセージ内容</param>
        /// <param name="rollBackTransaction">実行中のトランザクションをロールバックするか</param>
        public static void Alert(bool condition, string message, bool rollBackTransaction = true)
        {
            if (condition == true)
            {
                AlertCustomSystemException acsex = new AlertCustomSystemException(message);
                acsex.Data["RollBackTransaction"] = rollBackTransaction;
                throw acsex;
            }
        }

        /// <summary>
        /// 指定したエラーメッセージを利用して、CustomSystemExceptionを発生させます。
        /// トランザクション処理の場合、ロールバックし共通エラー画面へ遷移します。
        /// </summary>
        /// <param name="message">Exception内容</param>
        public static void ThrowCustomSystemException(string message)
        {
            ThrowCustomSystemException(true, message);
        }

        /// <summary>
        /// 指定条件に満たされる場合のみ、指定したエラーメッセージを利用して、CustomSystemExceptionを発生させます。
        /// トランザクション処理の場合、ロールバックし共通エラー画面へ遷移します。
        /// </summary>
        /// <param name="condition">指定条件</param>
        /// <param name="message">Exception内容</param>
        public static void ThrowCustomSystemException(bool condition, string message)
        {
            if (condition)
            {
                throw new CustomSystemException(message);
            }
        }

        /// <summary>
        /// 指定したエラーメッセージを利用して、CustomSystemExceptionを発生させます。
        /// トランザクション処理の場合、ロールバックし共通エラー画面へ遷移します。
        /// </summary>
        /// <param name="message">Exception内容</param>
        public static void ThrowIntersystechSystemException(string message)
        {
            ThrowIntersystechSystemException(true, message);
        }

        /// <summary>
        /// 指定条件に満たされる場合のみ、指定したエラーメッセージを利用して、CustomSystemExceptionを発生させます。
        /// トランザクション処理の場合、ロールバックし共通エラー画面へ遷移します。
        /// </summary>
        /// <param name="condition">指定条件</param>
        /// <param name="message">Exception内容</param>
        public static void ThrowIntersystechSystemException(bool condition, string message)
        {
            if (condition)
            {
                throw new IntersystechSystemException(message);
            }
        }
    }
}
