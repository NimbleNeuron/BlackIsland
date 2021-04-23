using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public class AnnounceService : ServiceBase
	{
		
		public void DeadAnnounce(DamageType damageType, WorldPlayerCharacter deadPlayer, ObjectType attackerObjectType, int attackerObjectId, int attackerDataCode, DamageSubType attackerDamageSubType, List<int> assistants)
		{
			if (damageType == DamageType.RedZone)
			{
				this.RedZoneDeadAnnounce(deadPlayer);
				return;
			}
			this.CreateDeadPlayerAnnounceInfo(deadPlayer, attackerObjectType, attackerObjectId, attackerDataCode, attackerDamageSubType, assistants);
		}

		
		private void RedZoneDeadAnnounce(WorldPlayerCharacter deadPlayer)
		{
			this.BroadcastAnnounce<DeadAnnounceInfo>(GameAnnounceType.DeadInRestrictArea, new DeadAnnounceInfo
			{
				deadPlayerObjectId = deadPlayer.PlayerSession.ObjectId,
				deadCharacterCode = deadPlayer.CharacterCode,
				aliveCount = MonoBehaviourInstance<GameService>.inst.Player.AlivePlayerCharacterCount() + MonoBehaviourInstance<GameService>.inst.Bot.GetAliveBotCharacterCount()
			});
		}

		
		public void MakeNoise(WorldObject creatorObject, WorldObject assistObject, NoiseType noiseType)
		{
			this.MakeNoise(creatorObject, creatorObject.GetPosition(), assistObject, noiseType);
		}

		
		public void MakeNoise(WorldObject creatorObject, Vector3 pos, WorldObject assistObject, NoiseType noiseType)
		{
			if (creatorObject == null)
			{
				return;
			}
			if (noiseType == NoiseType.None)
			{
				return;
			}
			if (this.HaveNoiseIgnoreState(creatorObject, assistObject, noiseType))
			{
				return;
			}
			int objectId = creatorObject.ObjectId;
			float num = (objectId != 0) ? this.noiseHistory.GetRecentNoiseHistory(objectId, noiseType) : 0f;
			if (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime > num + 2f)
			{
				NoiseData noiseData = GameDB.effectAndSound.GetNoiseData(noiseType);
				if (noiseData.pingChanceRate > 0f && noiseData.pingChanceRate >= UnityEngine.Random.value)
				{
					this.noiseHistory.UpdateNoiseHistory(objectId, noiseType, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
					int num2 = Physics.OverlapSphereNonAlloc(pos, noiseData.pingRange, this.colliderBuffer, GameConstants.LayerMask.WORLD_OBJECT_LAYER);
					if (num2 > 0)
					{
						List<Session> list = new List<Session>(num2);
						for (int i = 0; i < num2; i++)
						{
							WorldPlayerCharacter component = this.colliderBuffer[i].GetComponent<WorldPlayerCharacter>();
							if (!(component == null))
							{
								list.Add(component.PlayerSession);
							}
						}
						foreach (ObserverSession item in MonoBehaviourInstance<GameService>.inst.Player.ObserverSessions)
						{
							list.Add(item);
						}
						this.server.Broadcast(list, new RpcNoise
						{
							noisePos = new BlisVector(pos),
							creatorObjectId = objectId
						}, NetChannel.ReliableOrdered);
					}
				}
			}
		}

		
		public void AirSupplyAnnounce()
		{
			this.BroadcastAnnounce(GameAnnounceType.AirSupplyNotice);
		}

		
		public void RestrictionAreaAnnounce(AreaRestrictionAnnounceType restrictionType)
		{
			this.BroadcastAnnounce<AreaRestrictionAnnounceType>(GameAnnounceType.RestrictAreaNotice, restrictionType);
		}

		
		public void LastSafeConsoleAnnounce(LastSafeConsoleAnnounceType consoleAnnounceType)
		{
			this.BroadcastAnnounce<LastSafeConsoleAnnounceType>(GameAnnounceType.LastSafeConsole, consoleAnnounceType);
		}

		
		private void CreateDeadPlayerAnnounceInfo(WorldPlayerCharacter deadCharacter, ObjectType attackerObjectType, int attackerObjectId, int attackerDataCode, DamageSubType attackerDamageSubType, List<int> assistants)
		{
			if (deadCharacter == null)
			{
				Log.V("[CreateDeadPlayerAnnounceInfo] deadCharacter is null");
				return;
			}
			if (deadCharacter.PlayerSession == null)
			{
				Log.V("[CreateDeadPlayerAnnounceInfo] deadCharacter PlayerSession is null");
				return;
			}
			if (attackerObjectType != ObjectType.PlayerCharacter)
			{
				if (attackerObjectType != ObjectType.Monster)
				{
					if (attackerObjectType == ObjectType.BotPlayerCharacter)
					{
						goto IL_3A;
					}
				}
				else
				{
					this.BroadcastAnnounce<MonsterKillAnnounceInfo>(GameAnnounceType.DeadByMonster, new MonsterKillAnnounceInfo
					{
						deadPlayerObjectId = deadCharacter.ObjectId,
						deadCharacterCode = deadCharacter.CharacterCode,
						killMonsterCode = attackerDataCode,
						aliveCount = MonoBehaviourInstance<GameService>.inst.Player.AlivePlayerCharacterCount() + MonoBehaviourInstance<GameService>.inst.Bot.GetAliveBotCharacterCount()
					});
				}
				return;
			}
			IL_3A:
			WorldPlayerCharacter worldPlayerCharacter = MonoBehaviourInstance<GameService>.inst.World.Find<WorldPlayerCharacter>(attackerObjectId);
			WeaponType killWeaponType = WeaponType.None;
			Item weapon = worldPlayerCharacter.GetWeapon();
			if (weapon != null)
			{
				killWeaponType = weapon.ItemData.GetSubTypeData<ItemWeaponData>().weaponType;
			}
			if (worldPlayerCharacter.Status.PlayerKill <= 0)
			{
				Log.V("[CreateDeadPlayerAnnounceInfo] DeadPlayer : {0}({1}), Attacker : {2}({3}), KillWeaponType({4}) - PLAYER STATUS PLAYER KILL COUNT : {5}", new object[]
				{
					deadCharacter.PlayerSession.nickname,
					deadCharacter.CharacterCode.ToString(),
					worldPlayerCharacter.PlayerSession.nickname,
					worldPlayerCharacter.CharacterCode.ToString(),
					killWeaponType.ToString(),
					worldPlayerCharacter.Status.PlayerKill.ToString()
				});
				return;
			}
			this.BroadcastAnnounce<PlayerKillAnnounceInfo>(GameAnnounceType.PlayerKill, new PlayerKillAnnounceInfo
			{
				deadCharacterCode = deadCharacter.CharacterCode,
				deadPlayerObjectId = deadCharacter.ObjectId,
				killCharacterCode = worldPlayerCharacter.CharacterCode,
				killPlayerObjectId = worldPlayerCharacter.ObjectId,
				killCount = worldPlayerCharacter.Status.PlayerKill,
				killWeaponType = killWeaponType,
				trapKill = (attackerDamageSubType == DamageSubType.Trap),
				aliveCount = MonoBehaviourInstance<GameService>.inst.Player.AlivePlayerCharacterCount() + MonoBehaviourInstance<GameService>.inst.Bot.GetAliveBotCharacterCount(),
				assistants = assistants
			});
		}

		
		private void BroadcastAnnounce<T>(GameAnnounceType type, T packet)
		{
			this.server.Broadcast(new RpcGameAnnounce
			{
				announceType = type,
				announceInfo = Serializer.Default.Serialize<T>(packet)
			}, NetChannel.ReliableOrdered);
		}

		
		private void BroadcastAnnounce(GameAnnounceType type)
		{
			this.server.Broadcast(new RpcGameAnnounce
			{
				announceType = type
			}, NetChannel.ReliableOrdered);
		}

		
		public void WicklineMoveArea(int areaCode)
		{
			this.server.Broadcast(new RpcNoticeWicklineMoveArea
			{
				areaCode = areaCode
			}, NetChannel.ReliableOrdered);
		}

		
		public void WicklineKilled(int pAttackerId)
		{
			this.server.Broadcast(new RpcNoticeWicklineKilled
			{
				attackerObjectId = pAttackerId
			}, NetChannel.ReliableOrdered);
		}

		
		private bool HaveNoiseIgnoreState(WorldObject creatorObject, WorldObject assistObject, NoiseType noiseType)
		{
			if (CharacterStateDB.NoiseIgnoreCode == null)
			{
				return false;
			}
			if (!CharacterStateDB.NoiseIgnoreCode.ContainsKey(noiseType))
			{
				return false;
			}
			switch (noiseType)
			{
			case NoiseType.None:
				return false;
			case NoiseType.BasicHit:
			case NoiseType.MonsterKilled:
			case NoiseType.PlayerKilled:
				return !(assistObject == null) && assistObject.SkillAgent != null && assistObject.SkillAgent.AnyHaveStateByGroup(CharacterStateDB.NoiseIgnoreCode[noiseType]);
			case NoiseType.Gunshot:
			case NoiseType.FixedBoxOpen:
			case NoiseType.AirSupplyOpen:
			case NoiseType.CollectibleOpen:
			case NoiseType.TrapHit:
			case NoiseType.Crafting:
			case NoiseType.FootstepSoil:
			case NoiseType.FootstepWater:
			case NoiseType.FootstepWood:
			case NoiseType.FootstepConcrete:
			case NoiseType.FootstepMetal:
			case NoiseType.FootstepAsphalt:
			case NoiseType.FootstepGrass:
			case NoiseType.HyperLoopExit:
				if (creatorObject == null)
				{
					Log.E("creatorObject is null!!");
					return true;
				}
				if (creatorObject.SkillAgent == null)
				{
					Log.E("creatorObject.SkillAgent is null!!");
					return true;
				}
				return creatorObject.SkillAgent.AnyHaveStateByGroup(CharacterStateDB.NoiseIgnoreCode[noiseType]);
			default:
				return false;
			}
		}

		
		private const float NOISE_FREQUENCY = 2f;

		
		private readonly NoiseHistory noiseHistory = new NoiseHistory();

		
		private readonly Collider[] colliderBuffer = new Collider[100];
	}
}
