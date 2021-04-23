using System;

namespace Blis.Client
{
	[Serializable]
	public class LnSentence
	{
		public string key;


		public string[] value;

		public LnSentence(string key, string[] value)
		{
			this.key = key;
			this.value = value;
		}


		public string this[SupportLanguage key] {
			get
			{
				if (key <= (SupportLanguage) value.Length)
				{
					return value[key - SupportLanguage.Korean];
				}

				return string.Empty;
			}
		}


		public override string ToString()
		{
			return key;
		}
	}
}