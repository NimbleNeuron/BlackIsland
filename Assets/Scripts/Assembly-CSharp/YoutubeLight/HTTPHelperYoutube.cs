using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

namespace YoutubeLight
{
	
	internal static class HTTPHelperYoutube
	{
		
		public static string HtmlDecode(string value)
		{
			return value.DecodeHtmlChars();
		}

		
		public static string DecodeHtmlChars(this string source)
		{
			string[] array = source.Split(new string[]
			{
				"&#x"
			}, StringSplitOptions.None);
			for (int i = 1; i < array.Length; i++)
			{
				int num = array[i].IndexOf(';');
				string value = array[i].Substring(0, num);
				try
				{
					int num2 = Convert.ToInt32(value, 16);
					array[i] = ((char)num2).ToString() + array[i].Substring(num + 1);
				}
				catch
				{
				}
			}
			return string.Join("", array);
		}

		
		public static IDictionary<string, string> ParseQueryString(string s)
		{
			if (s.Contains("?"))
			{
				s = s.Substring(s.IndexOf('?') + 1);
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] array = Regex.Split(s, "&");
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = Regex.Split(array[i], "=");
				string key = array2[0];
				string value = string.Empty;
				if (array2.Length == 2)
				{
					value = array2[1];
				}
				else if (array2.Length > 2)
				{
					value = string.Join("=", array2.Skip(1).Take(array2.Length).ToArray<string>());
				}
				dictionary.Add(key, value);
			}
			return dictionary;
		}

		
		public static string ReplaceQueryStringParameter(string currentPageUrl, string paramToReplace, string newValue, string lsig)
		{
			IDictionary<string, string> dictionary = HTTPHelperYoutube.ParseQueryString(currentPageUrl);
			dictionary[paramToReplace] = newValue;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				if (!flag)
				{
					stringBuilder.Append("&");
				}
				if (keyValuePair.Key == "lsig")
				{
					if (keyValuePair.Value == "" || keyValuePair.Value == string.Empty)
					{
						stringBuilder.Append(keyValuePair.Key);
						stringBuilder.Append("=");
						stringBuilder.Append(lsig);
					}
					else
					{
						stringBuilder.Append(keyValuePair.Key);
						stringBuilder.Append("=");
						stringBuilder.Append(keyValuePair.Value);
					}
				}
				else
				{
					stringBuilder.Append(keyValuePair.Key);
					stringBuilder.Append("=");
					stringBuilder.Append(keyValuePair.Value);
				}
				flag = false;
			}
			return new UriBuilder(currentPageUrl)
			{
				Query = stringBuilder.ToString()
			}.ToString();
		}

		
		public static string UrlDecode(string url)
		{
			return UnityWebRequest.UnEscapeURL(url);
		}
	}
}
