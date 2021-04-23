namespace Blis.Client
{
	public static class SupportLanguageExtension
	{
		public static SupportLanguage ToSupportLanguage(this string code)
		{
			switch (code)
			{
				case "de":
					return SupportLanguage.German;
				case "en":
					return SupportLanguage.English;
				case "es":
					return SupportLanguage.Spanish;
				case "es_LA":
					return SupportLanguage.SpanishLatin;
				case "fr":
					return SupportLanguage.French;
				case "in":
					return SupportLanguage.Indonesian;
				case "ja":
					return SupportLanguage.Japanese;
				case "ko":
					return SupportLanguage.Korean;
				case "pt":
					return SupportLanguage.Portuguese;
				case "pt_LA":
					return SupportLanguage.PortugueseLatin;
				case "ru":
					return SupportLanguage.Russian;
				case "th":
					return SupportLanguage.Thai;
				case "vi":
					return SupportLanguage.Vietnamese;
				case "zh_CN":
					return SupportLanguage.ChineseSimplified;
				case "zh_TW":
					return SupportLanguage.ChineseTraditional;
				default:
					return SupportLanguage.English;
			}

			// co: dotPeek
			// uint num = <PrivateImplementationDetails>.ComputeStringHash(code);
			// if (num <= 1095059089U)
			// {
			// 	if (num <= 1011465184U)
			// 	{
			// 		if (num != 815439409U)
			// 		{
			// 			if (num != 963731029U)
			// 			{
			// 				if (num == 1011465184U)
			// 				{
			// 					if (code == "vi")
			// 					{
			// 						return SupportLanguage.Vietnamese;
			// 					}
			// 				}
			// 			}
			// 			else if (code == "zh_CN")
			// 			{
			// 				return SupportLanguage.ChineseSimplified;
			// 			}
			// 		}
			// 		else if (code == "zh_TW")
			// 		{
			// 			return SupportLanguage.ChineseTraditional;
			// 		}
			// 	}
			// 	else if (num <= 1092248970U)
			// 	{
			// 		if (num != 1079686353U)
			// 		{
			// 			if (num == 1092248970U)
			// 			{
			// 				if (code == "en")
			// 				{
			// 					return SupportLanguage.English;
			// 				}
			// 			}
			// 		}
			// 		else if (code == "es_LA")
			// 		{
			// 			return SupportLanguage.SpanishLatin;
			// 		}
			// 	}
			// 	else if (num != 1094220446U)
			// 	{
			// 		if (num == 1095059089U)
			// 		{
			// 			if (code == "th")
			// 			{
			// 				return SupportLanguage.Thai;
			// 			}
			// 		}
			// 	}
			// 	else if (code == "in")
			// 	{
			// 		return SupportLanguage.Indonesian;
			// 	}
			// }
			// else if (num <= 1461901041U)
			// {
			// 	if (num <= 1176137065U)
			// 	{
			// 		if (num != 1111292255U)
			// 		{
			// 			if (num == 1176137065U)
			// 			{
			// 				if (code == "es")
			// 				{
			// 					return SupportLanguage.Spanish;
			// 				}
			// 			}
			// 		}
			// 		else if (code == "ko")
			// 		{
			// 			return SupportLanguage.Korean;
			// 		}
			// 	}
			// 	else if (num != 1213488160U)
			// 	{
			// 		if (num == 1461901041U)
			// 		{
			// 			if (code == "fr")
			// 			{
			// 				return SupportLanguage.French;
			// 			}
			// 		}
			// 	}
			// 	else if (code == "ru")
			// 	{
			// 		return SupportLanguage.Russian;
			// 	}
			// }
			// else if (num <= 1545391778U)
			// {
			// 	if (num != 1530832633U)
			// 	{
			// 		if (num == 1545391778U)
			// 		{
			// 			if (code == "de")
			// 			{
			// 				return SupportLanguage.German;
			// 			}
			// 		}
			// 	}
			// 	else if (code == "pt_LA")
			// 	{
			// 		return SupportLanguage.PortugueseLatin;
			// 	}
			// }
			// else if (num != 1565420801U)
			// {
			// 	if (num == 1816099348U)
			// 	{
			// 		if (code == "ja")
			// 		{
			// 			return SupportLanguage.Japanese;
			// 		}
			// 	}
			// }
			// else if (code == "pt")
			// {
			// 	return SupportLanguage.Portuguese;
			// }
			// return SupportLanguage.English;
		}


		public static string GetCurrency(this SupportLanguage language)
		{
			switch (language)
			{
				case SupportLanguage.Korean:
					return "KRW";
				case SupportLanguage.English:
					return "USD";
				case SupportLanguage.Japanese:
					return "JPY";
				case SupportLanguage.ChineseSimplified:
					return "CNY";
				case SupportLanguage.ChineseTraditional:
				case SupportLanguage.SpanishLatin:
				case SupportLanguage.PortugueseLatin:
				case SupportLanguage.Indonesian:
				case SupportLanguage.Russian:
				case SupportLanguage.Thai:
				case SupportLanguage.Vietnamese:
					return "USD";
				case SupportLanguage.French:
				case SupportLanguage.Spanish:
				case SupportLanguage.Portuguese:
				case SupportLanguage.German:
					return "EUR";
				default:
					return "USD";
			}
		}


		public static string GetAppLanguageCode(this SupportLanguage language)
		{
			switch (language)
			{
				case SupportLanguage.Korean:
					return "ko";
				case SupportLanguage.English:
					return "en";
				case SupportLanguage.Japanese:
					return "ja";
				case SupportLanguage.ChineseSimplified:
					return "zh_CN";
				case SupportLanguage.ChineseTraditional:
					return "zh_TW";
				case SupportLanguage.French:
					return "fr";
				case SupportLanguage.Spanish:
					return "es";
				case SupportLanguage.SpanishLatin:
					return "es_LA";
				case SupportLanguage.Portuguese:
					return "pt";
				case SupportLanguage.PortugueseLatin:
					return "pt_LA";
				case SupportLanguage.Indonesian:
					return "in";
				case SupportLanguage.German:
					return "de";
				case SupportLanguage.Russian:
					return "ru";
				case SupportLanguage.Thai:
					return "th";
				case SupportLanguage.Vietnamese:
					return "vi";
				default:
					return "en";
			}
		}


		public static string GetISO639_1(this SupportLanguage language)
		{
			switch (language)
			{
				case SupportLanguage.Korean:
					return "ko";
				case SupportLanguage.English:
					return "en";
				case SupportLanguage.Japanese:
					return "ja";
				case SupportLanguage.ChineseSimplified:
					return "zh-CN";
				case SupportLanguage.ChineseTraditional:
					return "zh-TW";
				case SupportLanguage.French:
					return "fr";
				case SupportLanguage.Spanish:
					return "es";
				case SupportLanguage.SpanishLatin:
					return "es-419";
				case SupportLanguage.Portuguese:
					return "pt";
				case SupportLanguage.PortugueseLatin:
					return "pt-BR";
				case SupportLanguage.Indonesian:
					return "in";
				case SupportLanguage.German:
					return "de";
				case SupportLanguage.Russian:
					return "ru";
				case SupportLanguage.Thai:
					return "th";
				case SupportLanguage.Vietnamese:
					return "vi";
				default:
					return "en";
			}
		}


		public static string GetSupportVoiceLanguageCode(this SupportLanguage language)
		{
			switch (language)
			{
				case SupportLanguage.Korean:
				case SupportLanguage.ChineseSimplified:
				case SupportLanguage.ChineseTraditional:
				case SupportLanguage.Indonesian:
				case SupportLanguage.Thai:
				case SupportLanguage.Vietnamese:
					return "ko";
				case SupportLanguage.English:
					return "en";
				case SupportLanguage.Japanese:
					return "ja";
			}

			return "en";
		}


		public static string GetFontName(this SupportLanguage language)
		{
			if (language == SupportLanguage.Japanese)
			{
				return "FOT-SkipStd-D";
			}

			if (language != SupportLanguage.Russian)
			{
				return "KBIZM";
			}

			return "Roboto-Regular";
		}
	}
}