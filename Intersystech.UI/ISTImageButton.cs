using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Intersystech.Utility;
using Intersystech.ExceptionLibrary;

[assembly: TagPrefix("Intersystech.UI", "ist")]
namespace Intersystech.UI
{
    /// <summary>
    /// ISTImageButtonクラス
    /// </summary>
    public class ISTImageButton : ImageButton
    {
        /// <summary>
        /// IAlertHelperオブジェクト
        /// </summary>
        private IAlertHelper AlertHelper
        {
            get { return Singleton<AlertHelper>.Instance; }
        }

        /// <summary>
        /// Intersystech.UI.ISTImageButtonのClick イベントを発生させます。
        /// </summary>
        protected override void OnClick(ImageClickEventArgs e)
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
