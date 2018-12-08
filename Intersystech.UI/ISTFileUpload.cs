using System.Web;
using System.Web.UI.WebControls;
using Intersystech.Extension;
using System.Web.UI;

[assembly: TagPrefix("Intersystech.UI", "ist")]
namespace Intersystech.UI
{
    /// <summary>
    /// ISTFileUploadクラス
    /// </summary>
    public class ISTFileUpload : FileUpload
    {
        /// <summary>
        /// Intersystech.UI.ISTFileUpload コントロールを使用して、アップロードする、クライアント上のファイルの名前を取得します。
        /// </summary>
        public new string FileName
        {
            get
            {
                return HttpUtility.HtmlEncode(base.FileName);
            }
        }
    }
}
