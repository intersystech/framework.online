using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Intersystech.Utility;
using Intersystech.Extension;
using Intersystech.ExceptionLibrary;

[assembly: TagPrefix("Intersystech.UI", "ist")]
namespace Intersystech.UI
{
    /// <summary>
    /// ISTDropDownListクラス
    /// </summary>
    public class ISTDropDownList : DropDownList
    {
        /// <summary>
        /// IAlertHelperオブジェクト
        /// </summary>
        private IAlertHelper AlertHelper
        {
            get { return Singleton<AlertHelper>.Instance; }
        }

        /// <summary>
        /// Intersystech.UI.ISTDropDownList コントロールのSelectedValueプロパティを取得または設定します。
        /// </summary>
        public new string Text
        {
            get
            {
                return base.Text.ToSafeValue();
            }
            set
            {
                base.Text = value;
            }
        }

        /// <summary>
        /// Intersystech.UI.ISTDropDownList コントロールのSelectedValueプロパティを取得します。
        /// </summary>
        public new string SelectedValue
        {
            get
            {
                return base.SelectedValue.ToSafeValue();
            }
            set
            {
                base.SelectedValue = value;
            }
        }

        /// <summary>
        /// Intersystech.UI.ISTDropDownList コントロールの表示されるテキストを取得します。
        /// </summary>
        public string SelectedText
        {
            get
            {
                ListItem selectedItem = base.SelectedItem;
                if (selectedItem == null)
                    return string.Empty;

                return selectedItem.Text.ToSafeValue();
            }
        }

        /// <summary>
        /// Intersystech.UI.ISTDropDownListのSelectedIndexChanged イベントを発生させます。
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
        /// Intersystech.UI.ISTDropDownListのTextChanged イベントを発生させます。
        /// </summary>
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

        /// <summary>
        /// 呼び出されたサーバー コントロールと、そのすべての子コントロールにデータ ソースをバインドします。
        /// </summary>
        /// <param name="ds">データセット</param>
        /// <param name="dataTextField">リスト項目のテキストの内容のフィールド</param>
        /// <param name="dataValueField">リスト項目のテキストの値のフィールド</param>
        /// <param name="isTopBlank">先頭に空白行を含めるか</param>
        public void DataBind(DataSet ds, string dataTextField, string dataValueField, bool isTopBlank)
        {
            this.DataBind(ds.Tables[0], dataTextField, dataValueField, isTopBlank);
        }

        /// <summary>
        /// 呼び出されたサーバー コントロールと、そのすべての子コントロールにデータ ソースをバインドします。
        /// </summary>
        /// <param name="dt">データテーブル</param>
        /// <param name="dataTextField">リスト項目のテキストの内容のフィールド</param>
        /// <param name="dataValueField">リスト項目のテキストの値のフィールド</param>
        /// <param name="isTopBlank">先頭に空白行を含めるか</param>
        public void DataBind(DataTable dt, string dataTextField, string dataValueField, bool isTopBlank)
        {
            if (dt.Rows.Count == 0)
            {
                DataTable dtBlank = new DataTable();
                dtBlank.Columns.Add(dataTextField, typeof(string));
                dtBlank.Columns.Add(dataValueField, typeof(string));
                DataRow newRow = dtBlank.NewRow();
                newRow[dataTextField] = string.Empty;
                newRow[dataValueField] = string.Empty;

                dtBlank.Rows.InsertAt(newRow, 0);
                dtBlank.AcceptChanges();

                base.DataSource = dtBlank;
            }
            else
            {
                DataTable dataSource = new DataTable();
                dataSource = (DataTable)dt.Copy();
                if (isTopBlank == true)
                {
                    DataRow newRow = dataSource.NewRow();
                    newRow[dataTextField] = string.Empty;
                    newRow[dataValueField] = string.Empty;
                    dataSource.Rows.InsertAt(newRow, 0);
                    dataSource.AcceptChanges();
                }
                base.DataSource = dataSource;
            }

            base.DataTextField = dataTextField;
            base.DataValueField = dataValueField;
            base.DataBind();
        }
    }
}
