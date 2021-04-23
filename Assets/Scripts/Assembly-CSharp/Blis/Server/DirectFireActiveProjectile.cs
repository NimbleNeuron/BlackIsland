using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.DirectFireActiveProjectile)]
	public class DirectFireActiveProjectile : SkillScript
	{
		
		private const float CaltropGap = 0.4f;

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (extraData as WorldProjectile != null)
			{
				ProjectileData data =
					GameDB.projectile.GetData(Singleton<DirectFireSkillActiveData>.inst.ProjectileCode_2);
				float caltropDistance = 0.4f + data.collisionObjectRadius;
				CreateCaltrop(Singleton<DirectFireSkillActiveData>.inst.InnerCaltropCount, caltropDistance, data.code);
				float caltropDistance2 = 0.8f + data.collisionObjectRadius * 3f;
				CreateCaltrop(Singleton<DirectFireSkillActiveData>.inst.OuterCaltropCount, caltropDistance2, data.code);
			}

			yield return WaitForFrame();
			Finish();
		}

		
		private void CreateCaltrop(int caltropCount, float caltropDistance, int projectileCode)
		{
			float num = 360f / caltropCount;
			for (int i = 0; i < caltropCount; i++)
			{
				ProjectileProperty projectile = PopProjectileProperty(Caster.Owner.SkillAgent, projectileCode);
				Vector3 targetDirection = Quaternion.AngleAxis(num * (i + 1), Vector3.up) * Caster.Forward;
				projectile.SetTargetDirection(targetDirection);
				projectile.SetDistance(caltropDistance);
				projectile.SetSpeed(projectile.Distance, projectile.ProjectileData.duration);
				projectile.SetActionOnCollisionCharacter(
					delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
						Vector3 damageDirection)
					{
						int skillLevel = SkillLevel;
						CharacterStateData data =
							GameDB.characterState.GetData(
								Singleton<DirectFireSkillActiveData>.inst.DebuffState_2[skillLevel]);
						float num2 = targetAgent.IsHaveStateByGroup(data.group, Caster.Owner.ObjectId)
							? Singleton<DirectFireSkillActiveData>.inst.DamageReduce_2
							: 1f;
						parameterCollection.Clear();
						parameterCollection.Add(SkillScriptParameterType.Damage,
							Singleton<DirectFireSkillActiveData>.inst.DamageByLevel_2[skillLevel] * num2);
						parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
							Singleton<DirectFireSkillActiveData>.inst.SkillApCoef_2 * num2);
						DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
							projectile.ProjectileData.damageSubType, 0, parameterCollection,
							projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
						AddState(targetAgent, Singleton<DirectFireSkillActiveData>.inst.DebuffState_2[skillLevel]);
					});
				LaunchProjectile(projectile, Caster.Position);
			}
		}
	}
}