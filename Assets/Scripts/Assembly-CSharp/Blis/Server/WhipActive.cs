using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.WhipActive)]
	public class WhipActive : SkillScript
	{
		
		private SkillScriptParameterCollection collisionDamage;

		
		private SkillAgent collisionTarget;

		
		private HookLineProperty hookLineProperty;

		
		private bool waitforProjectile = true;

		
		protected override void Start()
		{
			base.Start();
			if (hookLineProperty == null)
			{
				hookLineProperty = CreateHookLineProjectile(Caster, Singleton<WhipSkillActiveData>.inst.ProjectileCode,
					Singleton<WhipSkillActiveData>.inst.HookLineProjectileCodes[Caster.Character.CharacterCode]);
				hookLineProperty.SetLinkFromCharacter(Caster.Character);
				hookLineProperty.SetActionOnCollisionCharacter(OnCollisionCharacter);
				collisionDamage = SkillScriptParameterCollection.Create();
			}

			waitforProjectile = true;
			collisionTarget = null;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 direction = GameUtil.Direction(Caster.Position, info.cursorPosition);
			LookAtDirection(Caster, direction);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			hookLineProperty.SetTargetDirection(direction);
			WorldHookLineProjectile projectile = LaunchHookLineProjectile(hookLineProperty);
			Caster.AttachSight(projectile, 1f, 0f, false);
			while (waitforProjectile && !projectile.IsArrived)
			{
				yield return WaitForFrame();
			}

			PlaySkillAction(Caster, 1, collisionTarget);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		private void OnCollisionCharacter(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
			Vector3 damageDirection)
		{
			collisionDamage.Clear();
			collisionDamage.Add(SkillScriptParameterType.Damage,
				Singleton<WhipSkillActiveData>.inst.DamageByLevel[SkillLevel]);
			collisionDamage.Add(SkillScriptParameterType.DamageApCoef, Singleton<WhipSkillActiveData>.inst.DamageCoef);
			DamageTo(targetAgent, attackerInfo, DamageType.Skill, DamageSubType.Normal, 0, collisionDamage,
				SkillSlotSet.WeaponSkill, damagePoint, damageDirection,
				Singleton<WhipSkillActiveData>.inst.EffectSoundCode);
			Vector3 vector = GameUtil.Direction(targetAgent.Position, Caster.Position);
			float num = GameUtil.Distance(Caster.Position, targetAgent.Position);
			Vector3 vector2 = Caster.Position - vector * Singleton<WhipSkillActiveData>.inst.KnockBackMinDistance;
			Vector3 vector3;
			float num4;
			if (!MoveAgent.CanStandToPosition(vector2, 2147483640, out vector3))
			{
				Vector3 vector4;
				MoveAgent.CanStraightMoveToDestination(Caster.Position, targetAgent.Position, 2147483640, out vector4);
				Vector3 vector5;
				MoveAgent.CanStraightMoveToDestination(targetAgent.Position, Caster.Position, 2147483640, out vector5);
				float num2 = GameUtil.DistanceOnPlane(vector2, vector4);
				float num3 = GameUtil.DistanceOnPlane(vector2, vector5);
				num4 = GameUtil.DistanceOnPlane(targetAgent.Position, num2 < num3 ? vector4 : vector5);
			}
			else
			{
				num4 = num - Singleton<WhipSkillActiveData>.inst.KnockBackMinDistance;
			}

			bool flag = 0.01f <= num4;
			AirborneState airborneState = CreateState<AirborneState>(targetAgent, 2000001, 0,
				Singleton<WhipSkillActiveData>.inst.AirborneDuration);
			airborneState.Init(Singleton<WhipSkillActiveData>.inst.AirborneDuration,
				Singleton<WhipSkillActiveData>.inst.AirbornePower, !flag);
			targetAgent.AddState(airborneState, Caster.ObjectId);
			if (flag)
			{
				KnockbackState knockbackState = CreateState<KnockbackState>(targetAgent, 2000010, 0,
					Singleton<WhipSkillActiveData>.inst.KnockBackMoveDuration);
				knockbackState.Init(vector, num4, Singleton<WhipSkillActiveData>.inst.KnockBackMoveDuration,
					EasingFunction.Ease.EaseOutQuad, true);
				targetAgent.AddState(knockbackState, Caster.ObjectId);
			}

			waitforProjectile = false;
			collisionTarget = targetAgent;
		}
	}
}