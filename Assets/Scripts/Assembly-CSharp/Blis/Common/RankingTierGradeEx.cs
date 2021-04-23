using Blis.Client;

namespace Blis.Common
{
	public static class RankingTierGradeEx
	{
		public static string GetName(this RankingTierGrade rankingTierGrade)
		{
			switch (rankingTierGrade)
			{
				case RankingTierGrade.One:
					return Ln.Get("단계 1");
				case RankingTierGrade.Two:
					return Ln.Get("단계 2");
				case RankingTierGrade.Three:
					return Ln.Get("단계 3");
				case RankingTierGrade.Four:
					return Ln.Get("단계 4");
				default:
					return "";
			}
		}
	}
}