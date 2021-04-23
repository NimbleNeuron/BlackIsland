using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SilviaBikeState)]
	public class SilviaBikeState : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			if (SwitchSkillSet(SkillSlotIndex.Attack, SkillSlotSet.Attack_2))
			{
				SwitchSkillSet(SkillSlotIndex.Active1, SkillSlotSet.Active1_2);
				SwitchSkillSet(SkillSlotIndex.Active2, SkillSlotSet.Active2_2);
				SwitchSkillSet(SkillSlotIndex.Active3, SkillSlotSet.Active3_2);
				SwitchSkillSet(SkillSlotIndex.Active4, SkillSlotSet.Active4_2);
			}

			AddState(Caster,
				Singleton<SilviaSkillBikeData>.inst.BikeSpeedUpState[Caster.GetSkillLevel(SkillSlotIndex.Active4)]);
			AddState(Caster, Singleton<SilviaSkillBikeData>.inst.BikeSpeedDownStateCode);
			AddState(Caster, Singleton<SilviaSkillBikeData>.inst.BikeSpeedCalculateStateCode);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterSkillLevelUpEvent = (Action<WorldPlayerCharacter, SkillSlotIndex, int>) Delegate.Remove(
				inst.OnAfterSkillLevelUpEvent,
				new Action<WorldPlayerCharacter, SkillSlotIndex, int>(OnAfterSkillLevelUp));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnAfterSkillLevelUpEvent = (Action<WorldPlayerCharacter, SkillSlotIndex, int>) Delegate.Combine(
				inst2.OnAfterSkillLevelUpEvent,
				new Action<WorldPlayerCharacter, SkillSlotIndex, int>(OnAfterSkillLevelUp));
			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<SilviaSkillHumanData>.inst.NormalAttackReinforceState).group,
				Caster.ObjectId);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterSkillLevelUpEvent = (Action<WorldPlayerCharacter, SkillSlotIndex, int>) Delegate.Remove(
				inst.OnAfterSkillLevelUpEvent,
				new Action<WorldPlayerCharacter, SkillSlotIndex, int>(OnAfterSkillLevelUp));
		}

		
		private void OnAfterSkillLevelUp(WorldPlayerCharacter target, SkillSlotIndex slotIndex, int level)
		{
			if (target == null)
			{
				return;
			}

			if (target.ObjectId != Caster.ObjectId)
			{
				return;
			}

			if (slotIndex != SkillSlotIndex.Active4)
			{
				return;
			}

			AddState(Caster, Singleton<SilviaSkillBikeData>.inst.BikeSpeedUpState[level]);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}