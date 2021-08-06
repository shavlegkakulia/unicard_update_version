using System;
using System.Collections.Generic;

//using mscorlib.dll;
//using Mono.


namespace Kuni.Core
{
	public class SecurityHelper
	{
		public SecurityHelper ()
		{
		}

		//		static string CalculateHash (string content)
		//		{
		//			using (MD5 hasher = MD5.Create ()) {
		//				byte[] data = hasher.ComputeHash (Encoding.Default.GetBytes (content));
		//				return Convert.ToBase64String (data);
		//			}
		//		}
		//
		//		private string CalculateHMac (List<string> values)
		//		{
		//			string headers = values.Aggregate ((a, b) => a + b) + PublicKey;
		//
		//			using (var hasher = new HMACSHA256 (Encoding.UTF8.GetBytes (PrivateKey))) {
		//				byte[] data = hasher.ComputeHash (Encoding.UTF8.GetBytes (headers));
		//				return Convert.ToBase64String (data);
		//			}
		//		}
	}
}

