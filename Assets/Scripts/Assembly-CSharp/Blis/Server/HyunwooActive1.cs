using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyunwooActive1)]
	public class HyunwooActive1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private CollisionSector3D sector;

		
		protected override void Start()
		{
			base.Start();
			if (sector == null)
			{
				sector = new CollisionSector3D(Vector3.zero, SkillRange, SkillAngle, Vector3.zero);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 direction = GameUtil.DirectionOnPlane(Caster.Position, info.cursorPosition);
			LookAtDirection(Caster, direction);
			CasterLockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			sector.UpdatePosition(Caster.Position);
			sector.UpdateNormalized(direction);
			sector.UpdateRadius(SkillRange);
			sector.UpdateAngle(SkillAngle);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(sector);
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<HyunwooSkillActive1Data>.inst.SkillApCoef);
			parameterCollection.Add(SkillScriptParameterType.Damage,
				Singleton<HyunwooSkillActive1Data>.inst.DamageByLevel[SkillLevel]);
			DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection, 0);
			AddState(enemyCharacters, Singleton<HyunwooSkillActive1Data>.inst.DebuffState);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}