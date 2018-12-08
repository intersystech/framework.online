using System;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using Intersystech.CommonIF;
using Intersystech.ExceptionLibrary;
using Intersystech.Utility;

namespace Intersystech.UI
{
    /// <summary>
    /// AbstractPageクラス
    /// <remarks>System.Web.UI.Pageを継承し拡張機能を持つクラスです。</remarks>
    /// </summary>
    /// <typeparam name="T">AbstractExecuteHandler</typeparam>
    public abstract class AbstractPage<T> : Page where T : AbstractExecuteHandler
    {
        #region 変数
        /// <summary>
        /// ログ連結文字列
        /// </summary>
        private string LogChar = new string(':', 50);

        #endregion

        #region プロパティ

        /// <summary>
        /// ページハンドラー
        /// </summary>
        protected IPageHandler PageHandler
        {
            get { return Singleton<PageHandler>.Instance; }
        }
        /// <summary>
        /// ビジネスファサードを呼び出すためのハンドラー
        /// </summary>
        protected T ExecuteHandler
        {
            get { return Singleton<T>.Instance; }
        }

        /// <summary>
        /// IAlertHelperオブジェクト
        /// </summary>
        private IAlertHelper AlertHelper
        {
            get { return Singleton<AlertHelper>.Instance; }
        }

        /// <summary>
        /// ページ内のデータが変更されたかを示す値を取得します。
        /// <remarks>(True:変更あり、False:変更なし)</remarks>
        /// </summary>
        protected bool IsDataDirty
        {
            get
            {
                return Request.Form["IsDataDirty"] == "1";
            }
        }

        #endregion

        #region イベント

        /// <summary>
        /// ページの初期化の開始時に System.Web.UI.Page.PreInit イベントを発生させます。
        /// </summary>
        /// <param name="e">イベント データを格納している System.EventArgs。</param>
        protected override void OnPreInit(EventArgs e)
        {
            try
            {
                base.OnPreInit(e);
                if (IsPostBack == false)
                {
                    //if (PageHandler.IsIEGreaterThan8())
                    //{
                    // ページ読み込み開始
                    //string message = string.Concat("ページ読込開始", LogChar, PageHandler.PageFullName);
                    //ExecuteHandler.TraceLogger.Information(message);
                    //}
                    // URLクエリ文字列を取得し復号化する
                    SetQueryString(HttpContext.Current.Request);
                }
            }
            catch (AlertCustomSystemException acsex)
            {
                this.ShowAlert(acsex.Message);
            }
        }

        /// <summary>
        /// System.Web.UI.Control.Load イベントを発生させます。
        /// </summary>
        /// <param name="e">イベント データを格納している System.EventArgs。</param>
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                var page = HttpContext.Current.Handler as Page;
                page.Header.DataBind();

                ChangeDocumentMode();

                base.OnLoad(e);

                if (!IsPostBack)
                {
                    // 変更チェックScript登録
                    RegisterCheckDataDirty();

                    // URLクエリ文字列をクリア
                    PageHandler.QueryString.Clear();
                }
            }
            catch (AlertCustomSystemException acsex)
            {
                this.ShowAlert(acsex.Message);
            }
        }

        /// <summary>
        /// URLクエリ文字列を設定します。
        /// </summary>
        /// <param name="request">Web要求</param>
        private void SetQueryString(HttpRequest request)
        {
            PageHandler.QueryString.Clear();

            foreach (string key in request.QueryString.AllKeys)
            {
                string value = request.QueryString[key];
                try
                {
                    value = SecurityHelper.DecryptQueryString(value);
                }
                catch (Exception)
                {
                }
                finally
                {
                    PageHandler.QueryString.Add(key, value);
                }
            }
        }

        /// <summary>
        /// ページの読み込み段階の終了時に System.Web.UI.Page.LoadComplete イベントを発生させます。
        /// </summary>
        /// <param name="e">イベント データを格納している System.EventArgs。</param>
        protected override void OnLoadComplete(EventArgs e)
        {
            try
            {
                base.OnLoadComplete(e);

                if (IsPostBack)
                {
                    // 確認ダイアログ結果がOKの場合
                    if (Request.Form["DialogResult"] == "1")
                    {
                        // 変更フラグをクリア
                        SetDataClean();

                        // 確認ダイアログフラグをクリア
                        Request.Form["DialogResult"].Replace("1", "0");
                        DoConfirmOK(Request.Form["CommandParameter"]);

                        // 変更チェックScript登録
                        RegisterCheckDataDirty();
                    }
                    else
                    {
                        // 変更チェック
                        if (IsDataDirty == true)
                        {
                            SetDataDirty();
                        }
                        else
                        {
                            // 変更フラグをクリア
                            SetDataClean();
                        }
                    }
                }
                else
                {
                    // 変更フラグをクリア
                    SetDataClean();

                    //// ページ読み込み完了
                    //string message = string.Concat("ページ読込完了", LogChar, this.PageHandler.PageFullName);
                    //ExecuteHandler.TraceLogger.Information(message);
                }

                // ページロード日時(排他チェック用)
                //SessionHelper.Set("PageLoadTime", PageHandler.PageLoadTime);
            }
            catch (AlertCustomSystemException acsex)
            {
                this.ShowAlert(acsex.Message);
            }
        }

        /// <summary>
        /// 処理されていないページ例外が発生した場合に、呼び出されます。
        /// </summary>
        /// <param name="e">イベント データを格納している System.EventArgs。</param>
        protected override void OnError(EventArgs e)
        {
            Exception error = Server.GetLastError();
            while (error.InnerException != null)
            {
                error = error.InnerException;
            }
            string message = error.Message;
            string stackTrace = error.StackTrace;

            //ExecuteHandler.TraceLogger.Error(message);
            //ExecuteHandler.TraceLogger.Error(stackTrace);

            // セッションクリア
            SessionHelper.Clear();
            // セッションにエラー情報を格納
            SessionHelper.SetError(error);

            // 前回の例外を削除
            Server.ClearError();

            // IISのカスタムエラー画面を無視
            Response.TrySkipIisCustomErrors = true;

            // 独自カスタムエラー画面へ遷移
            CustomErrorsSection customErrorsSection = WebConfigurationManager.GetSection("system.web/customErrors") as CustomErrorsSection;
            string defaultRedirect = customErrorsSection.DefaultRedirect;
            Server.Transfer(defaultRedirect, true);
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 変更チェックScript登録
        /// </summary>
        private void RegisterCheckDataDirty()
        {
            StringBuilder isDataDirtySb = new StringBuilder();
            // 自動呼出(※ScriptManagerありが前提)
            isDataDirtySb.AppendLine(@"function pageLoad()");
            isDataDirtySb.AppendLine(@"{");
            isDataDirtySb.AppendLine(@"    try");
            isDataDirtySb.AppendLine(@"    {");
            isDataDirtySb.AppendLine(@"        bindCheckDataDirtyEvent();");
            isDataDirtySb.AppendLine(@"    }");
            isDataDirtySb.AppendLine(@"    catch (e)");
            isDataDirtySb.AppendLine(@"    {");
            isDataDirtySb.AppendLine(@"    }");
            isDataDirtySb.AppendLine(@"}");
            // チェックデータ変更イベント
            isDataDirtySb.AppendLine(@"function bindCheckDataDirtyEvent() {");
            // テキスト、チェックボックス、ラジオボックス
            isDataDirtySb.AppendLine(@"    var inputObj = $('input');");
            isDataDirtySb.AppendLine(@"    for (var i = 0; i < inputObj.length; i++) {");
            isDataDirtySb.AppendLine(@"        var obj = inputObj[i];");
            isDataDirtySb.AppendLine(@"        if ($(obj).closest('.checkDataDirtyNone').length == 0) {");
            isDataDirtySb.AppendLine(@"            var elementType = obj.type;");
            isDataDirtySb.AppendLine(@"            var isReadonly = obj.readOnly;");
            isDataDirtySb.AppendLine(@"            var isVisible = obj.style.display != 'none';");
            isDataDirtySb.AppendLine(@"            if (elementType != 'submit' &&");
            isDataDirtySb.AppendLine(@"                elementType != 'button' &&");
            isDataDirtySb.AppendLine(@"                elementType != 'hidden' &&");
            isDataDirtySb.AppendLine(@"                isReadonly == false &&");
            isDataDirtySb.AppendLine(@"                isVisible == true) {");
            isDataDirtySb.AppendLine(@"                obj.attachEvent('onpropertychange', OnPropChanged);");
            isDataDirtySb.AppendLine(@"            }");
            isDataDirtySb.AppendLine(@"        }");
            isDataDirtySb.AppendLine(@"    }");
            // テキストエリア
            isDataDirtySb.AppendLine(@"    var textAreaObj = $('textarea');");
            isDataDirtySb.AppendLine(@"    for (var i = 0; i < textAreaObj.length; i++) {");
            isDataDirtySb.AppendLine(@"        var obj = textAreaObj[i];");
            isDataDirtySb.AppendLine(@"        if ($(obj).closest('.checkDataDirtyNone').length == 0) {");
            isDataDirtySb.AppendLine(@"            obj.attachEvent('onpropertychange', OnPropChanged);");
            isDataDirtySb.AppendLine(@"        }");
            isDataDirtySb.AppendLine(@"    }");
            // コンボボックス
            isDataDirtySb.AppendLine(@"    var selectObj = $('select');");
            isDataDirtySb.AppendLine(@"    for (var i = 0; i < selectObj.length; i++) {");
            isDataDirtySb.AppendLine(@"        var obj = selectObj[i];");
            isDataDirtySb.AppendLine(@"        if ($(obj).closest('.checkDataDirtyNone').length == 0) {");
            isDataDirtySb.AppendLine(@"            obj.attachEvent('onpropertychange', OnPropChanged);");
            isDataDirtySb.AppendLine(@"        }");
            isDataDirtySb.AppendLine(@"    }");
            isDataDirtySb.AppendLine(@"}");
            // データ変更フラグを'1'にする
            isDataDirtySb.AppendLine(@"function OnPropChanged(event) {");
            isDataDirtySb.AppendLine(@"    var propNm = event.propertyName.toLowerCase();");
            isDataDirtySb.AppendLine(@"    if (propNm == 'value' || propNm == 'checked') {");
            isDataDirtySb.AppendLine(@"        $('[id$=IsDataDirty]').val('1');");
            isDataDirtySb.AppendLine(@"    }");
            isDataDirtySb.AppendLine(@"}");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "checkDataDirty", isDataDirtySb.ToString(), true);
        }

        /// <summary>
        /// ConfirmダイアログのOKボタンが押下された時に、実行されます。
        /// </summary>
        /// <param name="commandParameter">呼出元を区別するためのパラメータ</param>
        protected virtual void DoConfirmOK(string commandParameter)
        {
        }

        /// <summary>
        /// ドキュメントモード変更
        /// </summary>
        private void ChangeDocumentMode()
        {
            HttpBrowserCapabilities browser = Request.Browser;
            string name = browser.Browser.ToLower();
            int majorVersion = browser.MajorVersion;

            if (name == "ie" && majorVersion == 9)
            {
                // IE9の場合、テーブルセルがずれる可能性があるため(http://support.microsoft.com/kb/2904940/ja)、
                // ドキュメントモードをIE8に変換する
                PageHandler.RemoveDocumentMode();
                PageHandler.AddDocumentMode(majorVersion - 1);
            }
        }

        /// <summary>
        /// Alertメッセージを表示します。
        /// </summary>
        /// <param name="message">メッセージ内容</param>
        protected virtual void ShowAlert(string message)
        {
            AlertHelper.Alert(message);
        }

        /// <summary>
        /// Alertメッセージを表示した後、指定ページへ遷移します。
        /// </summary>
        /// <typeparam name="TPage">指定ページ</typeparam>
        /// <param name="message">メッセージ内容</param>
        protected virtual void ShowAlertTo<TPage>(string message) where TPage : Page
        {
            AlertHelper.AlertTo<TPage>(message, showFunction, milliseconds);
        }

        private string showFunction;
        private string hideFunction;
        private int milliseconds;
        /// <summary>
        /// ローディング画面を制御するjavascript関数を設定します。
        /// </summary>
        /// <param name="showFunction">ローディング画面を表示するjavascript関数名(括弧付き)</param>
        /// <param name="hideFunction">ローディング画面を非表示にするjavascript関数名(括弧付き)</param>
        /// <param name="milliseconds">ローディング画面が表示されるまでのタイマー時間(単位:ミリ秒)</param>
        protected void RegisterLoadingFunction(string showFunction, string hideFunction, int milliseconds = 0)
        {
            ExceptionUtility.ThrowCustomSystemException(milliseconds < 0, "第三引数は0以上の値を指定してください。");
            this.showFunction = showFunction;
            this.hideFunction = hideFunction;
            this.milliseconds = milliseconds;
        }

        /// <summary>
        /// Confirmダイアログを表示します。
        /// </summary>
        /// <param name="message">メッセージ内容</param>
        /// <param name="commandParameter">呼出元を区別するためのパラメータ</param>
        protected virtual void ShowConfirm(string message, string commandParameter = "")
        {
            if (string.IsNullOrEmpty(message))
                return;

            ScriptManager.RegisterHiddenField(this, "DialogResult", "0");
            ScriptManager.RegisterHiddenField(this, "CommandParameter", commandParameter);

            string sMsg = message.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\"", "''");

            StringBuilder sb = new StringBuilder();

            if (this.showFunction == null || this.showFunction.Trim() == string.Empty)
            {
                sb.Append(@" if(confirm( """ + sMsg + @""" ))");
                sb.Append(@" { ");
                sb.Append(@"document.forms[0].DialogResult.value='1';");
                sb.Append(@"document.forms[0].submit();");
                sb.Append("}");
                sb.Append(@" else { ");
                sb.Append("document.forms[0].DialogResult.value='0'; }");
            }
            else
            {
                string showLoadingFunction = this.showFunction.TrimEnd(';') + ";";
                string hideLoadingFunction = this.hideFunction.TrimEnd(';') + ";";
                sb.Append(@"setTimeout(function() {");
                sb.Append(@showLoadingFunction);
                sb.Append(@" if(confirm( """ + sMsg + @""" ))");
                sb.Append(@" { ");
                sb.Append(@"document.forms[0].DialogResult.value='1';");
                sb.Append(@"document.forms[0].submit();");
                sb.Append("}");
                sb.Append(@" else { ");
                sb.Append(@hideLoadingFunction);
                sb.Append("document.forms[0].DialogResult.value='0'; }");
                sb.Append("}, " + this.milliseconds + ");");
            }

            string script = sb.ToString();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showConfirm", script, true);
        }

        /// <summary>
        /// データ変更状態を変更ありにします。
        /// <remarks>IsDataDirtyの戻り値はTrueになります。</remarks>
        /// </summary>
        protected virtual void SetDataDirty()
        {
            // 変更フラグ登録
            ScriptManager.RegisterHiddenField(this, "IsDataDirty", "1");
        }

        /// <summary>
        /// データ変更状態を変更なしにします。
        /// <remarks>IsDataDirtyの戻り値はFalseになります。</remarks>
        /// </summary>
        protected virtual void SetDataClean()
        {
            // 変更フラグクリア
            ScriptManager.RegisterHiddenField(this, "IsDataDirty", "0");
        }

        #endregion
    }
}
