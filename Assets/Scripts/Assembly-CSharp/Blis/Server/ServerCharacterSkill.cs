using System;
using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	public class ServerCharacterSkill : CharacterSkill
	{
		
		
		
		public event Action<SkillSlotSet, MasteryType> OnStackSkillNeedCharge;

		
		public ServerCharacterSkill(int characterCode, ObjectType objectType, SpecialSkillId specialSkillId) : base(characterCode, objectType)
		{
			base.InitServer(specialSkillId);
			base.InitSkillSequence();
		}

		
		public bool StartCooldown(SkillSlotSet skillSlotSet, float currentTime, float cooldown, float cooldownReduceRate)
		{
			if (this.skillStack.IsChargingSkillStack(skillSlotSet, MasteryType.None))
			{
				return false;
			}
			float reservationCooldown = this.GetReservationCooldown(skillSlotSet);
			this.RemoveReservationCooldown(skillSlotSet);
			return base.StartCooldown(skillSlotSet, currentTime, cooldown, cooldownReduceRate, reservationCooldown);
		}

		
		public override bool StartCooldown(MasteryType masteryType, float currentTime, float cooldown)
		{
			return !this.skillStack.IsChargingSkillStack(SkillSlotSet.WeaponSkill, masteryType) && base.StartCooldown(masteryType, currentTime, cooldown);
		}

		
		public override void UpdateStackSkillTimer(float currentTime)
		{
			Action action = null;
			for (int i = this.skillStack.GetTimerCount(true) - 1; i >= 0; i--)
			{
				MasteryType masteryType = this.skillStack.GetWeaponSkillTimer(i);
				if (this.skillCooldown.CheckCooldown(masteryType, currentTime))
				{
					if (!base.IsMaxStack(masteryType))
					{
						this.skillStack.SkillStackChange(SkillSlotSet.WeaponSkill, masteryType, 1);
					}
					this.skillStack.RemoveTimer(SkillSlotSet.WeaponSkill, masteryType);
					if (!base.IsMaxStack(masteryType) && this.OnStackSkillNeedCharge != null)
					{
						action = (Action)Delegate.Combine(action, new Action(delegate()
						{
							this.OnStackSkillNeedCharge(SkillSlotSet.WeaponSkill, masteryType);
						}));
					}
				}
			}
			for (int i = this.skillStack.GetTimerCount(false) - 1; i >= 0; i--)
			{
				SkillSlotSet skillSlotIndex = this.skillStack.GetCharacterSkillTimer(i);
				if (this.skillCooldown.CheckCooldown(skillSlotIndex, currentTime))
				{
					if (!base.IsMaxStack(skillSlotIndex))
					{
						this.skillStack.SkillStackChange(skillSlotIndex, MasteryType.None, 1);
					}
					this.skillStack.RemoveTimer(skillSlotIndex, MasteryType.None);
					if (!base.IsMaxStack(skillSlotIndex) && this.OnStackSkillNeedCharge != null)
					{
						action = (Action)Delegate.Combine(action, new Action(delegate()
						{
							this.OnStackSkillNeedCharge(skillSlotIndex, MasteryType.None);
						}));
					}
				}
			}
			if (action != null)
			{
				action();
			}
		}

		
		protected override void MaxStackSetting(SkillSlotSet skillSlotSet)
		{
			base.MaxStackSetting(skillSlotSet);
			if (this.skillStack.IsStackableSkill(skillSlotSet, MasteryType.None) && !base.IsMaxStack(skillSlotSet))
			{
				Action<SkillSlotSet, MasteryType> onStackSkillNeedCharge = this.OnStackSkillNeedCharge;
				if (onStackSkillNeedCharge == null)
				{
					return;
				}
				onStackSkillNeedCharge(skillSlotSet, MasteryType.None);
			}
		}

		
		protected override void MaxStackSetting(MasteryType masteryType)
		{
			base.MaxStackSetting(masteryType);
			if (this.skillStack.IsStackableSkill(SkillSlotSet.WeaponSkill, masteryType) && !base.IsMaxStack(masteryType))
			{
				Action<SkillSlotSet, MasteryType> onStackSkillNeedCharge = this.OnStackSkillNeedCharge;
				if (onStackSkillNeedCharge == null)
				{
					return;
				}
				onStackSkillNeedCharge(SkillSlotSet.WeaponSkill, masteryType);
			}
		}

		
		public override void ResetCooldown()
		{
			base.ResetCooldown();
			this.reservationCooldownMap.Clear();
		}

		
		public float GetReservationSequenceCooldown(SkillSlotSet skillSlotSet)
		{
			if (this.reservationSquenceCooldownMap.ContainsKey(skillSlotSet))
			{
				float result = this.reservationSquenceCooldownMap[skillSlotSet];
				this.reservationSquenceCooldownMap.Remove(skillSlotSet);
				return result;
			}
			return 0f;
		}

		
		public void SetReservationSequenceCooldown(SkillSlotSet skillSlotSet, float reservationCooldown)
		{
			if (this.reservationSquenceCooldownMap.ContainsKey(skillSlotSet))
			{
				this.reservationSquenceCooldownMap[skillSlotSet] = reservationCooldown;
				return;
			}
			this.reservationSquenceCooldownMap.Add(skillSlotSet, reservationCooldown);
		}

		
		public bool IsChargingSkillStack(SkillSlotSet skillSlotSet)
		{
			return this.skillStack.IsChargingSkillStack(skillSlotSet, MasteryType.None);
		}

		
		public bool IsChargingSkillStack(MasteryType masteryType)
		{
			return this.skillStack.IsChargingSkillStack(SkillSlotSet.WeaponSkill, masteryType);
		}

		
		public void SetReservationCooldown(SkillSlotSet skillSlotSet, float reservationCooldown)
		{
			this.reservationCooldownMap[skillSlotSet] = reservationCooldown;
		}

		
		public bool IsReservationCooldown(SkillSlotSet skillSlotSet)
		{
			return this.reservationCooldownMap.ContainsKey(skillSlotSet) && this.reservationCooldownMap[skillSlotSet] != 0f;
		}

		
		public void RemoveReservationCooldown(SkillSlotSet skillSlotSet)
		{
			if (!this.reservationCooldownMap.ContainsKey(skillSlotSet))
			{
				return;
			}
			this.reservationCooldownMap[skillSlotSet] = 0f;
		}

		
		public float GetReservationCooldown(SkillSlotSet skillSlotSet)
		{
			float result;
			if (this.reservationCooldownMap.TryGetValue(skillSlotSet, out result))
			{
				return result;
			}
			return 0f;
		}

		
		private readonly Dictionary<SkillSlotSet, float> reservationSquenceCooldownMap = new Dictionary<SkillSlotSet, float>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>.Instance);

		
		private readonly Dictionary<SkillSlotSet, float> reservationCooldownMap = new Dictionary<SkillSlotSet, float>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>.Instance);
	}
}
