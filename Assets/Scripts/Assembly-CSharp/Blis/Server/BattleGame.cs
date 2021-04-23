using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using Newtonsoft.Json;

namespace Blis.Server
{
	
	public class BattleGame
	{
		public string battleHost;
		public string battleTokenKey;
		
		[JsonConverter(typeof(MicrosecondEpochConverter))]
		public DateTime endDtm;
		
		public long id;
		public MatchingMode matchingMode;
		public MatchingTeamMode matchingTeamMode;
		public int seasonId;
		
		[JsonConverter(typeof(MicrosecondEpochConverter))]
		public DateTime startDtm;
		public List<long> userIds;
		
		public string vip;

		public BattleGame(string battleTokenKey, BattleToken battleToken)
		{
			id = battleToken.gameId;
			seasonId = battleToken.seasonId;
			matchingMode = battleToken.matchingMode;
			matchingTeamMode = battleToken.matchingTeamMode;
			this.battleTokenKey = battleTokenKey;
			battleHost = battleToken.host;
			vip = battleToken.vipHost;
			startDtm = DateTime.MaxValue;
			endDtm = DateTime.MaxValue;
			userIds = new List<long>();
		}

		public GameStatus gameStatus => MonoBehaviourInstance<GameService>.inst.GameStatus;
	}
}