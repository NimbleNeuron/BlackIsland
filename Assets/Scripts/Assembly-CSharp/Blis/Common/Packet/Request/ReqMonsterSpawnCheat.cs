using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.ReqMonsterSpawnCheat, false)]
	public class ReqMonsterSpawnCheat : ReqPacket
	{
		
		[Key(1)] public int level;

		
		[Key(0)] public MonsterType monsterType;

		
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession) { }
	}
}