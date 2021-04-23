using System.Collections;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.MagnusActive4)]
	public class MagnusActive4 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private float crossDirection;

		
		private bool isCurve;

		
		private bool isSkip;

		
		private CollisionBox3D projectileCollision;

		
		private Vector3 targetDirection;

		
		
		public override bool CanStopDuringSkillPlaying => false;

		
		protected override void Start()
		{
			base.Start();
			isSkip = false;
			isCurve = false;
			targetDirection = GameUtil.DirectionOnPlane(Caster.Position, info.cursorPosition);
			LookAtDirection(Caster, targetDirection);
			crossDirection = 0f;
			float value = Singleton<MagnusSkillActive4Data>.inst.SkillDuration[SkillLevel];
			AddState(Caster, Singleton<MagnusSkillActive4Data>.inst.BuffState, value);
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active2_1);
			LockSkillSlot(SkillSlotSet.Active3_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			Caster.MoveInDirection(targetDirection);
			StartConcentration();
			float moveStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			float rotateElapsedTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			float skillDuration = Singleton<MagnusSkillActive4Data>.inst.SkillDuration[SkillLevel];
			while (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - moveStartTime < skillDuration &&
			       !isSkip && !CheckCollisionCharacter() && !Caster.IsStopped())
			{
				if (isCurve && 0.1f < MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime -
					rotateElapsedTime)
				{
					rotateElapsedTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
					Vector3 forward = Caster.Forward;
					forward.y = 0f;
					Vector3 vector = Vector3.Cross(forward, targetDirection);
					if (0.001f < vector.magnitude)
					{
						if (vector.y < 0f)
						{
							if (0f < crossDirection)
							{
								isCurve = false;
								Caster.MoveInDirection(targetDirection);
							}
						}
						else if (0f < vector.y && crossDirection < 0f)
						{
							isCurve = false;
							Caster.MoveInDirection(targetDirection);
						}
					}
				}

				yield return WaitForFrame();
			}

			FinishConcentration(false);
			if (!Caster.IsStopped())
			{
				Caster.StopMove();
			}

			PlaySkillAction(Caster, 1);
			yield return WaitForSeconds(Singleton<MagnusSkillActive4Data>.inst.SkillAttackDelay);
			ProjectileProperty projectile =
				PopProjectileProperty(Caster, Singleton<MagnusSkillActive4Data>.inst.ProjectileCode);
			projectile.SetTargetDirection(Caster.Forward);
			projectile.SetActionOnExplosion(delegate(SkillAgent targetAgent, AttackerInfo attackerInfo,
				Vector3 damagePoint, Vector3 damageDirection)
			{
				parameterCollection.Clear();
				int skillLevel = SkillLevel;
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<MagnusSkillActive4Data>.inst.DamageByLevel[skillLevel]);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<MagnusSkillActive4Data>.inst.SkillApCoef[skillLevel]);
				DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
					projectile.ProjectileData.damageSubType, 0, parameterCollection,
					projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
			});
			LaunchProjectile(projectile);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			CharacterStateData data = GameDB.characterState.GetData(Singleton<MagnusSkillActive4Data>.inst.BuffState);
			if (Caster.IsHaveStateByGroup(data.group, Caster.ObjectId))
			{
				Caster.RemoveStateByGroup(data.group, Caster.ObjectId);
			}

			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active2_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
		}

		
		private bool CheckCollisionCharacter()
		{
			Vector3 forward = Caster.Forward;
			Vector3 position = Caster.Position + forward * Singleton<MagnusSkillActive4Data>.inst.CheckMoveOnRide;
			if (projectileCollision == null)
			{
				projectileCollision = new CollisionBox3D(position, 0f, 0f, forward);
			}
			else
			{
				projectileCollision.UpdatePosition(position);
				projectileCollision.UpdateNormalized(forward);
				projectileCollision.UpdateWidth(Singleton<MagnusSkillActive4Data>.inst.WidthOnRide);
				projectileCollision.UpdateDepth(Singleton<MagnusSkillActive4Data>.inst.DepthOnRide);
			}

			return GetEnemyCharacters(projectileCollision).Any<SkillAgent>();
		}

		
		protected override void OnPlayAgain(Vector3 hitPoint)
		{
			base.OnPlayAgain(hitPoint);
			isSkip = true;
		}

		
		public override bool UseOnMove()
		{
			return true;
		}

		
		public override void OnMove(Vector3 hitPoint)
		{
			targetDirection = GameUtil.DirectionOnPlane(Caster.Position, hitPoint);
			Vector3 forward = Caster.Forward;
			forward.y = 0f;
			Vector3 vector = Vector3.Cross(forward, targetDirection);
			if (1E-45f < vector.sqrMagnitude)
			{
				crossDirection = vector.y;
				if (vector.y < 0f)
				{
					isCurve = true;
					Caster.MoveInCurve(-Singleton<MagnusSkillActive4Data>.inst.BikeCurveSpeed);
					return;
				}

				if (0f < vector.y)
				{
					isCurve = true;
					Caster.MoveInCurve(Singleton<MagnusSkillActive4Data>.inst.BikeCurveSpeed);
					return;
				}

				isCurve = false;
				Caster.MoveInDirection(forward);
			}
		}
	}
}