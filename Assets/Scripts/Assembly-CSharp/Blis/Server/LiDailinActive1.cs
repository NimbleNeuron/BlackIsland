using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LiDailinActive1)]
	public class LiDailinActive1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameters = SkillScriptParameterCollection.Create();

		
		private readonly WaitForFrameUpdate waitFrameForCollsionStopCheck = new WaitForFrameUpdate();

		
		private CollisionCircle3D collisionHit;

		
		private CollisionCircle3D collisionStop;

		
		private int curSequece;

		
		private bool isReinforce;

		
		protected override void Start()
		{
			base.Start();
			isReinforce = false;
			curSequece = 0;
			if (collisionHit == null)
			{
				collisionHit = new CollisionCircle3D(Vector3.zero, 0f);
				collisionStop = new CollisionCircle3D(Vector3.zero, 0f);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (isReinforce)
			{
				AddState(Caster, Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackStateCode);
				if (curSequece <= 1)
				{
					AddState(Caster, Singleton<LiDailinSkillData>.inst.A1ReinforceStateCode);
				}
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			curSequece = GetSkillSequence(SkillSlotSet.Active1_1);
			if (curSequece == 0)
			{
				isReinforce = Caster.Status.ExtraPoint >= Singleton<LiDailinSkillData>.inst.SkillReinforceExtraPoint;
				if (isReinforce)
				{
					Caster.ExtraPointModifyTo(Caster, Singleton<LiDailinSkillData>.inst.A1ReinforceConsumeExtraPoint);
				}
			}
			else
			{
				isReinforce = Caster.AnyHaveStateByGroup(GameDB.characterState
					.GetData(Singleton<LiDailinSkillData>.inst.A1ReinforceStateCode).group);
			}

			float dashDistance = isReinforce
				? Singleton<LiDailinSkillData>.inst.A1ReinforceDashDistance
				: Singleton<LiDailinSkillData>.inst.A1DashDistance;
			bool passingWall;
			if (curSequece == 0)
			{
				PlaySkillAction(Caster, isReinforce ? 2 : 1);
				passingWall = false;
			}
			else if (curSequece == 1)
			{
				PlaySkillAction(Caster, isReinforce ? 12 : 11);
				passingWall = false;
			}
			else
			{
				PlaySkillAction(Caster, isReinforce ? 22 : 21);
				passingWall = true;
			}

			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
				LookAtPosition(Caster, info.cursorPosition);
			}

			Vector3 direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			Vector3 vector;
			bool flag;
			float num;
			Caster.MoveToDirectionForTime(direction, dashDistance, Singleton<LiDailinSkillData>.inst.A1DashDuration,
				EasingFunction.Ease.Linear, passingWall, out vector, out flag, out num);
			StartCoroutine(CheckCharacterCollisionStop());
			int effectCode = isReinforce
				? Singleton<LiDailinSkillData>.inst.A1EffectCodeReinforce
				: Singleton<LiDailinSkillData>.inst.A1EffectCode;
			int baseDamage = isReinforce
				? Singleton<LiDailinSkillData>.inst.A1ReinforceBaseDamage[SkillLevel]
				: Singleton<LiDailinSkillData>.inst.A1BaseDamage[SkillLevel];
			float apCoef = isReinforce
				? Singleton<LiDailinSkillData>.inst.A1ReinforceApDamage
				: Singleton<LiDailinSkillData>.inst.A1ApDamage;
			collisionHit.UpdatePosition(Caster.Position);
			collisionHit.UpdateRadius(SkillWidth * 0.5f);
			if (curSequece == 2)
			{
				yield return WaitForSeconds(Singleton<LiDailinSkillData>.inst.A1ThirdAttackTime);
				collisionHit.UpdatePosition(Caster.Position);
				using (List<SkillAgent>.Enumerator enumerator = GetEnemyCharacters(collisionHit).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SkillAgent target = enumerator.Current;
						parameters.Clear();
						parameters.Add(SkillScriptParameterType.Damage, baseDamage);
						parameters.Add(SkillScriptParameterType.DamageApCoef, apCoef);
						DamageTo(target, DamageType.Skill, DamageSubType.Normal, 0, parameters, effectCode);
					}

					goto IL_511;
				}
			}

			float attackStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime +
			                        Singleton<LiDailinSkillData>.inst.A1AttackStartTime;
			float attackEndTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime +
			                      Singleton<LiDailinSkillData>.inst.A1AttackEndTime;
			HashSet<int> strickenObjectIds = new HashSet<int>();
			while (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime <= attackEndTime)
			{
				yield return WaitForFrame();
				if (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime >= attackStartTime)
				{
					collisionHit.UpdatePosition(Caster.Position);
					foreach (SkillAgent skillAgent in GetEnemyCharacters(collisionHit))
					{
						if (!strickenObjectIds.Contains(skillAgent.ObjectId))
						{
							parameters.Clear();
							parameters.Add(SkillScriptParameterType.Damage, baseDamage);
							parameters.Add(SkillScriptParameterType.DamageApCoef, apCoef);
							DamageTo(skillAgent, DamageType.Skill, DamageSubType.Normal, 0, parameters, effectCode);
							strickenObjectIds.Add(skillAgent.ObjectId);
						}
					}
				}
			}

			strickenObjectIds = null;
			IL_511:
			yield return WaitForSeconds(Singleton<LiDailinSkillData>.inst.A1AfterDelayTime[curSequece]);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		private IEnumerator CheckCharacterCollisionStop()
		{
			float radius = Caster.Stat.Radius;
			collisionStop.UpdatePosition(Caster.Position);
			collisionStop.UpdateRadius(radius * Singleton<LiDailinSkillData>.inst.A1StopRadiusModifier);
			for (;;)
			{
				NavMeshHit navMeshHit;
				if (!NavMesh.SamplePosition(Caster.Position, out navMeshHit, 0.1f, 2147483640))
				{
					yield return waitFrameForCollsionStopCheck.Frame(1);
				}
				else
				{
					collisionStop.UpdatePosition(Caster.Position);
					if (GetEnemyCharacters(collisionStop).Count > 0)
					{
						break;
					}

					yield return waitFrameForCollsionStopCheck.Frame(1);
				}
			}

			Caster.StopMove();
		}

		
		protected override void GetSkillRange(ref float minRange, ref float maxRange)
		{
			if (Singleton<LiDailinSkillData>.inst.SkillReinforceExtraPoint <= Caster.Status.ExtraPoint)
			{
				minRange = Singleton<LiDailinSkillData>.inst.A1ReinforceDashDistance;
				maxRange = minRange;
				return;
			}

			minRange = Singleton<LiDailinSkillData>.inst.A1DashDistance;
			maxRange = minRange;
		}
	}
}