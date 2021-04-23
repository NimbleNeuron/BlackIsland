using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SilviaHumanActive1Passive)]
	public class SilviaHumanActive1Passive : SkillScript
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

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
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

			if (damageInfo.DamageType != DamageType.Normal)
			{
				return;
			}

			if (damageInfo.DamageSubType != DamageSubType.Normal)
			{
				return;
			}

			bool? flag = GetEquipWeaponMasteryType(Caster).IsLaunchProjectile();
			if (flag == null)
			{
				return;
			}

			SkillAgent caster = Caster;
			SkillSlotSet skillSlotSetFlag = SkillSlotSet.Active1_1;
			bool? flag2 = flag;
			bool flag3 = true;
			ModifySkillCooldown(caster, skillSlotSetFlag,
				(flag2.GetValueOrDefault() == flag3) & (flag2 != null)
					? Singleton<SilviaSkillHumanData>.inst.A1CooldownModifyRange
					: Singleton<SilviaSkillHumanData>.inst.A1CooldownModifyMelee);
		}
	}
}