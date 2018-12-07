using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Intersystech.DataModel
{
    /// <summary>
    /// Dto抽象クラス
    /// </summary>
    public abstract class Dto
    {
        private DateTime localDatetime = DateTime.Now;
        private string isDeleted = "0";
        private const string KEY_USERINFO = "UserInfo";

        private UserInfoBase GetUserInfoBase()
        {
            if(HttpContext.Current==null || HttpContext.Current.Session == null || HttpContext.Current.Session[KEY_USERINFO] == null)
            {
                return new UserInfoBase();
            }
            else
            {
                return HttpContext.Current.Session[KEY_USERINFO] as UserInfoBase;
            }
        }

        /// <summary>
        /// 削除されたかどうかを示す値
        /// </summary>
        public string IsDeleted
        {
            get
            {
                return this.isDeleted;
            }
            set
            {
                this.isDeleted = value;
            }
        }

        /// <summary>
        /// 作成ユーザID
        /// </summary>
        public string CreateUserID
        {
            set
            {
                GetUserInfoBase().Id = value;
            }
            get
            {
                return GetUserInfoBase().Id;
            }
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreateDatetime
        {
            get
            {
                return this.localDatetime;
            }
            set
            {
                this.localDatetime = value;
            }
        }

        /// <summary>
        /// 更新ユーザID
        /// </summary>
        public string UpdateUserID
        {
            set
            {
                GetUserInfoBase().Id = value;
            }
            get
            {
                return GetUserInfoBase().Id;
            }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime UpdateDatetime
        {
            get
            {
                return this.localDatetime;
            }
            set
            {
                this.localDatetime = value;
            }
        }
    }
}
