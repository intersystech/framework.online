using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Intersystech.Utility;
using Intersystech.ExceptionLibrary;

[assembly: TagPrefix("Intersystech.UI", "ist")]
namespace Intersystech.UI
{
    /// <summary>
    /// ISTListViewクラス
    /// </summary>
    public class ISTListView : ListView
    {
        /// <summary>
        /// IAlertHelperオブジェクト
        /// </summary>
        private IAlertHelper AlertHelper
        {
            get { return Singleton<AlertHelper>.Instance; }
        }

        /// <summary>
        /// Intersystech.UI.ISTListViewのItemCanceling イベントを発生させます。
        /// </summary>
        protected override void OnItemCanceling(ListViewCancelEventArgs e)
        {
            try
            {
                base.OnItemCanceling(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }

        /// <summary>
        /// Intersystech.UI.ISTListViewのItemCommand イベントを発生させます。
        /// </summary>
        protected override void OnItemCommand(ListViewCommandEventArgs e)
        {
            try
            {
                base.OnItemCommand(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
        /// <summary>
        /// Intersystech.UI.ISTListViewのItemCreated イベントを発生させます。
        /// </summary>
        protected override void OnItemCreated(ListViewItemEventArgs e)
        {
            try
            {
                base.OnItemCreated(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
        /// <summary>
        /// Intersystech.UI.ISTListViewのItemDataBound イベントを発生させます。
        /// </summary>
        protected override void OnItemDataBound(ListViewItemEventArgs e)
        {

            try
            {
                base.OnItemDataBound(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
        /// <summary>
        /// Intersystech.UI.ISTListViewのItemDeleted イベントを発生させます。
        /// </summary>
        protected override void OnItemDeleted(ListViewDeletedEventArgs e)
        {
            try
            {
                base.OnItemDeleted(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
        /// <summary>
        /// Intersystech.UI.ISTListViewのItemDeleting イベントを発生させます。
        /// </summary>
        protected override void OnItemDeleting(ListViewDeleteEventArgs e)
        {
            try
            {
                base.OnItemDeleting(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
        /// <summary>
        /// Intersystech.UI.ISTListViewのItemEditing イベントを発生させます。
        /// </summary>
        protected override void OnItemEditing(ListViewEditEventArgs e)
        {
            try
            {
                base.OnItemEditing(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
        /// <summary>
        /// Intersystech.UI.ISTListViewのItemInserted イベントを発生させます。
        /// </summary>
        protected override void OnItemInserted(ListViewInsertedEventArgs e)
        {
            try
            {
                base.OnItemInserted(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
        /// <summary>
        /// Intersystech.UI.ISTListViewのItemInserting イベントを発生させます。
        /// </summary>
        protected override void OnItemInserting(ListViewInsertEventArgs e)
        {
            try
            {
                base.OnItemInserting(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
        /// <summary>
        /// Intersystech.UI.ISTListViewのItemUpdated イベントを発生させます。
        /// </summary>
        protected override void OnItemUpdated(ListViewUpdatedEventArgs e)
        {
            try
            {
                base.OnItemUpdated(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
        /// <summary>
        /// Intersystech.UI.ISTListViewのItemUpdating イベントを発生させます。
        /// </summary>
        protected override void OnItemUpdating(ListViewUpdateEventArgs e)
        {
            try
            {
                base.OnItemUpdating(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
        /// <summary>
        /// Intersystech.UI.ISTListViewのLayoutCreated イベントを発生させます。
        /// </summary>
        protected override void OnLayoutCreated(EventArgs e)
        {
            try
            {
                base.OnLayoutCreated(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
        /// <summary>
        /// Intersystech.UI.ISTListViewのPagePropertiesChanged イベントを発生させます。
        /// </summary>
        protected override void OnPagePropertiesChanged(EventArgs e)
        {
            try
            {
                base.OnPagePropertiesChanged(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
        /// <summary>
        /// Intersystech.UI.ISTListViewのPagePropertiesChanging イベントを発生させます。
        /// </summary>
        protected override void OnPagePropertiesChanging(PagePropertiesChangingEventArgs e)
        {
            try
            {
                base.OnPagePropertiesChanging(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
        /// <summary>
        /// Intersystech.UI.ISTListViewのSelectedIndexChanged イベントを発生させます。
        /// </summary>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            try
            {
                base.OnSelectedIndexChanged(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }

        /// <summary>
        /// Intersystech.UI.ISTListViewのSelectedIndexChanging イベントを発生させます。
        /// </summary>
        protected override void OnSelectedIndexChanging(ListViewSelectEventArgs e)
        {
            try
            {
                base.OnSelectedIndexChanging(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }

        /// <summary>
        /// Intersystech.UI.ISTListViewのSorted イベントを発生させます。
        /// </summary>
        protected override void OnSorted(EventArgs e)
        {
            try
            {
                base.OnSorted(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }

        /// <summary>
        /// Intersystech.UI.ISTListViewのSorting イベントを発生させます。
        /// </summary>
        protected override void OnSorting(ListViewSortEventArgs e)
        {
            try
            {
                base.OnSorting(e);
            }
            catch (AlertCustomSystemException acsex)
            {
                AlertHelper.Alert(acsex.Message);
            }
        }
    }
}
