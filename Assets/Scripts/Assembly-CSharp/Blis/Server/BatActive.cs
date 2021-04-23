using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.BatActive)]
	public class BatActive : SkillScript
	{
		
		private const float NOCK_BACK_TIME = 0.5f;

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private CollisionSector3D sector;

		
		protected override void Start()
		{
			base.Start();
			if (sector == null)
			{
				sector = new CollisionSector3D(Vector3.zero, info.SkillRange, info.SkillAngle, Vector3.zero);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 lookDirection = GameUtil.Direction(Caster.Position, info.cursorPosition);
			LookAtDirection(Caster, lookDirection);
			CasterLockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			sector.UpdatePosition(Caster.Position);
			sector.UpdateNormalized(lookDirection);
			sector.UpdateRadius(info.SkillRange);
			sector.UpdateAngle(info.SkillAngle);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(sector);
			int skillLevel = SkillLevel;
			foreach (SkillAgent skillAgent in enemyCharacters)
			{
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<BatSkillActiveData>.inst.DamageByLevel[skillLevel]);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<BatSkillActiveData>.inst.SkillApCoef[skillLevel]);
				DamageTo(skillAgent, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection, 2000017);
				Vector3 vector = GameUtil.DirectionOnPlane(Caster.Position, skillAgent.Position);
				if (vector == Vector3.zero)
				{
					vector = lookDirection;
				}

				KnockbackState knockbackState = CreateState<KnockbackState>(skillAgent, 2000010);
				knockbackState.Init(vector, Singleton<BatSkillActiveData>.inst.KnockBackDistance, 0.5f,
					EasingFunction.Ease.EaseOutQuad, false);
				knockbackState.SetActionOnCollisionWall(delegate(SkillAgent self)
				{
					AddState(self, 2000009, Singleton<BatSkillActiveData>.inst.StunDuration);
				});
				skillAgent.AddState(knockbackState, Caster.ObjectId);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}