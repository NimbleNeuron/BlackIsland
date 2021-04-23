using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.EmmaActive2)]
	public class EmmaActive2 : EmmaSkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			LookAtPosition(Caster, info.cursorPosition);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (0f < SkillCastingTime1)
			{
				yield return FirstCastingTime();
			}

			WorldPlayerCharacter worldPlayerCharacter = Caster.Character as WorldPlayerCharacter;
			List<WorldSummonTrap> fireworkHats = GetFireworkHats(worldPlayerCharacter);
			if (fireworkHats != null && 0 < fireworkHats.Count)
			{
				foreach (WorldSummonTrap obj in fireworkHats)
				{
					MonoBehaviourInstance<GameService>.inst.Spawn.DestroyWorldObject(obj);
				}
			}

			WorldSummonBase worldSummonBase = MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(
				worldPlayerCharacter, Singleton<EmmaSkillActive2Data>.inst.FireworkHatSummonCode, info.cursorPosition);
			PlaySkillAction(Caster, 1, worldSummonBase.SkillAgent);
			ProjectileProperty projectileProperty = PopProjectileProperty(Caster,
				Singleton<EmmaSkillActive2Data>.inst.FireworkHatExplosionAreaProjectileCode);
			WorldProjectile target = LaunchProjectile(projectileProperty, info.cursorPosition);
			projectileProperty.SetExplosionSkill(SkillId.EmmaActive2Explosion);
			Caster.AttachSight(target, projectileProperty.ProjectileData.explosionRadius,
				projectileProperty.ProjectileData.lifeTimeAfterArrival, false);
			if (0f < SkillFinishDelayTime)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish();
		}

		
		private static List<WorldSummonTrap> GetFireworkHats(WorldPlayerCharacter owner)
		{
			List<WorldSummonBase> ownSummons = owner.GetOwnSummons(IsFireworkHat);
			if (ownSummons == null || ownSummons.Count <= 0)
			{
				return null;
			}

			List<WorldSummonTrap> list = new List<WorldSummonTrap>();
			foreach (WorldSummonBase worldSummonBase in ownSummons)
			{
				list.Add(worldSummonBase as WorldSummonTrap);
			}

			return list;
		}
	}
}