using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Collections.Specialized;

namespace Intersystech.UI
{
    /// <summary>
    /// IPageHandlerインターフェース
    /// </summary>
    public interface IPageHandler
    {
        /// <summary>
        /// ページロード時刻
        /// </summary>
        [Obsolete]
        DateTime PageLoadTime
        {
            get;
            set;
        }

        /// <summary>
        /// 最終更新日時
        /// </summary>
        [Obsolete]
        string LastUpdatedDatetime
        {
            get; set;
        }

        /// <summary>
        /// ホストIPアドレス
        /// </summary>
        String HostIPAddress
        {
            get;
        }

        /// <summary>
        /// 現在の要求の生のURL
        /// </summary>
        String RawUrl
        {
            get;
        }

        /// <summary>
        /// 拡張子を含むページの物理名
        /// </summary>
        String PageFullName
        {
            get;
        }

        /// <summary>
        /// ブラウザー名(小文字)
        /// </summary>
        [Obsolete]
        String BrowserName
        {
            get;
        }

        /// <summary>
        /// ブラウザーのメジャー (整数) バージョン番号
        /// </summary>
        int BrowserMajorVersion
        {
            get;
        }

        /// <summary>
        /// ブラウザーの完全なバージョン番号 (整数と小数) 
        /// </summary>
        String BrowserFullVersion
        {
            get;
        }

        /// <summary>
        /// クエリ文字列変数のコレクションを取得します。
        /// </summary>
        NameValueCollection QueryString
        {
            get;
        }

        /// <summary>
        /// Microsoftのネットブラウザーであるか
        /// </summary>
        /// <returns>Internet ExplorerまたはEdgeの場合、true。それ以外の場合、false。</returns>
        bool IsMicrosoftNetBrowser();

        /// <summary>
        /// IEのバージョンが8以上であるかどうかを示す値を取得します。
        /// </summary>
        /// <returns>false:IE8以前のバージョン,true:IE8以降のバージョン</returns>
        [Obsolete]
        Boolean IsIEGreaterThan8();

        /// <summary>
        /// ドキュメントモードを解除します。
        /// </summary>
        void RemoveDocumentMode();

        /// <summary>
        /// ドキュメントモードを指定します。
        /// </summary>
        /// <param name="majorVersion">IEのメジャーバージョン</param>
        void AddDocumentMode(int majorVersion);

        /// <summary>
        /// Javascriptが有効になっているかをチェックします。
        /// </summary>
        /// <param name="message">Javascriptが無効の場合のメッセージ</param>
        void CheckJavascriptValid(string message);

        /// <summary>
        /// 指定ページへ遷移します(Response.Redirectと同等の機能を提供します)。
        /// </summary>
        /// <typeparam name="TPage">指定ページ</typeparam>
        /// <param name="endResponse">現在のページの実行を終了するかどうかを示します</param>
        void RedirectTo<TPage>(bool endResponse = true) where TPage : Page;

        /// <summary>
        /// 指定ページへ遷移します(Response.Redirectと同等の機能を提供します)。
        /// </summary>
        /// <param name="url">指定URL</param>
        /// <param name="endResponse">現在のページの実行を終了するかどうかを示します</param>
        void RedirectTo(string url, bool endResponse = true);

        /// <summary>
        /// 指定ページへ遷移します(Server.Transferと同等の機能を提供します)。
        /// </summary>
        /// <typeparam name="TPage">指定ページ</typeparam>
        /// <param name="endResponse">現在のページの実行を終了するかどうかを示します</param>
        void TransferTo<TPage>(bool endResponse = true) where TPage : Page;

        /// <summary>
        /// 指定ページへ遷移します(Server.Transferと同等の機能を提供します)。
        /// </summary>
        /// <param name="url">指定URL</param>
        /// <param name="endResponse">現在のページの実行を終了するかどうかを示します</param>
        void TransferTo(string url, bool endResponse = true);
    }
}
