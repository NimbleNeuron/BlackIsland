using System;
using System.Collections.Generic;

namespace Blis.Common
{
	public class SkillEvolution
	{
		private Dictionary<int, int> earnedPointMap = new Dictionary<int, int>();


		private Dictionary<SkillEvolutionPointType, int> pointMap =
			new Dictionary<SkillEvolutionPointType, int>(
				SingletonComparerEnum<SkillEvolutionPointTypeComparer, SkillEvolutionPointType>.Instance);


		private List<StateStackSkillEvolutionPointData> stateStackPointDataList =
			new List<StateStackSkillEvolutionPointData>();


		private List<WeaponCraftSkillEvolutionPointData> weaponCraftPointDataList =
			new List<WeaponCraftSkillEvolutionPointData>();

		public SkillEvolutionSnapshot CreateSnapshot()
		{
			return new SkillEvolutionSnapshot
			{
				earnedPointMap = earnedPointMap,
				pointMap = pointMap
			};
		}


		public void Init(SkillEvolutionSnapshot snapshot)
		{
			if (snapshot == null)
			{
				return;
			}

			earnedPointMap = snapshot.earnedPointMap;
			pointMap = snapshot.pointMap;
		}


		public int GetPoint(SkillEvolutionPointType pointType)
		{
			if (pointMap.ContainsKey(pointType))
			{
				return pointMap[pointType];
			}

			return 0;
		}


		public Dictionary<SkillEvolutionPointType, int> GetPoints()
		{
			return pointMap;
		}


		public void OnCraftingItem(int characterCode, ItemData itemData,
			Action<SkillEvolutionPointType, int> updatePoint)
		{
			if (itemData.itemType == ItemType.Weapon)
			{
				if (weaponCraftPointDataList.Count == 0)
				{
					GameDB.skill.TryGetEvolutionPointData<WeaponCraftSkillEvolutionPointData>(characterCode,
						ref weaponCraftPointDataList);
				}

				foreach (WeaponCraftSkillEvolutionPointData weaponCraftSkillEvolutionPointData in
					weaponCraftPointDataList)
				{
					if (weaponCraftSkillEvolutionPointData.masteryType.Equals(itemData.GetMasteryType()) &&
					    weaponCraftSkillEvolutionPointData.itemGrade.Equals(itemData.itemGrade) &&
					    (0 >= weaponCraftSkillEvolutionPointData.itemCode ||
					     weaponCraftSkillEvolutionPointData.itemCode.Equals(itemData.code)))
					{
						if (CheckLimitPoint(weaponCraftSkillEvolutionPointData))
						{
							IncreasePoint(weaponCraftSkillEvolutionPointData);
							if (updatePoint != null)
							{
								updatePoint(weaponCraftSkillEvolutionPointData.pointType,
									pointMap[weaponCraftSkillEvolutionPointData.pointType]);
							}
						}

						break;
					}
				}
			}
		}


		public void OnStateStack(int characterCode, int stateGroupCode, int stackCount,
			Action<SkillEvolutionPointType, int> updatePoint)
		{
			if (stateStackPointDataList.Count == 0)
			{
				GameDB.skill.TryGetEvolutionPointData<StateStackSkillEvolutionPointData>(characterCode,
					ref stateStackPointDataList);
			}

			foreach (StateStackSkillEvolutionPointData stateStackSkillEvolutionPointData in stateStackPointDataList)
			{
				if (stateStackSkillEvolutionPointData.stateGroupCode.Equals(stateGroupCode) &&
				    stackCount >= stateStackSkillEvolutionPointData.stackCountCondition &&
				    CheckLimitPoint(stateStackSkillEvolutionPointData))
				{
					IncreasePoint(stateStackSkillEvolutionPointData);
					if (updatePoint != null)
					{
						updatePoint(stateStackSkillEvolutionPointData.pointType,
							pointMap[stateStackSkillEvolutionPointData.pointType]);
					}
				}
			}
		}


		private bool CheckLimitPoint(SkillEvolutionPointData pointData)
		{
			int num = 0;
			if (earnedPointMap.ContainsKey(pointData.code))
			{
				num = earnedPointMap[pointData.code];
			}

			return num < pointData.limitPoint;
		}


		private void IncreasePoint(SkillEvolutionPointData pointData)
		{
			if (!earnedPointMap.ContainsKey(pointData.code))
			{
				earnedPointMap.Add(pointData.code, 0);
			}

			Dictionary<int, int> dictionary = earnedPointMap;
			int code = pointData.code;
			int num = dictionary[code];
			dictionary[code] = num + 1;
			if (!pointMap.ContainsKey(pointData.pointType))
			{
				pointMap.Add(pointData.pointType, 0);
			}

			Dictionary<SkillEvolutionPointType, int> dictionary2 = pointMap;
			SkillEvolutionPointType pointType = pointData.pointType;
			num = dictionary2[pointType];
			dictionary2[pointType] = num + 1;
		}


		public bool AnyPoint()
		{
			int num = 0;
			foreach (KeyValuePair<SkillEvolutionPointType, int> keyValuePair in pointMap)
			{
				num += keyValuePair.Value;
			}

			return 0 < num;
		}


		public bool IsHavePoint(SkillSlotIndex skillSlotIndex)
		{
			if (skillSlotIndex == SkillSlotIndex.Attack)
			{
				return false;
			}

			if (skillSlotIndex != SkillSlotIndex.WeaponSkill)
			{
				return IsHavePoint(skillSlotIndex.ToEvolutionPointType()) ||
				       IsHavePoint(SkillEvolutionPointType.CharacterSkill);
			}

			return IsHavePoint(skillSlotIndex.ToEvolutionPointType());
		}


		public bool IsHavePoint(SkillEvolutionPointType pointType)
		{
			return 0 < GetPoint(pointType);
		}


		public bool UsePoint(SkillSlotIndex skillSlotIndex, int usePoint, ref SkillEvolutionPointType usePointType,
			ref int remainPoint)
		{
			if (skillSlotIndex == SkillSlotIndex.Attack)
			{
				return false;
			}

			if (skillSlotIndex != SkillSlotIndex.WeaponSkill)
			{
				return UsePoint(skillSlotIndex.ToEvolutionPointType(), usePoint, ref usePointType, ref remainPoint) ||
				       UsePoint(SkillEvolutionPointType.CharacterSkill, usePoint, ref usePointType, ref remainPoint);
			}

			return UsePoint(SkillEvolutionPointType.WeaponSkill, usePoint, ref usePointType, ref remainPoint);
		}


		private bool UsePoint(SkillEvolutionPointType pointType, int usePoint, ref SkillEvolutionPointType usePointType,
			ref int remainPoint)
		{
			if (!pointMap.ContainsKey(pointType))
			{
				return false;
			}

			if (pointMap[pointType] < usePoint)
			{
				return false;
			}

			Dictionary<SkillEvolutionPointType, int> dictionary = pointMap;
			dictionary[pointType] -= usePoint;
			usePointType = pointType;
			remainPoint = pointMap[pointType];
			return true;
		}


		public void UpdatePoint(SkillEvolutionPointType pointType, int point)
		{
			pointMap[pointType] = point;
		}
	}
}