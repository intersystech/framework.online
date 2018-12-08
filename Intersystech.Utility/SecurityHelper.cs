using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Intersystech.Utility
{
    /// <summary>
    /// SecurityHelperクラス
    /// </summary>
    public sealed class SecurityHelper
    {
        /// <summary>
        /// 対称アルゴリズムに使用する共有キー
        /// </summary>
        private const string KEY = "security";
        /// <summary>
        /// 対称アルゴリズムに使用する初期化ベクター
        /// </summary>
        private const string IV = "security";
        /// <summary>
        /// トークン生成に使用するバイト数
        /// </summary>
        private const int TOKEN_LENGTH = 16;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private SecurityHelper()
        {
        }

        /// <summary>
        /// 文字列を暗号化します。
        /// </summary>
        /// <param name="text">暗号化対象文字列</param>
        /// <returns>暗号化された文字列</returns>
        public static string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text) == true)
                return string.Empty;

            byte[] beforeEncrypt;
            byte[] afterEncrypt;

            UTF8Encoding encoding = new UTF8Encoding();

            DES des = new DESCryptoServiceProvider();
            ICryptoTransform encryptor = des.CreateEncryptor(encoding.GetBytes(KEY), encoding.GetBytes(IV));

            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            beforeEncrypt = encoding.GetBytes(text);

            csEncrypt.Write(beforeEncrypt, 0, beforeEncrypt.Length);
            csEncrypt.FlushFinalBlock();

            afterEncrypt = msEncrypt.ToArray();

            return Convert.ToBase64String(afterEncrypt);
        }

        /// <summary>
        /// 文字列を復号化します。
        /// </summary>
        /// <param name="text">復号化対象文字列</param>
        /// <returns>復号化された文字列</returns>
        public static string Decrypt(string text)
        {
            if (string.IsNullOrEmpty(text) == true)
                return string.Empty;

            byte[] beforeDecrypt;
            byte[] afterDecrypt;

            UTF8Encoding encoding = new UTF8Encoding();

            DES des = new DESCryptoServiceProvider();
            ICryptoTransform decryptor = des.CreateDecryptor(encoding.GetBytes(KEY), encoding.GetBytes(IV));

            MemoryStream msDecrypt = new MemoryStream();
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write);

            beforeDecrypt = Convert.FromBase64String(text);

            csDecrypt.Write(beforeDecrypt, 0, beforeDecrypt.Length);
            csDecrypt.FlushFinalBlock();

            afterDecrypt = msDecrypt.ToArray();

            return encoding.GetString(afterDecrypt);
        }

        /// <summary>
        /// 32バイトのトークンを作成します。
        /// </summary>
        /// <returns>トークン文字列</returns>
        public static string GetToken()
        {
            byte[] token = new byte[TOKEN_LENGTH];

            RNGCryptoServiceProvider gen = new RNGCryptoServiceProvider();
            gen.GetBytes(token);

            StringBuilder buf = new StringBuilder();

            for (int i = 0; i < token.Length; i++)
            {
                //16*2=32バイト
                buf.AppendFormat("{0:x2}", token[i]);
            }

            return buf.ToString();
        }

        /// <summary>
        /// 指定した長さのバイト数のトークンを作成します。
        /// </summary>
        /// <returns>トークン文字列</returns>
        public static string GetToken(int tokenLength)
        {
            byte[] token = new byte[tokenLength];

            RNGCryptoServiceProvider gen = new RNGCryptoServiceProvider();
            gen.GetBytes(token);

            StringBuilder buf = new StringBuilder();

            for (int i = 0; i < token.Length; i++)
            {
                //tokenLength*2バイト
                buf.AppendFormat("{0:x2}", token[i]);
            }

            return buf.ToString();
        }

        /// <summary>
        /// URLクエリ文字列を暗号化します。
        /// </summary>
        /// <param name="text">暗号化対象URLクエリ文字列</param>
        /// <returns>暗号化されたURLクエリ文字列</returns>
        public static string EncryptQueryString(string text)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(text);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                    }
                    text = Convert.ToBase64String(ms.ToArray());
                }
            }
            return HttpUtility.UrlEncode(text);
        }

        /// <summary>
        /// URLクエリ文字列を複合化します。
        /// </summary>
        /// <param name="text">復号化対象URLクエリ文字列</param>
        /// <returns>復号化されたURLクエリ文字列</returns>
        public static string DecryptQueryString(string text)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            text = HttpUtility.UrlDecode(text).Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(text);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                    }
                    text = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return text;
        }

        /// <summary>
        /// ゲートウェイのIPアドレスを取得します。
        /// </summary>
        /// <returns>ゲートウェイのIPアドレス文字列</returns>
        public static string GetGatewayIPAddress()
        {
            string ip = null;
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (GatewayIPAddressInformation gateway in adapter.GetIPProperties().GatewayAddresses)
                    {
                        if (gateway.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ip = gateway.Address.MapToIPv4().ToString();
                            break;
                        }
                    }
                }
            }

            return ip;
        }
    }
}
