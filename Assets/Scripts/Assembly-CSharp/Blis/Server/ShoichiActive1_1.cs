using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ShoichiActive1_1)]
	public class ShoichiActive1_1 : SkillScript
	{
		
		private CollisionBox3D collisionBox;

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
			if (collisionBox == null)
			{
				collisionBox = new CollisionBox3D(Vector3.zero, 0f, 0f, Vector3.zero);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			LookAtDirection(Caster, direction);
			CasterLockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			collisionBox.UpdateNormalized(direction);
			collisionBox.UpdatePosition(Caster.Position + Caster.Forward * SkillRange * 0.5f);
			collisionBox.UpdateWidth(SkillWidth);
			collisionBox.UpdateDepth(SkillRange);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(collisionBox);
			if (enemyCharacters.Count > 0)
			{
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<ShoichiSkillActive1Data>.inst.SkillApCoef);
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<ShoichiSkillActive1Data>.inst.DamageByLevel[SkillLevel]);
				DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
					Singleton<ShoichiSkillActive1Data>.inst.EffectAndSoundCode);
				AddState(enemyCharacters, Singleton<ShoichiSkillActive1Data>.inst.DeBuffStateCode);
				AddState(Caster, Singleton<ShoichiSkillActive1Data>.inst.BuffStateCode);
				Caster.Owner.SetReservationSequenceCooldown(SkillSlotSet.Active1_1,
					Singleton<ShoichiSkillActive1Data>.inst.CooldownReduce);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override bool IsChangedSkillSequence()
		{
			return Caster.IsHaveStateByGroup(Singleton<ShoichiSkillActive1Data>.inst.BuffStateGroup, Caster.ObjectId);
		}
	}
}