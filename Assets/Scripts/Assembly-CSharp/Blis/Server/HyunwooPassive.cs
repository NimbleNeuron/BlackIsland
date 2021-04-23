using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyunwooPassive)]
	public class HyunwooPassive : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
		}

		
		private void OnAfterDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (damageInfo == null || damageInfo.Attacker == null)
			{
				return;
			}

			if (Caster.ObjectId == victim.ObjectId)
			{
				if (Caster.ObjectId == damageInfo.Attacker.ObjectId)
				{
					return;
				}
			}
			else if (Caster.ObjectId != damageInfo.Attacker.ObjectId)
			{
				return;
			}

			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<HyunwooSkillPassiveData>.inst.BuffState[SkillLevel]);
			CharacterState characterState = Caster.FindStateByGroup(data.group, Caster.ObjectId);
			if (Caster.ObjectId == damageInfo.Attacker.ObjectId && characterState != null &&
			    Caster.Status.Hp < Caster.Stat.MaxHp && data.maxStack <= characterState.StackCount)
			{
				Caster.RemoveStateByGroup(data.group, Caster.ObjectId);
				HpHealTo(Caster, Caster.Stat.MaxHp, Singleton<HyunwooSkillPassiveData>.inst.HpCoef[SkillLevel], 0, true,
					Singleton<HyunwooSkillPassiveData>.inst.EffectAndSoundCode);
				return;
			}

			if (characterState == null || characterState.StackCount < data.maxStack)
			{
				AddState(Caster, data.code,
					Caster.ObjectId == damageInfo.Attacker.ObjectId
						? Singleton<HyunwooSkillPassiveData>.inst.IncreasePassiveBuffStackCountOnAttack
						: Singleton<HyunwooSkillPassiveData>.inst.IncreasePassiveBuffStackCountOnHit);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		public override void OnUpgradePassiveSkill()
		{
			base.OnUpgradePassiveSkill();
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<HyunwooSkillPassiveData>.inst.BuffState[SkillLevel]);
			if (Caster.IsHaveStateByGroup(data.group, Caster.ObjectId))
			{
				Caster.OverwriteState(data.code, Caster.ObjectId);
			}
		}
	}
}