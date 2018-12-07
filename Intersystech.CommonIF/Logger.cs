using System;
using System.IO;
using System.Linq;
using System.Text;
using Intersystech.ExceptionLibrary;
using Intersystech.Extension;
using Intersystech.Utility;
using System.Net;
using System.Web;
using System.Net.Sockets;
using System.Threading;

namespace Intersystech.CommonIF
{
    /// <summary>
    /// Loggerクラス
    /// </summary>
    public class Logger : ILogger
    {
        #region 変数
        
        /// <summary>
        /// ログ連結文字列
        /// </summary>
        private string LOG_CHAR = new string(':', 50);

        /// <summary>
        /// ファイル名に使えない文字
        /// </summary>
        private string[] BAD_VALUE = new string[] { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };

        #endregion

        #region プロパティ
        /// <summary>
        /// ログパス
        /// </summary>
        private string TraceLogPath
        {
            get
            {
                string traceLogPath = ConfigHelper.GetAppSettingsValue("TraceLogPath");
                var request = HttpContext.Current.Request;
                
                if(request == null)
                {
                    throw new ArgumentNullException("request");
                }

                traceLogPath = request.MapPath(traceLogPath);
                if (Directory.Exists(traceLogPath) == false)
                {
                    Directory.CreateDirectory(traceLogPath);
                }
                return traceLogPath;
            }
        }

        /// <summary>
        /// ログファイル名
        /// </summary>
        private string TraceLogName
        {
            get
            {
                string traceLogName = string.Empty;
                int logIndex = 1;
                DirectoryInfo directoryInfo = new DirectoryInfo(TraceLogPath);
                FileInfo latestFileInfo = directoryInfo.GetFiles()
                    .Where(x=>x.Name.Contains(UserId.ToString()))
                    .OrderByDescending(x => int.Parse(x.Name.Replace(x.Extension, string.Empty).Split('_')[1]))
                    .FirstOrDefault();
                if (latestFileInfo != null)
                {
                    string logName = latestFileInfo.Name.Replace(latestFileInfo.Extension, string.Empty);
                    if (int.TryParse(logName.Split('_')[1], out logIndex) == true)
                    {
                        logIndex = logIndex + 1;
                    }
                }

                return string.Concat("[", UserId, "]trace_", logIndex, ".log");
            }
        }

        /// <summary>
        /// ログファイルサイズ
        /// </summary>
        private long TraceLogSize
        {
            get
            {
                return long.Parse(ConfigHelper.GetAppSettingsValue("TraceLogSize"));
            }
        }

        /// <summary>
        /// ログレベル
        /// </summary>
        private int TraceLogLevel
        {
            get
            {
                return int.Parse(ConfigHelper.GetAppSettingsValue("TraceLogLevel"));
            }
        }

        /// <summary>
        /// ホストIPアドレス
        /// </summary>
        private string HostIPAddress
        {
            get
            {
                var request = HttpContext.Current.Request;
                string ipString;
                if (string.IsNullOrEmpty(request.ServerVariables["HTTP_X_FORWARDED_FOR"]) == false)
                {
                    ipString = request.ServerVariables["HTTP_X_FORWARDED_FOR"]
                        .Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .FirstOrDefault();
                }
                else
                {
                    ipString = request.ServerVariables["REMOTE_ADDR"];
                    // ローカルホスト名取得
                    if (request.IsLocal == true)
                    {
                        IPAddress[] ipAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                        ipString = ipAddresses.Where(x => x.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault().ToString();
                    }
                }

                return ipString;
            }
        }

        /// <summary>
        /// ユーザID
        /// </summary>
        private string UserId
        {
            get
            {
                if (HttpContext.Current == null || HttpContext.Current.User == null)
                    return string.Empty;

                string userId = HttpContext.Current.User.Identity.Name;
                return this.TrimBadChar(userId);
            }
        }

        /// <summary>
        /// ログファイル自動削除設定されているか
        /// </summary>
        private bool IsAutoClear
        {
            get
            {
                return ConfigHelper.GetAppSettingsValue("IsTraceLogAutoClear") == "1";
            }
        }

        /// <summary>
        /// ログファイル自動削除基準値
        /// </summary>
        private int ClearDays
        {
            get
            {
                return Convert.ToInt32(ConfigHelper.GetAppSettingsValue("TraceLogAutoClearDays"));
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 指定文字列をログファイルに記録します。
        /// </summary>
        /// <param name="log">ログファイルオブジェクト</param>
        /// <param name="logType">ログタイプ</param>
        public void Write(Log log, LogType logType)
        {
            try
            {
                Write(log.Message, log.StackTrace, logType);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 指定文字列をログファイルに記録します。
        /// </summary>
        /// <param name="message">ログを説明するメッセージ</param>
        public void Write(string message)
        {
            try
            {
                this.Write(message, null, LogType.Information);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 指定文字列をログファイルに記録します。
        /// </summary>
        /// <param name="message">ログを説明するメッセージ</param>
        /// <param name="logType">ログタイプ</param>
        public void Write(string message, LogType logType)
        {
            try
            {
                this.Write(message, null, logType);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 指定文字列をログファイルに記録します。
        /// </summary>
        /// <param name="message">ログを説明するメッセージ</param>
        /// <param name="stackTrace">呼び出し履歴の直前のフレームを説明する文字列</param>
        /// <param name="logType">ログタイプ</param>
        public void Write(string message, string stackTrace, LogType logType)
        {
            if (this.TraceLogLevel == 0)
            {
                // ログを出力しない
                return;
            }
            else if (this.TraceLogLevel == 1 && logType != LogType.Error)
            {
                // エラー情報のみ出力する
                return;
            }
            else if (string.IsNullOrEmpty(UserId) == true)
            {
                // セッションのユーザIDが設定されなければ、リターン
                return;
            }

            string logFile = Path.Combine(TraceLogPath, TraceLogName);
            string logFileCopy = Path.GetTempFileName();

            try
            {
                string userId = UserId;
                DirectoryInfo directoryInfo = new DirectoryInfo(TraceLogPath);
                FileInfo latestFileInfo = directoryInfo.GetFiles()
                    .Where(x => x.Name.Contains(userId))
                    .OrderByDescending(x => int.Parse(x.Name.Replace(x.Extension, string.Empty).Split('_')[1])).FirstOrDefault();
                if (latestFileInfo != null)
                {
                    // 指定サイズを超えない場合、既存ファイル利用
                    if (latestFileInfo.Length <= this.TraceLogSize)
                    {
                        logFile = latestFileInfo.FullName;
                    }
                }

                StringBuilder logText = new StringBuilder();
                // 固定分
                logText.Append(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                logText.Append(StringExtension.WhiteSpace);
                logText.Append(HostIPAddress);
                logText.Append(StringExtension.WhiteSpace);
                logText.Append(userId);
                logText.Append(StringExtension.WhiteSpace);
                // 可変分
                string messageFormat = "[{0}{1}{2}]";
                if (logType == LogType.Start)
                {
                    message = string.Format(messageFormat, "メソッド実行開始", LOG_CHAR, message);
                }
                else if (logType == LogType.Stop)
                {
                    message = string.Format(messageFormat, "メソッド実行完了", LOG_CHAR, message);
                }
                else if (logType == LogType.Error)
                {
                    message = "【エラー】" + message;
                }
                else if (logType == LogType.Information)
                {
                    message = "[" + message + "]";
                }

                if (string.IsNullOrEmpty(message) == true)
                {
                    logText.AppendLine();
                }
                else
                {
                    logText.AppendLine(message);
                }

                if (string.IsNullOrEmpty(stackTrace) == false)
                {
                    logText.AppendLine(stackTrace);
                }

                using (FileStream stream = new FileStream(logFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                {
                    using (FileStream streamCopy = new FileStream(logFileCopy, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        stream.CopyTo(streamCopy);
                        using (StreamWriter sw = new StreamWriter(streamCopy, Encoding.Default))
                        {
                            // 自動的にフラッシュされるようにする
                            sw.AutoFlush = true;

                            // スレッドセーフラッパを作成
                            TextWriter tw = TextWriter.Synchronized(sw);
                            tw.Write(logText.ToString());
                            tw.Flush();
                            tw.Close();
                        }
                    }
                }

                File.Copy(logFileCopy, logFile, true);
            }
            catch (IOException)
            {
                File.Copy(logFileCopy, Path.Combine(TraceLogPath, TraceLogName));
            }
            catch (Exception ex)
            {
                ExceptionUtility.Throw(ex.Message);
            }
            finally
            {
                File.Delete(logFileCopy);
            }
        }

        /// <summary>
        /// 最終更新日が、現在の日付から指定された削除基準日数を減算した日付より小さいまたは等しい
        /// トレースログファイルを削除します。
        /// <para>最終更新日 &#8804; 現在の日付 - 削除基準日数 のトレースログファイルを削除します</para>
        /// </summary>
        public void AutoClear()
        {
            if (IsAutoClear == false)
                return;

            try
            {
                string[] files = Directory.GetFiles(TraceLogPath);
                DateTime today = DateTime.Today;
                DateTime deleteDay = today.AddDays(-ClearDays);

                var filesForDelete = files.Where(x => Convert.ToDateTime(File.GetLastWriteTime(x).ToShortDateString()).CompareTo(deleteDay) <= 0);
                foreach (var file in filesForDelete)
                {
                    File.Delete(file.ToString());
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// ファイル名に使えない文字を削除します
        /// </summary>
        /// <param name="value">文字列</param>
        /// <returns>文字列</returns>
        private string TrimBadChar(string value)
        {
            string result = value;
            foreach (string badChar in BAD_VALUE)
            {
                result = result.Replace(badChar, string.Empty);
            }
            return result;
        }

        #endregion
    }
}
