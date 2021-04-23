using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SisselaPassiveNormalAttackMount)]
	public class SisselaPassiveNormalAttackMount : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			MasteryType masteryType = GetEquipWeaponMasteryType(Caster);
			LookAtTarget(Caster, Target, 0.1f);
			yield return WaitForSecondsByAttackSpeed(Caster,
				Singleton<SisselaSkillData>.inst.NormalAttackDelay[masteryType]);
			ProjectileProperty projectile =
				PopProjectileProperty(Caster, Singleton<SisselaSkillData>.inst.ProjectileCode[masteryType]);
			projectile.SetTargetObject(Target.ObjectId);
			projectile.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<SisselaSkillData>.inst.PassiveNormalAttackBaseDamage);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<SisselaSkillData>.inst.PassiveNormalAttackApDamageSkillType);
					parameterCollection.Add(SkillScriptParameterType.DamageCharacterLvCoef,
						Singleton<SisselaSkillData>.inst.PassiveNormalAttackCharacterLvPerDamage);
					DamageTo(targetAgent, attackerInfo, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
						projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection,
						Singleton<SisselaSkillData>.inst.PassiveNormalAttackDebuffEffectAndSound[masteryType]);
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<SisselaSkillData>.inst.PassiveNormalAttackApDamageNormalType);
					DamageTo(targetAgent, attackerInfo, DamageType.Normal, DamageSubType.Normal, 0, parameterCollection,
						projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
					AddState(targetAgent,
						Singleton<SisselaSkillData>.inst.PassiveNormalAttackDebuff[
							Caster.GetSkillLevel(SkillSlotIndex.Passive)]);
				});
			LaunchProjectile(projectile);
			FinishNormalAttack();
			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<SisselaSkillData>.inst.PassiveNormalAttackMountState).group,
				Caster.ObjectId);
			yield return WaitForSecondsByAttackSpeed(Caster, 0.13f);
			Finish();
		}
	}
}