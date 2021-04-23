using System.Collections.Generic;
using Blis.Common.Utils;
using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqIgnorePlayer, false)]
	public class ReqIgnorePlayer : ReqPacketForResponse
	{
		[Key(2)] public IgnoreType ignoreType;


		[Key(3)] public bool isChangeFlag;


		[Key(1)] public List<int> targetObjectIds;


		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			MonoBehaviourInstance<GameService>.inst.IgnoreTargetService.OnSwitchFlag(playerSession.Character.ObjectId,
				targetObjectIds, ignoreType, isChangeFlag);
			return new ResIgnorePlayer
			{
				isIgnore = isChangeFlag
			};
		}
	}
}