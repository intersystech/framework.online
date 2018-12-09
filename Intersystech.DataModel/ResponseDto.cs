using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Intersystech.DataModel
{
    /// <summary>
    /// レスポンスDtoクラス
    /// </summary>
    [Serializable]
    public class ResponseDto : Dto
    {
        private DataSet resultSet = new DataSet();

        /// <summary>
        /// 実行結果データセット
        /// </summary>
        public DataSet AffectedResultSet
        {
            get
            {
                return this.resultSet;
            }
            set
            {
                this.resultSet = value;
            }
        }

        /// <summary>
        /// 実行結果オブジェクト
        /// </summary>
        public object AffectedObject
        {
            get;
            set;
        }

        private int affectedCount = 0;
        /// <summary>
        /// 実行結果件数
        /// </summary>
        public int AffectedCount
        {
            set
            {
                this.affectedCount = value;
            }
            get
            {
                return this.affectedCount;
            }
        }

        private DateTime localDatetime = DateTime.Now;
        /// <summary>
        /// システム日時
        /// </summary>
        public DateTime SystemDatetime
        {
            get
            {
                return localDatetime;
            }
        }
    }
}
