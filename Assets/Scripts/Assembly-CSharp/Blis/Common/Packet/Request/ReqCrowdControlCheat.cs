using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.ReqCrowdControlCheat, false)]
	public class ReqCrowdControlCheat : ReqPacket
	{
		
		[Key(3)] public string codeKey;

		
		[Key(1)] public float duration;

		
		[Key(2)] public bool spawnDummy;

		
		[Key(0)] public int stateCode;

		
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession) { }
	}
}