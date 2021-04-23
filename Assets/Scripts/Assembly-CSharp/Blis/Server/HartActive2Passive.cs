using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HartActive2Passive)]
	public class HartActive2Passive : HartSkillScript
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

			SkillSlotSet? skillSlotSet = damageInfo.SkillSlotSet;
			SkillSlotSet skillSlotSet2 = SkillSlotSet.WeaponSkill;
			if ((skillSlotSet.GetValueOrDefault() == skillSlotSet2) & (skillSlotSet != null))
			{
				return;
			}

			if (damageInfo.DamageSubType == DamageSubType.Trap)
			{
				return;
			}

			CharacterStateData data = GameDB.characterState.GetData(Singleton<HartSkillActive2Data>.inst.BuffState2);
			if (!Caster.IsHaveStateByGroup(data.group, Caster.ObjectId))
			{
				return;
			}

			AddState(victim.SkillAgent, Singleton<HartSkillActive2Data>.inst.DebuffState[SkillEvolutionLevel]);
			Caster.RemoveStateByGroup(data.group, Caster.ObjectId);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}