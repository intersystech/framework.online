using Intersystech.ExceptionLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Web.Configuration;

namespace Intersystech.Utility
{
    /// <summary>
    /// メールヘルパークラス
    /// <para>メールの送信機能を提供します。</para>
    /// </summary>
    public sealed class MailHelper
    {
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private MailHelper()
        {
        }
        #endregion

        #region 変数
        private static SmtpSection smtpSection = (SmtpSection)WebConfigurationManager.GetWebApplicationSection("system.net/mailSettings/smtp");
        #endregion

        #region プロパティ
        /// <summary>
        /// Web.Config構成ファイルの SMTP セクションを取得します。
        /// </summary>
        public static SmtpSection SmtpSection
        {
            get
            {
                return smtpSection;
            }
        }
        #endregion

        #region メソッド
        /// <summary>
        /// メールを送信します。
        /// </summary>
        /// <param name="host">SMTP トランザクションで使用されるホストの名前または IP アドレス</param>
        /// <param name="port">SMTP トランザクションで使用されるポート</param>
        /// <param name="userName">資格情報と関連付けられたユーザー名</param>
        /// <param name="password">資格情報と関連付けられたユーザー名のパスワード</param>
        /// <param name="from">電子メールの差出人アドレス</param>
        /// <param name="to">電子メールの受信者のアドレス</param>
        /// <param name="subject">電子メールの件名</param>
        /// <param name="body">メッセージ本文</param>
        /// <param name="timeout">タイムアウト値(ミリ秒)</param>
        public static void Send(string host
            , int port
            , string userName
            , string password
            , string from
            , string to
            , string subject
            , string body
            , int timeout = 15000)
        {
            //SmtpClientの作成
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(userName, password);
            //SMTPサーバーなどを設定する
            smtpClient.Host = host;
            smtpClient.Port = port;
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Timeout = timeout;
            smtpClient.ServicePoint.MaxIdleTime = 1;

            //メールを送信する
            MailMessage mailMessage = new MailMessage(from, to, subject, body);
            mailMessage.SubjectEncoding = UTF8Encoding.UTF8;
            mailMessage.BodyEncoding = UTF8Encoding.UTF8;
            smtpClient.Send(mailMessage);
        }

        /// <summary>
        /// メールを送信します。
        /// </summary>
        /// <param name="from">電子メールの差出人アドレス</param>
        /// <param name="to">電子メールの受信者のアドレス</param>
        /// <param name="subject">電子メールの件名</param>
        /// <param name="body">メッセージ本文</param>
        /// <param name="timeout">タイムアウト値(ミリ秒)</param>
        public static void Send(string from, string to, string subject, string body, int timeout = 15000)
        {
            //SmtpClientの作成
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(SmtpSection.Network.UserName, SmtpSection.Network.Password);

            //SMTPサーバーなどを設定する
            smtpClient.Host = SmtpSection.Network.Host;
            smtpClient.Port = SmtpSection.Network.Port;
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Timeout = timeout;
            smtpClient.ServicePoint.MaxIdleTime = 1;

            //メールを送信する
            MailMessage mailMessage = new MailMessage(from, to, subject, body);
            mailMessage.SubjectEncoding = UTF8Encoding.UTF8;
            mailMessage.BodyEncoding = UTF8Encoding.UTF8;
            smtpClient.Send(mailMessage);
        }

        #endregion
    }
}
