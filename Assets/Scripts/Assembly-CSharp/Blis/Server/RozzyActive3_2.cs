using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.RozziActive3_2)]
	public class RozzyActive3_2 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damageParam = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
			LookAtTarget(Caster, Target);
			Caster.LockRotation(true);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			RozzyActive3_2 rozzyActive32 = this;
			rozzyActive32.Start();
			if (rozzyActive32.SkillCastingTime1 > 0.0)
			{
				yield return rozzyActive32.FirstCastingTime();
			}

			ProjectileProperty projectile = rozzyActive32.PopProjectileProperty(rozzyActive32.Caster,
				Singleton<RozziSkillActive3Data>.inst.Active3ProjectileCode);
			projectile.SetActionOnCollisionCharacter(
				(targetAgent, attackerInfo, damagePoint,
					damageDirection) =>
				{
					Caster.RemoveStateByGroup(
						GameDB.characterState.GetData(Singleton<RozziSkillActive3Data>.inst.Active3_2EnableStateCode)
							.group, Caster.ObjectId);
					damageParam.Clear();
					damageParam.Add(SkillScriptParameterType.Damage,
						Singleton<RozziSkillActive3Data>.inst.DamageActive3_2ByLevel[SkillLevel]);
					damageParam.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<RozziSkillActive3Data>.inst.DamageActive3_2ApCoef);
					DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
						projectile.ProjectileData.damageSubType, 0, damageParam,
						projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
					KnockbackState state = CreateState<KnockbackState>(targetAgent, 2000010);
					Vector3 direction = GameUtil.DirectionOnPlane(Caster.Position, targetAgent.Position);
					state.Init(direction, Singleton<RozziSkillActive3Data>.inst.Active3KnockBackDistance,
						Singleton<RozziSkillActive3Data>.inst.Active3KnockBackSpeed, EasingFunction.Ease.EaseOutQuad,
						false);
					state.SetActionOnCollisionWall(self => AddState(self, 2000009,
						Singleton<RozziSkillActive3Data>.inst.Active3KnockBackStunDuration));
					targetAgent.AddState(state, Caster.ObjectId);
				});
			projectile.SetTargetObject(rozzyActive32.Target.ObjectId);
			rozzyActive32.LaunchProjectile(projectile);
			if (rozzyActive32.SkillFinishDelayTime > 0.0)
			{
				yield return rozzyActive32.FinishDelayTime();
			}

			rozzyActive32.Finish();

			// co: dotPeek
			// this.Start();
			// if (base.SkillCastingTime1 > 0f)
			// {
			// 	yield return base.FirstCastingTime();
			// }
			// ProjectileProperty projectile = base.PopProjectileProperty(base.Caster, Singleton<RozziSkillActive3Data>.inst.Active3ProjectileCode);
			// Action<SkillAgent> <>9__1;
			// projectile.SetActionOnCollisionCharacter(delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint, Vector3 damageDirection)
			// {
			// 	this.Caster.RemoveStateByGroup(GameDB.characterState.GetData(Singleton<RozziSkillActive3Data>.inst.Active3_2EnableStateCode).group, this.Caster.ObjectId);
			// 	this.damageParam.Clear();
			// 	this.damageParam.Add(SkillScriptParameterType.Damage, (float)Singleton<RozziSkillActive3Data>.inst.DamageActive3_2ByLevel[this.SkillLevel]);
			// 	this.damageParam.Add(SkillScriptParameterType.DamageApCoef, Singleton<RozziSkillActive3Data>.inst.DamageActive3_2ApCoef);
			// 	this.DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType, projectile.ProjectileData.damageSubType, 0, this.damageParam, projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0, true, 0, 1f, true);
			// 	KnockbackState knockbackState = this.CreateState<KnockbackState>(targetAgent, 2000010, 0, null);
			// 	Vector3 direction = GameUtil.DirectionOnPlane(this.Caster.Position, targetAgent.Position);
			// 	knockbackState.Init(direction, Singleton<RozziSkillActive3Data>.inst.Active3KnockBackDistance, Singleton<RozziSkillActive3Data>.inst.Active3KnockBackSpeed, EasingFunction.Ease.EaseOutQuad, false);
			// 	KnockbackState knockbackState2 = knockbackState;
			// 	Action<SkillAgent> actionOnCollisionWall;
			// 	if ((actionOnCollisionWall = <>9__1) == null)
			// 	{
			// 		actionOnCollisionWall = (<>9__1 = delegate(SkillAgent self)
			// 		{
			// 			this.AddState(self, 2000009, new float?(Singleton<RozziSkillActive3Data>.inst.Active3KnockBackStunDuration));
			// 		});
			// 	}
			// 	knockbackState2.SetActionOnCollisionWall(actionOnCollisionWall);
			// 	targetAgent.AddState(knockbackState, this.Caster.ObjectId);
			// });
			// projectile.SetTargetObject(base.Target.ObjectId);
			// base.LaunchProjectile(projectile);
			// if (base.SkillFinishDelayTime > 0f)
			// {
			// 	yield return base.FinishDelayTime();
			// }
			// this.Finish(false);
			// yield break;
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Caster.LockRotation(false);
		}
	}
}