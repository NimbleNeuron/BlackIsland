using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdRemoveItemCooldown, false)]
	public class CmdRemoveItemCooldown : CommandPacket
	{
		
		[Key(0)] public int itemCode;

		
		public override void Action(ClientService service)
		{
			ItemData itemData = GameDB.item.FindItemByCode(itemCode);
			service.MyPlayer.RemoveItemCooldown(itemData);
		}
	}
}