using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AdrianaActive3)]
	public class AdrianaActive3 : AdrianaSkillScript
	{
		
		private CollisionCircle3D collisionFireFlame;

		
		protected override void Start()
		{
			base.Start();
			LockSkillSlot(SkillSlotSet.Active2_1);
			LockSkillSlot(SkillSlotSet.Active4_1);
			if (collisionFireFlame == null)
			{
				collisionFireFlame = new CollisionCircle3D(Vector3.zero, 0f);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (0f < SkillCastingTime1)
			{
				yield return FirstCastingTime();
				LookAtPosition(Caster, info.cursorPosition);
			}

			CasterLockRotation(true);
			PlaySkillAction(Caster, 1);
			Vector3 direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			AdrianaSkillActive3Data adrianaSkillActive3Data = Singleton<AdrianaSkillActive3Data>.inst;
			GameService gameService = MonoBehaviourInstance<GameService>.inst;
			float moveStartTime = gameService.CurrentServerFrameTime;
			Vector3 vector;
			bool flag;
			float finalDuration;
			Caster.MoveToDirectionForTime(direction, adrianaSkillActive3Data.DashDistance,
				adrianaSkillActive3Data.DashDuration, EasingFunction.Ease.Linear, true, out vector, out flag,
				out finalDuration);
			while (Caster.IsMoving() && gameService.CurrentServerFrameTime - moveStartTime <= finalDuration)
			{
				ProjectileProperty projectileProperty =
					PopProjectileProperty(Caster, adrianaSkillActive3Data.FireFlame2ProjectileCode);
				projectileProperty.SetTargetDirection(Caster.Forward);
				WorldProjectile target = LaunchProjectile(projectileProperty, Caster.Position);
				Caster.AttachSight(target, SkillInnerRange, projectileProperty.ProjectileData.lifeTimeAfterArrival,
					false);
				collisionFireFlame.UpdatePosition(Caster.Position);
				collisionFireFlame.UpdateRadius(projectileProperty.ProjectileData.collisionObjectRadius);
				if (ProcessChangeProjectileFromOilAreaToFireFlame1(Caster.Character as WorldMovableCharacter,
					collisionFireFlame))
				{
					PlaySkillAction(Caster, 2);
				}

				yield return WaitForSeconds(adrianaSkillActive3Data.FireFlame2ProjectileTerm);
			}

			if (0f < SkillFinishDelayTime)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			UnlockSkillSlot(SkillSlotSet.Active2_1);
			UnlockSkillSlot(SkillSlotSet.Active4_1);
			base.Finish(cancel);
		}
	}
}