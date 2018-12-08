using System;
using System.Web;
using System.Web.UI;
using Intersystech.Utility;
using Intersystech.ExceptionLibrary;

namespace Intersystech.UI
{
    /// <summary>
    /// AbstractMasterPageクラス
    /// <remarks>UI側向け様々な機能を提供します。</remarks>
    /// </summary>
    /// <typeparam name="T">AbstractExecuteHandler</typeparam>
    public abstract class AbstractMasterPage<T> : System.Web.UI.MasterPage where T : AbstractExecuteHandler
    {
        #region プロパティ

        /// <summary>
        /// ページハンドラー
        /// </summary>
        protected IPageHandler PageHandler
        {
            get { return Singleton<PageHandler>.Instance; }
        }

        /// <summary>
        /// イベントサービスハンドラ
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

        #endregion

        #region メソッド

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

                base.OnLoad(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                this.ShowAlert(acsex.Message);
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

        #endregion
    }
}