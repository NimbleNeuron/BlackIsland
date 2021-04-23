using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdAddItemCooldown, false)]
	public class CmdAddItemCooldown : CommandPacket
	{
		[Key(1)] public BlisFixedPoint cooldown;


		[Key(0)] public int itemCode;


		public override void Action(ClientService service)
		{
			ItemData itemData = GameDB.item.FindItemByCode(itemCode);
			service.MyPlayer.SetItemCooldown(itemData, cooldown.Value);
		}
	}
}