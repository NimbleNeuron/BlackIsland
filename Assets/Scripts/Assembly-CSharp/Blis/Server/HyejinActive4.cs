using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyejinActive4)]
	public class HyejinActive4 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection concentrationDamageParam =
			SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection projectileDamageParam = SkillScriptParameterCollection.Create();

		
		private bool canMoveDuringSkillPlaying = true;

		
		private CollisionCircle3D collision;

		
		private int projectileCount;

		
		
		public override bool CanMoveDuringSkillPlaying => canMoveDuringSkillPlaying;

		
		protected override void Start()
		{
			base.Start();
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active2_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
			if (GetSkillSequence(SkillSlotSet.Active3_1) != 1)
			{
				LockSkillSlot3();
			}

			if (collision == null)
			{
				collision = new CollisionCircle3D(Caster.Position, SkillWidth);
				projectileCount = Singleton<HyejinSkillData>.inst.A4ProjectileCount;
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (IsLockSkillSlot(SkillSlotSet.Active1_1))
			{
				UnlockSkillSlot(SkillSlotSet.Active1_1);
				UnlockSkillSlot(SkillSlotSet.Active2_1);
				UnlockSkillSlot(SkillSlotSet.Active3_1);
				UnlockSkillSlot(SkillSlotSet.WeaponSkill);
			}

			if (Caster.IsHaveStateByGroup(
				GameDB.characterState.GetData(Singleton<HyejinSkillData>.inst.A4ConcentrationDebuffCode).group,
				Caster.ObjectId))
			{
				Caster.RemoveStateByGroup(
					GameDB.characterState.GetData(Singleton<HyejinSkillData>.inst.A4ConcentrationDebuffCode).group,
					Caster.ObjectId);
			}

			canMoveDuringSkillPlaying = true;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			AddState(Caster, Singleton<HyejinSkillData>.inst.A4ConcentrationDebuffCode);
			yield return WaitForSeconds(SkillConcentrationTime);
			FinishConcentration(false);
			canMoveDuringSkillPlaying = false;
			Caster.StopMove();
			PlaySkillAction(Caster, 2);
			if (!IsLockSkillSlot(SkillSlotSet.Active3_1))
			{
				LockSkillSlot3();
			}

			collision.UpdatePosition(Caster.Position);
			collision.UpdateRadius(SkillWidth);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
			if (enemyCharacters.Count > 0)
			{
				concentrationDamageParam.Clear();
				concentrationDamageParam.Add(SkillScriptParameterType.Damage,
					Singleton<HyejinSkillData>.inst.A4ConcentrationEndBaseDamage[SkillLevel]);
				concentrationDamageParam.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<HyejinSkillData>.inst.A4ConcentrationEndApDamage);
				DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 1, concentrationDamageParam,
					Singleton<HyejinSkillData>.inst.A4ConcentrationEndDamageEffect);
			}

			if (SkillCastingTime2 > 0f)
			{
				yield return SecondCastingTime();
			}

			Vector3 vector = Vector3.left - Caster.Forward;
			float num = Mathf.Atan2(vector.z, vector.x) * 57.29578f * 2f;
			for (int i = 0; i < projectileCount; i++)
			{
				ProjectileProperty projectileProperty =
					PopProjectileProperty(Caster, Singleton<HyejinSkillData>.inst.A4ProjectileCode);
				projectileProperty.SetTargetObject(Caster.ObjectId);
				projectileProperty.SetStartAngle(i * 360f / projectileCount + num);
				projectileProperty.SetActionOnCollisionCharacter(
					delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
						Vector3 damageDirection)
					{
						projectileDamageParam.Clear();
						projectileDamageParam.Add(SkillScriptParameterType.Damage,
							Singleton<HyejinSkillData>.inst.A4ProjectileBaseDamage[SkillLevel]);
						projectileDamageParam.Add(SkillScriptParameterType.DamageApCoef,
							Singleton<HyejinSkillData>.inst.A4ProjectileApDamage);
						DamageTo(targetAgent, attackerInfo, projectileProperty.ProjectileData.damageType,
							projectileProperty.ProjectileData.damageSubType, 1, projectileDamageParam,
							projectileProperty.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
					});
				WorldProjectile target = LaunchProjectile(projectileProperty);
				Caster.AttachSight(target, 1f, projectileProperty.ProjectileData.lifeTimeAfterArrival, false);
			}

			AddState(Caster, Singleton<HyejinSkillData>.inst.A4ProjectileState);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		private void LockSkillSlot3()
		{
			LockSkillSlot(SkillSlotSet.Active3_1);
			PlaySkillAction(Caster, 1);
		}
	}
}