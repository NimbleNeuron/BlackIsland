using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YoutubeLight
{
	
	internal static class MagicHands
	{
		
		private static string ApplyOperation(string cipher, string op)
		{
			char c = op[0];
			if (c == 'r')
			{
				return new string(cipher.ToCharArray().Reverse<char>().ToArray<char>());
			}
			if (c == 's')
			{
				int opIndex = MagicHands.GetOpIndex(op);
				return cipher.Substring(opIndex);
			}
			if (c != 'w')
			{
				throw new NotImplementedException("Couldn't find cipher operation.");
			}
			int opIndex2 = MagicHands.GetOpIndex(op);
			return MagicHands.SwapFirstChar(cipher, opIndex2);
		}

		
		public static string DecipherWithOperations(string cipher, string operations)
		{
			return operations.Split(new string[]
			{
				" "
			}, StringSplitOptions.RemoveEmptyEntries).Aggregate(cipher, new Func<string, string, string>(MagicHands.ApplyOperation));
		}

		
		private static string GetFunctionFromLine(string currentLine)
		{
			return new Regex("\\w+\\.(?<functionID>\\w+)\\(").Match(currentLine).Groups["functionID"].Value;
		}

		
		private static int GetOpIndex(string op)
		{
			return int.Parse(new Regex(".(\\d+)").Match(op).Result("$1"));
		}

		
		private static string SwapFirstChar(string cipher, int index)
		{
			StringBuilder stringBuilder = new StringBuilder(cipher);
			stringBuilder[0] = cipher[index];
			stringBuilder[index] = cipher[0];
			return stringBuilder.ToString();
		}
	}
}
