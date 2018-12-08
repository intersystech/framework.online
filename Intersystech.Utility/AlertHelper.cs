using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;

namespace Intersystech.Utility
{
    /// <summary>
    /// IAlertHelperを実装するクラスです。
    /// </summary>
    public class AlertHelper : IAlertHelper
    {
        /// <summary>
        /// 現在のページ
        /// </summary>
        private Page CurrentPage
        {
            get
            {
                return HttpContext.Current.Handler as Page;
            }
        }
        /// <summary>
        /// Alertメッセージを表示します。
        /// </summary>
        /// <param name="message">メッセージ内容</param>
        public virtual void Alert(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            string sMsg = message.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\"", "''");
            string script = string.Format(@"alert(""{0}"");", sMsg);

            ScriptManager.RegisterStartupScript(CurrentPage, typeof(Page), SecurityHelper.GetToken(), script, true);
        }

        /// <summary>
        /// Alertメッセージを表示した後、指定ページへ遷移します。
        /// </summary>
        /// <typeparam name="TPage">指定ページ</typeparam>
        /// <param name="message">メッセージ内容</param>
        /// <param name="showFunction">ローディング画面を表示するjavascript関数名(括弧付き)</param>
        /// <param name="milliseconds">ローディング画面が表示されるまでのタイマー時間(単位:ミリ秒)</param>
        public virtual void AlertTo<TPage>(string message, string showFunction = "", int milliseconds = 0) where TPage : Page
        {
            if (string.IsNullOrEmpty(message))
                return;

			string sMsg = message.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\"", "''");

            Control ctrl = Singleton<Control>.Instance;
            string page = ctrl.ResolveUrl(ConfigHelper.GetUrl<TPage>());

            StringBuilder sb = new StringBuilder();
            if (showFunction == null || showFunction.Trim() == string.Empty)
            {
                sb.Append(@"alert( """ + sMsg + @""" );");
                sb.Append(@"document.location.href='" + page + "';");
            }
            else
            {
                string showLoadingFunction = showFunction.TrimEnd(';') + ";";
                sb.Append(@"setTimeout(function() {");
                sb.Append(@showLoadingFunction);
                sb.Append(@"alert( """ + sMsg + @""" );");
                // IE8対応(参考：http://support.microsoft.com/kb/2762271/ja)
                sb.Append(@"var isIE8 = navigator.userAgent.indexOf('Trident/4.0') > -1;");
                sb.Append(@"if (isIE8) {");
                sb.Append(@"var fakeLink = document.createElement('a');");
                sb.Append(@"if (typeof (fakeLink.click) != 'undefined') {");
                sb.Append(@"fakeLink.href = '" + page + "';");
                sb.Append(@"document.body.appendChild(fakeLink);");
                sb.Append(@"fakeLink.click();");
                sb.Append(@"}");
                sb.Append(@"} else {");
                sb.Append(@"document.location.href='" + page + "';");
                sb.Append(@"}");
                sb.Append("}, " + milliseconds + ");");
            }
            string script = sb.ToString();

            ScriptManager.RegisterStartupScript(CurrentPage, typeof(Page), SecurityHelper.GetToken(), script, true);
        }
    }
}
