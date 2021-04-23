using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Common.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blis.Common
{
	public class StringUtil : MonoBehaviour
	{
		public enum ColorStringType
		{
			Ally,

			Enemy,

			Notice,

			Normal,

			Area
		}


		private static StringBuilder sbText;


		private static byte[] arr;

		public static bool IsOverSizeANSI(string text, int maxSize)
		{
			char[] array = text.ToCharArray();
			int num = 0;
			foreach (char ch in array)
			{
				if (IsEnglish(ch) || IsNumeric(ch))
				{
					num++;
				}
				else
				{
					num += 2;
				}
			}

			return num > maxSize;
		}


		public static string ExtractNumb(string _text)
		{
			return Regex.Replace(_text, "\\D", "");
		}


		public static string GetStringToEncodingType(string _msg, Encoding _toType)
		{
			byte[] bytes = Encoding.Default.GetBytes(_msg);
			byte[] bytes2 = Encoding.Convert(Encoding.Default, _toType, bytes);
			return _toType.GetString(bytes2);
		}


		public static string Coloring(Color32 color, string str)
		{
			return string.Concat("<color=#", ColorUtility.ToHtmlStringRGB(color), ">", str, "</color>");
		}


		public static bool IsEnglish(char ch)
		{
			return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z';
		}


		public static bool IsKorean(char ch, bool isAllowInitial)
		{
			return '가' <= ch && ch <= '힣' || isAllowInitial && ('ᄀ' <= ch && ch <= 'ᇿ' || 'ㄱ' <= ch && ch <= 'ㆎ');
		}


		public static bool IsJapanese(char ch)
		{
			return '぀' <= ch && ch <= 'ゟ' || '゠' <= ch && ch <= 'ヿ' || '一' <= ch && ch <= '龿' || '々' == ch;
		}


		public static bool IsChinese(char ch)
		{
			return ch >= '一' && ch <= '鿿' || ch >= '㐀' && ch <= '䶵' || ch >= '⺀' && ch <= '⻿' || ch >= '豈' && ch <= '頻';
		}


		public static bool IsThai(string ch)
		{
			char[] array = ch.ToCharArray();
			if (array.Length == 0)
			{
				return false;
			}

			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] >= 'ก' && array[i] <= '๛')
				{
					return true;
				}
			}

			return false;
		}


		public static bool IsNumeric(char ch)
		{
			return '0' <= ch && ch <= '9';
		}


		public static bool IsBasicLatin(char ch)
		{
			return ' ' <= ch && ch <= '\u007f';
		}


		public static bool IsValidSpeech(string text)
		{
			foreach (char ch in text)
			{
				if (!IsEnglish(ch) && !IsKorean(ch, true) && !IsJapanese(ch) && !IsChinese(ch) && !IsNumeric(ch) &&
				    !IsBasicLatin(ch))
				{
					return true;
				}
			}

			return false;
		}


		public static bool IsVaildStr(string strText)
		{
			foreach (char ch in strText)
			{
				if (!IsEnglish(ch) && !IsKorean(ch, false) && !IsJapanese(ch) && !IsChinese(ch) && !IsNumeric(ch))
				{
					return false;
				}
			}

			return true;
		}


		public static bool IsIncludedSpaceChar(string _sInfo)
		{
			return _sInfo.Contains(" ");
		}


		public static bool IsIncludedLineFeed(string _string)
		{
			return _string.Contains("\n");
		}


		public static string NumberMeasure(int param_value)
		{
			string text = param_value.ToString();
			char c = 'Z';
			if (text.Length < 4)
			{
				return text;
			}

			if (text.Length < 7)
			{
				c = 'K';
			}
			else if (text.Length < 10)
			{
				c = 'M';
			}
			else if (text.Length < 13)
			{
				c = 'B';
			}

			if (text.Length % 3 == 0)
			{
				text = text.Substring(0, 3) + c;
			}
			else if (text.Length % 3 == 1)
			{
				text = string.Format("{0}.{1}{2}{3}", text[0], text[1], text[2], c);
			}
			else
			{
				text = string.Format("{0}{1}.{2}{3}", text[0], text[1], text[2], c);
			}

			return text;
		}


		public static string AssetToString(int money)
		{
			return string.Format("{0:n0}", money);
		}


		public static string AssetToUnitString(int money)
		{
			if (money.ToString().Length >= 10)
			{
				return string.Format("{0:n0}M", money / 1000000);
			}

			if (money.ToString().Length >= 7)
			{
				return string.Format("{0:n0}K", money / 1000);
			}

			return string.Format("{0:n0}", money);
		}


		public static string CreateBotNickname(string name, long number)
		{
			return string.Format("{0} Bot {1}", name, number.ToString());
		}


		public static string KeycodeToString(string key)
		{
			switch (key)
			{
				case "BackQuote":
					return "`";
				case "Backslash":
					return "\\";
				case "Comma":
					return ",";
				case "Equals":
					return "=";
				case "LeftBracket":
					return "[";
				case "Minus":
					return "-";
				case "Period":
					return ".";
				case "Quote":
					return "'";
				case "RightBracket":
					return "]";
				case "Semicolon":
					return ";";
				case "Slash":
					return "/";
				default:
					return key;
			}
		}

		// co: dotPeek
		// public static string KeycodeToString(string key)
		// {
		// 	uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
		// 	if (num <= 1522423415U)
		// 	{
		// 		if (num <= 651038163U)
		// 		{
		// 			if (num != 257323198U)
		// 			{
		// 				if (num == 651038163U)
		// 				{
		// 					if (key == "RightBracket")
		// 					{
		// 						return "]";
		// 					}
		// 				}
		// 			}
		// 			else if (key == "Semicolon")
		// 			{
		// 				return ";";
		// 			}
		// 		}
		// 		else if (num != 1050238388U)
		// 		{
		// 			if (num != 1258159639U)
		// 			{
		// 				if (num == 1522423415U)
		// 				{
		// 					if (key == "Quote")
		// 					{
		// 						return "'";
		// 					}
		// 				}
		// 			}
		// 			else if (key == "Backslash")
		// 			{
		// 				return "\\";
		// 			}
		// 		}
		// 		else if (key == "Equals")
		// 		{
		// 			return "=";
		// 		}
		// 	}
		// 	else if (num <= 1898928778U)
		// 	{
		// 		if (num != 1706424088U)
		// 		{
		// 			if (num != 1732852044U)
		// 			{
		// 				if (num == 1898928778U)
		// 				{
		// 					if (key == "Slash")
		// 					{
		// 						return "/";
		// 					}
		// 				}
		// 			}
		// 			else if (key == "LeftBracket")
		// 			{
		// 				return "[";
		// 			}
		// 		}
		// 		else if (key == "Comma")
		// 		{
		// 			return ",";
		// 		}
		// 	}
		// 	else if (num != 2267317284U)
		// 	{
		// 		if (num != 3388260431U)
		// 		{
		// 			if (num == 3818333214U)
		// 			{
		// 				if (key == "BackQuote")
		// 				{
		// 					return "`";
		// 				}
		// 			}
		// 		}
		// 		else if (key == "Minus")
		// 		{
		// 			return "-";
		// 		}
		// 	}
		// 	else if (key == "Period")
		// 	{
		// 		return ".";
		// 	}
		// 	return key;
		// }


		public static string GetColorString(Color color, string content)
		{
			string text = ColorUtility.ToHtmlStringRGB(color);
			return string.Concat("<color=#", text, ">", content, "</color>");
		}


		public static string GetColorString(string hex, string content)
		{
			return string.Concat("<color=#", hex, ">", content, "</color>");
		}


		public static string GetColorString(ColorStringType type, string content)
		{
			switch (type)
			{
				case ColorStringType.Ally:
					return GetColorString("00ffff", content);
				case ColorStringType.Enemy:
					return GetColorString(Color.red, content);
				case ColorStringType.Notice:
					return GetColorString("FFA500", content);
				case ColorStringType.Area:
					return GetColorString(Color.yellow, content);
			}

			return content;
		}


		public static string GetColorConvertString(string content, ColorStringType type, int startIndex, int endIndex)
		{
			string text = content.Substring(startIndex, endIndex - startIndex);
			content = content.Replace(text, GetColorString(type, text));
			return content;
		}


		public static string GetLocalizedPrice(string currency, int originalPrice)
		{
			string result;
			if (!(currency == "KRW"))
			{
				if (!(currency == "JPY"))
				{
					if (!(currency == "CNY"))
					{
						if (!(currency == "EUR"))
						{
							result = string.Format("${0:0.##}", originalPrice / 1100f);
						}
						else
						{
							result = string.Format("€{0:0.##}", originalPrice / 1400f * 1.1f);
						}
					}
					else
					{
						result = string.Format("{0}元", (int) Math.Round(originalPrice / 170f * 1.1f));
					}
				}
				else
				{
					result = string.Format("￥{0:n0}", (int) Math.Round(originalPrice / 11f * 1.1f));
				}
			}
			else
			{
				result = string.Format("{0:n0}원", originalPrice);
			}

			return result;
		}


		public static string RandomString(int length)
		{
			return new string((from s in Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
				select s[Random.Range(0, s.Length)]).ToArray<char>());
		}


		public static bool ValidateNicknameLength(string nickname)
		{
			return ArchStringUtil.IsOverSizeANSI(nickname, 2) && !ArchStringUtil.IsOverSizeANSI(nickname, 16);
		}
	}
}