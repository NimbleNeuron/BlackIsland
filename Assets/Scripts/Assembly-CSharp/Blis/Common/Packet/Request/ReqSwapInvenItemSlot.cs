using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.ReqSwapInvenItemSlot, false)]
	public class ReqSwapInvenItemSlot : ReqPacket
	{
		
		[Key(0)] public int indexA;

		
		[Key(1)] public int indexB;

		
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			playerSession.Character.SwapInventoryItem(indexA, indexB);
			playerSession.Character.SendInventoryUpdate(UpdateInventoryType.Swap);
		}
	}
}