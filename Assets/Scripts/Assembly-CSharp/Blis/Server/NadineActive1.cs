using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.NadineActive1)]
	public class NadineActive1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private bool isActivePassive;

		
		private bool isShoot;

		
		private Vector3 shootDirection;

		
		protected override void Start()
		{
			base.Start();
			isShoot = false;
			isActivePassive = false;
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<NadineSkillActive1Data>.inst.PassiveBuffState);
			if (Caster.IsHaveStateByGroup(data.group, Caster.ObjectId))
			{
				isActivePassive = Singleton<NadineSkillActive1Data>.inst.PassiveStackCount <=
				                  Caster.GetStackByGroup(data.group, Caster.ObjectId);
			}

			AddState(Caster, Singleton<NadineSkillActive1Data>.inst.BuffState);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			float concentrationTime = SkillConcentrationTime;
			float waitingTime = Singleton<NadineSkillActive1Data>.inst.WaitingTime;
			while (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - startTime <
				concentrationTime + waitingTime && !isShoot)
			{
				yield return WaitForFrame();
			}

			if (isShoot)
			{
				PlaySkillAction(Caster, 1);
				LookAtDirection(Caster, shootDirection);
				FinishConcentration(false);
				float num = (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - startTime) /
				            concentrationTime;
				if (1f < num)
				{
					num = 1f;
				}

				float attackRange = Caster.Stat.AttackRange;
				float b = Caster.Stat.AttackRange * (isActivePassive
					? Singleton<NadineSkillActive1Data>.inst.MaxSkillRange2
					: Singleton<NadineSkillActive1Data>.inst.MaxSkillRange);
				float distance = Mathf.Lerp(attackRange, b, num);
				float coef = Mathf.Lerp(Singleton<NadineSkillActive1Data>.inst.MinSkillApCoef,
					Singleton<NadineSkillActive1Data>.inst.MaxSkillApCoef, num);
				int damage = (int) Mathf.Lerp(Singleton<NadineSkillActive1Data>.inst.MinDamageByLevel[SkillLevel],
					Singleton<NadineSkillActive1Data>.inst.MaxDamageByLevel[SkillLevel], num);
				int equipWeaponMasteryType = (int) GetEquipWeaponMasteryType(Caster);
				CharacterStateData data =
					GameDB.characterState.GetData(Singleton<NadineSkillActive1Data>.inst.PassiveBuffState);
				int stackByGroup = Caster.GetStackByGroup(data.group, Caster.ObjectId);
				damage += stackByGroup;
				ProjectileProperty projectile = PopProjectileProperty(Caster,
					Singleton<NadineSkillActive1Data>.inst.ProjectileCode[equipWeaponMasteryType]);
				projectile.SetTargetDirection(shootDirection);
				projectile.SetDistance(distance);
				projectile.SetActionOnCollisionCharacter(
					delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
						Vector3 damageDirection)
					{
						parameterCollection.Clear();
						parameterCollection.Add(SkillScriptParameterType.Damage, damage);
						parameterCollection.Add(SkillScriptParameterType.DamageApCoef, coef);
						DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
							projectile.ProjectileData.damageSubType, 0, parameterCollection,
							projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
					});
				WorldProjectile target = LaunchProjectile(projectile);
				Caster.AttachSight(target, 0.7f, 0f, false);
			}
			else
			{
				FinishConcentration(true);
				SpHealTo(Caster, 0, 0f, (int) (SkillCost * Singleton<NadineSkillActive1Data>.inst.RecoverySpRatio),
					true, 0);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			CharacterStateData data = GameDB.characterState.GetData(Singleton<NadineSkillActive1Data>.inst.BuffState);
			Caster.RemoveStateByGroup(data.group, Caster.ObjectId);
		}

		
		protected override void OnPlayAgain(Vector3 hitPoint)
		{
			base.OnPlayAgain(hitPoint);
			isShoot = true;
			shootDirection = GameUtil.Direction(Caster.Position, GetSkillPoint(hitPoint));
		}

		
		protected override void GetSkillRange(ref float minRange, ref float maxRange)
		{
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<NadineSkillActive1Data>.inst.PassiveBuffState);
			bool flag = false;
			if (Caster.IsHaveStateByGroup(data.group, Caster.ObjectId))
			{
				flag = Singleton<NadineSkillActive1Data>.inst.PassiveStackCount <=
				       Caster.GetStackByGroup(data.group, Caster.ObjectId);
			}

			minRange = Caster.Stat.AttackRange;
			maxRange = Caster.Stat.AttackRange *
			           (flag
				           ? Singleton<NadineSkillActive1Data>.inst.MaxSkillRange2
				           : Singleton<NadineSkillActive1Data>.inst.MaxSkillRange);
		}
	}
}