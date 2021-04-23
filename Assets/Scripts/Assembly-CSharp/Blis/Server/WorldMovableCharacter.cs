using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using Blis.Server.CharacterAction;
using UnityEngine;

namespace Blis.Server
{
	
	public abstract class WorldMovableCharacter : WorldCharacter, IServerMoveAgentOwner
	{
		
		protected override int GetTeamNumber()
		{
			return 0;
		}

		
		
		public ServerCharacterSkill CharacterSkill
		{
			get
			{
				return this.characterSkill;
			}
		}

		
		
		public SkillController SkillController
		{
			get
			{
				return this.skillController;
			}
		}

		
		
		public MovableCharacterController Controller
		{
			get
			{
				return this.controller;
			}
		}

		
		
		public SummonObjectController MySummonController
		{
			get
			{
				return this.mySummonController;
			}
		}

		
		
		public CombatInvolvementAgent CombatInvolvementAgent
		{
			get
			{
				return this.combatInvolvementAgent;
			}
		}

		
		
		public CombatInvolvementAgent.CombatInvolvementResult CombatInvolvementResult
		{
			get
			{
				return this.combatInvolvementResult;
			}
		}

		
		
		public ServerMoveAgent MoveAgent
		{
			get
			{
				return this.moveAgent;
			}
		}

		
		public bool IsStopped()
		{
			return !this.moveAgent.IsMoving;
		}

		
		
		public override bool IsInBush
		{
			get
			{
				return this.moveAgent.IsInBush;
			}
		}

		
		
		public override int WalkableNavMask
		{
			get
			{
				return this.moveAgent.WalkableNavMask;
			}
		}

		
		
		public bool IsGunReloading
		{
			get
			{
				return this.isGunReloading;
			}
		}

		
		
		public override bool IsInCombat
		{
			get
			{
				return this.isInCombat;
			}
		}

		
		protected override void Init(CharacterStat stat)
		{
			GameUtil.BindOrAdd<ServerMoveAgent>(base.gameObject, ref this.moveAgent);
			this.moveAgent.Init(0f, 2147483640);
			ServerMoveAgent serverMoveAgent = this.moveAgent;
			serverMoveAgent.RefreshNextCorners = (ServerMoveAgent.RefreshNextCornersEvent)Delegate.Remove(serverMoveAgent.RefreshNextCorners, new ServerMoveAgent.RefreshNextCornersEvent(this.MoveToDestination));
			ServerMoveAgent serverMoveAgent2 = this.moveAgent;
			serverMoveAgent2.RefreshNextCorners = (ServerMoveAgent.RefreshNextCornersEvent)Delegate.Combine(serverMoveAgent2.RefreshNextCorners, new ServerMoveAgent.RefreshNextCornersEvent(this.MoveToDestination));
			ServerMoveAgent serverMoveAgent3 = this.moveAgent;
			serverMoveAgent3.OnStop = (ServerMoveAgent.OnStopEvent)Delegate.Remove(serverMoveAgent3.OnStop, new ServerMoveAgent.OnStopEvent(this.OnStopMove));
			ServerMoveAgent serverMoveAgent4 = this.moveAgent;
			serverMoveAgent4.OnStop = (ServerMoveAgent.OnStopEvent)Delegate.Combine(serverMoveAgent4.OnStop, new ServerMoveAgent.OnStopEvent(this.OnStopMove));
			ServerMoveAgent serverMoveAgent5 = this.moveAgent;
			serverMoveAgent5.InBush = (ServerMoveAgent.InBushEvent)Delegate.Remove(serverMoveAgent5.InBush, new ServerMoveAgent.InBushEvent(base.InBush));
			ServerMoveAgent serverMoveAgent6 = this.moveAgent;
			serverMoveAgent6.InBush = (ServerMoveAgent.InBushEvent)Delegate.Combine(serverMoveAgent6.InBush, new ServerMoveAgent.InBushEvent(base.InBush));
			ServerMoveAgent serverMoveAgent7 = this.moveAgent;
			serverMoveAgent7.OutBush = (ServerMoveAgent.OutBushEvent)Delegate.Remove(serverMoveAgent7.OutBush, new ServerMoveAgent.OutBushEvent(base.OutBush));
			ServerMoveAgent serverMoveAgent8 = this.moveAgent;
			serverMoveAgent8.OutBush = (ServerMoveAgent.OutBushEvent)Delegate.Combine(serverMoveAgent8.OutBush, new ServerMoveAgent.OutBushEvent(base.OutBush));
			this.mySkillAgent = new WorldMovableCharacterSkillAgent(this);
			GameUtil.BindOrAdd<MovableCharacterController>(base.gameObject, ref this.controller);
			this.controller.Init(this);
			GameUtil.BindOrAdd<SummonObjectController>(base.gameObject, ref this.mySummonController);
			this.mySummonController.Init(this);
			GameUtil.BindOrAdd<SkillController>(base.gameObject, ref this.skillController);
			this.SkillController.Init(this);
			this.SkillController.SetActionOnPlaySkill(new Action<SkillUseInfo>(this.PlaySkill));
			this.SkillController.SetActionOnPlayPassiveSkill(new Action<SkillUseInfo>(this.PlayPassiveSkill));
			this.SkillController.SetActionOnFinishSkill(new Action<SkillSlotSet, MasteryType, SkillId, bool, bool, bool>(this.FinishSkill));
			this.SkillController.SetActionOnFinishStateSkill(new Action<SkillSlotSet, MasteryType, SkillId, bool, bool>(this.FinishStateSkill));
			this.SkillController.SetActionOnFinishPassiveSkill(new Action<SkillSlotSet, SkillId, bool, bool>(this.FinishPassiveSkill));
			this.SkillController.SetActionOnStartSkill(new Action<SkillUseInfo>(this.StartSkill));
			this.SkillController.SetActionOnStartPassiveSkill(new Action<SkillUseInfo>(this.StartPassiveSkill));
			this.SkillController.SetActionOnStartStateSkill(new Action<SkillUseInfo, int>(this.StartStateSkill));
			this.SkillController.SetActionOnConsumeSkillResources(new Action<SkillData>(this.ConsumeSkillResources));
			this.isGunReloading = false;
			this.isInCombat = false;
			this.activatedPassive = false;
			this.preAreaMaskCode = 0;
			base.Init(stat);
			this.combatInvolvementAgent = new CombatInvolvementAgent(MonoBehaviourInstance<GameService>.inst, this);
			this.combatInvolvementResult = new CombatInvolvementAgent.CombatInvolvementResult();
		}

		
		protected void InitCharacterSkill(int characterCode, SpecialSkillId specialSkillId)
		{
			this.characterSkill = new ServerCharacterSkill(characterCode, this.GetObjectType(), specialSkillId);
			this.characterSkill.OnSequenceTimeOver += this.SequenceTimeOver;
			this.characterSkill.OnStackSkillNeedCharge += this.StackSkillNeedCharge;
		}

		
		public virtual void SetInCombat(bool isCombat, WorldCharacter target)
		{
			this.isInCombat = isCombat;
			if (isCombat)
			{
				float num = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime + GameConstants.IN_COMBAT_TIME;
				if (this.inCombatTime < num)
				{
					this.inCombatTime = num;
				}
			}
		}

		
		public void SetInCombatTime(float time)
		{
			this.SetInCombat(true, null);
			this.inCombatTime += time;
		}

		
		protected virtual void UpdateSkillSequenceTimer()
		{
			if (this.CharacterSkill != null && this.CharacterSkill.IsHaveSequenceTimer())
			{
				this.CharacterSkill.UpdateSequenceTimer(MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime, MasteryType.None);
			}
		}

		
		protected virtual void UpdateSkillStackTimer()
		{
			if (this.CharacterSkill != null)
			{
				this.CharacterSkill.UpdateStackSkillTimer(MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
			}
		}

		
		protected WorldPlayerCharacter KillProcess(DamageInfo damageInfo)
		{
			this.combatInvolvementAgent.GetCombatInvolvementResult(ref this.combatInvolvementResult);
			if (this.CombatInvolvementResult.FinishingAttackerObjectType == ObjectType.PlayerCharacter)
			{
				WorldPlayerCharacter worldPlayerCharacter = MonoBehaviourInstance<GameService>.inst.World.Find<WorldPlayerCharacter>(this.CombatInvolvementResult.FinishingAttackerObjectId);
				if (worldPlayerCharacter != null)
				{
					for (int i = 0; i < this.CombatInvolvementResult.Assistants.Count; i++)
					{
						MonoBehaviourInstance<GameService>.inst.World.Find<WorldPlayerCharacter>(this.CombatInvolvementResult.Assistants[i]).OnKillAssist(this, this.combatInvolvementResult.Assistants.Count);
					}
					worldPlayerCharacter.OnKill(damageInfo, this, this.combatInvolvementResult.Assistants);
				}
				return worldPlayerCharacter;
			}
			return null;
		}

		
		protected override void Dead(int finishingAttacker, List<int> assistants, DamageType damageType)
		{
			if (!this.isAlive)
			{
				return;
			}
			base.Dead(finishingAttacker, assistants, damageType);
			this.skillController.CancelAll();
			this.StopDeadMove();
		}

		
		protected void StopDeadMove()
		{
			if (this.moveAgent.CurrentMoveStrategyType == MoveStrategyType.Straight)
			{
				Vector3 vector;
				if (Blis.Common.MoveAgent.CanStandToPosition(base.GetPosition(), this.WalkableNavMask, out vector))
				{
					this.Controller.Stop();
					if (this.moveAgent.IsMoving)
					{
						this.StopMove();
					}
				}
			}
			else
			{
				this.Controller.Stop();
				if (this.moveAgent.IsMoving)
				{
					this.StopMove();
				}
			}
		}

		
		protected override void OnUpdateStat(HashSet<StatType> updateStats)
		{
			base.OnUpdateStat(updateStats);
			if (updateStats.Contains(StatType.MoveSpeed) || updateStats.Contains(StatType.MoveSpeedRatio) || updateStats.Contains(StatType.MoveSpeedOutOfCombat) || updateStats.Contains(StatType.MoveSpeedOutOfCombatRatio))
			{
				this.UpdateMoveSpeed();
			}
		}

		
		protected override void OnChangedStateEffectStat()
		{
			base.OnChangedStateEffectStat();
			this.UpdateMoveSpeed();
		}

		
		protected void UpdateMoveSpeed()
		{
			float num = this.isInCombat ? base.Stat.MoveSpeed : base.Stat.MoveSpeedOutOfCombat;
			float num2 = this.stateEffector.AnyForcedMoveSpeedState() ? this.stateEffector.GetForcedMoveSpeed() : num;
			if (base.Status.MoveSpeed != num2)
			{
				BlisFixedPoint blisFixedPoint = new BlisFixedPoint(num2);
				base.Status.SetMoveSpeed(blisFixedPoint.Value);
				if (this.moveAgent.IsMoving)
				{
					this.moveAgent.UpdateMoveSpeed(blisFixedPoint.Value);
					base.EnqueueCommand(new CmdUpdateMoveSpeedWhenMoving
					{
						moveSpeed = blisFixedPoint
					});
					return;
				}
				base.EnqueueCommand(new CmdUpdateMoveSpeed
				{
					moveSpeed = blisFixedPoint
				});
			}
		}

		
		private Vector3 CalcDelta(Vector3 target)
		{
			return Vector3.Scale(target, this.GroundZero) - Vector3.Scale(base.transform.position, this.GroundZero);
		}

		
		protected override void OnFrameUpdate()
		{
			base.OnFrameUpdate();
			this.moveAgent.FrameUpdate(MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime);
			this.Controller.FrameUpdate();
			if (!this.activatedPassive)
			{
				this.CastPassiveSkill(SkillSlotIndex.Passive);
				this.activatedPassive = true;
			}
			this.UpdateInternal();
		}

		
		protected virtual void UpdateInternal()
		{
			if (this.IsInCombat && this.inCombatTime < MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime)
			{
				this.SetInCombat(false, null);
			}
			this.UpdateSkillSequenceTimer();
			this.UpdateSkillStackTimer();
		}

		
		public virtual void CancelGunReload()
		{
		}

		
		public override bool IsCanNormalAttack()
		{
			return this.IsExistNormalAttack() && base.StateEffector.CanNormalAttack();
		}

		
		public bool IsExistNormalAttack()
		{
			SkillSlotSet? skillSlotSet = this.characterSkill.GetSkillSlotSet(SkillSlotIndex.Attack);
			return skillSlotSet != null && GameDB.skill.GetSkillSetData(this.GetCharacterCode(), base.ObjectType, skillSlotSet.Value) != null;
		}

		
		public void MoveInDirection(Vector3 direction)
		{
			if (!this.isAlive)
			{
				return;
			}
			Vector3 normalized = direction.normalized;
			BlisVector direction2 = new BlisVector(normalized);
			this.moveAgent.MoveInDirection(normalized, base.Status.MoveSpeed);
			base.EnqueueCommand(new CmdMoveByDirection
			{
				position = new BlisVector(base.transform.position),
				direction = direction2
			});
		}

		
		public void MoveInCurve(float angularSpeed)
		{
			if (!this.isAlive)
			{
				return;
			}
			Vector3 forward = base.transform.forward;
			BlisVector startRotation = new BlisVector(forward);
			this.moveAgent.MoveInCurve(forward, angularSpeed, base.Status.MoveSpeed);
			base.EnqueueCommand(new CmdMoveInCurve
			{
				startRotation = startRotation,
				angularSpeed = new BlisFixedPoint(angularSpeed)
			});
		}

		
		public void MoveToTarget(WorldObject obj, float approachDistance = 0f, bool useAttackRange = true)
		{
			if (approachDistance != 0f && GameUtil.DistanceOnPlane(base.GetPosition(), obj.GetPosition()) <= approachDistance)
			{
				return;
			}
			WorldCharacter worldCharacter = obj as WorldCharacter;
			if (worldCharacter != null)
			{
				Vector3 a = GameUtil.DirectionOnPlane(base.GetPosition(), worldCharacter.GetPosition());
				float num = base.Stat.Radius + worldCharacter.Stat.Radius;
				if (useAttackRange)
				{
					float num2 = ((3f < base.Stat.AttackRange) ? 3f : base.Stat.AttackRange) - num;
					if (num < num2)
					{
						num = num2;
					}
				}
				this.MoveToDestination(worldCharacter.GetPosition() - a * (num - this.StoppingDistance));
				return;
			}
			this.MoveToDestination(obj.GetInteractionPoint(base.GetPosition()));
		}

		
		public void MoveToDestination(Vector3 destination)
		{
			if (!this.isAlive)
			{
				return;
			}
			BlisVector startPosition = new BlisVector(base.transform.position);
			BlisVector destination2 = new BlisVector(destination);
			this.moveAgent.MoveToDestination(startPosition, destination2, base.Status.MoveSpeed);
			ServerMoveToDestination serverMoveToDestination;
			if ((serverMoveToDestination = (this.moveAgent.CurrentMoveStrategy as ServerMoveToDestination)) != null)
			{
				BlisVector[] snapshotCorners = serverMoveToDestination.GetSnapshotCorners();
				if (snapshotCorners.Length != 0)
				{
					base.EnqueueCommand(new CmdMoveToDestination
					{
						destination = new BlisVector(serverMoveToDestination.Destination),
						corners = serverMoveToDestination.GetSnapshotCorners()
					});
				}
			}
		}

		
		public void MoveToDestination(Vector3 destination, BlisVector[] nextCorners)
		{
			if (nextCorners == null || nextCorners.Length == 0)
			{
				return;
			}
			base.EnqueueCommand(new CmdMoveToDestination
			{
				destination = new BlisVector(destination),
				corners = nextCorners
			});
		}

		
		public void MoveStraight(Vector3 destination, float duration, EasingFunction.Ease ease, bool canRotate)
		{
			if (!this.isAlive)
			{
				return;
			}
			BlisVector startPosition = new BlisVector(base.transform.position);
			BlisVector destination2 = new BlisVector(destination);
			this.moveAgent.MoveStraight(startPosition, destination2, duration, ease, canRotate);
			base.EnqueueCommand(new CmdMoveStraight
			{
				destination = destination2,
				duration = new BlisFixedPoint(duration),
				ease = ease,
				CanRotate = canRotate
			});
		}

		
		public void MoveStraightWithoutNavSpeed(Vector3 moveStartPos, Vector3 moveEndPos, float moveSpeed)
		{
			float num = GameUtil.DistanceOnPlane(moveStartPos, moveEndPos);
			this.MoveStraightWithoutNavDuration(moveStartPos, moveEndPos, num / moveSpeed);
		}

		
		public void MoveStraightWithoutNavDuration(Vector3 moveStartPos, Vector3 moveEndPos, float duration)
		{
			this.moveAgent.MoveStraightWithoutNav(moveStartPos, moveEndPos, duration);
			base.EnqueueCommand(new CmdMoveStraightWithoutNav
			{
				startPos = new BlisVector(moveStartPos),
				endPos = new BlisVector(moveEndPos),
				duration = new BlisFixedPoint(duration)
			});
		}

		
		public void MoveToTargetWithoutNavSpeed(Vector3 moveStartPos, WorldCharacter target, float moveSpeed, float arriveRadius)
		{
			this.moveAgent.MoveToTargetWithoutNav(moveStartPos, target, moveSpeed, arriveRadius);
			base.EnqueueCommand(new CmdMoveToTargetWithoutNav
			{
				startPos = new BlisVector(moveStartPos),
				targetId = target.ObjectId,
				moveSpeed = new BlisFixedPoint(moveSpeed),
				arriveRadius = new BlisFixedPoint(arriveRadius)
			});
		}

		
		public void WarpTo(Vector3 destination, bool needCheckNavMesh)
		{
			if (!this.isAlive)
			{
				return;
			}
			Vector3 vector;
			if (needCheckNavMesh && Blis.Common.MoveAgent.SamplePosition(destination, 2147483640, out vector))
			{
				destination = vector;
			}
			BlisVector blisVector = new BlisVector(destination);
			this.moveAgent.Warp(blisVector);
			base.EnqueueCommand(new CmdWarpTo
			{
				destination = blisVector
			});
		}

		
		public void StopMove()
		{
			this.moveAgent.Stop();
		}

		
		private void OnStopMove()
		{
			base.EnqueueCommand(new CmdStopMove());
		}

		
		public void LockRotation(bool isLock)
		{
			this.moveAgent.LockRotation(isLock);
			base.EnqueueCommand(new CmdLockRotation
			{
				isLock = isLock,
				rotationY = new BlisFixedPoint(base.transform.eulerAngles.y)
			});
		}

		
		public override void LookAt(Vector3 lookTo, float duration, bool isServerRotateInstant)
		{
			if (!this.moveAgent.IsLockRotation())
			{
				int num = Mathf.RoundToInt(GameUtil.GetDirectionAngle(base.transform.forward, lookTo) * 100f);
				if (num == 0)
				{
					return;
				}
				if (duration != 0f && (num == 18000 || num == -18000))
				{
					lookTo = Quaternion.AngleAxis(-1f, Vector3.up) * lookTo;
				}
			}
			Quaternion lookTo2 = GameUtil.LookRotation(lookTo);
			base.EnqueueCommand(new CmdLookAt
			{
				lookAtFromY = new BlisFixedPoint(base.transform.eulerAngles.y),
				lookAtToY = new BlisFixedPoint(lookTo2.eulerAngles.y),
				duration = new BlisFixedPoint(duration)
			});
			this.moveAgent.LookAt(base.transform.rotation, lookTo2, isServerRotateInstant ? 0f : duration);
		}

		
		public bool IsLockRotation()
		{
			return this.moveAgent.IsLockRotation();
		}

		
		public virtual void Interact(WorldObject target)
		{
			this.StopMove();
			if (target != null)
			{
				this.LookAt(target.InteractDirection(base.GetPosition()), 0.19f, true);
			}
		}

		
		public int GetCurrentAreaMask()
		{
			int currentAreaMask = AreaUtil.GetCurrentAreaMask(base.transform.position);
			if (currentAreaMask == 0)
			{
				return this.preAreaMaskCode;
			}
			this.preAreaMaskCode = currentAreaMask;
			return currentAreaMask;
		}

		
		public bool IsMoving()
		{
			return this.moveAgent.IsMoving;
		}

		
		public AreaData GetCurrentAreaData(LevelData levelData)
		{
			return AreaUtil.GetCurrentAreaDataByMask(levelData, this.moveAgent.WalkableNavMask, this.GetCurrentAreaMask());
		}

		
		public override void Damage(DamageInfo damageInfo)
		{
			if (!this.isAlive)
			{
				return;
			}
			this.combatInvolvementAgent.AddCombatEvent(damageInfo);
			base.Damage(damageInfo);
			this.SetInCombat(true, damageInfo.TargetInCombat ? damageInfo.Attacker : null);
		}

		
		public virtual bool StartSkillCooldown(SkillSlotSet skillSlotSet, MasteryType masteryType, float cooldown)
		{
			return cooldown > 0f && this.CharacterSkill.StartCooldown(skillSlotSet, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime, cooldown, base.Stat.CooldownReduction);
		}

		
		public virtual SkillSlotSet ModifyCooldown(SkillSlotSet skillSlotSetFlag, float modifyValue)
		{
			foreach (SkillSlotSet skillSlotSet in GameDB.skill.allSkillSlotSet)
			{
				if (skillSlotSet != SkillSlotSet.None && skillSlotSetFlag.HasFlag(skillSlotSet))
				{
					if (skillSlotSet.SlotSet2Index() == SkillSlotIndex.Attack)
					{
						skillSlotSetFlag &= ~skillSlotSet;
					}
					else if (this.CheckCooldown(skillSlotSet) || this.IsHoldCooldown(skillSlotSet))
					{
						skillSlotSetFlag &= ~skillSlotSet;
					}
					else
					{
						this.CharacterSkill.ModifyCooldown(skillSlotSet, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime, modifyValue);
					}
				}
			}
			return skillSlotSetFlag;
		}

		
		public virtual void SetHoldSkillCooldown(SkillSlotSet skillSlotSet, MasteryType masteryType, bool isHold)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				this.CharacterSkill.SetHoldCooldown(masteryType, isHold);
				return;
			}
			this.CharacterSkill.SetHoldCooldown(skillSlotSet, isHold);
		}

		
		public virtual bool SwitchSkillSet(SkillSlotIndex skillSlotIndex, SkillSlotSet skillSlotSet)
		{
			SkillSlotSet? skillSlotSet2 = this.characterSkill.GetSkillSlotSet(skillSlotIndex);
			if (!this.characterSkill.SwitchSkillSet(skillSlotIndex, skillSlotSet))
			{
				return false;
			}
			if (skillSlotSet2 != null && this.GetSkillLevel(skillSlotIndex) > 0)
			{
				SkillData skillData = this.GetSkillData(skillSlotSet2.Value, -1);
				if (skillData != null && skillData.PassiveSkillId != SkillId.None)
				{
					this.CancelPassiveSkill(skillData.PassiveSkillId);
				}
			}
			this.CastPassiveSkill(skillSlotIndex);
			return true;
		}

		
		public int GetSkillSequence(SkillSlotSet skillSlotSet)
		{
			return this.CharacterSkill.GetSkillSequence(skillSlotSet);
		}

		
		private bool IsLastSequence(SkillSlotSet skillSlotSet)
		{
			return this.CharacterSkill.IsLastSequence(skillSlotSet);
		}

		
		public virtual void SequenceTimeOver(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			if (!this.CharacterSkill.IsPlayingSequenceTimer(skillSlotSet, masteryType))
			{
				return;
			}
			this.CharacterSkill.ResetSkillSequence(skillSlotSet);
			SkillData skillData = this.GetSkillData(skillSlotSet, -1);
			this.CharacterSkill.StartCooldown(skillSlotSet, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime, skillData.cooldown + this.CharacterSkill.GetReservationCooldown(skillSlotSet), base.Stat.CooldownReduction);
		}

		
		public virtual void EndSequenceSkill(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			if (this.CharacterSkill.IsFirstSequence(skillSlotSet))
			{
				return;
			}
			this.CharacterSkill.ResetSkillSequence(skillSlotSet);
			SkillData skillData = this.GetSkillData(skillSlotSet, -1);
			this.CharacterSkill.StartCooldown(skillSlotSet, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime, skillData.cooldown + this.CharacterSkill.GetReservationCooldown(skillSlotSet), base.Stat.CooldownReduction);
		}

		
		public virtual void StackSkillNeedCharge(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			SkillData skillData = this.GetSkillData(skillSlotSet, -1);
			this.CharacterSkill.StartCooldown(skillSlotSet, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime, skillData.cooldown, base.Stat.CooldownReduction);
		}

		
		public void StartConcentration(SkillData skillData)
		{
			this.CharacterSkill.StartConcentration();
			base.EnqueueCommand(new CmdStartConcentration
			{
				skillCode = skillData.code
			});
		}

		
		public void EndConcentration(SkillData skillData, bool cancel)
		{
			this.CharacterSkill.EndConcentration();
			base.EnqueueCommand(new CmdEndConcentration
			{
				skillCode = skillData.code,
				cancel = cancel
			});
		}

		
		public bool IsConcentration()
		{
			return this.CharacterSkill.IsConcentrating();
		}

		
		protected void StartPassiveSkill(SkillUseInfo skillUseInfo)
		{
			base.EnqueueCommand(new CmdStartPassiveSkill
			{
				skillId = skillUseInfo.skillData.PassiveSkillId,
				skillCode = skillUseInfo.skillData.code,
				skillEvolutionLevel = skillUseInfo.skillEvolutionLevel,
				targetObjectId = ((skillUseInfo.target != null) ? skillUseInfo.target.ObjectId : 0)
			});
		}

		
		protected void StartStateSkill(SkillUseInfo skillUseInfo, int casterObjectId)
		{
			base.EnqueueCommand(new CmdStartStateSkill
			{
				skillId = skillUseInfo.stateData.GroupData.skillId,
				skillCode = skillUseInfo.skillData.code,
				skillEvolutionLevel = skillUseInfo.skillEvolutionLevel,
				casterId = casterObjectId
			});
		}

		
		public void StartSkill(SkillUseInfo skillUseInfo)
		{
			base.EnqueueCommand(new CmdStartSkill
			{
				skillId = skillUseInfo.skillData.SkillId,
				skillCode = skillUseInfo.skillData.code,
				skillEvolutionLevel = skillUseInfo.skillEvolutionLevel,
				targetObjectId = ((skillUseInfo.target != null) ? skillUseInfo.target.ObjectId : 0)
			});
		}

		
		public void PlayPassiveSkill(SkillUseInfo skillUseInfo)
		{
			this.skillController.PlayPassive(skillUseInfo);
		}

		
		public void PlaySkill(SkillUseInfo skillUseInfo)
		{
			if (skillUseInfo.injected)
			{
				this.SkillController.Play(skillUseInfo, 0);
				return;
			}
			SkillData skillData = GameDB.skill.GetSkillData(skillUseInfo.SkillCode);
			if (!this.CharacterSkill.IsSingleSequence(skillUseInfo.skillSlotSet))
			{
				this.CharacterSkill.RemoveSequenceTimer(skillUseInfo.skillSlotSet, skillUseInfo.weaponSkillMastery);
			}
			this.UpdateSkillCooldown(skillUseInfo, skillData);
			this.SkillController.Play(skillUseInfo, this.GetSkillSequence(skillUseInfo.skillSlotSet));
		}

		
		public void UpdateSkillCooldown(SkillUseInfo skillUseInfo, SkillData skillData)
		{
			SkillCooldownType cooldownType = skillData.CooldownType;
			if (cooldownType != SkillCooldownType.None)
			{
				if (cooldownType == SkillCooldownType.SkillStart)
				{
					this.StartSkillCooldown(skillUseInfo.skillSlotSet, skillUseInfo.weaponSkillMastery, skillData.cooldown);
					return;
				}
				if (skillData.CanAdditionalAction)
				{
					if (skillData.cooldownForAdditionalAction > 0f)
					{
						this.StartSkillCooldown(skillUseInfo.skillSlotSet, skillUseInfo.weaponSkillMastery, skillData.cooldownForAdditionalAction);
					}
				}
				else
				{
					this.SetHoldSkillCooldown(skillUseInfo.skillSlotSet, skillUseInfo.weaponSkillMastery, true);
				}
			}
		}

		
		public void StartStateSkill(SkillUseInfo skillUseInfo, CharacterState state)
		{
			base.EnqueueCommand(new CmdStartStateSkill
			{
				skillId = skillUseInfo.stateData.GroupData.skillId,
				skillCode = skillUseInfo.skillData.code,
				skillEvolutionLevel = skillUseInfo.skillEvolutionLevel,
				casterId = state.Caster.ObjectId
			});
			this.SkillController.PlayStateSkill(skillUseInfo, state);
		}

		
		public void PlayStateSkill(SkillUseInfo skillUseInfo, CharacterState state)
		{
			this.SkillController.PlayStateSkill(skillUseInfo, state);
		}

		
		protected void CastPassiveSkill(SkillSlotIndex skillSlotIndex)
		{
			if (this.GetSkillLevel(skillSlotIndex) == 0)
			{
				return;
			}
			SkillSlotSet? skillSlotSet = this.characterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return;
			}
			SkillData skillData = this.GetSkillData(skillSlotSet.Value, -1);
			if (skillData == null)
			{
				return;
			}
			if (skillData.PassiveSkillId == SkillId.None)
			{
				return;
			}
			SkillUseInfo skillUseInfo = SkillUseInfo.Create(this.mySkillAgent, this.mySkillAgent, skillData, skillSlotSet.Value, MasteryType.None, this.CharacterSkill.GetSkillEvolutionLevel(skillSlotIndex), Vector3.zero, Vector3.zero, null, false);
			this.SkillController.CastPassiveSkill(skillUseInfo);
		}

		
		protected void OverwritePassiveSkill(SkillSlotIndex skillSlotIndex)
		{
			if (this.GetSkillLevel(skillSlotIndex) == 0)
			{
				return;
			}
			SkillSlotSet? skillSlotSet = this.characterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return;
			}
			SkillData skillData = this.GetSkillData(skillSlotSet.Value, -1);
			if (skillData == null)
			{
				return;
			}
			if (skillData.PassiveSkillId == SkillId.None)
			{
				return;
			}
			SkillUseInfo skillUseInfo = SkillUseInfo.Create(this.mySkillAgent, this.mySkillAgent, skillData, skillSlotSet.Value, MasteryType.None, this.CharacterSkill.GetSkillEvolutionLevel(skillSlotIndex), Vector3.zero, Vector3.zero, null, false);
			this.SkillController.OverwritePassiveSkill(skillUseInfo);
		}

		
		protected void CancelPassiveSkill(SkillId passiveSkillId)
		{
			if (passiveSkillId == SkillId.None)
			{
				return;
			}
			this.SkillController.CancelPassiveSkill(passiveSkillId);
		}

		
		public void CancelStateSkill(SkillId skillId, int casterId, bool cancel)
		{
			this.SkillController.CancelStateSkill(skillId, casterId, cancel);
		}

		
		public void CancelSkill(SkillId skillId)
		{
			this.SkillController.Cancel(skillId);
		}

		
		public void PlayPassiveSkill(SkillId skillId, int actionNo, int targetId, BlisVector targetPosition)
		{
			base.EnqueueCommand(new CmdPlayPassiveSkill
			{
				skillId = skillId,
				actionNo = actionNo,
				targetId = targetId,
				targetPos = targetPosition
			});
		}

		
		public void PlaySkillAction(SkillId skillId, int actionNo, int targetId, BlisVector targetPosition)
		{
			base.EnqueueCommand(new CmdPlaySkillAction
			{
				skillId = skillId,
				actionNo = actionNo,
				targets = new List<SkillActionTarget>
				{
					new SkillActionTarget
					{
						targetId = targetId,
						targetPos = targetPosition
					}
				}
			});
		}

		
		public void PlaySkillAction(SkillId skillId, int actionNo, List<SkillActionTarget> targets)
		{
			base.EnqueueCommand(new CmdPlaySkillAction
			{
				skillId = skillId,
				actionNo = actionNo,
				targets = targets
			});
		}

		
		protected float GetSequenceCooldown(SkillData skillData, SkillSlotSet skillSlotSet)
		{
			SequenceItemCooldownApply itemCooldownApply = skillData.ItemCooldownApply;
			if (itemCooldownApply - SequenceItemCooldownApply.Cooldown <= 2)
			{
				return GameUtil.GetCooldown(skillData.SequenceCooldown, base.Stat.CooldownReduction) + this.CharacterSkill.GetReservationSequenceCooldown(skillSlotSet);
			}
			return skillData.SequenceCooldown + this.CharacterSkill.GetReservationSequenceCooldown(skillSlotSet);
		}

		
		protected float GetSequenceWaitTime(SkillData skillData, float sequenceCooltime)
		{
			if (skillData.ItemCooldownApply < SequenceItemCooldownApply.CooldownIncreaseWaitTime)
			{
				return skillData.CastWaitTime;
			}
			float num = skillData.SequenceCooldown - sequenceCooltime;
			float num2 = skillData.CastWaitTime;
			if (skillData.ItemCooldownApply.Equals(SequenceItemCooldownApply.CooldownDecreaseWaitTime))
			{
				num2 -= num;
			}
			if (skillData.ItemCooldownApply.Equals(SequenceItemCooldownApply.CooldownIncreaseWaitTime))
			{
				num2 += num;
			}
			return num2;
		}

		
		protected virtual void FinishSkill(SkillSlotSet skillSlotSet, MasteryType masteryType, SkillId skillId, bool toNextSequence, bool cancel, bool startCooldown)
		{
			if (toNextSequence)
			{
				this.CharacterSkill.NextSequence(skillSlotSet);
				SkillData skillData = this.GetSkillData(skillSlotSet, -1);
				float sequenceCooldown = this.GetSequenceCooldown(skillData, skillSlotSet);
				float sequenceWaitTime = this.GetSequenceWaitTime(skillData, sequenceCooldown);
				if (!this.CharacterSkill.IsFirstSequence(skillSlotSet) && 0f < sequenceWaitTime)
				{
					this.CharacterSkill.SetSequenceDuration(skillSlotSet, masteryType, sequenceWaitTime, sequenceCooldown, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
				}
				bool flag = false;
				if (startCooldown && this.CharacterSkill.GetSkillSequence(skillSlotSet) == 0)
				{
					this.StartSkillCooldown(skillSlotSet, masteryType, this.GetSkillData(skillSlotSet, 0).cooldown);
					flag = true;
				}
				if (!flag)
				{
					if (skillSlotSet == SkillSlotSet.WeaponSkill)
					{
						if (this.CharacterSkill.IsHoldCooldown(masteryType))
						{
							this.SetHoldSkillCooldown(skillSlotSet, masteryType, false);
						}
					}
					else if (this.CharacterSkill.IsHoldCooldown(skillSlotSet))
					{
						this.SetHoldSkillCooldown(skillSlotSet, masteryType, false);
					}
				}
			}
			else if (startCooldown)
			{
				this.StartSkillCooldown(skillSlotSet, masteryType, this.GetSkillData(skillSlotSet, 0).cooldown);
			}
			base.EnqueueCommand(new CmdFinishSkill
			{
				skillId = skillId,
				cancel = cancel,
				skillSlotSet = skillSlotSet
			});
		}

		
		private void FinishStateSkill(SkillSlotSet skillSlotSet, MasteryType masteryType, SkillId skillId, bool cancel, bool startCooldown)
		{
			if (startCooldown)
			{
				this.StartSkillCooldown(skillSlotSet, masteryType, this.GetSkillData(skillSlotSet, 0).cooldown);
			}
			base.EnqueueCommand(new CmdFinishStateSkill
			{
				skillId = skillId,
				cancel = cancel
			});
		}

		
		private void FinishPassiveSkill(SkillSlotSet skillSlotSet, SkillId skillId, bool cancel, bool startCooldown)
		{
			base.EnqueueCommand(new CmdFinishPassiveSkill
			{
				skillId = skillId,
				cancel = cancel
			});
		}

		
		protected virtual void ConsumeSkillResources(SkillData skillData)
		{
		}

		
		public bool IsReadySkill(SkillSlotSet skillSlotSet)
		{
			return this.GetSkillData(skillSlotSet, -1) != null && this.IsLearnSkill(skillSlotSet.SlotSet2Index()) && this.CheckCooldown(skillSlotSet) && (!this.IsLastSequence(skillSlotSet) || !this.IsHoldCooldown(skillSlotSet)) && this.EnoughSkillResources(skillSlotSet);
		}

		
		public bool IsLearnSkill(SkillSlotIndex skillSlotIndex)
		{
			return this.GetSkillLevel(skillSlotIndex) >= 1;
		}

		
		public virtual SkillData GetSkillData(SkillSlotSet skillSlotSet, int sequence = -1)
		{
			return GameDB.skill.GetSkillData(this.GetCharacterCode(), base.ObjectType, skillSlotSet, this.GetSkillLevel(skillSlotSet.SlotSet2Index()), (sequence >= 0) ? sequence : this.GetSkillSequence(skillSlotSet));
		}

		
		public SkillData GetSkillData(SkillSlotIndex skillSlotIndex, int sequence = -1)
		{
			SkillSlotSet? skillSlotSet = this.characterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return null;
			}
			return this.GetSkillData(skillSlotSet.Value, sequence);
		}

		
		protected virtual int GetSkillLevel(SkillSlotIndex skillSlotIndex)
		{
			return this.CharacterSkill.GetSkillLevel(skillSlotIndex);
		}

		
		public virtual bool CheckCooldown(SkillSlotSet skillSlotSet)
		{
			return this.CharacterSkill.CheckCooldown(skillSlotSet, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
		}

		
		protected virtual bool IsHoldCooldown(SkillSlotSet skillSlotSet)
		{
			return this.CharacterSkill.IsHoldCooldown(skillSlotSet);
		}

		
		public bool EnoughSkillResources(SkillSlotSet skillSlotSet)
		{
			SkillData skillData = this.GetSkillData(skillSlotSet, -1);
			return this.CheckSkillResource(skillData.CostType, skillData.CostKey, skillData.cost) && this.CheckSkillResource(skillData.ExCostType, skillData.ExCostKey, skillData.exCost);
		}

		
		protected virtual bool CheckSkillResource(SkillCostType costType, int costKey, int cost)
		{
			switch (costType)
			{
			case SkillCostType.NoCost:
				return true;
			case SkillCostType.Sp:
				return cost <= base.Status.Sp;
			case SkillCostType.StateStack:
				return cost <= this.stateEffector.GetStackByGroup(costKey, 0);
			case SkillCostType.EquipWeaponStack:
				return false;
			case SkillCostType.Hp:
				return true;
			case SkillCostType.Ep:
				return cost <= base.Status.ExtraPoint;
			default:
				return false;
			}
		}

		
		private bool IsValidTarget(SkillSlotSet skillSlotSet, WorldCharacter target)
		{
			SkillData skillData = this.GetSkillData(skillSlotSet, -1);
			return this.IsValidTarget(skillData, target);
		}

		
		private bool IsValidTarget(SkillData skillData, WorldCharacter target)
		{
			if (target == null)
			{
				return false;
			}
			switch (skillData.TargetType)
			{
			case SkillTargetType.Self:
				return base.ObjectId == target.ObjectId;
			case SkillTargetType.ExceptSelf:
				return base.ObjectId != target.ObjectId;
			case SkillTargetType.Ally:
				return base.GetHostileType(target) == HostileType.Ally;
			case SkillTargetType.Enemy:
				return base.GetHostileType(target) == HostileType.Enemy;
			case SkillTargetType.NotSpecified:
			case SkillTargetType.NotSpecifiedAndSummonObject:
				return true;
			default:
				return false;
			}
		}

		
		private bool IsItWithinSkillRange(SkillSlotSet skillSlotSet, WorldCharacter target)
		{
			SkillData skillData = this.GetSkillData(skillSlotSet, -1);
			return this.IsItWithinSkillRange(skillData, target);
		}

		
		private bool IsItWithinSkillRange(SkillData skillData, WorldCharacter target)
		{
			switch (skillData.CastWaysType)
			{
			case SkillCastWaysType.Instant:
				return true;
			case SkillCastWaysType.Directional:
			case SkillCastWaysType.PickPoint:
			case SkillCastWaysType.PickPointInArea:
			case SkillCastWaysType.PickPointThenDirection:
				return false;
			case SkillCastWaysType.PickTargetEdge:
				return !(target == null) && GameUtil.DistanceOnPlane(base.GetPosition(), target.GetPosition()) <= ServerUtil.GetSkillRangeWithRadius(skillData, this, target);
			case SkillCastWaysType.PickTargetCenter:
				return !(target == null) && GameUtil.DistanceOnPlane(base.GetPosition(), target.GetPosition()) <= ServerUtil.GetSkillRange(skillData, this);
			default:
				return false;
			}
		}

		
		private bool IsItWithinSkillRange(SkillSlotSet skillSlotSet, ref Vector3 point, ref Vector3 release)
		{
			SkillData skillData = this.GetSkillData(skillSlotSet, -1);
			return this.IsItWithinSkillRange(skillData, ref point, ref release);
		}

		
		private bool IsItWithinSkillRange(SkillData skillData, ref Vector3 point, ref Vector3 release)
		{
			switch (skillData.CastWaysType)
			{
			case SkillCastWaysType.Instant:
			case SkillCastWaysType.Directional:
				return true;
			case SkillCastWaysType.PickTargetEdge:
			case SkillCastWaysType.PickTargetCenter:
				return false;
			case SkillCastWaysType.PickPoint:
				return GameUtil.DistanceOnPlane(base.GetPosition(), point) <= skillData.range;
			case SkillCastWaysType.PickPointInArea:
				if (skillData.range < GameUtil.DistanceOnPlane(base.GetPosition(), point))
				{
					point = base.GetPosition() + GameUtil.DirectionOnPlane(base.GetPosition(), point) * skillData.range;
				}
				return true;
			case SkillCastWaysType.PickPointThenDirection:
				if (GameUtil.DistanceOnPlane(base.GetPosition(), point) <= skillData.range)
				{
					if (skillData.length < GameUtil.DistanceOnPlane(point, release))
					{
						release = point + GameUtil.DirectionOnPlane(point, release) * skillData.length;
					}
					return true;
				}
				return false;
			default:
				return false;
			}
		}

		
		public void SetGunReloading(bool isGunReloading)
		{
			this.isGunReloading = isGunReloading;
		}

		
		protected override void CompleteAddState(CharacterState state)
		{
			base.CompleteAddState(state);
			StateType stateType = state.StateGroupData.stateType;
			if (state.Caster.ObjectId != base.ObjectId && state.Caster.IsTypeOf<WorldMovableCharacter>() && state.StateGroupData.effectType == EffectType.Debuff)
			{
				this.SetInCombat(true, state.StateGroupData.debuffCombatTarget ? state.Caster : null);
				WorldMovableCharacter worldMovableCharacter = state.Caster as WorldMovableCharacter;
				if (worldMovableCharacter != null)
				{
					worldMovableCharacter.SetInCombat(true, state.StateGroupData.debuffCombatTarget ? this : null);
				}
			}
			if (stateType.IsCrowdControl())
			{
				this.OnCrowdControl(state);
			}
		}

		
		protected virtual void OnCrowdControl(CharacterState state)
		{
			if (this.CharacterSkill.IsConcentrating())
			{
				base.EnqueueCommand(new CmdCrowdControl
				{
					stateType = state.StateGroupData.stateType
				});
			}
			this.controller.OnCrowdControl();
			this.SkillController.OnCrowdControl(state.StateGroupData.stateType, state.StateData.GroupData.effectType);
			if (state.CancelMove())
			{
				this.StopMove();
			}
		}

		
		public virtual bool CanMove()
		{
			return this.stateEffector.CanMove() && this.SkillController.CanMoveDuringSkillPlaying();
		}

		
		public virtual bool CanStop()
		{
			return this.SkillController.CanStopDuringSkillPlaying();
		}

		
		public virtual bool CanAnyAction(ActionType actionType)
		{
			return this.stateEffector.CanAction(actionType) && this.SkillController.CanActionDuringSkillPlaying();
		}

		
		public virtual bool CanUseSkillWithoutRangeCheck(SkillSlotSet skillSlotSet, WorldCharacter target, Vector3 pressPoint, Vector3 releasePoint)
		{
			if (!base.IsAlive)
			{
				return false;
			}
			SkillData skillData = this.GetSkillData(skillSlotSet, -1);
			if (skillData == null)
			{
				return false;
			}
			SkillSlotIndex? skillSlotIndex = this.characterSkill.GetSkillSlotIndex(skillSlotSet);
			if (skillSlotIndex == null)
			{
				return false;
			}
			SkillSlotIndex? skillSlotIndex2 = skillSlotIndex;
			SkillSlotIndex skillSlotIndex3 = SkillSlotIndex.Attack;
			if (skillSlotIndex2.GetValueOrDefault() == skillSlotIndex3 & skillSlotIndex2 != null)
			{
				if (!this.IsCanNormalAttack())
				{
					return false;
				}
			}
			else if (!base.StateEffector.CanUseSkill(skillData))
			{
				return false;
			}
			return (this.SkillController.IsPlaying(skillSlotSet) || this.IsReadySkill(skillSlotSet)) && !this.SkillController.IsLockSlot(skillSlotSet) && this.SkillController.CanCastDuringSkillPlaying(base.ObjectId, skillSlotSet, skillData) && ((skillData.CastWaysType != SkillCastWaysType.PickTargetEdge && skillData.CastWaysType != SkillCastWaysType.PickTargetCenter) || this.IsValidTarget(skillSlotSet, target)) && (!(target != null) || base.ObjectType == ObjectType.Monster || this.sightAgent.IsInAllySight(target.SightAgent, target.GetPosition(), target.Stat.Radius, target.SightAgent.IsInvisibleCheckWithMemorizer(this.objectId)));
		}

		
		public virtual bool CanUseInjectSkill(int skillDataCode, WorldCharacter target, ref Vector3 point, ref Vector3 release)
		{
			if (!base.IsAlive)
			{
				return false;
			}
			SkillData skillData = GameDB.skill.GetSkillData(skillDataCode);
			if (skillData == null)
			{
				return false;
			}
			if (!base.StateEffector.CanUseSkill(skillData))
			{
				return false;
			}
			if (skillData.CastWaysType == SkillCastWaysType.PickTargetEdge || skillData.CastWaysType == SkillCastWaysType.PickTargetCenter)
			{
				if (target == null || !this.IsValidTarget(skillData, target) || !this.IsItWithinSkillRange(skillData, target))
				{
					return false;
				}
			}
			else if (!this.IsItWithinSkillRange(skillData, ref point, ref release))
			{
				return false;
			}
			return this.SkillController.CanCastDuringSkillPlaying(base.ObjectId, SkillSlotSet.None, skillData) && this.skillController.IsCanUseSkill(skillData.SkillId, 0, target, new Vector3?(point)) == UseSkillErrorCode.None && (!(target != null) || base.ObjectType == ObjectType.Monster || this.sightAgent.IsInAllySight(target.SightAgent, target.GetPosition(), target.Stat.Radius, target.SightAgent.IsInvisibleCheckWithMemorizer(this.objectId)));
		}

		
		public virtual bool CanUseSkill(SkillSlotSet skillSlotSet, WorldCharacter target, ref Vector3 point, ref Vector3 release)
		{
			if (!base.IsAlive)
			{
				return false;
			}
			if (this.IsDyingCondition)
			{
				return false;
			}
			int skillSequence = this.GetSkillSequence(skillSlotSet);
			SkillData skillData = this.GetSkillData(skillSlotSet, skillSequence);
			if (skillData == null)
			{
				return false;
			}
			SkillSlotIndex? skillSlotIndex = this.characterSkill.GetSkillSlotIndex(skillSlotSet);
			if (skillSlotIndex == null)
			{
				return false;
			}
			SkillSlotIndex? skillSlotIndex2 = skillSlotIndex;
			SkillSlotIndex skillSlotIndex3 = SkillSlotIndex.Attack;
			if (skillSlotIndex2.GetValueOrDefault() == skillSlotIndex3 & skillSlotIndex2 != null)
			{
				if (!this.IsCanNormalAttack())
				{
					return false;
				}
			}
			else if (!base.StateEffector.CanUseSkill(skillData))
			{
				return false;
			}
			if (!this.SkillController.IsPlaying(skillSlotSet) && !this.IsReadySkill(skillSlotSet))
			{
				return false;
			}
			if (this.SkillController.IsLockSlot(skillSlotSet))
			{
				return false;
			}
			if (this.skillController.IsCanUseSkill(skillData.SkillId, skillSequence, target, new Vector3?(point)) != UseSkillErrorCode.None)
			{
				return false;
			}
			if (skillData.CastWaysType == SkillCastWaysType.PickTargetEdge || skillData.CastWaysType == SkillCastWaysType.PickTargetCenter)
			{
				if (target == null || !this.IsValidTarget(skillSlotSet, target) || !this.IsItWithinSkillRange(skillSlotSet, target))
				{
					return false;
				}
			}
			else if (!this.IsItWithinSkillRange(skillSlotSet, ref point, ref release))
			{
				return false;
			}
			return this.SkillController.CanCastDuringSkillPlaying(base.ObjectId, skillSlotSet, skillData) && (!(target != null) || base.ObjectType == ObjectType.Monster || this.sightAgent.IsInAllySight(target.SightAgent, target.GetPosition(), target.Stat.Radius, target.SightAgent.IsInvisibleCheckWithMemorizer(this.objectId)));
		}

		
		public bool IsCanUseSkillInScriptCondition(SkillSlotSet skillSlotSet, WorldCharacter hitTarget, Vector3? point)
		{
			int skillSequence = this.GetSkillSequence(skillSlotSet);
			SkillData skillData = this.GetSkillData(skillSlotSet, skillSequence);
			return skillData != null && this.skillController.IsCanUseSkill(skillData.SkillId, skillSequence, hitTarget, point) == UseSkillErrorCode.None;
		}

		
		private bool CheckOnlySkillCoolDown(SkillSlotSet skillSlotSet)
		{
			return this.CharacterSkill.CheckOnlySkillCoolDown(skillSlotSet, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
		}

		
		private bool CanReserveSkill(SkillSlotSet skillSlotSet)
		{
			if (!base.IsAlive)
			{
				return false;
			}
			int skillSequence = this.GetSkillSequence(skillSlotSet);
			SkillData skillData = this.GetSkillData(skillSlotSet, skillSequence);
			if (skillData == null)
			{
				return false;
			}
			if (this.SkillController.IsLockSlot(skillSlotSet))
			{
				return false;
			}
			if (skillSequence > 0)
			{
				SkillData skillData2 = this.GetSkillData(skillSlotSet, skillSequence - 1);
				if (skillData2 != null)
				{
					SkillGroupData skillGroupData = GameDB.skill.GetSkillGroupData(skillData2.group);
					if (skillGroupData != null && skillGroupData.sequenceIncreaseType == SequenceIncreaseType.Conditional)
					{
						return false;
					}
				}
			}
			return this.CheckOnlySkillCoolDown(skillSlotSet) && (skillData.PlayType == SkillPlayType.Alone || !skillData.InstantCast());
		}

		
		public virtual UseSkillErrorCode UseSkill(SkillSlotSet skillSlotSet, Vector3 hitPosition, Vector3 releasePosition)
		{
			return this.UseSkill__(skillSlotSet, hitPosition, releasePosition, null, MasteryType.None);
		}

		
		public virtual UseSkillErrorCode UseSkill(SkillSlotSet skillSlotSet, WorldCharacter hitTarget)
		{
			return this.UseSkill__(skillSlotSet, hitTarget, null, MasteryType.None);
		}

		
		public virtual UseSkillErrorCode UseInjectSkill(SkillUseInfo skillUseInfo)
		{
			return this.UseInjectSkill__(skillUseInfo, null);
		}

		
		protected virtual UseSkillErrorCode UseSkill__(SkillSlotSet skillSlotSet, Vector3 hitPosition, Vector3 releasePosition, Action extraAction = null, MasteryType masteryType = MasteryType.None)
		{
			bool flag = false;
			int skillSequence = this.GetSkillSequence(skillSlotSet);
			SkillData skillData = this.GetSkillData(skillSlotSet, skillSequence);
			if (skillData == null)
			{
				return UseSkillErrorCode.InvalidAction;
			}
			UseSkillErrorCode useSkillErrorCode = this.skillController.IsCanUseSkill(skillData.SkillId, skillSequence, null, new Vector3?(hitPosition));
			if (useSkillErrorCode != UseSkillErrorCode.None)
			{
				return useSkillErrorCode;
			}
			bool flag2 = false;
			if (skillData.CanAdditionalAction)
			{
				if (this.SkillController.IsPlaying(skillSlotSet))
				{
					flag = (this.CheckCooldown(skillSlotSet) && !this.IsHoldCooldown(skillSlotSet));
					if (flag)
					{
						flag2 = true;
						this.PlayAgain(skillSlotSet, hitPosition, skillData);
					}
				}
				else if (this.controller.IsReservedAction(skillSlotSet))
				{
					flag = (this.CheckCooldown(skillSlotSet) && !this.IsHoldCooldown(skillSlotSet) && skillData.cooldownForAdditionalAction <= 0f);
					if (flag)
					{
						flag2 = true;
						this.controller.ReserveAddtionalAction(skillSlotSet, hitPosition, skillData);
					}
				}
			}
			if (!flag2)
			{
				flag = this.CanUseSkillWithoutRangeCheck(skillSlotSet, null, hitPosition, releasePosition);
				if (!flag)
				{
					flag = this.CanReserveSkill(skillSlotSet);
				}
				if (flag)
				{
					if (extraAction != null)
					{
						extraAction();
					}
					this.Controller.UsePointSkill(skillData, skillSlotSet, masteryType, hitPosition, releasePosition);
				}
			}
			if (!flag)
			{
				return UseSkillErrorCode.NotAvailableNow;
			}
			return UseSkillErrorCode.None;
		}

		
		protected virtual UseSkillErrorCode UseSkill__(SkillSlotSet skillSlotSet, WorldCharacter hitTarget, Action extraAction = null, MasteryType masteryType = MasteryType.None)
		{
			int skillSequence = this.GetSkillSequence(skillSlotSet);
			SkillData skillData = this.GetSkillData(skillSlotSet, skillSequence);
			if (skillData == null)
			{
				return UseSkillErrorCode.NotAvailableNow;
			}
			UseSkillErrorCode useSkillErrorCode = this.skillController.IsCanUseSkill(skillData.SkillId, skillSequence, hitTarget, null);
			if (useSkillErrorCode != UseSkillErrorCode.None)
			{
				return useSkillErrorCode;
			}
			bool flag;
			if (skillData.CanAdditionalAction && this.SkillController.IsPlaying(skillSlotSet))
			{
				flag = (this.CheckCooldown(skillSlotSet) && !this.IsHoldCooldown(skillSlotSet));
				if (flag)
				{
					this.SkillController.PlayAgain(skillData, this.GetSkillSequence(skillSlotSet), hitTarget);
				}
			}
			else
			{
				flag = this.CanUseSkillWithoutRangeCheck(skillSlotSet, hitTarget, Vector3.zero, Vector3.zero);
				if (!flag)
				{
					flag = this.CanReserveSkill(skillSlotSet);
				}
				if (flag)
				{
					if (extraAction != null)
					{
						extraAction();
					}
					this.Controller.UseTargetSkill(skillData, skillSlotSet, masteryType, hitTarget);
				}
			}
			if (!flag)
			{
				return UseSkillErrorCode.NotAvailableNow;
			}
			return UseSkillErrorCode.None;
		}

		
		protected UseSkillErrorCode UseInjectSkill__(SkillUseInfo skillUseInfo, Action extraAction = null)
		{
			if (skillUseInfo.skillData == null)
			{
				return UseSkillErrorCode.InvalidAction;
			}
			if (extraAction != null)
			{
				extraAction();
			}
			this.Controller.UseInjectSkill(skillUseInfo);
			return UseSkillErrorCode.None;
		}

		
		public void PlayAgain(SkillSlotSet skillSlotSet, Vector3 cursorPosition, SkillData skillData)
		{
			this.SetHoldSkillCooldown(skillSlotSet, MasteryType.None, true);
			this.SkillController.PlayAgain(skillData, this.GetSkillSequence(skillSlotSet), cursorPosition);
		}

		
		public bool NormalAttack(WorldCharacter target)
		{
			if (!target.IsAlive)
			{
				return false;
			}
			SkillSlotSet? skillSlotSet = this.characterSkill.GetSkillSlotSet(SkillSlotIndex.Attack);
			if (skillSlotSet == null)
			{
				return false;
			}
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			if (!this.CanUseSkill(skillSlotSet.Value, target, ref zero, ref zero2))
			{
				return false;
			}
			if (!this.SkillController.IsReadyNormalAttack(base.Stat.AttackDelay))
			{
				return false;
			}
			if (base.IsTypeOf<WorldPlayerCharacter>())
			{
				WorldPlayerCharacter worldPlayerCharacter = (WorldPlayerCharacter)this;
				Item weapon = worldPlayerCharacter.GetWeapon();
				if (weapon == null)
				{
					return false;
				}
				if (weapon.ItemData.IsGunType())
				{
					if (worldPlayerCharacter.IsGunReloading)
					{
						return false;
					}
					if (worldPlayerCharacter.Status.Bullet == 0)
					{
						worldPlayerCharacter.GunReload(true);
						return false;
					}
				}
				if (weapon.ItemData.IsThrowType() && weapon.IsEmptyBullet())
				{
					return false;
				}
				if (worldPlayerCharacter.IsRunningCastingAction())
				{
					return false;
				}
			}
			this.StopMove();
			SkillData skillData = this.GetSkillData(SkillSlotIndex.Attack, -1);
			SkillUseInfo skillUseInfo = SkillUseInfo.Create(this.mySkillAgent, target.SkillAgent, skillData, skillSlotSet.Value, MasteryType.None, 0, Vector3.zero, Vector3.zero, null, false);
			this.SkillController.Cast(skillUseInfo, this.GetSkillSequence(skillUseInfo.skillSlotSet));
			return true;
		}

		
		public void MountNormalAttack(int skillCode)
		{
			this.SkillController.MountNormalAttack(skillCode);
		}

		
		public void UnmountNormalAttack()
		{
			this.SkillController.UnmountNormalAttack();
		}

		
		public void Invisible(bool isInvisible)
		{
			base.SightAgent.SetIsInvisible(isInvisible);
			base.EnqueueCommand(new CmdUpdateCharacterInvisible
			{
				isInvisible = isInvisible
			});
			if (isInvisible && this.attachedSights != null)
			{
				for (int i = this.attachedSights.Count - 1; i >= 0; i--)
				{
					ServerSightAgent serverSightAgent = this.attachedSights[i];
					if (serverSightAgent.IsRemoveWhenInvisibleStart)
					{
						base.EnqueueCommand(new CmdRemoveSight
						{
							attachSightId = serverSightAgent.AttachSightId,
							targetId = serverSightAgent.ObjectId
						});
						serverSightAgent.Destroy();
					}
				}
			}
		}

		
		public bool CompareBush(WorldCharacter targetCharacter)
		{
			WorldMovableCharacter worldMovableCharacter = targetCharacter as WorldMovableCharacter;
			return worldMovableCharacter != null && this.moveAgent.CompareBush(worldMovableCharacter.moveAgent);
		}

		
		public List<KeyValuePair<int, int>> GetBuffers()
		{
			return this.stateEffector.GetBuffers(base.ObjectId);
		}

		
		public List<KeyValuePair<int, int>> GetDebuffers()
		{
			return this.stateEffector.GetDebuffers(base.ObjectId);
		}

		
		public virtual void OnSkillReserveCancel(SkillActionBase skillActionBase)
		{
		}

		
		public void SetReservationSequenceCooldown(SkillSlotSet skillSlotSet, float reservationCooldown)
		{
			this.characterSkill.SetReservationSequenceCooldown(skillSlotSet, reservationCooldown);
		}

		
		public void SetReservationCooldown(SkillSlotSet skillSlotSet, float reservationCooldown)
		{
			this.characterSkill.SetReservationCooldown(skillSlotSet, reservationCooldown);
		}

		
		public void RemoveReservationCooldown(SkillSlotSet skillSlotSet)
		{
			this.characterSkill.RemoveReservationCooldown(skillSlotSet);
		}

		
		public bool IsReservationCooldown(SkillSlotSet skillSlotSet)
		{
			return this.characterSkill.IsReservationCooldown(skillSlotSet);
		}

		
		public void ResetDestination(Vector3 destination)
		{
			this.moveAgent.ResetDestination(destination);
			base.EnqueueCommand(new CmdResetDestination
			{
				destination = new BlisVector(destination)
			});
		}

		
		private ServerCharacterSkill characterSkill;

		
		protected SkillController skillController;

		
		protected MovableCharacterController controller;

		
		private SummonObjectController mySummonController;

		
		private CombatInvolvementAgent combatInvolvementAgent;

		
		private CombatInvolvementAgent.CombatInvolvementResult combatInvolvementResult;

		
		protected ServerMoveAgent moveAgent;

		
		public readonly float StoppingDistance = 0.5f;

		
		private bool isGunReloading;

		
		protected bool isInCombat;

		
		private bool activatedPassive;

		
		private int preAreaMaskCode;

		
		private float inCombatTime;

		
		private readonly Vector3 GroundZero = new Vector3(1f, 0f, 1f);
	}
}
