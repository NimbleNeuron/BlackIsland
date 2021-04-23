using Blis.Client;
using UnityEngine;

namespace Blis.Common
{
	public static class RankingTierTypeEx
	{
		public static string GetName(this RankingTierType rankingTierType)
		{
			switch (rankingTierType)
			{
				case RankingTierType.Unrank:
					return Ln.Get("언랭크");
				case RankingTierType.Iron:
					return Ln.Get("아이언 서브젝트");
				case RankingTierType.Bronze:
					return Ln.Get("브론즈 서브젝트");
				case RankingTierType.Silver:
					return Ln.Get("실버 서브젝트");
				case RankingTierType.Gold:
					return Ln.Get("골드 서브젝트");
				case RankingTierType.Platinum:
					return Ln.Get("플래티넘 서브젝트");
				case RankingTierType.Diamond:
					return Ln.Get("다이아몬드 서브젝트");
				case RankingTierType.Demigod:
					return Ln.Get("데미갓");
				case RankingTierType.Eternity:
					return Ln.Get("이터니티");
				default:
					if (rankingTierType != RankingTierType.Normal)
					{
						return "";
					}

					return Ln.Get("일반");
			}
		}


		public static Color GetColor(this RankingTierType rankingTierType)
		{
			switch (rankingTierType)
			{
				case RankingTierType.Unrank:
					return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				case RankingTierType.Iron:
					return new Color32(171, 171, 171, byte.MaxValue);
				case RankingTierType.Bronze:
					return new Color32(byte.MaxValue, 195, 148, byte.MaxValue);
				case RankingTierType.Silver:
					return new Color32(57, 46, 64, byte.MaxValue);
				case RankingTierType.Gold:
					return new Color32(byte.MaxValue, 248, 147, byte.MaxValue);
				case RankingTierType.Platinum:
					return new Color32(181, byte.MaxValue, 223, byte.MaxValue);
				case RankingTierType.Diamond:
					return new Color32(byte.MaxValue, 252, 245, byte.MaxValue);
				case RankingTierType.Demigod:
					return new Color32(130, 203, 227, byte.MaxValue);
				case RankingTierType.Eternity:
					return new Color32(244, 205, byte.MaxValue, byte.MaxValue);
				default:
					if (rankingTierType != RankingTierType.Normal)
					{
						return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
					}

					return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			}
		}
	}
}