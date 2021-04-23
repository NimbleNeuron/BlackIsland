using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.XiukaiActive3)]
	public class XiukaiActive3 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameters = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D collision;

		
		protected override void Start()
		{
			base.Start();
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

			Vector3 direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			PlaySkillAction(Caster, 2);
			float dashStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			Vector3 vector;
			bool flag;
			float finalDashDuration;
			Caster.MoveToDirectionForTime(direction, Singleton<XiukaiSkillActive3Data>.inst.DashDistance,
				Singleton<XiukaiSkillActive3Data>.inst.DashDuration, EasingFunction.Ease.EaseOutQuad, true, out vector,
				out flag, out finalDashDuration);
			collision.UpdateRadius(SkillWidth * 0.5f);
			SkillAgent targetEnemy = null;
			bool isFirstCheck = true;
			while (Caster.IsMoving() && dashStartTime + finalDashDuration >=
				MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime)
			{
				if (isFirstCheck)
				{
					isFirstCheck = false;
				}
				else
				{
					yield return WaitForFrame();
				}

				collision.UpdatePosition(Caster.Position);
				List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
				if (enemyCharacters.Count != 0)
				{
					targetEnemy = enemyCharacters.NearestOne(Caster.Position);
					break;
				}
			}

			if (targetEnemy != null)
			{
				Caster.StopMove();
				bool flag2 = targetEnemy.AnyHaveNegativelyAffectsMovementState();
				parameters.Clear();
				parameters.Add(SkillScriptParameterType.Damage,
					Singleton<XiukaiSkillActive3Data>.inst.BaseDamage[SkillLevel]);
				parameters.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<XiukaiSkillActive3Data>.inst.SkillApCoef);
				int effectAndSoundCode;
				if (flag2)
				{
					parameters.Add(SkillScriptParameterType.DamageCasterMaxHpCoef,
						Singleton<XiukaiSkillActive3Data>.inst.SkillAddDamageMaxHpRatio2[SkillLevel] * 0.01f);
					effectAndSoundCode = 1013402;
				}
				else
				{
					parameters.Add(SkillScriptParameterType.DamageCasterMaxHpCoef,
						Singleton<XiukaiSkillActive3Data>.inst.SkillAddDamageMaxHpRatio[SkillLevel] * 0.01f);
					effectAndSoundCode = 1013401;
				}

				DamageTo(targetEnemy, DamageType.Skill, DamageSubType.Normal, 0, parameters, effectAndSoundCode);
				if (flag2)
				{
					AirborneState airborneState = CreateState<AirborneState>(targetEnemy, 2000001, 0,
						Singleton<XiukaiSkillActive3Data>.inst.AirborneDuration);
					if (airborneState != null)
					{
						airborneState.Init(Singleton<XiukaiSkillActive3Data>.inst.AirborneDuration);
					}

					AddState(targetEnemy, airborneState);
				}

				PlaySkillAction(Caster, 3);
				if (SkillCastingTime2 > 0f)
				{
					yield return SecondCastingTime();
				}
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
	}
}