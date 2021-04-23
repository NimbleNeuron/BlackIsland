using System;
using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	public class MasteryDB
	{
		private List<CharacterMasteryData> characterMasteryList;


		private List<MasteryExpData> masteryExpList;


		private List<MasteryLevelData> masteryLevelList;


		private List<WeaponTypeInfoData> weaponTypeInfoList;

		public void SetData<T>(List<T> data)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(WeaponTypeInfoData))
			{
				weaponTypeInfoList = data.Cast<WeaponTypeInfoData>().ToList<WeaponTypeInfoData>();
				return;
			}

			if (typeFromHandle == typeof(CharacterMasteryData))
			{
				characterMasteryList = data.Cast<CharacterMasteryData>().ToList<CharacterMasteryData>();
				return;
			}

			if (typeFromHandle == typeof(MasteryExpData))
			{
				masteryExpList = data.Cast<MasteryExpData>().ToList<MasteryExpData>();
				return;
			}

			if (typeFromHandle == typeof(MasteryLevelData))
			{
				masteryLevelList = data.Cast<MasteryLevelData>().ToList<MasteryLevelData>();
			}
		}


		public List<CharacterMasteryData> GetAllCharacterMasteryData()
		{
			return characterMasteryList;
		}


		public CharacterMasteryData GetCharacterMasteryData(int code)
		{
			return characterMasteryList.Find(data => data.code == code);
		}


		public MasteryLevelData GetMasteryLevelData(MasteryType masteryType, int masteryLevel)
		{
			return masteryLevelList.Find(data => data.type == masteryType && data.masteryLevel == masteryLevel);
		}


		public MasteryExpData GetMasteryExpData(MasteryConditionType type, ItemGrade itemGrade)
		{
			return masteryExpList.Find(data => data.conditionType == type && data.grade == itemGrade);
		}


		public List<MasteryExpData> FindMasteryExpData(MasteryType masteryType)
		{
			if (masteryType.IsWeaponMastery())
			{
				return masteryExpList.FindAll(x =>
					x.masteryType1 == MasteryType.None && x.value1 > 0 ||
					x.masteryType2 == MasteryType.None && x.value2 > 0 ||
					x.masteryType3 == MasteryType.None && x.value3 > 0);
			}

			return masteryExpList.FindAll(x =>
				x.masteryType1 == masteryType || x.masteryType2 == masteryType || x.masteryType3 == masteryType);
		}


		public WeaponTypeInfoData GetWeaponTypeInfoData(WeaponType weaponType)
		{
			return weaponTypeInfoList.Find(data => data.type == weaponType);
		}


		public List<WeaponTypeInfoData> GetAllWeaponTypeInfoDatas()
		{
			return weaponTypeInfoList;
		}
	}
}