using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Kuni.Core.UnicardApiProvider
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
		Task<TResultObject> Post<TResultObject> (string url, Dictionary<string, string> headers, string body) where TResultObject : UnicardApiBaseResponse;

		Task<TResultObject> Post<TResultObject> (string url, string body) where TResultObject : UnicardApiBaseResponse;

	}
}

