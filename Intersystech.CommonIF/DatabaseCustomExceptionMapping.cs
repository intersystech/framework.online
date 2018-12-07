using Intersystech.DataModel;
using Intersystech.ExceptionLibrary;
using Intersystech.Utility;
//using MySql.Data.MySqlClient;
//using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Xml.Schema;

namespace Intersystech.CommonIF
{
    /// <summary>
    /// データベースカスタム例外マッピングクラス
    /// </summary>
    public class DatabaseCustomExceptionMapping : IDatabaseCustomExceptionMapping
    {
        #region 変数
        /// <summary>
        /// トレースログ
        /// </summary>
        //private TraceLogger Logger = new TraceLogger();
        #endregion

        #region プロパティ
        /// <summary>
        /// カスタム例外マッピングが有効か
        /// </summary>
        public bool IsCustomExceptionMappingEnabled
        {
            get
            {
                return ConfigHelper.GetAppSettingsValue("CustomExceptionMappingEnabled") == "1";
            }
        }

        /// <summary>
        /// カスタム例外マッピングパス
        /// </summary>
        private string CustomExceptionMappingPath
        {
            get
            {
                return ConfigHelper.GetAppSettingsValue("CustomExceptionMappingFilePath");
            }
        }

        /// <summary>
        /// データベース種類
        /// </summary>
        private string DatabaseType
        {
            get
            {
                int databaseTypeValue = int.Parse(ConfigHelper.GetAppSettingsValue("DatabaseType"));
                return Enum.GetName(typeof(DatabaseType), databaseTypeValue);
            }
        }
        #endregion

        #region メソッド
        /// <summary>
        /// データベースカスタム例外マッピングファイルの内容を取得します。
        /// </summary>
        /// <returns>データベースカスタム例外マッピングファイルの内容</returns>
        public List<Tuple<int, string, string>> GetKeyValueList()
        {
            List<Tuple<int, string, string>> list = new List<Tuple<int, string, string>>();
            XmlDocument xmlDocument = GetXmlDocument();
            for (int i = 0; i < xmlDocument.GetElementsByTagName("customExceptionMapping").Count; i++)
            {
                XmlNode xmlNode = xmlDocument.GetElementsByTagName("customExceptionMapping")[i];
                int errorNumber = int.Parse(xmlNode.Attributes["errorNumber"].Value);
                string mappedMessage = xmlNode.Attributes["mappedMessage"].Value;
                string databaseType = xmlNode.Attributes["database"].Value;
                string resolveType = xmlNode.Attributes["resolve"].Value;

                if (databaseType == DatabaseType)
                {
                    list.Add(new Tuple<int, string, string>(errorNumber, mappedMessage, resolveType));
                }
            }

            return list;
        }

        /// <summary>
        /// カスタム例外マッピングXMLドキュメントを取得します。
        /// </summary>
        /// <returns>XMLドキュメント</returns>
        private XmlDocument GetXmlDocument()
        {
            Page page = new Page();
            string absolutePath = page.Server.MapPath(CustomExceptionMappingPath);
            string xsdFile = Path.Combine(HttpRuntime.BinDirectory, "DatabaseCustomException.xsd");
            string xmlFile = Path.Combine(absolutePath, "DatabaseCustomException.xml");

            XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
            xmlSchemaSet.Add("http://tempuri.org/DatabaseCustomException.xsd", XmlTextReader.Create(new StreamReader(xsdFile)));

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Schemas.Add(xmlSchemaSet);
            xmlDocument.Load(xmlFile);

            xmlDocument.Validate(ValidationErrorHandler);

            return xmlDocument;
        }

        /// <summary>
        /// スキーマ検証イベント
        /// </summary>
        /// <param name="sender">オブジェクト</param>
        /// <param name="e">ValidationEventHandler に関する詳細情報</param>
        private void ValidationErrorHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
            {
                ExceptionUtility.ThrowCustomSystemException(e.Message);
            }
        }

        /// <summary>
        /// カスタム例外を判定します。
        /// </summary>
        /// <param name="ex">例外オブジェクト</param>
        public void Resolve(Exception ex)
        {
            if (IsCustomExceptionMappingEnabled == false)
                return;

            var customExceptionMappingKeyValueList = GetKeyValueList();
            //if (ex.GetType() == typeof(OracleException))
            //{
            //    var oracleEx = ex as OracleException;
            //    foreach (var item in customExceptionMappingKeyValueList)
            //    {
            //        if (oracleEx.Number == item.Item1)
            //        {
            //            Logger.Write(oracleEx.Message, LogType.Error);
            //            if (item.Item3 == "Throw")
            //            {
            //                ExceptionUtility.Throw(item.Item2);
            //            }
            //            else
            //            {
            //                ExceptionUtility.Alert(item.Item2);
            //            }
            //        }
            //    }
            //}
            //else if (ex.GetType() == typeof(MySqlException))
            //{
            //    var mySqlEx = ex as MySqlException;
            //    Logger.Write(mySqlEx.Number + ":" + mySqlEx.Message, LogType.Error);
            //    foreach (var item in customExceptionMappingKeyValueList)
            //    {
            //        if (mySqlEx.Number == item.Item1)
            //        {
            //            if (item.Item3 == "Throw")
            //            {
            //                ExceptionUtility.Throw(item.Item2);
            //            }
            //            else
            //            {
            //                ExceptionUtility.Alert(item.Item2);
            //            }
            //        }
            //    }
            //}
            //else 
            if(ex.GetType() == typeof(SqlException))
            {
                var sqlEx = ex as SqlException;
                //Logger.Error(sqlEx.Number + ":" + sqlEx.Message);
                foreach (var item in customExceptionMappingKeyValueList)
                {
                    if (sqlEx.Number == item.Item1)
                    {
                        if (item.Item3 == "Throw")
                        {
                            ExceptionUtility.ThrowCustomSystemException(item.Item2);
                        }
                        else
                        {
                            ExceptionUtility.Alert(item.Item2);
                        }
                    }
                }
            }
            else if (ex.GetType() == typeof(OleDbException))
            {
                var oleDbEx = ex as OleDbException;
                //Logger.Error(oleDbEx.ErrorCode + ":" + oleDbEx.Message);
                foreach (var item in customExceptionMappingKeyValueList)
                {
                    if (oleDbEx.ErrorCode == item.Item1)
                    {
                        if (item.Item3 == "Throw")
                        {
                            ExceptionUtility.ThrowCustomSystemException(item.Item2);
                        }
                        else
                        {
                            ExceptionUtility.Alert(item.Item2);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
