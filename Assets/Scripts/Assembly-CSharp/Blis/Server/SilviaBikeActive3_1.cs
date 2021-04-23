using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SilviaBikeActive3_1)]
	public class SilviaBikeActive3_1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
			LockSkillSlot(SkillSlotSet.Active4_2);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			UnlockSkillSlot(SkillSlotSet.Active4_2);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 targetPosition = Target.Position;
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			Vector3 vector;
			bool flag;
			float num;
			Caster.MoveToDestinationAtSpeed(targetPosition, Singleton<SilviaSkillBikeData>.inst.A3Active3MoveSpeed,
				EasingFunction.Ease.EaseInSine, true, out vector, out flag, out num);
			float arriveTime = num + MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			while (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime < arriveTime && Caster.IsMoving())
			{
				yield return WaitForFrame();
			}

			if (Vector3.Distance(Target.Position, Caster.Position) <=
			    Singleton<SilviaSkillBikeData>.inst.A3CollisionRadius)
			{
				int skillLevel = SkillLevel;
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<SilviaSkillBikeData>.inst.A3BaseDamage[skillLevel]);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<SilviaSkillBikeData>.inst.A3ApDamage);
				parameterCollection.Add(SkillScriptParameterType.DamageCharacterSpeedCoef,
					Singleton<SilviaSkillBikeData>.inst.A3SpeedCoefDamage[skillLevel]);
				DamageTo(Target, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
					Singleton<SilviaSkillBikeData>.inst.A3CollisionEffectCode);
				KnockbackState knockbackState = CreateState<KnockbackState>(Target, 2000010);
				knockbackState.Init(Caster.Forward, Singleton<SilviaSkillBikeData>.inst.A3KnockBackDistance,
					Singleton<SilviaSkillBikeData>.inst.A3knockBackDuration, EasingFunction.Ease.EaseOutQuad, false);
				Target.AddState(knockbackState, Caster.ObjectId);
				AddState(Caster, Singleton<SilviaSkillBikeData>.inst.A3AfterStateCode);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}