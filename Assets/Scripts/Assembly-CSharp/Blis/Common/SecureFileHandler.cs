using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Blis.Common
{
	public static class SecureFileHandler
	{
		private static readonly bool enableEncrypt = true;


		private static readonly string sKey = "blackisland!@#";

		public static void WriteEncryptFile(string fileName, string data)
		{
			using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
			{
				using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
				{
					if (!enableEncrypt)
					{
						using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
						{
							streamWriter.Write(data);
							return;
						}
					}

					byte[] bytes = Encoding.ASCII.GetBytes(sKey.Length.ToString());
					PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(sKey, bytes);
					using (ICryptoTransform cryptoTransform =
						rijndaelManaged.CreateEncryptor(passwordDeriveBytes.GetBytes(32),
							passwordDeriveBytes.GetBytes(16)))
					{
						using (CryptoStream cryptoStream =
							new CryptoStream(fileStream, cryptoTransform, CryptoStreamMode.Write))
						{
							using (StreamWriter streamWriter2 = new StreamWriter(cryptoStream, Encoding.UTF8))
							{
								streamWriter2.Write(data);
							}
						}
					}
				}
			}
		}


		public static string ReadEncryptFile(string fileName)
		{
			string result;
			using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
			{
				using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
				{
					if (!enableEncrypt)
					{
						using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
						{
							return streamReader.ReadToEnd();
						}
					}

					byte[] bytes = Encoding.ASCII.GetBytes(sKey.Length.ToString());
					PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(sKey, bytes);
					using (ICryptoTransform cryptoTransform =
						rijndaelManaged.CreateDecryptor(passwordDeriveBytes.GetBytes(32),
							passwordDeriveBytes.GetBytes(16)))
					{
						using (CryptoStream cryptoStream =
							new CryptoStream(fileStream, cryptoTransform, CryptoStreamMode.Read))
						{
							using (StreamReader streamReader2 = new StreamReader(cryptoStream))
							{
								result = streamReader2.ReadToEnd();
							}
						}
					}
				}
			}

			return result;
		}
	}
}