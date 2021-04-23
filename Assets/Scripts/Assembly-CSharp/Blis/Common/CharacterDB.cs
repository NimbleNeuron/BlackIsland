using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blis.Common
{
	public class CharacterDB
	{
		private List<ActionCostData> actionCostDataList = new List<ActionCostData>();


		private List<CharacterExpData> characterExpList = new List<CharacterExpData>();


		private List<CharacterLevelUpStatData> characterLevelUpStatList = new List<CharacterLevelUpStatData>();


		private List<CharacterData> characterList = new List<CharacterData>();


		private List<CharacterModeModifierData> characterModeModifierDataList = new List<CharacterModeModifierData>();


		private List<CharacterSkinData> characterSkinList = new List<CharacterSkinData>();


		private List<CriticalChanceData> criticalChanceDataList = new List<CriticalChanceData>();


		private List<SummonData> summonDataList = new List<SummonData>();


		private List<WeaponAnimatorLayersData> weaponAnimatorLayersDataList = new List<WeaponAnimatorLayersData>();


		private readonly Dictionary<int, Dictionary<WeaponType, List<WeaponAnimatorLayersData>>>
			weaponAnimatorLayersDataMap = new Dictionary<int, Dictionary<WeaponType, List<WeaponAnimatorLayersData>>>();


		private List<WeaponMountData> weaponMountDataList = new List<WeaponMountData>();


		private readonly Dictionary<int, Dictionary<int, Dictionary<WeaponType, List<WeaponMountData>>>>
			weaponMountDataMap = new Dictionary<int, Dictionary<int, Dictionary<WeaponType, List<WeaponMountData>>>>();

		public CharacterDB()
		{
			if (GameDB.useDummyData)
			{
				InitDummy();
			}
		}


		public void SetData<T>(List<T> data)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(CharacterData))
			{
				characterList = data.Cast<CharacterData>().ToList<CharacterData>();
				return;
			}

			if (typeFromHandle == typeof(CharacterSkinData))
			{
				characterSkinList = data.Cast<CharacterSkinData>().ToList<CharacterSkinData>();
				return;
			}

			if (typeFromHandle == typeof(CharacterExpData))
			{
				characterExpList = data.Cast<CharacterExpData>().ToList<CharacterExpData>();
				return;
			}

			if (typeFromHandle == typeof(CharacterLevelUpStatData))
			{
				characterLevelUpStatList = data.Cast<CharacterLevelUpStatData>().ToList<CharacterLevelUpStatData>();
				return;
			}

			if (typeFromHandle == typeof(CharacterModeModifierData))
			{
				characterModeModifierDataList =
					data.Cast<CharacterModeModifierData>().ToList<CharacterModeModifierData>();
				return;
			}

			if (typeFromHandle == typeof(SummonData))
			{
				summonDataList = data.Cast<SummonData>().ToList<SummonData>();
				return;
			}

			if (typeFromHandle == typeof(ActionCostData))
			{
				actionCostDataList = data.Cast<ActionCostData>().ToList<ActionCostData>();
				return;
			}

			if (typeFromHandle == typeof(CriticalChanceData))
			{
				criticalChanceDataList = data.Cast<CriticalChanceData>().ToList<CriticalChanceData>();
				return;
			}

			if (typeFromHandle == typeof(WeaponMountData))
			{
				weaponMountDataList = data.Cast<WeaponMountData>().ToList<WeaponMountData>();
				if (weaponMountDataList == null || weaponMountDataList.Count <= 0)
				{
					return;
				}

				foreach (WeaponMountData weaponMountData in weaponMountDataList)
				{
					int characterCode = weaponMountData.characterCode;
					int skinIndex = weaponMountData.skinIndex;
					WeaponType weaponType = weaponMountData.weaponType;
					if (!weaponMountDataMap.ContainsKey(characterCode))
					{
						weaponMountDataMap.Add(characterCode,
							new Dictionary<int, Dictionary<WeaponType, List<WeaponMountData>>>());
					}

					if (!weaponMountDataMap[characterCode].ContainsKey(skinIndex))
					{
						weaponMountDataMap[characterCode].Add(skinIndex,
							new Dictionary<WeaponType, List<WeaponMountData>>(
								SingletonComparerEnum<WeaponTypeComparer, WeaponType>.Instance));
					}

					if (!weaponMountDataMap[characterCode][skinIndex].ContainsKey(weaponType))
					{
						weaponMountDataMap[characterCode][skinIndex].Add(weaponType, new List<WeaponMountData>());
					}

					weaponMountDataMap[characterCode][skinIndex][weaponType].Add(weaponMountData);
				}

				using (List<int>.Enumerator enumerator2 = new List<int>(weaponMountDataMap.Keys).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						int key = enumerator2.Current;
						foreach (int num in new List<int>(weaponMountDataMap[key].Keys))
						{
							if (num != 0)
							{
								foreach (WeaponType key2 in new List<WeaponType>(weaponMountDataMap[key][0].Keys))
								{
									if (!weaponMountDataMap[key][num].ContainsKey(key2))
									{
										List<WeaponMountData> value = weaponMountDataMap[key][0][key2]
											.ToList<WeaponMountData>();
										weaponMountDataMap[key][num].Add(key2, value);
									}
									else
									{
										List<WeaponMountData> list = weaponMountDataMap[key][num][key2];
										IEnumerable<WeaponMountData> collection =
											from x in weaponMountDataMap[key][0][key2]
											where list.FindIndex(e => e.mountType == x.mountType) == -1
											select x;
										list.AddRange(collection);
										list.RemoveAll(x =>
											string.IsNullOrEmpty(x.prefab) || x.prefab.ToLower().Equals("none"));
									}
								}
							}
						}
					}

					return;
				}
			}

			if (typeFromHandle == typeof(WeaponAnimatorLayersData))
			{
				weaponAnimatorLayersDataList = data.Cast<WeaponAnimatorLayersData>().ToList<WeaponAnimatorLayersData>();
				if (weaponAnimatorLayersDataList != null && weaponAnimatorLayersDataList.Count > 0)
				{
					foreach (WeaponAnimatorLayersData weaponAnimatorLayersData in weaponAnimatorLayersDataList)
					{
						int characterCode2 = weaponAnimatorLayersData.characterCode;
						WeaponType weaponType2 = weaponAnimatorLayersData.weaponType;
						if (!weaponAnimatorLayersDataMap.ContainsKey(characterCode2))
						{
							weaponAnimatorLayersDataMap.Add(characterCode2,
								new Dictionary<WeaponType, List<WeaponAnimatorLayersData>>(
									SingletonComparerEnum<WeaponTypeComparer, WeaponType>.Instance));
						}

						if (!weaponAnimatorLayersDataMap[characterCode2].ContainsKey(weaponType2))
						{
							weaponAnimatorLayersDataMap[characterCode2]
								.Add(weaponType2, new List<WeaponAnimatorLayersData>());
						}

						weaponAnimatorLayersDataMap[characterCode2][weaponType2].Add(weaponAnimatorLayersData);
					}
				}
			}
		}


		public CharacterData GetCharacterData(int code)
		{
			return characterList.Find(data => data.code == code);
		}


		public List<CharacterData> GetAllCharacterData()
		{
			return characterList;
		}


		public CharacterExpData GetExpData(int level)
		{
			return characterExpList.Find(data => data.level == level);
		}


		public CharacterLevelUpStatData GetLevelUpStatData(int code)
		{
			return characterLevelUpStatList.Find(data => data.code == code);
		}


		public CharacterModeModifierData GetCharacterModeModifierData(int characterCode, WeaponType weapon)
		{
			return characterModeModifierDataList.Find(data =>
				data.characterCode == characterCode && data.weaponType == weapon);
		}


		public List<SummonData> GetAllSummonData()
		{
			return summonDataList;
		}


		public SummonData GetSummonData(int code)
		{
			SummonData summonData = summonDataList.Find(d => d.code == code);
			if (summonData == null)
			{
				Log.E("Failed to find summonData by code[{0}]", code);
			}

			return summonData;
		}


		public ActionCostData GetActionCost(CastingActionType type)
		{
			return actionCostDataList.Find(data => data.type == type);
		}


		public float GetActualCriticalChance(int probability)
		{
			int num = Mathf.Min(probability, criticalChanceDataList.Count) - 1;
			if (num < 0)
			{
				return 0f;
			}

			return criticalChanceDataList[num].actualUse;
		}


		public List<WeaponMountData> GetMountDataList(int characterCode, int skinIndex)
		{
			List<WeaponMountData> list = new List<WeaponMountData>();
			if (weaponMountDataMap[characterCode][skinIndex].Count <= 0)
			{
				return list;
			}

			foreach (KeyValuePair<WeaponType, List<WeaponMountData>> keyValuePair in weaponMountDataMap[characterCode][
				skinIndex])
			{
				list.AddRange(keyValuePair.Value);
			}

			return list;
		}


		public List<WeaponMountData> GetMountDataList(int characterCode, int skinIndex, WeaponType weaponType)
		{
			return weaponMountDataMap[characterCode][skinIndex][weaponType];
		}


		public List<WeaponAnimatorLayersData> GetAnimatorLayers(int characterCode, WeaponType weaponType)
		{
			return weaponAnimatorLayersDataMap[characterCode][weaponType];
		}


		private void InitDummy()
		{
			characterList = new List<CharacterData>();
			characterSkinList = new List<CharacterSkinData>();
			characterExpList = new List<CharacterExpData>();
			characterLevelUpStatList = new List<CharacterLevelUpStatData>();
			characterModeModifierDataList = new List<CharacterModeModifierData>();
			summonDataList = new List<SummonData>();
			actionCostDataList = new List<ActionCostData>();
			criticalChanceDataList = new List<CriticalChanceData>();
			weaponMountDataList = new List<WeaponMountData>();
			weaponAnimatorLayersDataList = new List<WeaponAnimatorLayersData>();
			InitCharacterDummy();
			InitCharacterSkinDummy();
			LoadDummyCharacterLevelData();
			LoadDummyCharacterLevelUpStatData();
		}


		private void AddCharacterLevelData(int level, int levelUpExp, int accumulateLevelUpExp)
		{
			characterExpList.Add(new CharacterExpData(level, levelUpExp, accumulateLevelUpExp));
		}


		private void AddCharacterLevelUpStatData(int hp, int attack, int defence)
		{
			characterLevelUpStatList.Add(new CharacterLevelUpStatData(1, "", hp, 0, attack, defence, 0f, 0f, 0f, 0));
		}


		private void LoadDummyCharacterLevelData()
		{
			AddCharacterLevelData(1, 100, 100);
			AddCharacterLevelData(2, 120, 220);
			AddCharacterLevelData(3, 130, 350);
			AddCharacterLevelData(4, 140, 490);
			AddCharacterLevelData(5, 150, 640);
			AddCharacterLevelData(6, 160, 800);
			AddCharacterLevelData(7, 0, 800);
		}


		private void LoadDummyCharacterLevelUpStatData()
		{
			AddCharacterLevelUpStatData(35, 6, 8);
		}


		private void InitCharacterDummy()
		{
			AddCharacter(1, "Jackie", 518, 300, 0, 0, 33, 21, 2f, 10f, 0.155f, 5f, 10f, 1f, 1.5f, "Jackie");
			AddCharacter(2, "Aya", 562, 300, 0, 0, 38, 25, 8f, 13f, 0.125f, 5f, 10f, 1f, 1.5f, "Aya");
			AddCharacter(3, "Fiora", 562, 300, 0, 0, 38, 25, 8f, 13f, 0.125f, 5f, 10f, 1f, 1.5f, "Fiora");
			AddCharacter(4, "Rosalio", 562, 300, 0, 0, 38, 25, 8f, 13f, 0.125f, 5f, 10f, 1f, 1.5f, "Rosalio");
			AddCharacter(5, "Alex", 562, 300, 0, 0, 38, 25, 8f, 13f, 0.125f, 5f, 10f, 1f, 1.5f, "Alex");
			AddCharacter(6, "Nadine", 562, 300, 0, 0, 38, 25, 8f, 13f, 0.125f, 5f, 10f, 1f, 1.5f, "Nadine");
			AddCharacter(7, "Hyunwoo", 562, 300, 0, 0, 38, 25, 8f, 13f, 0.125f, 5f, 10f, 1f, 1.5f, "Hyunwoo");
		}


		private void InitCharacterSkinDummy()
		{
			int num = 0;
			SkinGrade grade = SkinGrade.Common;
			SkinPurchaseType purchaseType = SkinPurchaseType.FREE;
			string format = "Effects/{0}/S{1:D3}";
			string format2 = "Projectile/{0}/S{1:D3}";
			string format3 = "Object/{0}/S{1:D3}";
			string format4 = "Sound/FX/{0}/S{1:D3}";
			string format5 = "Sound/VOICE/{0}/S{1:D3}";
			string format6 = "Weapons/{0}/S{1:D3}";
			string format7 = "Weapons/{0}/C{1:D3}";
			for (int i = 0; i < characterList.Count; i++)
			{
				int num2 = i + 1;
				string resource = characterList[i].resource;
				characterSkinList.Add(new CharacterSkinData(num2, num2, num, grade, purchaseType,
					string.Format(format, resource, num), string.Format(format2, resource, num),
					string.Format(format3, resource, num), string.Format(format4, resource, num),
					string.Format(format5, resource, num), string.Format(format6, resource, num),
					string.Format(format7, resource, num)));
			}
		}


		private void AddCharacter(int code, string name, int maxHp, int maxSp, int initExtraPoint, int maxExtraPoint,
			int attackPower, int defense, float hpRegen, float spRegen, float attackSpeed, float moveSpeed,
			float sightRange, float radius, float uiHeight, string resource)
		{
			characterList.Add(new CharacterData(code, name, maxHp, maxSp, initExtraPoint, maxExtraPoint, attackPower,
				defense, hpRegen, spRegen, attackSpeed, moveSpeed, sightRange, radius, uiHeight, resource));
		}


		public CharacterSkinData GetSkinData(int characterCode, int skinIndex)
		{
			if (skinIndex == 0)
			{
				return characterSkinList.FirstOrDefault(data => data.characterCode == characterCode);
			}

			return characterSkinList.FirstOrDefault(data =>
				data.characterCode == characterCode && data.index == skinIndex);
		}


		public CharacterSkinData GetSkinData(int skinCode)
		{
			return characterSkinList.FirstOrDefault(data => data.code == skinCode);
		}


		public List<CharacterSkinData> GetSkinDataList(int characterCode)
		{
			return (from data in characterSkinList
				where data.characterCode == characterCode
				select data).ToList<CharacterSkinData>();
		}


		public List<CharacterSkinData> GetDefaultSkinDataList()
		{
			return (from data in characterSkinList
				where data.purchaseType == SkinPurchaseType.FREE
				select data).ToList<CharacterSkinData>();
		}


		public List<CharacterSkinData> GetAllSkinDataList()
		{
			return characterSkinList;
		}
	}
}