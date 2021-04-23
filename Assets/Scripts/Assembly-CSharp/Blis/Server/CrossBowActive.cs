using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.CrossBowActive)]
	public class CrossBowActive : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection parameterCollectionStun =
			SkillScriptParameterCollection.Create();

		
		private readonly List<WorldProjectile> targetSharedProjectiles = new List<WorldProjectile>();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			CasterLockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			targetSharedProjectiles.Clear();
			Vector3 point = -Caster.Right;
			for (int i = 0; i < 9; i++)
			{
				Vector3 direction = Quaternion.Euler(0f, 45f + i * 10f, 0f) * point;
				ProjectileProperty projectile =
					PopProjectileProperty(Caster, Singleton<CrossBowSkillActiveData>.inst.ProjectileCode);
				projectile.SetTargetDirection(direction);
				projectile.SetActionOnCollisionCharacter(
					delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
						Vector3 damageDirection)
					{
						parameterCollection.Clear();
						parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
							Singleton<CrossBowSkillActiveData>.inst.SkillApCoef[SkillLevel]);
						DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
							projectile.ProjectileData.damageSubType, 0, parameterCollection,
							projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
						KnockbackState knockbackState = CreateState<KnockbackState>(targetAgent, 2000010);
						knockbackState.Init(direction,
							Singleton<CrossBowSkillActiveData>.inst.KnockBackDistance[SkillLevel], 0.3f,
							EasingFunction.Ease.EaseOutQuad, false);
						knockbackState.SetActionOnCollisionWall(delegate(SkillAgent self)
						{
							AddState(self, 2000009, Singleton<CrossBowSkillActiveData>.inst.StunDuration[SkillLevel]);
							parameterCollectionStun.Clear();
							parameterCollectionStun.Add(SkillScriptParameterType.DamageApCoef,
								Singleton<CrossBowSkillActiveData>.inst.SkillApCoef[SkillLevel]);
							DamageTo(self, DamageType.Skill, DamageSubType.Normal, 0, parameterCollectionStun, 0);
						});
						targetAgent.AddState(knockbackState, Caster.ObjectId);
					});
				WorldProjectile item = LaunchProjectile(projectile);
				targetSharedProjectiles.Add(item);
			}

			for (int j = 0; j < targetSharedProjectiles.Count; j++)
			{
				targetSharedProjectiles[j].ShareTarget(targetSharedProjectiles);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}