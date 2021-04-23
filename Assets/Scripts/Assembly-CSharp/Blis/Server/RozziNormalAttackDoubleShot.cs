using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.RozziNormalAttackDoubleShot)]
	public class RozziNormalAttackDoubleShot : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollectionPistol1 =
			SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection parameterCollectionPistol2 =
			SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection parameterCollectionSniper =
			SkillScriptParameterCollection.Create();

		
		private bool existHitPoint;

		
		private Vector3 hitPoint = Vector3.zero;

		
		private int skillLv = 1;

		
		protected override void Start()
		{
			base.Start();
			existHitPoint = false;
			skillLv = Caster.GetSkillLevel(SkillSlotIndex.Passive);
			LookAtTarget(Caster, Target);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			CheckReload();
			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<RozziSkillPassiveData>.inst.DoubleShotStateCodeByLevel[skillLv])
					.group, Caster.ObjectId);
			if (existHitPoint)
			{
				CharacterStateData data =
					GameDB.characterState.GetData(Singleton<RozziSkillActive1Data>.inst.Active1MoveStateCode);
				Caster.RemoveStateByGroup(data.group, Caster.ObjectId);
				CommonState commonState =
					CreateState<CommonState>(Caster, Singleton<RozziSkillActive1Data>.inst.Active1MoveStateCode);
				commonState.SetExtraData(hitPoint);
				AddState(Caster, commonState);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			RozziNormalAttackDoubleShot attackDoubleShot = this;
			attackDoubleShot.Start();
			if (attackDoubleShot.IsEnoughBullet())
			{
				MasteryType masteryType = attackDoubleShot.GetEquipWeaponMasteryType(attackDoubleShot.Caster);
				switch (masteryType)
				{
					case MasteryType.Pistol:
						RozziNormalAttackDoubleShot attackDoubleShot1 = attackDoubleShot;
						if (Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolDelay[0] > 0.0)
						{
							if (attackDoubleShot.Caster.Stat.AttackSpeed > 1.0)
							{
								yield return attackDoubleShot.WaitForSecondsByAttackSpeed(
									attackDoubleShot.Caster,
									Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolDelay[0]);
							}
							else
							{
								yield return attackDoubleShot.WaitForSeconds(
									Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolDelay[0]);
							}
						}

						ProjectileProperty projectilePistol_1 =
							attackDoubleShot.PopProjectileProperty(attackDoubleShot.Caster,
								Singleton<RozziSkillDoubleShotData>.inst.ProjectileCodePistolDoubleShotLeft);
						projectilePistol_1.SetActionOnCollisionCharacter(
							(targetAgent, attackerInfo,
								damagePoint, damageDirection) =>
							{
								attackDoubleShot1.parameterCollectionPistol1.Clear();
								attackDoubleShot1.parameterCollectionPistol1.Add(SkillScriptParameterType.DamageApCoef,
									Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolApCoefByLevel_1[
										attackDoubleShot1.skillLv]);
								attackDoubleShot1.DamageTo(targetAgent, attackerInfo,
									projectilePistol_1.ProjectileData.damageType,
									projectilePistol_1.ProjectileData.damageSubType, 0,
									attackDoubleShot1.parameterCollectionPistol1,
									projectilePistol_1.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
							});
						projectilePistol_1.SetTargetObject(attackDoubleShot.Target.ObjectId);
						attackDoubleShot.LaunchProjectile(projectilePistol_1);
						MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(attackDoubleShot.Caster.WorldObject,
							null, NoiseType.Gunshot);
						if (Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolDelay[1] > 0.0)
						{
							if (attackDoubleShot.Caster.Stat.AttackSpeed > 1.0)
							{
								yield return attackDoubleShot.WaitForSecondsByAttackSpeed(
									attackDoubleShot.Caster,
									Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolDelay[1]);
							}
							else
							{
								yield return attackDoubleShot.WaitForSeconds(
									Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolDelay[1]);
							}
						}

						ProjectileProperty projectilePistol_2 =
							attackDoubleShot.PopProjectileProperty(attackDoubleShot.Caster,
								Singleton<RozziSkillDoubleShotData>.inst.ProjectileCodePistolDoubleShotRight);
						projectilePistol_2.SetActionOnCollisionCharacter(
							(targetAgent, attackerInfo,
								damagePoint, damageDirection) =>
							{
								attackDoubleShot1.parameterCollectionPistol2.Clear();
								attackDoubleShot1.parameterCollectionPistol2.Add(SkillScriptParameterType.DamageApCoef,
									Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolApCoefByLevel_2[
										attackDoubleShot1.skillLv]);
								attackDoubleShot1.DamageTo(targetAgent, attackerInfo,
									projectilePistol_2.ProjectileData.damageType,
									projectilePistol_2.ProjectileData.damageSubType, 0,
									attackDoubleShot1.parameterCollectionPistol2,
									projectilePistol_2.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
							});
						projectilePistol_2.SetTargetObject(attackDoubleShot.Target.ObjectId);
						attackDoubleShot.LaunchProjectile(projectilePistol_2);
						break;
					case MasteryType.SniperRifle:
						RozziNormalAttackDoubleShot attackDoubleShot2 = attackDoubleShot;
						if (Singleton<RozziSkillDoubleShotData>.inst.DoubleShotSniperDelay > 0.0)
						{
							if (attackDoubleShot.Caster.Stat.AttackSpeed > 1.0)
							{
								yield return attackDoubleShot.WaitForSecondsByAttackSpeed(
									attackDoubleShot.Caster,
									Singleton<RozziSkillDoubleShotData>.inst.DoubleShotSniperDelay);
							}
							else
							{
								yield return attackDoubleShot.WaitForSeconds(
									Singleton<RozziSkillDoubleShotData>.inst.DoubleShotSniperDelay);
							}
						}

						ProjectileProperty projectileSniper = attackDoubleShot.PopProjectileProperty(
							attackDoubleShot.Caster, Singleton<RozziSkillDoubleShotData>.inst.ProjectileCodeSniper);
						projectileSniper.SetActionOnCollisionCharacter(
							(targetAgent, attackerInfo,
								damagePoint, damageDirection) =>
							{
								attackDoubleShot2.parameterCollectionSniper.Clear();
								attackDoubleShot2.parameterCollectionSniper.Add(SkillScriptParameterType.DamageApCoef,
									Singleton<RozziSkillDoubleShotData>.inst.DoubleShotSniperApCoefByLevel[
										attackDoubleShot2.skillLv]);
								attackDoubleShot2.DamageTo(targetAgent, attackerInfo,
									projectileSniper.ProjectileData.damageType,
									projectileSniper.ProjectileData.damageSubType, 0,
									attackDoubleShot2.parameterCollectionSniper,
									projectileSniper.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
							});
						projectileSniper.SetTargetObject(attackDoubleShot.Target.ObjectId);
						attackDoubleShot.LaunchProjectile(projectileSniper);
						MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(attackDoubleShot.Caster.WorldObject,
							null, NoiseType.Gunshot);
						break;
				}

				if (Singleton<RozziSkillDoubleShotData>.inst.FinishDelayTime[masteryType] > 0.0)
				{
					if (attackDoubleShot.Caster.Stat.AttackSpeed > 1.0)
					{
						yield return attackDoubleShot.WaitForSecondsByAttackSpeed(attackDoubleShot.Caster,
							Singleton<RozziSkillDoubleShotData>.inst.FinishDelayTime[masteryType]);
					}
					else
					{
						yield return attackDoubleShot.WaitForSeconds(
							Singleton<RozziSkillDoubleShotData>.inst.FinishDelayTime[masteryType]);
					}
				}
			}

			attackDoubleShot.Finish();

			// co: dotPeek
			// this.Start();
			// if (base.IsEnoughBullet())
			// {
			// 	MasteryType masteryType = base.GetEquipWeaponMasteryType(base.Caster);
			// 	if (masteryType == MasteryType.Pistol)
			// 	{
			// 		RozziNormalAttackDoubleShot.<>c__DisplayClass8_0 CS$<>8__locals1 = new RozziNormalAttackDoubleShot.<>c__DisplayClass8_0();
			// 		CS$<>8__locals1.<>4__this = this;
			// 		if (Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolDelay[0] > 0f)
			// 		{
			// 			if (base.Caster.Stat.AttackSpeed > 1f)
			// 			{
			// 				yield return base.WaitForSecondsByAttackSpeed(base.Caster, Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolDelay[0]);
			// 			}
			// 			else
			// 			{
			// 				yield return base.WaitForSeconds(Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolDelay[0]);
			// 			}
			// 		}
			// 		CS$<>8__locals1.projectilePistol_1 = base.PopProjectileProperty(base.Caster, Singleton<RozziSkillDoubleShotData>.inst.ProjectileCodePistolDoubleShotLeft);
			// 		CS$<>8__locals1.projectilePistol_1.SetActionOnCollisionCharacter(delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint, Vector3 damageDirection)
			// 		{
			// 			CS$<>8__locals1.<>4__this.parameterCollectionPistol1.Clear();
			// 			CS$<>8__locals1.<>4__this.parameterCollectionPistol1.Add(SkillScriptParameterType.DamageApCoef, Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolApCoefByLevel_1[CS$<>8__locals1.<>4__this.skillLv]);
			// 			CS$<>8__locals1.<>4__this.DamageTo(targetAgent, attackerInfo, CS$<>8__locals1.projectilePistol_1.ProjectileData.damageType, CS$<>8__locals1.projectilePistol_1.ProjectileData.damageSubType, 0, CS$<>8__locals1.<>4__this.parameterCollectionPistol1, CS$<>8__locals1.projectilePistol_1.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0, true, 0, 1f, true);
			// 		});
			// 		CS$<>8__locals1.projectilePistol_1.SetTargetObject(base.Target.ObjectId);
			// 		base.LaunchProjectile(CS$<>8__locals1.projectilePistol_1);
			// 		MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(base.Caster.WorldObject, null, NoiseType.Gunshot);
			// 		if (Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolDelay[1] > 0f)
			// 		{
			// 			if (base.Caster.Stat.AttackSpeed > 1f)
			// 			{
			// 				yield return base.WaitForSecondsByAttackSpeed(base.Caster, Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolDelay[1]);
			// 			}
			// 			else
			// 			{
			// 				yield return base.WaitForSeconds(Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolDelay[1]);
			// 			}
			// 		}
			// 		CS$<>8__locals1.projectilePistol_2 = base.PopProjectileProperty(base.Caster, Singleton<RozziSkillDoubleShotData>.inst.ProjectileCodePistolDoubleShotRight);
			// 		CS$<>8__locals1.projectilePistol_2.SetActionOnCollisionCharacter(delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint, Vector3 damageDirection)
			// 		{
			// 			CS$<>8__locals1.<>4__this.parameterCollectionPistol2.Clear();
			// 			CS$<>8__locals1.<>4__this.parameterCollectionPistol2.Add(SkillScriptParameterType.DamageApCoef, Singleton<RozziSkillDoubleShotData>.inst.DoubleShotPistolApCoefByLevel_2[CS$<>8__locals1.<>4__this.skillLv]);
			// 			CS$<>8__locals1.<>4__this.DamageTo(targetAgent, attackerInfo, CS$<>8__locals1.projectilePistol_2.ProjectileData.damageType, CS$<>8__locals1.projectilePistol_2.ProjectileData.damageSubType, 0, CS$<>8__locals1.<>4__this.parameterCollectionPistol2, CS$<>8__locals1.projectilePistol_2.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0, true, 0, 1f, true);
			// 		});
			// 		CS$<>8__locals1.projectilePistol_2.SetTargetObject(base.Target.ObjectId);
			// 		base.LaunchProjectile(CS$<>8__locals1.projectilePistol_2);
			// 		CS$<>8__locals1 = null;
			// 	}
			// 	else if (masteryType == MasteryType.SniperRifle)
			// 	{
			// 		RozziNormalAttackDoubleShot.<>c__DisplayClass8_1 CS$<>8__locals2 = new RozziNormalAttackDoubleShot.<>c__DisplayClass8_1();
			// 		CS$<>8__locals2.<>4__this = this;
			// 		if (Singleton<RozziSkillDoubleShotData>.inst.DoubleShotSniperDelay > 0f)
			// 		{
			// 			if (base.Caster.Stat.AttackSpeed > 1f)
			// 			{
			// 				yield return base.WaitForSecondsByAttackSpeed(base.Caster, Singleton<RozziSkillDoubleShotData>.inst.DoubleShotSniperDelay);
			// 			}
			// 			else
			// 			{
			// 				yield return base.WaitForSeconds(Singleton<RozziSkillDoubleShotData>.inst.DoubleShotSniperDelay);
			// 			}
			// 		}
			// 		CS$<>8__locals2.projectileSniper = base.PopProjectileProperty(base.Caster, Singleton<RozziSkillDoubleShotData>.inst.ProjectileCodeSniper);
			// 		CS$<>8__locals2.projectileSniper.SetActionOnCollisionCharacter(delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint, Vector3 damageDirection)
			// 		{
			// 			CS$<>8__locals2.<>4__this.parameterCollectionSniper.Clear();
			// 			CS$<>8__locals2.<>4__this.parameterCollectionSniper.Add(SkillScriptParameterType.DamageApCoef, Singleton<RozziSkillDoubleShotData>.inst.DoubleShotSniperApCoefByLevel[CS$<>8__locals2.<>4__this.skillLv]);
			// 			CS$<>8__locals2.<>4__this.DamageTo(targetAgent, attackerInfo, CS$<>8__locals2.projectileSniper.ProjectileData.damageType, CS$<>8__locals2.projectileSniper.ProjectileData.damageSubType, 0, CS$<>8__locals2.<>4__this.parameterCollectionSniper, CS$<>8__locals2.projectileSniper.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0, true, 0, 1f, true);
			// 		});
			// 		CS$<>8__locals2.projectileSniper.SetTargetObject(base.Target.ObjectId);
			// 		base.LaunchProjectile(CS$<>8__locals2.projectileSniper);
			// 		MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(base.Caster.WorldObject, null, NoiseType.Gunshot);
			// 		CS$<>8__locals2 = null;
			// 	}
			// 	if (Singleton<RozziSkillDoubleShotData>.inst.FinishDelayTime[masteryType] > 0f)
			// 	{
			// 		if (base.Caster.Stat.AttackSpeed > 1f)
			// 		{
			// 			yield return base.WaitForSecondsByAttackSpeed(base.Caster, Singleton<RozziSkillDoubleShotData>.inst.FinishDelayTime[masteryType]);
			// 		}
			// 		else
			// 		{
			// 			yield return base.WaitForSeconds(Singleton<RozziSkillDoubleShotData>.inst.FinishDelayTime[masteryType]);
			// 		}
			// 	}
			// }
			// this.Finish(false);
			// yield break;
		}

		
		public override bool UseOnMove()
		{
			return true;
		}

		
		public override void OnMove(Vector3 hitPoint)
		{
			if (Caster.IsPlayingStateSkillScript(SkillId.RozziActive1MoveState))
			{
				existHitPoint = true;
				this.hitPoint = hitPoint;
			}
		}
	}
}