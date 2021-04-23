using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AxePassive)]
	public class AxePassive : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterNormalDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(
				inst.OnAfterNormalDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterNormalDamageProcess));
		}

		
		private void OnAfterNormalDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
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

			AddState(Caster, Singleton<AxeSkillActiveData>.inst.BuffState);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			CharacterStateData data = GameDB.characterState.GetData(Singleton<AxeSkillActiveData>.inst.BuffState);
			Caster.RemoveStateByGroup(data.GroupData.group, Caster.ObjectId);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterNormalDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(
				inst.OnAfterNormalDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterNormalDamageProcess));
		}
	}
}