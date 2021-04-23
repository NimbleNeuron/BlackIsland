using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blis.Client
{
	public static class Ln
	{
		private const string ErrPrefix = "LnErr";
		private static Dictionary<LnType, string> lnTypePath = null;
		private static LnData lnData;
		private static LnData defaultLnData;
		private static readonly object[] formatParam_1 = new object[1];
		private static readonly object[] formatParam_2 = new object[2];
		private static readonly object[] formatParam_3 = new object[3];
		private static readonly object[] formatParam_4 = new object[4];
		private static readonly object[] formatParam_5 = new object[5];
		private static readonly object[] formatParam_6 = new object[6];

		public static bool IsLoaded => lnData != null;
		public static bool DefaultIsLoaded => defaultLnData != null;
		public static void DefaultLoad(LnData defaulLnData)
		{
			defaultLnData = defaulLnData;
		}

		public static void Load(LnData lnData)
		{
			Ln.lnData = lnData;
		}

		public static SupportLanguage GetCurrentLanguage()
		{
			if (lnData == null)
			{
				throw new NullReferenceException("[Ln] LnData Is Null");
			}

			return lnData.Lang;
		}

		public static string Get(LnType lnType, string key)
		{
			if (lnTypePath == null)
			{
				lnTypePath = new Dictionary<LnType, string>(SingletonComparerEnum<LnTypeComparer, LnType>.Instance);
				foreach (LnType key2 in (LnType[]) Enum.GetValues(typeof(LnType)))
				{
					string value = key2.ToString().Replace("_", "/") + "/";
					lnTypePath.Add(key2, value);
				}
			}

			return Get(lnTypePath[lnType] + key);
		}


		public static string Get(string key)
		{
			if (!IsLoaded)
			{
				return string.Empty;
			}

			string @string = lnData.GetString(key);
			if (@string != null && !string.IsNullOrEmpty(@string))
			{
				return @string;
			}

			bool defaultIsLoaded = DefaultIsLoaded;
			string string2 = defaultLnData.GetString(key);
			if (string2 != null && !string.IsNullOrEmpty(string2))
			{
				return string2;
			}

			if (Debug.isDebugBuild)
			{
				return "LnErr[" + key + "]";
			}

			return string.Empty;
		}


		public static string Format(string key, object[] param_0)
		{
			return FormatInternal(key, param_0);
		}


		public static string Format(string key, string param_0)
		{
			formatParam_1[0] = param_0;
			return FormatInternal(key, formatParam_1);
		}


		public static string Format(string key, int param_0)
		{
			return Format(key, param_0.ToString());
		}


		public static string Format(string key, long param_0)
		{
			return Format(key, param_0.ToString());
		}


		public static string Format(string key, float param_0)
		{
			return Format(key, param_0.ToString("0.00"));
		}


		public static string Format(string key, string param_0, string param_1)
		{
			formatParam_2[0] = param_0;
			formatParam_2[1] = param_1;
			return FormatInternal(key, formatParam_2);
		}


		public static string Format(string key, int param_0, string param_1)
		{
			return Format(key, param_0.ToString(), param_1);
		}


		public static string Format(string key, int param_0, int param_1)
		{
			return Format(key, param_0.ToString(), param_1.ToString());
		}


		public static string Format(string key, string param_0, string param_1, string param_2)
		{
			formatParam_3[0] = param_0;
			formatParam_3[1] = param_1;
			formatParam_3[2] = param_2;
			return FormatInternal(key, formatParam_3);
		}


		public static string Format(string key, int param_0, int param_1, int param_2)
		{
			return Format(key, param_0.ToString(), param_1.ToString(), param_2.ToString());
		}


		public static string Format(string key, string param_0, string param_1, string param_2, string param_3)
		{
			formatParam_4[0] = param_0;
			formatParam_4[1] = param_1;
			formatParam_4[2] = param_2;
			formatParam_4[3] = param_3;
			return FormatInternal(key, formatParam_4);
		}


		public static string Format(string key, int param_0, int param_1, int param_2, int param_3)
		{
			return Format(key, param_0.ToString(), param_1.ToString(), param_2.ToString(), param_3.ToString());
		}


		public static string Format(string key, string param_0, string param_1, string param_2, string param_3,
			string param_4)
		{
			formatParam_5[0] = param_0;
			formatParam_5[1] = param_1;
			formatParam_5[2] = param_2;
			formatParam_5[3] = param_3;
			formatParam_5[4] = param_4;
			return FormatInternal(key, formatParam_5);
		}


		public static string Format(string key, int param_0, int param_1, int param_2, int param_3, int param_4)
		{
			return Format(key, param_0.ToString(), param_1.ToString(), param_2.ToString(), param_3.ToString(),
				param_4.ToString());
		}


		public static string Format(string key, string param_0, string param_1, string param_2, string param_3,
			string param_4, string param_5)
		{
			formatParam_6[0] = param_0;
			formatParam_6[1] = param_1;
			formatParam_6[2] = param_2;
			formatParam_6[3] = param_3;
			formatParam_6[4] = param_4;
			formatParam_6[5] = param_5;
			return FormatInternal(key, formatParam_6);
		}


		public static string Format(string key, int param_0, int param_1, int param_2, int param_3, int param_4,
			int param_5)
		{
			return Format(key, param_0.ToString(), param_1.ToString(), param_2.ToString(), param_3.ToString(),
				param_4.ToString(), param_5.ToString());
		}


		private static string FormatInternal(string key, params object[] paramArray)
		{
			string text = Get(key);
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}

			if (Debug.isDebugBuild && text.Contains("LnErr"))
			{
				return text;
			}

			string text2 = string.Empty;
			try
			{
				text2 = string.Format(text, paramArray);
			}
			catch (Exception) { }

			if (lnData.Lang == SupportLanguage.Korean && !string.IsNullOrEmpty(text2))
			{
				text2 = Korean.ReplaceJosa(text2);
			}

			return text2;
		}


		public static bool HasKey(string key)
		{
			return lnData.GetString(key) != null;
		}
	}
}