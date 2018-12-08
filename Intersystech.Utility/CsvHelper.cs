using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.VisualBasic.FileIO;

namespace Intersystech.Utility
{
    /// <summary>
    /// CSVファイルの出力及び読込機能を提供するユーティリティクラスです。
    /// </summary>
    public sealed class CsvHelper
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private CsvHelper() { }

        /// <summary>
        /// データテーブルのデータをCsvファイルへ出力します。
        /// </summary>
        /// <param name="dataTable">出力用データを格納するデータテーブル</param>
        /// <param name="filePath">CSVファイル名(絶対パスを含む)</param>
        /// <param name="writeHeader">データテーブルのヘッダーを出力するかを示す値</param>
        /// <param name="encodingName">文字エンコーディング</param>
        public static void Export(DataTable dataTable, string filePath, bool writeHeader = true, string encodingName = "Shift_JIS")
        {
            // CSV内容
            string csvContent = Load(dataTable, writeHeader);

            //CSVファイルに書き込むときに使うEncoding
            Encoding encoding = Encoding.GetEncoding(encodingName);

            //書き込むファイルを開く
            using (StreamWriter sr = new StreamWriter(filePath, false, encoding))
            {
                sr.Write(csvContent);
            }
        }

        /// <summary>
        /// データテーブルのデータをCSV形式でクライアント側へダウンロードします。
        /// </summary>
        /// <param name="dataTable">出力データを格納するデータテーブル</param>
        /// <param name="fileName">CSVファイル名(パスを含まない)</param>
        /// <param name="writeHeader">データテーブルのヘッダーを出力するかを示す値</param>
        /// <param name="encodingName">文字エンコーディング</param>
        public static void Download(DataTable dataTable, string fileName, bool writeHeader = true, string encodingName = "Shift_JIS")
        {
            // CSV内容
            string csvContent = Load(dataTable, writeHeader);

            //-----------------------------------------------------------------------------------------
            // ダウンロード処理
            //-----------------------------------------------------------------------------------------
            // ダウンロードダイアログのファイル名
            string downloadFileName = HttpUtility.UrlEncode(fileName);
            // Response情報クリア
            HttpContext.Current.Response.ClearContent();
            // バッファリング
            HttpContext.Current.Response.Buffer = true;
            // HTTPヘッダー情報設定
            Encoding encoding = Encoding.GetEncoding(encodingName);
            HttpContext.Current.Response.ContentEncoding = encoding;
            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", downloadFileName));
            HttpContext.Current.Response.ContentType = "text/comma-separated-values";
            // ファイル書込
            HttpContext.Current.Response.Write(csvContent);
            // フラッシュ
            HttpContext.Current.Response.Flush();
            // レスポンス終了
            HttpContext.Current.Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// データテーブルを読込み、Csv出力用文字列を返します。
        /// </summary>
        /// <param name="dataTable">出力データを格納するデータテーブル</param>
        /// <param name="writeHeader">データテーブルのヘッダーを出力するかを示す値</param>
        /// <returns>Csvファイル内容を表す文字列</returns>
        private static string Load(DataTable dataTable, bool writeHeader)
        {
            StringBuilder sb = new StringBuilder();

            //ヘッダを書き込む
            if (writeHeader)
            {
                IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>().
                                                  Select(column => EncloseDoubleQuotes(column.ColumnName));
                sb.AppendLine(string.Join(",", columnNames));
            }

            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => EncloseDoubleQuotes(field.ToString()));
                sb.AppendLine(string.Join(",", fields));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 文字列をダブルクォートで囲む
        /// </summary>
        /// <param name="field">文字列</param>
        /// <returns>ダブルクォートで囲んだ文字列</returns>
        private static string EncloseDoubleQuotes(string field)
        {
            if (field.IndexOf('"') > -1)
            {
                //"を""とする
                field = field.Replace("\"", "\"\"");
            }
            return "\"" + field + "\"";
        }

        /// <summary>
        /// CSVファイルを読込み、型指定されたデータセットを返します。
        /// </summary>
        /// <typeparam name="T">型指定データセット</typeparam>
        /// <param name="filePath">CSVファイル名(パスを含む)</param>
        /// <param name="isFirstRowHeader">先頭行をヘッダーにするか</param>
        /// <param name="encodingName">文字エンコーディング</param>
        /// <param name="delimiter">区切り文字</param>
        /// <returns>CSVファイル内容を格納したデータセット</returns>
        public static T ReadToDataSet<T>(string filePath, bool isFirstRowHeader = true, string encodingName = "Shift_JIS", string delimiter = ",") where T : DataSet
        {
            using (FileStream csvFileStream = new FileStream(filePath, FileMode.Open))
            {
                return ReadToDataSet<T>(csvFileStream, isFirstRowHeader, encodingName, delimiter);
            }
        }

        /// <summary>
        /// CSVファイルストリームを読込み、型指定されたデータセットを返します。
        /// </summary>
        /// <typeparam name="T">型指定データセット</typeparam>
        /// <param name="csvFileStream">CSVファイルストリーム</param>
        /// <param name="isFirstRowHeader">先頭行をヘッダーにするか</param>
        /// <param name="encodingName">文字エンコーディング</param>
        /// <param name="delimiter">区切り文字</param>
        /// <returns>CSVファイル内容を格納したデータセット</returns>
        public static T ReadToDataSet<T>(Stream csvFileStream, bool isFirstRowHeader = true, string encodingName = "Shift_JIS", string delimiter = ",") where T : DataSet
        {
            T typedDataSet = Activator.CreateInstance<T>();
            typedDataSet.Tables[0].Clear();

            using (DataSet dataSet = new DataSet())
            {
                dataSet.Merge(ReadToDataSet(csvFileStream, isFirstRowHeader, encodingName, delimiter));

                foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                {
                    typedDataSet.Tables[0].LoadDataRow(dataRow.ItemArray, true);
                }
            }
            typedDataSet.AcceptChanges();

            return typedDataSet;
        }

        /// <summary>
        /// CSVファイルを読込み、汎用型のデータセットを返します。
        /// </summary>
        /// <param name="filePath">CSVファイル名(パスを含む)</param>
        /// <param name="isFirstRowHeader">先頭行をヘッダーにするか</param>
        /// <param name="encodingName">文字エンコーディング</param>
        /// <param name="delimiter">区切り文字</param>
        /// <returns>CSVファイル内容を格納したデータセット</returns>
        public static DataSet ReadToDataSet(string filePath, bool isFirstRowHeader = true, string encodingName = "Shift_JIS", string delimiter = ",")
        {
            using (FileStream csvFileStream = new FileStream(filePath, FileMode.Open))
            {
                return ReadToDataSet(csvFileStream, isFirstRowHeader, encodingName, delimiter);
            }
        }

        /// <summary>
        /// CSVファイルストリームを読込み、汎用型のデータセットを返します。
        /// </summary>
        /// <param name="csvFileStream">CSVファイルストリーム</param>
        /// <param name="isFirstRowHeader">先頭行をヘッダーにするか</param>
        /// <param name="encodingName">文字エンコーディング</param>
        /// <param name="delimiter">区切り文字</param>
        /// <returns>CSVファイル内容を格納したデータセット</returns>
        public static DataSet ReadToDataSet(Stream csvFileStream, bool isFirstRowHeader = true, string encodingName = "Shift_JIS", string delimiter = ",")
        {
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add("CsvTable");
            DataTable dataTable = dataSet.Tables[0];

            //Shift JISで読み込む
            using (TextFieldParser tfp = new TextFieldParser(csvFileStream, Encoding.GetEncoding(encodingName)))
            {
                //フィールドが文字で区切られているとする
                //デフォルトでDelimitedなので、必要なし
                tfp.TextFieldType = FieldType.Delimited;
                //区切り文字を,とする
                tfp.Delimiters = new string[] { delimiter };
                //フィールドを"で囲み、改行文字、区切り文字を含めることができるか
                //デフォルトでtrueなので、必要なし
                tfp.HasFieldsEnclosedInQuotes = true;
                //フィールドの前後からスペースを削除する
                //デフォルトがtrue
                tfp.TrimWhiteSpace = false;

                int rowIndex = 0;
                while (!tfp.EndOfData)
                {
                    //フィールドを読み込む
                    string[] fields = tfp.ReadFields();
                    if (isFirstRowHeader == true)
                    {
                        if (rowIndex == 0)
                        {
                            foreach (string columnName in fields)
                            {
                                dataTable.Columns.Add(columnName);
                            }
                        }
                        else
                        {
                            var newRow = dataTable.NewRow();
                            newRow.ItemArray = fields;
                            dataTable.Rows.Add(newRow);
                        }
                    }
                    else
                    {
                        if (rowIndex == 0)
                        {
                            for (int i = 0, counter = fields.Length; i < counter; i++)
                            {
                                string columnName = string.Concat("Column", i + 1);
                                dataTable.Columns.Add(columnName);
                            }
                        }

                        var newRow = dataTable.NewRow();
                        newRow.ItemArray = fields;
                        dataTable.Rows.Add(newRow);
                    }

                    rowIndex++;
                }
            }
            dataTable.AcceptChanges();

            return dataSet;
        }
    }
}
