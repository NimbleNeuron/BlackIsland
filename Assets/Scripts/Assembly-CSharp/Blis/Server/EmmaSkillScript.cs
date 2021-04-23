using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public abstract class EmmaSkillScript : SkillScript
	{
		
		protected static bool IsPigeon(WorldSummonBase summon)
		{
			return summon.SummonData.code == Singleton<EmmaSkillActive1Data>.inst.PigeonSummonCode;
		}

		
		protected static bool IsFireworkHat(WorldSummonBase summon)
		{
			return summon.SummonData.code == Singleton<EmmaSkillActive2Data>.inst.FireworkHatSummonCode;
		}

		
		protected void LaunchMagicRabbitBeamProjectile(SkillSlotIndex skillSlotIndex, int enemyObjectId)
		{
			ProjectileProperty projectileProperty = PopProjectileProperty(Caster,
				Singleton<EmmaSkillActive3Data>.inst.MagicRabbitBeamProjectileCode);
			projectileProperty.SetTargetObject(enemyObjectId);
			projectileProperty.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					if (skillSlotIndex == SkillSlotIndex.Active3)
					{
						AddState(targetAgent, Singleton<EmmaSkillActive3Data>.inst.MagicRabbitStateCode[SkillLevel]);
						AddState(targetAgent, Singleton<EmmaSkillActive3Data>.inst.MagicRabbitFetterStateCode);
						return;
					}

					if (skillSlotIndex == SkillSlotIndex.Active4)
					{
						AddState(targetAgent, Singleton<EmmaSkillActive4Data>.inst.MagicRabbitStateCode);
						AddState(targetAgent, Singleton<EmmaSkillActive4Data>.inst.MagicRabbitFetterStateCode);
					}
				});
			LaunchProjectile(projectileProperty);
		}
	}
}