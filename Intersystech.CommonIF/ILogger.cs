namespace Intersystech.CommonIF
{
    /// <summary>
    /// ログ記録インターフェース
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 指定文字列をログファイルに記録します
        /// </summary>
        /// <param name="message">ログを説明するメッセージ</param>
        void Write(string message);

        /// <summary>
        /// 指定文字列をログファイルに記録します
        /// </summary>
        /// <param name="log">ログファイルオブジェクト</param>
        /// <param name="logType">ログタイプ</param>
        void Write(Log log, LogType logType);

        /// <summary>
        /// 指定文字列をログファイルに記録します
        /// </summary>
        /// <param name="message">ログを説明するメッセージ</param>
        /// <param name="stackTrace">呼び出し履歴の直前のフレームを説明する文字列</param>
        /// <param name="logType">ログタイプ</param>
        void Write(string message, string stackTrace, LogType logType);

        /// <summary>
        /// 指定文字列をログファイルに記録します
        /// </summary>
        /// <param name="message">ログを説明するメッセージ</param>
        /// <param name="logType">ログタイプ</param>
        void Write(string message, LogType logType);

        /// <summary>
        /// 最終更新日が、現在の日付から指定された削除基準日数を減算した日付より小さいまたは等しい
        /// トレースログファイルを削除します。
        /// <para>最終更新日 &#8804; 現在の日付 - 削除基準日数 のトレースログファイルを削除します</para>
        /// </summary>
        void AutoClear();

        ///// <summary>
        ///// 指定日より前の日付に作成されたトレースログファイルを削除します
        ///// </summary>
        ///// <param name="date">削除基準日付(yyyy/MM/dd形式)</param>
        //void Clear(string date);
    }
}
