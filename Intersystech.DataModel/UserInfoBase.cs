using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersystech.DataModel
{
    /// <summary>
    /// ユーザ情報クラス
    /// </summary>
    [Serializable]
    public class UserInfoBase
    {
        private string id;
        /// <summary>
        /// ユーザID
        /// </summary>
        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        private string name;
        /// <summary>
        /// ユーザ名
        /// </summary>
        public string Name {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
    }
}
