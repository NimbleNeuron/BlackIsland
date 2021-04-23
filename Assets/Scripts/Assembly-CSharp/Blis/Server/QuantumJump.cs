using System.Collections;
using Blis.Common;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Server
{
	
	[SkillScript(SkillId.QuantumJump)]
	public class QuantumJump : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			if (Caster.IsConcentration())
			{
				(Caster.Character as WorldPlayerCharacter).SkillController.CancelConcentrationSkill(skillScript =>
					!skillScript.CanMoveDuringSkillPlaying);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			Vector3 vector = GameUtil.DirectionOnPlane(Caster.Position, info.cursorPosition);
			Vector3 vector2 = info.cursorPosition;
			bool flag = false;
			if (!Caster.IsLockRotation())
			{
				LookAtDirection(Caster, vector);
				Caster.LockRotation(true);
				flag = true;
			}

			if (GameUtil.DistanceOnPlane(Caster.Position, info.cursorPosition) > SkillRange)
			{
				vector.y = GameUtil.Direction(Caster.Position, info.cursorPosition).y;
				vector2 = Caster.Position + vector * SkillRange;
			}

			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(vector2, out navMeshHit, SkillRange, 2147483640))
			{
				vector2 = navMeshHit.position;
			}

			Caster.WarpTo(vector2);
			Caster.StopMove();
			if (flag)
			{
				Caster.LockRotation(false);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}