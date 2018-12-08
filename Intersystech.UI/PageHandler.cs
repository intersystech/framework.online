using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Intersystech.CommonIF;
using Intersystech.Extension;
using Intersystech.Utility;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

namespace Intersystech.UI
{
    /// <summary>
    /// PageHandlerクラス
    /// </summary>
    public class PageHandler : IPageHandler
    {
        #region 変数
        ///// <summary>
        ///// ログ記録インターフェース
        ///// </summary>
        //private TraceLogger TraceLogger = new TraceLogger();

        #endregion

        #region プロパティ

        /// <summary>
        /// ページロード時刻
        /// </summary>
        private DateTime pageLoadTime;

        /// <summary>
        /// ページロード時刻
        /// </summary>
        DateTime IPageHandler.PageLoadTime
        {
            get
            {
                if (HttpContext.Current.Request["__EVENTTARGET"] == null)
                {
                    // 初期表示の場合
                    pageLoadTime = DateTime.Now;
                }
                else
                {
                    if (SessionHelper.Get("PageLoadTime") != null)
                    {
                        // PostBackの場合
                        pageLoadTime = Convert.ToDateTime(SessionHelper.Get("PageLoadTime"));
                    }
                    else
                    {
                        // 原因不明でSessionがNULLの場合
                        pageLoadTime = DateTime.Now;
                    }
                }
                return pageLoadTime;
            }
            set
            {
                SessionHelper.Set("PageLoadTime", value);
            }
        }

        /// <summary>
        /// 最終更新日時
        /// </summary>
        string IPageHandler.LastUpdatedDatetime
        {
            get
            {
                return Convert.ToString(SessionHelper.Get("LastUpdatedDatetime"));
            }
            set
            {
                SessionHelper.Set("LastUpdatedDatetime", value);
            }
        }

        /// <summary>
        /// ホストIPアドレス
        /// </summary>
        String IPageHandler.HostIPAddress
        {
            get
            {
                var request = HttpContext.Current.Request;
                string ipString;
                if (string.IsNullOrEmpty(request.ServerVariables["HTTP_X_FORWARDED_FOR"]) == false)
                {
                    ipString = request.ServerVariables["HTTP_X_FORWARDED_FOR"]
                        .Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .FirstOrDefault();
                }
                else
                {
                    ipString = request.ServerVariables["REMOTE_ADDR"];
                    // ローカルホスト名取得
                    if (request.IsLocal == true)
                    {
                        IPAddress[] ipAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                        ipString = ipAddresses.Where(x => x.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault().ToString();
                    }
                }

                return ipString;
            }
        }

        /// <summary>
        /// 現在の要求の生のURL
        /// </summary>
        public string RawUrl
        {
            get
            {
                return HttpContext.Current.Request.RawUrl;
            }
        }

        /// <summary>
        /// 拡張子を含むページの物理名
        /// </summary>
        string IPageHandler.PageFullName
        {
            get { return HttpContext.Current.Request.CurrentExecutionFilePath; }
        }

        /// <summary>
        /// ブラウザー名(小文字)
        /// </summary>
        [Obsolete]
        string IPageHandler.BrowserName
        {
            get
            {
                HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
                return browser.Browser.ToLower();
            }
        }

        /// <summary>
        /// ブラウザーのメジャー (整数) バージョン番号
        /// </summary>
        int IPageHandler.BrowserMajorVersion
        {
            get
            {
                HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
                return browser.MajorVersion;
            }
        }

        /// <summary>
        /// ブラウザーの完全なバージョン番号 (整数と小数) 
        /// </summary>
        string IPageHandler.BrowserFullVersion
        {
            get
            {
                HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
                return browser.Version;
            }
        }

        /// <summary>
        /// クエリ文字列変数のコレクションを取得します。
        /// </summary>
        private NameValueCollection queryString = new NameValueCollection();
        NameValueCollection IPageHandler.QueryString
        {
            get
            {
                return this.queryString;
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PageHandler()
        {

        }

        #endregion

        #region メソッド

        /// <summary>
        /// Microsoftのネットブラウザーであるか
        /// </summary>
        /// <returns>Internet ExplorerまたはEdgeの場合、true。それ以外の場合、false。</returns>
        public bool IsMicrosoftNetBrowser()
        {
            string userAgent = HttpContext.Current.Request.UserAgent;
            if (userAgent.IndexOf("Edge") > -1 || userAgent.IndexOf("Trident") > -1)
                return true;

            return false;
        }

        /// <summary>
        /// Internet Explorerのバージョンが8以上であるかどうかを示す値を取得します。
        /// </summary>
        /// <returns>false:IE8以前のバージョン,true:IE8以降のバージョン</returns>
        [Obsolete]
        public bool IsIEGreaterThan8()
        {
            HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
            string name = browser.Browser.ToLower();
            int majorVersion = browser.MajorVersion;
            string version = browser.Version;

            //string message = string.Format("ブラウザー種類：{0}, Version：{1}", name, version);
            //TraceLogger.Information(message);

            if ((name != "ie" && name != "internetexplorer") || majorVersion < 8)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// ドキュメントモードを解除します。
        /// </summary>
        public void RemoveDocumentMode()
        {
            var page = HttpContext.Current.Handler as Page;
            for (int i = 0, counter = page.Header.Controls.Count; i < counter; i++)
            {
                var control = page.Header.Controls[i];
                if (control.GetType() == typeof(HtmlMeta))
                {
                    HtmlMeta meta = control as HtmlMeta;
                    if (meta.HttpEquiv.Contains("X-UA-Compatible"))
                    {
                        page.Header.Controls.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// ドキュメントモードを指定します。
        /// </summary>
        /// <param name="majorVersion">IEのメジャーバージョン</param>
        public void AddDocumentMode(int majorVersion)
        {
            var page = HttpContext.Current.Handler as Page;
            HtmlMeta htmlMeta = new HtmlMeta();
            htmlMeta.HttpEquiv = "X-UA-Compatible";
            htmlMeta.Content = string.Format("IE={0}", majorVersion);
            page.Header.Controls.AddAt(0, htmlMeta);
        }

        /// <summary>
        /// 指定ページへ遷移します(Response.Redirectと同等の機能を提供します)。
        /// </summary>
        /// <typeparam name="TPage">Webページ</typeparam>
        /// <param name="endResponse">現在のページの実行を終了するかどうかを示します</param>
        public void RedirectTo<TPage>(bool endResponse = true) where TPage : Page
        {
            string url = ConfigHelper.GetUrl<TPage>();
            RedirectTo(url, endResponse);
        }

        /// <summary>
        /// 指定ページへ遷移します(Response.Redirectと同等の機能を提供します)。
        /// </summary>
        /// <param name="url">指定URL</param>
        /// <param name="endResponse">現在のページの実行を終了するかどうかを示します</param>
        public void RedirectTo(string url, bool endResponse = true)
        {
            try
            {
                Control ctrl = Singleton<Control>.Instance;
                url = ctrl.ResolveUrl(url) + GetQueryStringValue();

                HttpContext.Current.Response.Redirect(url, endResponse);
            }
            catch (ThreadAbortException)
            {
                // 何もしない
            }
        }

        /// <summary>
        /// 指定ページへ遷移します(Server.Transferと同等の機能を提供します)。
        /// </summary>
        /// <typeparam name="TPage">Webページ</typeparam>
        /// <param name="endResponse">現在のページの実行を終了するかどうかを示します</param>
        public void TransferTo<TPage>(bool endResponse = true) where TPage : Page
        {
            string url = ConfigHelper.GetMappedUrl<TPage>();
            TransferTo(url, endResponse);
        }

        /// <summary>
        /// 指定ページへ遷移します(Server.Transferと同等の機能を提供します)。
        /// </summary>
        /// <param name="url">指定URL</param>
        /// <param name="endResponse">現在のページの実行を終了するかどうかを示します</param>
        public void TransferTo(string url, bool endResponse = true)
        {
            try
            {
                Control ctrl = Singleton<Control>.Instance;
                url = ctrl.ResolveUrl(url) + GetQueryStringValue();

                HttpContext.Current.Server.Transfer(url, endResponse);
            }
            catch (ThreadAbortException)
            {
                // 何もしない
            }
        }

        /// <summary>
        /// URLクエリ文字列を取得します。
        /// </summary>
        /// <returns>URLクエリ文字列</returns>
        private string GetQueryStringValue()
        {
            string query = string.Empty;
            if (queryString.Count > 0)
            {
                query = "?";
                for (int i = 0, counter = queryString.Count; i < counter; i++)
                {
                    string key = queryString.GetKey(i);
                    string value = SecurityHelper.EncryptQueryString(queryString[i].Split(',').FirstOrDefault());
                    if (counter == 1)
                    {
                        query += string.Join("=", key, value);
                    }
                    else
                    {
                        query += string.Concat(string.Join("=", key, value), "&");
                    }
                }
                query = query.TrimEnd('&');
            }

            this.queryString.Clear();
            return query;
        }

        /// <summary>
        /// Javascriptが有効になっているかをチェックします。
        /// </summary>
        /// <param name="message">Javascriptが無効の場合のメッセージ</param>
        public void CheckJavascriptValid(string message)
        {
            StringBuilder jsInvalidSb = new StringBuilder();
            jsInvalidSb.Append(@"<div id='divJsInvalid' style='width: 100%; height:100%; background-color: #ffffff; position: absolute; top:0; left:0;'>");
            jsInvalidSb.Append(@"<div id='jsInvalid' style='position: relative; margin-left:auto; margin-top:20%;margin-right:auto; width: 600px; text-align: left; border: 5px solid #ffaaaa; padding: 10px;'>");
            jsInvalidSb.Append(@"<p style='text-align:center;font-size: 16px; font-weight: bold;'>");
            string sMsg = message.Replace(@"\r\n", "<br />");
            jsInvalidSb.Append(sMsg);
            jsInvalidSb.Append(@"</p>");
            jsInvalidSb.Append(@"</div>");
            jsInvalidSb.Append(@"</div>");

            jsInvalidSb.Append(@"<script>");
            jsInvalidSb.Append(@"document.getElementById('jsInvalid').style.display = 'none';");
            jsInvalidSb.Append(@"document.getElementById('divJsInvalid').style.display = 'none';");
            jsInvalidSb.Append(@"</script>");

            HtmlGenericControl div = new HtmlGenericControl("div");
            div.InnerHtml = jsInvalidSb.ToString();
            div.ID = "divCheckJavascriptValid";

            var page = HttpContext.Current.CurrentHandler as Page;
            bool isDivExisted = false;
            foreach (Control ctr in page.Form.Controls)
            {
                if (ctr.ID == div.ID)
                {
                    isDivExisted = true;
                    break;
                }
            }
            if (isDivExisted == false)
            {
                page.Form.Controls.Add(div);
            }
        }

        #endregion
    }
}
