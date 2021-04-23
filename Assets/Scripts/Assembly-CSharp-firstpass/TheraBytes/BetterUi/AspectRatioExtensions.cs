using System;

namespace TheraBytes.BetterUi
{
	
	public static class AspectRatioExtensions
	{
		
		public static float GetRatioValue(this AspectRatio ratio)
		{
			switch (ratio)
			{
			case AspectRatio.Portrait21To9:
				return 0.421875f;
			case AspectRatio.Portrait16To9:
				return 0.5625f;
			case AspectRatio.Portrait5To3:
				return 0.6f;
			case AspectRatio.Portrait16To10:
				return 0.625f;
			case AspectRatio.Portrait3To2:
				return 0.6666667f;
			case AspectRatio.Portrait4To3:
				return 0.75f;
			case AspectRatio.Portrait5To4:
				return 0.8f;
			case AspectRatio._:
				return 1f;
			case AspectRatio.Landscape5To4:
				return 1.25f;
			case AspectRatio.Landscape4To3:
				return 1.3333334f;
			case AspectRatio.Landscape3To2:
				return 1.5f;
			case AspectRatio.Landscape16To10:
				return 1.6f;
			case AspectRatio.Landscape5To3:
				return 1.6666666f;
			case AspectRatio.Landscape16To9:
				return 1.7777778f;
			case AspectRatio.Landscape21To9:
				return 2.3703704f;
			default:
				throw new ArgumentException();
			}
		}

		
		public static string ToShortDetailedString(this AspectRatio ratio)
		{
			if (ratio == AspectRatio._)
			{
				return "↑ Portrait    ↓ Landscape";
			}
			string arg = ratio.ToShortString();
			float ratioValue = ratio.GetRatioValue();
			return string.Format("{0}\t= {1:0.00}", arg, ratioValue);
		}

		
		public static string ToShortString(this AspectRatio ratio)
		{
			switch (ratio)
			{
			case AspectRatio.Portrait21To9:
				return "9:21 (27:64)";
			case AspectRatio.Portrait16To9:
				return "9:16";
			case AspectRatio.Portrait5To3:
				return "3:5";
			case AspectRatio.Portrait16To10:
				return "10:16 (5:8)";
			case AspectRatio.Portrait3To2:
				return "2:3";
			case AspectRatio.Portrait4To3:
				return "3:4";
			case AspectRatio.Portrait5To4:
				return "4:5";
			case AspectRatio._:
				return "1:1";
			case AspectRatio.Landscape5To4:
				return "5:4";
			case AspectRatio.Landscape4To3:
				return "4:3";
			case AspectRatio.Landscape3To2:
				return "3:2";
			case AspectRatio.Landscape16To10:
				return "16:10 (8:5)";
			case AspectRatio.Landscape5To3:
				return "5:3";
			case AspectRatio.Landscape16To9:
				return "16:9";
			case AspectRatio.Landscape21To9:
				return "21:9 (64:27)";
			default:
				throw new ArgumentException();
			}
		}

		
		private const float LANDSCAPE5TO4 = 1.25f;

		
		private const float LANDSCAPE4TO3 = 1.3333334f;

		
		private const float LANDSCAPE3TO2 = 1.5f;

		
		private const float LANDSCAPE16TO10 = 1.6f;

		
		private const float LANDSCAPE5TO3 = 1.6666666f;

		
		private const float LANDSCAPE16TO9 = 1.7777778f;

		
		private const float LANDSCAPE21TO9 = 2.3703704f;

		
		private const float PORTRAIT21TO9 = 0.421875f;

		
		private const float PORTRAIT16TO9 = 0.5625f;

		
		private const float PORTRAIT5TO3 = 0.6f;

		
		private const float PORTRAIT16TO10 = 0.625f;

		
		private const float PORTRAIT3TO2 = 0.6666667f;

		
		private const float PORTRAIT4TO3 = 0.75f;

		
		private const float PORTRAIT5TO4 = 0.8f;
	}
}
