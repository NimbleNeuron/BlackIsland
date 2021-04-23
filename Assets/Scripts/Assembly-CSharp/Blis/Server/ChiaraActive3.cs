using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ChiaraActive3)]
	public class ChiaraActive3 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection collisionDamage = SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection disconnectionTimeOutDamage =
			SkillScriptParameterCollection.Create();

		
		private HookLineProperty hookLineProperty;

		
		protected override void Start()
		{
			base.Start();
			if (hookLineProperty == null)
			{
				hookLineProperty = CreateHookLineProjectile(Caster, Singleton<ChiaraSkillData>.inst.A3ProjectileCode,
					Singleton<ChiaraSkillData>.inst.A3HookLineCode);
				hookLineProperty.SetLinkFromCharacter(Caster.Character);
				hookLineProperty.SetActionOnCollisionCharacter(OnCollisionCharacter);
				hookLineProperty.SetDisconnectionRangeOutAction(OnDisconnectionRangeOut);
				hookLineProperty.SetDisconnectionTimeOutAction(OnDisconnectionTimeOut);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
				LookAtPosition(Caster, info.cursorPosition);
			}

			Vector3 targetDirection = GameUtil.Direction(Caster.Position, GetSkillPoint());
			hookLineProperty.SetTargetDirection(targetDirection);
			LaunchHookLineProjectile(hookLineProperty);
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
				Singleton<ChiaraSkillData>.inst.A3CollisionBaseDamage[SkillLevel]);
			collisionDamage.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<ChiaraSkillData>.inst.A3CollisionApDamage);
			DamageTo(targetAgent, attackerInfo, DamageType.Skill, DamageSubType.Normal, 0, collisionDamage,
				SkillSlotSet.Active2_1, damagePoint, damageDirection,
				Singleton<ChiaraSkillData>.inst.A3CollisionEffectCode);
		}

		
		private void OnDisconnectionRangeOut(WorldCharacter target, WorldHookLineProjectile hookLineProjectile)
		{
			PlaySkillAction(Caster, SkillId.ChiaraActive2, 1, target.ObjectId);
		}

		
		private void OnDisconnectionTimeOut(WorldCharacter target, WorldHookLineProjectile hookLineProjectile)
		{
			SkillAgent skillAgent = target.SkillAgent;
			disconnectionTimeOutDamage.Clear();
			disconnectionTimeOutDamage.Add(SkillScriptParameterType.Damage,
				Singleton<ChiaraSkillData>.inst.A3ConnectionCompleteBaseDamage[SkillLevel]);
			disconnectionTimeOutDamage.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<ChiaraSkillData>.inst.A3ConnectionCompleteApDamage);
			DamageTo(skillAgent, DamageType.Skill, DamageSubType.Normal, 0, disconnectionTimeOutDamage,
				Singleton<ChiaraSkillData>.inst.A3ConnectionCompleteEffectCode);
			AddState(skillAgent, Singleton<ChiaraSkillData>.inst.A3FetterStateCode);
		}
	}
}