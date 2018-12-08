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
    /// SessionHelperクラス
    /// </summary>
    public sealed class SessionHelper
    {
        private const string KEY_ERROR = "CustomError";
        private const string KEY_USERINFO = "UserInfo";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private SessionHelper()
        {
        }

        /// <summary>
        /// 現在のセッションをキャンセルします。
        /// </summary>
        public static void Abandon()
        {
            HttpContext.Current.Session.Abandon();
        }

        /// <summary>
        /// TPage名＋キー名でセッション値を設定します。
        /// </summary>
        /// <typeparam name="TPage">Page型</typeparam>
        /// <param name="key">セッション値のキー名</param>
        /// <param name="value">指定された名前のセッション値</param>
        public static void Set<TPage>(string key, object value) where TPage : Page
        {
            Page page = Singleton<TPage>.Instance;
            string pageName = typeof(TPage).Name;
            string sessionKey = string.Concat(key, "_", pageName);
            Set(sessionKey, value);
        }

        /// <summary>
        /// ユーザ情報をセッションに設定します。
        /// </summary>
        /// <param name="value">ユーザ情報</param>
        public static void SetUserInfo(object value)
        {
            Set(KEY_USERINFO, value);
        }

        /// <summary>
        /// 名前別のセッション値を設定します。
        /// </summary>
        /// <param name="key">セッション値のキー名</param>
        /// <param name="value">指定された名前のセッション値</param>
        public static void Set(string key, object value)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[key] = value;
            }
        }

        /// <summary>
        /// ユーザ情報を取得します。
        /// </summary>
        /// <typeparam name="TType">class型</typeparam>
        /// <returns>オブジェクト</returns>
        public static TType GetUserInfo<TType>() where TType : class
        {
            return Get(KEY_USERINFO) as TType;
        }

        /// <summary>
        /// セッションキーに一致するオブジェクトを取得します。
        /// </summary>
        /// <typeparam name="TPage">Page型</typeparam>
        /// <typeparam name="TType">class型</typeparam>
        /// <param name="key">セッションキー</param>
        /// <returns>オブジェクト</returns>
        public static TType Get<TPage, TType>(string key)
            where TPage : Page
            where TType : class
        {
            string pageName = typeof(TPage).Name;
            string sessionKey = string.Concat(key, "_", pageName);

            return Get(sessionKey) as TType;
        }

        /// <summary>
        /// TPage名＋セッションキーに一致するオブジェクトを取得します。
        /// </summary>
        /// <typeparam name="TPage">Page型</typeparam>
        /// <param name="key">セッションキー</param>
        /// <returns>オブジェクト</returns>
        public static object Get<TPage>(string key) where TPage : Page
        {
            string pageName = typeof(TPage).Name;
            string sessionKey = string.Concat(key, "_", pageName);

            return Get(sessionKey);
        }

        /// <summary>
        /// セッションキーに一致するオブジェクトを取得します。
        /// </summary>
        /// <param name="key">セッションキー</param>
        /// <returns>オブジェクト</returns>
        public static object Get(string key)
        {
            if (IsNullOrTimeout() == true)
            {
                return null;
            }
            return HttpContext.Current.Session[key];
        }

        /// <summary>
        /// セッション状態のコレクションから項目を削除します。
        /// </summary>
        /// <param name="key">セッション状態のコレクションから削除する項目の名前</param>
        public static void Remove(string key)
        {
            if (HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Remove(key);
            }
        }

        /// <summary>
        /// セッション状態のコレクションから項目を削除します。
        /// </summary>
        /// <typeparam name="TPage">Page型</typeparam>
        /// <param name="key">セッションキー</param>
        public static void Remove<TPage>(string key) where TPage : Page
        {
            string pageName = typeof(TPage).Name;
            string sessionKey = string.Concat(key, "_", pageName);
            Remove(sessionKey);
        }

        /// <summary>
        /// セッション状態のコレクションからすべてのキーと値を削除します。
        /// </summary>
        public static void Clear()
        {
            if (IsNullOrTimeout() == false)
            {
                HttpContext.Current.Session.Clear();
            }
        }

        /// <summary>
        /// Web.configファイルのセッションのタイムアウト値を取得します。
        /// </summary>
        /// <returns>タイムアウト時間(分単位)</returns>
        public static int GetTimeout()
        {
            return ConfigHelper.GetTimeout();
        }

        /// <summary>
        /// Web.configファイルのセッションのタイムアウト値を設定します。
        /// </summary>
        /// <param name="minutes">タイムアウト時間(分単位)</param>
        public static void SetTimeout(int minutes)
        {
            ConfigHelper.SetTimeout(minutes);
        }

        /// <summary>
        /// エラーを設定します。
        /// </summary>
        /// <param name="exception">エラー</param>
        public static void SetError(Exception exception)
        {
            Set(KEY_ERROR, exception);
        }

        /// <summary>
        /// エラーを取得します。
        /// </summary>
        /// <returns>エラーメッセージ</returns>
        public static Exception GetError()
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null)
            {
                return null;
            }
            return HttpContext.Current.Session[KEY_ERROR] as Exception;
        }

        /// <summary>
        /// システムのセッション状態がタイムアウト値に達したかをチェックします。
        /// </summary>
        /// <returns>false:有効,true:無効</returns>
        public static bool IsNullOrTimeout()
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null)
            {
                return true;
            }
            else
            {
                if (HttpContext.Current.Session.IsNewSession)
                {
                    string cookie = HttpContext.Current.Request.Headers["Cookie"];
                    if (null != cookie && cookie.IndexOf("ASP.NET_SessionId") >= 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
