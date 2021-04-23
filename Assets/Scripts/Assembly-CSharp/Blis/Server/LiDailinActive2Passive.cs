using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LiDailinActive2Passive)]
	public class LiDailinActive2Passive : SkillScript
	{
		
		private float cooldownReduce;

		
		protected override void Start()
		{
			base.Start();
			if (cooldownReduce == 0f)
			{
				cooldownReduce = -Singleton<LiDailinSkillData>.inst.A2CooldownReduce;
			}

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
			if (victim == null)
			{
				return;
			}

			if (victim == Caster.Character)
			{
				return;
			}

			if (damageInfo == null)
			{
				return;
			}

			if (damageInfo.Attacker == null || damageInfo.Attacker != Caster.Character)
			{
				return;
			}

			if (damageInfo.DamageType != DamageType.Normal)
			{
				return;
			}

			ModifySkillCooldown(Caster, SkillSlotSet.Active2_1, cooldownReduce);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return null;
		}
	}
}