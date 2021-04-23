using System.Collections.Generic;

namespace Blis.Client
{
	public class LnData
	{
		private readonly Dictionary<string, string> data;


		public LnData(SupportLanguage lang)
		{
			data = new Dictionary<string, string>();
			Lang = lang;
		}


		public SupportLanguage Lang { get; }


		public void Marge(string key, string value)
		{
			if (data.ContainsKey(key))
			{
				data[key] = value;
				return;
			}

			data.Add(key, value);
		}


		public string GetString(string key)
		{
			if (data.ContainsKey(key))
			{
				return data[key];
			}

			return null;
		}
	}
}