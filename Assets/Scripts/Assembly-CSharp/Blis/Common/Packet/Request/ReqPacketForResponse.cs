using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	
	[Union(0, typeof(ReqMissingCommand))]
	[Union(1, typeof(Handshake))]
	[Union(2, typeof(TesterHandshake))]
	[Union(3, typeof(ReqJoin))]
	[Union(4, typeof(ReqUseTargetSkill))]
	[Union(5, typeof(ReqUsePointSkill))]
	[Union(6, typeof(ReqStop))]
	[Union(7, typeof(ReqPickUpItem))]
	[Union(8, typeof(ReqOpenCorpse))]
	[Union(9, typeof(ReqCloseBox))]
	[Union(10, typeof(ReqUseItem))]
	[Union(11, typeof(ReqWalkableArea))]
	[Union(12, typeof(ReqAskGameSetup))]
	[Union(13, typeof(ReqAskGameStart))]
	[Union(14, typeof(ReqGameSnapshot))]
	[Union(15, typeof(ReqNicknamePair))]
	[Union(16, typeof(ReqEquipItem))]
	[MessagePackObject(false)]
	public abstract class ReqPacketForResponse : IPacket
	{
		
		public abstract ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession);

		
		public virtual ResPacket Action(GameServer gameServer, GameService gameService, ObserverSession observerSession)
		{
			return null;
		}

		
		[IgnoreMember]
		protected const int LAST_KEY_IDX = 0;

		
		[Key(0)]
		public uint reqId;
	}
}
