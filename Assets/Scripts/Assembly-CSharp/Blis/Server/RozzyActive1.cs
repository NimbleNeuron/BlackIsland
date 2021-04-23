using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.RozziActive1)]
	public class RozzyActive1 : SkillScript
	{
		
		private CollisionBox3D collision;

		
		private readonly SkillScriptParameterCollection damageParam = SkillScriptParameterCollection.Create();

		
		private Vector3? hitPoint;

		
		private bool isHit;

		
		protected override void Start()
		{
			Vector3 direction = GameUtil.Direction(Caster.Position, info.cursorPosition);
			LookAtDirection(Caster, direction);
			CasterLockRotation(true);
			base.Start();
			hitPoint = null;
			isHit = false;
			if (collision == null)
			{
				collision = new CollisionBox3D(Vector3.zero, 0f, 0f, Vector3.zero);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 dir = Caster.Forward;
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			collision.UpdatePosition(Caster.Position +
			                         dir * (Singleton<RozziSkillActive1Data>.inst.Active1DamageLength * 0.5f));
			collision.UpdateWidth(SkillWidth);
			collision.UpdateDepth(Singleton<RozziSkillActive1Data>.inst.Active1DamageLength);
			collision.UpdateNormalized(dir);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
			foreach (SkillAgent target in enemyCharacters)
			{
				damageParam.Clear();
				damageParam.Add(SkillScriptParameterType.Damage,
					Singleton<RozziSkillActive1Data>.inst.DamageByLevel[SkillLevel]);
				damageParam.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<RozziSkillActive1Data>.inst.DamageApCoef);
				DamageTo(target, DamageType.Skill, DamageSubType.Normal, 0, damageParam,
					Singleton<RozziSkillActive1Data>.inst.Active1HitEffectCode);
			}

			isHit = enemyCharacters.Count > 0;
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (isHit)
			{
				CommonState commonState =
					CreateState<CommonState>(Caster, Singleton<RozziSkillActive1Data>.inst.Active1MoveStateCode);
				if (hitPoint != null)
				{
					commonState.SetExtraData(hitPoint.Value);
				}

				AddState(Caster, commonState);
				ModifySkillCooldown(Caster, SkillSlotSet.Active1_1,
					Singleton<RozziSkillActive1Data>.inst.Aactive1CooldownReduce);
			}
		}

		
		public override bool UseOnMove()
		{
			return true;
		}

		
		public override void OnMove(Vector3 hitPoint)
		{
			base.OnMove(hitPoint);
			this.hitPoint = hitPoint;
		}

		
		public override bool UseOnTargetOn()
		{
			return true;
		}

		
		public override void OnTargetOn(WorldObject target)
		{
			base.OnTargetOn(target);
			hitPoint = target.GetPosition();
		}
	}
}