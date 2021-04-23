using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public abstract class WorldCharacter : WorldObject
	{
		
		protected override ColliderAgent GetColliderAgent()
		{
			return this.colliderAgent;
		}

		
		protected abstract int GetCharacterCode();

		
		
		public int CharacterCode
		{
			get
			{
				return this.GetCharacterCode();
			}
		}

		
		
		public bool IsAlive
		{
			get
			{
				return this.isAlive;
			}
		}

		
		
		public virtual bool IsDyingCondition
		{
			get
			{
				return false;
			}
		}

		
		
		public CharacterStat Stat
		{
			get
			{
				return this.stat;
			}
		}

		
		
		public CharacterStatus Status
		{
			get
			{
				return this.status;
			}
		}

		
		
		public StateEffector StateEffector
		{
			get
			{
				return this.stateEffector;
			}
		}

		
		
		public ServerSightAgent SightAgent
		{
			get
			{
				return this.sightAgent;
			}
		}

		
		
		public bool IsInvisible
		{
			get
			{
				return this.SightAgent.IsInvisible;
			}
		}

		
		
		public virtual bool IsInBush
		{
			get
			{
				return false;
			}
		}

		
		protected override SkillAgent GetSkillAgent()
		{
			return this.mySkillAgent;
		}

		
		
		public float ActualCriticalChance
		{
			get
			{
				return this.actualCriticalChance;
			}
		}

		
		
		public virtual bool IsInCombat
		{
			get
			{
				return true;
			}
		}

		
		
		public virtual bool IsAI
		{
			get
			{
				return false;
			}
		}

		
		protected override IItemBox GetItemBox()
		{
			if (this.isAlive || this.corpseBox == null)
			{
				throw new GameException(ErrorType.InvalidAction);
			}
			return this.corpseBox;
		}

		
		protected virtual void Init(CharacterStat stat)
		{
			this.SetStat(stat);
			this.Status.SetHp(stat.MaxHp);
			this.Status.SetSp(stat.MaxSp);
			this.Status.SetLevel(1);
			this.Status.SetShield(0);
			this.Status.SetMoveSpeed(stat.MoveSpeed);
			this.isAlive = true;
			GameUtil.BindOrAdd<StateEffector>(base.gameObject, ref this.stateEffector);
			this.stateEffector.CompleteAddState = new StateEffector.ModifyState(this.CompleteAddState);
			this.stateEffector.CompleteRemoveState = new StateEffector.ModifyStateWithCmd(this.CompleteRemoveState);
			this.stateEffector.CompleteChangedState = new StateEffector.ModifyState(this.CompleteChangedState);
			this.stateEffector.CompleteResetCreateTimeState = new StateEffector.ModifyState(this.CompleteResetCreateTimeState);
			this.stateEffector.CompletePauseState = new StateEffector.ModifyState(this.CompletePauseState);
			this.stateEffector.OnChangedStat = new StateEffector.ChangedStat(this.OnChangedStateEffectStat);
			this.stateEffector.Init();
			GameUtil.BindOrAdd<ServerSightAgent>(base.gameObject, ref this.sightAgent);
			this.sightAgent.InitCharacterSight(this);
			this.sightAgent.SetDetect(true, false);
			this.sightAgent.UpdateSightRange(stat.SightRange);
			this.sightAgent.UpdateSightAngle(stat.SightAngle);
			GameUtil.BindOrAdd<CharacterColliderAgent>(base.gameObject, ref this.colliderAgent);
			this.colliderAgent.Init(stat.Radius);
			this.status.ChangeHpEvent = new Action<int>(this.OnChangeHp);
			this.collisionObject = new CollisionCircle3D(base.GetPosition(), stat.Radius);
		}

		
		public void SetStat(CharacterStat stat)
		{
			this.stat = stat;
			stat.ChangeMaxHpEvent = new Action<int>(this.OnChangeMaxHp);
		}

		
		protected override void OnFrameUpdate()
		{
			base.OnFrameUpdate();
			this.StateEffector.FrameUpdate();
			this.StatFlushUpdates();
		}

		
		protected virtual void StatFlushUpdates()
		{
			this.broadcastUpdateStats.Clear();
			List<CharacterStatValue> list = this.stat.FlushUpdates();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].statType.IsRequiredByClient() && list[i].statType.IsBroadcastType())
				{
					this.broadcastUpdateStats.Add(list[i]);
				}
			}
			if (0 < this.broadcastUpdateStats.Count)
			{
				base.EnqueueCommand(new CmdBroadcastUpdateStat
				{
					updates = this.broadcastUpdateStats
				});
			}
			if (0 < list.Count)
			{
				this.updateHashSet.Clear();
				list.ForEach(delegate(CharacterStatValue stat)
				{
					this.updateHashSet.Add(stat.statType);
				});
				this.OnUpdateStat(this.updateHashSet);
			}
		}

		
		public override bool IsAttackable(WorldCharacter target)
		{
			return this.isAlive && !(target == null) && target.IsAlive && !target.IsUntargetable() && this.GetHostileType(target) == HostileType.Enemy && this.SightAgent.IsInAllySight(target.SightAgent, target.GetPosition(), target.Stat.Radius, target.SightAgent.IsInvisibleCheckWithMemorizer(this.objectId));
		}

		
		public virtual bool IsCanNormalAttack()
		{
			return false;
		}

		
		public void ResetActualCriticalChance()
		{
			this.actualCriticalChance = 0f;
		}

		
		public void AddActualCriticalChance(float addChance)
		{
			this.actualCriticalChance += addChance;
		}

		
		public bool IsInAttackableDistance(WorldCharacter target)
		{
			return !(target == null) && this.IsInDistance(base.GetPosition(), this.stat.AttackRange, target.GetPosition(), target.Stat.Radius);
		}

		
		public bool IsInTeamRevivalDistance(WorldCharacter target)
		{
			return !(target == null) && this.IsInDistance(base.GetPosition(), this.stat.Radius, target.GetPosition(), target.stat.Radius);
		}

		
		public bool IsInDistance(Vector3 position, float range, Vector3 targetPosition, float targetRaidus)
		{
			position.y = 0f;
			targetPosition.y = 0f;
			return (targetPosition - position).sqrMagnitude <= Mathf.Pow(range + targetRaidus, 2f);
		}

		
		public bool IsInInteractableDistance(WorldObject target)
		{
			Vector3 interactionPoint = target.GetInteractionPoint(base.GetPosition());
			float num = Mathf.Max(0.6f, this.stat.Radius);
			return GameUtil.DistanceOnPlane(interactionPoint, base.GetPosition()) <= num;
		}

		
		protected virtual void CompleteAddState(CharacterState state)
		{
			base.EnqueueCommand(new CmdAddState
			{
				code = state.Code,
				casterId = state.Caster.objectId,
				duration = new BlisFixedPoint(state.Duration),
				stackCount = state.StackCount,
				reserveCount = state.ReserveCount,
				originalDuration = new BlisFixedPoint(state.OriginalDuration)
			});
			SingletonMonoBehaviour<BattleEventCollector>.inst.OnCompleteAddState(this, state);
		}

		
		protected virtual void CompleteRemoveState(CharacterState state, bool sendPacket)
		{
			if (sendPacket)
			{
				base.EnqueueCommand(new CmdRemoveState
				{
					group = state.Group,
					casterId = state.Caster.objectId
				});
			}
			SingletonMonoBehaviour<BattleEventCollector>.inst.OnCompleteRemoveState(this, state);
		}

		
		private void CompleteChangedState(CharacterState state)
		{
			base.EnqueueCommand(new CmdUpdateState
			{
				group = state.Group,
				casterId = state.Caster.objectId,
				stackCount = state.StackCount,
				reserveCount = state.ReserveCount,
				duration = new BlisFixedPoint(state.Duration),
				createdTime = new BlisFixedPoint(state.CreatedTime)
			});
			SingletonMonoBehaviour<BattleEventCollector>.inst.OnCompleteChangedState(this, state);
		}

		
		private void CompleteResetCreateTimeState(CharacterState state)
		{
			base.EnqueueCommand(new CmdResetCreateTimeState
			{
				group = state.Group,
				casterId = state.Caster.objectId
			});
			SingletonMonoBehaviour<BattleEventCollector>.inst.OnCompleteChangedState(this, state);
		}

		
		private void CompletePauseState(CharacterState state)
		{
			base.EnqueueCommand(new CmdPauseState
			{
				group = state.Group,
				casterId = state.Caster.ObjectId,
				durationPauseEndTime = state.DurationPauseEndTime,
				duration = state.Duration
			});
		}

		
		protected virtual void OnChangedStateEffectStat()
		{
			this.stat.UpdateStateStat(this.stateEffector.GetAllStat());
			if (this.status.Shield != this.stateEffector.GetShield())
			{
				this.status.SetShield(this.stateEffector.GetShield());
				base.EnqueueCommand(new CmdUpdateShield
				{
					shield = this.status.Shield
				});
			}
		}

		
		protected virtual void OnUpdateStat(HashSet<StatType> updateStats)
		{
			if (updateStats.Contains(StatType.MaxHp) || updateStats.Contains(StatType.MaxSp))
			{
				this.status.SetHp(Mathf.Min(this.stat.MaxHp, this.status.Hp));
				this.status.SetSp(Mathf.Min(this.stat.MaxSp, this.status.Sp));
			}
			if (updateStats.Contains(StatType.Radius))
			{
				this.colliderAgent.UpdateRadius(this.stat.Radius);
			}
			if (updateStats.Contains(StatType.SightRange))
			{
				this.sightAgent.UpdateSightRange(this.stat.SightRange);
			}
			if (updateStats.Contains(StatType.SightAngle))
			{
				this.sightAgent.UpdateSightAngle(this.stat.SightAngle);
			}
		}

		
		public virtual void Damage(DamageInfo damageInfo)
		{
			if (!this.isAlive)
			{
				return;
			}
			if (damageInfo.DamageType == DamageType.Sp)
			{
				if (damageInfo.MinRemain > 0 && this.status.Sp - damageInfo.Damage < damageInfo.MinRemain)
				{
					damageInfo.SetDamage(Mathf.Max(this.status.Sp - damageInfo.MinRemain, 0));
				}
				this.status.SubSp(damageInfo.Damage);
				int attackerId = 0;
				if (damageInfo.Attacker != null)
				{
					damageInfo.Attacker.IfTypeOf<WorldMovableCharacter>(delegate(WorldMovableCharacter character)
					{
						character.SetInCombat(true, damageInfo.TargetInCombat ? damageInfo.Attacker : null);
					});
					attackerId = damageInfo.Attacker.objectId;
				}
				base.EnqueueCommand(new CmdSpDamage
				{
					attackerId = attackerId,
					damage = damageInfo.Damage,
					effectCode = damageInfo.EffectAndSoundCode
				});
				return;
			}
			int num2;
			int num = this.stateEffector.Damage(damageInfo, out num2);
			if (damageInfo.MinRemain > 0 && this.status.Hp - num < damageInfo.MinRemain)
			{
				num = Mathf.Max(this.status.Hp - damageInfo.MinRemain, 0);
			}
			if (0 < num)
			{
				this.status.SubHp(num);
			}
			int attackerId2 = 0;
			damageInfo.SetDamage(num);
			if (damageInfo.Attacker != null)
			{
				damageInfo.Attacker.IfTypeOf<WorldMovableCharacter>(delegate(WorldMovableCharacter character)
				{
					character.SetInCombat(true, damageInfo.TargetInCombat ? damageInfo.Attacker : null);
				});
				attackerId2 = damageInfo.Attacker.objectId;
			}
			if (0 < damageInfo.Damage)
			{
				if (damageInfo.DamageType == DamageType.Skill)
				{
					base.EnqueueCommand(new CmdSkillDamage
					{
						attackerId = attackerId2,
						damage = damageInfo.Damage,
						effectCode = damageInfo.EffectAndSoundCode
					});
				}
				else
				{
					base.EnqueueCommand(new CmdDamage
					{
						attackerId = attackerId2,
						damage = damageInfo.Damage,
						isCritical = damageInfo.IsCritical,
						effectCode = damageInfo.EffectAndSoundCode
					});
				}
			}
			if (num2 > 0)
			{
				base.EnqueueCommand(new CmdBlock
				{
					damage = num2
				});
			}
			if (this.status.Hp <= 0)
			{
				this.OnDying(damageInfo);
			}
		}

		
		protected virtual void OnDying(DamageInfo damageInfo)
		{
			WorldCharacter attacker = damageInfo.Attacker;
			if (attacker != null)
			{
				attacker.OnKill(damageInfo, this, this.emptyAssistants);
			}
			WorldCharacter attacker2 = damageInfo.Attacker;
			this.Dead((attacker2 != null) ? attacker2.ObjectId : 0, null, damageInfo.DamageType);
		}

		
		public virtual void OnKill(DamageInfo damageInfo, WorldCharacter deadCharacter, List<int> assistCharacterObjectIds)
		{
			base.EnqueueCommand(new CmdKill
			{
				deadCharacterObjectId = deadCharacter.ObjectId,
				assistCharacterObjectIds = assistCharacterObjectIds
			});
		}

		
		protected virtual void Dead(int finishingAttacker, List<int> assistants, DamageType damageType)
		{
			if (!this.isAlive)
			{
				return;
			}
			SightAgent owner = this.sightAgent.GetOwner();
			if (owner != null)
			{
				owner.RemoveAllySight(this.sightAgent);
			}
			this.status.SubHp(this.stat.MaxHp);
			this.status.SubSp(this.stat.MaxSp);
			this.status.UpdateBullet(0);
			this.isAlive = false;
			this.stateEffector.RemoveOnDead(false);
			base.EnqueueCommand(new CmdDead
			{
				finishingAttacker = finishingAttacker,
				assistantIds = assistants
			});
		}

		
		public void Dead(DamageType damageType)
		{
			this.Dead(base.ObjectId, null, damageType);
		}

		
		protected virtual void OnHealHp(int delta)
		{
		}

		
		protected virtual void OnHealSp(int delta)
		{
		}

		
		public virtual void Heal(HealInfo healInfo)
		{
			if (!this.isAlive)
			{
				return;
			}
			int num = healInfo.Hp;
			int sp = healInfo.Sp;
			if (num == 0 && sp == 0)
			{
				return;
			}
			int num2 = 0;
			int num3 = 0;
			if (this.isAlive && 0 < num && this.status.Hp < this.stat.MaxHp)
			{
				if (healInfo.NeedApplyHealRatio && this.stat.HpHealRatio != 0f)
				{
					num = Mathf.CeilToInt((float)num * (1f + this.stat.HpHealRatio));
				}
				if (0 < num)
				{
					num2 = Mathf.Min(this.stat.MaxHp - this.status.Hp, num);
					this.status.AddHp(num2);
					this.OnHealHp(num2);
				}
			}
			if (this.isAlive && 0 < sp && this.status.Sp < this.stat.MaxSp)
			{
				num3 = Mathf.Min(this.stat.MaxSp - this.status.Sp, sp);
				this.status.AddSp(num3);
				this.OnHealSp(num3);
			}
			if (num2 > 0 && num3 > 0)
			{
				base.EnqueueCommand(healInfo.GetHealPacket(num2, num3));
				return;
			}
			if (num2 > 0)
			{
				base.EnqueueCommand(healInfo.GetHpHealPacket(num2));
				return;
			}
			if (num3 > 0)
			{
				base.EnqueueCommand(healInfo.GetSpHealPacket(num3));
			}
		}

		
		public virtual void ModifyExtraPoint(int changeAmountExtraPoint)
		{
		}

		
		public void Evasion()
		{
			base.EnqueueCommand(new CmdEvasion());
		}

		
		public CollisionObject3D GetCollisionObject()
		{
			this.collisionObject.UpdatePosition(base.GetPosition());
			return this.collisionObject;
		}

		
		public HostileType GetHostileType(WorldCharacter target)
		{
			return this.GetHostileAgent().GetHostileType(target.GetHostileAgent());
		}

		
		protected abstract HostileAgent GetHostileAgent();

		
		public List<WorldCharacter> FindEnemiesForAttack(Vector3 center, float range)
		{
			List<WorldCharacter> list = this.sightAgent.FindAllInAllySights<WorldCharacter>();
			for (int i = list.Count - 1; i >= 0; i--)
			{
				WorldCharacter worldCharacter = list[i];
				if (!worldCharacter.CanFindForAttack() || !worldCharacter.isAlive || this.GetHostileType(worldCharacter) != HostileType.Enemy || !this.IsInDistance(center, range, worldCharacter.GetPosition(), worldCharacter.stat.Radius))
				{
					list.RemoveAt(i);
				}
			}
			return list;
		}

		
		public List<WorldMonster> FindEnemyMonstersForAttack(Vector3 center, float range, List<MonsterType> monsterTypeList)
		{
			List<WorldMonster> list = this.sightAgent.FindAllInAllySights<WorldMonster>();
			for (int i = list.Count - 1; i >= 0; i--)
			{
				WorldMonster worldMonster = list[i];
				if (!monsterTypeList.Contains(worldMonster.MonsterData.monster) || !worldMonster.isAlive || !worldMonster.CanFindForAttack() || this.GetHostileType(worldMonster) != HostileType.Enemy || !this.IsInDistance(center, range, worldMonster.GetPosition(), worldMonster.stat.Radius))
				{
					list.RemoveAt(i);
				}
			}
			return list;
		}

		
		public List<WorldPlayerCharacter> FindEnemyPlayersForAttack(Vector3 center, float range, PlayerType playerType)
		{
			List<WorldPlayerCharacter> list = this.sightAgent.FindAllInAllySights<WorldPlayerCharacter>();
			for (int i = list.Count - 1; i >= 0; i--)
			{
				WorldPlayerCharacter worldPlayerCharacter = list[i];
				if (worldPlayerCharacter.PlayerType != playerType || !worldPlayerCharacter.CanFindForAttack() || !worldPlayerCharacter.isAlive || this.GetHostileType(worldPlayerCharacter) != HostileType.Enemy || !this.IsInDistance(center, range, worldPlayerCharacter.GetPosition(), worldPlayerCharacter.stat.Radius))
				{
					list.RemoveAt(i);
				}
			}
			return list;
		}

		
		public List<WorldPlayerCharacter> FindEnemyPlayersForAttack(Vector3 center, float range)
		{
			List<WorldPlayerCharacter> list = this.sightAgent.FindAllInAllySights<WorldPlayerCharacter>();
			for (int i = list.Count - 1; i >= 0; i--)
			{
				WorldPlayerCharacter worldPlayerCharacter = list[i];
				if (worldPlayerCharacter.ObjectType != ObjectType.PlayerCharacter || !worldPlayerCharacter.CanFindForAttack() || !worldPlayerCharacter.isAlive || this.GetHostileType(worldPlayerCharacter) != HostileType.Enemy || !this.IsInDistance(center, range, worldPlayerCharacter.GetPosition(), worldPlayerCharacter.stat.Radius))
				{
					list.RemoveAt(i);
				}
			}
			return list;
		}

		
		public WorldCharacter FindEnemyForAttack()
		{
			return this.FindEnemyForAttack(base.GetPosition(), this.stat.AttackRange);
		}

		
		public WorldCharacter FindEnemyForAttack(Vector3 center, float range)
		{
			return this.FindEnemiesForAttack(center, range).NearestOne(center);
		}

		
		public WorldMonster FindEnemyMonsterForAttack(Vector3 center, float range, List<MonsterType> monsterType)
		{
			return this.FindEnemyMonstersForAttack(center, range, monsterType).NearestOne(center);
		}

		
		public WorldPlayerCharacter FindEnemyPlayerForAttack(Vector3 center, float range, PlayerType playerType)
		{
			return this.FindEnemyPlayersForAttack(center, range, playerType).NearestOne(center);
		}

		
		public WorldPlayerCharacter FindEnemyPlayerForAttack(Vector3 center, float range)
		{
			return this.FindEnemyPlayersForAttack(center, range).NearestOne(center);
		}

		
		public List<WorldCharacter> GetEnemies(CollisionObject3D collisionObject)
		{
			List<WorldCharacter> enemiesWithinRange = this.GetEnemiesWithinRange(collisionObject.Position, collisionObject.Radius, null);
			for (int i = enemiesWithinRange.Count - 1; i >= 0; i--)
			{
				if (!collisionObject.Collision(enemiesWithinRange[i].GetCollisionObject()))
				{
					enemiesWithinRange.RemoveAt(i);
				}
			}
			return enemiesWithinRange;
		}

		
		public List<WorldCharacter> GetEnemiesWithinRange(Vector3 position, float range, Func<WorldCharacter, bool> condition)
		{
			if (this.enemies == null)
			{
				this.enemies = new List<WorldCharacter>();
			}
			else
			{
				this.enemies.Clear();
			}
			if (this.findEnemyColliders == null)
			{
				this.findEnemyColliders = new Collider[50];
			}
			int num = Physics.OverlapSphereNonAlloc(position, range, this.findEnemyColliders, GameConstants.LayerMask.WORLD_OBJECT_LAYER);
			while (this.findEnemyColliders.Length <= num)
			{
				this.findEnemyColliders = new Collider[this.findEnemyColliders.Length + 50];
				num = Physics.OverlapSphereNonAlloc(position, range, this.findEnemyColliders, GameConstants.LayerMask.WORLD_OBJECT_LAYER);
			}
			for (int i = 0; i < num; i++)
			{
				WorldCharacter component = this.findEnemyColliders[i].GetComponent<WorldCharacter>();
				if (!(component == null) && component.IsAlive && base.ObjectId != component.ObjectId && this.GetHostileType(component) == HostileType.Enemy && (condition == null || condition(component)) && !component.IsUntargetable())
				{
					this.enemies.Add(component);
				}
			}
			return this.enemies;
		}

		
		public virtual bool IsAggressive()
		{
			return false;
		}

		
		public virtual bool CanFindForAttack()
		{
			return !this.IsUntargetable() && (this.IsInCombat || this.IsAggressive());
		}

		
		private ServerSightAgent SightAgentCheckOverlap(WorldObject target, float sightRange, float duration)
		{
			if (target.attachedSights == null)
			{
				return null;
			}
			for (int i = 0; i < target.attachedSights.Count; i++)
			{
				if (target.attachedSights[i].GetOwner() == this.SightAgent && target.attachedSights[i].SightRange == sightRange && target.attachedSights[i].DestroyTime < duration + MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime)
				{
					return target.attachedSights[i];
				}
			}
			return null;
		}

		
		private void SetRemoveSight(ServerSightAgent sightAgent, float duration, int targetId)
		{
			if ((UnityEngine.Object) sightAgent == (UnityEngine.Object) null)
				return;
			if ((double) duration == 0.0)
			{
				sightAgent.StopDestroy();
			}
			else
			{
				int attachSightId = sightAgent.AttachSightId;
				sightAgent.DelayDestroy(duration, (Action) (() => this.EnqueueCommand((CommandPacket) new CmdRemoveSight()
				{
					attachSightId = attachSightId,
					targetId = targetId
				})));
			}
			
			// co: dotPeek
			// WorldCharacter.<>c__DisplayClass94_0 CS$<>8__locals1 = new WorldCharacter.<>c__DisplayClass94_0();
			// CS$<>8__locals1.<>4__this = this;
			// CS$<>8__locals1.targetId = targetId;
			// if (sightAgent == null)
			// {
			// 	return;
			// }
			// if (duration == 0f)
			// {
			// 	sightAgent.StopDestroy();
			// 	return;
			// }
			// int attachSightId = sightAgent.AttachSightId;
			// sightAgent.DelayDestroy(duration, delegate
			// {
			// 	CS$<>8__locals1.<>4__this.EnqueueCommand(new CmdRemoveSight
			// 	{
			// 		attachSightId = attachSightId,
			// 		targetId = CS$<>8__locals1.targetId
			// 	});
			// });
		}

		
		public ServerSightAgent AttachSightPosition(Vector3 position, float sightRange, float duration, bool isCheckOverlap, bool isRemoveWhenInvisibleStart)
		{
			if (isCheckOverlap)
			{
				ServerSightAgent serverSightAgent = this.SightAgent.SightAgentPositionCheckOverlap(position, sightRange, duration);
				if (serverSightAgent != null)
				{
					(serverSightAgent.Target as WorldSightObject).DelayDestroySelf(duration);
					return serverSightAgent;
				}
			}
			return this.CreateSightPosition(position, sightRange, duration, isRemoveWhenInvisibleStart);
		}

		
		public ServerSightAgent CreateSightPosition(Vector3 position, float sightRange, float duration, bool isRemoveWhenInvisibleStart)
		{
			return MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSightObject(this, position, sightRange, duration, isRemoveWhenInvisibleStart).SightAgent;
		}

		
		public ServerSightAgent AttachSight(WorldObject target, float sightRange, float duration, bool isCheckOverlap, bool isRemoveWhenInvisibleStart)
		{
			ServerSightAgent serverSightAgent = null;
			if (isCheckOverlap)
			{
				serverSightAgent = this.SightAgentCheckOverlap(target, sightRange, duration);
			}
			if (serverSightAgent == null)
			{
				serverSightAgent = target.gameObject.AddComponent<ServerSightAgent>();
				serverSightAgent.InitAttachSight(target, MonoBehaviourInstance<GameService>.inst.World.GetSightId());
				serverSightAgent.SetOwner(this.SightAgent);
				serverSightAgent.SetDetect(true, false);
				serverSightAgent.UpdateSightRange(sightRange);
				serverSightAgent.UpdateSightAngle(360);
				serverSightAgent.SetIsRemoveWhenInvisibleStart(isRemoveWhenInvisibleStart);
				base.EnqueueCommand(new CmdAddSight
				{
					attachSightId = serverSightAgent.AttachSightId,
					targetId = target.ObjectId,
					sightRange = new BlisFixedPoint(sightRange)
				});
			}
			this.SetRemoveSight(serverSightAgent, duration, target.ObjectId);
			return serverSightAgent;
		}

		
		public void RemoveSight(ServerSightAgent removeSightAgent, int targetId)
		{
			if (removeSightAgent == null)
			{
				return;
			}
			base.EnqueueCommand(new CmdRemoveSight
			{
				attachSightId = removeSightAgent.AttachSightId,
				targetId = targetId
			});
			removeSightAgent.Destroy();
		}

		
		public void ResetSightDestroyTime(ServerSightAgent resetSightAgent, float duration)
		{
			if (resetSightAgent == null)
			{
				return;
			}
			resetSightAgent.DelayDestroy(duration, delegate
			{
				this.EnqueueCommand(new CmdRemoveSight
				{
					attachSightId = resetSightAgent.AttachSightId,
					targetId = resetSightAgent.ObjectId
				});
			});
		}

		
		public void AddState(CharacterState state, int casterId)
		{
			if (state.Code != 10004)
			{
				if (!this.isAlive)
				{
					return;
				}
				if (this.IsUntargetable() && state.StateData.GroupData.effectType.Equals(EffectType.Debuff))
				{
					return;
				}
			}
			if (state.Caster == null || state.Self == null)
			{
				return;
			}
			this.StateEffector.Add(state, casterId);
		}

		
		public void OverwriteState(int stateCode, int casterId)
		{
			if (!this.isAlive)
			{
				return;
			}
			this.StateEffector.Overwrite(stateCode, casterId);
		}

		
		public void RemoveStateByGroup(int stateGroup)
		{
			this.RemoveStateByGroup(stateGroup, 0);
		}

		
		public bool RemoveStateByGroup(int stateGroup, int casterId)
		{
			return (stateGroup == 10004 || this.isAlive) && this.StateEffector.RemoveByGroup(stateGroup, casterId);
		}

		
		public void RemoveAllStateByType(StateType stateType)
		{
			if (!this.isAlive)
			{
				return;
			}
			this.StateEffector.RemoveAllByType(stateType);
		}

		
		public void ModifyStateValue(int stateGroup, int casterId, float durationChangeAmount, int changeStackCount, bool isResetCreateedTime)
		{
			if (!this.isAlive)
			{
				return;
			}
			this.StateEffector.ModifyStateValue(stateGroup, casterId, durationChangeAmount, changeStackCount, isResetCreateedTime);
		}

		
		public void RemoveAllStackByGroup(int stateGroup, int casterId)
		{
			if (!this.isAlive)
			{
				return;
			}
			int stackByGroup = this.StateEffector.GetStackByGroup(stateGroup, casterId);
			if (0 < stackByGroup)
			{
				this.StateEffector.ModifyStateValue(stateGroup, casterId, 0f, -stackByGroup, false);
			}
		}

		
		public void Airborne(float airborneDuration, float airbornePower)
		{
			if (!this.isAlive)
			{
				return;
			}
			base.EnqueueCommand(new CmdAirborne
			{
				duration = new BlisFixedPoint(airborneDuration),
				power = new BlisFixedPoint(airbornePower)
			});
		}

		
		public void MemorizedObjectAdd<T>(List<T> pMemorizedCharacterList) where T : WorldCharacter
		{
			WorldPlayerCharacter worldPlayerCharacter = this as WorldPlayerCharacter;
			if (worldPlayerCharacter == null)
			{
				Log.E("MemorizedObjectAdd character is not player");
				return;
			}
			if (pMemorizedCharacterList.Count == 0)
			{
				return;
			}
			HashSet<int> hashSet = new HashSet<int>();
			foreach (T t in pMemorizedCharacterList)
			{
				t.SightAgent.AddMemorizerPlayer(base.ObjectId);
				if (this.SightAgent.AddMemorizedTarget(t.ObjectId))
				{
					hashSet.Add(t.ObjectId);
				}
			}
			if (!hashSet.Any<int>())
			{
				return;
			}
			worldPlayerCharacter.SendMemorizerUpdate(hashSet);
		}

		
		public bool IsUntargetable()
		{
			return this.stateEffector.IsHaveStateByType(StateType.Untargetable);
		}

		
		private void OnChangeHp(int hp)
		{
			Action<int> changeHpEvent = this.ChangeHpEvent;
			if (changeHpEvent != null)
			{
				changeHpEvent(hp);
			}
			this.stateEffector.OnChangeHp(hp);
		}

		
		private void OnChangeMaxHp(int maxHp)
		{
			Action<int> changeMaxHpEvent = this.ChangeMaxHpEvent;
			if (changeMaxHpEvent != null)
			{
				changeMaxHpEvent(maxHp);
			}
			this.stateEffector.OnChangeMaxHp(maxHp);
		}

		
		public void AddOwnProjectile(WorldProjectile addTarget)
		{
			this.ownProjectiles.Add(addTarget);
		}

		
		public void RemoveOwnProjectile(WorldProjectile removeTarget)
		{
			this.ownProjectiles.Remove(removeTarget);
		}

		
		public WorldProjectile GetOwnProjectile(Func<WorldProjectile, bool> condition)
		{
			for (int i = this.ownProjectiles.Count - 1; i >= 0; i--)
			{
				if (this.ownProjectiles[i] == null || !this.ownProjectiles[i].IsAlive)
				{
					this.ownProjectiles.RemoveAt(i);
				}
				else if (condition(this.ownProjectiles[i]))
				{
					return this.ownProjectiles[i];
				}
			}
			return null;
		}

		
		public List<WorldProjectile> GetOwnProjectiles(Func<WorldProjectile, bool> condition)
		{
			List<WorldProjectile> list = null;
			for (int i = this.ownProjectiles.Count - 1; i >= 0; i--)
			{
				if (this.ownProjectiles[i] == null || !this.ownProjectiles[i].IsAlive)
				{
					this.ownProjectiles.RemoveAt(i);
				}
				else if (condition(this.ownProjectiles[i]))
				{
					if (list == null)
					{
						list = new List<WorldProjectile>();
					}
					list.Add(this.ownProjectiles[i]);
				}
			}
			return list;
		}

		
		public void DoOwnProjectileAction(Func<WorldProjectile, bool> condition, Action<WorldProjectile> eachProjectileAction)
		{
			if (eachProjectileAction == null)
			{
				return;
			}
			for (int i = this.ownProjectiles.Count - 1; i >= 0; i--)
			{
				if (this.ownProjectiles[i] == null || !this.ownProjectiles[i].IsAlive)
				{
					this.ownProjectiles.RemoveAt(i);
				}
				else if (condition(this.ownProjectiles[i]))
				{
					eachProjectileAction(this.ownProjectiles[i]);
				}
			}
		}

		
		protected void InBush()
		{
			base.EnqueueCommand(new CmdInBush());
		}

		
		protected void OutBush()
		{
			base.EnqueueCommand(new CmdOutBush());
		}

		
		protected bool isAlive;

		
		private CharacterStat stat;

		
		private readonly CharacterStatus status = new CharacterStatus();

		
		protected StateEffector stateEffector;

		
		protected CharacterColliderAgent colliderAgent;

		
		protected ServerSightAgent sightAgent;

		
		protected SkillAgent mySkillAgent;

		
		private float actualCriticalChance;

		
		private CollisionObject3D collisionObject;

		
		private CommandPacket CmdUpdateStat;

		
		protected ItemBox corpseBox;

		
		public Action<int> ChangeHpEvent;

		
		public Action<int> ChangeMaxHpEvent;

		
		private List<WorldProjectile> ownProjectiles = new List<WorldProjectile>();

		
		protected readonly List<CharacterStatValue> broadcastUpdateStats = new List<CharacterStatValue>();

		
		protected HashSet<StatType> updateHashSet = new HashSet<StatType>();

		
		private readonly List<int> emptyAssistants = new List<int>();

		
		private const int cacheSize = 50;

		
		private Collider[] findEnemyColliders;

		
		private List<WorldCharacter> enemies;
	}
}
