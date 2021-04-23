using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyunwooActive3)]
	public class HyunwooActive3 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damageHit = SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection damageStun = SkillScriptParameterCollection.Create();

		
		private readonly List<int> hitEnemies = new List<int>();

		
		private float additionalDashStartTime;

		
		private Vector3 casterFinalDestination;

		
		private CollisionBox3D collision;

		
		private bool hitAction;

		
		protected override void Start()
		{
			base.Start();
			hitAction = false;
			hitEnemies.Clear();
			if (collision == null)
			{
				collision = new CollisionBox3D(Vector3.zero, SkillWidth,
					Singleton<HyunwooSkillActive3Data>.inst.CollisionBoxDepth, Vector3.zero);
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
			Vector3 direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			collision.UpdateNormalized(direction);
			collision.UpdateWidth(SkillWidth);
			collision.UpdateDepth(Singleton<HyunwooSkillActive3Data>.inst.CollisionBoxDepth);
			float moveStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			bool canMoveToDestination;
			float finalDuration;
			Caster.MoveToDirectionForTime(direction, Singleton<HyunwooSkillActive3Data>.inst.DashDistance,
				Singleton<HyunwooSkillActive3Data>.inst.DashDuration, EasingFunction.Ease.Linear, false,
				out casterFinalDestination, out canMoveToDestination, out finalDuration);
			hitAction = CheckCollision(Singleton<HyunwooSkillActive3Data>.inst.HitDashDistance,
				Singleton<HyunwooSkillActive3Data>.inst.HitDashDuration, direction);
			while (Caster.IsMoving() && !hitAction &&
			       MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - moveStartTime <= finalDuration)
			{
				yield return WaitForFrame();
				hitAction = CheckCollision(Singleton<HyunwooSkillActive3Data>.inst.HitDashDistance,
					Singleton<HyunwooSkillActive3Data>.inst.HitDashDuration, direction);
			}

			if (hitAction)
			{
				PlaySkillAction(Caster, 1);
				moveStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
				Caster.MoveToDirectionForTime(direction, Singleton<HyunwooSkillActive3Data>.inst.HitDashDistance,
					Singleton<HyunwooSkillActive3Data>.inst.HitDashDuration, EasingFunction.Ease.EaseOutQuad, false,
					out casterFinalDestination, out canMoveToDestination, out finalDuration);
				float magnitude = (casterFinalDestination - Caster.Position).magnitude;
				float dashDuration =
					finalDuration * (magnitude / Singleton<HyunwooSkillActive3Data>.inst.HitDashDistance);
				CheckCollision(magnitude, dashDuration, direction);
				while (Caster.IsMoving() &&
				       MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - moveStartTime <= finalDuration)
				{
					yield return WaitForFrame();
					magnitude = (casterFinalDestination - Caster.Position).magnitude;
					dashDuration = finalDuration *
					               (magnitude / Singleton<HyunwooSkillActive3Data>.inst.HitDashDistance);
					CheckCollision(magnitude, dashDuration, direction);
				}

				PlaySkillAction(Caster, 2);
				if (!canMoveToDestination)
				{
					Caster.MoveToDirectionForTime(-direction, 1.2f, 0.2f, EasingFunction.Ease.Linear, false,
						out casterFinalDestination, out canMoveToDestination, out finalDuration);
					yield return WaitForSeconds(finalDuration);
					yield return WaitForFrame();
				}
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		private bool CheckCollision(float dashDistance, float dashDuration, Vector3 direction)
		{
			bool result = false;
			collision.UpdatePosition(Caster.Position +
			                         direction * (Singleton<HyunwooSkillActive3Data>.inst.CollisionBoxDepth * 0.5f));
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
			if (enemyCharacters.Count > 0)
			{
				using (List<SkillAgent>.Enumerator enumerator = enemyCharacters.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SkillAgent enemy = enumerator.Current;
						if (!hitEnemies.Any(x => x == enemy.ObjectId))
						{
							int num = (int) (enemy.Status.Hp *
							                 Singleton<HyunwooSkillActive3Data>.inst.RateByLevel[SkillLevel]);
							if (num < 1)
							{
								num = 1;
							}

							num += (int) (Caster.Stat.Defense * Singleton<HyunwooSkillActive3Data>.inst.SkillDefCoef);
							damageHit.Clear();
							damageHit.Add(SkillScriptParameterType.Damage, num);
							DamageTo(enemy, DamageType.Skill, DamageSubType.Normal, 0, damageHit,
								Singleton<HyunwooSkillActive3Data>.inst.EffectAndSoundCode);
							AddState(enemy, Singleton<HyunwooSkillActive3Data>.inst.DebuffState[SkillLevel]);
							KnockbackState knockbackState = CreateState<KnockbackState>(enemy, 2000010);
							knockbackState.Init(direction, dashDistance, dashDuration, EasingFunction.Ease.EaseOutQuad,
								false);
							knockbackState.SetActionOnCollisionWall(delegate(SkillAgent self)
							{
								AddState(self, 2000009, Singleton<HyunwooSkillActive3Data>.inst.StunDuration);
								damageStun.Clear();
								damageStun.Add(SkillScriptParameterType.Damage,
									Singleton<HyunwooSkillActive3Data>.inst.DamageByLevel[SkillLevel]);
								DamageTo(self, DamageType.Skill, DamageSubType.Normal, 0, damageStun,
									Singleton<HyunwooSkillActive3Data>.inst.EffectAndSoundCode);
							});
							enemy.AddState(knockbackState, Caster.ObjectId);
							hitEnemies.Add(enemy.ObjectId);
						}
					}
				}

				if (0 < hitEnemies.Count)
				{
					result = true;
				}
			}

			return result;
		}
	}
}