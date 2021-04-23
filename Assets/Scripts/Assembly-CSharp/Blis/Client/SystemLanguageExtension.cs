using UnityEngine;

namespace Blis.Client
{
	public static class SystemLanguageExtension
	{
		public static SupportLanguage ToSupportLanguage(this SystemLanguage systemLang)
		{
			if (systemLang <= SystemLanguage.German)
			{
				if (systemLang == SystemLanguage.English)
				{
					return SupportLanguage.English;
				}

				if (systemLang == SystemLanguage.French)
				{
					return SupportLanguage.French;
				}

				if (systemLang == SystemLanguage.German)
				{
					return SupportLanguage.German;
				}
			}
			else if (systemLang <= SystemLanguage.Portuguese)
			{
				switch (systemLang)
				{
					case SystemLanguage.Indonesian:
						return SupportLanguage.Indonesian;
					case SystemLanguage.Italian:
						break;
					case SystemLanguage.Japanese:
						return SupportLanguage.Japanese;
					case SystemLanguage.Korean:
						return SupportLanguage.Korean;
					default:
						if (systemLang == SystemLanguage.Portuguese)
						{
							return SupportLanguage.Portuguese;
						}

						break;
				}
			}
			else
			{
				if (systemLang == SystemLanguage.Russian)
				{
					return SupportLanguage.Russian;
				}

				switch (systemLang)
				{
					case SystemLanguage.Spanish:
						return SupportLanguage.Spanish;
					case SystemLanguage.Thai:
						return SupportLanguage.Thai;
					case SystemLanguage.Vietnamese:
						return SupportLanguage.Vietnamese;
					case SystemLanguage.ChineseSimplified:
						return SupportLanguage.ChineseSimplified;
					case SystemLanguage.ChineseTraditional:
						return SupportLanguage.ChineseTraditional;
				}
			}

			return SupportLanguage.English;
		}
	}
}