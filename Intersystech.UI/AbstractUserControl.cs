using System;
using System.Text;
using System.Web.UI;
using Intersystech.Utility;
using Intersystech.ExceptionLibrary;

namespace Intersystech.UI
{
    /// <summary>
    /// AbstractUserControlクラス
    /// <remarks>System.Web.UI.UserControlを継承し拡張機能を持つクラスです。</remarks>
    /// </summary>
    /// <typeparam name="T">AbstractExecuteHandler</typeparam>
    public abstract class AbstractUserControl<T> : UserControl where T : AbstractExecuteHandler
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

        #region イベント

        /// <summary>
        /// System.Web.UI.Control.Load イベントを発生させます。
        /// </summary>
        /// <param name="e">イベント データを格納している System.EventArgs。</param>
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);

                if (!IsPostBack)
                {
                    LoadIsNotPostBack(this, e);
                }
                else
                {
                    LoadIsPostBack(this, e);
                }
            }
            catch (AlertCustomSystemException acsex)
            {
                this.ShowAlert(acsex.Message);
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// PostBack以外の時に、実行されます。
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">イベント データを格納している System.EventArgs。</param>
        protected virtual void LoadIsNotPostBack(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// PostBackの時に、実行されます。
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">イベント データを格納している System.EventArgs。</param>
        protected virtual void LoadIsPostBack(object sender, EventArgs e)
        {
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
        /// <param name="showFunction">ローディング画面を表示するjavascript関数名(括弧付き)</param>
        /// <param name="milliseconds">ローディング画面が表示されるまでのタイマー時間(単位:ミリ秒)</param>
        protected virtual void ShowAlertTo<TPage>(string message, string showFunction = "", int milliseconds = 0) where TPage : Page
        {
            AlertHelper.AlertTo<TPage>(message, showFunction, milliseconds);
        }

        #endregion
    }
}