using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Intersystech.Utility
{
    /// <summary>
    /// Alertメッセージを表示する機能を提供するインターフェスクラスです。
    /// </summary>
    public interface IAlertHelper
    {
        /// <summary>
        /// Alertメッセージを表示します。
        /// </summary>
        /// <param name="message">メッセージ内容</param>
        void Alert(string message);

        /// <summary>
        /// Alertメッセージを表示した後、指定ページへ遷移します。
        /// </summary>
        /// <typeparam name="TPage">指定ページ</typeparam>
        /// <param name="message">メッセージ内容</param>
        /// <param name="showFunction">ローディング画面を表示するjavascript関数名(括弧付き)</param>
        /// <param name="milliseconds">ローディング画面が表示されるまでのタイマー時間(単位:ミリ秒)</param>
        void AlertTo<TPage>(string message, string showFunction = "", int milliseconds = 0) where TPage : Page;
    }
}
