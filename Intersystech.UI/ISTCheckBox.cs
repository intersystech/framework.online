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
    /// ISTCheckBoxクラス
    /// </summary>
    public class ISTCheckBox : CheckBox
    {
        /// <summary>
        /// Intersystech.UI.ISTCheckBox に関連付けられるテキスト ラベルを取得または設定します。
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
        /// Intersystech.UI.ISTCheckBoxのCheckedChanged イベントを発生させます。
        /// </summary>
        /// <param name="e">イベント データを格納している System.EventArgs。</param>
        protected override void OnCheckedChanged(EventArgs e)
        {
            try
            {
                base.OnCheckedChanged(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }

        /// <summary>
        /// 文字列の値をもとに、チェックオンとチェックオフに切り替えます。
        /// "1":チェックオン　"0":チェックオフ
        /// </summary>
        public string CheckedValue
        {
            get
            {
                return this.Checked ? "1" : "0";
            }
            set
            {
                if (value == "1")
                {
                    this.Checked = true;
                }
                else if (value == "0")
                {
                    this.Checked = false;
                }
            }
        }
    }
}
