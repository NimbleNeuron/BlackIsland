using System;

namespace Blis.Common
{
	public static class WeaponTypeEx
	{
		public static bool IsGunType(this WeaponType weaponType)
		{
			return weaponType - WeaponType.Pistol <= 2;
		}


		public static bool IsThrowType(this WeaponType weaponType)
		{
			return weaponType - WeaponType.HighAngleFire <= 1;
		}


		public static MasteryType GetWeaponMasteryType(this WeaponType weaponType)
		{
			switch (weaponType)
			{
				case WeaponType.None:
					return MasteryType.None;
				case WeaponType.Glove:
					return MasteryType.Glove;
				case WeaponType.Tonfa:
					return MasteryType.Tonfa;
				case WeaponType.Bat:
					return MasteryType.Bat;
				case WeaponType.Whip:
					return MasteryType.Whip;
				case WeaponType.HighAngleFire:
					return MasteryType.HighAngleFire;
				case WeaponType.DirectFire:
					return MasteryType.DirectFire;
				case WeaponType.Bow:
					return MasteryType.Bow;
				case WeaponType.CrossBow:
					return MasteryType.CrossBow;
				case WeaponType.Pistol:
					return MasteryType.Pistol;
				case WeaponType.AssaultRifle:
					return MasteryType.AssaultRifle;
				case WeaponType.SniperRifle:
					return MasteryType.SniperRifle;
				case WeaponType.Cannon:
					return MasteryType.Cannon;
				case WeaponType.Hammer:
					return MasteryType.Hammer;
				case WeaponType.Axe:
					return MasteryType.Axe;
				case WeaponType.OneHandSword:
					return MasteryType.OneHandSword;
				case WeaponType.TwoHandSword:
					return MasteryType.TwoHandSword;
				case WeaponType.Polearm:
					return MasteryType.Polearm;
				case WeaponType.DualSword:
					return MasteryType.DualSword;
				case WeaponType.Spear:
					return MasteryType.Spear;
				case WeaponType.Nunchaku:
					return MasteryType.Nunchaku;
				case WeaponType.Rapier:
					return MasteryType.Rapier;
				case WeaponType.Guitar:
					return MasteryType.Guitar;
				default:
					throw new ArgumentOutOfRangeException("weaponType", weaponType, null);
			}
		}
	}
}