using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public abstract class WorldSummonBase : WorldCharacter
	{
		
		
		public WorldPlayerCharacter Owner
		{
			get
			{
				return this.owner;
			}
		}

		
		
		public AttackerInfo AttackerInfo
		{
			get
			{
				this.attackerInfo.SetAttackerStat(this.owner, this.cachedOwnerStat);
				return this.attackerInfo;
			}
		}

		
		protected override int GetTeamNumber()
		{
			WorldPlayerCharacter worldPlayerCharacter = this.Owner;
			if (worldPlayerCharacter == null)
			{
				return 0;
			}
			return worldPlayerCharacter.TeamNumber;
		}

		
		
		public SummonData SummonData
		{
			get
			{
				return this.summonData;
			}
		}

		
		protected override HostileAgent GetHostileAgent()
		{
			return this.hostileAgent;
		}

		
		
		protected float CreatedTime
		{
			get
			{
				return this.createdTime;
			}
		}

		
		public override bool IsAggressive()
		{
			return true;
		}

		
		
		public SkillController SkillController
		{
			get
			{
				return this.skillController;
			}
		}

		
		public virtual void Init(SummonData summonData, WorldPlayerCharacter owner)
		{
			this.owner = owner;
			this.attackerInfo.SetAttackerStat(this.owner, this.cachedOwnerStat);
			this.mySkillAgent = new WorldSummonSkillAgent(this);
			this.hostileAgent = new SummonHostileAgent(this, (owner != null) ? owner.ObjectId : 0);
			this.summonData = summonData;
			base.name = summonData.name;
			this.createdTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			this.expireTimer = summonData.duration;
			CharacterStat characterStat = new CharacterStat();
			characterStat.UpdateCharacterStat(summonData);
			GameUtil.BindOrAdd<SkillController>(base.gameObject, ref this.skillController);
			base.Init(characterStat);
			if (owner != null)
			{
				if (summonData.sightShare)
				{
					this.sightAgent.SetOwner(owner.SightAgent);
				}
				owner.AddOwnSummon(this);
			}
			this.sightAgent.SetDetect(summonData.detectShare, summonData.detectInvisible);
			if (summonData.createVisibleTime > 0f)
			{
				base.StartCoroutine(CoroutineUtil.DelayedAction(summonData.createVisibleTime, delegate()
				{
					this.SetIsInvisible(true);
				}));
			}
		}

		
		public override byte[] CreateSnapshot()
		{
			return WorldObject.serializer.Serialize<SummonSnapshot>(new SummonSnapshot
			{
				statusSnapshot = WorldObject.serializer.Serialize<SummonStatusSnapshot>(new SummonStatusSnapshot(base.Status)),
				initialStat = base.Stat.CreateSnapshot(),
				initialStateEffect = base.StateEffector.CreateSnapshot(),
				skillController = this.SkillController.CreateSnapshot(),
				isInCombat = this.IsInCombat,
				isInvisible = base.IsInvisible,
				ownerId = ((this.owner != null) ? this.owner.ObjectId : 0),
				summonId = this.summonData.code
			});
		}

		
		public override void DestroySelf()
		{
			base.StartCoroutine(CoroutineUtil.DelayedAction(1, delegate()
			{
				MonoBehaviourInstance<GameService>.inst.Spawn.DestroyWorldObject(this);
			}));
		}

		
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.deadAction = null;
			this.expireAction = null;
			this.customAction = null;
			this.customActionCondition = null;
			if (this.owner != null)
			{
				this.owner.RemoveOwnSummon(this);
			}
		}

		
		public override void OnKill(DamageInfo damageInfo, WorldCharacter deadCharacter, List<int> assistTeamMemberObjectIds)
		{
			deadCharacter.IfTypeOf<WorldMonster>(delegate(WorldMonster monster)
			{
				if (monster.MonsterData.monster == MonsterType.Wickline)
				{
					string format = "[WICKLINE DEAD] SummonBase OnKill : ObjectType({0}), Name({1})";
					object arg = base.ObjectType;
					SummonData summonData = this.summonData;
					string text = string.Format(format, arg, (summonData != null) ? summonData.name : null);
					if (this.Owner != null)
					{
						string str = text;
						string str2 = ", Owner(";
						PlayerSession playerSession = this.Owner.PlayerSession;
						text = str + str2 + ((playerSession != null) ? playerSession.nickname : null) + ")";
					}
					else
					{
						text += ", Owner is Null";
					}
					Log.V(text);
				}
			});
			WorldPlayerCharacter worldPlayerCharacter = this.Owner;
			if (worldPlayerCharacter == null)
			{
				return;
			}
			worldPlayerCharacter.OnKill(damageInfo, this, assistTeamMemberObjectIds);
		}

		
		protected override void Dead(int finishingAttacker, List<int> assistants, DamageType damageType)
		{
			if (!this.isAlive)
			{
				return;
			}
			base.Dead(finishingAttacker, assistants, damageType);
			Action<WorldSummonBase> action = this.deadAction;
			if (action != null)
			{
				action(this);
			}
			this.DestroyGameObject();
		}

		
		protected void DestroyGameObject()
		{
			// co: generated method
			base.StartCoroutine(CoroutineUtil.DelayedAction(1, new Action(this.DestroyGameObjectg__SpawnDestroyWorldObject_31_0)));
		}
		
		
		private void Expire()
		{
			DirectDamageCalculator damageCalculator = new DirectDamageCalculator(WeaponType.None, DamageType.RedZone, DamageSubType.Normal, 0, 0, base.Status.Hp, 0, 1f);
			AttackerInfo attackerInfo = new AttackerInfo();
			attackerInfo.SetAttackerStat(null, null);
			Singleton<DamageService>.inst.SelfDamageTo(this, attackerInfo, damageCalculator, new Vector3?(base.GetPosition()), 0);
			Action action = this.expireAction;
			if (action == null)
			{
				return;
			}
			action();
		}

		
		protected override void OnFrameUpdate()
		{
			base.OnFrameUpdate();
			if (!base.IsAlive || this.summonData == null)
			{
				return;
			}
			if (0f < this.expireTimer)
			{
				this.expireTimer -= MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				if (this.expireTimer <= 0f)
				{
					this.Expire();
					return;
				}
			}
			if (this.customActionCondition != null && this.customActionCondition(this))
			{
				Action<WorldSummonBase> action = this.customAction;
				if (action == null)
				{
					return;
				}
				action(this);
			}
		}

		
		public override bool IsAttackable(WorldCharacter target)
		{
			return this.isAlive && !(target == null) && target.IsAlive && !target.IsUntargetable() && this.owner.GetHostileType(target) == HostileType.Enemy;
		}

		
		public void SetIsInvisible(bool isInvisible)
		{
			base.SightAgent.SetIsInvisible(isInvisible);
			this.UpdateSummonInvisible();
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

		
		public void UpdateSummonInvisible()
		{
			base.EnqueueCommand(new CmdUpdateCharacterInvisible
			{
				isInvisible = base.IsInvisible
			});
		}

		
		public override bool CanFindForAttack()
		{
			return base.CanFindForAttack() && !this.summonData.isInvincibility;
		}

		
		public void ResetDuration(float duration)
		{
			this.expireTimer = duration;
			base.EnqueueCommand(new CmdResetSummonDuration
			{
				duration = new BlisFixedPoint(duration)
			});
		}

		
		public bool IsOwner(int objectId)
		{
			return this.owner != null && objectId == this.owner.ObjectId;
		}

		
		public void DeadAction(Action<WorldSummonBase> action)
		{
			this.deadAction = action;
		}

		
		public void SetActionOnExpire(Action action)
		{
			this.expireAction = action;
		}

		
		public void SetCustomAction(Action<WorldSummonBase> customAction, Func<WorldSummonBase, bool> customActionCondition)
		{
			this.customAction = customAction;
			this.customActionCondition = customActionCondition;
		}

		
		public void SelfCustomAction(WorldSummonBase summonBase)
		{
			if (!summonBase.ObjectId.Equals(base.ObjectId))
			{
				return;
			}
			Action<WorldSummonBase> action = this.customAction;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		
		public void StartSkill(SkillId skillId)
		{
		}

		
		public void PlayPassiveSkill(SkillUseInfo skillUseInfo)
		{
		}

		
		public void PlaySkill(SkillUseInfo skillUseInfo)
		{
		}

		
		public void PlaySkillAction(SkillId skillId, int actionNo, int targetId, BlisVector targetPosition)
		{
		}

		
		public void PlaySkillAction(SkillId skillId, int actionNo, List<SkillActionTarget> targets)
		{
		}

		
		private void FinishSkill(SkillScript skillScript, bool toNextSequence, bool cancel)
		{
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

		
		[CompilerGenerated]
		private void DestroyGameObjectg__SpawnDestroyWorldObject_31_0()
		{
			MonoBehaviourInstance<GameService>.inst.Spawn.DestroyWorldObject(this);
		}

		
		private WorldPlayerCharacter owner;

		
		private readonly SimpleCharacterStat cachedOwnerStat = new SimpleCharacterStat();

		
		private readonly AttackerInfo attackerInfo = new AttackerInfo();

		
		private SummonData summonData;

		
		private SummonHostileAgent hostileAgent;

		
		private float createdTime;

		
		private float expireTimer;

		
		private Action<WorldSummonBase> deadAction;

		
		private Action expireAction;

		
		private Func<WorldSummonBase, bool> customActionCondition;

		
		private Action<WorldSummonBase> customAction;

		
		protected SkillController skillController;
	}
}
