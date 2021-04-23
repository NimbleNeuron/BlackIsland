using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ZahirActive2)]
	public class ZahirActive2 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
				LookAtPosition(Caster, info.cursorPosition);
			}

			Vector3 targetDirection = GameUtil.Direction(Caster.Position, GetSkillPoint());
			ProjectileProperty projectile =
				PopProjectileProperty(Caster, Singleton<ZahirSkillActive2Data>.inst.ProjectileCode);
			projectile.SetTargetDirection(targetDirection);
			projectile.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					if (targetAgent.IsHaveStateByGroup(Singleton<ZahirSkillPassiveData>.inst.PassiveDebuffStateGroup,
						Caster.ObjectId))
					{
						AddState(targetAgent, Singleton<ZahirSkillActive2Data>.inst.DebuffState);
						ModifySkillCooldown(Caster,
							SkillSlotSet.Active1_1 | SkillSlotSet.Active3_1 | SkillSlotSet.Active4_1,
							Singleton<ZahirSkillActive2Data>.inst.SkillCooldownReduce);
					}

					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<ZahirSkillActive2Data>.inst.DamageByLevel[SkillLevel]);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<ZahirSkillActive2Data>.inst.SkillApCoef);
					DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
						projectile.ProjectileData.damageSubType, 0, parameterCollection,
						projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
				});
			LaunchProjectile(projectile);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}