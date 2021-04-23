using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.GuitarActive)]
	public class GuitarActive : SkillScript
	{
		
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
			PlaySkillAction(Caster, 1);
			ProjectileProperty projectile =
				PopProjectileProperty(Caster, Singleton<GuitarSkillActiveData>.inst.ProjectileCode);
			projectile.SetTargetDirection(targetDirection);
			projectile.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					SkillScriptParameterCollection skillScriptParameterCollection =
						SkillScriptParameterCollection.Create();
					skillScriptParameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<GuitarSkillActiveData>.inst.SkillApCoef[SkillLevel]);
					DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
						projectile.ProjectileData.damageSubType, 0, skillScriptParameterCollection,
						projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
					CharacterState characterState = CreateState(targetAgent, 2000005, 0,
						Singleton<GuitarSkillActiveData>.inst.CharmDuration);
					characterState.AddExternalStat(StatType.MoveSpeedRatio,
						Singleton<GuitarSkillActiveData>.inst.MoveSpeedRatio, StatType.None, 0f);
					AddState(targetAgent, characterState);
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