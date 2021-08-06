using System;
using System.Collections.Generic;
using Kuni.Core.Models;

namespace Kuni.Core
{
	public interface ICustomSecurityProvider
	{
		string CalculateHash (string content);

		string CalculateHMac (List<string> values);

		string EncryptStringAES (string plainText, string sharedSecret);

		string DecryptStringAES (string cipherText, string sharedSecret);

		void SaveCredentials (string userID, String username, String password, String sessionId, string fbId = null);

		UserModel GetSecurityParams ();

		UserModel GetCredentials ();

		string GetKey ();
	}
}

