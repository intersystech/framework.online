using System.Collections.Generic;
using System;
namespace Intersystech.CommonIF
{
	/// <summary>
	/// データベースカスタム例外マッピングインターフェース
	/// </summary>
	public interface IDatabaseCustomExceptionMapping
	{
		/// <summary>
		/// カスタム例外マッピングが有効か
		/// </summary>
		bool IsCustomExceptionMappingEnabled { get; }

		/// <summary>
		/// データベースカスタム例外マッピングファイルの内容を取得します。
		/// </summary>
		/// <returns>データベースカスタム例外マッピングファイルの内容</returns>
		List<Tuple<int, string, string>> GetKeyValueList();

		/// <summary>
		/// カスタム例外を判定します。
		/// </summary>
		/// <param name="ex">例外オブジェクト</param>
		void Resolve(Exception ex);
	}
}
