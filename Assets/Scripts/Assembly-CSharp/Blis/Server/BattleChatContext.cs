using System;
using System.Collections.Generic;
using Blis.Common;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Blis.Server
{
	
	public class BattleChatContext
	{
		[JsonProperty]
		public long gameId { get; private set; }

		[JsonProperty]
		[JsonConverter(typeof(MicrosecondEpochConverter))]
		public DateTime startDtm { get; private set; }

		[JsonProperty]
		[JsonConverter(typeof(MicrosecondEpochConverter))]
		public DateTime endDtm { get; private set; }
		
		[JsonProperty]
		public string battleTokenKey { get; private set; }

		[JsonProperty]
		public string terminatedReason { get; private set; }
		
		[JsonProperty]
		public List<BattleChatMember> members { get; private set; }

		[JsonProperty]
		public List<BattleChatMessage> messages { get; private set; }
		
		public BattleChatContext(long gameId, DateTime startDtm, DateTime endDtm, string battleTokenKey, string terminatedReason, List<BattleChatMember> members, [NotNull] List<BattleChatMessage> messages)
		{
			this.gameId = gameId;
			this.startDtm = startDtm;
			this.endDtm = endDtm;
			this.battleTokenKey = battleTokenKey;
			this.terminatedReason = terminatedReason;
			this.members = members;
			this.messages = messages;
		}
	}
}
