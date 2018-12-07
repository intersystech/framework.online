using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Intersystech.DataModel
{
    /// <summary>
    /// リクエストDtoクラス
    /// </summary>
    [Serializable]
    public class RequestDto : Dto
    {
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
