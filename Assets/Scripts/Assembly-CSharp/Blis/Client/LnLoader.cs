using System.Linq;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class LnLoader : SingletonMonoBehaviour<LnLoader>
	{
		private const string L10N_BASE_PATH = "LocalizationDB/LocalizationDB";

		public void LoadDefaultLangaugeData()
		{
			LnDB lnDB = Resources.Load<LnDB>("LocalizationDB/LocalizationDB");
			LnData lnData = new LnData(SupportLanguage.English);
			foreach (LnSentence lnSentence in lnDB.rawDataList)
			{
				lnData.Marge(lnSentence.key, lnSentence[SupportLanguage.English]);
			}

			Ln.DefaultLoad(lnData);
		}


		public void LoadData()
		{
			string supportLanguage = Singleton<LocalSetting>.inst.setting.supportLanguage;
			if (string.IsNullOrEmpty(supportLanguage))
			{
				LoadData(Application.systemLanguage.ToSupportLanguage());
				return;
			}

			LoadData(ParseLangCode(supportLanguage));
		}


		public void LoadData(SupportLanguage supportLanguage)
		{
			LnDB lnDB = Resources.Load<LnDB>("LocalizationDB/LocalizationDB");
			LnData lnData = new LnData(supportLanguage);
			foreach (LnSentence lnSentence in lnDB.rawDataList)
			{
				lnData.Marge(lnSentence.key, lnSentence[supportLanguage]);
			}

			Ln.Load(lnData);
			foreach (ILnEventHander lnEventHander in FindObjectsOfType<MonoBehaviour>().OfType<ILnEventHander>())
			{
				lnEventHander.OnLnDataChange();
			}
		}


		public SupportLanguage ParseLangCode(string code)
		{
			switch (code)
			{
				case "de":
					return SupportLanguage.German;
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
			// 				if (!(code == "en"))
			// 				{
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
	}
}