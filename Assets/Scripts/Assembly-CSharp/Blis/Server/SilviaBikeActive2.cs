using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SilviaBikeActive2)]
	public class SilviaBikeActive2 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damage = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D collision;

		
		protected override void Start()
		{
			base.Start();
			LockSkillSlot(SkillSlotSet.Active4_2);
			if (collision == null)
			{
				collision = new CollisionCircle3D(Vector3.zero, SkillInnerRange);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			UnlockSkillSlot(SkillSlotSet.Active4_2);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 skillPoint = GetSkillPoint();
			Vector3 direction = GameUtil.Direction(Caster.Position, skillPoint);
			LookAtDirection(Caster, direction);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			Vector3 vector;
			bool flag;
			float num;
			Caster.MoveToDestinationForTime(skillPoint, Singleton<SilviaSkillBikeData>.inst.A2SkillDuration,
				EasingFunction.Ease.Linear, true, out vector, out flag, out num, true);
			if (Singleton<SilviaSkillBikeData>.inst.A2SkillDuration > 0f)
			{
				yield return WaitForSeconds(Singleton<SilviaSkillBikeData>.inst.A2SkillDuration);
			}

			collision.UpdatePosition(Caster.Position +
			                         Caster.Forward * Singleton<SilviaSkillBikeData>.inst.A2SkillBikeWheelDistance);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
			PlaySkillAction(Caster, 1);
			int skillLevel = SkillLevel;
			foreach (SkillAgent target in enemyCharacters)
			{
				AirborneState airborneState = CreateState<AirborneState>(target, 2000001, 0,
					Singleton<SilviaSkillBikeData>.inst.A2SKillAirBoneDuration);
				airborneState.Init(Singleton<SilviaSkillBikeData>.inst.A2SKillAirBoneDuration);
				AddState(target, airborneState);
				damage.Clear();
				damage.Add(SkillScriptParameterType.Damage,
					Singleton<SilviaSkillBikeData>.inst.A2BaseDamage[skillLevel]);
				damage.Add(SkillScriptParameterType.DamageApCoef, Singleton<SilviaSkillBikeData>.inst.A2ApDamage);
				DamageTo(target, DamageType.Skill, DamageSubType.Normal, 0, damage,
					Singleton<SilviaSkillBikeData>.inst.A2DamageEffectSoundCode);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}