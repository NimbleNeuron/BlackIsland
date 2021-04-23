using System.Text;

namespace Common.Utils
{
	
	public static class ArchStringUtil
	{
		
		public static string CutOverSizeANSI(string text, int maxSize)
		{
			ArchStringUtil.sbText = new StringBuilder(maxSize);
			int num = 0;
			foreach (char c in text.ToCharArray())
			{
				if (ArchStringUtil.IsEnglish(c) || ArchStringUtil.IsNumeric(c))
				{
					num++;
				}
				else
				{
					num += 2;
				}
				if (maxSize < num)
				{
					break;
				}
				ArchStringUtil.sbText.Append(c);
			}
			return ArchStringUtil.sbText.ToString();
		}

		
		public static bool IsOverSizeANSI(string text, int maxSize)
		{
			char[] array = text.ToCharArray();
			int num = 0;
			foreach (char ch in array)
			{
				if (ArchStringUtil.IsEnglish(ch) || ArchStringUtil.IsNumeric(ch))
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

		
		public static bool IsEnglish(char ch)
		{
			return ('a' <= ch && ch <= 'z') || ('A' <= ch && ch <= 'Z');
		}

		
		public static bool IsKorean(char ch, bool isAllowInitial)
		{
			return ('가' <= ch && ch <= '힣') || (isAllowInitial && (('ᄀ' <= ch && ch <= 'ᇿ') || ('ㄱ' <= ch && ch <= 'ㆎ')));
		}

		
		public static bool IsJapanese(char ch)
		{
			return ('぀' <= ch && ch <= 'ゟ') || ('゠' <= ch && ch <= 'ヿ') || ('一' <= ch && ch <= '龿') || '々' == ch;
		}

		
		public static bool IsChinese(char ch)
		{
			return (ch >= '一' && ch <= '鿿') || (ch >= '㐀' && ch <= '䶵') || (ch >= '⺀' && ch <= '⻿') || (ch >= '豈' && ch <= '頻');
		}

		
		public static bool IsNumeric(char ch)
		{
			return '0' <= ch && ch <= '9';
		}

		
		private static StringBuilder sbText;

		
		private static byte[] arr;
	}
}
