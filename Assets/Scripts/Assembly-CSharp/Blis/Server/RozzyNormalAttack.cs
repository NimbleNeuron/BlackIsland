using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.RozziNormalAttack)]
	public class RozzyNormalAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			RozzyNormalAttack rozzyNormalAttack = this;
			rozzyNormalAttack.Start();
			rozzyNormalAttack.LookAtTarget(rozzyNormalAttack.Caster, rozzyNormalAttack.Target, 0.1f);
			MasteryType weaponMasteryType = rozzyNormalAttack.GetEquipWeaponMasteryType(rozzyNormalAttack.Caster);
			int masteryTypeNum = (int) weaponMasteryType;
			int projectileCode = weaponMasteryType == MasteryType.Pistol
				? Singleton<RozziSkillNormalAttackData>.inst.GetPistolProjectileCode()
				: Singleton<RozziSkillNormalAttackData>.inst.ProjectileCodeSniper;
			if (rozzyNormalAttack.IsEnoughBullet())
			{
				int actionNo = Singleton<RozziSkillNormalAttackData>.inst.IsRightProjectileCodePistol(projectileCode)
					? 1
					: 2;
				rozzyNormalAttack.PlaySkillAction(rozzyNormalAttack.Caster, rozzyNormalAttack.info.skillData.SkillId,
					actionNo);
				yield return rozzyNormalAttack.WaitForSecondsByAttackSpeed(rozzyNormalAttack.Caster,
					Singleton<RozziSkillNormalAttackData>.inst.NormalAttackDelay[masteryTypeNum]);
				ProjectileProperty projectileProperty =
					rozzyNormalAttack.PopProjectileProperty(rozzyNormalAttack.Caster, projectileCode);
				projectileProperty.SetTargetObject(rozzyNormalAttack.Target.ObjectId);
				projectileProperty.SetActionOnCollisionCharacter(
					(targetAgent, attackerInfo, damagePoint,
						damageDirection) =>
					{
						parameterCollection.Clear();
						parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
							Singleton<RozziSkillNormalAttackData>.inst.NormalAttackApCoef[masteryTypeNum]);
						DamageTo(targetAgent, attackerInfo, projectileProperty.ProjectileData.damageType,
							projectileProperty.ProjectileData.damageSubType, 0, parameterCollection,
							projectileProperty.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
					});
				rozzyNormalAttack.LaunchProjectile(projectileProperty);
				MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(rozzyNormalAttack.Caster.WorldObject,
					null, NoiseType.Gunshot);
				rozzyNormalAttack.FinishNormalAttack();
			}

			if (rozzyNormalAttack.IsEnoughBullet())
			{
				yield return rozzyNormalAttack.WaitForSecondsByAttackSpeed(rozzyNormalAttack.Caster, 0.13f);
			}

			rozzyNormalAttack.CheckReload();
			rozzyNormalAttack.Finish();

			// co: dotPeek
			// RozzyNormalAttack.<>c__DisplayClass2_0 CS$<>8__locals1 = new RozzyNormalAttack.<>c__DisplayClass2_0();
			// CS$<>8__locals1.<>4__this = this;
			// this.Start();
			// base.LookAtTarget(base.Caster, base.Target, 0.1f, false);
			// MasteryType equipWeaponMasteryType = base.GetEquipWeaponMasteryType(base.Caster);
			// CS$<>8__locals1.masteryTypeNum = (int)equipWeaponMasteryType;
			// int projectileCode = (equipWeaponMasteryType == MasteryType.Pistol) ? Singleton<RozziSkillNormalAttackData>.inst.GetPistolProjectileCode() : Singleton<RozziSkillNormalAttackData>.inst.ProjectileCodeSniper;
			// if (base.IsEnoughBullet())
			// {
			// 	RozzyNormalAttack.<>c__DisplayClass2_1 CS$<>8__locals2 = new RozzyNormalAttack.<>c__DisplayClass2_1();
			// 	CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			// 	int actionNo = Singleton<RozziSkillNormalAttackData>.inst.IsRightProjectileCodePistol(projectileCode) ? 1 : 2;
			// 	base.PlaySkillAction(base.Caster, this.info.skillData.SkillId, actionNo, 0, null);
			// 	yield return base.WaitForSecondsByAttackSpeed(base.Caster, Singleton<RozziSkillNormalAttackData>.inst.NormalAttackDelay[CS$<>8__locals2.CS$<>8__locals1.masteryTypeNum]);
			// 	CS$<>8__locals2.projectileProperty = base.PopProjectileProperty(base.Caster, projectileCode);
			// 	CS$<>8__locals2.projectileProperty.SetTargetObject(base.Target.ObjectId);
			// 	CS$<>8__locals2.projectileProperty.SetActionOnCollisionCharacter(delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint, Vector3 damageDirection)
			// 	{
			// 		CS$<>8__locals2.CS$<>8__locals1.<>4__this.parameterCollection.Clear();
			// 		CS$<>8__locals2.CS$<>8__locals1.<>4__this.parameterCollection.Add(SkillScriptParameterType.DamageApCoef, Singleton<RozziSkillNormalAttackData>.inst.NormalAttackApCoef[CS$<>8__locals2.CS$<>8__locals1.masteryTypeNum]);
			// 		CS$<>8__locals2.CS$<>8__locals1.<>4__this.DamageTo(targetAgent, attackerInfo, CS$<>8__locals2.projectileProperty.ProjectileData.damageType, CS$<>8__locals2.projectileProperty.ProjectileData.damageSubType, 0, CS$<>8__locals2.CS$<>8__locals1.<>4__this.parameterCollection, CS$<>8__locals2.projectileProperty.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0, true, 0, 1f, true);
			// 	});
			// 	base.LaunchProjectile(CS$<>8__locals2.projectileProperty);
			// 	MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(base.Caster.WorldObject, null, NoiseType.Gunshot);
			// 	base.FinishNormalAttack();
			// 	CS$<>8__locals2 = null;
			// }
			// if (base.IsEnoughBullet())
			// {
			// 	yield return base.WaitForSecondsByAttackSpeed(base.Caster, 0.13f);
			// }
			// base.CheckReload();
			// this.Finish(false);
			// yield break;
		}
	}
}