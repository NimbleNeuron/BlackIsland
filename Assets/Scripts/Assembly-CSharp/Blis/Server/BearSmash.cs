using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.BearSmash)]
	public class BearSmash : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private readonly CollisionCircle3D sector = new CollisionCircle3D(Vector3.zero, 0f);

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			yield return WaitForSeconds(SkillConcentrationTime);
			FinishConcentration(false);
			sector.UpdatePosition(Caster.Position + Caster.Forward * Caster.Stat.Radius);
			sector.UpdateRadius(SkillWidth * 0.5f);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(sector);
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.Damage, Singleton<BearSkillSmashData>.inst.SkillDamage);
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<BearSkillSmashData>.inst.SkillApCoef);
			DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection, 2000008);
			AddState(enemyCharacters, 2000009, Singleton<BearSkillSmashData>.inst.StunDuration);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}