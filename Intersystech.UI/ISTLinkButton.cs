using Intersystech.ExceptionLibrary;
using Intersystech.Extension;
using Intersystech.Utility;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: TagPrefix("Intersystech.UI", "ist")]
namespace Intersystech.UI
{
    /// <summary>
    /// ISTLinkButtonクラス
    /// </summary>
    public class ISTLinkButton : LinkButton
    {
        /// <summary>
        /// Intersystech.UI.ISTLinkButton コントロールに表示されるテキスト キャプションを取得または設定します。
        /// </summary>
        public new string Text
        {
            get
            {
                return HttpUtility.HtmlDecode(base.Text).ToSafeValue();
            }
            set
            {
                base.Text = HttpUtility.HtmlEncode(value);
            }
        }

        /// <summary>
        /// IAlertHelperオブジェクト
        /// </summary>
        private IAlertHelper AlertHelper
        {
            get { return Singleton<AlertHelper>.Instance; }
        }

        /// <summary>
        /// Intersystech.UI.ISTLinkButtonのClick イベントを発生させます。
        /// </summary>
        /// <param name="e">イベント データを格納している System.EventArgs。</param>
        protected override void OnClick(EventArgs e)
        {
            try
            {
                base.OnClick(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
    }
}
