using System;
using System.Globalization;

namespace Intersystech.Extension
{
    /// <summary>
    /// DateTime型の拡張クラス
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// yyyyMMdd形式の文字列を返します。
        /// </summary>
        /// <param name="dateTime">対象日付型オブジェクト</param>
        /// <returns>yyyyMMdd形式の文字列</returns>
        public static string ToStrYmd(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
        }

        /// <summary>
        /// yyyy/MM/dd形式の文字列を返します。
        /// </summary>
        /// <param name="dateTime">対象日付型オブジェクト</param>
        /// <returns>yyyy/MM/dd形式の文字列</returns>
        public static string ToStrYmdSlash(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy/MM/dd");
        }

        /// <summary>
        /// yyyyMMddHHmmss形式の文字列を返します。
        /// </summary>
        /// <param name="dateTime">対象日付型オブジェクト</param>
        /// <returns>yyyyMMddHHmmss形式の文字列</returns>
        public static string ToStrYmdHms(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// HHmm形式の文字列を返します。
        /// </summary>
        /// <param name="dateTime">対象日付型オブジェクト</param>
        /// <returns>HHmm形式の文字列</returns>
        public static string ToStrHm(this DateTime dateTime)
        {
            return dateTime.ToString("HHmm");
        }

        /// <summary>
        /// HH:mm形式の文字列を返します。
        /// </summary>
        /// <param name="dateTime">対象日付型オブジェクト</param>
        /// <returns>HH:mm形式の文字列</returns>
        public static string ToStrHmColon(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm");
        }

        /// <summary>
        /// yyyy/MM/dd HH:mm:ss形式の文字列を返します。
        /// </summary>
        /// <param name="dateTime">対象日付型オブジェクト</param>
        /// <returns>yyyy/MM/dd HH:mm:ss形式の文字列</returns>
        public static string ToStrYmdHmsColon(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy/MM/dd HH:mm:ss");
        }

        /// <summary>
        /// 和暦形式ggyy年M月d日の半角文字列を返します。
        /// <example>例：平成26年1月1日</example>
        /// </summary>
        /// <param name="dateTime">対象日付型オブジェクト</param>
        /// <returns></returns>
        public static string ToJapaneseYmd(this DateTime dateTime)
        {
            CultureInfo culture = new CultureInfo("ja-JP", true);
            culture.DateTimeFormat.Calendar = new JapaneseCalendar();

           return dateTime.ToString("ggyy年M月d日", culture);
        }

        /// <summary>
        /// 和暦形式M月d日の半角文字列を返します。
        /// <example>例：1月1日</example>
        /// </summary>
        /// <param name="dateTime">対象日付型オブジェクト</param>
        /// <returns></returns>
        public static string ToJapaneseMd(this DateTime dateTime)
        {
            CultureInfo culture = new CultureInfo("ja-JP", true);
            culture.DateTimeFormat.Calendar = new JapaneseCalendar();

            return dateTime.ToString("M月d日", culture);
        }

        /// <summary>
        /// 曜日の省略名を返します。
        /// <example>例：日、月</example>
        /// </summary>
        /// <param name="dateTime">対象日付型オブジェクト</param>
        /// <param name="languageCd">言語コード</param>
        /// <returns></returns>
        public static string GetDayName(this DateTime dateTime, string languageCd)
        {
            return dateTime.ToString("ddd", new CultureInfo(languageCd));
        }

        /// <summary>
        /// 和暦形式ggyy年M月d日の全角文字列を返します。
        /// <example>例：平成２６年１月１日</example>
        /// </summary>
        /// <param name="dateTime">対象日付型オブジェクト</param>
        /// <returns></returns>
        public static string ToJapaneseYmdZenkaku(this DateTime dateTime)
        {
            return ToJapaneseYmd(dateTime).ToZenkaku();
        }

        /// <summary>
        /// 和暦形式ggyy年MM月dd日の半角文字列を返します。
        /// <example>例：平成26年01月01日</example>
        /// </summary>
        /// <param name="dateTime">対象日付型オブジェクト</param>
        /// <returns></returns>
        public static string ToJapaneseYmmdd(this DateTime dateTime)
        {
            CultureInfo culture = new CultureInfo("ja-JP", true);
            culture.DateTimeFormat.Calendar = new JapaneseCalendar();

            return dateTime.ToString("ggyy年MM月dd日", culture);
        }

        /// <summary>
        /// 和暦形式ggyy年MM月dd日の全角文字列を返します。
        /// <example>例：平成２６年０１月０１日</example>
        /// </summary>
        /// <param name="dateTime">対象日付型オブジェクト</param>
        /// <returns></returns>
        public static string ToJapaneseYmmddZenkaku(this DateTime dateTime)
        {
            return ToJapaneseYmmdd(dateTime).ToZenkaku();
        }

        /// <summary>
        /// 指定した日付を含むその年の週を返します。
        /// </summary>
        /// <param name="dateTime">対象日付型オブジェクト</param>
        /// <returns>パラメーターの日付を含む年の週を表す 1 から始まる整数。</returns>
        public static int GetWeekOfYear(this DateTime dateTime)
        {
            GregorianCalendar gregorianCalendar = new GregorianCalendar(GregorianCalendarTypes.Localized);
            return gregorianCalendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        /// <summary>
        /// 指定した日付を含むその年の週の初日を返します。
        /// </summary>
        /// <param name="dateTime">対象日付型オブジェクト</param>
        /// <returns>パラメーターの日付を含む年の週の初日。</returns>
        public static DateTime GetFirstDayOfWeek(this DateTime dateTime)
        {
            int during = DayOfWeek.Sunday - dateTime.DayOfWeek;
            return dateTime.AddDays(during);
        }

        /// <summary>
        /// 指定した日付を含むその年の週の最終日を返します。
        /// </summary>
        /// <param name="dateTime">対象日付型オブジェクト</param>
        /// <returns>パラメーターの日付を含む年の週の最終日。</returns>
        public static DateTime GetLastDayOfWeek(this DateTime dateTime)
        {
            int during = DayOfWeek.Saturday - dateTime.DayOfWeek;
            return dateTime.AddDays(during);
        }
    }
}
