using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ShoichiActive2)]
	public class ShoichiActive2 : SkillScript
	{
		
		private readonly List<WorldSummonBase> passiveDaggers = new List<WorldSummonBase>();

		
		private CollisionBox3D collision;

		
		protected override void Start()
		{
			base.Start();
			if (collision == null)
			{
				collision = new CollisionBox3D(Vector3.zero, SkillWidth, 0f, Vector3.zero);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 targetPosition = Target.Position;
			Vector3 vector = targetPosition - Caster.Position;
			Vector3 dir = vector.normalized;
			LookAtDirection(Caster, dir);
			CasterLockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			if (!(Target.Character is WorldSummonBase))
			{
				targetPosition += dir;
			}

			float num = Vector3.Distance(Caster.Position, targetPosition);
			Vector3 position = Caster.Position + dir * num * 0.5f;
			collision.UpdatePosition(position);
			collision.UpdateDepth(num);
			collision.UpdateWidth(SkillWidth);
			collision.UpdateNormalized(dir);
			List<SkillAgent> allies = GetAllies(collision);
			passiveDaggers.Clear();
			for (int i = 0; i < allies.Count; i++)
			{
				if (allies[i].Character is WorldSummonBase)
				{
					WorldSummonBase worldSummonBase = allies[i].Character as WorldSummonBase;
					if (!(worldSummonBase.Owner == null) && worldSummonBase.Owner.ObjectId == Caster.ObjectId &&
					    worldSummonBase.SummonData.code ==
					    Singleton<ShoichiSkillPassiveData>.inst.PassiveSummonObjectId)
					{
						passiveDaggers.Add(worldSummonBase);
					}
				}
			}

			for (int j = 0; j < passiveDaggers.Count; j++)
			{
				passiveDaggers[j].SelfCustomAction(passiveDaggers[j]);
			}

			List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
			bool flag;
			float num2;
			Caster.MoveToDestinationForTime(targetPosition, 0f, EasingFunction.Ease.Linear, true, out vector, out flag,
				out num2);
			AddState(enemyCharacters, Singleton<ShoichiSkillActive2Data>.inst.DebuffState);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override UseSkillErrorCode IsCanUseSkill(WorldCharacter hitTarget, Vector3? cursorPosition,
			WorldMovableCharacter caster)
		{
			WorldSummonBase worldSummonBase;
			if ((worldSummonBase = hitTarget as WorldSummonBase) != null)
			{
				if (caster.GetHostileType(hitTarget) == HostileType.Enemy)
				{
					return UseSkillErrorCode.NotInvalidTarget;
				}

				if (worldSummonBase.Owner == null)
				{
					return UseSkillErrorCode.NotInvalidTarget;
				}

				if (worldSummonBase.Owner.ObjectId != caster.ObjectId)
				{
					return UseSkillErrorCode.NotInvalidTarget;
				}

				if (!worldSummonBase.SummonData.code.Equals(Singleton<ShoichiSkillPassiveData>.inst
					.PassiveSummonObjectId))
				{
					return UseSkillErrorCode.NotInvalidTarget;
				}
			}
			else if (!caster.IsAttackable(hitTarget))
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			return UseSkillErrorCode.None;
		}
	}
}