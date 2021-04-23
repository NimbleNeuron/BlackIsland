using System;
using System.Collections.Generic;

namespace Blis.Common
{
	public abstract class CharacterSkill
	{
		private readonly int characterCode;


		private readonly ObjectType objectType;


		private readonly List<SequenceTime> sequenceTimer = new List<SequenceTime>();


		private bool isConcentrating;


		protected SkillCooldown skillCooldown;


		protected SkillEvolution skillEvolution;


		protected SkillLevel skillLevel;


		protected SkillSequencer skillSequencer;


		private Dictionary<SkillSlotIndex, SkillSlotSet> skillSlotMap;


		protected SkillStack skillStack;


		private SpecialSkillId specialSkillId;


		public CharacterSkill(int characterCode, ObjectType objectType)
		{
			this.characterCode = characterCode;
			this.objectType = objectType;
			skillLevel = new SkillLevel();
			skillCooldown = new SkillCooldown();
			skillStack = new SkillStack(skillLevel, characterCode, objectType);
			skillStack.InitSkillMaxStack();
			skillSequencer = new SkillSequencer();
			skillEvolution = new SkillEvolution();
		}


		public SkillEvolution SkillEvolution => skillEvolution;


		public SpecialSkillId SpecialSkillId => specialSkillId;


		
		
		public event Action<SkillSlotSet, MasteryType> OnSequenceTimeOver;


		protected void InitServer(SpecialSkillId specialSkillId)
		{
			skillSlotMap = GameDB.skill.GetDefaultSkillSet(characterCode, objectType);
			this.specialSkillId = specialSkillId;
		}


		public byte[] CreateSnapshot()
		{
			return Serializer.Default.Serialize<CharacterSkillSnapshot>(new CharacterSkillSnapshot
			{
				skillLevelSnapshot = skillLevel.CreateSnapshot(),
				skillCooldownSnapshot = skillCooldown.CreateSnapshot(),
				skillSequencerSnapshot = skillSequencer.CreateSnapshot(),
				skillEvolutionSnapshot = skillEvolution.CreateSnapshot(),
				skillStackSnapshot = skillStack.CreateSnapshot(),
				skillSlotMapSnapshot = skillSlotMap,
				specialSkillId = specialSkillId,
				isConcentrating = isConcentrating
			});
		}


		public byte[] CreateAllySnapshot()
		{
			return Serializer.Default.Serialize<CharacterSkillSnapshot>(new CharacterSkillSnapshot
			{
				skillLevelSnapshot = skillLevel.CreateSnapshot(),
				skillCooldownSnapshot = skillCooldown.CreateSnapshot(),
				skillStackSnapshot = skillStack.CreateSnapshot(),
				skillSlotMapSnapshot = skillSlotMap,
				specialSkillId = specialSkillId,
				isConcentrating = isConcentrating
			});
		}


		public byte[] CreatePlayerSnapshot()
		{
			return Serializer.Default.Serialize<CharacterSkillSnapshot>(new CharacterSkillSnapshot
			{
				skillSlotMapSnapshot = skillSlotMap,
				specialSkillId = specialSkillId,
				isConcentrating = isConcentrating
			});
		}


		public virtual bool Init(byte[] snapshot)
		{
			if (snapshot == null || snapshot.Length == 0)
			{
				return false;
			}

			CharacterSkillSnapshot characterSkillSnapshot =
				Serializer.Default.Deserialize<CharacterSkillSnapshot>(snapshot);
			skillLevel.Init(characterSkillSnapshot.skillLevelSnapshot);
			skillCooldown.Init(characterSkillSnapshot.skillCooldownSnapshot);
			skillStack.InitSnapshot(characterSkillSnapshot.skillStackSnapshot);
			skillStack.InitSkillMaxStack();
			if (characterSkillSnapshot.skillSequencerSnapshot != null)
			{
				skillSequencer.Init(characterSkillSnapshot.skillSequencerSnapshot);
			}

			skillEvolution.Init(characterSkillSnapshot.skillEvolutionSnapshot);
			skillSlotMap = characterSkillSnapshot.skillSlotMapSnapshot;
			specialSkillId = characterSkillSnapshot.specialSkillId;
			isConcentrating = characterSkillSnapshot.isConcentrating;
			return true;
		}


		public void InitWeaponSkill(MasteryType masteryType)
		{
			if (!masteryType.IsWeaponMastery())
			{
				return;
			}

			InitSkillSequence(masteryType);
			skillStack.InitSkillMaxStack(masteryType);
		}


		protected void InitSkillSequence()
		{
			Dictionary<SkillSlotSet, SkillSet>.KeyCollection allSkillSetKey =
				GameDB.skill.GetAllSkillSetKey(characterCode, objectType);
			if (allSkillSetKey == null)
			{
				return;
			}

			foreach (SkillSlotSet skillSlotSet in allSkillSetKey)
			{
				SkillSet skillSetData = GameDB.skill.GetSkillSetData(characterCode, objectType, skillSlotSet);
				skillSequencer.UpdateSkillSequence(skillSlotSet,
					skillSetData != null ? skillSetData.GetActiveMaxSequence() : 0);
			}

			SkillSet skillSetData2 = GameDB.skill.GetSkillSetData(specialSkillId);
			skillSequencer.UpdateSkillSequence(SkillSlotSet.SpecialSkill,
				skillSetData2 != null ? skillSetData2.GetActiveMaxSequence() : 0);
		}


		private void InitSkillSequence(MasteryType masteryType)
		{
			SkillSet skillSetData = GameDB.skill.GetSkillSetData(masteryType);
			skillSequencer.UpdateSkillSequence(SkillSlotSet.WeaponSkill, skillSetData.GetActiveMaxSequence());
		}


		public void UpgradeSkill(SkillSlotIndex skillSlotIndex)
		{
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill || skillSlotIndex == SkillSlotIndex.SpecialSkill)
			{
				return;
			}

			skillLevel.UpgradeSkill(skillSlotIndex);
			SkillSlotSet? skillSlotSet = GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet != null)
			{
				MaxStackSetting(skillSlotSet.Value);
			}
		}


		public void SetSkillLevel(SkillSlotIndex skillSlotIndex, int level)
		{
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				return;
			}

			if (level < 0)
			{
				return;
			}

			skillLevel.SetSkillLevel(skillSlotIndex, level);
			SkillSlotSet? skillSlotSet = GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet != null)
			{
				MaxStackSetting(skillSlotSet.Value);
			}
		}


		public int GetSkillLevel(SkillSlotIndex skillSlotIndex)
		{
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				return 0;
			}

			return skillLevel.GetSkillLevel(skillSlotIndex);
		}


		public void UpgradeSkill(MasteryType masteryType)
		{
			skillLevel.UpgradeSkill(masteryType);
			MaxStackSetting(masteryType);
		}


		public int GetSkillLevel(MasteryType masteryType)
		{
			return skillLevel.GetSkillLevel(masteryType);
		}


		public bool UseSkillEvolutionPoint(SkillSlotIndex skillSlotIndex, ref SkillEvolutionPointType usePointType,
			ref int remainPoint)
		{
			return skillEvolution.UsePoint(skillSlotIndex, 1, ref usePointType, ref remainPoint);
		}


		public void UpdateSkillEvolutionPoint(SkillEvolutionPointType pointType, int point)
		{
			skillEvolution.UpdatePoint(pointType, point);
		}


		public void EvolutionSkill(SkillSlotIndex skillSlotIndex)
		{
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill || skillSlotIndex == SkillSlotIndex.SpecialSkill)
			{
				return;
			}

			skillLevel.EvolutionSkill(skillSlotIndex);
		}


		public int GetSkillEvolutionLevel(SkillSlotIndex skillSlotIndex)
		{
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				return 0;
			}

			return skillLevel.GetEvolutionSkill(skillSlotIndex);
		}


		public void EvolutionSkill(MasteryType masteryType)
		{
			skillLevel.EvolutionSkill(masteryType);
		}


		public int GetSkillEvolutionLevel(MasteryType masteryType)
		{
			return skillLevel.GetEvolutionSkill(masteryType);
		}


		public bool CanSkillEvolution(SkillSlotIndex skillSlotIndex, SkillEvolutionData skillEvolutionData,
			MasteryType weaponMasteryType)
		{
			if (!SkillEvolution.IsHavePoint(skillSlotIndex))
			{
				return false;
			}

			if (skillSlotIndex != SkillSlotIndex.WeaponSkill)
			{
				return GetSkillEvolutionLevel(skillSlotIndex) < skillEvolutionData.maxEvolutionLevel;
			}

			return GetSkillEvolutionLevel(weaponMasteryType) < skillEvolutionData.maxEvolutionLevel;
		}


		public virtual void ResetCooldown()
		{
			skillCooldown.ResetCooldown();
			skillSequencer.ResetAll();
			sequenceTimer.Clear();
			skillStack.ResetCooldown();
		}


		public List<SkillSlotSet> GetPlayingSequenceSkillSlotSets()
		{
			return skillSequencer.GetPlayingSkillSlotSets();
		}


		public virtual bool UseSkillStack(SkillSlotSet skillSlotSet, MasteryType weaponType, SkillData skillData,
			float currentTime)
		{
			return skillStack.UseSkillStack(skillSlotSet, weaponType, skillData, currentTime);
		}


		protected virtual bool StartCooldown(SkillSlotSet skillSlotSet, float currentTime, float cooldown,
			float cooldownReduceRate, float cooldownModifyConst)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				return false;
			}

			skillSequencer.Reset(skillSlotSet);
			RemoveSequenceTimer(skillSlotSet, MasteryType.None);
			if (skillStack.IsStackableSkill(skillSlotSet, MasteryType.None))
			{
				skillStack.AddTimer(skillSlotSet, MasteryType.None);
			}

			skillCooldown.StartCooldown(skillSlotSet, currentTime,
				GameUtil.GetCooldown(cooldown, cooldownReduceRate) + cooldownModifyConst);
			return true;
		}


		public void ModifyCooldown(SkillSlotSet skillSlotSet, float currentTime, float modifyValue)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				return;
			}

			skillCooldown.ModifyCooldown(skillSlotSet, currentTime, modifyValue);
		}


		public void ModifyCooldown(SkillSlotIndex skillSlotIndex, float currentTime, float modifyValue)
		{
			SkillSlotSet? skillSlotSet = GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return;
			}

			ModifyCooldown(skillSlotSet.Value, currentTime, modifyValue);
		}


		public bool? IsHoldCooldown(SkillSlotIndex skillSlotIndex)
		{
			SkillSlotSet? skillSlotSet = GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return null;
			}

			return IsHoldCooldown(skillSlotSet.Value);
		}


		public bool IsHoldCooldown(SkillSlotSet skillSlotSet)
		{
			return skillSlotSet != SkillSlotSet.WeaponSkill && skillCooldown.IsHold(skillSlotSet);
		}


		public void SetHoldCooldown(SkillSlotIndex skillSlotIndex, bool isHold)
		{
			SkillSlotSet? skillSlotSet = GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return;
			}

			SetHoldCooldown(skillSlotSet.Value, isHold);
		}


		public void SetHoldCooldown(SkillSlotSet skillSlotSet, bool isHold)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				return;
			}

			skillCooldown.SetHoldCooldown(skillSlotSet, isHold);
		}


		public bool? CheckCooldown(SkillSlotIndex skillSlotIndex, float currentTime)
		{
			SkillSlotSet? skillSlotSet = GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return null;
			}

			return CheckCooldown(skillSlotSet.Value, currentTime);
		}


		public bool CheckCooldown(SkillSlotSet skillSlotSet, float currentTime)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				return false;
			}

			foreach (SequenceTime sequenceTime in sequenceTimer)
			{
				if (skillSlotSet == sequenceTime.SkillSlotSet)
				{
					if (sequenceTime.CanStartTime > currentTime)
					{
						return false;
					}

					break;
				}
			}

			if (skillStack.IsStackableSkill(skillSlotSet, MasteryType.None))
			{
				return skillStack.CanUseStackSkill(skillSlotSet, MasteryType.None, currentTime);
			}

			return skillCooldown.CheckCooldown(skillSlotSet, currentTime);
		}


		public bool CheckOnlySkillCoolDown(SkillSlotSet skillSlotSet, float currentTime)
		{
			return skillCooldown.CheckCooldown(skillSlotSet, currentTime);
		}


		public float GetMaxCooldown(SkillSlotSet skillSlotSet)
		{
			return skillCooldown.GetMaxCooldown(skillSlotSet);
		}


		public float GetCooldown(SkillSlotSet skillSlotSet, float currentTime)
		{
			return skillCooldown.GetCooldown(skillSlotSet, currentTime);
		}


		public virtual bool StartCooldown(MasteryType masteryType, float currentTime, float cooldown)
		{
			skillSequencer.Reset(SkillSlotSet.WeaponSkill);
			RemoveSequenceTimer(SkillSlotSet.WeaponSkill, masteryType);
			if (skillStack.IsStackableSkill(SkillSlotSet.WeaponSkill, masteryType))
			{
				skillStack.AddTimer(SkillSlotSet.WeaponSkill, masteryType);
			}

			skillCooldown.StartCooldown(masteryType, currentTime, cooldown);
			return true;
		}


		public void ModifyCooldown(MasteryType masteryType, float currentTime, float modifyValue)
		{
			skillCooldown.ModifyCooldown(masteryType, currentTime, modifyValue);
		}


		public bool IsHoldCooldown(MasteryType masteryType)
		{
			return skillCooldown.IsHold(masteryType);
		}


		public void SetHoldCooldown(MasteryType masteryType, bool isHold)
		{
			skillCooldown.SetHoldCooldown(masteryType, isHold);
		}


		public virtual bool CheckCooldown(MasteryType masteryType, float currentTime)
		{
			if (skillStack.IsStackableSkill(SkillSlotSet.WeaponSkill, masteryType))
			{
				return skillStack.CanUseStackSkill(SkillSlotSet.WeaponSkill, masteryType, currentTime);
			}

			foreach (SequenceTime sequenceTime in sequenceTimer)
			{
				if (SkillSlotSet.WeaponSkill == sequenceTime.SkillSlotSet && masteryType == sequenceTime.MasteryType)
				{
					if (sequenceTime.CanStartTime > currentTime)
					{
						return false;
					}

					break;
				}
			}

			return skillCooldown.CheckCooldown(masteryType, currentTime);
		}


		public float GetCooldown(MasteryType masteryType, float currentTime)
		{
			return skillCooldown.GetCooldown(masteryType, currentTime);
		}


		public int GetSkillSequence(SkillSlotSet skillSlotSet)
		{
			return skillSequencer.GetSequence(skillSlotSet);
		}


		public virtual void SetSkillSequence(SkillSlotSet skillSlotSet, int seq)
		{
			skillSequencer.SetSequence(skillSlotSet, seq);
		}


		public virtual void NextSequence(SkillSlotSet skillSlotSet)
		{
			skillSequencer.Next(skillSlotSet);
		}


		public virtual void ResetSkillSequence(SkillSlotSet skillSlotSet)
		{
			skillSequencer.Reset(skillSlotSet);
		}


		public virtual bool IsSingleSequence(SkillSlotSet skillSlotSet)
		{
			return skillSequencer.IsSingleSequence(skillSlotSet);
		}


		public virtual bool IsLastSequence(SkillSlotSet skillSlotSet)
		{
			return skillSequencer.IsLastSequence(skillSlotSet);
		}


		public virtual int GetMaxSequence(SkillSlotSet skillSlotSet)
		{
			return skillSequencer.GetMaxSequence(skillSlotSet);
		}


		public virtual bool IsFirstSequence(SkillSlotSet skillSlotSet)
		{
			return skillSequencer.GetSequence(skillSlotSet) == 0;
		}


		public void StartConcentration()
		{
			isConcentrating = true;
		}


		public void EndConcentration()
		{
			isConcentrating = false;
		}


		public bool IsConcentrating()
		{
			return isConcentrating;
		}


		public void SetSequenceDuration(SkillSlotIndex skillSlotIndex, MasteryType masteryType, float duration,
			float sequenceCooldown, float currentTime)
		{
			SkillSlotSet? skillSlotSet = GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return;
			}

			SetSequenceDuration(skillSlotSet.Value, masteryType, duration, sequenceCooldown, currentTime);
		}


		public void SetSequenceDuration(SkillSlotSet skillSlotSet, MasteryType masteryType, float duration,
			float sequenceCooldown, float currentTime)
		{
			if (duration <= 0f && sequenceCooldown <= 0f)
			{
				return;
			}

			float canStartTime = currentTime + sequenceCooldown;
			float num;
			if (duration <= 0f)
			{
				num = -1f;
			}
			else
			{
				num = duration + currentTime + sequenceCooldown;
			}

			bool flag = false;
			for (int i = 0; i < sequenceTimer.Count; i++)
			{
				if (sequenceTimer[i].SkillSlotSet == skillSlotSet)
				{
					SequenceTime value = sequenceTimer[i];
					value.SetEndTime = num;
					sequenceTimer[i] = value;
					flag = true;
					break;
				}
			}

			if (!flag)
			{
				sequenceTimer.Add(new SequenceTime(skillSlotSet, masteryType, num, canStartTime));
			}
		}


		public void StopSequenceTimer(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			if (!IsPlayingSequenceTimer(skillSlotSet, masteryType))
			{
				return;
			}

			Action<SkillSlotSet, MasteryType> onSequenceTimeOver = OnSequenceTimeOver;
			if (onSequenceTimeOver != null)
			{
				onSequenceTimeOver(skillSlotSet, masteryType);
			}

			RemoveSequenceTimer(skillSlotSet, masteryType);
		}


		public void RemoveSequenceTimer(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			for (int i = sequenceTimer.Count - 1; i >= 0; i--)
			{
				if (skillSlotSet == SkillSlotSet.WeaponSkill)
				{
					if (sequenceTimer[i].SkillSlotSet == skillSlotSet && sequenceTimer[i].MasteryType == masteryType)
					{
						sequenceTimer.RemoveAt(i);
					}
				}
				else if (sequenceTimer[i].SkillSlotSet == skillSlotSet)
				{
					sequenceTimer.RemoveAt(i);
				}
			}
		}


		public bool IsPlayingSequenceTimer(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			for (int i = 0; i < sequenceTimer.Count; i++)
			{
				if (skillSlotSet == SkillSlotSet.WeaponSkill)
				{
					if (sequenceTimer[i].SkillSlotSet == skillSlotSet && sequenceTimer[i].MasteryType == masteryType)
					{
						return true;
					}
				}
				else if (sequenceTimer[i].SkillSlotSet == skillSlotSet)
				{
					return true;
				}
			}

			return false;
		}


		public void UpdateSequenceTimer(float currentTime, MasteryType masteryType)
		{
			for (int i = sequenceTimer.Count - 1; i >= 0; i--)
			{
				SequenceTime item = sequenceTimer[i];
				if (item.EndTime <= 0f && item.CanStartTime <= currentTime)
				{
					sequenceTimer.RemoveAt(i);
				}
				else if (item.EndTime <= currentTime)
				{
					Action<SkillSlotSet, MasteryType> onSequenceTimeOver = OnSequenceTimeOver;
					if (onSequenceTimeOver != null)
					{
						onSequenceTimeOver(item.SkillSlotSet, masteryType);
					}

					if (sequenceTimer.Contains(item))
					{
						sequenceTimer.Remove(item);
					}
				}
			}
		}


		public bool IsHaveSequenceTimer()
		{
			return 0 < sequenceTimer.Count;
		}


		public Dictionary<SkillSlotIndex, int> GetCharacterSkillLevels()
		{
			return new Dictionary<SkillSlotIndex, int>
			{
				{
					SkillSlotIndex.Passive,
					skillLevel.GetSkillLevel(SkillSlotIndex.Passive)
				},
				{
					SkillSlotIndex.Active1,
					skillLevel.GetSkillLevel(SkillSlotIndex.Active1)
				},
				{
					SkillSlotIndex.Active2,
					skillLevel.GetSkillLevel(SkillSlotIndex.Active2)
				},
				{
					SkillSlotIndex.Active3,
					skillLevel.GetSkillLevel(SkillSlotIndex.Active3)
				},
				{
					SkillSlotIndex.Active4,
					skillLevel.GetSkillLevel(SkillSlotIndex.Active4)
				}
			};
		}


		public abstract void UpdateStackSkillTimer(float currentTime);


		protected bool IsMaxStack(SkillSlotSet skillSlotSet)
		{
			return skillStack.IsMaxStack(skillSlotSet, MasteryType.None);
		}


		protected bool IsMaxStack(MasteryType masteryType)
		{
			return skillStack.IsMaxStack(SkillSlotSet.WeaponSkill, masteryType);
		}


		protected virtual void MaxStackSetting(SkillSlotSet skillSlotSet)
		{
			skillStack.MaxStackSetting(skillSlotSet, MasteryType.None);
		}


		protected virtual void MaxStackSetting(MasteryType masteryType)
		{
			skillStack.MaxStackSetting(SkillSlotSet.WeaponSkill, masteryType);
		}


		public float GetStackUseIntervalTime(SkillSlotSet skillSlotSet, float currentTime)
		{
			return skillStack.GetIntervalTime(skillSlotSet, MasteryType.None, currentTime);
		}


		public float GetStackUseIntervalTime(MasteryType masteryType, float currentTime)
		{
			return skillStack.GetIntervalTime(SkillSlotSet.WeaponSkill, masteryType, currentTime);
		}


		public SkillSlotSet? GetSkillSlotSet(SkillSlotIndex skillSlotIndex)
		{
			if (skillSlotIndex == SkillSlotIndex.SpecialSkill)
			{
				return SkillSlotSet.SpecialSkill;
			}

			if (skillSlotMap == null)
			{
				return null;
			}

			if (!skillSlotMap.ContainsKey(skillSlotIndex))
			{
				return null;
			}

			return skillSlotMap[skillSlotIndex];
		}


		public SkillSlotIndex? GetSkillSlotIndex(SkillSlotSet skillSlotSet)
		{
			if (skillSlotSet == SkillSlotSet.SpecialSkill)
			{
				return SkillSlotIndex.SpecialSkill;
			}

			if (skillSlotMap == null)
			{
				return null;
			}

			foreach (KeyValuePair<SkillSlotIndex, SkillSlotSet> keyValuePair in skillSlotMap)
			{
				if (keyValuePair.Value == skillSlotSet)
				{
					return keyValuePair.Key;
				}
			}

			return null;
		}


		public bool SwitchSkillSet(SkillSlotIndex skillSlotIndex, SkillSlotSet skillSlotSet)
		{
			if (!skillSlotSet.IsValidRange(skillSlotIndex))
			{
				return false;
			}

			if (skillSlotMap == null)
			{
				return false;
			}

			if (skillSlotMap[skillSlotIndex] == skillSlotSet)
			{
				return false;
			}

			skillSlotMap[skillSlotIndex] = skillSlotSet;
			return true;
		}


		protected struct SequenceTime
		{
			public SkillSlotSet SkillSlotSet { get; }


			public MasteryType MasteryType { get; }


			public float EndTime => endTime;


			public float CanStartTime { get; }


			
			public float SetEndTime {
				set => endTime = value;
			}


			public SequenceTime(SkillSlotSet skillSlotSet, MasteryType masteryType, float endTime, float canStartTime)
			{
				SkillSlotSet = skillSlotSet;
				MasteryType = masteryType;
				this.endTime = endTime;
				CanStartTime = canStartTime;
			}


			private float endTime;
		}
	}
}