using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	public class PlayerSession : Session
	{
		
		
		public PlayerStartingSettings StartingSettings
		{
			get
			{
				return this.startingSettings;
			}
		}

		
		
		public WorldPlayerCharacter Character
		{
			get
			{
				return this.character;
			}
		}

		
		
		public new int TeamNumber
		{
			get
			{
				return this.teamNumber;
			}
		}

		
		
		public bool IsObserving
		{
			get
			{
				return this.isObserving;
			}
		}

		
		
		public DateTime ObservingStartTime
		{
			get
			{
				return this.observingStartTime;
			}
		}

		
		
		public int TeamMemberNumber
		{
			get
			{
				return this.teamMembers.Count;
			}
		}

		
		
		public int OpenBoxId
		{
			get
			{
				return this.openBoxId;
			}
		}

		
		
		public int CalPlayTime
		{
			get
			{
				return this.calPlayTime;
			}
		}

		
		
		public int CalWatchTime
		{
			get
			{
				return this.calWatchTime;
			}
		}

		
		
		public int CalTotalTime
		{
			get
			{
				return this.calTotalTime;
			}
		}

		
		
		public bool IsSurrender
		{
			get
			{
				return this.isSurrender;
			}
		}

		
		public PlayerSession(int connectionId, long userId, string nickname, int characterId, int weaponCode, int skinCode, int teamNumber, int teamMmr, int privateMmr, int matchCount, int preMade, List<int> missionList, Dictionary<EmotionPlateType, int> emotion, string ip, string zipCode, string country, string countryCode, string isp) : base(connectionId, userId, nickname)
		{
			this.characterId = characterId;
			this.skinCode = skinCode;
			this.teamMmr = teamMmr;
			this.privateMmr = privateMmr;
			this.matchCount = matchCount;
			this.preMade = preMade;
			this.startingWeaponCode = weaponCode;
			this.startingSettings = new PlayerStartingSettings();
			if (0 < weaponCode)
			{
				ItemData itemData = GameDB.item.FindItemByCode(weaponCode);
				MasteryType masteryType = itemData.GetMasteryType();
				this.startingSettings.SetStartingEquipment(itemData);
				RecommendStarting recommendStarting = GameDB.recommend.FindStartingData(characterId, masteryType);
				this.startingSettings.SetStartItemGroupCode((recommendStarting != null) ? recommendStarting.startItemGroupCode : 0);
			}
			this.teamNumber = teamNumber;
			this.teamMembers = new List<PlayerSession>();
			this.teamCharacters = new List<WorldPlayerCharacter>();
			this.missionList = new List<MissionData>();
			this.missionResultMap = new Dictionary<int, int>();
			foreach (int key in missionList)
			{
				MissionData missionData = GameDB.mission.GetMissionData(key);
				this.missionList.Add(missionData);
				this.missionResultMap.Add(missionData.key, 0);
			}
			this.killer = "";
			this.killerDetail = "";
			this.causeOfDeath = "";
			this.logCompleted = false;
			this.isSurrender = false;
			this.emotion = emotion;
			this.IpAddress = ip;
			this.ZipCode = zipCode;
			this.Country = country;
			this.CountryCode = countryCode;
			this.ISP = isp;
		}

		
		public void SetCharacter(WorldPlayerCharacter character)
		{
			base.SetWorldObject(character);
			this.character = character;
			this.character.SetSession(this);
		}

		
		public void ClearTeam()
		{
			this.teamMembers.Clear();
		}

		
		public void AddTeamMember(PlayerSession teamMember)
		{
			if (!this.teamMembers.Contains(teamMember))
			{
				this.teamMembers.Add(teamMember);
				this.teamCharacters.Add(teamMember.Character);
			}
		}

		
		public void EnqueueTeamMemberCommand(PacketWrapper commandPacketWrapper)
		{
			base.EnqueueCommandPacket(commandPacketWrapper);
			for (int i = 0; i < this.teamMembers.Count; i++)
			{
				this.teamMembers[i].EnqueueCommandPacket(commandPacketWrapper);
			}
		}

		
		public void BroadcastTeamMember(RpcPacket packet, NetChannel netChannel = NetChannel.ReliableOrdered)
		{
			MonoBehaviourInstance<GameServer>.inst.Send(this, packet, netChannel);
			for (int i = 0; i < this.teamMembers.Count; i++)
			{
				MonoBehaviourInstance<GameServer>.inst.Send(this.teamMembers[i], packet, netChannel);
			}
		}

		
		public void BroadcastTarget(RpcPacket packet, NetChannel netChannel = NetChannel.ReliableOrdered)
		{
			MonoBehaviourInstance<GameServer>.inst.Send(this, packet, netChannel);
		}

		
		public List<WorldPlayerCharacter> GetTeamCharacters()
		{
			if (this.teamMembers.Count != this.teamCharacters.Count)
			{
				this.teamCharacters.Clear();
				foreach (PlayerSession playerSession in this.teamMembers)
				{
					this.teamCharacters.Add(playerSession.Character);
				}
			}
			return this.teamCharacters;
		}

		
		public void SettingObserving()
		{
			this.isObserving = true;
			this.observingStartTime = DateTime.Now;
		}

		
		public void ExitTeamGame(bool openGameResult)
		{
			this.isObserving = false;
			this.Character.ExitTeamGame(openGameResult);
		}

		
		public void ExitTeamGame()
		{
			this.isObserving = false;
		}

		
		public void EnqueueCommandTeamOnly(PacketWrapper packetWrapper)
		{
			MonoBehaviourInstance<GameServer>.inst.EnqueueCommandTeamOnly(this, packetWrapper);
		}

		
		public void EnqueueCommandTeamAndObserver(PacketWrapper packetWrapper)
		{
			MonoBehaviourInstance<GameServer>.inst.EnqueueCommandTeamAndObserver(this, packetWrapper);
		}

		
		public void SetOpenBoxId(int openBoxId)
		{
			this.openBoxId = openBoxId;
		}

		
		protected override List<PacketWrapper> processSight(List<PacketWrapper> originalCommandQueue)
		{
			this.commandQueue.Clear();
			foreach (PacketWrapper packetWrapper in originalCommandQueue)
			{
				PacketType packetType = packetWrapper.packetType;
				if (packetType <= PacketType.CmdMoveByDirection)
				{
					if (packetType - PacketType.CmdWarpTo > 1 && packetType - PacketType.CmdMoveStraight > 2)
					{
						goto IL_7A;
					}
				}
				else if (packetType - PacketType.CmdMoveInCurve > 2 && packetType != PacketType.CmdHyperLoop)
				{
					goto IL_7A;
				}
				MatchingMode matchingMode = MonoBehaviourInstance<GameService>.inst.MatchingMode;
				if (!matchingMode.IsStandaloneMode() && !matchingMode.IsTutorialMode())
				{
					int objectId = packetWrapper.GetPacket<ObjectCommandPacket>().objectId;
					if (this.Character.IsOutSight(objectId))
					{
						continue;
					}
				}
				IL_7A:
				this.commandQueue.Add(packetWrapper);
			}
			return this.commandQueue;
		}

		
		public void SetSurrender()
		{
			this.isSurrender = true;
		}

		
		
		public int MinLatency
		{
			get
			{
				return this.minLatency;
			}
		}

		
		
		public int MaxLatency
		{
			get
			{
				return this.maxLatency;
			}
		}

		
		
		public int AvgLatency
		{
			get
			{
				if (this.recvCount <= 0)
				{
					return 0;
				}
				return this.sumLatency / this.recvCount;
			}
		}

		
		
		
		public string killer { get; set; }

		
		
		
		public string killerDetail { get; set; }

		
		
		
		public string causeOfDeath { get; set; }

		
		
		
		public string Identifier { get; set; }

		
		
		public string IpAddress { get; }

		
		
		public string Country { get; }

		
		
		
		public string Language { get; private set; }

		
		
		public string CountryCode { get; }

		
		
		public string ISP { get; }

		
		
		public string ZipCode { get; }

		
		public void SetLanguage(string language)
		{
			this.Language = language;
		}

		
		public void UpdateLatency(int latency)
		{
			if (latency > 0)
			{
				if (this.minLatency > latency)
				{
					this.minLatency = latency;
				}
				if (this.maxLatency < latency)
				{
					this.maxLatency = latency;
				}
			}
			this.recvCount++;
			this.sumLatency += latency;
		}

		
		public void CalculatePlayTime(DateTime gameStartTime)
		{
			this.calPlayTime = 0;
			this.calWatchTime = 0;
			this.calTotalTime = 0;
			if (this.ObservingStartTime == DateTime.MinValue)
			{
				this.calPlayTime = (int)(DateTime.Now - gameStartTime).TotalSeconds;
				this.calTotalTime = this.calPlayTime;
				return;
			}
			this.calPlayTime = (int)(this.ObservingStartTime - gameStartTime).TotalSeconds;
			this.calWatchTime = (int)(DateTime.Now - this.ObservingStartTime).TotalSeconds;
			this.calTotalTime = this.calWatchTime + this.calPlayTime;
		}

		
		public readonly int characterId;

		
		public readonly int startingWeaponCode;

		
		public readonly int skinCode;

		
		public readonly int teamMmr;

		
		public readonly int preMade;

		
		public readonly int privateMmr;

		
		public readonly int matchCount;

		
		public readonly Dictionary<int, int> missionResultMap;

		
		public readonly List<MissionData> missionList;

		
		public readonly Dictionary<EmotionPlateType, int> emotion;

		
		private PlayerStartingSettings startingSettings;

		
		private WorldPlayerCharacter character;

		
		private int teamNumber;

		
		private bool isObserving;

		
		private DateTime observingStartTime;

		
		private readonly List<PlayerSession> teamMembers;

		
		private readonly List<WorldPlayerCharacter> teamCharacters;

		
		private int openBoxId;

		
		private int calPlayTime;

		
		private int calWatchTime;

		
		private int calTotalTime;

		
		private bool isSurrender;

		
		public bool logCompleted;

		
		private readonly List<PacketWrapper> commandQueue = new List<PacketWrapper>();

		
		private int minLatency = int.MaxValue;

		
		private int maxLatency = int.MinValue;

		
		private int sumLatency;

		
		private int recvCount;
	}
}
