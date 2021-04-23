using System;
using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ZahirActive1)]
	public class ZahirActive1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			ProjectileProperty projectile =
				PopProjectileProperty(Caster, Singleton<ZahirSkillActive1Data>.inst.ProjectileCode);
			projectile.SetActionOnExplosion(delegate(SkillAgent targetAgent, AttackerInfo attackerInfo,
				Vector3 damagePoint, Vector3 damageDirection)
			{
				if (targetAgent.IsHaveStateByGroup(Singleton<ZahirSkillPassiveData>.inst.PassiveDebuffStateGroup,
					Caster.ObjectId))
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<ZahirSkillActive1Data>.inst.DamageByLevel_2[SkillLevel]);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<ZahirSkillActive1Data>.inst.SkillApCoef);
					DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
						projectile.ProjectileData.damageSubType, 0, parameterCollection,
						projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
					int num = Math.Max(1, targetAgent.Status.PlayerKill - Caster.Status.PlayerKill);
					int num2 = Math.Max(Singleton<ZahirSkillActive1Data>.inst.ZahirSkillActive1DebuffMinBonus,
						-num * Singleton<ZahirSkillActive1Data>.inst.DebuffStackByLevel_2[SkillLevel]);
					CharacterState characterState =
						CreateState(targetAgent, Singleton<ZahirSkillActive1Data>.inst.DebuffState_2);
					characterState.AddExternalStat(StatType.DefenseRatio, num2, StatType.None, 0f);
					AddState(targetAgent, characterState);
					return;
				}

				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<ZahirSkillActive1Data>.inst.DamageByLevel[SkillLevel]);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<ZahirSkillActive1Data>.inst.SkillApCoef);
				DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
					projectile.ProjectileData.damageSubType, 0, parameterCollection,
					projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
			});
			WorldProjectile worldProjectile = LaunchProjectile(projectile, GetSkillPoint());
			Caster.AttachSight(worldProjectile, SkillInnerRange,
				worldProjectile.Property.ProjectileData.lifeTimeAfterArrival + 1f, false);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}