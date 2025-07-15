using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Azuro.Common.Cryptography
{
	/// <summary>
	/// Class contains functionality for encrypting / decrypting as well as hashing of data.
	/// It makes use of Rijndael (256bit key,128bit IV) for symmetric encryption and MD5 for hashing.
	/// 
	/// Use the encryption and decryption methods by supplying a password key for encrypting data that can be reversed 
	/// to plain text by supplying the same password key. These methods use internal key salting for added security but remember 
	/// to store the password key securely and hidden.
	/// 
	/// Use the hashing methods for creating hashed strings from input data. This data cannot be reversed as MD5 hashing 
	/// is a one-way algorithm. The hashing methods can also support custom salting.
	/// </summary>
	public class CryptographicHelper : IDisposable
	{
		/// <summary>
		/// Internal representation of the symmetric algorithm instance used for encryption and decryption.
		/// </summary>
		private SymmetricAlgorithm m_cryptoService;
		private bool m_disposed;

		/// <summary>
		/// Constructs a new instance of the Symmetric algorithm provider.
		/// </summary>
		public CryptographicHelper()
		{
			//	TODO: Make it possible to have multiple encryption strategies
			m_cryptoService = new RijndaelManaged();
			m_cryptoService.Mode = CipherMode.CBC;
		}
		#region IDisposable Members

		/// <summary>
		/// Release resources held by the Symmetric algorithm provider.
		/// </summary>
		public void Dispose()
		{
			if (!m_disposed)
			{
				m_cryptoService.Clear();
				m_cryptoService = null;
				m_disposed = true;
			}
		}

		#endregion

		/// <summary>
		/// Takes a plain text string and returns an Md5 result of the input string.
		/// </summary>
		/// <param name="input">The input string to be hashed.</param>
		/// <returns>An MD5 hashed value created from the input string.</returns>
		public static string ToMD5Hash(string input)
		{
			byte[] data = Encoding.Default.GetBytes(input);
			MD5 md5 = new MD5CryptoServiceProvider();
			md5.ComputeHash(data, 0, data.Length);
			string hash = ToHex(md5.Hash);
			md5.Clear();
			return hash;
		}
		
		/// <summary>
		/// Takes a plain text string and returns an Md5 result of the input string.
		/// </summary>
		/// <param name="input">The input string to be hashed.</param>
		/// <param name="salt">A string salt value for added security.</param>
		/// <returns>An MD5 hashed value created from the input string and salt value.</returns>
		public static string ToMD5Hash(string input, string salt)
		{
			return ToMD5Hash(input + salt);
		}

		/// <summary>
		/// Encrypts an input string with the supplied password.
		/// </summary>
		/// <param name="input">The input string to be encrypted.</param>
		/// <param name="password">The password for decrypting the data.</param>
		/// <returns>An encrypted string.</returns>
		public string Encrypt(string input, string password)
		{
			//get input string and salt into binary
			byte[] bInput = StrToBytes(input);
			byte[] salt = StrToBytes(password.Length.ToString());

			//Derive a key from the password.
			PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);

			//Get encryptor from the secret key bytes
			//Rijndael default key length is 256 bit = 32 bytes
			//Rijndael default IV length is 128bit = 16 bytes
			ICryptoTransform encryptor = m_cryptoService.CreateEncryptor(
				secretKey.GetBytes(32), secretKey.GetBytes(16));

			//Create a crypto stream for processing data, output written to memory stream
			//use write mode for encryption
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

			// Encrypt and Flush
			cryptoStream.Write(bInput, 0, bInput.Length);
			cryptoStream.FlushFinalBlock();

			//get cipher binary
			byte[] CipherBytes = memoryStream.ToArray();

			//close streams
			memoryStream.Close();
			cryptoStream.Close();

			//return base64 encoded string.
			string encrypted = ToBase64(CipherBytes);
			return encrypted;
		}

		/// <summary>
		/// Decrypts an encrypted input string with the supplied password key.
		/// </summary>
		/// <param name="input">The input string to be decrypted.</param>
		/// <param name="password">The password key for decrypting the data.</param>
		/// <returns>An decrypted plain text string.</returns>
		public string Decrypt(string input, string password)
		{
			//get input string and salt into binary
			byte[] bInput = Convert.FromBase64String(input);
			byte[] salt = StrToBytes(password.Length.ToString());

			//Derive a key from the supplied password.
			PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);

			//Get decryptor from the secret key bytes
			//Rijndael default key length is 256 bit = 32 bytes
			//Rijndael default IV length is 128bit = 16 bytes
			ICryptoTransform decryptor = m_cryptoService.CreateDecryptor(
				secretKey.GetBytes(32), secretKey.GetBytes(16));

			//Create a crypto stream for processing data, output written to memory stream
			//use read mode for decryption
			MemoryStream memoryStream = new MemoryStream(bInput);
			CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

			//Allocate buffer for decrypted data, wont be longer than encrypted data
			byte[] PlainText = new byte[bInput.Length];

			//Decrypt
			int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

			//close streams
			memoryStream.Close();
			cryptoStream.Close();

			//return decrypted unicode string
			string decrypted = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
			return decrypted;
		}

		/// <summary>
		/// Helper function to convert Strings to Bytes Array
		/// </summary>
		/// <param name="input">A String you want the Byte Array for</param>
		/// <returns>Byte Array from string</returns>
		public static byte[] StrToBytes(string input)
		{
			return StrToBytes(input, Encoding.Default);
		}

		/// <summary>
		/// Helper function to convert Strings to Bytes Array
		/// </summary>
		/// <param name="input">A String you want the Byte Array for</param>
		/// <returns>Byte Array from string</returns>
		public static byte[] StrToBytes(string input, Encoding encoding)
		{
			return encoding.GetBytes(input);
		}
		
		/// <summary>
		/// Helper function to convert Input to Base64 representation
		/// </summary>
		/// <param name="Input">Byte array to be converted</param>
		/// <returns>Base64 representation of input</returns>
		public static string ToBase64(String Input)
		{
			return ToBase64(StrToBytes(Input));
		}
		
		/// <summary>
		/// Helper function to convert Input to Base64 representation
		/// </summary>
		/// <param name="Input">Byte array to be converted</param>
		/// <returns>Base64 representation of input</returns>
		public static string ToBase64(byte[] Input)
		{
			return Convert.ToBase64String(Input);
		}
		
		/// <summary>
		/// Helper function to convert Input to hexadecimal representation
		/// </summary>
		/// <param name="Input">Byte array to be converted</param>
		/// <returns>Hexadecimal representation of input</returns>
		public static string ToHex(byte[] Input)
		{
			StringBuilder sb = new StringBuilder();
			foreach (byte b in Input)
			{
				sb.Append(Convert.ToString(b, 16).ToUpper().PadLeft(2, '0'));
			}
			return sb.ToString();
		}
		
		/// <summary>
		/// Helper function to convert Input to hexadecimal representation
		/// </summary>
		/// <param name="Input">String to be converted</param>
		/// <returns>Hexadecimal representation of input</returns>
		public static string ToHex(string Input)
		{
			return ToHex(StrToBytes(Input));
		}
		
		/// <summary>
		/// Returns a SHA1 hash of input string
		/// </summary>
		/// <param name="input">Value to be hashed</param>
		/// <returns>A SHA1 hash of the input.</returns>
		public static string ToSHA1Hash(string input)
		{
			SHA1 hash = new SHA1CryptoServiceProvider();
			string result = ToHex(hash.ComputeHash(StrToBytes(input)));
			hash.Clear();
			return result;
		}
		
		/// <summary>
		/// Returns a SHA256 hash of input string
		/// </summary>
		/// <param name="input">Value to be hashed</param>
		/// <returns>A SHA256 hash of the input.</returns>
		public static string ToSHA256Hash(string input)
		{
			SHA256 hash = new SHA256Managed();
			string result = ToHex(hash.ComputeHash(StrToBytes(input)));
			hash.Clear();
			return result;
		}
		
		/// <summary>
		/// Returns a SHA384 hash of input string
		/// </summary>
		/// <param name="input">Value to be hashed</param>
		/// <returns>A SHA384 hash of the input.</returns>
		public static string ToSHA384Hash(string input)
		{
			SHA384 hash = new SHA384Managed();
			string result = ToHex(hash.ComputeHash(StrToBytes(input)));
			hash.Clear();
			return result;
		}
		
		/// <summary>
		/// Returns a SHA512 hash of input string
		/// </summary>
		/// <param name="input">Value to be hashed</param>
		/// <returns>A SHA512 hash of the input.</returns>
		public static string ToSHA512Hash(string input)
		{
			SHA512 hash = new SHA512Managed();
			string result = ToHex(hash.ComputeHash(StrToBytes(input)));
			hash.Clear();
			return result;
		}
		
		/// <summary>
		/// Returns a HMACSHA1 hash of input string using provided key.
		/// </summary>
		/// <param name="input">Value to be hashed</param>
		/// <param name="key">Salt value</param>
		/// <returns>A HMACSHA1 hash of the input.</returns>
		public static string ToHMACSHA1Hash(string input, string key)
		{
			HMACSHA1 hash = new HMACSHA1();
			hash.Key = StrToBytes(key);
			string result = ToHex(hash.ComputeHash(StrToBytes(input)));
			hash.Clear();
			return result;
		}
	}
}
