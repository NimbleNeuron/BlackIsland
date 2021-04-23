using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SisselaActive3)]
	public class SisselaActive3 : SisselaSkillScript
	{
		
		public const SkillSlotSet skillSlotSetFlag = SkillSlotSet.Active2_1 | SkillSlotSet.Active3_1;

		
		private bool isProjectileFired;

		
		private WorldSummonServant wilson;

		
		protected override void Start()
		{
			if (wilson == null)
			{
				WorldPlayerCharacter owner = (WorldPlayerCharacter) Caster.Character;
				wilson = GetWilson(owner);
			}

			wilson.SetCanControl(false);
			base.Start();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (!isProjectileFired)
			{
				LockSkillSlotWithPacket(SkillSlotSet.Active2_1 | SkillSlotSet.Active3_1, false);
				wilson.SetCanControl(true);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 direction = GameUtil.Direction(wilson.GetPosition(), GetSkillPoint());
			Vector3 planeCursorDir = new Vector3(direction.x, 0f, direction.z).normalized;
			if (IsWilsonUnion())
			{
				SetWilsonState(false, wilson);
				wilson.WarpTo(
					planeCursorDir * Singleton<SisselaSkillData>.inst.A3SeparateStartDistance + wilson.GetPosition(),
					false);
			}

			LockSkillSlotWithPacket(SkillSlotSet.Active2_1 | SkillSlotSet.Active3_1, true);
			isProjectileFired = false;
			wilson.LookAt(direction);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			Vector3 spawnPosition = wilson.GetPosition() +
			                        Singleton<SisselaSkillData>.inst.A3ProjectileStartPointAdd * planeCursorDir;
			ProjectileProperty projectileProperty =
				PopProjectileProperty(Caster, Singleton<SisselaSkillData>.inst.A3ProjectileCode);
			projectileProperty.SetTargetDirection(direction);
			projectileProperty.SetActionOnArrive(ProjectileOnArrive);
			projectileProperty.SetActionOnCollisionCharacter(ProjectileOnCollisionCharacter);
			WorldProjectile projectile = LaunchProjectile(projectileProperty, spawnPosition);
			PlaySkillAction(Caster, 3, projectile.SkillAgent);
			isProjectileFired = true;
			while (projectile != null && projectile.IsAlive)
			{
				yield return WaitForFrame();
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		private void ProjectileOnArrive(Vector3 arrivePos, bool isCollision, WorldProjectile worldProjectile)
		{
			if (isCollision)
			{
				return;
			}

			PlaySkillAction(Caster, 1);
			LockSkillSlotWithPacket(SkillSlotSet.Active2_1 | SkillSlotSet.Active3_1, false);
			wilson.SetCanControl(true);
		}

		
		private void ProjectileOnCollisionCharacter(SkillAgent targetAgent, AttackerInfo attackerInfo,
			Vector3 collisionPoint, Vector3 collisionDirection)
		{
			GrabState grabState = CreateState<GrabState>(targetAgent, Singleton<SisselaSkillData>.inst.A3GrabState);
			grabState.Init(wilson, Singleton<SisselaSkillData>.inst.A3GrabSpeed);
			AddState(targetAgent, grabState);
			if (!targetAgent.IsHaveStateByGroup(
				GameDB.characterState.GetData(Singleton<SisselaSkillData>.inst.A3GrabState).group, Caster.ObjectId))
			{
				ProjectileOnArrive(collisionPoint, false, null);
				return;
			}

			PlaySkillAction(Caster, 2, targetAgent);
		}

		
		protected override Vector3 GetSkillSubjectPosition()
		{
			return wilson.GetPosition();
		}
	}
}