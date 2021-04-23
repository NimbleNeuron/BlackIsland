using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using Blis.Server.CharacterAction;
using UnityEngine;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.PlayerCharacter)]
	public class WorldPlayerCharacter : WorldMovableCharacter
	{
		private void Ref()
		{
			Reference.Use(revivalCastingTime);
		}
		
		
		
		public Mastery Mastery
		{
			get
			{
				return this.mastery;
			}
		}

		
		
		public PlayerSession PlayerSession
		{
			get
			{
				return this.playerSession;
			}
		}

		
		
		public Equipment Equipment
		{
			get
			{
				return this.equipment;
			}
		}

		
		
		public Inventory Inventory
		{
			get
			{
				return this.inventory;
			}
		}

		
		
		public int CharacterSettingCode
		{
			get
			{
				return this.characterSettingCode;
			}
		}

		
		
		public int Exp
		{
			get
			{
				return this.exp;
			}
		}

		
		
		public new int CharacterCode
		{
			get
			{
				return this.characterCode;
			}
		}

		
		
		
		public int SkinCode { get; private set; }

		
		
		public bool IsRest
		{
			get
			{
				return this.isRest;
			}
		}

		
		
		public float SurvivableTime
		{
			get
			{
				return this.survivableTime;
			}
		}

		
		
		public MMRContext MMRContext
		{
			get
			{
				return this.mmrContext;
			}
		}

		
		
		public float GunReloadTime
		{
			get
			{
				return this.gunReloadTime;
			}
		}

		
		
		public virtual PlayerType PlayerType
		{
			get
			{
				return PlayerType.UserPlayer;
			}
		}

		
		protected override int GetCharacterCode()
		{
			return this.characterCode;
		}

		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.PlayerCharacter;
		}

		
		protected override int GetTeamNumber()
		{
			return this.teamNumber;
		}

		
		protected override HostileAgent GetHostileAgent()
		{
			return this.hostileAgent;
		}

		
		public Item FindEquip(int itemCode)
		{
			return this.equipment.FindEquip(itemCode);
		}

		
		public Item FindEquipById(int id)
		{
			return this.equipment.FindEquipById(id);
		}

		
		public bool InventoryHasSpace(Item item)
		{
			return this.inventory.CanAddItem(item);
		}

		
		
		public override bool IsDyingCondition
		{
			get
			{
				return this.isDyingCondition;
			}
		}

		
		
		public bool IsRevivaling
		{
			get
			{
				return this.isRevivaling;
			}
		}

		
		
		public int RevivalCount
		{
			get
			{
				return this.revivalCount;
			}
		}

		
		
		public bool IsObserving
		{
			get
			{
				return this.playerSession.IsObserving;
			}
		}

		
		public virtual void Init(int characterCode, int skinCode, int teamNumber, SpecialSkillId specialSkillId)
		{
			GameUtil.BindOrAdd<MovableCharacterController>(base.gameObject, ref this.controller);
			this.characterCode = characterCode;
			this.SkinCode = skinCode;
			this.teamNumber = teamNumber;
			this.equipment = new Equipment();
			this.inventory = new Inventory();
			this.visitedAreaMaskCodeFlag = 0;
			this.mastery = new Mastery(this);
			this.mastery.OnMasteryLevelUp += this.OnMasteryLevelUp;
			this.survivableTime = 30f;
			CharacterExpData expData = GameDB.character.GetExpData(1);
			this.nextLevelExp = ((expData != null) ? expData.levelUpExp : 0);
			this.warpMove = false;
			CharacterStat characterStat = new CharacterStat();
			characterStat.UpdateCharacterStat(GameDB.character.GetCharacterData(characterCode), 1);
			characterStat.UpdateEquipmentStat(this.equipment.GetEquipStats());
			characterStat.UpdateMasteryStat(this.mastery.GetMasteryStats(this.GetEquipWeaponMasteryType()));
			base.Status.SetExtraPoint(characterStat.InitExtraPoint);
			base.InitCharacterSkill(characterCode, specialSkillId);
			base.Init(characterStat);
			this.hostileAgent = new PlayerHostileAgent(this);
			this.mySkillAgent = new WorldPlayerCharacterSkillAgent(this);
			this.pingTarget = base.gameObject.AddComponent<PingTarget>();
		}

		
		protected override void OnPreFrameUpdate()
		{
			base.OnPreFrameUpdate();
			MatchingMode matchingMode = MonoBehaviourInstance<GameService>.inst.MatchingMode;
			if (!matchingMode.IsStandaloneMode() && !matchingMode.IsTutorialMode())
			{
				this.UpdateInSightCharacters();
			}
			this.UpdateVisibleCharacters();
		}

		
		protected override void OnFrameUpdate()
		{
			base.OnFrameUpdate();
			List<MasteryValue> list = this.mastery.FlushUpdates();
			if (list.Count > 0)
			{
				this.EnqueueRpcCommand(new CmdUpdateMastery
				{
					updates = list
				});
			}
			if (this.listCooldowns.Count > 0)
			{
				for (int i = 0; i < this.listCooldowns.Count; i++)
				{
					if (this.listCooldowns[i].WeaponItem == null)
					{
						i--;
					}
					else
					{
						this.listCooldowns[i].Update();
					}
				}
			}
		}

		
		private bool IsAroundProjectile(WorldObject worldObject)
		{
			WorldProjectile worldProjectile = worldObject as WorldProjectile;
			if (worldProjectile == null)
			{
				return false;
			}
			ProjectileProperty property = worldProjectile.Property;
			return ((property != null) ? property.ProjectileData : null) != null && worldProjectile.Property.ProjectileData.type == ProjectileType.Around;
		}

		
		public void AddAlwaysInsightObjectId(int pObjectId)
		{
			this.alwaysInsightObjectId.Add(pObjectId);
		}

		
		public void RemoveAlwaysInSightObjectId(int pObjectId)
		{
			this.alwaysInsightObjectId.Remove(pObjectId);
		}

		
		private float GetSightRangeByType(Type type)
		{
			float targetTypeInSightRange = base.SightAgent.GetTargetTypeInSightRange(type);
			return (targetTypeInSightRange < 11f) ? 12f : (targetTypeInSightRange + 1f);
		}

		
		private void UpdateInSightCharacters()
		{
			if (this.IsAI)
			{
				return;
			}
			HashSet<int> hashSet = this.prevOutSights;
			this.prevOutSights = this.currOutSights;
			this.currOutSights = hashSet;
			this.currOutSights.Clear();
			this.currInSights.Clear();
			float sightRangeByType = this.GetSightRangeByType(typeof(WorldPlayerCharacter));
			this.UpdateInSightCharacters(sightRangeByType, MonoBehaviourInstance<GameService>.inst.Player.PlayerSessions);
			this.UpdateInSightCharacters<WorldPlayerCharacter>(sightRangeByType, MonoBehaviourInstance<GameService>.inst.Bot.Characters, null);
			this.UpdateInSightCharacters<WorldObject>(this.GetSightRangeByType(typeof(WorldSummonBase)), MonoBehaviourInstance<GameService>.inst.World.GetCachedObjects(typeof(WorldSummonBase)), null);
			this.UpdateInSightCharacters<WorldObject>(this.GetSightRangeByType(typeof(WorldMonster)), MonoBehaviourInstance<GameService>.inst.World.GetCachedObjects(typeof(WorldMonster)), null);
			List<WorldObject> cachedObjects = MonoBehaviourInstance<GameService>.inst.World.GetCachedObjects(typeof(WorldProjectile));
			this.UpdateInSightCharacters<WorldObject>(this.GetSightRangeByType(typeof(WorldProjectile)), cachedObjects, new Func<WorldObject, bool>(this.IsAroundProjectile));
			for (int i = 0; i < this.currInSights.Count; i++)
			{
				WorldObject worldObject = this.currInSights[i];
				foreach (SightRangeLink sightRangeLink in worldObject.GetSightRangeLinks())
				{
					WorldObject linkedObject = sightRangeLink.GetLinkedObject(worldObject);
					if (this.currOutSights.Contains(linkedObject.ObjectId))
					{
						this.currInSights.Add(linkedObject);
						this.currOutSights.Remove(linkedObject.ObjectId);
					}
				}
			}
			foreach (WorldObject worldObject2 in this.currInSights)
			{
				int objectId = worldObject2.ObjectId;
				bool flag = this.prevOutSights.Contains(objectId);
				if (!this.checkedSights.Contains(objectId))
				{
					flag = true;
					this.checkedSights.Add(objectId);
				}
				if (flag)
				{
					bool flag2 = false;
					using (List<WorldObject>.Enumerator enumerator3 = cachedObjects.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							if (enumerator3.Current.ObjectId == worldObject2.ObjectId)
							{
								flag2 = true;
								break;
							}
						}
					}
					if (!flag2)
					{
						CmdInSight cmdInSight = new CmdInSight
						{
							objectId = objectId,
							position = new BlisVector(worldObject2.GetPosition())
						};
						IServerMoveAgentOwner serverMoveAgentOwner;
						if ((serverMoveAgentOwner = (worldObject2 as IServerMoveAgentOwner)) != null)
						{
							cmdInSight.moveAgentSnapshot = serverMoveAgentOwner.MoveAgent.CreateSnapshot();
						}
						this.EnqueueRpcCommand(new PacketWrapper(cmdInSight));
					}
				}
			}
			foreach (int num in this.currOutSights)
			{
				if (!this.prevOutSights.Contains(num))
				{
					bool flag3 = false;
					using (List<WorldObject>.Enumerator enumerator2 = cachedObjects.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.ObjectId == num)
							{
								flag3 = true;
								break;
							}
						}
					}
					if (!flag3)
					{
						CmdOutSight packet = new CmdOutSight
						{
							objectId = num
						};
						this.EnqueueRpcCommand(new PacketWrapper(packet));
					}
				}
			}
		}

		
		private void UpdateInSightCharacters(float extraSightRange, IEnumerable<PlayerSession> playerSessions)
		{
			foreach (PlayerSession playerSession in playerSessions)
			{
				this.UpdateInSightCharacter(extraSightRange, playerSession.Character);
			}
		}

		
		private void UpdateInSightCharacters<T>(float extraSightRange, List<T> cachedWorldObjects, Func<T, bool> condition) where T : WorldObject
		{
			foreach (T t in cachedWorldObjects)
			{
				if (condition == null || condition(t))
				{
					this.UpdateInSightCharacter(extraSightRange, t);
				}
			}
		}

		
		private void UpdateInSightCharacter(float extraSightRange, WorldObject worldObject)
		{
			bool flag;
			if (this.alwaysInsightObjectId.Contains(worldObject.ObjectId))
			{
				flag = true;
			}
			else
			{
				WorldCharacter worldCharacter = worldObject as WorldCharacter;
				if (worldCharacter == null)
				{
					flag = base.SightAgent.IsInAllySightWithoutWall(extraSightRange, null, worldObject.GetPosition(), 0.1f, false);
				}
				else
				{
					flag = base.SightAgent.IsInAllySightWithoutWall(extraSightRange, worldCharacter.SightAgent, worldCharacter.GetPosition(), worldCharacter.Stat.Radius, false);
					if (!flag && worldCharacter.ObjectId == base.ObjectId)
					{
						flag = true;
						Log.W("Self character not in sight: extraSightRange={0}, position={1}, radius={2}", new object[]
						{
							extraSightRange,
							worldCharacter.GetPosition(),
							worldCharacter.Stat.Radius
						});
					}
				}
			}
			if (flag)
			{
				this.currInSights.Add(worldObject);
				return;
			}
			this.currOutSights.Add(worldObject.ObjectId);
		}

		
		private void UpdateVisibleCharacters()
		{
			if (this.IsAI)
			{
				return;
			}
			HashSet<int> hashSet = this.prevInvisibleCharacters;
			this.prevInvisibleCharacters = this.currInvisibleCharacters;
			this.currInvisibleCharacters = hashSet;
			this.currInvisibleCharacters.Clear();
			this.UpdateVisibleCharacters(MonoBehaviourInstance<GameService>.inst.Player.PlayerSessions);
			this.UpdateVisibleCharacters<WorldPlayerCharacter>(MonoBehaviourInstance<GameService>.inst.Bot.Characters, null);
			this.UpdateVisibleCharacters<WorldObject>(MonoBehaviourInstance<GameService>.inst.World.GetCachedObjects(typeof(WorldSummonBase)), null);
			this.UpdateVisibleCharacters<WorldObject>(MonoBehaviourInstance<GameService>.inst.World.GetCachedObjects(typeof(WorldMonster)), null);
			foreach (int num in this.currInvisibleCharacters)
			{
				if (!this.prevInvisibleCharacters.Contains(num))
				{
					CmdOnInvisible packet = new CmdOnInvisible
					{
						objectId = num
					};
					this.EnqueueRpcCommand(new PacketWrapper(packet));
				}
			}
		}

		
		private void UpdateVisibleCharacters(IEnumerable<PlayerSession> playerSessions)
		{
			foreach (PlayerSession playerSession in playerSessions)
			{
				this.UpdateVisibleCharacter(playerSession.Character);
			}
		}

		
		private void UpdateVisibleCharacters<T>(List<T> cachedWorldObjects, Func<T, bool> condition) where T : WorldObject
		{
			foreach (T t in cachedWorldObjects)
			{
				if (condition == null || condition(t))
				{
					this.UpdateVisibleCharacter(t as WorldCharacter);
				}
			}
		}

		
		private void UpdateVisibleCharacter(WorldCharacter worldCharacter)
		{
			if (worldCharacter == null)
			{
				return;
			}
			bool flag = !this.currOutSights.Contains(worldCharacter.ObjectId) && base.SightAgent.IsInAllySight(worldCharacter.SightAgent, worldCharacter.GetPosition(), worldCharacter.Stat.Radius, worldCharacter.SightAgent.IsInvisibleCheckWithMemorizer(base.ObjectId));
			if (!flag && worldCharacter.ObjectId == base.ObjectId)
			{
				flag = true;
				Log.W("Self character not visible: position={0}, radius={1}, isInvisible={2}", new object[]
				{
					worldCharacter.GetPosition(),
					worldCharacter.Stat.Radius,
					worldCharacter.SightAgent.IsInvisibleCheckWithMemorizer(base.ObjectId)
				});
			}
			if (flag)
			{
				bool flag2 = this.prevInvisibleCharacters.Contains(worldCharacter.ObjectId);
				if (!flag2 && !this.checkedInvisibleCharacters.Contains(worldCharacter.ObjectId))
				{
					flag2 = true;
					this.checkedInvisibleCharacters.Add(worldCharacter.ObjectId);
				}
				if (flag2)
				{
					CmdOnVisible packet = new CmdOnVisible
					{
						objectId = worldCharacter.ObjectId
					};
					this.EnqueueRpcCommand(new PacketWrapper(packet));
				}
			}
			else
			{
				this.currInvisibleCharacters.Add(worldCharacter.ObjectId);
			}
		}

		
		protected override void StatFlushUpdates()
		{
			this.broadcastUpdateStats.Clear();
			this.rpcUpdateStats.Clear();
			List<CharacterStatValue> list = base.Stat.FlushUpdates();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].statType.IsRequiredByClient())
				{
					if (list[i].statType.IsBroadcastType())
					{
						this.broadcastUpdateStats.Add(list[i]);
					}
					else
					{
						this.rpcUpdateStats.Add(list[i]);
					}
				}
			}
			if (0 < this.broadcastUpdateStats.Count)
			{
				base.EnqueueCommand(new CmdBroadcastUpdateStat
				{
					updates = this.broadcastUpdateStats
				});
			}
			if (0 < this.rpcUpdateStats.Count)
			{
				this.EnqueueRpcCommand(new CmdUpdateStat
				{
					updates = this.rpcUpdateStats
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

		
		public override bool IsAggressive()
		{
			return true;
		}

		
		public void SetSession(PlayerSession playerSession)
		{
			this.playerSession = playerSession;
			this.mmrContext = new MMRContext(this, playerSession.teamMmr);
		}

		
		public override byte[] CreateSnapshot()
		{
			return WorldObject.serializer.Serialize<PlayerCharacterSnapshot>(new PlayerCharacterSnapshot
			{
				statusSnapshot = WorldObject.serializer.Serialize<PlayerStatusSnapshot>(new PlayerStatusSnapshot(base.Status)),
				initialStat = base.Stat.CreateSnapshot(),
				initialStateEffect = base.StateEffector.CreateSnapshot(),
				skillController = base.SkillController.CreateSnapshot(),
				moveAgentSnapshot = this.moveAgent.CreateSnapshot(),
				isInCombat = this.IsInCombat,
				isInvisible = base.IsInvisible,
				characterCode = this.characterCode,
				skinCode = this.SkinCode,
				masteryLevels = this.mastery.GetMasteryLevels(),
				whoKilledMe = base.CombatInvolvementResult.FinishingAttackerObjectId,
				teamNumber = this.playerSession.TeamNumber,
				isDyingCondition = this.isDyingCondition,
				mapMarks = this.pingTarget.MarkInfos,
				isRest = this.isRest,
				lockedSlotSetFlag = this.lockedSlotSetFlag
			});
		}

		
		public byte[] CreateMyPlayerSnapshot()
		{
			return WorldObject.serializer.Serialize<PlayerSnapshot>(new PlayerSnapshot
			{
				observing = this.IsObserving,
				disconnected = MonoBehaviourInstance<GameService>.inst.Server.IsDisconnected(this.playerSession),
				characterSkillSnapshot = base.CharacterSkill.CreateSnapshot(),
				skillPoint = this.skillPoint,
				masteryValues = this.mastery.GetMasteryValues(),
				visitedAreaMaskCodeFlag = this.visitedAreaMaskCodeFlag,
				rank = this.rank,
				ignoreTargets = MonoBehaviourInstance<GameService>.inst.IgnoreTargetService.GetIgnoreUsers(this.objectId)
			});
		}

		
		public byte[] CreateAllyPlayerSnapshot()
		{
			return WorldObject.serializer.Serialize<PlayerSnapshot>(new PlayerSnapshot
			{
				observing = this.IsObserving,
				disconnected = MonoBehaviourInstance<GameService>.inst.Server.IsDisconnected(this.playerSession),
				characterSkillSnapshot = base.CharacterSkill.CreateAllySnapshot(),
				rank = this.rank
			});
		}

		
		public byte[] CreatePlayerSnapshot()
		{
			return WorldObject.serializer.Serialize<PlayerSnapshot>(new PlayerSnapshot
			{
				observing = this.IsObserving,
				disconnected = MonoBehaviourInstance<GameService>.inst.Server.IsDisconnected(this.playerSession),
				characterSkillSnapshot = base.CharacterSkill.CreatePlayerSnapshot(),
				rank = this.rank
			});
		}

		
		private void EnqueueRpcCommand(PacketWrapper packetWrapper)
		{
			if (this.IsAI || this.playerSession == null)
			{
				return;
			}
			this.playerSession.EnqueueCommandPacket(packetWrapper);
		}

		
		private void EnqueueRpcCommand(CommandPacket commandPacket)
		{
			if (this.IsAI || this.playerSession == null)
			{
				return;
			}
			if (commandPacket is ObjectCommandPacket)
			{
				Log.E("EnqueueRpcCommand: {0}", commandPacket.ToString());
				throw new Exception();
			}
			this.playerSession.EnqueueCommandPacket(new PacketWrapper(commandPacket));
		}

		
		private void EnqueueRpcCommandWithObserver(CommandPacket commandPacket)
		{
			if (this.IsAI || this.playerSession == null)
			{
				return;
			}
			ObjectCommandPacket objectCommandPacket;
			if ((objectCommandPacket = (commandPacket as ObjectCommandPacket)) != null && objectCommandPacket.objectId == 0)
			{
				Log.E("EnqueueRpcCommandWithObserver: ObjectId does not exist. ({0})", commandPacket.ToString());
				throw new Exception();
			}
			MonoBehaviourInstance<GameServer>.inst.EnqueueRpcCommandWithObserver(this.playerSession, new PacketWrapper(commandPacket));
		}

		
		private void EnqueueCommandTeamOnly(CommandPacket commandPacket)
		{
			if (this.IsAI || this.playerSession == null)
			{
				return;
			}
			ObjectCommandPacket objectCommandPacket;
			if ((objectCommandPacket = (commandPacket as ObjectCommandPacket)) != null && objectCommandPacket.objectId == 0)
			{
				Log.E("EnqueueTeamMemberCommand: ObjectId does not exist. ({0})", commandPacket.ToString());
				throw new Exception();
			}
			this.playerSession.EnqueueCommandTeamOnly(new PacketWrapper(commandPacket));
		}

		
		private void EnqueueCommandTeamAndObserver(CommandPacket commandPacket)
		{
			if (this.IsAI || this.playerSession == null)
			{
				return;
			}
			ObjectCommandPacket objectCommandPacket;
			if ((objectCommandPacket = (commandPacket as ObjectCommandPacket)) != null && objectCommandPacket.objectId == 0)
			{
				Log.E("EnqueueTeamMemberCommand: ObjectId does not exist. ({0})", commandPacket.ToString());
				throw new Exception();
			}
			MonoBehaviourInstance<GameServer>.inst.EnqueueCommandTeamAndObserver(this.playerSession, new PacketWrapper(commandPacket));
		}

		
		public void AddExp(int addExp)
		{
			if (addExp == 0)
			{
				return;
			}
			this.exp += addExp;
			int num = base.Status.Level;
			while (this.nextLevelExp > 0 && this.exp >= this.nextLevelExp)
			{
				CharacterExpData expData = GameDB.character.GetExpData(num + 1);
				if (expData == null)
				{
					this.exp = 0;
					this.nextLevelExp = 0;
					break;
				}
				num++;
				this.exp = Mathf.Max(0, this.exp - this.nextLevelExp);
				this.nextLevelExp = expData.levelUpExp;
			}
			this.EnqueueRpcCommandWithObserver(new CmdUpdateExp
			{
				objectId = base.ObjectId,
				exp = this.exp
			});
			if (num > base.Status.Level)
			{
				int maxHp = base.Stat.MaxHp;
				int maxSp = base.Stat.MaxSp;
				base.Stat.UpdateCharacterStat(GameDB.character.GetCharacterData(this.characterCode), num);
				int hp = base.Stat.MaxHp - maxHp;
				int sp = base.Stat.MaxSp - maxSp;
				base.Status.SetLevel(num);
				base.EnqueueCommand(new CmdUpdateLevel
				{
					level = num
				});
				HealInfo healInfo = HealInfo.Create(hp, sp);
				healInfo.SetHealer(this);
				this.Heal(healInfo);
				this.AddSkillUpgradePoint();
			}
		}

		
		public void AddMasteryConditionExp(MasteryConditionType type, int masteryValue)
		{
			if (this.isDyingCondition)
			{
				return;
			}
			this.AddMasteryConditionExp(new UpdateMasteryInfo
			{
				conditionType = type,
				takeMasteryValue = masteryValue
			});
		}

		
		public void AddMasteryConditionExp(UpdateMasteryInfo updateMasteryInfo)
		{
			if (this.isDyingCondition)
			{
				return;
			}
			if (updateMasteryInfo != null)
			{
				this.mastery.AddMasteryConditionExp(updateMasteryInfo);
			}
		}

		
		private void OnMasteryLevelUp(MasteryType masteryType, int characterExp)
		{
			base.Stat.UpdateMasteryStat(this.mastery.GetMasteryStats(this.GetEquipWeaponMasteryType()));
			if (masteryType == MasteryType.Craft)
			{
				base.Stat.UpdateEquipmentStat(this.equipment.GetEquipStats());
			}
			this.AddExp(characterExp);
			base.EnqueueCommand(new CmdMasteryLevelUp
			{
				masteryType = masteryType,
				level = this.mastery.GetMasteryLevel(masteryType)
			});
			MonoBehaviourInstance<GameService>.inst.PlayerCharacter.UpdateHighestPlayerLevel(this);
		}

		
		public MasteryInfo GetMasteryInfo(MasteryType masteryType)
		{
			return this.mastery.GetMasteryInfo(masteryType);
		}

		
		public bool CheckFirstVisit(int areaMask)
		{
			SingletonMonoBehaviour<BattleEventCollector>.inst.OnCurrentAreaCheck(this, areaMask);
			if (areaMask == (this.visitedAreaMaskCodeFlag & areaMask))
			{
				return false;
			}
			this.visitedAreaMaskCodeFlag |= areaMask;
			MonoBehaviourInstance<GameServer>.inst.Send(this.playerSession, new RpcVisitedNewArea
			{
				VisitedNewAreaMask = areaMask
			}, NetChannel.ReliableOrdered);
			SingletonMonoBehaviour<BattleEventCollector>.inst.OnAfterVisitedAreaAdd(this, areaMask);
			return true;
		}

		
		public void SetCharacterSettingCode(int code)
		{
			this.characterSettingCode = code;
		}

		
		public void Rest(bool rest, bool ignoreCrowdControl)
		{
			if (this.isInCombat && rest)
			{
				return;
			}
			if (!this.isAlive || this.isDyingCondition || this.isRest == rest || (!ignoreCrowdControl && !this.stateEffector.CanCharacterControl()))
			{
				return;
			}
			if (rest && MonoBehaviourInstance<GameService>.inst.Area.IsLastSafeZoneActive() && MonoBehaviourInstance<GameService>.inst.Area.IsInFinalSafeZone(base.GetPosition()))
			{
				base.EnqueueCommand(new CmdRest
				{
					rest = false
				});
				return;
			}
			base.StopMove();
			this.isRest = rest;
			if (rest)
			{
				base.AddState(new RestState(10001, this, this), this.objectId);
			}
			else
			{
				base.RemoveAllStateByType(StateType.Rest);
			}
			base.EnqueueCommand(new CmdRest
			{
				rest = rest
			});
		}

		
		public bool EquipItem(Item item)
		{
			if (item.ItemData.itemType == ItemType.Weapon && !this.mastery.HasMastery(item.ItemData.GetMasteryType()))
			{
				return false;
			}
			Item item2;
			if (this.equipment.Equip(item, out item2))
			{
				int num = 0;
				if (item2 == null || this.inventory.AddItem(item2, out num))
				{
					if (num != 0)
					{
						throw new GameException(ErrorType.Internal);
					}
					this.EquipmentChanged(item, item2);
					this.UpdateMissionProgress(MissionConditionType.GET_ITEM, 1, item.itemCode);
					return true;
				}
			}
			return false;
		}

		
		public void EquipmentChanged(Item item, Item unequip)
		{
			base.Stat.UpdateEquipmentStat(this.equipment.GetEquipStats());
			base.Stat.UpdateMasteryStat(this.mastery.GetMasteryStats(this.GetEquipWeaponMasteryType()));
			if (unequip != null && unequip.id == this.reloadItemId)
			{
				this.CancelGunReload();
			}
			if (item != null && item.ItemData.IsGunType() && this.runningReload == null)
			{
				this.UpdateGunReloadTime(item.itemCode);
				this.GunReload(true);
			}
			if ((item != null && item.GetEquipSlotType() == EquipSlotType.Weapon) || (unequip != null && unequip.GetEquipSlotType() == EquipSlotType.Weapon))
			{
				WeaponType newWeapon = (item == null) ? WeaponType.None : item.WeaponTypeInfoData.type;
				WeaponType oldWeapon = (unequip == null) ? WeaponType.None : unequip.WeaponTypeInfoData.type;
				this.ApplyModeModifier(oldWeapon, newWeapon);
			}
			this.UnequipWeaponProcess(unequip);
			this.EquipWeaponProcess(item);
		}

		
		private void UnequipWeaponProcess(Item item)
		{
			if (item == null)
			{
				return;
			}
			if (!item.ItemData.GetMasteryType().IsWeaponMastery())
			{
				return;
			}
			if (item != null && item.ItemData.IsGunType() && this.runningReload == null)
			{
				base.Status.UpdateBullet(0);
			}
			SkillData weaponSkillData = this.GetWeaponSkillData(item);
			if (weaponSkillData != null)
			{
				base.CancelPassiveSkill(weaponSkillData.PassiveSkillId);
			}
			if (!base.CharacterSkill.IsFirstSequence(SkillSlotSet.WeaponSkill))
			{
				base.CharacterSkill.StopSequenceTimer(SkillSlotSet.WeaponSkill, item.ItemData.GetMasteryType());
			}
		}

		
		private void EquipWeaponProcess(Item item)
		{
			if (item == null)
			{
				return;
			}
			if (!item.ItemData.GetMasteryType().IsWeaponMastery())
			{
				return;
			}
			base.CastPassiveSkill(SkillSlotIndex.WeaponSkill);
			base.CharacterSkill.InitWeaponSkill(item.ItemData.GetMasteryType());
			if (item.ItemData.IsThrowType())
			{
				this.WeaponItemBulletCooldown(item);
			}
		}

		
		public void UpdateGunReloadTime(int itemCode)
		{
			this.gunReloadTime = GameDB.item.GetGunReloadTime(itemCode);
		}

		
		public void GunReload(bool playReloadAnimation)
		{
			this.GunReload(playReloadAnimation, this.gunReloadTime);
		}

		
		public void GunReload(bool playReloadAnimation, float reloadTime)
		{
			if (!this.isAlive)
			{
				return;
			}
			if (this.IsDyingCondition)
			{
				return;
			}
			if (base.SkillAgent.IsFullBullet())
			{
				return;
			}
			this.reloadItemId = this.GetWeapon().id;
			base.EnqueueCommand(new CmdStartGunReload
			{
				playReloadAnimation = playReloadAnimation
			});
			this.runningReload = base.StartCoroutine(CoroutineUtil.DelayedAction(reloadTime, delegate()
			{
				this.FinishGunReload(false);
			}));
			base.SetGunReloading(true);
		}

		
		private void FinishGunReload(bool cancelReload)
		{
			this.runningReload = null;
			base.SetGunReloading(false);
			if (!cancelReload)
			{
				base.Status.UpdateBullet(GameDB.item.GetBulletCapacity(this.GetWeapon().ItemData.code));
			}
			base.EnqueueCommand(new CmdFinishGunReload
			{
				cancelReload = cancelReload
			});
		}

		
		public override void CancelGunReload()
		{
			if (this.runningReload == null)
			{
				return;
			}
			base.StopCoroutine(this.runningReload);
			this.FinishGunReload(true);
		}

		
		public void ConsumeBullet()
		{
			Item weapon = this.GetWeapon();
			if (weapon.ItemData.IsGunType())
			{
				base.Status.ConsumeBullet();
				base.EnqueueCommand(new CmdConsumeBullet());
				return;
			}
			if (weapon.ItemData.IsThrowType())
			{
				weapon.SubBullet(1);
				this.WeaponItemBulletCooldown(weapon);
				this.SendEquipmentUpdate();
			}
		}

		
		public bool UnequipItem(Item item)
		{
			if (this.equipment.Unequip(item))
			{
				this.EquipmentChanged(null, item);
				return true;
			}
			return false;
		}

		
		public bool IsEquippedWeapon()
		{
			return this.equipment.GetWeapon() != null;
		}

		
		public bool HasEquipment(int itemId)
		{
			return this.equipment.FindEquipById(itemId) != null;
		}

		
		public Item GetWeapon()
		{
			return this.equipment.GetWeapon();
		}

		
		public Item GetArmor(ArmorType armorType)
		{
			return this.equipment.GetArmor(armorType);
		}

		
		public MasteryType GetEquipWeaponMasteryType()
		{
			return this.GetWeaponMasteryType(this.GetWeapon());
		}

		
		private MasteryType GetWeaponMasteryType(Item weapon)
		{
			if (weapon == null)
			{
				return MasteryType.None;
			}
			return weapon.ItemData.GetMasteryType();
		}

		
		public override void Interact(WorldObject target)
		{
			base.Interact(target);
			if (target != null)
			{
				this.EnqueueRpcCommand(new CmdInteract
				{
					targetId = target.ObjectId
				});
			}
		}

		
		public void EmotionCharacterVoice(int emotionCharObjectId, int emotionCharVoiceType)
		{
			if (!base.IsAlive)
			{
				return;
			}
			base.EnqueueCommand(new CmdEmotionCharacterVoice
			{
				characterObjectId = emotionCharObjectId,
				charVoiceType = emotionCharVoiceType
			});
		}

		
		public void AddInteractedObject(int objectId)
		{
			this.interactedObjectIds.Add(objectId);
		}

		
		public bool IsInteractedObject(WorldObject obj)
		{
			return this.interactedObjectIds.Contains(obj.ObjectId);
		}

		
		protected override void ConsumeSkillResources(SkillData skillData)
		{
			base.ConsumeSkillResources(skillData);
			this.ConsumeSkillResources(skillData.CostType, skillData.CostKey, skillData.cost);
			this.ConsumeSkillResources(skillData.ExCostType, skillData.ExCostKey, skillData.exCost);
		}

		
		public void ConsumeSkillResources(SkillCostType costType, int costKey, int cost)
		{
			if (cost <= 0)
			{
				return;
			}
			switch (costType)
			{
			case SkillCostType.NoCost:
			case SkillCostType.EquipWeaponStack:
				break;
			case SkillCostType.Sp:
				base.Status.SubSp(cost);
				this.AddMasteryConditionExp(MasteryConditionType.ConsumeSp, cost);
				base.EnqueueCommand(new CmdConsumeSkillCost
				{
					costType = SkillCostType.Sp,
					consumeValue = cost
				});
				return;
			case SkillCostType.StateStack:
				base.ModifyStateValue(costKey, 0, 0f, -cost, false);
				return;
			case SkillCostType.Hp:
			{
				int num = base.Status.SubHpCompare(cost, 1);
				this.AddMasteryConditionExp(MasteryConditionType.ConsumeHp, Mathf.Max(num, 0));
				base.EnqueueCommand(new CmdConsumeSkillCost
				{
					costType = SkillCostType.Hp,
					consumeValue = num
				});
				return;
			}
			case SkillCostType.Ep:
				base.Status.SubEp(cost);
				base.EnqueueCommand(new CmdConsumeSkillCost
				{
					costType = SkillCostType.Ep,
					consumeValue = cost
				});
				break;
			default:
				return;
			}
		}

		
		public List<BulletItem> CreateBulletItemSnapshot()
		{
			List<BulletItem> list = new List<BulletItem>();
			foreach (BulletCooldown bulletCooldown in this.listCooldowns)
			{
				BulletItem item = new BulletItem(bulletCooldown.WeaponItem, bulletCooldown.RemainCooldown);
				list.Add(item);
			}
			return list;
		}

		
		public void WeaponItemBulletCooldown(Item weaponItem)
		{
			if (weaponItem.IsFullBullet())
			{
				return;
			}
			BulletCooldown bulletCooldown = this.listCooldowns.Find((BulletCooldown x) => x.WeaponItem.id == weaponItem.id);
			if (bulletCooldown != null)
			{
				bulletCooldown.MergeItem(weaponItem);
				return;
			}
			BulletCooldown bulletCooldown2 = new BulletCooldown(this, weaponItem);
			bulletCooldown2.addBullet += this.AddBullet;
			bulletCooldown2.stopCooldown += this.FinishBulletCooldown;
			bulletCooldown2.StartBulletCooldown();
			this.listCooldowns.Add(bulletCooldown2);
		}

		
		private void AddBullet(int itemId, ItemMadeType madeType, int addBullet)
		{
			Item mergeItem = this.inventory.FindItemById(itemId, madeType);
			if (mergeItem == null)
			{
				mergeItem = this.equipment.GetWeapon();
			}
			mergeItem.AddBullet(addBullet);
			this.listCooldowns.Find((BulletCooldown x) => x.WeaponItem.id == mergeItem.id).MergeItem(mergeItem);
		}

		
		public void FinishBulletCooldown(int itemId)
		{
			if (this.listCooldowns.Count == 0)
			{
				return;
			}
			BulletCooldown bulletCooldown = this.listCooldowns.Find((BulletCooldown x) => x.WeaponItem.id == itemId);
			if (bulletCooldown != null)
			{
				bulletCooldown.FinishBulletCooldown();
				this.listCooldowns.Remove(bulletCooldown);
			}
		}

		
		public Item GetItemInOrderByAmount(int targetItemCode)
		{
			List<Item> list = new List<Item>();
			List<Item> list2 = this.inventory.FindItems(targetItemCode);
			if (list2 != null && list2.Count > 0)
			{
				list.AddRange(list2);
			}
			Item item = this.equipment.FindEquip(targetItemCode);
			if (item != null)
			{
				list.Add(item);
			}
			if (list.Count > 0)
			{
				list.Sort((Item x, Item y) => x.Amount.CompareTo(y.Amount));
				return list.ElementAt(0);
			}
			return null;
		}

		
		public bool AnyHaveSkillUpgradePoint()
		{
			return this.Internal_IsHaveSkillUpgradePoint() || this.Internal_IsHaveSkillUpgradePoint(this.GetEquipWeaponMasteryType());
		}

		
		public bool IsHaveSkillUpgradePoint(SkillSlotIndex skillSlotIndex)
		{
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				return this.Internal_IsHaveSkillUpgradePoint(this.GetEquipWeaponMasteryType());
			}
			return this.Internal_IsHaveSkillUpgradePoint();
		}

		
		private bool Internal_IsHaveSkillUpgradePoint()
		{
			return 0 < this.skillPoint;
		}

		
		private bool Internal_IsHaveSkillUpgradePoint(MasteryType masteryType)
		{
			return 0 < this.mastery.GetMasterySkillPoint(masteryType);
		}

		
		public void AddSkillUpgradePoint()
		{
			this.skillPoint++;
			this.EnqueueRpcCommand(new CmdUpdateSkillPoint
			{
				skillPoint = this.skillPoint
			});
		}

		
		public SkillData GetWeaponSkillData(Item item)
		{
			if (!item.ItemData.GetMasteryType().IsWeaponMastery())
			{
				return null;
			}
			return GameDB.skill.GetSkillData(item.ItemData.GetMasteryType(), base.CharacterSkill.GetSkillLevel(item.ItemData.GetMasteryType()), base.GetSkillSequence(SkillSlotSet.WeaponSkill));
		}

		
		protected override void FinishSkill(SkillSlotSet skillSlotSet, MasteryType masteryType, SkillId skillId, bool toNextSequence, bool cancel, bool startCooldown)
		{
			if (toNextSequence)
			{
				base.CharacterSkill.NextSequence(skillSlotSet);
				SkillData skillData = this.GetSkillData(skillSlotSet, -1);
				float sequenceCooldown = base.GetSequenceCooldown(skillData, skillSlotSet);
				float sequenceWaitTime = base.GetSequenceWaitTime(skillData, sequenceCooldown);
				if (!base.CharacterSkill.IsFirstSequence(skillSlotSet) && 0f < sequenceWaitTime)
				{
					base.CharacterSkill.SetSequenceDuration(skillSlotSet, masteryType, sequenceWaitTime, sequenceCooldown, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
				}
				this.EnqueueRpcCommand(new CmdSetSkillSequence
				{
					skillSlotSet = skillSlotSet,
					masteryType = masteryType,
					sequence = base.CharacterSkill.GetSkillSequence(skillSlotSet),
					duration = new BlisFixedPoint(sequenceWaitTime),
					sequenceCooldown = new BlisFixedPoint(sequenceCooldown)
				});
				bool flag = false;
				if (startCooldown && base.CharacterSkill.GetSkillSequence(skillSlotSet) == 0)
				{
					this.StartSkillCooldown(skillSlotSet, masteryType, this.GetSkillData(skillSlotSet, 0).cooldown);
					flag = true;
				}
				if (!flag)
				{
					if (skillSlotSet == SkillSlotSet.WeaponSkill)
					{
						if (base.CharacterSkill.IsHoldCooldown(masteryType))
						{
							this.SetHoldSkillCooldown(skillSlotSet, masteryType, false);
						}
					}
					else if (base.CharacterSkill.IsHoldCooldown(skillSlotSet))
					{
						this.SetHoldSkillCooldown(skillSlotSet, masteryType, false);
					}
				}
			}
			else if (startCooldown)
			{
				SkillData skillData2 = this.GetSkillData(skillSlotSet, 0);
				this.StartSkillCooldown(skillSlotSet, masteryType, skillData2.cooldown);
				if (!skillData2.SequenceIncreaseType.Equals(SequenceIncreaseType.None))
				{
					this.EnqueueRpcCommand(new CmdResetSkillSequence
					{
						skillSlotSet = skillSlotSet
					});
				}
			}
			base.EnqueueCommand(new CmdFinishSkill
			{
				skillId = skillId,
				cancel = cancel,
				skillSlotSet = skillSlotSet
			});
		}

		
		public override SkillData GetSkillData(SkillSlotSet skillSlotSet, int sequence = -1)
		{
			if (skillSlotSet == SkillSlotSet.None)
			{
				return null;
			}
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				return GameDB.skill.GetSkillData(this.GetEquipWeaponMasteryType(), base.CharacterSkill.GetSkillLevel(this.GetEquipWeaponMasteryType()), (sequence >= 0) ? sequence : base.GetSkillSequence(skillSlotSet));
			}
			if (skillSlotSet.SlotSet2Index() == SkillSlotIndex.SpecialSkill)
			{
				return GameDB.skill.GetSkillData(base.CharacterSkill.SpecialSkillId, (sequence >= 0) ? sequence : base.GetSkillSequence(skillSlotSet));
			}
			return base.GetSkillData(skillSlotSet, sequence);
		}

		
		protected override int GetSkillLevel(SkillSlotIndex skillSlotIndex)
		{
			if (skillSlotIndex != SkillSlotIndex.WeaponSkill)
			{
				return base.GetSkillLevel(skillSlotIndex);
			}
			return base.CharacterSkill.GetSkillLevel(this.GetEquipWeaponMasteryType());
		}

		
		public override bool CheckCooldown(SkillSlotSet skillSlotSet)
		{
			if (skillSlotSet != SkillSlotSet.WeaponSkill)
			{
				return base.CharacterSkill.CheckCooldown(skillSlotSet, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
			}
			return base.CharacterSkill.CheckCooldown(this.GetEquipWeaponMasteryType(), MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
		}

		
		protected override bool IsHoldCooldown(SkillSlotSet skillSlotSet)
		{
			if (skillSlotSet != SkillSlotSet.WeaponSkill)
			{
				return base.CharacterSkill.IsHoldCooldown(skillSlotSet);
			}
			return base.CharacterSkill.IsHoldCooldown(this.GetEquipWeaponMasteryType());
		}

		
		protected override bool CheckSkillResource(SkillCostType costType, int costKey, int cost)
		{
			if (costType == SkillCostType.EquipWeaponStack)
			{
				return this.IsEquippedWeapon() && (!this.GetWeapon().WeaponConsumable || cost <= this.GetWeapon().Amount);
			}
			return base.CheckSkillResource(costType, costKey, cost);
		}

		
		public bool CanSkillUpgrade(SkillSlotIndex skillSlotIndex)
		{
			if (!this.IsHaveSkillUpgradePoint(skillSlotIndex))
			{
				return false;
			}
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				int level = (0 < this.GetSkillLevel(skillSlotIndex)) ? (this.GetSkillLevel(skillSlotIndex) + 1) : 1;
				if (GameDB.skill.GetSkillData(this.GetEquipWeaponMasteryType(), level, 0) != null)
				{
					return true;
				}
			}
			else
			{
				SkillSlotSet? skillSlotSet = base.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
				if (skillSlotSet == null)
				{
					return false;
				}
				List<SkillData> characterSkills = GameDB.skill.GetCharacterSkills(this.GetCharacterCode(), ObjectType.PlayerCharacter, skillSlotSet.Value, (0 < this.GetSkillLevel(skillSlotIndex)) ? (this.GetSkillLevel(skillSlotIndex) + 1) : 1);
				if (0 < characterSkills.Count && characterSkills[0].activeLevel <= base.Status.Level)
				{
					return true;
				}
			}
			return false;
		}

		
		public bool CanSkillEvolution(SkillSlotIndex skillSlotIndex)
		{
			SkillData skillData = base.GetSkillData(skillSlotIndex, -1);
			if (!skillData.Evolutionable)
			{
				return false;
			}
			SkillEvolutionData evolutionData = GameDB.skill.GetEvolutionData(skillData);
			return evolutionData != null && base.CharacterSkill.CanSkillEvolution(skillSlotIndex, evolutionData, this.GetEquipWeaponMasteryType());
		}

		
		public void UpdateSurvivableTime(float remainTime)
		{
			this.survivableTime = remainTime;
			base.EnqueueCommand(new CmdUpdateSurvivableTime
			{
				survivalTime = new BlisFixedPoint(this.survivableTime)
			});
		}

		
		protected override void OnUpdateStat(HashSet<StatType> updateStats)
		{
			base.OnUpdateStat(updateStats);
			if (updateStats.Contains(StatType.MaxExtraPoint))
			{
				base.Status.SetExtraPoint(Mathf.Min(base.Stat.MaxExtraPoint, base.Status.ExtraPoint));
			}
		}

		
		protected override void OnHealHp(int deltaHp)
		{
			base.OnHealHp(deltaHp);
			this.AddMasteryConditionExp(MasteryConditionType.HpRecover, deltaHp);
		}

		
		protected override void OnHealSp(int deltaSp)
		{
			base.OnHealSp(deltaSp);
			this.AddMasteryConditionExp(MasteryConditionType.SpRecover, deltaSp);
		}

		
		public override void Heal(HealInfo healInfo)
		{
			if (!this.isAlive)
			{
				return;
			}
			base.CombatInvolvementAgent.AddSupportEvent(healInfo);
			if (this.IsDyingCondition)
			{
				return;
			}
			base.Heal(healInfo);
		}

		
		public override void Damage(DamageInfo damageInfo)
		{
			if (!this.isAlive)
			{
				return;
			}
			base.Damage(damageInfo);
			if (damageInfo.DamageType == DamageType.Sp)
			{
				return;
			}
			this.AddMasteryConditionExp(MasteryConditionType.ConsumeHp, damageInfo.Damage);
			if (damageInfo.DamageType == DamageType.Normal)
			{
				MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(this, damageInfo.Attacker, NoiseType.BasicHit);
				if (damageInfo.DamageSubType == DamageSubType.Normal && damageInfo.Attacker != null && !damageInfo.Attacker.IsInvisible)
				{
					base.AttachSightPosition(damageInfo.Attacker.GetPosition(), 3f, 4f, true, true);
				}
			}
			if (damageInfo.DamageType != DamageType.DyingCondition)
			{
				this.CancelActionCasting(CastingCancelType.Damage);
			}
		}

		
		public override void SetInCombat(bool isCombat, WorldCharacter target)
		{
			bool flag = isCombat != this.isInCombat;
			base.SetInCombat(isCombat, target);
			if (flag)
			{
				this.CancelRest();
				base.EnqueueCommand(new CmdUpdateInCombat
				{
					isCombat = isCombat
				});
				base.UpdateMoveSpeed();
			}
			if (isCombat && target != null && GameUtil.DistanceOnPlane(target.GetPosition(), base.GetPosition()) <= 40f)
			{
				this.controller.SetInCombat(target);
			}
		}

		
		protected override void UpdateSkillSequenceTimer()
		{
			if (base.CharacterSkill.IsHaveSequenceTimer())
			{
				base.CharacterSkill.UpdateSequenceTimer(MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime, this.GetEquipWeaponMasteryType());
			}
		}

		
		protected override void UpdateSkillStackTimer()
		{
			base.CharacterSkill.UpdateStackSkillTimer(MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
		}

		
		public void StartMovingDistanceMeasurement()
		{
			this.lastPosition = base.GetPosition();
			base.StartCoroutine(this.UpdateMoveDistance());
			base.StartCoroutine(this.HpSpRegen());
		}

		
		private IEnumerator UpdateMoveDistance()
		{
			while (this.isAlive)
			{
				Vector3 position = base.GetPosition();
				if (this.warpMove)
				{
					this.warpMove = false;
				}
				else
				{
					float num = Vector3.Distance(this.lastPosition, position);
					this.moveDistance += num;
				}
				this.lastPosition = position;
				if (this.moveDistance > 1f)
				{
					this.AddMasteryConditionExp(new UpdateMasteryInfo
					{
						conditionType = MasteryConditionType.MoveRange,
						takeMasteryValue = (int)this.moveDistance
					});
					this.moveDistance -= (float)((int)this.moveDistance);
				}
				yield return this.waitSecondUpdateMoveDistance.Seconds(1f);
			}
		}

		
		private IEnumerator HpSpRegen()
		{
			while (base.IsAlive)
			{
				bool flag = base.Status.Hp < base.Stat.MaxHp;
				bool flag2 = base.Status.Sp < base.Stat.MaxSp;
				if (!flag && !flag2)
				{
					yield return this.waitSecondHpSpRegen.Seconds(0.5f);
				}
				else
				{
					this.regenRemainHp += base.Stat.HpRegen;
					this.regenRemainSp += base.Stat.SpRegen;
					int hp = flag ? Mathf.FloorToInt(this.regenRemainHp * (1f + base.Stat.IncreaseModeHealRatio)) : 0;
					int sp = flag2 ? Mathf.FloorToInt(this.regenRemainSp) : 0;
					HealInfo healInfo = HealInfo.Create(hp, sp);
					healInfo.SetHealer(this);
					healInfo.SetShowUI(false);
					healInfo.SetNeedApplyHealRatio(false);
					this.Heal(healInfo);
					this.regenRemainHp -= (float)((int)this.regenRemainHp);
					this.regenRemainSp -= (float)((int)this.regenRemainSp);
					yield return this.waitSecondHpSpRegen.Seconds(0.5f);
				}
			}
		}

		
		public override void OnKill(DamageInfo damageInfo, WorldCharacter deadCharacter, List<int> assistTeamMemberObjectIds)
		{
			if (!deadCharacter.IsTypeOf<WorldMovableCharacter>())
			{
				return;
			}
			int killMonsterGainExp = 0;
			deadCharacter.IfTypeOf<WorldMonster>(delegate(WorldMonster monster)
			{
				MonsterLevelUpStatData monsterLevelUpStatData = GameDB.monster.GetMonsterLevelUpStatData(monster.MonsterData.code);
				int num2 = monster.MonsterData.gainExp + monsterLevelUpStatData.gainExp * (monster.Status.Level - 1);
				this.UpdateMissionProgress(MissionConditionType.WILD_ANIMAL_HUNTING, 1, monster.MonsterData.code);
				killMonsterGainExp = Mathf.RoundToInt((float)num2 * (1f + 0.2f * (float)assistTeamMemberObjectIds.Count)) / (assistTeamMemberObjectIds.Count + 1);
			});
			SingletonMonoBehaviour<BattleEventCollector>.inst.OnKill(deadCharacter, damageInfo);
			bool flag = deadCharacter.ObjectType == ObjectType.PlayerCharacter || deadCharacter.ObjectType == ObjectType.BotPlayerCharacter;
			this.AddMasteryConditionExp(new UpdateMasteryInfo
			{
				conditionType = (flag ? MasteryConditionType.KillPlayer : MasteryConditionType.KillMonster),
				masteryType = this.GetEquipWeaponMasteryType(),
				takeMasteryValue = 1,
				extraValue = (flag ? deadCharacter.Status.Level : killMonsterGainExp),
				assistMemberCount = assistTeamMemberObjectIds.Count
			});
			if (flag)
			{
				base.Status.AddPlayerKillCount();
				if (!MonoBehaviourInstance<GameService>.inst.IsTeamMode)
				{
					this.UpdateSurvivableTime(Mathf.Min(this.survivableTime + 7f, 30f));
					float num = (10f + (float)deadCharacter.Status.Level) / 100f;
					int hp = Mathf.FloorToInt((float)(base.Stat.MaxHp - base.Status.Hp) * num);
					int sp = Mathf.FloorToInt((float)(base.Stat.MaxSp - base.Status.Sp) * num);
					HealInfo healInfo = HealInfo.Create(hp, sp);
					healInfo.SetNeedApplyHealRatio(false);
					this.Heal(healInfo);
				}
			}
			else
			{
				base.Status.AddMonsterKillCount();
			}
			if (deadCharacter.ObjectType == ObjectType.PlayerCharacter || deadCharacter.ObjectType == ObjectType.BotPlayerCharacter)
			{
				this.SendEmotionIcon(EmotionPlateType.FINISH);
			}
			base.OnKill(damageInfo, deadCharacter, assistTeamMemberObjectIds);
		}

		
		public void OnKillAssist(WorldCharacter deadCharacter, int assistMemberCount)
		{
			if (!deadCharacter.IsTypeOf<WorldMovableCharacter>())
			{
				return;
			}
			int killMonsterGainExp = 0;
			deadCharacter.IfTypeOf<WorldMonster>(delegate(WorldMonster monster)
			{
				MonsterLevelUpStatData monsterLevelUpStatData = GameDB.monster.GetMonsterLevelUpStatData(monster.MonsterData.code);
				int num = monster.MonsterData.gainExp + monsterLevelUpStatData.gainExp * (monster.Status.Level - 1);
				killMonsterGainExp = Mathf.RoundToInt((float)num * (1f + 0.2f * (float)assistMemberCount)) / (assistMemberCount + 1);
			});
			bool flag = deadCharacter.ObjectType == ObjectType.PlayerCharacter || deadCharacter.ObjectType == ObjectType.BotPlayerCharacter;
			this.AddMasteryConditionExp(new UpdateMasteryInfo
			{
				conditionType = (flag ? MasteryConditionType.AssistKillPlayer : MasteryConditionType.KillMonster),
				masteryType = this.GetEquipWeaponMasteryType(),
				takeMasteryValue = 1,
				extraValue = (flag ? deadCharacter.Status.Level : killMonsterGainExp),
				assistMemberCount = assistMemberCount
			});
			if (flag)
			{
				base.Status.AddPlayerKillAssistCount();
			}
		}

		
		private void OnDeadFinalSurvivor(CombatInvolvementAgent.CombatInvolvementResult result)
		{
			foreach (WorldPlayerCharacter worldPlayerCharacter in this.playerSession.GetTeamCharacters())
			{
				worldPlayerCharacter.Dead(result.FinishingAttackerObjectId, result.Assistants, result.FinishingAttackDamageType);
			}
		}

		
		protected override void OnDying(DamageInfo damageInfo)
		{
			bool flag;
			if (this.IsGetDyingCondition(damageInfo.DamageType, out flag))
			{
				base.CombatInvolvementAgent.SetDyingConditionEvent();
				this.OnDyingCondition();
				base.KillProcess(damageInfo);
				if (base.CombatInvolvementResult.FinishingAttackerObjectType != ObjectType.None)
				{
					AreaData currentAreaData = AreaUtil.GetCurrentAreaData(MonoBehaviourInstance<GameService>.inst.CurrentLevel, base.GetPosition(), 2147483640);
					base.EnqueueCommand(new CmdSystemChat
					{
						type = SystemChatType.BeginDyingCondition,
						areaCode = ((currentAreaData != null) ? currentAreaData.code : 0),
						senderObjectId = this.objectId,
						characterCode = this.characterCode,
						targetObjectId = base.CombatInvolvementResult.FinishingAttackerObjectId,
						targetCharacterCode = base.CombatInvolvementResult.FinishingAttackerDataCode,
						isNoticeColor = true,
						isMonster = (base.CombatInvolvementResult.FinishingAttackerObjectType == ObjectType.Monster)
					});
				}
			}
			else
			{
				if (flag)
				{
					MonoBehaviourInstance<GameService>.inst.Player.ToAnnihilateTeam(base.KillProcess(damageInfo), this);
					this.OnDeadFinalSurvivor(base.CombatInvolvementResult);
				}
				this.Dead(base.CombatInvolvementResult.FinishingAttackerObjectId, base.CombatInvolvementResult.Assistants, base.CombatInvolvementResult.FinishingAttackDamageType);
			}
		}

		
		protected override void Dead(int finishingAttacker, List<int> assistants, DamageType damageType)
		{
			if (!this.isAlive)
			{
				return;
			}
			this.isRest = false;
			this.SendEmotionIcon(EmotionPlateType.DEAD);
			base.Dead(finishingAttacker, assistants, damageType);
			this.EndPlayingSequenceSkills();
			if (this.IsRunningCastingAction())
			{
				this.CancelActionCasting(CastingCancelType.Action);
			}
			if (this.teamrevivalCastingCharacter != null)
			{
				this.teamrevivalCastingCharacter.CancelActionCasting(CastingCancelType.Action);
			}
			List<Item> items = this.inventory.GetItems();
			items.AddRange(this.equipment.GetEquips());
			this.corpseBox = new ItemBox(this.objectId, this.inventory.MaxCount);
			this.corpseBox.SetItems(items);
			if (MonoBehaviourInstance<GameService>.inst.IsTeamMode)
			{
				this.AddMasteryExpIncrementTeamMember();
			}
			long attackerUserId = 0L;
			string attackerNickName = string.Empty;
			WorldObject worldObject = null;
			bool flag = MonoBehaviourInstance<GameService>.inst.World.TryFind<WorldObject>(base.CombatInvolvementResult.FinishingAttackerObjectId, ref worldObject);
			ObjectType finishingAttackerObjectType = base.CombatInvolvementResult.FinishingAttackerObjectType;
			if (finishingAttackerObjectType != ObjectType.PlayerCharacter)
			{
				if (finishingAttackerObjectType == ObjectType.Monster)
				{
					MonsterData monsterData = GameDB.monster.GetMonsterData(base.CombatInvolvementResult.FinishingAttackerDataCode);
					if (monsterData != null)
					{
						attackerNickName = monsterData.monster.ToString();
					}
				}
			}
			else
			{
				attackerNickName = base.CombatInvolvementResult.FinishingAttackerName;
				if (!flag)
				{
					Log.V(string.Format("[EXCEPTION][Dead] Object Not found : ObjectId({0}), Name({1}), AttackerDataCode({2}), DamageType{3}, DamageSubType({4}), DamageCode({5})", new object[]
					{
						base.CombatInvolvementResult.FinishingAttackerObjectId,
						base.CombatInvolvementResult.FinishingAttackerName,
						base.CombatInvolvementResult.FinishingAttackerDataCode,
						base.CombatInvolvementResult.FinishingAttackDamageType,
						base.CombatInvolvementResult.FinishingAttackDamageSubType,
						base.CombatInvolvementResult.FinishingAttackDamageData
					}));
				}
				else
				{
					WorldPlayerCharacter worldPlayerCharacter = worldObject as WorldPlayerCharacter;
					if (worldPlayerCharacter != null)
					{
						attackerUserId = worldPlayerCharacter.playerSession.userId;
						attackerNickName = worldPlayerCharacter.playerSession.nickname;
					}
					else
					{
						Log.V(string.Format("[EXCEPTION][Dead] Casting Fail to WorldPlayerCharacter from WorldObject : ObjectId({0}), Name({1}), AttackerDataCode({2}), DamageType{3}, DamageSubType({4}), DamageCode({5})", new object[]
						{
							base.CombatInvolvementResult.FinishingAttackerObjectId,
							base.CombatInvolvementResult.FinishingAttackerName,
							base.CombatInvolvementResult.FinishingAttackerDataCode,
							base.CombatInvolvementResult.FinishingAttackDamageType,
							base.CombatInvolvementResult.FinishingAttackDamageSubType,
							base.CombatInvolvementResult.FinishingAttackDamageData
						}));
					}
				}
			}
			MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(this, worldObject, NoiseType.PlayerKilled);
			if (damageType != DamageType.RedZone)
			{
				base.EnqueueCommand(new CmdSystemChat
				{
					type = SystemChatType.PlayerDead,
					senderObjectId = this.playerSession.ObjectId,
					characterCode = this.characterCode,
					isNoticeColor = true
				});
			}
			if (assistants != null)
			{
				SingletonMonoBehaviour<BattleEventCollector>.inst.OnAfterAssist(this, finishingAttacker, assistants);
			}
			MonoBehaviourInstance<GameService>.inst.Player.OnPlayerDead(this, attackerUserId, base.CombatInvolvementResult.FinishingAttackerObjectType, base.CombatInvolvementResult.FinishingAttackerObjectId, base.CombatInvolvementResult.FinishingAttackerDataCode, attackerNickName, base.CombatInvolvementResult.FinishingAttackDamageType, base.CombatInvolvementResult.FinishingAttackDamageSubType, base.CombatInvolvementResult.FinishingAttackDamageData, assistants, null);
		}

		
		public void Surrender()
		{
			DirectDamageCalculator damageCalculator = new DirectDamageCalculator(WeaponType.None, DamageType.RedZone, DamageSubType.Normal, 0, 0, base.Stat.MaxHp * 2, 0, 1f);
			Singleton<DamageService>.inst.EnvironmentDamageTo(this, damageCalculator, null, 2000001);
			if (base.CombatInvolvementResult.FinishingAttackerObjectId == 0)
			{
				base.EnqueueCommand(new CmdSystemChat
				{
					type = SystemChatType.SurrenderGame,
					senderObjectId = this.playerSession.ObjectId,
					characterCode = this.characterCode,
					isNoticeColor = true
				});
			}
		}

		
		public void ExitTeamGame(bool openGameResult)
		{
			this.MMRContext.OnDeath(null);
			int num = MonoBehaviourInstance<GameService>.inst.IsTeamMode ? MonoBehaviourInstance<GameService>.inst.GetCurrentTeamRank(this.teamNumber) : MonoBehaviourInstance<GameService>.inst.GetCurrentRank();
			int num2 = MonoBehaviourInstance<GameService>.inst.CalcTeamRank();
			MonoBehaviourInstance<GameService>.inst.Player.ProcessBattleResult(this, num2, null);
			if (openGameResult)
			{
				PlayerService.DeadInfo deadPlayerInfo = new PlayerService.DeadInfo(this, 0L, ObjectType.None, 0, 0, "", DamageType.None, DamageSubType.Normal, 0, null);
				MonoBehaviourInstance<GameService>.inst.Player.SendFinishGame(this.playerSession, num, deadPlayerInfo);
				return;
			}
			MonoBehaviourInstance<GameServer>.inst.Send(this.playerSession, new RpcExitGame(), NetChannel.ReliableOrdered);
		}

		
		public void SendResultScoreBoardRanking(int rank)
		{
			base.EnqueueCommand(new CmdRanking
			{
				teamNumber = this.teamNumber,
				rank = rank
			});
		}

		
		public void SetRank(int rank)
		{
			this.rank = rank;
		}

		
		private void ExecuteCastingAction()
		{
			try
			{
				Action action = this.castingCallback;
				if (action != null)
				{
					action();
				}
			}
			catch (Exception e)
			{
				MonoBehaviourInstance<GameServer>.inst.HandleException(this.playerSession, e, null);
			}
			finally
			{
				this.runningCastingType = CastingActionType.None;
				this.runningCastingAction = null;
				this.castingCallback = null;
				this.castingCancelCallback = null;
				this.openCastingAIrSupplyObjectId = 0;
			}
		}

		
		public bool IsRunningCastingAction()
		{
			return this.runningCastingAction != null;
		}

		
		private bool IsMyCastingAction()
		{
			return this.runningCastingType.IsMyCastingActionType();
		}

		
		public bool CollectResource(WorldResourceItemBox resourceBox)
		{
			if (resourceBox.IsCollected)
			{
				return false;
			}
			if (resourceBox.IsCooldown)
			{
				return false;
			}
			CollectibleData collectibleData = MonoBehaviourInstance<GameService>.inst.CurrentLevel.GetCollectibleData(resourceBox.ResourceDataCode);
			ItemData itemData = GameDB.item.FindItemByCode(collectibleData.itemCode);
			if (itemData == null)
			{
				return false;
			}
			if (!this.inventory.CanAddItem(new Item(-1, itemData.code, itemData.initialCount, 0)))
			{
				return false;
			}
			int i;
			int num;
			int num2;
			for (i = collectibleData.dropCount; i > 0; i -= num - num2)
			{
				num = i;
				if (itemData.stackable < i)
				{
					num = itemData.stackable;
				}
				Item item = MonoBehaviourInstance<GameService>.inst.Spawn.CreateItem(itemData, num);
				if (!this.inventory.AddItem(item, out num2))
				{
					break;
				}
			}
			this.UpdateMissionProgress(MissionConditionType.GET_ITEM, collectibleData.dropCount - i, itemData.code);
			this.AddMasteryConditionExp(MasteryConditionType.CollectibleOpen, 1);
			if (collectibleData.cooldown == 0 || collectibleData.code == 7)
			{
				resourceBox.Collected();
				MonoBehaviourInstance<GameService>.inst.Spawn.DestroyWorldObject(resourceBox);
			}
			else
			{
				resourceBox.StartCooldown((float)collectibleData.cooldown);
			}
			return true;
		}

		
		public void CancelActionCasting(CastingCancelType castingCancelType)
		{
			if (this.runningCastingAction == null)
			{
				return;
			}
			if (!this.castingCancelCondition.IsCancel(castingCancelType))
			{
				return;
			}
			base.StopCoroutine(this.runningCastingAction);
			base.EnqueueCommand(new CmdCancelActionCasting
			{
				extraParam = this.openCastingAIrSupplyObjectId
			});
			try
			{
				Action action = this.castingCancelCallback;
				if (action != null)
				{
					action();
				}
			}
			catch (Exception ex)
			{
				Log.V(ex.ToString());
				MonoBehaviourInstance<GameServer>.inst.HandleException(this.playerSession, ex, null);
			}
			finally
			{
				this.runningCastingType = CastingActionType.None;
				this.runningCastingAction = null;
				this.castingCallback = null;
				this.castingCancelCallback = null;
				this.openCastingAIrSupplyObjectId = 0;
			}
		}

		
		public bool ForceAddInventoryItem(Item item)
		{
			try
			{
				int num;
				this.AddInventoryItem(item, out num);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		
		public void AddInventoryItem(Item item, out int remainCount)
		{
			int amount = item.Amount;
			if (this.inventory.AddItem(item, out remainCount))
			{
				this.UpdateMissionProgress(MissionConditionType.GET_ITEM, amount, item.itemCode);
				return;
			}
			if (amount != item.Amount)
			{
				this.UpdateMissionProgress(MissionConditionType.GET_ITEM, amount, item.itemCode);
				return;
			}
			throw new GameException(ErrorType.NotEnoughInventory);
		}

		
		public Item RemoveInventoryItem(int itemId, ItemMadeType madeType)
		{
			Item item = this.inventory.RemoveItemById(itemId, madeType);
			if (item == null)
			{
				throw new GameException(ErrorType.ItemNotFound);
			}
			return item;
		}

		
		public void SubInventoryItem(int itemId, ItemMadeType madeType)
		{
			this.inventory.SubItemById(itemId, madeType);
		}

		
		public void SwapInventoryItem(int srcItemId, int destItemId)
		{
			this.inventory.SwapItem(srcItemId, destItemId);
		}

		
		public void SendInventoryUpdate(UpdateInventoryType updateType)
		{
			List<InvenItem> list = this.inventory.FlushUpdates();
			if (list.Count > 0)
			{
				MonoBehaviourInstance<GameServer>.inst.Send(this.playerSession, new RpcUpdateInventory
				{
					updates = list,
					updateType = updateType
				}, NetChannel.ReliableOrdered);
			}
		}

		
		public void SendEquipmentUpdate()
		{
			List<EquipItem> list = this.equipment.FlushUpdates();
			if (list.Count > 0)
			{
				MonoBehaviourInstance<GameServer>.inst.Broadcast(new RpcUpdateEquipment
				{
					objectId = this.objectId,
					updates = list
				}, NetChannel.ReliableOrdered);
			}
		}

		
		public void CompleteMakeItem()
		{
			MonoBehaviourInstance<GameServer>.inst.Send(this.playerSession, new RpcCompleteMakeItem(), NetChannel.ReliableOrdered);
		}

		
		public void FlushAllUpdatesBeforeStart()
		{
			List<CharacterStatValue> list = base.Stat.FlushUpdates();
			if (list.Count > 0)
			{
				MonoBehaviourInstance<GameServer>.inst.Broadcast(new RpcBroadCastingUpdateBeforeStart
				{
					objectId = base.ObjectId,
					equips = this.equipment.FlushUpdates(),
					stats = list,
					statusSnapshot = WorldObject.serializer.Serialize<PlayerStatusSnapshot>(new PlayerStatusSnapshot(base.Status)),
					position = new BlisVector(base.GetPosition()),
					walkableNavMask = this.WalkableNavMask
				}, NetChannel.ReliableOrdered);
				MonoBehaviourInstance<GameServer>.inst.Send(this.playerSession, new RpcUpdateBeforeStart
				{
					skillPoint = this.skillPoint,
					inventoryItems = this.inventory.FlushUpdates(),
					characterSkillLevels = base.CharacterSkill.GetCharacterSkillLevels(),
					masteryValues = this.mastery.GetMasteryValues()
				}, NetChannel.ReliableOrdered);
				HashSet<StatType> hashSet = new HashSet<StatType>();
				foreach (CharacterStatValue characterStatValue in list)
				{
					hashSet.Add(characterStatValue.statType);
				}
				this.OnUpdateStat(hashSet);
			}
		}

		
		public int GetInventoryInsertableCount(ItemData itemData)
		{
			return this.inventory.GetInventoryInsertableCount(itemData);
		}

		
		public void SetCastingCallback(Action callback)
		{
			this.castingCallback = callback;
		}

		
		public void StartActionCasting(CastingActionType type, bool isConsumeCost, Action startCallback, Action completeCallback, Action cancelCallback = null, int extraParam = 0)
		{
			if (this.IsRunningCastingAction())
			{
				return;
			}
			if (!this.isAlive)
			{
				return;
			}
			if (this.IsDyingCondition)
			{
				return;
			}
			if (base.SkillController.AnyPlayingSkill())
			{
				return;
			}
			if (!type.IgnoreCrowdControl() && !this.stateEffector.CanAction(type))
			{
				return;
			}
			ActionCostData costData = GameDB.character.GetActionCost(type);
			if (costData == null)
			{
				throw new GameException(string.Format("Invalid Action Type : {0}", type));
			}
			int consumeCost = isConsumeCost ? costData.sp : 0;
			if (base.Status.Sp < consumeCost)
			{
				throw new GameException(ErrorType.NotEnoughSp);
			}
			SingletonMonoBehaviour<BattleEventCollector>.inst.OnBeforeActionCasting(this, costData);
			this.castingCancelCallback = cancelCallback;
			this.SetCastingCallback(delegate
			{
				Action completeCallback2 = completeCallback;
				if (completeCallback2 != null)
				{
					completeCallback2();
				}
				this.ConsumeSkillResources(SkillCostType.Sp, 0, consumeCost);
				SingletonMonoBehaviour<BattleEventCollector>.inst.OnAfterActionCasting(this, costData);
			});
			if (startCallback != null)
			{
				startCallback();
			}
			this.StopWhenCasting(type);
			ActionCostData actionCost = GameDB.character.GetActionCost(type);
			if (type == CastingActionType.AirSupplyOpen)
			{
				this.openCastingAIrSupplyObjectId = extraParam;
			}
			this.castingCancelCondition = actionCost.effectCancelCondition;
			float num = actionCost.time;
			if (type == CastingActionType.InstallTrap)
			{
				num -= base.SkillAgent.Stat.InstallTrapCastingTimeReduce;
				if (num < 0f)
				{
					num = 0f;
				}
			}
			if (num <= 0f)
			{
				this.ExecuteCastingAction();
				return;
			}
			this.runningCastingType = type;
			this.runningCastingAction = base.StartCoroutine(CoroutineUtil.DelayedAction(num, new Action(this.ExecuteCastingAction)));
			base.EnqueueCommand(new CmdStartActionCasting
			{
				type = type,
				castTime = new BlisFixedPoint(num),
				extraParam = extraParam
			});
		}

		
		private void StopWhenCasting(CastingActionType type)
		{
			if (type - CastingActionType.Resurrect > 1)
			{
				this.controller.Stop();
			}
		}

		
		public void UpgradeSkill(SkillSlotIndex skillSlotIndex)
		{
			int num;
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				num = this.mastery.GetMasterySkillPoint(this.GetEquipWeaponMasteryType()) - 1;
			}
			else
			{
				num = Math.Max(0, this.skillPoint - 1);
			}
			this.EnqueueCommandTeamAndObserver(new CmdUpgradeSkill
			{
				objectId = base.ObjectId,
				skillSlotIndex = skillSlotIndex,
				skillPoint = num
			});
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				MasteryType equipWeaponMasteryType = this.GetEquipWeaponMasteryType();
				base.CharacterSkill.UpgradeSkill(equipWeaponMasteryType);
				this.skillController.UpgradeWeaponSkill(equipWeaponMasteryType);
				this.mastery.UseWeaponSkillPoint(equipWeaponMasteryType);
				SingletonMonoBehaviour<BattleEventCollector>.inst.OnAfterSkillLevelUp(this, skillSlotIndex, base.CharacterSkill.GetSkillLevel(equipWeaponMasteryType));
			}
			else
			{
				base.CharacterSkill.UpgradeSkill(skillSlotIndex);
				this.skillController.UpgradeSkill(base.CharacterSkill.GetSkillSlotSet(skillSlotIndex), this.characterCode, this.GetObjectType());
				this.skillPoint = num;
				SingletonMonoBehaviour<BattleEventCollector>.inst.OnAfterSkillLevelUp(this, skillSlotIndex, base.CharacterSkill.GetSkillLevel(skillSlotIndex));
			}
			if (base.GetSkillData(skillSlotIndex, -1) != null)
			{
				this.SetAddSkillUpOrder(skillSlotIndex, base.GetSkillData(skillSlotIndex, -1).group);
			}
			if (this.GetSkillLevel(skillSlotIndex) == 1)
			{
				base.CastPassiveSkill(skillSlotIndex);
				return;
			}
			base.OverwritePassiveSkill(skillSlotIndex);
		}

		
		private void SetAddSkillUpOrder(SkillSlotIndex slotIndex, int skillGroup)
		{
			Dictionary<int, int> dictionary = this.skillUpOrderMap;
			int key = this.skillUpOrder + 1;
			this.skillUpOrder = key;
			dictionary.Add(key, skillGroup);
			string key2 = slotIndex.ToString();
			int num;
			if (this.skillIndexLevelMap.TryGetValue(key2, out num))
			{
				this.skillIndexLevelMap[key2] = num + 1;
				return;
			}
			this.skillIndexLevelMap.Add(key2, 1);
		}

		
		public void EvolutionSkill(SkillSlotIndex skillSlotIndex)
		{
			SkillEvolutionPointType pointType = SkillEvolutionPointType.CharacterSkill;
			int currentPoint = 0;
			if (!base.CharacterSkill.UseSkillEvolutionPoint(skillSlotIndex, ref pointType, ref currentPoint))
			{
				return;
			}
			if (skillSlotIndex == SkillSlotIndex.WeaponSkill)
			{
				base.CharacterSkill.EvolutionSkill(this.GetEquipWeaponMasteryType());
			}
			else
			{
				base.CharacterSkill.EvolutionSkill(skillSlotIndex);
			}
			this.EnqueueRpcCommand(new CmdEvolutionSkill
			{
				skillSlotIndex = skillSlotIndex,
				pointType = pointType,
				currentPoint = currentPoint
			});
			base.OverwritePassiveSkill(skillSlotIndex);
		}

		
		public void UpdateSkillEvolutionPoint(SkillEvolutionPointType pointType, int currentPoint)
		{
			this.EnqueueRpcCommand(new CmdUpdateSkillEvolutionPoint
			{
				pointType = pointType,
				currentPoint = currentPoint
			});
		}

		
		public override bool StartSkillCooldown(SkillSlotSet skillSlotSet, MasteryType masteryType, float cooldown)
		{
			SkillSlotIndex skillSlotIndex = skillSlotSet.SlotSet2Index();
			float reservationCooldown = base.CharacterSkill.GetReservationCooldown(skillSlotSet);
			bool flag;
			float num;
			if (skillSlotSet.SlotSet2Index() == SkillSlotIndex.WeaponSkill)
			{
				flag = base.CharacterSkill.StartCooldown(masteryType, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime, cooldown);
				num = 1f;
			}
			else if (skillSlotIndex == SkillSlotIndex.SpecialSkill)
			{
				flag = base.CharacterSkill.StartCooldown(skillSlotSet, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime, cooldown, 0f);
				num = 1f;
			}
			else
			{
				flag = base.StartSkillCooldown(skillSlotSet, masteryType, cooldown);
				num = GameUtil.GetCooldowncooldownReduction(base.Stat.CooldownReduction);
			}
			if (flag)
			{
				if (skillSlotSet.SlotSet2Index() != SkillSlotIndex.Attack)
				{
					this.EnqueueCommandTeamAndObserver(new CmdStartSkillCooldown
					{
						objectId = base.ObjectId,
						skillSlotSet = skillSlotSet,
						masteryType = masteryType,
						cooldown = new BlisFixedPoint(cooldown * num + reservationCooldown)
					});
				}
			}
			else
			{
				this.SetHoldSkillCooldown(skillSlotSet, masteryType, false);
			}
			return flag;
		}

		
		public override SkillSlotSet ModifyCooldown(SkillSlotSet SkillSlotSetFlag, float modifyValue)
		{
			SkillSlotSetFlag = base.ModifyCooldown(SkillSlotSetFlag, modifyValue);
			if (SkillSlotSetFlag != SkillSlotSet.None)
			{
				this.EnqueueCommandTeamAndObserver(new CmdModifySkillCooldown
				{
					objectId = base.ObjectId,
					skillSlotSetFlag = SkillSlotSetFlag,
					time = new BlisFixedPoint(modifyValue)
				});
			}
			return SkillSlotSetFlag;
		}

		
		public override void SetHoldSkillCooldown(SkillSlotSet skillSlotSet, MasteryType masteryType, bool isHold)
		{
			base.SetHoldSkillCooldown(skillSlotSet, masteryType, isHold);
			if (skillSlotSet.SlotSet2Index() != SkillSlotIndex.Attack)
			{
				this.EnqueueCommandTeamAndObserver(new CmdHoldSkillCooldown
				{
					objectId = base.ObjectId,
					skillSlotSet = skillSlotSet,
					masteryType = masteryType,
					isHold = isHold
				});
			}
		}

		
		public override bool SwitchSkillSet(SkillSlotIndex skillSlotIndex, SkillSlotSet skillSlotSet)
		{
			if (!base.SwitchSkillSet(skillSlotIndex, skillSlotSet))
			{
				return false;
			}
			this.EnqueueCommandTeamAndObserver(new CmdSwitchSkillSet
			{
				objectId = base.ObjectId,
				skillSlotIndex = skillSlotIndex,
				skillSlotSet = skillSlotSet
			});
			return true;
		}

		
		public override void SequenceTimeOver(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			if (!base.CharacterSkill.IsPlayingSequenceTimer(skillSlotSet, masteryType))
			{
				return;
			}
			this.ResetSequenceSkill(skillSlotSet, masteryType);
		}

		
		public override void EndSequenceSkill(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			if (base.CharacterSkill.IsFirstSequence(skillSlotSet))
			{
				return;
			}
			this.ResetSequenceSkill(skillSlotSet, masteryType);
		}

		
		private void EndPlayingSequenceSkills()
		{
			List<SkillSlotSet> playingSequenceSkillSlotSets = base.CharacterSkill.GetPlayingSequenceSkillSlotSets();
			MasteryType equipWeaponMasteryType = this.GetEquipWeaponMasteryType();
			for (int i = 0; i < playingSequenceSkillSlotSets.Count; i++)
			{
				if (playingSequenceSkillSlotSets[i] >= SkillSlotSet.Active4_1 && playingSequenceSkillSlotSets[i] <= SkillSlotSet.Active4_5)
				{
					this.ResetSequenceSkill(playingSequenceSkillSlotSets[i], equipWeaponMasteryType);
				}
				else
				{
					base.EndSequenceSkill(playingSequenceSkillSlotSets[i], equipWeaponMasteryType);
				}
			}
		}

		
		private void ResetSequenceSkill(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			float num;
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				base.CharacterSkill.ResetSkillSequence(skillSlotSet);
				num = GameDB.skill.GetSkillData(masteryType, base.CharacterSkill.GetSkillLevel(masteryType), base.GetSkillSequence(skillSlotSet)).cooldown + base.CharacterSkill.GetReservationCooldown(skillSlotSet);
				base.CharacterSkill.StartCooldown(masteryType, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime, num);
			}
			else
			{
				base.EndSequenceSkill(skillSlotSet, masteryType);
				num = base.CharacterSkill.GetCooldown(skillSlotSet, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
			}
			this.EnqueueRpcCommand(new CmdResetSkillSequence
			{
				skillSlotSet = skillSlotSet
			});
			this.EnqueueCommandTeamAndObserver(new CmdStartSkillCooldown
			{
				objectId = base.ObjectId,
				skillSlotSet = skillSlotSet,
				masteryType = masteryType,
				cooldown = new BlisFixedPoint(num)
			});
		}

		
		public override void StackSkillNeedCharge(SkillSlotSet skillSlotSet, MasteryType masteryType)
		{
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				if (base.CharacterSkill.IsChargingSkillStack(masteryType))
				{
					return;
				}
			}
			else if (base.CharacterSkill.IsChargingSkillStack(skillSlotSet))
			{
				return;
			}
			float cooldown;
			if (skillSlotSet == SkillSlotSet.WeaponSkill)
			{
				cooldown = GameDB.skill.GetSkillData(masteryType, base.CharacterSkill.GetSkillLevel(masteryType), base.GetSkillSequence(skillSlotSet)).cooldown;
				base.CharacterSkill.StartCooldown(masteryType, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime, cooldown);
			}
			else
			{
				base.StackSkillNeedCharge(skillSlotSet, masteryType);
				cooldown = base.CharacterSkill.GetCooldown(skillSlotSet, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
			}
			this.EnqueueCommandTeamAndObserver(new CmdStartSkillCooldown
			{
				objectId = base.ObjectId,
				skillSlotSet = skillSlotSet,
				masteryType = masteryType,
				cooldown = new BlisFixedPoint(cooldown)
			});
		}

		
		public bool IsEquipableItem(ItemData itemData)
		{
			return (itemData.itemType == ItemType.Weapon && this.IsEquipableWeapon(itemData)) || itemData.itemType == ItemType.Armor;
		}

		
		private bool IsEquipableWeapon(ItemData itemData)
		{
			if (itemData.itemType == ItemType.Weapon)
			{
				ItemWeaponData subTypeData = itemData.GetSubTypeData<ItemWeaponData>();
				return this.GetMasteryInfo(subTypeData.GetMasteryType()).masteryType > MasteryType.None;
			}
			return false;
		}

		
		public Item FindInventoryItemById(int itemId, ItemMadeType madeType)
		{
			return this.inventory.FindItemById(itemId, madeType);
		}

		
		public Item FindInventoryItem(int itemCode)
		{
			return this.inventory.FindItem(itemCode);
		}

		
		public bool UseItem(int itemId, ItemMadeType madeType)
		{
			if (!base.IsAlive)
			{
				throw new GameException(ErrorType.CharacterNotAlive);
			}
			if (this.IsDyingCondition)
			{
				return false;
			}
			Item item = this.FindInventoryItemById(itemId, madeType);
			if (item == null)
			{
				throw new GameException(ErrorType.ItemNotFound);
			}
			if (item.Amount <= 0)
			{
				throw new GameException(ErrorType.NotEnoughItem);
			}
			if (this.IsCooldown(item.ItemData))
			{
				throw new GameException(ErrorType.NotAvailableYet);
			}
			this.stateEffector.CanCharacterControl();
			if (item.ItemData.itemType == ItemType.Consume)
			{
				ItemConsumableData subTypeData = item.ItemData.GetSubTypeData<ItemConsumableData>();
				if (subTypeData.heal > 0)
				{
					BasicHealCalculator calculator = new BasicHealCalculator(0, 0f, subTypeData.heal, 0, 0f, 0);
					Singleton<HealService>.inst.SelfHealTo(this, calculator, true, 0);
				}
				if (subTypeData.hpRecover > 0)
				{
					CharacterState characterState = this.stateEffector.FindStateByGroup(10002, 0);
					if (characterState != null && characterState.ReserveCount + 1 >= 3)
					{
						throw new GameException(ErrorType.CantConsumeItem);
					}
					int recovery = item.GetRecovery(false);
					this.HpRecover(10002, recovery);
				}
				if (subTypeData.spRecover > 0)
				{
					CharacterState characterState2 = this.stateEffector.FindStateByGroup(10003, 0);
					if (characterState2 != null && characterState2.ReserveCount + 1 >= 3)
					{
						throw new GameException(ErrorType.CantConsumeItem);
					}
					int recovery2 = item.GetRecovery(true);
					this.SpRecover(10003, recovery2);
				}
				SingletonMonoBehaviour<BattleEventCollector>.inst.OnAfterConsumeItemConsumable(this, subTypeData);
			}
			this.inventory.UseItem(item);
			if (item.IsEmpty())
			{
				this.RemoveInventoryItem(item.id, item.madeType);
			}
			if (item.ItemData.itemType == ItemType.Consume)
			{
				this.SetItemCooldown(item.ItemData);
			}
			this.SendInventoryUpdate(UpdateInventoryType.Use);
			return true;
		}

		
		public void HpRecover(int stateCode, int hpRecoverAmount)
		{
			RegenState regenState = new RegenState(stateCode, this, this);
			regenState.SetHpRecover(hpRecoverAmount);
			base.AddState(regenState, this.objectId);
		}

		
		public void SpRecover(int stateCode, int spRecoverAmount)
		{
			SpRegenState spRegenState = new SpRegenState(stateCode, this, this);
			spRegenState.SetSpRecover(spRecoverAmount);
			base.AddState(spRegenState, this.objectId);
		}

		
		private void SetItemCooldown(ItemData itemData)
		{
			if (!this.ItemCooldowns.Contains(itemData))
			{
				this.ItemCooldowns.Add(itemData);
				float num = 1f;
				this.EnqueueRpcCommand(new CmdAddItemCooldown
				{
					itemCode = itemData.code,
					cooldown = new BlisFixedPoint(num)
				});
				base.StartCoroutine(CoroutineUtil.DelayedAction(num, delegate()
				{
					this.ItemCooldowns.Remove(itemData);
					this.EnqueueRpcCommand(new CmdRemoveItemCooldown
					{
						itemCode = itemData.code
					});
				}));
			}
		}

		
		public bool IsCooldown(ItemData itemData)
		{
			for (int i = 0; i < this.ItemCooldowns.Count; i++)
			{
				if (this.ItemCooldowns[i].IsShareCooldown(itemData))
				{
					return true;
				}
			}
			return false;
		}

		
		public override bool CanUseSkillWithoutRangeCheck(SkillSlotSet skillSlotSet, WorldCharacter target, Vector3 pressPoint, Vector3 releasePoint)
		{
			return base.CanUseSkillWithoutRangeCheck(skillSlotSet, target, pressPoint, releasePoint) && (skillSlotSet.SlotSet2Index() == SkillSlotIndex.Passive || (!this.isRest && this.IsEquippedWeapon())) && !this.IsDyingCondition;
		}

		
		public override bool CanUseInjectSkill(int SkillDataCode, WorldCharacter target, ref Vector3 point, ref Vector3 release)
		{
			return base.CanUseInjectSkill(SkillDataCode, target, ref point, ref release) && !this.isRest && this.IsEquippedWeapon() && !this.IsDyingCondition;
		}

		
		public override bool CanAnyAction(ActionType actionType)
		{
			return base.CanAnyAction(actionType) && !this.IsDyingCondition;
		}

		
		public override bool CanMove()
		{
			if (!base.CanMove())
			{
				return false;
			}
			if (this.IsRunningCastingAction())
			{
				return !this.IsMyCastingAction();
			}
			return !this.isDyingCondition || MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - this.onDyingConditionTime >= 3f;
		}

		
		public override UseSkillErrorCode UseSkill(SkillSlotSet skillSlotSet, Vector3 hitPosition, Vector3 releasePosition)
		{
			return this.UseSkill__(skillSlotSet, hitPosition, releasePosition, delegate()
			{
				this.CancelActionCasting(CastingCancelType.Action);
			}, MasteryType.None);
		}

		
		public override UseSkillErrorCode UseSkill(SkillSlotSet skillSlotSet, WorldCharacter hitTarget)
		{
			return this.UseSkill__(skillSlotSet, hitTarget, delegate()
			{
				this.CancelActionCasting(CastingCancelType.Action);
			}, MasteryType.None);
		}

		
		public override UseSkillErrorCode UseInjectSkill(SkillUseInfo skillUseInfo)
		{
			return base.UseInjectSkill__(skillUseInfo, delegate
			{
				this.CancelActionCasting(CastingCancelType.Action);
			});
		}

		
		public void PlayingSkillOnSelect(WorldCharacter hitTarget, Vector3 hitPosition, Vector3 releasePosition)
		{
			base.SkillController.OnSelect(hitTarget, hitPosition, releasePosition);
		}

		
		public int GetSumMasteryLevel()
		{
			return this.mastery.GetSumMasteryLevel();
		}

		
		public void HyperLoop(Vector3 dest)
		{
			if (!this.isAlive)
			{
				return;
			}
			Vector3 vector;
			if (Blis.Common.MoveAgent.SamplePosition(dest, 2147483640, out vector))
			{
				dest = vector;
			}
			BlisVector blisVector = new BlisVector(dest);
			this.moveAgent.Warp(blisVector);
			MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(this, dest, null, NoiseType.HyperLoopExit);
			base.EnqueueCommand(new CmdHyperLoop
			{
				destination = blisVector
			});
			AreaData currentAreaData = base.GetCurrentAreaData(MonoBehaviourInstance<GameService>.inst.CurrentLevel);
			this.EnqueueCommandTeamAndObserver(new CmdSystemChat
			{
				type = SystemChatType.TeamUseHyperLoop,
				senderObjectId = this.playerSession.ObjectId,
				characterCode = this.characterCode,
				areaCode = ((currentAreaData != null) ? currentAreaData.code : 0)
			});
			this.warpMove = true;
			SingletonMonoBehaviour<BattleEventCollector>.inst.OnAfterHyperLoopWarp(this, dest, (currentAreaData != null) ? currentAreaData.code : 0);
		}

		
		private void OnDyingCondition()
		{
			if (this.IsDyingCondition)
			{
				return;
			}
			this.isRest = false;
			if (this.IsRunningCastingAction())
			{
				this.CancelActionCasting(CastingCancelType.Action);
			}
			this.isDyingCondition = true;
			this.onDyingConditionTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			base.Status.SetHp(500);
			base.StopDeadMove();
			this.stateEffector.RemoveOnDead(true);
			this.skillController.CancelAll();
			this.EndPlayingSequenceSkills();
			base.AddState(new CommonState(10009, this, this), base.ObjectId);
			base.EnqueueCommand(new CmdDyingCondition
			{
				hp = base.Status.Hp,
				sp = base.Status.Sp
			});
			if (this.dyingConditionProcess != null)
			{
				base.StopCoroutine(this.dyingConditionProcess);
			}
			this.dyingConditionProcess = base.StartCoroutine(this.DyingConditionProcess());
		}

		
		public bool IsTeamAnnihilation()
		{
			bool result = true;
			foreach (WorldPlayerCharacter worldPlayerCharacter in this.playerSession.GetTeamCharacters())
			{
				if (!worldPlayerCharacter.IsDyingCondition && worldPlayerCharacter.isAlive)
				{
					result = false;
					break;
				}
			}
			return result;
		}

		
		private IEnumerator DyingConditionProcess()
		{
			int damage = (int)Mathf.Pow(this.dotDamage, (float)this.revivalCount);
			DamageInfo dotDamageInfo = DamageInfo.Create(damage, DamageType.DyingCondition, DamageSubType.Dot, 0, 0, 0);
			dotDamageInfo.SetAttacker(this);
			while (base.IsAlive && this.IsDyingCondition)
			{
				if (!this.isRevivaling)
				{
					this.Damage(dotDamageInfo);
				}
				yield return this.dotSeconds;
			}
		}

		
		public void StartRevival(WorldPlayerCharacter saver)
		{
			this.isRevivaling = true;
			this.teamrevivalCastingCharacter = saver;
		}

		
		public void OnRevivalEnd(WorldPlayerCharacter caster, bool cancel)
		{
			this.isRevivaling = false;
			this.teamrevivalCastingCharacter = null;
			if (!cancel)
			{
				this.TeamRevival(caster);
				SingletonMonoBehaviour<BattleEventCollector>.inst.OnAfterTeamRevival(caster, this);
			}
		}

		
		public void TeamRevival(WorldPlayerCharacter caster)
		{
			base.CombatInvolvementAgent.ClearDyingConditionEvent();
			this.isDyingCondition = false;
			base.RemoveStateByGroup(10009, base.ObjectId);
			this.revivalCount++;
			base.Status.SetHp(base.Stat.MaxHp / 2);
			base.Status.SetSp(base.Stat.MaxSp / 2);
			base.CastPassiveSkill(SkillSlotIndex.Passive);
			base.CastPassiveSkill(SkillSlotIndex.Active1);
			base.CastPassiveSkill(SkillSlotIndex.Active2);
			base.CastPassiveSkill(SkillSlotIndex.Active3);
			base.CastPassiveSkill(SkillSlotIndex.Active4);
			base.CastPassiveSkill(SkillSlotIndex.WeaponSkill);
			base.EnqueueCommand(new CmdTeamRevival
			{
				hp = base.Status.Hp,
				sp = base.Status.Sp
			});
		}

		
		private bool IsGetDyingCondition(DamageType damageType, out bool finalSurvival)
		{
			finalSurvival = false;
			if (this.IsTeamAnnihilation())
			{
				finalSurvival = true;
			}
			return !finalSurvival && damageType != DamageType.RedZone && base.IsAlive && !this.IsDyingCondition;
		}

		
		private void AddMasteryExpIncrementTeamMember()
		{
			List<WorldPlayerCharacter> teamCharacters = this.playerSession.GetTeamCharacters();
			int num = 0;
			for (int i = 0; i < teamCharacters.Count; i++)
			{
				if (!teamCharacters[i].IsAlive)
				{
					num++;
				}
			}
			for (int j = 0; j < teamCharacters.Count; j++)
			{
				if (teamCharacters[j].IsAlive)
				{
					int percent = (num == 1) ? 15 : 15;
					teamCharacters[j].mastery.AddExpIncrementPercent(percent);
				}
			}
		}

		
		public void SendMemorizerUpdate(HashSet<int> addedObjId)
		{
			this.EnqueueRpcCommand(new CmdUpdateCharacterMemorizer
			{
				memorizedTargets = addedObjId
			});
		}

		
		public void SetStartingSettings(PlayerStartingSettings startingSettings)
		{
			this.SetMasteryLevel(startingSettings);
			this.SetWalkableAreas(startingSettings);
			this.SetSkillLevel(startingSettings);
			this.SetSkillPoint(startingSettings);
			this.SetEquipments(startingSettings);
			this.SetInventoryItems(startingSettings);
			int level = (0 < startingSettings.CharacterLevel) ? startingSettings.CharacterLevel : 1;
			CharacterExpData expData = GameDB.character.GetExpData(level);
			this.nextLevelExp = ((expData != null) ? expData.levelUpExp : 0);
			CharacterStat characterStat = new CharacterStat();
			characterStat.UpdateCharacterStat(GameDB.character.GetCharacterData(this.characterCode), level);
			characterStat.UpdateEquipmentStat(this.equipment.GetEquipStats());
			characterStat.UpdateMasteryStat(this.mastery.GetMasteryStats(this.GetEquipWeaponMasteryType()));
			base.SetStat(characterStat);
			base.Status.SetLevel(startingSettings.CharacterLevel);
			base.Status.SetHp(characterStat.MaxHp);
			base.Status.SetSp(characterStat.MaxSp);
			base.Status.SetMoveSpeed(characterStat.MoveSpeed);
			base.Status.SetExtraPoint(characterStat.InitExtraPoint);
		}

		
		private void SetMasteryLevel(PlayerStartingSettings startingSettings)
		{
			CharacterMasteryData characterMasteryData = GameDB.mastery.GetCharacterMasteryData(this.characterCode);
			this.mastery.SetMasteryLevel(characterMasteryData.weapon1, startingSettings.WeaponMasteryLevel1);
			this.mastery.SetMasteryLevel(characterMasteryData.weapon2, startingSettings.WeaponMasteryLevel2);
			this.mastery.SetMasteryLevel(characterMasteryData.weapon3, startingSettings.WeaponMasteryLevel3);
			this.mastery.SetMasteryLevel(characterMasteryData.weapon4, startingSettings.WeaponMasteryLevel4);
			this.mastery.SetMasteryLevel(characterMasteryData.exploration1, startingSettings.ExplorationMasteryLevel1);
			this.mastery.SetMasteryLevel(characterMasteryData.exploration2, startingSettings.ExplorationMasteryLevel2);
			this.mastery.SetMasteryLevel(characterMasteryData.exploration3, startingSettings.ExplorationMasteryLevel3);
			this.mastery.SetMasteryLevel(characterMasteryData.exploration4, startingSettings.ExplorationMasteryLevel4);
			this.mastery.SetMasteryLevel(characterMasteryData.survival1, startingSettings.SurvivalMasteryLevel1);
			this.mastery.SetMasteryLevel(characterMasteryData.survival2, startingSettings.SurvivalMasteryLevel2);
			this.mastery.SetMasteryLevel(characterMasteryData.survival3, startingSettings.SurvivalMasteryLevel3);
			this.mastery.SetMasteryLevel(characterMasteryData.survival4, startingSettings.SurvivalMasteryLevel4);
		}

		
		private void SetWalkableAreas(PlayerStartingSettings startingSettings)
		{
			this.moveAgent.SetWalkableAreas(startingSettings.WalkableAreaCodes);
		}

		
		private void SetSkillLevel(PlayerStartingSettings startingSettings)
		{
			base.CharacterSkill.SetSkillLevel(SkillSlotIndex.Passive, startingSettings.SkillLevelPassive);
			base.CharacterSkill.SetSkillLevel(SkillSlotIndex.Active1, startingSettings.SkillLevelActive1);
			base.CharacterSkill.SetSkillLevel(SkillSlotIndex.Active2, startingSettings.SkillLevelActive2);
			base.CharacterSkill.SetSkillLevel(SkillSlotIndex.Active3, startingSettings.SkillLevelActive3);
			base.CharacterSkill.SetSkillLevel(SkillSlotIndex.Active4, startingSettings.SkillLevelActive4);
		}

		
		private void SetSkillPoint(PlayerStartingSettings startingSettings)
		{
			if (startingSettings.SkillPoint < 0)
			{
				return;
			}
			this.skillPoint = startingSettings.SkillPoint;
		}

		
		private void SetEquipments(PlayerStartingSettings startingSettings)
		{
			int key = startingSettings.StartingWeapon.Key;
			int value = startingSettings.StartingWeapon.Value;
			ItemData itemData = GameDB.item.FindItemByCode(key);
			this.EquipItem(MonoBehaviourInstance<GameService>.inst.Spawn.CreateItem(itemData, value));
			foreach (KeyValuePair<int, int> keyValuePair in startingSettings.StartingArmors)
			{
				if (keyValuePair.Key != 0)
				{
					ItemData itemData2 = GameDB.item.FindItemByCode(keyValuePair.Key);
					this.EquipItem(MonoBehaviourInstance<GameService>.inst.Spawn.CreateItem(itemData2, keyValuePair.Value));
				}
			}
		}

		
		private void SetInventoryItems(PlayerStartingSettings startingSettings)
		{
			foreach (KeyValuePair<int, int> keyValuePair in startingSettings.StartingInventoryItems)
			{
				ItemData itemData = GameDB.item.FindItemByCode(keyValuePair.Key);
				int num;
				this.AddInventoryItem(MonoBehaviourInstance<GameService>.inst.Spawn.CreateItem(itemData, keyValuePair.Value), out num);
			}
		}

		
		public override void ModifyExtraPoint(int changeAmountExtraPoint)
		{
			base.ModifyExtraPoint(changeAmountExtraPoint);
			if (!this.isAlive)
			{
				return;
			}
			if (changeAmountExtraPoint == 0)
			{
				return;
			}
			if (changeAmountExtraPoint > 0)
			{
				changeAmountExtraPoint = Mathf.Min(base.Stat.MaxExtraPoint - base.Status.ExtraPoint, changeAmountExtraPoint);
			}
			base.Status.ModifyExtraPoint(changeAmountExtraPoint);
			base.EnqueueCommand(new CmdSetExtraPoint
			{
				setExtraPoint = base.Status.ExtraPoint
			});
		}

		
		public void UpdateMissionProgress(MissionConditionType conditionType, int count, int conditionCode = -1)
		{
			if (count <= 0)
			{
				return;
			}
			if (this.playerSession.missionList.Count == 0)
			{
				return;
			}
			foreach (MissionData missionData in this.playerSession.missionList.FindAll((MissionData m) => m.conditionType == conditionType))
			{
				if (missionData.conditionItemCode > 0)
				{
					if (missionData.conditionItemCode == conditionCode)
					{
						Dictionary<int, int> missionResultMap = this.playerSession.missionResultMap;
						int key = missionData.key;
						missionResultMap[key] += count;
					}
				}
				else
				{
					Dictionary<int, int> missionResultMap = this.playerSession.missionResultMap;
					int key = missionData.key;
					missionResultMap[key] += count;
				}
			}
		}

		
		protected override void CompleteAddState(CharacterState state)
		{
			base.CompleteAddState(state);
			StateType stateType = state.StateGroupData.stateType;
			if (stateType.IsCrowdControl() && stateType.CanNotAction() != ActionCategoryType.None)
			{
				this.CancelActionCasting(CastingCancelType.Action);
			}
		}

		
		public void SendPingTarget(PingType type, Vector3 targetPosition, WorldObject targetObject, int teamSlot)
		{
			if (this.pingTarget.ForbidPing)
			{
				CmdSystemChat packet = new CmdSystemChat
				{
					type = SystemChatType.PingForbid,
					isNoticeColor = true
				};
				this.playerSession.EnqueueCommandPacket(new PacketWrapper(packet));
				return;
			}
			this.pingTarget.SendPingTarget();
			CmdPing packet2 = new CmdPing
			{
				type = type,
				pingObjectId = ((targetObject == null) ? 0 : targetObject.ObjectId),
				senderObjectId = this.objectId,
				pingPosition = new BlisVector(targetPosition),
				teamSlot = teamSlot
			};
			this.playerSession.EnqueueCommandTeamOnly(new PacketWrapper(packet2));
			Vector3 position;
			Blis.Common.MoveAgent.SampleWidePosition(targetPosition, 2147483640, out position);
			AreaData currentAreaData = AreaUtil.GetCurrentAreaData(MonoBehaviourInstance<GameService>.inst.CurrentLevel, position, 2147483640);
			int num = (currentAreaData != null) ? currentAreaData.code : 0;
			CmdSystemChat cmdSystemChat = null;
			if (num == 0)
			{
				return;
			}
			switch (type)
			{
			case PingType.Run:
				cmdSystemChat = new CmdSystemChat
				{
					type = SystemChatType.PingMoving,
					senderObjectId = this.playerSession.ObjectId,
					characterCode = this.characterCode,
					areaCode = num
				};
				break;
			case PingType.Warning:
				cmdSystemChat = new CmdSystemChat
				{
					type = SystemChatType.PingWarning,
					senderObjectId = this.playerSession.ObjectId,
					characterCode = this.characterCode,
					areaCode = num
				};
				break;
			case PingType.Escape:
				cmdSystemChat = new CmdSystemChat
				{
					type = SystemChatType.PingEscape,
					senderObjectId = this.playerSession.ObjectId,
					characterCode = this.characterCode,
					areaCode = num
				};
				break;
			case PingType.Help:
				cmdSystemChat = new CmdSystemChat
				{
					type = SystemChatType.PingSupport,
					senderObjectId = this.playerSession.ObjectId,
					characterCode = this.characterCode,
					areaCode = num
				};
				break;
			case PingType.Target:
			case PingType.Select:
				if (targetObject != null)
				{
					if (targetObject.ObjectType == ObjectType.PlayerCharacter || targetObject.ObjectType == ObjectType.BotPlayerCharacter)
					{
						WorldPlayerCharacter worldPlayerCharacter = targetObject as WorldPlayerCharacter;
						if (worldPlayerCharacter != null)
						{
							cmdSystemChat = new CmdSystemChat
							{
								type = SystemChatType.PingSelectCharacter,
								senderObjectId = this.playerSession.ObjectId,
								characterCode = this.characterCode,
								targetObjectId = worldPlayerCharacter.ObjectId,
								targetCharacterCode = worldPlayerCharacter.characterCode,
								areaCode = num,
								isMonster = false
							};
						}
					}
					else if (targetObject.ObjectType == ObjectType.Monster)
					{
						cmdSystemChat = new CmdSystemChat
						{
							type = SystemChatType.PingSelectSpecial,
							senderObjectId = this.playerSession.ObjectId,
							characterCode = this.characterCode,
							targetObjectId = targetObject.ObjectId,
							areaCode = num,
							isMonster = true
						};
					}
					else if (targetObject.ObjectType == ObjectType.Item)
					{
						WorldItem worldItem = targetObject as WorldItem;
						if (worldItem != null && worldItem.GetItem() != null)
						{
							cmdSystemChat = new CmdSystemChat
							{
								type = SystemChatType.PingFindItem,
								senderObjectId = this.playerSession.ObjectId,
								characterCode = this.characterCode,
								targetObjectId = worldItem.GetItem().itemCode,
								areaCode = num
							};
						}
					}
					else
					{
						cmdSystemChat = new CmdSystemChat
						{
							type = SystemChatType.PingSelectObject,
							senderObjectId = this.playerSession.ObjectId,
							characterCode = this.characterCode,
							targetObjectId = targetObject.ObjectId,
							areaCode = num
						};
					}
				}
				break;
			}
			if (cmdSystemChat != null)
			{
				this.playerSession.EnqueueTeamMemberCommand(new PacketWrapper(cmdSystemChat));
			}
		}

		
		public void SendMarkTarget(Vector3 targetPosition, WorldObject targetObject, int teamSlot)
		{
			if (this.pingTarget.IsAdjacentMark(teamSlot, targetPosition))
			{
				this.RemoveMarkTarget(teamSlot);
			}
			else
			{
				this.pingTarget.AddMarkTarget(teamSlot, targetPosition);
				List<WorldPlayerCharacter> teamCharacters = this.playerSession.GetTeamCharacters();
				for (int i = 0; i < teamCharacters.Count; i++)
				{
					teamCharacters[i].pingTarget.AddMarkTarget(teamSlot, targetPosition);
				}
				Vector3 position;
				Blis.Common.MoveAgent.SampleWidePosition(targetPosition, 2147483640, out position);
				AreaData currentAreaData = AreaUtil.GetCurrentAreaData(MonoBehaviourInstance<GameService>.inst.CurrentLevel, position, 2147483640);
				int num = (currentAreaData != null) ? currentAreaData.code : 0;
				if (num != 0)
				{
					if (targetObject == null)
					{
						CmdSystemChat packet = new CmdSystemChat
						{
							type = SystemChatType.MarkPosition,
							senderObjectId = this.playerSession.ObjectId,
							characterCode = this.characterCode,
							areaCode = num
						};
						this.playerSession.EnqueueCommandTeamOnly(new PacketWrapper(packet));
					}
					else
					{
						CmdSystemChat packet2 = new CmdSystemChat
						{
							type = SystemChatType.MarkSelect,
							senderObjectId = this.playerSession.ObjectId,
							characterCode = this.characterCode,
							targetObjectId = targetObject.ObjectId,
							areaCode = num
						};
						this.playerSession.EnqueueCommandTeamOnly(new PacketWrapper(packet2));
					}
				}
			}
			this.UpdateMarkTarget();
		}

		
		public void SendRemoveMarkTarget(int teamSlot)
		{
			this.RemoveMarkTarget(teamSlot);
			this.UpdateMarkTarget();
		}

		
		private void RemoveMarkTarget(int teamSlot)
		{
			List<WorldPlayerCharacter> teamCharacters = this.playerSession.GetTeamCharacters();
			this.pingTarget.RemoveMark(teamSlot);
			for (int i = 0; i < teamCharacters.Count; i++)
			{
				teamCharacters[i].pingTarget.RemoveMark(teamSlot);
			}
		}

		
		private void UpdateMarkTarget()
		{
			CmdUpdateMarkTarget packet = new CmdUpdateMarkTarget
			{
				marks = this.pingTarget.MarkInfos
			};
			this.playerSession.EnqueueCommandTeamOnly(new PacketWrapper(packet));
		}

		
		public void SendEmotionIcon(EmotionPlateType type)
		{
			if (!base.IsAlive)
			{
				return;
			}
			int num;
			if (this.playerSession.emotion.TryGetValue(type, out num) && num > 0)
			{
				base.EnqueueCommand(new CmdEmotionIcon
				{
					emotionIconCode = num
				});
			}
		}

		
		public void AddOwnSummon(WorldSummonBase addTarget)
		{
			this.ownSummons.Add(addTarget);
		}

		
		public void RemoveOwnSummon(WorldSummonBase removeTarget)
		{
			this.ownSummons.Remove(removeTarget);
		}

		
		public WorldSummonBase GetOwnSummon(Func<WorldSummonBase, bool> condition)
		{
			for (int i = this.ownSummons.Count - 1; i >= 0; i--)
			{
				if (this.ownSummons[i] == null || !this.ownSummons[i].IsAlive)
				{
					this.ownSummons.RemoveAt(i);
				}
				else if (condition(this.ownSummons[i]))
				{
					return this.ownSummons[i];
				}
			}
			return null;
		}

		
		public List<WorldSummonBase> GetOwnSummons(Func<WorldSummonBase, bool> condition)
		{
			List<WorldSummonBase> list = null;
			for (int i = this.ownSummons.Count - 1; i >= 0; i--)
			{
				if (this.ownSummons[i] == null || !this.ownSummons[i].IsAlive)
				{
					this.ownSummons.RemoveAt(i);
				}
				else if (condition(this.ownSummons[i]))
				{
					if (list == null)
					{
						list = new List<WorldSummonBase>();
					}
					list.Add(this.ownSummons[i]);
				}
			}
			return list;
		}

		
		public void SendToastMessage(ToastMessageType toastMessageType)
		{
			MonoBehaviourInstance<GameServer>.inst.Send(this.playerSession, new RpcToastMessage
			{
				toastMessageType = toastMessageType
			}, NetChannel.ReliableOrdered);
		}

		
		public void LockSkillSlot(SkillSlotSet skillSlotSet, bool isLock)
		{
			this.skillController.LockSkillSlot(skillSlotSet, isLock);
		}

		
		public void LockSkillSlot(SpecialSkillId specialSkillId, bool isLock)
		{
			this.skillController.LockSkillSlot(specialSkillId, isLock);
		}

		
		public void LockSkillSlotsWithPacket(SkillSlotSet skillSlotSetFlag, bool isLock)
		{
			foreach (SkillSlotSet skillSlotSet in GameDB.skill.allSkillSlotSet)
			{
				if (skillSlotSet != SkillSlotSet.None && skillSlotSetFlag.HasFlag(skillSlotSet))
				{
					this.skillController.LockSkillSlot(skillSlotSet, isLock);
				}
			}
			if (isLock)
			{
				this.lockedSlotSetFlag |= skillSlotSetFlag;
			}
			else
			{
				this.lockedSlotSetFlag &= ~skillSlotSetFlag;
			}
			MonoBehaviourInstance<GameServer>.inst.Send(this.playerSession, new RpcSkillSlotLock
			{
				skillSlotSetFlag = skillSlotSetFlag,
				isLock = isLock
			}, NetChannel.ReliableOrdered);
		}

		
		public override void OnSkillReserveCancel(SkillActionBase skillActionBase)
		{
			MonoBehaviourInstance<GameServer>.inst.Send(this.playerSession, new RpcSkillReserveCancel
			{
				skillSlotSet = skillActionBase.SkillSlotSet
			}, NetChannel.ReliableOrdered);
		}

		
		protected override UseSkillErrorCode UseSkill__(SkillSlotSet skillSlotSet, Vector3 hitPosition, Vector3 releasePosition, Action extraAction = null, MasteryType masteryType = MasteryType.None)
		{
			masteryType = this.GetEquipWeaponMasteryType();
			return base.UseSkill__(skillSlotSet, hitPosition, releasePosition, extraAction, masteryType);
		}

		
		protected override UseSkillErrorCode UseSkill__(SkillSlotSet skillSlotSet, WorldCharacter hitTarget, Action extraAction = null, MasteryType masteryType = MasteryType.None)
		{
			masteryType = this.GetEquipWeaponMasteryType();
			return base.UseSkill__(skillSlotSet, hitTarget, extraAction, masteryType);
		}

		
		public Dictionary<string, int> GetLogSkillLevelInfo()
		{
			return this.skillIndexLevelMap;
		}

		
		public Dictionary<string, int> GetLogSkillLevelInfoLegacy()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (KeyValuePair<int, int> keyValuePair in this.skillUpOrderMap)
			{
				SkillData skillData = GameDB.skill.GetSkillData(keyValuePair.Value);
				if (dictionary.ContainsKey(skillData.Name))
				{
					Dictionary<string, int> dictionary2 = dictionary;
					string name = skillData.Name;
					dictionary2[name]++;
				}
				else
				{
					dictionary.Add(skillData.Name, 1);
				}
			}
			return dictionary;
		}

		
		public Dictionary<int, string> GetLogSkillUpOrder()
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			foreach (KeyValuePair<int, int> keyValuePair in this.skillUpOrderMap)
			{
				SkillData skillData = GameDB.skill.GetSkillData(keyValuePair.Value);
				dictionary.Add(keyValuePair.Key, skillData.Name);
			}
			return dictionary;
		}

		
		public Dictionary<int, int> GetSkillUpOrder()
		{
			return this.skillUpOrderMap;
		}

		
		public Dictionary<string, int> GetLogSkillEvolutionLevelInfo()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (KeyValuePair<int, int> keyValuePair in this.skillEvolutionOrderMap)
			{
				SkillData skillData = GameDB.skill.GetSkillData(keyValuePair.Value);
				if (dictionary.ContainsKey(skillData.Name))
				{
					Dictionary<string, int> dictionary2 = dictionary;
					string name = skillData.Name;
					dictionary2[name]++;
				}
				else
				{
					dictionary.Add(skillData.Name, 1);
				}
			}
			return dictionary;
		}

		
		public Dictionary<int, string> GetLogSkillEvolutionOrder()
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			foreach (KeyValuePair<int, int> keyValuePair in this.skillEvolutionOrderMap)
			{
				SkillData skillData = GameDB.skill.GetSkillData(keyValuePair.Value);
				dictionary.Add(keyValuePair.Key, skillData.Name);
			}
			return dictionary;
		}

		
		protected void ApplyModeModifier(WeaponType oldWeapon, WeaponType newWeapon)
		{
			if (oldWeapon == newWeapon)
			{
				return;
			}
			MatchingTeamMode matchingTeamMode = MonoBehaviourInstance<GameService>.inst.MatchingTeamMode;
			CharacterModeModifierData characterModeModifierData = GameDB.character.GetCharacterModeModifierData(this.characterCode, newWeapon);
			CharacterState characterState = null;
			bool flag = false;
			switch (matchingTeamMode)
			{
			case MatchingTeamMode.None:
			case MatchingTeamMode.Solo:
				characterState = CharacterState.Create(11000, this, this, 1, new float?(0f));
				if (!characterModeModifierData.AnySololModifier())
				{
					characterState.AddExternalStat(StatType.IncreaseModeDamageRatio, characterModeModifierData.soloIncreaseModeDamageRatio, StatType.None, 0f);
					characterState.AddExternalStat(StatType.PreventModeDamageRatio, characterModeModifierData.soloPreventModeDamageRatio, StatType.None, 0f);
					characterState.AddExternalStat(StatType.IncreaseModeHealRatio, characterModeModifierData.soloIncreaseModeHealRatio, StatType.None, 0f);
					characterState.AddExternalStat(StatType.IncreaseModeShieldRatio, characterModeModifierData.soloIncreaseModeShieldRatio, StatType.None, 0f);
					flag = true;
				}
				break;
			case MatchingTeamMode.Duo:
				characterState = CharacterState.Create(11001, this, this, 1, new float?(0f));
				if (!characterModeModifierData.AnyDuoModifier())
				{
					characterState.AddExternalStat(StatType.IncreaseModeDamageRatio, characterModeModifierData.duoIncreaseModeDamageRatio, StatType.None, 0f);
					characterState.AddExternalStat(StatType.PreventModeDamageRatio, characterModeModifierData.duoPreventModeDamageRatio, StatType.None, 0f);
					characterState.AddExternalStat(StatType.IncreaseModeHealRatio, characterModeModifierData.duoIncreaseModeHealRatio, StatType.None, 0f);
					characterState.AddExternalStat(StatType.IncreaseModeShieldRatio, characterModeModifierData.duoIncreaseModeShieldRatio, StatType.None, 0f);
					flag = true;
				}
				break;
			case MatchingTeamMode.Squad:
				characterState = CharacterState.Create(11002, this, this, 1, new float?(0f));
				if (!characterModeModifierData.AnyTrioModifier())
				{
					characterState.AddExternalStat(StatType.IncreaseModeDamageRatio, characterModeModifierData.squadIncreaseModeDamageRatio, StatType.None, 0f);
					characterState.AddExternalStat(StatType.PreventModeDamageRatio, characterModeModifierData.squadPreventModeDamageRatio, StatType.None, 0f);
					characterState.AddExternalStat(StatType.IncreaseModeHealRatio, characterModeModifierData.squadIncreaseModeHealRatio, StatType.None, 0f);
					characterState.AddExternalStat(StatType.IncreaseModeShieldRatio, characterModeModifierData.squadIncreaseModeShieldRatio, StatType.None, 0f);
					flag = true;
				}
				break;
			}
			this.stateEffector.RemoveByGroup(characterState.Group, base.ObjectId);
			this.CompleteRemoveState(characterState, true);
			if (flag)
			{
				this.stateEffector.AddModeModifierStat(characterState, base.ObjectId);
				this.stateEffector.StatUpdateInstantly();
				this.CompleteAddState(characterState);
			}
		}

		
		public bool IsOutSight(int objectId)
		{
			return this.currOutSights.Contains(objectId);
		}

		
		public HashSet<int> GetOutSightCharacterIds()
		{
			return this.currOutSights;
		}

		
		public HashSet<int> GetInvisibleCharacterIds()
		{
			return this.currInvisibleCharacters;
		}

		
		public void CancelRest()
		{
			if (this.IsRest)
			{
				CastingActionType actionCostType = ActionCostData.GetActionCostType(false);
				this.StartActionCasting(actionCostType, false, null, delegate
				{
					this.Rest(false, true);
				}, null, 0);
			}
		}

		
		public bool IsLuke()
		{
			return this.characterCode == 22;
		}

		
		protected Mastery mastery;

		
		private PingTarget pingTarget;

		
		private PlayerSession playerSession;

		
		private Equipment equipment;

		
		private Inventory inventory;

		
		private int visitedAreaMaskCodeFlag;

		
		private HashSet<int> interactedObjectIds = new HashSet<int>();

		
		private HashSet<WorldSummonBase> visibleSummons;

		
		private PlayerHostileAgent hostileAgent;

		
		private const int StartLevel = 1;

		
		private int skillPoint = 1;

		
		private int characterSettingCode;

		
		private int exp;

		
		private int nextLevelExp;

		
		private int characterCode;

		
		private bool isRest;

		
		protected float survivableTime;

		
		protected MMRContext mmrContext;

		
		private int reloadItemId;

		
		private Coroutine runningReload;

		
		private bool warpMove;

		
		private float gunReloadTime = 2f;

		
		public int rank;

		
		public int teamRank = -1;

		
		private int teamNumber;

		
		private List<ItemData> ItemCooldowns = new List<ItemData>();

		
		private readonly List<CharacterStatValue> rpcUpdateStats = new List<CharacterStatValue>();

		
		private readonly List<MissionData> dailyMissions = new List<MissionData>();

		
		private bool isDyingCondition;

		
		private float onDyingConditionTime;

		
		private bool isRevivaling;

		
		private int revivalCount;

		
		private Coroutine dyingConditionProcess;

		
		private readonly float dotDamage = 2f;

		
		private readonly float revivalCastingTime = 10f;

		
		private readonly WaitForSeconds dotSeconds = new WaitForSeconds(1f);

		
		private List<WorldSummonBase> ownSummons = new List<WorldSummonBase>();

		
		private List<BulletCooldown> listCooldowns = new List<BulletCooldown>();

		
		private int skillUpOrder;

		
		private readonly Dictionary<int, int> skillUpOrderMap = new Dictionary<int, int>();

		
		private readonly Dictionary<string, int> skillIndexLevelMap = new Dictionary<string, int>();

		
		private int skillEvolutionOrder;

		
		private readonly Dictionary<int, int> skillEvolutionOrderMap = new Dictionary<int, int>();

		
		private SkillSlotSet lockedSlotSetFlag;

		
		private List<WorldObject> currInSights = new List<WorldObject>();

		
		private HashSet<int> currOutSights = new HashSet<int>();

		
		private HashSet<int> prevOutSights = new HashSet<int>();

		
		private readonly HashSet<int> checkedSights = new HashSet<int>();

		
		private HashSet<int> currInvisibleCharacters = new HashSet<int>();

		
		private HashSet<int> prevInvisibleCharacters = new HashSet<int>();

		
		private readonly HashSet<int> checkedInvisibleCharacters = new HashSet<int>();

		
		private const float MinMySightRange = 11f;

		
		private const float ExtraSightRange = 1f;

		
		private HashSet<int> alwaysInsightObjectId = new HashSet<int>();

		
		private Vector3 lastPosition;

		
		private float moveDistance;

		
		private WaitForFrameUpdate waitSecondUpdateMoveDistance = new WaitForFrameUpdate();

		
		private WaitForFrameUpdate waitSecondHpSpRegen = new WaitForFrameUpdate();

		
		private float regenRemainHp;

		
		private float regenRemainSp;

		
		private Action castingCallback;

		
		private Action castingCancelCallback;

		
		private CastingActionType runningCastingType;

		
		private Coroutine runningCastingAction;

		
		private int openCastingAIrSupplyObjectId;

		
		private EffectCancelCondition castingCancelCondition;

		
		private WorldPlayerCharacter teamrevivalCastingCharacter;
	}
}
