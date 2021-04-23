using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Server
{
	
	public class SpawnService : ServiceBase
	{
		
		
		public float? WicklineRespawnTime
		{
			get
			{
				return this.wicklineRespawnTime;
			}
		}

		
		public void AddRespawnMonster(MonsterSpawnPoint spawnPoint)
		{
			if (this.game.MatchingMode.IsTutorialMode() && !GameDB.tutorial.GetTutorialSettingData(this.game.MatchingMode.ConvertToTutorialType()).enableMonsterReSpawn)
			{
				return;
			}
			MonsterData data = GameDB.monster.GetMonsterData(spawnPoint.monsterCode);
			if (data == null)
			{
				return;
			}
			if (data.regenTime == 0)
			{
				return;
			}
			base.StartCoroutine(CoroutineUtil.DelayedAction((float)data.regenTime, delegate()
			{
				this.RespawnMonster(spawnPoint, data);
			}));
		}

		
		private void RespawnMonster(MonsterSpawnPoint spawnPoint, MonsterData data)
		{
			BlisVector blisVector = new BlisVector(spawnPoint.transform.position);
			WorldMonster worldMonster = this.world.Spawn<WorldMonster>(blisVector.SamplePosition(), spawnPoint.transform.rotation);
			MonsterSpawnLevelData monsterSpawnLevelData = GameDB.monster.GetMonsterSpawnLevelData(this.game.PlayerCharacter.GetTopPlayerLevel());
			worldMonster.Init(data, spawnPoint, monsterSpawnLevelData.GetSpawnLevel(data.monster));
			worldMonster.SetAggressive(MonoBehaviourInstance<GameService>.inst.Area.DayNight);
			this.server.EnqueueCommand(new CmdSpawn
			{
				snapshot = worldMonster.CreateSnapshotWrapper()
			});
		}

		
		public WorldObserver SpawnObserver()
		{
			WorldObserver worldObserver = this.world.Spawn<WorldObserver>(new Vector3(0f, GameConstants.DefaultObjectY, 0f), false);
			worldObserver.Init();
			return worldObserver;
		}

		
		public WorldPlayerCharacter SpawnPlayerCharacter(int characterCode, int skinCode, int teamNumber, SpecialSkillId specialSkillId)
		{
			Vector3 position = this.game.Level.GetInitialPlayerSpawnPoint().transform.position;
			WorldPlayerCharacter worldPlayerCharacter = this.world.Spawn<WorldPlayerCharacter>(new Vector3(position.x, GameConstants.DefaultObjectY, position.z), false);
			worldPlayerCharacter.Init(characterCode, skinCode, teamNumber, specialSkillId);
			return worldPlayerCharacter;
		}

		
		public WorldBotPlayerCharacter SpawnBotPlayerCharacter(int characterCode, int teamNumber, SpecialSkillId specialSkillId)
		{
			BlisVector blisVector = new BlisVector(this.game.Level.GetInitialPlayerSpawnPoint().transform.position);
			WorldBotPlayerCharacter worldBotPlayerCharacter = this.world.Spawn<WorldBotPlayerCharacter>(blisVector.SamplePosition(), true);
			worldBotPlayerCharacter.Init(characterCode, 0, teamNumber, specialSkillId);
			return worldBotPlayerCharacter;
		}

		
		public WorldBotPlayerCharacter SpawnBotPlayerCharacter(int characterCode, int teamNumber, int areaCode, int index, SpecialSkillId specialSkillId)
		{
			BlisVector blisVector = new BlisVector(this.game.Level.GetPlayerSpawnPoint(areaCode, index).transform.position);
			WorldBotPlayerCharacter worldBotPlayerCharacter = this.world.Spawn<WorldBotPlayerCharacter>(blisVector.SamplePosition(), true);
			worldBotPlayerCharacter.Init(characterCode, 0, teamNumber, specialSkillId);
			return worldBotPlayerCharacter;
		}

		
		public WorldItem SpawnItem(Vector3 center, Item item, Vector3 itemPosition)
		{
			Vector3 vector = itemPosition + new Vector3(0f, 0.35f, 0f);
			NavMeshHit navMeshHit;
			if (NavMesh.Raycast(vector, center, out navMeshHit, -1))
			{
				vector = center + new Vector3(0f, 0.35f, 0f);
			}
			if (Vector3.Distance(center, vector) > 6f)
			{
				vector = center;
			}
			WorldItem worldItem = this.world.Spawn<WorldItem>(vector);
			worldItem.Init(item);
			this.server.EnqueueCommand(new CmdSpawn
			{
				snapshot = worldItem.CreateSnapshotWrapper()
			});
			return worldItem;
		}

		
		public Item CreateItem(ItemData itemData, int amount)
		{
			int bulletCapacity = GameDB.item.GetBulletCapacity(itemData.code);
			return new Item(this.game.IncreaseAndGetItemId(), itemData.code, amount, bulletCapacity, itemData);
		}

		
		public WorldSummonBase SpawnSummon(WorldPlayerCharacter ownerCharacter, int summonCode, Vector3 position)
		{
			SummonData summonData = GameDB.character.GetSummonData(summonCode);
			WorldSummonBase worldSummonBase = null;
			switch (summonData.objectType)
			{
			case ObjectType.SummonCamera:
				worldSummonBase = this.world.Spawn<WorldSummonCamera>(position);
				break;
			case ObjectType.SummonTrap:
				worldSummonBase = this.world.Spawn<WorldSummonTrap>(position);
				break;
			case ObjectType.SummonServant:
				worldSummonBase = this.world.Spawn<WorldSummonServant>(position);
				break;
			}
			SingletonMonoBehaviour<BattleEventCollector>.inst.OnInstalledTrap(worldSummonBase, ownerCharacter, summonData);
			worldSummonBase.Init(summonData, ownerCharacter);
			if (ownerCharacter != null && summonData.attackPower > 0 && summonData.objectType == ObjectType.SummonTrap)
			{
				ownerCharacter.AddMasteryConditionExp(MasteryConditionType.UseTrap, 1);
			}
			this.server.EnqueueCommand(new CmdSpawn
			{
				snapshot = worldSummonBase.CreateSnapshotWrapper()
			});
			return worldSummonBase;
		}

		
		public WorldSecurityCamera SpawnSecurityCamera(Vector3 position, Quaternion rotation)
		{
			WorldSecurityCamera worldSecurityCamera = this.world.Spawn<WorldSecurityCamera>(position, rotation);
			worldSecurityCamera.Init();
			return worldSecurityCamera;
		}

		
		public WorldSecurityConsole SpawnSecurityConsole(Vector3 position, Quaternion rotation)
		{
			WorldSecurityConsole worldSecurityConsole = this.world.Spawn<WorldSecurityConsole>(position, rotation);
			worldSecurityConsole.Init();
			return worldSecurityConsole;
		}

		
		public WorldHyperloop SpawnHyperloop(Vector3 position, Quaternion rotation)
		{
			WorldHyperloop worldHyperloop = this.world.Spawn<WorldHyperloop>(position, rotation);
			worldHyperloop.Init();
			return worldHyperloop;
		}

		
		public WorldSightObject SpawnSightObject(WorldCharacter owner, Vector3 position, float sightRange, float duration, bool isRemoveWhenInvisibleStart)
		{
			WorldSightObject worldSightObject = this.world.Spawn<WorldSightObject>(position);
			worldSightObject.Init(owner, sightRange, isRemoveWhenInvisibleStart);
			worldSightObject.DelayDestroySelf(duration);
			this.server.EnqueueCommand(new CmdSpawn
			{
				snapshot = worldSightObject.CreateSnapshotWrapper()
			});
			return worldSightObject;
		}

		
		public void DestroyWorldObject(WorldObject obj)
		{
			if (obj == null)
			{
				return;
			}
			if (!this.world.HasObjectId(obj.ObjectId))
			{
				return;
			}
			this.world.DestroyObject(obj);
			this.server.EnqueueCommand(new CmdDestroy
			{
				objectId = obj.ObjectId
			});
		}

		
		public void SpawnDummy(Vector3 position, int prefabNo)
		{
			WorldDummy worldDummy = this.world.Spawn<WorldDummy>(position);
			worldDummy.Init(prefabNo);
			this.server.EnqueueCommand(new CmdSpawn
			{
				snapshot = worldDummy.CreateSnapshotWrapper()
			});
		}

		
		public void SpawnSkillDummy(Vector3 position, int prefabNo)
		{
			WorldSkillDummy worldSkillDummy = this.world.Spawn<WorldSkillDummy>(position);
			worldSkillDummy.Init(prefabNo);
			this.server.EnqueueCommand(new CmdSpawn
			{
				snapshot = worldSkillDummy.CreateSnapshotWrapper()
			});
		}

		
		public WorldAirSupplyItemBox SpawnAirSupplyItemBox(ScheduledAirSupply airSupply)
		{
			WorldAirSupplyItemBox worldAirSupplyItemBox = this.world.Spawn<WorldAirSupplyItemBox>(airSupply.objectId, airSupply.position, airSupply.rotation);
			worldAirSupplyItemBox.Init(airSupply.itemSpawnPointCode, airSupply.items.Count, airSupply.highestIitemGrade);
			for (int i = 0; i < airSupply.items.Count; i++)
			{
				worldAirSupplyItemBox.ItemBox.AddItem(airSupply.items[i]);
			}
			return worldAirSupplyItemBox;
		}

		
		public WorldStaticItemBox SpawnStaticItemBox(Vector3 position, Quaternion rotation, int code, ItemBoxSize boxSize)
		{
			WorldStaticItemBox worldStaticItemBox = this.world.Spawn<WorldStaticItemBox>(position, rotation);
			worldStaticItemBox.Init(code, boxSize);
			return worldStaticItemBox;
		}

		
		public WorldResourceItemBox SpawnResourceItemBox(Vector3 position, Quaternion rotation, int code, int resourceDataCode, int areaCode, float initSpawnTime)
		{
			WorldResourceItemBox worldResourceItemBox = this.world.Spawn<WorldResourceItemBox>(position, rotation);
			worldResourceItemBox.Init(code, resourceDataCode, areaCode, initSpawnTime);
			return worldResourceItemBox;
		}

		
		public void SpawnTutorialMonster(LevelData currentLevel)
		{
			foreach (MonsterSpawnPoint monsterSpawnPoint in currentLevel.monsterSpawnPoints)
			{
				MonsterData monsterData = GameDB.monster.GetMonsterData(monsterSpawnPoint.monsterCode);
				if (monsterData == null)
				{
					new GameException(string.Format("Failed to find monsterData by monsterCode = {0}", monsterSpawnPoint.monsterCode));
				}
				else if (monsterData.monster != MonsterType.Wickline)
				{
					BlisVector blisVector = new BlisVector(monsterSpawnPoint.transform.position);
					WorldMonster worldMonster = this.world.Spawn<WorldMonster>(blisVector.SamplePosition(), monsterSpawnPoint.transform.rotation);
					MonsterSpawnLevelData monsterSpawnLevelData = GameDB.monster.GetMonsterSpawnLevelData(1);
					worldMonster.Init(monsterData, monsterSpawnPoint, monsterSpawnLevelData.GetSpawnLevel(monsterData.monster));
				}
			}
		}

		
		public void SpawnMonsterCreateTimeZero(LevelData currentLevel)
		{
			foreach (MonsterSpawnPoint monsterSpawnPoint in currentLevel.monsterSpawnPoints)
			{
				MonsterData monsterData = GameDB.monster.GetMonsterData(monsterSpawnPoint.monsterCode);
				if (monsterData == null)
				{
					new GameException(string.Format("Failed to find monsterData by monsterCode = {0}", monsterSpawnPoint.monsterCode));
				}
				else if (monsterData.createTime == 0)
				{
					BlisVector blisVector = new BlisVector(monsterSpawnPoint.transform.position);
					WorldMonster worldMonster = this.world.Spawn<WorldMonster>(blisVector.SamplePosition(), monsterSpawnPoint.transform.rotation);
					MonsterSpawnLevelData monsterSpawnLevelData = GameDB.monster.GetMonsterSpawnLevelData(1);
					worldMonster.Init(monsterData, monsterSpawnPoint, monsterSpawnLevelData.GetSpawnLevel(monsterData.monster));
				}
			}
		}

		
		public void SpawnMonsterCreateTimeNonZero(LevelData currentLevel)
		{
			Dictionary<MonsterType, List<SpawnService.MonsterSpawnInfo>> dictionary = new Dictionary<MonsterType, List<SpawnService.MonsterSpawnInfo>>();
			foreach (MonsterSpawnPoint monsterSpawnPoint in currentLevel.monsterSpawnPoints)
			{
				MonsterData monsterData = GameDB.monster.GetMonsterData(monsterSpawnPoint.monsterCode);
				if (monsterData == null)
				{
					new GameException(string.Format("Failed to find monsterData by monsterCode = {0}", monsterSpawnPoint.monsterCode));
				}
				else if (monsterData.createTime > 0)
				{
					List<SpawnService.MonsterSpawnInfo> list = dictionary.ContainsKey(monsterData.monster) ? dictionary[monsterData.monster] : null;
					if (list == null)
					{
						list = new List<SpawnService.MonsterSpawnInfo>();
						dictionary.Add(monsterData.monster, list);
					}
					list.Add(new SpawnService.MonsterSpawnInfo
					{
						data = monsterData,
						point = monsterSpawnPoint
					});
				}
			}
			foreach (MonsterType monsterType in EnumUtil.GetValues<MonsterType>())
			{
				List<SpawnService.MonsterSpawnInfo> monsterSpawnInfo = dictionary.ContainsKey(monsterType) ? dictionary[monsterType] : null;
				if (monsterSpawnInfo != null && monsterSpawnInfo.Count != 0)
				{
					MonsterData monsterData2 = GameDB.monster.GetMonsterData(monsterType);
					base.StartCoroutine(CoroutineUtil.DelayedAction((float)monsterData2.createTime, delegate()
					{
						this.SpawnMonster(monsterSpawnInfo);
					}));
					if (monsterData2.monster == MonsterType.Wickline)
					{
						this.wicklineRespawnTime = new float?(MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime + (float)GameDB.monster.GetMonsterData(7).createTime);
						this.server.Broadcast(new RpcNoticeWicklineSpawnStart
						{
							wicklineRespawnTime = new BlisFixedPoint(MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime + (float)GameDB.monster.GetMonsterData(7).createTime)
						}, NetChannel.ReliableOrdered);
					}
				}
			}
		}

		
		private void SpawnMonster(List<SpawnService.MonsterSpawnInfo> monsterSpawnInfos)
		{
			List<SnapshotWrapper> list = new List<SnapshotWrapper>();
			foreach (SpawnService.MonsterSpawnInfo monsterSpawnInfo in monsterSpawnInfos)
			{
				Transform transform = monsterSpawnInfo.point.transform;
				WorldMonster worldMonster = this.world.Spawn<WorldMonster>(new BlisVector(transform.position).SamplePosition(), transform.rotation);
				MonsterSpawnLevelData monsterSpawnLevelData = GameDB.monster.GetMonsterSpawnLevelData(this.game.PlayerCharacter.GetTopPlayerLevel());
				worldMonster.Init(monsterSpawnInfo.data, monsterSpawnInfo.point, monsterSpawnLevelData.GetSpawnLevel(monsterSpawnInfo.data.monster));
				list.Add(worldMonster.CreateSnapshotWrapper());
			}
			this.server.EnqueueCommand(new CmdSpawns
			{
				snapshots = list
			});
		}

		
		public List<WorldResourceItemBox> listLifeoftrees = new List<WorldResourceItemBox>();

		
		private float? wicklineRespawnTime;

		
		public class MonsterSpawnInfo
		{
			
			public MonsterData data;

			
			public MonsterSpawnPoint point;
		}
	}
}
