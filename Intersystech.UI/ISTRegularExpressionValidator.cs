using Intersystech.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: TagPrefix("Intersystech.UI", "ist")]
namespace Intersystech.UI
{
    /// <summary>
    /// 入力コントロールのユーザ定義検証を実行します。
    /// </summary>
    [ToolboxData(@"<{0}:ISTRegularExpressionValidator runat=""server"" ErrorMessageID=""ErrorMessageID""></{0}:ISTRegularExpressionValidator>")]
    public class ISTRegularExpressionValidator : RegularExpressionValidator
    {
        /// <summary>
        /// System.Web.UI.Control.Init イベントを発生させます。
        /// </summary>
        /// <param name="e">イベント データを格納している System.EventArgs。</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        private string languageId;
        /// <summary>
        /// 言語コード
        /// </summary>
        public string LanguageId
        {
            get
            {
                if (string.IsNullOrEmpty(this.languageId))
                {
                    this.languageId = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                }
                return this.languageId;
            }
            set
            {
                this.languageId = value;
            }
        }

        /// <summary>
        /// エラーメッセージID
        /// </summary>
        public string ErrorMessageID
        {
            get;
            set;
        }
    }
}
