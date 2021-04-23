using System.Collections.Generic;
using Blis.Common;

namespace Blis.Client.UI
{
	public class UpdatePlayerInfo : UIAction
	{
		public int? characterCode;


		public int? combatLevel;


		public List<Item> equipment;


		public int? growthLevel;


		public bool? isAlive;


		public bool? isDisconnected;


		public bool? isDyingCondition;


		public bool? isInSight;


		public bool? isObserving;


		public bool? isUseTempNickname;


		public int? level;


		public Dictionary<MasteryType, int> masterysLevel;


		public int? monsterKill;


		public string name;


		public NicknamePair nickNamePair;


		public int objectId;


		public int? playerKill;


		public int? playerKillAssist;


		public int rank;


		public int? searchLevel;


		public int teamNumber;


		public int teamSlot;


		public long userId;

		public UpdatePlayerInfo(int objectId, int teamNumber, int teamSlot, int rank, long userId)
		{
			this.objectId = objectId;
			this.teamNumber = teamNumber;
			this.teamSlot = teamSlot;
			this.rank = rank;
			this.userId = userId;
		}


		public UpdatePlayerInfo(LocalPlayerCharacter localCharacter)
		{
			userId = localCharacter.PlayerContext.userId;
			objectId = localCharacter.ObjectId;
			teamNumber = localCharacter.TeamNumber;
			teamSlot = localCharacter.TeamSlot;
			rank = localCharacter.Rank;
			name = localCharacter.Nickname;
			characterCode = localCharacter.CharacterCode;
			level = localCharacter.Status.Level;
			equipment = localCharacter.GetEquipments();
			isAlive = localCharacter.IsAlive;
			isDyingCondition = localCharacter.IsDyingCondition;
			isObserving = localCharacter.IsObserving;
			isDisconnected = localCharacter.IsDisconnected;
			combatLevel = localCharacter.GetHighestMasteryCategoryLevel(MasteryCategory.Combat);
			searchLevel = localCharacter.GetMasteryCategoryLevel(MasteryCategory.Search);
			growthLevel = localCharacter.GetMasteryCategoryLevel(MasteryCategory.Growth);
			masterysLevel = localCharacter.MasterysLevel;
			playerKill = localCharacter.Status.PlayerKill;
			monsterKill = localCharacter.Status.MonsterKill;
			playerKillAssist = localCharacter.Status.PlayerKillAssist;
		}


		public UpdatePlayerInfo(int objectId, int teamNumber, int teamSlot, int rank, long userId,
			bool isUseTempNickname, NicknamePair nicknamePair)
		{
			this.objectId = objectId;
			this.teamNumber = teamNumber;
			this.teamSlot = teamSlot;
			this.rank = rank;
			this.userId = userId;
			this.isUseTempNickname = isUseTempNickname;
			nickNamePair = nicknamePair;
		}
	}
}