using System;
using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.EmmaPassive)]
	public class EmmaPassive : SkillScript
	{
		
		protected override void Start()
		{
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnBeforeNormalDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(
				inst.OnBeforeNormalDamageProcess, new Action<WorldCharacter, DamageInfo>(OnBeforeNormalDamageProcess));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnAfterSkillDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(
				inst2.OnAfterSkillDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterSkillDamageProcess));
			BattleEventCollector inst3 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst3.OnAfterActionCastingEvent = (Action<WorldCharacter, ActionCostData>) Delegate.Combine(
				inst3.OnAfterActionCastingEvent, new Action<WorldCharacter, ActionCostData>(OnAfterActionCastingEvent));
			BattleEventCollector inst4 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst4.OnBeforeSkillActiveEvent = (Action<WorldCharacter, SkillData, SkillSlotSet>) Delegate.Combine(
				inst4.OnBeforeSkillActiveEvent,
				new Action<WorldCharacter, SkillData, SkillSlotSet>(OnBeforeSkillActiveEvent));
			base.Start();
			PlayPassiveSkill(info);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			for (;;)
			{
				if (Caster.GetSkillCooldown(SkillSlotSet.Passive_1) == 0f &&
				    !Caster.AnyHaveStateByGroup(Singleton<EmmaSkillPassiveData>.inst
					    .CheerUpNormalAttackBuffStateGroupCode) &&
				    !Caster.AnyHaveStateByGroup(Singleton<EmmaSkillPassiveData>.inst.CheerUpShieldStateGroupCode))
				{
					AddState(Caster, Singleton<EmmaSkillPassiveData>.inst.CheerUpNormalAttackBuffStateCode);
				}

				yield return WaitForSeconds(Singleton<EmmaSkillPassiveData>.inst.PassiveUpdateTime);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnBeforeNormalDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(
				inst.OnBeforeNormalDamageProcess, new Action<WorldCharacter, DamageInfo>(OnBeforeNormalDamageProcess));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnAfterSkillDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(
				inst2.OnAfterSkillDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterSkillDamageProcess));
			BattleEventCollector inst3 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst3.OnAfterActionCastingEvent = (Action<WorldCharacter, ActionCostData>) Delegate.Remove(
				inst3.OnAfterActionCastingEvent, new Action<WorldCharacter, ActionCostData>(OnAfterActionCastingEvent));
			BattleEventCollector inst4 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst4.OnBeforeSkillActiveEvent = (Action<WorldCharacter, SkillData, SkillSlotSet>) Delegate.Remove(
				inst4.OnBeforeSkillActiveEvent,
				new Action<WorldCharacter, SkillData, SkillSlotSet>(OnBeforeSkillActiveEvent));
		}

		
		private void OnBeforeNormalDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			if (damageInfo.Attacker == null)
			{
				return;
			}

			if (damageInfo.Attacker != Caster.Character)
			{
				return;
			}

			if (Caster.GetSkillCooldown(SkillSlotSet.Passive_1) == 0f)
			{
				int num = Mathf.RoundToInt(Caster.Stat.MaxSp *
				                           Singleton<EmmaSkillPassiveData>.inst.CheerUpShieldAdditionalMaxSpRatio[
					                           SkillLevel]);
				CharacterStateData data =
					GameDB.characterState.GetData(Singleton<EmmaSkillPassiveData>.inst.CheerUpShieldStateCode);
				ShieldState shieldState = CreateState<ShieldState>(Caster,
					Singleton<EmmaSkillPassiveData>.inst.CheerUpShieldStateCode, 0, data.duration);
				shieldState.Init(0f, Singleton<EmmaSkillPassiveData>.inst.CheerUpShieldByLevel[SkillLevel] + num);
				AddState(Caster, shieldState);
				PlayPassiveSkill(info);
			}
		}

		
		private void OnAfterSkillDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			if (damageInfo.Attacker == null)
			{
				return;
			}

			if (damageInfo.Attacker != Caster.Character)
			{
				return;
			}

			if (Caster.AnyHaveStateByGroup(Singleton<EmmaSkillPassiveData>.inst.CheerUpNormalAttackBuffStateGroupCode))
			{
				Caster.RemoveStateByGroup(Singleton<EmmaSkillPassiveData>.inst.CheerUpNormalAttackBuffStateGroupCode,
					Caster.ObjectId);
			}
		}

		
		private void OnAfterActionCastingEvent(WorldCharacter actionCaster, ActionCostData actionCostData)
		{
			if (actionCaster == Caster.Character)
			{
				Active3SkillHealProcess(actionCostData.sp);
			}
		}

		
		private void OnBeforeSkillActiveEvent(WorldCharacter skillCaster, SkillData skillData,
			SkillSlotSet skillSlotSet)
		{
			if (skillCaster == Caster.Character && skillData.CostType == SkillCostType.Sp)
			{
				Active3SkillHealProcess(skillData.cost);
			}
		}

		
		private void Active3SkillHealProcess(int costSp)
		{
			int skillLevel = Caster.GetSkillLevel(SkillSlotIndex.Active3);
			if (0 < skillLevel)
			{
				HpHealTo(Caster, 0, 0f,
					Mathf.RoundToInt(costSp *
					                 Singleton<EmmaSkillActive3Data>.inst
						                 .HealRatioPerConsumeSPBySkillLevel[skillLevel]), false,
					Singleton<EmmaSkillActive3Data>.inst.MagicRabbitHealEffectAndSoundCode);
			}
		}
	}
}