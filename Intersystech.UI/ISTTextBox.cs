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
    /// ISTTextBoxクラス
    /// </summary>
    public class ISTTextBox : TextBox
    {
        /// <summary>
        /// サーバー コントロールが System.Web.UI.Page オブジェクトに読み込まれると発生します。
        /// </summary>
        /// <param name="e">イベント データを格納している System.EventArgs。</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        /// <summary>
        /// IAlertHelperオブジェクト
        /// </summary>
        private IAlertHelper AlertHelper
        {
            get { return Singleton<AlertHelper>.Instance; }
        }

        /// <summary>
        /// Intersystech.UI.ISTTextBox コントロールのテキストの内容の前後にある半角空白または改行コードを取除くかどうかを示す値を取得または設定します。
        /// </summary>
        public bool IsTrimNecessary
        {
            get;
            set;
        }

        /// <summary>
        /// Intersystech.UI.ISTTextBox コントロールのテキストの内容を取得または設定します。
        /// </summary>
        public new string Text
        {
            get
            {
                if (base.TextMode == TextBoxMode.Password)
                {
                    return base.Text.ToSafeValue();
                }
                return IsTrimNecessary == true ?
                    base.Text.Trim().ToSafeValue() : base.Text.ToSafeValue();
            }
            set
            {
                base.Text = value;
            }
        }

        /// <summary>
        /// Intersystech.UI.ISTTextBox コントロールのテキストの入力ヒントを取得または設定します。
        /// </summary>
        public string PlaceHolder
        {
            get
            {
                return this.Attributes["placeholder"];
            }
            set
            {
                this.Attributes.Add("placeholder", value);
            }
        }

        /// <summary>
        /// Intersystech.UI.ISTTextBoxのTextChanged イベントを発生させます。
        /// </summary>
        /// <param name="e">イベント データを格納している System.EventArgs。</param>
        protected override void OnTextChanged(EventArgs e)
        {
            try
            {
                base.OnTextChanged(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
    }
}
