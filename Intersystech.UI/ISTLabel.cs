using System.Web;
using System.Web.UI.WebControls;
using Intersystech.Extension;
using System.Web.UI;

[assembly: TagPrefix("Intersystech.UI", "ist")]
namespace Intersystech.UI
{
    /// <summary>
    /// ISTLabelクラス
    /// </summary>
    public class ISTLabel : Label
    {
        /// <summary>
        /// ラベルコントロールと同じ仕様であるかを示す値を取得または設定します。
        /// <para>true の場合、コントロールのテキストの内容をエンコードまたはデコードしません。</para>
        /// </summary>
        public bool IsAspLabel
        {
            get;
            set;
        }

        /// <summary>
        /// Intersystech.UI.ISTLabel コントロールのテキストの内容を取得または設定します。
        /// </summary>
        public new string Text
        {
            get
            {
                if (IsAspLabel)
                {
                    return base.Text.ToSafeValue();
                }
                return HttpUtility.HtmlDecode(base.Text).ToSafeValue();
            }
            set
            {
                if (IsAspLabel)
                {
                    base.Text = value;
                }
                else
                {
                    base.Text = HttpUtility.HtmlEncode(value);
                }
            }
        }
    }
}
