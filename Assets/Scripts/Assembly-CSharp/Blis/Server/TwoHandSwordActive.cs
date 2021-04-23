using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.TwoHandSwordActive)]
	public class TwoHandSwordActive : SkillScript
	{
		
		private readonly HashSet<int> alreadyAttackedId = new HashSet<int>();

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private readonly List<SkillAgent> targetAgents = new List<SkillAgent>();

		
		private CollisionBox3D collision;

		
		protected override void Start()
		{
			base.Start();
			if (collision == null)
			{
				collision = new CollisionBox3D(Vector3.zero, Singleton<TwoHandSwordSkillActiveData>.inst.DamageBoxWidth,
					Singleton<TwoHandSwordSkillActiveData>.inst.DamageBoxDepth, Vector3.zero);
			}

			alreadyAttackedId.Clear();
			targetAgents.Clear();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 direction = GameUtil.Direction(Caster.Position, info.cursorPosition);
			LookAtDirection(Caster, direction);
			CasterLockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			float stateStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			BlockingState blockingState = CreateState<BlockingState>(Caster,
				Singleton<TwoHandSwordSkillActiveData>.inst.BuffState[SkillLevel]);
			blockingState.Init(Singleton<TwoHandSwordSkillActiveData>.inst.BlockingAngle,
				Singleton<TwoHandSwordSkillActiveData>.inst.BlockCount,
				Singleton<TwoHandSwordSkillActiveData>.inst.blockDamageSubType);
			AddState(Caster, blockingState);
			while (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - stateStartTime <=
			       Singleton<TwoHandSwordSkillActiveData>.inst.BlockingDurtaion)
			{
				yield return WaitForFrame();
			}

			PlaySkillAction(Caster, 1);
			float moveStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			Vector3 vector;
			bool flag;
			float finalDuration;
			Caster.MoveToDirectionForTime(direction, Singleton<TwoHandSwordSkillActiveData>.inst.DashDistance,
				Singleton<TwoHandSwordSkillActiveData>.inst.DashDuration, EasingFunction.Ease.Linear, false, out vector,
				out flag, out finalDuration);
			collision.UpdateNormalized(direction);
			while (Caster.IsMoving() &&
			       MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - moveStartTime <= finalDuration)
			{
				yield return WaitForFrame();
				collision.UpdatePosition(Caster.Position +
				                         Singleton<TwoHandSwordSkillActiveData>.inst.DamageBoxDepth * 0.5f * direction);
				targetAgents.Clear();
				foreach (SkillAgent skillAgent in GetEnemyCharacters(collision))
				{
					if (!alreadyAttackedId.Contains(skillAgent.ObjectId))
					{
						alreadyAttackedId.Add(skillAgent.ObjectId);
						targetAgents.Add(skillAgent);
					}
				}

				if (targetAgents.Count > 0)
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<TwoHandSwordSkillActiveData>.inst.DamageApCoef[SkillLevel]);
					DamageTo(targetAgents, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
						Singleton<TwoHandSwordSkillActiveData>.inst.DamageEffectAndSoundCode);
				}
			}

			yield return WaitForSeconds(Singleton<TwoHandSwordSkillActiveData>.inst.DashDuration);
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