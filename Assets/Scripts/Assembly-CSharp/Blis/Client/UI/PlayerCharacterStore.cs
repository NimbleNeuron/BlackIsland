using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;

namespace Blis.Client.UI
{
	[UIActionMapping(typeof(UpdatePlayerInfo))]
	public class PlayerCharacterStore : UIStore<PlayerCharacterStore>
	{
		private readonly Dictionary<int, PlayerInfo> lastPlayerInfos = new Dictionary<int, PlayerInfo>();


		private readonly Dictionary<int, PlayerInfo> playerInfos = new Dictionary<int, PlayerInfo>();


		public IEnumerable<PlayerInfo> PlayerInfos => playerInfos.Values;


		public IEnumerable<PlayerInfo> LastPlayerInfos => lastPlayerInfos.Values;


		protected override void ActionHandle(UIAction action)
		{
			action.IfTypeIs<UpdatePlayerInfo>(delegate(UpdatePlayerInfo data)
			{
				PlayerInfo playerInfo2;
				if (!playerInfos.ContainsKey(data.objectId))
				{
					PlayerInfo playerInfo = default;
					playerInfo.userId = data.userId;
					playerInfo.objectId = data.objectId;
					playerInfo.teamNumber = data.teamNumber;
					playerInfo.teamSlot = data.teamSlot;
					playerInfo.rank = data.rank;
					playerInfo.isInSight = data.isInSight ?? false;
					playerInfo.name = data.name;
					playerInfo.characterCode = data.characterCode ?? 0;
					playerInfo.level = data.level ?? 0;
					List<Item> equipment = data.equipment;
					playerInfo.equipment = equipment != null ? equipment.ToList<Item>() : null;
					playerInfo.isAlive = data.isAlive ?? true;
					playerInfo.combatLevel = data.combatLevel ?? 0;
					playerInfo.searchLevel = data.searchLevel ?? 0;
					playerInfo.growthLevel = data.growthLevel ?? 0;
					playerInfo.masterysLevel = data.masterysLevel;
					playerInfo.playerKill = data.playerKill ?? 0;
					playerInfo.playerKillAssist = data.playerKillAssist ?? 0;
					playerInfo.monsterKill = data.monsterKill ?? 0;
					playerInfo.updateDtm = DateTime.Now;
					playerInfo2 = playerInfo;
				}
				else
				{
					playerInfo2 = playerInfos[data.objectId];
					playerInfo2.userId = data.userId;
					playerInfo2.teamNumber = data.teamNumber;
					playerInfo2.teamSlot = data.teamSlot;
					playerInfo2.rank = data.rank;
					playerInfo2.isInSight = data.isInSight ?? playerInfo2.isInSight;
					playerInfo2.name = data.name ?? playerInfo2.name;
					playerInfo2.characterCode = data.characterCode ?? playerInfo2.characterCode;
					playerInfo2.level = data.level ?? playerInfo2.level;
					playerInfo2.equipment =
						data.equipment != null ? data.equipment.ToList<Item>() : playerInfo2.equipment;
					playerInfo2.isAlive = data.isAlive ?? playerInfo2.isAlive;
					playerInfo2.isDyingCondition = data.isDyingCondition ?? playerInfo2.isDyingCondition;
					playerInfo2.isObserving = data.isObserving ?? playerInfo2.isObserving;
					playerInfo2.isDisconnected = data.isDisconnected ?? playerInfo2.isDisconnected;
					playerInfo2.combatLevel = data.combatLevel ?? playerInfo2.combatLevel;
					playerInfo2.searchLevel = data.searchLevel ?? playerInfo2.searchLevel;
					playerInfo2.growthLevel = data.growthLevel ?? playerInfo2.growthLevel;
					playerInfo2.masterysLevel = data.masterysLevel ?? playerInfo2.masterysLevel;
					playerInfo2.playerKill = data.playerKill ?? playerInfo2.playerKill;
					playerInfo2.playerKillAssist = data.playerKillAssist ?? playerInfo2.playerKillAssist;
					playerInfo2.monsterKill = data.monsterKill ?? playerInfo2.monsterKill;
					playerInfo2.updateDtm = DateTime.Now;
					playerInfo2.isUseTempNickname = data.isUseTempNickname ?? playerInfo2.isUseTempNickname;
					playerInfo2.nicknamePair = data.nickNamePair ?? playerInfo2.nicknamePair;
				}

				playerInfos[data.objectId] = playerInfo2;
				GlobalUserData.dicPlayerResults[data.objectId] = playerInfo2;
				if (playerInfo2.isInSight)
				{
					lastPlayerInfos[data.objectId] = playerInfos[data.objectId];
					return;
				}

				if (!playerInfo2.isAlive && lastPlayerInfos.ContainsKey(data.objectId))
				{
					lastPlayerInfos[data.objectId] = playerInfos[data.objectId];
				}
			});
		}


		protected override void PreCommit() { }


		public override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			playerInfos.Clear();
			lastPlayerInfos.Clear();
		}
	}
}