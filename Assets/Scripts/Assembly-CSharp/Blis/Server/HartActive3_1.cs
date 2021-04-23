using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HartActive3_1)]
	public class HartActive3_1 : HartSkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			if (IsEvolution())
			{
				AddState(Caster, Singleton<HartSkillActive3Data>.inst.BuffState[SkillEvolutionLevel]);
			}

			Vector3 direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			Vector3 vector;
			bool flag;
			float num;
			Caster.MoveToDirectionForTime(direction, SkillRange, Singleton<HartSkillActive3Data>.inst.DashDuration_1,
				EasingFunction.Ease.EaseOutCubic, true, out vector, out flag, out num);
			yield return WaitForSeconds(Singleton<HartSkillActive3Data>.inst.DashDuration_1);
			PlaySkillAction(Caster, 1);
			CollisionObject3D collisionObject =
				new CollisionCircle3D(Caster.Position, Singleton<HartSkillActive3Data>.inst.SkillAttackRange);
			SkillAgent skillAgent = GetEnemyCharacters(collisionObject).NearestOne(Caster.Position);
			if (skillAgent != null)
			{
				int projectileCode;
				if (IsEvolution())
				{
					projectileCode = IsEnchanted()
						? Singleton<HartSkillActive3Data>.inst.EnchantedEvolutionProjectileCode_1
						: Singleton<HartSkillActive3Data>.inst.EvolutionProjectileCode_1;
				}
				else
				{
					projectileCode = IsEnchanted()
						? Singleton<HartSkillActive3Data>.inst.EnchantedProjectileCode_1
						: Singleton<HartSkillActive3Data>.inst.ProjectileCode_1;
				}

				LaunchProjectile(skillAgent.ObjectId, projectileCode,
					Singleton<HartSkillActive3Data>.inst.DamageByLevel[SkillLevel],
					Singleton<HartSkillActive3Data>.inst.SkillApCoef);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}