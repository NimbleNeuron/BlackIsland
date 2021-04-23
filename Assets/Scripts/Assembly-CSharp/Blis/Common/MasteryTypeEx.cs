using System;
using UnityEngine;

namespace Blis.Common
{
	public static class MasteryTypeEx
	{
		public static string GetName(this MasteryType masteryType)
		{
			switch (masteryType)
			{
				case MasteryType.None:
					return "None";
				case MasteryType.Glove:
					return "건틀릿";
				case MasteryType.Tonfa:
					return "조";
				case MasteryType.Bat:
					return "봉";
				case MasteryType.Whip:
					return "채찍";
				case MasteryType.HighAngleFire:
					return "던지기";
				case MasteryType.DirectFire:
					return "암기";
				case MasteryType.Bow:
					return "활";
				case MasteryType.CrossBow:
					return "석궁";
				case MasteryType.Pistol:
					return "권총";
				case MasteryType.AssaultRifle:
					return "돌격소총";
				case MasteryType.SniperRifle:
					return "저격총";
				case MasteryType.Cannon:
					return "대포";
				case MasteryType.Hammer:
					return "망치";
				case MasteryType.Axe:
					return "도끼";
				case MasteryType.OneHandSword:
					return "한손검";
				case MasteryType.TwoHandSword:
					return "양손검";
				case MasteryType.Polearm:
					return "폴암";
				case MasteryType.DualSword:
					return "쌍검";
				case MasteryType.Spear:
					return "창";
				case MasteryType.Nunchaku:
					return "쌍절곤";
				case MasteryType.Rapier:
					return "레이피어";
				case MasteryType.Guitar:
					return "기타";
				default:
					switch (masteryType)
					{
						case MasteryType.Trap:
							return "트랩";
						case MasteryType.Craft:
							return "제작";
						case MasteryType.Search:
							return "탐색";
						case MasteryType.Move:
							return "이동";
						default:
							switch (masteryType)
							{
								case MasteryType.Health:
									return "체력";
								case MasteryType.Defense:
									return "방어";
								case MasteryType.Meditation:
									return "정신";
								case MasteryType.Hunt:
									return "사냥";
								default:
									return "이름 없음";
							}
					}
			}
		}


		public static bool IsWeaponMastery(this MasteryType masteryType)
		{
			if (masteryType <= MasteryType.Guitar)
			{
				if (masteryType != MasteryType.None)
				{
					if (masteryType - MasteryType.Glove > 21)
					{
						goto IL_28;
					}

					return true;
				}
			}
			else if (masteryType - MasteryType.Trap > 3 && masteryType - MasteryType.Health > 3)
			{
				goto IL_28;
			}

			return false;
			IL_28:
			throw new ArgumentOutOfRangeException("masteryType", masteryType, null);
		}


		public static WeaponType GetWeaponType(this MasteryType masteryType)
		{
			switch (masteryType)
			{
				case MasteryType.None:
					return WeaponType.None;
				case MasteryType.Glove:
					return WeaponType.Glove;
				case MasteryType.Tonfa:
					return WeaponType.Tonfa;
				case MasteryType.Bat:
					return WeaponType.Bat;
				case MasteryType.Whip:
					return WeaponType.Whip;
				case MasteryType.HighAngleFire:
					return WeaponType.HighAngleFire;
				case MasteryType.DirectFire:
					return WeaponType.DirectFire;
				case MasteryType.Bow:
					return WeaponType.Bow;
				case MasteryType.CrossBow:
					return WeaponType.CrossBow;
				case MasteryType.Pistol:
					return WeaponType.Pistol;
				case MasteryType.AssaultRifle:
					return WeaponType.AssaultRifle;
				case MasteryType.SniperRifle:
					return WeaponType.SniperRifle;
				case MasteryType.Cannon:
					return WeaponType.Cannon;
				case MasteryType.Hammer:
					return WeaponType.Hammer;
				case MasteryType.Axe:
					return WeaponType.Axe;
				case MasteryType.OneHandSword:
					return WeaponType.OneHandSword;
				case MasteryType.TwoHandSword:
					return WeaponType.TwoHandSword;
				case MasteryType.Polearm:
					return WeaponType.Polearm;
				case MasteryType.DualSword:
					return WeaponType.DualSword;
				case MasteryType.Spear:
					return WeaponType.Spear;
				case MasteryType.Nunchaku:
					return WeaponType.Nunchaku;
				case MasteryType.Rapier:
					return WeaponType.Rapier;
				case MasteryType.Guitar:
					return WeaponType.Guitar;
				default:
					if (masteryType - MasteryType.Trap > 3 && masteryType - MasteryType.Health > 3)
					{
						throw new ArgumentOutOfRangeException("masteryType", masteryType, null);
					}

					return WeaponType.None;
			}
		}


		public static MasteryCategory GetCategory(this MasteryType masteryType)
		{
			if (masteryType > MasteryType.Trap)
			{
				if (masteryType - MasteryType.Craft > 2)
				{
					if (masteryType - MasteryType.Health <= 2)
					{
						return MasteryCategory.Growth;
					}

					if (masteryType != MasteryType.Hunt)
					{
						return MasteryCategory.None;
					}
				}

				return MasteryCategory.Search;
			}

			if (masteryType - MasteryType.Glove <= 21)
			{
				return MasteryCategory.Combat;
			}

			if (masteryType != MasteryType.Trap)
			{
				return MasteryCategory.None;
			}

			return MasteryCategory.Growth;
		}


		public static Sprite GetIcon(this MasteryType masteryType)
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(string.Format("Ico_Ability_{0}",
				masteryType));
		}


		public static bool? IsLaunchProjectile(this MasteryType masteryType)
		{
			switch (masteryType)
			{
				case MasteryType.Glove:
				case MasteryType.Tonfa:
				case MasteryType.Bat:
				case MasteryType.Whip:
				case MasteryType.Hammer:
				case MasteryType.Axe:
				case MasteryType.OneHandSword:
				case MasteryType.TwoHandSword:
				case MasteryType.Polearm:
				case MasteryType.DualSword:
				case MasteryType.Spear:
				case MasteryType.Rapier:
					return false;
				case MasteryType.HighAngleFire:
				case MasteryType.DirectFire:
				case MasteryType.Bow:
				case MasteryType.CrossBow:
				case MasteryType.Pistol:
				case MasteryType.AssaultRifle:
				case MasteryType.SniperRifle:
				case MasteryType.Cannon:
				case MasteryType.Nunchaku:
				case MasteryType.Guitar:
					return true;
				default:
					return null;
			}
		}
	}
}