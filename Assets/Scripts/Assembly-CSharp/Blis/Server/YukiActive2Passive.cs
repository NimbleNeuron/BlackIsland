using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.YukiActive2Passive)]
	public class YukiActive2Passive : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield break;
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

			if (damageInfo.Attacker.ObjectId != Caster.ObjectId)
			{
				return;
			}

			if (victim.ObjectId == Caster.ObjectId)
			{
				return;
			}

			if (damageInfo.DamageType != DamageType.Normal && damageInfo.DamageType != DamageType.Skill)
			{
				return;
			}

			if (damageInfo.DamageSubType != DamageSubType.Normal)
			{
				return;
			}

			ModifySkillCooldown(Caster, SkillSlotSet.Active2_1,
				Singleton<YukiSkillActive2Data>.inst.CooldownReduceActive2);
		}
	}
}