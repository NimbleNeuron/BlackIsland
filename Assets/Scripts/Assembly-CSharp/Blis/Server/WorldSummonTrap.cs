using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.SummonTrap)]
	public class WorldSummonTrap : WorldSummonBase
	{
		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.SummonTrap;
		}

		
		
		public bool HasBursted
		{
			get
			{
				return this.hasBursted;
			}
		}

		
		public override void Init(SummonData summonData, WorldPlayerCharacter owner)
		{
			base.Init(summonData, owner);
			this.hasBursted = false;
			this.trapBurstAction = null;
			this.ropeBurstAction = null;
			this.ropeTrapDirection = Vector3.zero;
			this.ropeTrapLength = 0f;
			this.lifeLinkTarget = null;
		}

		
		private void UpdateTrapAttack()
		{
			if (base.SummonData.summonAttackType == SummonAttackType.None || this.trapBurstAction == null || base.CreatedTime + base.Stat.AttackSpeed >= MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime)
			{
				return;
			}
			Vector3 position = base.GetPosition();
			WorldCharacter worldCharacter = base.GetEnemiesWithinRange(position, base.Stat.AttackRange, (WorldCharacter wc) => wc.CanFindForAttack()).NearestOne(position);
			if (worldCharacter == null)
			{
				return;
			}
			this.hasBursted = true;
			this.Burst(worldCharacter, true, null);
			this.BurstLifeLink(true);
		}

		
		private void UpdateRopeAttack()
		{
			if (this.ropeBurstAction == null || this.ropeTrapLength <= 0f || this.ropeTrapDirection.Equals(Vector3.zero) || base.CreatedTime + base.Stat.AttackSpeed >= MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime)
			{
				return;
			}
			Vector3 position = base.GetPosition() + this.ropeTrapLength * 0.5f * this.ropeTrapDirection;
			float width = base.SummonData.attackRange * 2f;
			if (this.boxCollision == null)
			{
				this.boxCollision = new CollisionBox3D(position, width, this.ropeTrapLength, this.ropeTrapDirection);
			}
			else
			{
				this.boxCollision.UpdatePosition(position);
				this.boxCollision.UpdateWidth(width);
				this.boxCollision.UpdateDepth(this.ropeTrapLength);
				this.boxCollision.UpdateNormalized(this.ropeTrapDirection);
			}
			List<WorldCharacter> enemies = base.GetEnemies(this.boxCollision);
			if (enemies.Count == 0)
			{
				return;
			}
			WorldCharacter trapActivator = enemies.NearestOne(position);
			this.hasBursted = true;
			this.Burst(trapActivator, false, this.boxCollision);
			this.BurstLifeLink(false);
		}

		
		protected override int GetCharacterCode()
		{
			SummonData summonData = base.SummonData;
			if (summonData == null)
			{
				return 0;
			}
			return summonData.code;
		}

		
		protected override void OnFrameUpdate()
		{
			base.OnFrameUpdate();
			if (!base.IsAlive || this.hasBursted || base.SummonData == null)
			{
				return;
			}
			this.UpdateTrapAttack();
			this.UpdateRopeAttack();
		}

		
		public void Burst(WorldCharacter trapActivator, bool isTrapAction, CollisionObject3D checkCollider = null)
		{
			this.isAlive = false;
			base.EnqueueCommand(new CmdActiveTrap());
			base.StartCoroutine(CoroutineUtil.DelayedAction(base.SummonData.attackDelay, delegate()
			{
				List<WorldCharacter> list = new List<WorldCharacter>(1);
				if (this.SummonData.summonAttackType == SummonAttackType.Target)
				{
					if (trapActivator != null)
					{
						list.Add(trapActivator);
					}
				}
				else if (this.SummonData.summonAttackType == SummonAttackType.MultiTarget)
				{
					if (this.SummonData.rangeRadius != 0f)
					{
						list.AddRange(this.GetEnemyInBrustRange());
					}
				}
				else
				{
					if (this.SummonData.summonAttackType != SummonAttackType.StickMultiTarget)
					{
						throw new NotImplementedException("Not Implemented SummonAttackType");
					}
					if (this.SummonData.rangeRadius != 0f && trapActivator != null)
					{
						list.AddRange(this.GetEnemyInBrustRange());
					}
				}
				if (isTrapAction)
				{
					WorldSummonTrap.OnSummonBurstEvent onSummonBurstEvent = this.trapBurstAction;
					if (onSummonBurstEvent != null)
					{
						onSummonBurstEvent(list, this);
					}
				}
				else
				{
					WorldSummonTrap.OnSummonBurstEvent onSummonBurstEvent2 = this.ropeBurstAction;
					if (onSummonBurstEvent2 != null)
					{
						onSummonBurstEvent2(list, this);
					}
				}
				List<int> list2 = new List<int>();
				foreach (WorldCharacter worldCharacter in list)
				{
					list2.Add(worldCharacter.ObjectId);
					if (this.additionalStateEffectList != null)
					{
						foreach (int stateCode in this.additionalStateEffectList)
						{
							worldCharacter.AddState(new CommonState(stateCode, worldCharacter, this.Owner), this.Owner.ObjectId);
						}
					}
				}
				this.EnqueueCommand(new CmdBurstTrap
				{
					targets = list2
				});
				if (this.Owner != null)
				{
					MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(this.Owner, this.GetPosition(), null, NoiseType.TrapHit);
				}
				this.DestroyGameObject();
			}));
		}

		
		public bool HasLifeLink()
		{
			return this.lifeLinkTarget != null;
		}

		
		public void LifeLink(WorldSummonTrap lifeLinkTarget)
		{
			this.lifeLinkTarget = lifeLinkTarget;
		}

		
		private void BurstLifeLink(bool isTrapAction)
		{
			if (this.lifeLinkTarget != null && this.lifeLinkTarget.isAlive)
			{
				this.lifeLinkTarget.Burst(null, isTrapAction, null);
			}
		}

		
		public void InstallRopeTrap(WorldSummonBase target, SkillId skillId)
		{
			if (!this.isAlive || !target.IsAlive)
			{
				return;
			}
			this.ropeTrapLength = GameUtil.DistanceOnPlane(base.GetPosition(), target.GetPosition());
			this.ropeTrapDirection = GameUtil.DirectionOnPlane(base.GetPosition(), target.GetPosition());
			base.EnqueueCommand(new CmdInstallRopeTrap
			{
				targetId = target.ObjectId,
				skillId = skillId
			});
		}

		
		protected override void OnDying(DamageInfo damageInfo)
		{
			base.OnDying(damageInfo);
			WorldCharacter attacker = damageInfo.Attacker;
			this.DeadLifeLink((attacker != null) ? attacker.ObjectId : 0, null, damageInfo.DamageType);
		}

		
		private void DeadLifeLink(int finishingAttacker, List<int> assistants, DamageType damageType)
		{
			if (this.lifeLinkTarget != null && this.lifeLinkTarget.isAlive)
			{
				this.lifeLinkTarget.Dead(finishingAttacker, assistants, damageType);
			}
		}

		
		public void SetActionOnTrapBurst(WorldSummonTrap.OnSummonBurstEvent action)
		{
			this.trapBurstAction = action;
		}

		
		public void SetActionOnRopeBurst(WorldSummonTrap.OnSummonBurstEvent action)
		{
			this.ropeBurstAction = action;
		}

		
		public void SetAdditionalStateEffectList(List<int> value)
		{
			this.additionalStateEffectList = value;
		}

		
		public new void StartSkill(SkillId skillId)
		{
		}

		
		public void PlaySkillAction(SkillId skillId, int actionNo, int targetId)
		{
		}

		
		public void PlaySkillAction(SkillId skillId, int actionNo, List<int> targetIds)
		{
		}

		
		private void FinishSkill(SkillScript skillScript, bool toNextSequence, bool cancel)
		{
		}

		
		private List<WorldCharacter> GetEnemyInBrustRange()
		{
			this.collisionAgents.Clear();
			int i;
			for (i = Physics.OverlapSphereNonAlloc(base.GetPosition(), base.SummonData.rangeRadius, this.colliders, GameConstants.LayerMask.WORLD_OBJECT_LAYER); i >= this.colliders.Length; i = Physics.OverlapSphereNonAlloc(base.GetPosition(), base.SummonData.rangeRadius, this.colliders, GameConstants.LayerMask.WORLD_OBJECT_LAYER))
			{
				this.colliders = new Collider[i * 2];
			}
			for (int j = 0; j < i; j++)
			{
				WorldCharacter component = this.colliders[j].GetComponent<WorldCharacter>();
				if (!(component == null) && component.IsAlive && base.ObjectId != component.SkillAgent.ObjectId && base.GetHostileType(component) == HostileType.Enemy && !component.IsUntargetable())
				{
					this.collisionAgents.Add(component);
				}
			}
			return this.collisionAgents;
		}

		
		private bool hasBursted;

		
		private Vector3 ropeTrapDirection;

		
		private float ropeTrapLength;

		
		private WorldSummonTrap lifeLinkTarget;

		
		private readonly Collider[] findSummonColliders = new Collider[50];

		
		private WorldSummonTrap.OnSummonBurstEvent trapBurstAction;

		
		private WorldSummonTrap.OnSummonBurstEvent ropeBurstAction;

		
		private CollisionBox3D boxCollision;

		
		private List<int> additionalStateEffectList;

		
		private readonly List<WorldCharacter> collisionAgents = new List<WorldCharacter>();

		
		private Collider[] colliders = new Collider[50];

		
		public delegate void OnSummonBurstEvent(List<WorldCharacter> pTargets, WorldSummonBase pWorldSummonBase);
	}
}
