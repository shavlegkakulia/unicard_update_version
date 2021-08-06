using System;
using Kunicardus.Billboards.Core;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.IO;
using Foundation;
using Kunicardus.Billboards.Core.Models;
using Kunicardus.Billboards.Core.Helpers;

namespace iCunOS.BillBoards.Providers.SecurityProvider
{
	public class SecurityProvider : ICustomSecurityProvider
	{
		public string PublicKey { get ; set ; }

		private const string PrivateKey = "UnicardPublicAPIPrivateKey";

		#region ISecurityProvider implementation

		public string CalculateHash (string content)
		{
			using (MD5 hasher = MD5.Create ()) {
				byte[] data = hasher.ComputeHash (Encoding.Default.GetBytes (content));
				return Convert.ToBase64String (data);
			}
		}

		public string CalculateHMac (System.Collections.Generic.List<string> values)
		{
			string headers = values.Aggregate ((a, b) => a + b) + PublicKey;

			using (var hasher = new HMACSHA256 (Encoding.UTF8.GetBytes (PrivateKey))) {
				byte[] data = hasher.ComputeHash (Encoding.UTF8.GetBytes (headers));
				return Convert.ToBase64String (data);
			}
		}

		#endregion

		private static byte[] _salt = Encoding.ASCII.GetBytes ("o6706642kbM7c5");

		/// <summary>
		/// Encrypt the given string using AES.  The string can be decrypted using 
		/// DecryptStringAES().  The sharedSecret parameters must match.
		/// </summary>
		/// <param name="plainText">The text to encrypt.</param>
		/// <param name="sharedSecret">A password used to generate a key for encryption.</param>
		public string EncryptStringAES (string plainText, string sharedSecret)
		{
			if (string.IsNullOrEmpty (plainText))
				throw new ArgumentNullException ("plainText");
			if (string.IsNullOrEmpty (sharedSecret))
				throw new ArgumentNullException ("sharedSecret");

			string outStr = null;                       // Encrypted string to return
			// RijndaelManaged object used to encrypt the data.

			try {
				// generate the key from the shared secret and the salt
				using (Rfc2898DeriveBytes key = new Rfc2898DeriveBytes (sharedSecret, _salt)) {

					// Create a RijndaelManaged object
					using (RijndaelManaged aesAlg = new RijndaelManaged ()) {
						aesAlg.Key = key.GetBytes (aesAlg.KeySize / 8);

						// Create a decryptor to perform the stream transform.
						using (ICryptoTransform encryptor = aesAlg.CreateEncryptor (aesAlg.Key, aesAlg.IV)) {

							// Create the streams used for encryption.
							using (MemoryStream msEncrypt = new MemoryStream ()) {
								// prepend the IV
								msEncrypt.Write (BitConverter.GetBytes (aesAlg.IV.Length), 0, sizeof(int));
								msEncrypt.Write (aesAlg.IV, 0, aesAlg.IV.Length);
								using (CryptoStream csEncrypt = new CryptoStream (msEncrypt, encryptor, CryptoStreamMode.Write)) {
									using (StreamWriter swEncrypt = new StreamWriter (csEncrypt)) {
										//Write all data to the stream.
										swEncrypt.Write (plainText);
									}
								}
								outStr = Convert.ToBase64String (msEncrypt.ToArray ());
							}
						}
					}
				}
			} finally {

			}

			// Return the encrypted bytes from the memory stream.
			return outStr;
		}

		/// <summary>
		/// Decrypt the given string.  Assumes the string was encrypted using 
		/// EncryptStringAES(), using an identical sharedSecret.
		/// </summary>
		/// <param name="cipherText">The text to decrypt.</param>
		/// <param name="sharedSecret">A password used to generate a key for decryption.</param>
		public string DecryptStringAES (string cipherText, string sharedSecret)
		{
			if (string.IsNullOrEmpty (cipherText))
				throw new ArgumentNullException ("cipherText");
			if (string.IsNullOrEmpty (sharedSecret))
				throw new ArgumentNullException ("sharedSecret");

			// Declare the RijndaelManaged object
			// used to decrypt the data.
			//RijndaelManaged aesAlg = null;

			// Declare the string used to hold
			// the decrypted text.
			string plaintext = null;

			try {
				// generate the key from the shared secret and the salt

				using (Rfc2898DeriveBytes key = new Rfc2898DeriveBytes (sharedSecret, _salt)) {								
					// Create the streams used for decryption.                
					byte[] bytes = Convert.FromBase64String (cipherText);
					using (MemoryStream msDecrypt = new MemoryStream (bytes)) {
						// Create a RijndaelManaged object
						// with the specified key and IV.
						using (RijndaelManaged aesAlg = new RijndaelManaged ()) {
							aesAlg.Key = key.GetBytes (aesAlg.KeySize / 8);
							// Get the initialization vector from the encrypted stream
							aesAlg.IV = ReadByteArray (msDecrypt);
							// Create a decrytor to perform the stream transform.
							using (ICryptoTransform decryptor = aesAlg.CreateDecryptor (aesAlg.Key, aesAlg.IV)) {
								using (CryptoStream csDecrypt = new CryptoStream (msDecrypt, decryptor, CryptoStreamMode.Read)) {
									using (StreamReader srDecrypt = new StreamReader (csDecrypt)) {
										// Read the decrypted bytes from the decrypting stream
										// and place them in a string.
										plaintext = srDecrypt.ReadToEnd ();
									}
								}
							}
						}
					}
				}
			} finally {
				// Clear the RijndaelManaged object.

			}

			return plaintext;
		}

		private byte[] ReadByteArray (Stream s)
		{
			byte[] rawLength = new byte[sizeof(int)];
			if (s.Read (rawLength, 0, rawLength.Length) != rawLength.Length) {
				throw new SystemException ("Stream did not contain properly formatted byte array");
			}

			byte[] buffer = new byte[BitConverter.ToInt32 (rawLength, 0)];
			if (s.Read (buffer, 0, buffer.Length) != buffer.Length) {
				throw new SystemException ("Did not read byte array properly");
			}

			return buffer;
		}

		public static String Username {
			get { 
				string value = NSUserDefaults.StandardUserDefaults.StringForKey ("username"); 
				if (value == null)
					return "";
				else
					return value;
			}
			set {
				NSUserDefaults.StandardUserDefaults.SetString (value ?? "", "username"); 
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			}
		}

		public static String UserId {
			get { 
				string value = NSUserDefaults.StandardUserDefaults.StringForKey ("UserId"); 
				if (value == null)
					return "";
				else
					return value;
			}
			set {
				NSUserDefaults.StandardUserDefaults.SetString (value ?? "", "UserId"); 
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			}
		}

		public static String Password {
			get { 
				string value = NSUserDefaults.StandardUserDefaults.StringForKey ("password"); 
				if (value == null)
					return "";
				else
					return value;
			}
			set {
				NSUserDefaults.StandardUserDefaults.SetString (value ?? "", "password"); 
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			}
		}

		public static String FacebookId {
			get { 
				string value = NSUserDefaults.StandardUserDefaults.StringForKey ("FacebookId"); 
				if (value == null)
					return "";
				else
					return value;
			}
			set {
				NSUserDefaults.StandardUserDefaults.SetString ((value ?? "").ToString (), "FacebookId"); 
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			}
		}

		public static String SessionId {
			get { 
				string value = NSUserDefaults.StandardUserDefaults.StringForKey ("sessionid"); 
				if (value == null)
					return "";
				else
					return value;
			}
			set {
				NSUserDefaults.StandardUserDefaults.SetString (value ?? "", "sessionid"); 
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			}
		}

		public 	UserModel GetCredentials ()
		{
			UserModel model = new UserModel {				
				UserName = Username,
				Password = DecryptStringAES (Password, Constants.SecurityKey),			
				FbToken = FacebookId
			};
			return model;
		}

		public void SaveCredentials (string userID, string username, string password, string sessionId, string fbId = null)
		{
			if (!string.IsNullOrWhiteSpace (username)) {
				Username = username;
				Password = string.IsNullOrWhiteSpace (password) ? "" : EncryptStringAES (password, Constants.SecurityKey);
				SessionId = sessionId;
				UserId = userID;
				FacebookId = fbId;
			}
		}

		public UserModel GetSecurityParams ()
		{
			try {
				UserModel model = new UserModel {
					SessionId = SessionId,
					UserId = UserId
				};
				return model;	
			} catch {
				return null;
			}
		}

		public string GetKey ()
		{
			return PublicKey;
		}
	}
}

