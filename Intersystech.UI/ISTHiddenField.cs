using System.Web.UI.WebControls;
using Intersystech.Extension;
using System.Web.UI;

[assembly: TagPrefix("Intersystech.UI", "ist")]
namespace Intersystech.UI
{
    /// <summary>
    /// ISTHiddenFieldクラス
    /// </summary>
    [ValidationProperty("Value")]
    public class ISTHiddenField : HiddenField
    {
        /// <summary>
        /// Intersystech.UI.ISTHiddenField コントロールの値を取得または設定します。
        /// </summary>
        public new string Value
        {
            get
            {
                return base.Value.ToSafeValue();
            }
            set
            {
                base.Value = value;
            }
        }
    }
}
