using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.BowActiveProjectile)]
	public class BowActiveProjectile : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D circle;

		
		protected override void Start()
		{
			base.Start();
			if (circle == null)
			{
				circle = new CollisionCircle3D(Vector3.zero, 0f);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Caster.AttachSight(Caster.WorldObject, SkillInnerRange, 2f, false);
			if (Singleton<BowSkillActiveData>.inst.SkillDamageDelay > 0f)
			{
				yield return WaitForSeconds(Singleton<BowSkillActiveData>.inst.SkillDamageDelay);
			}

			circle.UpdatePosition(Caster.Position);
			circle.UpdateRadius(SkillInnerRange);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(circle);
			float damageInRangeRadius = Singleton<BowSkillActiveData>.inst.DamageInRangeRadius;
			foreach (SkillAgent skillAgent in enemyCharacters)
			{
				float num = GameUtil.DistanceOnPlane(skillAgent.Position, Caster.Position);
				parameterCollection.Clear();
				int skillLevel = SkillLevel;
				if (num <= damageInRangeRadius)
				{
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<BowSkillActiveData>.inst.DamageByLevel_IN[skillLevel]);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<BowSkillActiveData>.inst.SkillApCoef_IN[skillLevel]);
					DamageTo(skillAgent, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
						Singleton<BowSkillActiveData>.inst.EffectAndSoundWeaponType);
				}
				else
				{
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<BowSkillActiveData>.inst.DamageByLevel_OUT[skillLevel]);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<BowSkillActiveData>.inst.SkillApCoef_OUT[skillLevel]);
					DamageTo(skillAgent, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
						Singleton<BowSkillActiveData>.inst.EffectAndSoundWeaponType);
				}

				AddState(skillAgent, Singleton<BowSkillActiveData>.inst.DebuffState[skillLevel]);
			}

			Finish();
		}
	}
}