using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SilviaHumanActive2)]
	public class SilviaHumanActive2 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damage = SkillScriptParameterCollection.Create();

		
		private readonly HashSet<int> damagedTargetIds = new HashSet<int>();

		
		private CollisionBox3D collision;

		
		private List<SkillAgent> newTargets = new List<SkillAgent>();

		
		protected override void Start()
		{
			base.Start();
			if (collision == null)
			{
				collision = new CollisionBox3D(Vector3.zero, SkillWidth,
					Singleton<SilviaSkillHumanData>.inst.A2SkillDepth, Vector3.zero);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 skillPoint = GetSkillPoint();
			Vector3 dir = GameUtil.Direction(Caster.Position, skillPoint);
			LookAtDirection(Caster, dir);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			damagedTargetIds.Clear();
			collision.UpdatePosition(skillPoint);
			collision.UpdateRadius(SkillWidth);
			collision.UpdateNormalized(dir);
			int skillLv = SkillLevel;
			ProjectileProperty projectileProperty = PopProjectileProperty(Caster,
				Singleton<SilviaSkillHumanData>.inst.A2FinishLineProjectileCode[skillLv]);
			projectileProperty.SetUpdateAction(delegate(WorldProjectile worldProjectile)
			{
				FinishLineCustomAction(worldProjectile, skillLv);
			});
			projectileProperty.SetTargetDirection(dir);
			WorldProjectile target = LaunchProjectile(projectileProperty, skillPoint);
			Caster.AttachSight(target, Singleton<SilviaSkillHumanData>.inst.A2FinishLineSightRange,
				projectileProperty.ProjectileData.lifeTimeAfterArrival, false);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		private void FinishLineCustomAction(WorldProjectile projectile, int skillLv)
		{
			newTargets = GetEnemyCharacters(collision);
			for (int i = newTargets.Count - 1; i >= 0; i--)
			{
				if (damagedTargetIds.Contains(newTargets[i].ObjectId))
				{
					newTargets.RemoveAt(i);
				}
			}

			if (newTargets.Count == 0)
			{
				return;
			}

			float duration = GetDuration(skillLv);
			damage.Clear();
			damage.Add(SkillScriptParameterType.Damage, Singleton<SilviaSkillHumanData>.inst.A2BaseDamage[skillLv]);
			damage.Add(SkillScriptParameterType.DamageApCoef, Singleton<SilviaSkillHumanData>.inst.A2ApDamage);
			DamageTo(newTargets, DamageType.Skill, DamageSubType.Normal, 0, damage,
				Singleton<SilviaSkillHumanData>.inst.A2DamageEffectSoundCode);
			AddState(newTargets, Singleton<SilviaSkillHumanData>.inst.A2BaseDebuffCodes[skillLv], duration);
			for (int j = 0; j < newTargets.Count; j++)
			{
				int objectId = newTargets[j].ObjectId;
				if (objectId != 0)
				{
					damagedTargetIds.Add(objectId);
				}
			}
		}

		
		private float GetDuration(int skillLv)
		{
			float num = GameDB.characterState.GetData(Singleton<SilviaSkillHumanData>.inst.A2BaseDebuffCodes[skillLv])
				.duration;
			bool? flag = GetEquipWeaponMasteryType(Caster).IsLaunchProjectile();
			if (flag != null)
			{
				bool? flag2 = flag;
				bool flag3 = false;
				if ((flag2.GetValueOrDefault() == flag3) & (flag2 != null))
				{
					num += Singleton<SilviaSkillHumanData>.inst.A2NotLaunchProjectileAddTime;
				}
			}

			return num;
		}
	}
}