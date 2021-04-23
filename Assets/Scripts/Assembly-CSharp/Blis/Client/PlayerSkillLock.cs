using System.Collections.Generic;
using Blis.Common;

namespace Blis.Client
{
	public class PlayerSkillLock
	{
		private readonly Dictionary<SkillSlotSet, List<long>> lockAggressiveSkillByState =
			new Dictionary<SkillSlotSet, List<long>>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>
				.Instance);


		private readonly Dictionary<SkillSlotSet, List<long>> lockAllSkillByState =
			new Dictionary<SkillSlotSet, List<long>>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>
				.Instance);


		private readonly Dictionary<SkillSlotSet, List<long>> lockMovementSkillByState =
			new Dictionary<SkillSlotSet, List<long>>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>
				.Instance);


		private readonly LocalPlayerCharacter myCharacter;


		private readonly Dictionary<SkillSlotSet, int> skillSetLockCount =
			new Dictionary<SkillSlotSet, int>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>.Instance);


		private readonly Dictionary<SpecialSkillId, int> specialSkillLockCount =
			new Dictionary<SpecialSkillId, int>(SingletonComparerEnum<SpecialSkillIdComparer, SpecialSkillId>.Instance);

		public PlayerSkillLock(LocalPlayerCharacter myCharacter)
		{
			this.myCharacter = myCharacter;
		}


		public bool IsLock(SkillSlotSet skillSlotSet, bool canCastingWhileCCState, bool isAggressiveSkill,
			bool isMovementSkill)
		{
			if (!myCharacter.IsEquippedWeapon())
			{
				return true;
			}

			if (skillSetLockCount.ContainsKey(skillSlotSet) && 0 < skillSetLockCount[skillSlotSet])
			{
				return true;
			}

			if (!canCastingWhileCCState)
			{
				if (lockAllSkillByState.ContainsKey(skillSlotSet) && 0 < lockAllSkillByState[skillSlotSet].Count)
				{
					return true;
				}

				if (isAggressiveSkill && lockAggressiveSkillByState.ContainsKey(skillSlotSet) &&
				    0 < lockAggressiveSkillByState[skillSlotSet].Count)
				{
					return true;
				}

				if (isMovementSkill && lockMovementSkillByState.ContainsKey(skillSlotSet) &&
				    0 < lockMovementSkillByState[skillSlotSet].Count)
				{
					return true;
				}
			}

			return false;
		}


		public bool IsSpecialSkillLock(SpecialSkillId specialSkillId)
		{
			return specialSkillLockCount.ContainsKey(specialSkillId) && 0 < specialSkillLockCount[specialSkillId];
		}


		public void LockByState(SkillSlotSet skillSlotSet, bool isLock, long characterStateHashCode, bool lockSkill,
			bool lockAggressiveSkill, bool lockMovementSkill)
		{
			if (isLock)
			{
				LockByState(skillSlotSet, characterStateHashCode, lockSkill, lockAggressiveSkill, lockMovementSkill);
				return;
			}

			UnlockByState(skillSlotSet, characterStateHashCode, lockSkill, lockAggressiveSkill, lockMovementSkill);
		}


		private void LockByState(SkillSlotSet skillSlotSet, long characterStateHashCode, bool lockSkill,
			bool lockAggressiveSkill, bool lockMovementSkill)
		{
			if (lockSkill)
			{
				if (!lockAllSkillByState.ContainsKey(skillSlotSet))
				{
					lockAllSkillByState[skillSlotSet] = new List<long>();
				}

				lockAllSkillByState[skillSlotSet].Add(characterStateHashCode);
			}

			if (lockAggressiveSkill)
			{
				if (!lockAggressiveSkillByState.ContainsKey(skillSlotSet))
				{
					lockAggressiveSkillByState[skillSlotSet] = new List<long>();
				}

				lockAggressiveSkillByState[skillSlotSet].Add(characterStateHashCode);
			}

			if (lockMovementSkill)
			{
				if (!lockMovementSkillByState.ContainsKey(skillSlotSet))
				{
					lockMovementSkillByState[skillSlotSet] = new List<long>();
				}

				lockMovementSkillByState[skillSlotSet].Add(characterStateHashCode);
			}
		}


		private void UnlockByState(SkillSlotSet skillSlotSet, long characterStateHashCode, bool lockSkill,
			bool lockAggressiveSkill, bool lockMovementSkill)
		{
			if (lockSkill && lockAllSkillByState.ContainsKey(skillSlotSet))
			{
				lockAllSkillByState[skillSlotSet].Remove(characterStateHashCode);
			}

			if (lockAggressiveSkill && lockAggressiveSkillByState.ContainsKey(skillSlotSet))
			{
				lockAggressiveSkillByState[skillSlotSet].Remove(characterStateHashCode);
			}

			if (lockMovementSkill && lockMovementSkillByState.ContainsKey(skillSlotSet))
			{
				lockMovementSkillByState[skillSlotSet].Remove(characterStateHashCode);
			}
		}


		public void Lock(SpecialSkillId specialSkillId, bool isLock)
		{
			if (!specialSkillLockCount.ContainsKey(specialSkillId))
			{
				specialSkillLockCount.Add(specialSkillId, 0);
			}

			Dictionary<SpecialSkillId, int> dictionary = specialSkillLockCount;
			dictionary[specialSkillId] += isLock ? 1 : -1;
		}


		public void Lock(SkillSlotSet skillSlotSet, bool isLock)
		{
			if (!skillSetLockCount.ContainsKey(skillSlotSet))
			{
				skillSetLockCount[skillSlotSet] = 0;
			}

			Dictionary<SkillSlotSet, int> dictionary = skillSetLockCount;
			dictionary[skillSlotSet] += isLock ? 1 : -1;
		}
	}
}