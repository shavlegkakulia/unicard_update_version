using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Kunicardus.Billboards.Core.Services.UnicardApiProvider;
using Kunicardus.Billboards.Core.Models;

namespace Kunicardus.Billboards.Core.UnicardApiProvider
{
	public interface IUnicardApiProvider
	{
		/// <summary>
		/// Post a body 
		/// </summary>
		/// <typeparam name="TResultObject"> The expected result type, Must be Json serializable</typeparam>
		/// <param name="url"></param>
		/// <param name="headers"></param>
		/// <param name="body"></param>  
		/// <returns></returns>
		TResultObject Post<TResultObject> (string url, Dictionary<string, string> headers, string body) where TResultObject : UnicardApiBaseResponse;

		BillboardsBaseResponse<TResultObject> PostToApi<TResultObject> (string url, Dictionary<string, string> headers, string body) where TResultObject : class, new();

		BillboardsBaseResponse<TResultObject> GetFromApi<TResultObject> (string url, Dictionary<string, string> headers) where TResultObject : class, new();

		TResultObject UnsecuredPost<TResultObject> (string url, Dictionary<string, string> headers, string body) where TResultObject : UnicardApiBaseResponse;

		UserModel UpdateSession ();
	}
}

