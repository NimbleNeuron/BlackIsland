using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.SummonServant)]
	public class LocalSummonServant : LocalSummonBase, ILocalMoveAgentOwner
	{
		private const string AnimBoolOwnerDead = "bOwnerDead";


		private const string AnimBoolOwnerDyingCondition = "bOnwerDyingCondition";


		private const string AnimTriggerOwnerRevival = "tOwnerRevival";


		private static readonly int BoolOwnerDead = Animator.StringToHash("bOwnerDead");


		private static readonly int BoolOwnerDyingCondition = Animator.StringToHash("bOnwerDyingCondition");


		private static readonly int TriggerOwnerRevival = Animator.StringToHash("tOwnerRevival");


		private new readonly List<Renderer> indicatorRenderers = new List<Renderer>();


		protected LocalMoveAgent moveAgent;


		private LocalSummonServantData servantData;


		protected override bool IsInBush => moveAgent.IsInBush;


		public LocalSummonServantData ServantData => servantData;


		public LocalMoveAgent MoveAgent => moveAgent;


		public void MoveStraightWithoutNav(Vector3 moveStartPos, Vector3 moveEndPos, float duration)
		{
			if (moveAgent == null)
			{
				return;
			}

			moveAgent.MoveStraightWithoutNav(moveStartPos, moveEndPos, duration);
		}


		public void MoveToTargetWithoutNav(Vector3 moveStartPos, LocalCharacter target, float moveSpeed,
			float arriveRadius)
		{
			if (moveAgent == null)
			{
				return;
			}

			moveAgent.MoveToTargetWithoutNav(transform.position, target, moveSpeed, arriveRadius);
		}


		public void UpdateMoveSpeed(float moveSpeed)
		{
			throw new NotImplementedException();
		}


		public void MoveInCurve(Vector3 startRotation, float angularSpeed)
		{
			throw new NotImplementedException();
		}


		public void MoveInDirection(Vector3 direction)
		{
			throw new NotImplementedException();
		}


		public void MoveToDestination(BlisVector startPosition, BlisVector destination, BlisVector[] corners)
		{
			throw new NotImplementedException();
		}


		public void MoveStraight(BlisVector startPosition, BlisVector destination, float duration,
			EasingFunction.Ease ease, bool canRotate)
		{
			throw new NotImplementedException();
		}


		public void WarpTo(BlisVector destination)
		{
			if (!isAlive)
			{
				return;
			}

			moveAgent.Warp(destination);
		}


		public void StopMove()
		{
			moveAgent.Stop();
		}


		public void PauseMove()
		{
			moveAgent.Pause();
		}


		public void ResumeMove()
		{
			moveAgent.Resume();
		}


		public void LockRotation(bool isLock, Quaternion rotation)
		{
			if (moveAgent == null)
			{
				return;
			}

			moveAgent.LockRotation(isLock);
			if (isLock)
			{
				moveAgent.InstanceLookAt(rotation);
			}
		}


		public override void LookAt(Quaternion lookFrom, Quaternion lookTo, float angularSpeed)
		{
			if (moveAgent == null)
			{
				return;
			}

			moveAgent.LookAt(lookFrom, lookTo, angularSpeed);
		}

		protected override ObjectType GetObjectType()
		{
			return ObjectType.SummonServant;
		}


		public override void Init(byte[] snapshotData)
		{
			base.Init(snapshotData);
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnDeadAction += OnDeadEvent;
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnDyingConditionAction += OnDyingConditionEvent;
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnRevivalAction += OnRevivalEvent;
		}


		protected override void InitMoveAgent(MoveAgentSnapshot snapshot)
		{
			base.InitMoveAgent(snapshot);
			GameUtil.BindOrAdd<LocalMoveAgent>(gameObject, ref moveAgent);
			moveAgent.Init(0f, 2147483640);
			moveAgent.ApplySnapshot(snapshot, MonoBehaviourInstance<ClientService>.inst.World);
		}


		public override void OnDead(LocalCharacter attacker)
		{
			base.OnDead(attacker);
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnDeadAction -= OnDeadEvent;
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnDyingConditionAction -= OnDyingConditionEvent;
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnRevivalAction -= OnRevivalEvent;
		}


		protected override void UpdateInternal()
		{
			base.UpdateInternal();
			LocalMoveAgent localMoveAgent = moveAgent;
			if (localMoveAgent != null)
			{
				localMoveAgent.FrameUpdate(Time.deltaTime);
			}

			UpdateCharacterAnimator();
		}


		protected virtual void UpdateCharacterAnimator()
		{
			if (!CanMoveAnimation())
			{
				SetCharacterAnimatorFloat(LocalMovableCharacter.FloatMoveVelocity, 0f);
				SetCharacterAnimatorFloat(LocalMovableCharacter.FloatMoveSpeed, 0f);
				return;
			}

			SetCharacterAnimatorFloat(LocalMovableCharacter.FloatMoveVelocity,
				IsAlive && IsRun() ? Status.MoveSpeed : 0f);
			SetCharacterAnimatorFloat(LocalMovableCharacter.FloatMoveSpeed, Status.MoveSpeed / 3.5f);
		}


		private bool IsRun()
		{
			return !(moveAgent == null) && moveAgent.IsMoving;
		}


		private bool CanMoveAnimation()
		{
			using (List<CharacterStateValue>.Enumerator enumerator = States.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.StateType.ApplyMoveAnimation())
					{
						return false;
					}
				}
			}

			return true;
		}


		public void SetServantData(LocalSummonServantData servantData)
		{
			this.servantData = servantData;
		}


		private void OnDeadEvent(LocalCharacter target)
		{
			if (Owner == null)
			{
				SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnDeadAction -= OnDeadEvent;
				return;
			}

			if (target.ObjectId != Owner.ObjectId)
			{
				return;
			}

			SetCharacterAnimatorBool(BoolOwnerDead, true);
			SetCharacterAnimatorBool(BoolOwnerDyingCondition, false);
		}


		private void OnDyingConditionEvent(LocalCharacter target)
		{
			if (Owner == null)
			{
				SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnDyingConditionAction -= OnDyingConditionEvent;
				return;
			}

			if (target.ObjectId != Owner.ObjectId)
			{
				return;
			}

			SetCharacterAnimatorBool(BoolOwnerDyingCondition, true);
		}


		private void OnRevivalEvent(LocalCharacter target)
		{
			if (Owner == null)
			{
				SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnRevivalAction -= OnRevivalEvent;
				return;
			}

			if (target.ObjectId != Owner.ObjectId)
			{
				return;
			}

			SetCharacterAnimatorBool(BoolOwnerDead, false);
			SetCharacterAnimatorBool(BoolOwnerDyingCondition, false);
			SetCharacterAnimatorTrigger(TriggerOwnerRevival);
		}


		public override int GetCurrentAreaMask()
		{
			return AreaUtil.GetCurrentAreaMask(GetPosition());
		}


		public override AreaData GetCurrentAreaData(LevelData currentLevel)
		{
			if (moveAgent == null)
			{
				return AreaUtil.GetCurrentAreaDataByMask(currentLevel, 2147483640, GetCurrentAreaMask());
			}

			return AreaUtil.GetCurrentAreaDataByMask(currentLevel, moveAgent.WalkableNavMask, GetCurrentAreaMask());
		}


		protected override void ShowIndicator()
		{
			if (SummonData.attackPower <= 0f)
			{
				return;
			}

			if (splatManager == null)
			{
				return;
			}

			if (splatManager.CurrentIndicator == null)
			{
				return;
			}

			splatManager.CurrentIndicator.gameObject.SetActive(true);
		}


		protected override void HideIndicator()
		{
			if (SummonData.attackPower <= 0f)
			{
				return;
			}

			if (splatManager == null)
			{
				return;
			}

			if (splatManager.CurrentIndicator == null)
			{
				return;
			}

			splatManager.CurrentIndicator.gameObject.SetActive(false);
		}


		public override void OnSight() { }


		public void ShowIndicatorRenderers()
		{
			if (indicatorRenderers != null)
			{
				foreach (Renderer renderer in indicatorRenderers)
				{
					renderer.enabled = true;
				}
			}
		}


		public override void OnHide() { }


		public void HideIndicatorRenderers()
		{
			if (indicatorRenderers != null)
			{
				foreach (Renderer renderer in indicatorRenderers)
				{
					renderer.enabled = false;
				}
			}
		}


		public bool HasIndicator()
		{
			return splatManager != null;
		}


		public void AddIndicator(SkillData skillData)
		{
			GameUtil.BindOrAdd<SplatManager>(gameObject, ref splatManager);
			splatManager.CreateIndicator(skillData.Guideline);
			Splat indicator = splatManager.GetIndicator(skillData.Guideline);
			indicator.GetComponentsInChildren<Renderer>(indicatorRenderers);
			if (splatManager.CurrentIndicator == null)
			{
				indicator.Range = skillData.range;
				indicator.Length = skillData.length;
				indicator.Width = skillData.width;
				indicator.Angle = skillData.angle;
				splatManager.SetIndicator(indicator);
			}

			splatManager.CurrentIndicator.Range =
				Math.Abs(SummonData.attackRange) < 0.01f ? 0f : SummonData.attackRange + Stat.Radius;
		}


		public override ObjectOrder GetObjectOrder()
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
			{
				return ObjectOrder.SummonObjectEnemy_Servant;
			}

			if (OwnerId.Equals(MonoBehaviourInstance<ClientService>.inst.MyObjectId))
			{
				return ObjectOrder.SummonObjectMy_Servant;
			}

			return ObjectOrder.SummonObjectAlly_Servant;
		}


		public override void OutSight()
		{
			base.OutSight();
			moveAgent.Stop();
		}


		public override void OnVisible()
		{
			base.OnVisible();
			ShowIndicatorRenderers();
		}


		public class LocalSummonServantData { }
	}
}