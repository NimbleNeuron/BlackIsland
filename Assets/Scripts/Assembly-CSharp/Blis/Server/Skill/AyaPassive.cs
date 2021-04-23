using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AyaPassive)]
	public class AyaPassive : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnBeforeDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(
				inst.OnBeforeDamageProcess, new Action<WorldCharacter, DamageInfo>(OnBeforeDamageProcess));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(
				inst2.OnAfterDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnBeforeDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(
				inst.OnBeforeDamageProcess, new Action<WorldCharacter, DamageInfo>(OnBeforeDamageProcess));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(
				inst2.OnAfterDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
		}

		
		private void OnBeforeDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (Caster.ObjectId != victim.ObjectId)
			{
				return;
			}

			if (damageInfo == null)
			{
				return;
			}

			if (damageInfo.Attacker != null && Caster.ObjectId == damageInfo.Attacker.ObjectId)
			{
				return;
			}

			if (damageInfo.DamageSubType != DamageSubType.Normal)
			{
				return;
			}

			if (!IsReadySkill(Caster, SkillSlotSet.Passive_1))
			{
				return;
			}

			ShieldState shieldState = CreateState<ShieldState>(Caster, Singleton<AyaSkillPassiveData>.inst.BuffState);
			shieldState.Init(Singleton<AyaSkillPassiveData>.inst.SkillApCoef,
				Singleton<AyaSkillPassiveData>.inst.ShieldByLevel[SkillLevel]);
			AddState(Caster, shieldState);
			PlayPassiveSkill(info);
		}

		
		private void OnAfterDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (Caster.ObjectId == victim.ObjectId)
			{
				return;
			}

			if (damageInfo == null || damageInfo.Attacker == null)
			{
				return;
			}

			if (Caster.ObjectId != damageInfo.Attacker.ObjectId)
			{
				return;
			}

			if (Caster.GetSkillCooldown(SkillSlotSet.Passive_1) <= 0f)
			{
				return;
			}

			if (damageInfo.DamageSubType == DamageSubType.Trap)
			{
				return;
			}

			ModifySkillCooldown(Caster, SkillSlotSet.Passive_1, Singleton<AyaSkillPassiveData>.inst.CooldownReduce);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}