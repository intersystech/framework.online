using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Intersystech.Utility;
using Intersystech.ExceptionLibrary;

[assembly: TagPrefix("Intersystech.UI", "ist")]
namespace Intersystech.UI
{
    /// <summary>
    /// ISTButtonクラス
    /// </summary>
    public class ISTButton : Button
    {
        /// <summary>
        /// IAlertHelperオブジェクト
        /// </summary>
        private IAlertHelper AlertHelper
        {
            get { return Singleton<AlertHelper>.Instance; }
        }

        /// <summary>
        /// Intersystech.UI.ISTButtonのClick イベントを発生させます。
        /// </summary>
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
