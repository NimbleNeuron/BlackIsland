using System;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public class LocalCharacterSkill : CharacterSkill
	{
		private readonly SkillSequencer localSkillSequencer;

		public LocalCharacterSkill(int characterCode, ObjectType objectType) : base(characterCode, objectType)
		{
			localSkillSequencer = new SkillSequencer();
		}


		public override bool Init(byte[] snapshot)
		{
			if (!base.Init(snapshot))
			{
				return false;
			}

			CharacterSkillSnapshot characterSkillSnapshot =
				Serializer.Default.Deserialize<CharacterSkillSnapshot>(snapshot);
			if (characterSkillSnapshot.skillSequencerSnapshot != null)
			{
				localSkillSequencer.Init(characterSkillSnapshot.skillSequencerSnapshot);
			}

			InitSkillSequence();
			return true;
		}


		public bool StartCooldown(SkillSlotSet skillSlotSet, float currentTime, float cooldown)
		{
			bool flag = skillStack.IsChargingSkillStack(skillSlotSet, MasteryType.None);
			bool flag2 = base.StartCooldown(skillSlotSet, currentTime, cooldown, 0f, 0f);
			if (flag2)
			{
				if (flag)
				{
					skillStack.RemoveTimer(skillSlotSet, MasteryType.None);
					skillStack.SkillStackChange(skillSlotSet, MasteryType.None, 1);
				}

				localSkillSequencer.Reset(skillSlotSet);
			}

			return flag2;
		}


		public override bool StartCooldown(MasteryType masteryType, float currentTime, float cooldown)
		{
			if (base.StartCooldown(masteryType, currentTime, cooldown))
			{
				if (skillStack.IsChargingSkillStack(SkillSlotSet.WeaponSkill, masteryType))
				{
					skillStack.RemoveTimer(SkillSlotSet.WeaponSkill, masteryType);
					skillStack.SkillStackChange(SkillSlotSet.WeaponSkill, masteryType, 1);
				}

				localSkillSequencer.Reset(SkillSlotSet.WeaponSkill);
			}

			return base.StartCooldown(masteryType, currentTime, cooldown);
		}


		public override bool UseSkillStack(SkillSlotSet skillSlotSet, MasteryType weaponType, SkillData skillData,
			float currentTime)
		{
			if (!skillStack.IsStackableSkill(skillSlotSet, weaponType))
			{
				return false;
			}

			bool result = base.UseSkillStack(skillSlotSet, weaponType, skillData, currentTime);
			if (skillData != null && GameDB.skill.GetSkillGroupData(skillData.group).stackAble)
			{
				float stackUseIntervalTime = skillData.stackUseIntervalTime;
				if (stackUseIntervalTime > 0f)
				{
					if (GetSkillStack(skillSlotSet, weaponType) >= 1)
					{
						MonoBehaviourInstance<GameUI>.inst.SkillHud.SetStackIntervalTime(skillSlotSet,
							stackUseIntervalTime, stackUseIntervalTime);
						return result;
					}

					bool flag;
					if (skillSlotSet == SkillSlotSet.WeaponSkill)
					{
						flag = GetCooldown(weaponType, currentTime) < stackUseIntervalTime;
					}
					else
					{
						flag = GetCooldown(skillSlotSet, currentTime) < stackUseIntervalTime;
					}

					if (flag)
					{
						MonoBehaviourInstance<GameUI>.inst.SkillHud.SetStackIntervalTime(skillSlotSet,
							stackUseIntervalTime, stackUseIntervalTime);
					}
				}
			}

			return result;
		}


		public override void UpdateStackSkillTimer(float currentTime)
		{
			for (int i = skillStack.GetTimerCount(true) - 1; i >= 0; i--)
			{
				MasteryType weaponSkillTimer = skillStack.GetWeaponSkillTimer(i);
				if (skillCooldown.CheckCooldown(weaponSkillTimer, currentTime))
				{
					if (!IsMaxStack(weaponSkillTimer))
					{
						skillStack.SkillStackChange(SkillSlotSet.WeaponSkill, weaponSkillTimer, 1);
					}

					skillStack.RemoveTimer(SkillSlotSet.WeaponSkill, weaponSkillTimer);
				}
			}

			for (int i = skillStack.GetTimerCount(false) - 1; i >= 0; i--)
			{
				SkillSlotSet characterSkillTimer = skillStack.GetCharacterSkillTimer(i);
				if (skillCooldown.CheckCooldown(characterSkillTimer, currentTime))
				{
					if (!IsMaxStack(characterSkillTimer))
					{
						skillStack.SkillStackChange(characterSkillTimer, MasteryType.None, 1);
					}

					skillStack.RemoveTimer(characterSkillTimer, MasteryType.None);
				}
			}
		}


		public int GetSkillStack(SkillSlotSet skillSlotSet, MasteryType weaponType)
		{
			return skillStack.GetSkillStack(skillSlotSet, weaponType);
		}


		public void SetSkillStackValueChangeListener(Action<SkillSlotSet, MasteryType, int> listener)
		{
			skillStack.OnSkillStackValueChange = listener;
		}


		public void LocalSetSkillSequence(SkillSlotSet skillSlotSet, int seq)
		{
			localSkillSequencer.SetSequence(skillSlotSet, seq);
		}


		public void LocalNextSequence(SkillSlotSet skillSlotSet)
		{
			localSkillSequencer.Next(skillSlotSet);
		}


		public void LocalResetSkillSequence(SkillSlotSet skillSlotSet)
		{
			localSkillSequencer.Reset(skillSlotSet);
		}


		public override void SetSkillSequence(SkillSlotSet skillSlotSet, int seq)
		{
			base.SetSkillSequence(skillSlotSet, seq);
			localSkillSequencer.SetSequence(skillSlotSet, seq);
		}


		public override void NextSequence(SkillSlotSet skillSlotSet)
		{
			base.NextSequence(skillSlotSet);
			localSkillSequencer.Next(skillSlotSet);
		}


		public override void ResetSkillSequence(SkillSlotSet skillSlotSet)
		{
			base.ResetSkillSequence(skillSlotSet);
			localSkillSequencer.Reset(skillSlotSet);
		}


		public int GetLocalSkillSequence(SkillSlotSet skillSlotSet)
		{
			return localSkillSequencer.GetSequence(skillSlotSet);
		}


		public override void ResetCooldown()
		{
			base.ResetCooldown();
			localSkillSequencer.ResetAll();
		}
	}
}