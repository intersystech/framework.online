using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Configuration;
using System.Configuration;
using Intersystech.DataModel;

namespace Intersystech.Utility
{
    /// <summary>
    /// CookieHelperクラス
    /// </summary>
    public sealed class CookieHelper
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private CookieHelper()
        {
        }

        /// <summary>
        /// 名前別のセッション値を設定します。
        /// </summary>
        /// <param name="name">クッキーの名前</param>
        /// <param name="key">クッキー値のキー名</param>
        /// <param name="value">クッキーの値</param>
        /// <param name="expires">クッキーの有効期限</param>
        public static void Set(string name,string key, string value, DateTime expires)
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies[name];
            if (httpCookie == null)
            {
                httpCookie = new HttpCookie(name);
            }
            
            httpCookie[key] = value;
            httpCookie.Expires = expires;
            HttpContext.Current.Response.Cookies.Add(httpCookie);
        }

        /// <summary>
        /// セッションキーに一致するオブジェクトを取得します。
        /// </summary>
        /// <param name="name">クッキーの名前</param>
        /// <param name="key">クッキーのキー名</param>
        /// <returns>クッキーの値</returns>
        public static string Get(string name, string key)
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies[name];
            if (httpCookie!=null)
            {
                if (httpCookie[key] != null)
                {
                    return httpCookie[key];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// クッキーを削除します。
        /// </summary>
        /// <param name="name">クッキーの名前</param>
        public static void Remove(string name)
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies[name];
            if (httpCookie != null)
            {
                httpCookie.Expires = DateTime.Now.AddDays(-1d);
                HttpContext.Current.Response.Cookies.Add(httpCookie);
            }            
        }
    }
}
