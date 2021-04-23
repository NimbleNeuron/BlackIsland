using System;
using System.Collections.Generic;
using Blis.Common;

namespace Blis.Client.UI
{
	public struct PlayerInfo
	{
		public long userId;
		public int objectId;
		public int rank;
		public bool isInSight;
		public string name;
		public int characterCode;
		public List<Item> equipment;
		public bool isAlive;
		public bool isDyingCondition;
		public bool isObserving;
		public bool isDisconnected;
		public int teamNumber;
		public int teamSlot;
		public int level;
		public int combatLevel;
		public int searchLevel;
		public int growthLevel;
		public Dictionary<MasteryType, int> masterysLevel;
		public int playerKill;
		public int playerKillAssist;
		public int monsterKill;
		public DateTime updateDtm;
		public bool isUseTempNickname;
		public NicknamePair nicknamePair;
	}
}