using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Configuration;
using System.Web.UI;

namespace Intersystech.Utility
{
    /// <summary>
    /// ConfigHelperクラス
    /// </summary>
    public sealed class ConfigHelper
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private ConfigHelper()
        {
        }

        /// <summary>
        /// Web.configファイルのセッションのタイムアウト値を取得します。
        /// </summary>
        /// <returns>タイムアウト時間(分単位)</returns>
        public static int GetTimeout()
        {
            var sessionState = WebConfigurationManager.GetSection("system.web/sessionState") as SessionStateSection;
            return sessionState.Timeout.Minutes;
        }

        /// <summary>
        /// Web.configファイルのセッションのタイムアウト値を設定します。
        /// </summary>
        /// <param name="minutes">タイムアウト時間(分単位)</param>
        public static void SetTimeout(int minutes)
        {
            var configuration = WebConfigurationManager.OpenWebConfiguration("/");
            var sessionState = configuration.GetSection("system.web/sessionState") as SessionStateSection;

            sessionState.Timeout = new TimeSpan(0, minutes, 0);

            configuration.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// urlMappings 構成セクションより、ユーザーに表示される URL を取得します。
        /// </summary>
        /// <param name="pageName">Webページの物理クラス名</param>
        /// <returns>ユーザーに表示される URL</returns>
        public static string GetUrl(string pageName)
        {
            string url = string.Empty;
            pageName = pageName.LastIndexOf(".aspx") == -1 ? string.Concat(pageName, ".aspx") : pageName;

            UrlMappingsSection urlMappingSection = WebConfigurationManager.GetSection("system.web/urlMappings") as UrlMappingsSection;
            foreach (UrlMapping urlMapping in urlMappingSection.UrlMappings)
            {
                if (urlMapping.MappedUrl.LastIndexOf(pageName) != -1)
                {
                    url = urlMapping.Url;
                    break;
                }
            }
            return url;
        }

        /// <summary>
        /// urlMappings 構成セクションより、Webアプリケーション内の URL を取得します。
        /// </summary>
        /// <param name="pageName">Webページの物理クラス名</param>
        /// <returns>ユーザーに表示される URL</returns>
        public static string GetMappedUrl(string pageName)
        {
            string url = string.Empty;
            pageName = pageName.LastIndexOf(".aspx") == -1 ? string.Concat(pageName, ".aspx") : pageName;

            UrlMappingsSection urlMappingSection = WebConfigurationManager.GetSection("system.web/urlMappings") as UrlMappingsSection;
            foreach (UrlMapping urlMapping in urlMappingSection.UrlMappings)
            {
                if (urlMapping.MappedUrl.LastIndexOf(pageName) != -1)
                {
                    url = urlMapping.MappedUrl;
                    break;
                }
            }
            return url;
        }

        /// <summary>
        /// urlMappings 構成セクションより、ユーザーに表示される URL を取得します。
        /// </summary>
        /// <typeparam name="TPage">Webページ</typeparam>
        /// <returns>ユーザーに表示される URL</returns>
        public static string GetUrl<TPage>() where TPage : Page
        {
            string pageName = string.Concat(typeof(TPage).Name, ".aspx");
            return GetUrl(pageName);
        }

        /// <summary>
        /// urlMappings 構成セクションより、ユーザーに表示される URL を取得します。
        /// </summary>
        /// <param name="mappedUrlIndex">取得するURLMappingのインデックス</param>
        /// <returns>ユーザーに表示される URL</returns>
        public static string GetUrl(int mappedUrlIndex)
        {
            UrlMappingsSection urlMappingSection = WebConfigurationManager.GetSection("system.web/urlMappings") as UrlMappingsSection;
            return urlMappingSection.UrlMappings[mappedUrlIndex].Url;
        }

        /// <summary>
        /// urlMappings 構成セクションより、Webアプリケーション内の URL を取得します。
        /// </summary>
        /// <typeparam name="TPage">Webページ</typeparam>
        /// <returns>ユーザーに表示される URL</returns>
        public static string GetMappedUrl<TPage>() where TPage : Page
        {
            string pageName = string.Concat(typeof(TPage).Name, ".aspx");
            return GetMappedUrl(pageName);
        }

        /// <summary>
        /// urlMappings 構成セクションより、Webアプリケーション内の URL を取得します。
        /// </summary>
        /// <param name="mappedUrlIndex">取得するURLMappingのインデックス</param>
        /// <returns>ユーザーに表示される URL</returns>
        public static string GetMappedUrl(int mappedUrlIndex)
        {
            UrlMappingsSection urlMappingSection = WebConfigurationManager.GetSection("system.web/urlMappings") as UrlMappingsSection;
            return urlMappingSection.UrlMappings[mappedUrlIndex].MappedUrl;
        }

        /// <summary>
        /// 現在の Web アプリケーションの既定の構成の System.Configuration.AppSettingsSection データを取得します。
        /// </summary>
        /// <param name="key">AppSettingsキー</param>
        /// <returns>現在の Web アプリケーションの既定の構成の System.Configuration.AppSettingsSection オブジェクトを格納している<br/>System.Collections.Specialized.NameValueCollection オブジェクト。</returns>
        public static string GetAppSettingsValue(string key)
        {
            return WebConfigurationManager.AppSettings[key];
        }
    }
}
