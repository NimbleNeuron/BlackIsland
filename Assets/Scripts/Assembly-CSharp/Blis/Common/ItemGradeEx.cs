using UnityEngine;

namespace Blis.Common
{
	public static class ItemGradeEx
	{
		private static readonly string CommonRichText = "<color=#C4C4C4>";


		private static readonly string UncommonRichText = "<color=#35FF94>";


		private static readonly string RareRichText = "<color=#2AC0FF>";


		private static readonly string EpicRichText = "<color=#D05EFF>";


		private static readonly string LegendRichText = "<color=#FFD738>";


		public static Color GetColor(this ItemGrade grade)
		{
			switch (grade)
			{
				default:
					return new Color32(196, 196, 196, byte.MaxValue);
				case ItemGrade.Uncommon:
					return new Color32(53, byte.MaxValue, 148, byte.MaxValue);
				case ItemGrade.Rare:
					return new Color32(42, 192, byte.MaxValue, byte.MaxValue);
				case ItemGrade.Epic:
					return new Color32(208, 94, byte.MaxValue, byte.MaxValue);
				case ItemGrade.Legend:
					return new Color32(byte.MaxValue, 215, 56, byte.MaxValue);
			}
		}


		public static string GetRichText(this ItemGrade grade)
		{
			switch (grade)
			{
				case ItemGrade.Common:
					return CommonRichText;
				case ItemGrade.Uncommon:
					return UncommonRichText;
				case ItemGrade.Rare:
					return RareRichText;
				case ItemGrade.Epic:
					return EpicRichText;
				case ItemGrade.Legend:
					return LegendRichText;
				default:
					return string.Empty;
			}
		}


		public static Sprite GetGradeBgSprite(this ItemGrade itemGrade)
		{
			ResourceManager inst = SingletonMonoBehaviour<ResourceManager>.inst;
			string str = "Ico_ItemGradebg_";
			int num = (int) itemGrade;
			return inst.GetCommonSprite(str + num.ToString().PadLeft(2, '0'));
		}
	}
}