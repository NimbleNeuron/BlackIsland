using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.BoarCharge)]
	public class BoarCharge : SkillScript
	{
		
		private readonly CollisionBox3D collision = new CollisionBox3D(Vector3.zero, 0f, 0f, Vector3.zero);

		
		private readonly List<int> hitEnemies = new List<int>();

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private Vector3 casterFinalDestination;

		
		private Vector3 chargeDirection;

		
		protected override void Start()
		{
			base.Start();
			hitEnemies.Clear();
			chargeDirection = GameUtil.DirectionOnPlane(Caster.Position, Target.Position);
			LookAtDirection(Caster, chargeDirection);
			CasterLockRotation(true);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			yield return WaitForSeconds(SkillConcentrationTime);
			FinishConcentration(false);
			PlaySkillAction(Caster, 1);
			float chargeDistance = Singleton<BoarSkillChargeData>.inst.ChargeDistance;
			float chargeDuration = Singleton<BoarSkillChargeData>.inst.ChargeDuration;
			float moveStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			bool flag;
			float finalDuration;
			Caster.MoveToDirectionForTime(chargeDirection, chargeDistance, chargeDuration, EasingFunction.Ease.Linear,
				false, out casterFinalDestination, out flag, out finalDuration);
			while (Caster.IsMoving() &&
			       MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - moveStartTime <= finalDuration)
			{
				yield return WaitForFrame();
				CheckCollision(chargeDistance, chargeDuration);
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

		
		private bool CheckCollision(float chargeDistance, float chargeDuration)
		{
			bool result = false;
			collision.UpdatePosition(Caster.Position);
			collision.UpdateWidth(SkillWidth);
			collision.UpdateDepth(3E-45f);
			collision.UpdateNormalized(chargeDirection);
			using (List<SkillAgent>.Enumerator enumerator = GetEnemyCharacters(collision).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillAgent enemy = enumerator.Current;
					if (!hitEnemies.Any(x => x == enemy.ObjectId))
					{
						parameterCollection.Clear();
						parameterCollection.Add(SkillScriptParameterType.Damage,
							Singleton<BoarSkillChargeData>.inst.SkillDamage);
						parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
							Singleton<BoarSkillChargeData>.inst.SkillApCoef);
						DamageTo(enemy, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection, 2000009);
						float magnitude = (casterFinalDestination - Caster.Position).magnitude;
						float duration = chargeDuration * (magnitude / chargeDistance);
						KnockbackState knockbackState = CreateState<KnockbackState>(enemy, 2000010);
						knockbackState.Init(chargeDirection, magnitude, duration, EasingFunction.Ease.EaseOutQuad,
							false);
						knockbackState.SetActionOnCollisionWall(delegate(SkillAgent self)
						{
							AddState(self, 2000009, Singleton<BoarSkillChargeData>.inst.StunDuration);
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

			return result;
		}
	}
}