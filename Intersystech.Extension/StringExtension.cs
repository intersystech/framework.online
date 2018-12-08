using System;
using Microsoft.VisualBasic;
using System.Text;

namespace Intersystech.Extension
{
    /// <summary>
    /// Stringの拡張クラス
    /// </summary>
    public static class StringExtension
    {
        private const string whiteSpace = " ";
        private const string whiteSpaceZenkaku = "　";

        /// <summary>
        /// 半角スペース
        /// </summary>
        public static string WhiteSpace
        {
            get
            {
                return whiteSpace;
            }
        }

        /// <summary>
        /// 全角スペース
        /// </summary>
        public static string WhiteSpaceZenkaku
        {
            get
            {
                return whiteSpaceZenkaku;
            }
        }

        /// <summary>
        /// 半角フォーマットに変換された文字列を返します。
        /// </summary>
        /// <param name="str">対象文字列</param>
        /// <returns>半角文字列</returns>
        public static string ToHankaku(this string str)
        {
            return Strings.StrConv(str, VbStrConv.Narrow, 0);
        }

        /// <summary>
        /// 全角フォーマットに変換された文字列を返します。
        /// </summary>
        /// <param name="str">対象文字列</param>
        /// <returns>全角文字列</returns>
        public static string ToZenkaku(this string str)
        {
            return Strings.StrConv(str, VbStrConv.Wide, 0);
        }

        /// <summary>
        /// 対象文字列を日付型またはnullに変換します。
        /// </summary>
        /// <param name="str">対象文字列</param>
        /// <returns>日付型またはnull</returns>
        public static DateTime? ToDateTimeOrNull(this string str)
        {
            DateTime result = new DateTime();
            if (DateTime.TryParse(str, out result) == true)
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 対象文字列を日付型に変換します。変換できない場合、FormatExceptionをThrowします。
        /// </summary>
        /// <param name="str">対象文字列</param>
        /// <returns>日付型</returns>
        public static DateTime ToDateTime(this string str)
        {
            DateTime result = new DateTime();
            if (DateTime.TryParse(str, out result) == true)
            {
                return result;
            }
            throw new FormatException(string.Format("{0}を日付型へ変換できません。", str));
        }

        /// <summary>
        /// 対象文字列を列挙型に変換します。
        /// </summary>
        /// <typeparam name="T">変換先Type</typeparam>
        /// <param name="str">対象文字列</param>
        /// <returns>列挙型</returns>
        public static T ToEnumValue<T>(this string str)
        {
            return (T)Enum.Parse(typeof(T), str);
        }

        /// <summary>
        /// ['] を [''] に変換します。
        /// </summary>
        /// <param name="str">対象文字列</param>
        /// <returns>('')変換済みの文字列</returns>
        public static string ToSafeValue(this string str)
        {
            return string.IsNullOrEmpty(str) == true ? 
                string.Empty : str.Replace("'", "''");
        }

        /// <summary>
        /// 対象文字列をエンコードすることによって生成されるバイト数を計算します。
        /// </summary>
        /// <param name="str">対象文字列</param>
        /// <param name="codepage">使用するエンコーディングのコード ページ ID</param>
        /// <returns>対象文字列をエンコードすることによって生成されるバイト数</returns>
        public static int GetByteCount(this string str, int codepage)
        {
            Encoding encoding = Encoding.GetEncoding(codepage);
            return encoding.GetByteCount(str);
        }

        /// <summary>
        /// 対象文字列をエンコードすることによって生成されるバイト数を計算します。
        /// </summary>
        /// <param name="str">対象文字列</param>
        /// <param name="encodingName">エンコーディング名</param>
        /// <returns>対象文字列をエンコードすることによって生成されるバイト数</returns>
        public static int GetByteCount(this string str, string encodingName)
        {
            Encoding encoding = Encoding.GetEncoding(encodingName);
            return encoding.GetByteCount(str);
        }

        /// <summary>
        /// 対象文字列をShift_JISエンコードで生成されるバイト数を計算します。
        /// </summary>
        /// <param name="str">対象文字列</param>
        /// <returns>対象文字列をShift_JISエンコードで生成されるバイト数</returns>
        public static int GetByteCountShiftJIS(this string str)
        {
            int codePage = Encoding.GetEncoding("Shift_JIS").CodePage;
            return GetByteCount(str, codePage);
        }

        /// <summary>
        /// 対象文字列をUTF8エンコードで生成されるバイト数を計算します。
        /// </summary>
        /// <param name="str">対象文字列</param>
        /// <returns>対象文字列をUTF8エンコードで生成されるバイト数</returns>
        public static int GetByteCountUTF8(this string str)
        {
            int codePage = Encoding.UTF8.CodePage;
            return GetByteCount(str, codePage);
        }
    }
}
