using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LukeActive1_1)]
	public class LukeActive1_1 : SkillScript
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
			int dataCode = IsEvolution()
				? Singleton<LukeSkillActive1_1Data>.inst.EvolutionProjectileCode
				: Singleton<LukeSkillActive1_1Data>.inst.ProjectileCode;
			ProjectileProperty projectile = PopProjectileProperty(Caster, dataCode);
			projectile.SetTargetDirection(targetDirection);
			projectile.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackInfo, Vector3 damagePoint, Vector3 damageDirection)
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<LukeSkillActive1_1Data>.inst.BaseDamage[SkillLevel]);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<LukeSkillActive1_1Data>.inst.DamageApCoef);
					DamageTo(targetAgent, attackInfo, projectile.ProjectileData.damageType,
						projectile.ProjectileData.damageSubType, 0, parameterCollection,
						projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
					if (targetAgent.IsAlive)
					{
						AddState(Caster, Singleton<LukeSkillActive1_1Data>.inst.BuffCode);
						AddState(targetAgent, Singleton<LukeSkillActive1_1Data>.inst.DebuffCode);
					}
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