using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.YukiActive3)]
	public class YukiActive3 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private Vector3 casterFinalDestination;

		
		private CollisionCircle3D collision;

		
		private Vector3 direction;

		
		private bool hitAction;

		
		protected override void Start()
		{
			base.Start();
			hitAction = false;
			if (collision == null)
			{
				collision = new CollisionCircle3D(Vector3.zero, SkillWidth * 0.5f);
			}
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

			CasterLockRotation(true);
			collision.UpdateRadius(SkillWidth * 0.5f);
			float moveStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			bool flag;
			float finalDuration;
			Caster.MoveToDirectionForTime(Caster.Forward, Singleton<YukiSkillActive3Data>.inst.Distance,
				Singleton<YukiSkillActive3Data>.inst.Duration, EasingFunction.Ease.Linear, false,
				out casterFinalDestination, out flag, out finalDuration);
			hitAction = HitTarget();
			while (Caster.IsMoving() && !hitAction &&
			       MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - moveStartTime <= finalDuration)
			{
				yield return WaitForFrame();
				hitAction = HitTarget();
			}

			if (hitAction)
			{
				Caster.MoveToDirectionForTime(Caster.Forward, Singleton<YukiSkillActive3Data>.inst.AfterHitDistance,
					Singleton<YukiSkillActive3Data>.inst.AfterHitDuration, EasingFunction.Ease.Linear, false,
					out casterFinalDestination, out flag, out finalDuration);
				yield return WaitForSeconds(finalDuration);
			}

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

		
		private bool HitTarget()
		{
			collision.UpdatePosition(Caster.Position + Caster.Forward * Caster.Stat.Radius);
			bool result = false;
			using (List<SkillAgent>.Enumerator enumerator = GetEnemyCharacters(collision).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					SkillAgent target = enumerator.Current;
					int effectAndSoundCode = Caster.Status.ExtraPoint > 0
						? Singleton<YukiSkillActive3Data>.inst.PassiveEffectAndSound
						: Singleton<YukiSkillActive3Data>.inst.EffectAndSound;
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<YukiSkillActive3Data>.inst.SkillDamage[SkillLevel]);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<YukiSkillActive3Data>.inst.SkillApCoef);
					DamageTo(target, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
						effectAndSoundCode);
					PlaySkillAction(Caster, 1);
					AddState(target, Singleton<YukiSkillActive3Data>.inst.DebuffState);
					result = true;
				}
			}

			return result;
		}
	}
}