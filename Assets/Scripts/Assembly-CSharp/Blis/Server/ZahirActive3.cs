using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ZahirActive3)]
	public class ZahirActive3 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtDirection(Caster, info.cursorPosition);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
				LookAtDirection(Caster, info.cursorPosition);
			}

			Vector3 targetDirection = GameUtil.Direction(Caster.Position, GetSkillPoint());
			ProjectileProperty projectile =
				PopProjectileProperty(Caster, Singleton<ZahirSkillActive3Data>.inst.ProjectileCode);
			projectile.SetTargetDirection(targetDirection);
			projectile.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					if (targetAgent.IsHaveStateByGroup(Singleton<ZahirSkillPassiveData>.inst.PassiveDebuffStateGroup,
						Caster.ObjectId))
					{
						AirborneState airborneState = CreateState<AirborneState>(targetAgent, 2000001, 0,
							Singleton<ZahirSkillActive3Data>.inst.BigAirborneDuration);
						airborneState.Init(Singleton<ZahirSkillActive3Data>.inst.BigAirborneDuration);
						AddState(targetAgent, airborneState);
					}
					else
					{
						AirborneState airborneState2 = CreateState<AirborneState>(targetAgent, 2000001, 0,
							Singleton<ZahirSkillActive3Data>.inst.AirborneDuration);
						if (airborneState2 != null)
						{
							airborneState2.Init(Singleton<ZahirSkillActive3Data>.inst.AirborneDuration);
						}

						AddState(targetAgent, airborneState2);
						AddState(targetAgent, Singleton<ZahirSkillActive3Data>.inst.DebuffState);
					}

					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<ZahirSkillActive3Data>.inst.DamageByLevel[SkillLevel]);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<ZahirSkillActive3Data>.inst.SkillApCoef);
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