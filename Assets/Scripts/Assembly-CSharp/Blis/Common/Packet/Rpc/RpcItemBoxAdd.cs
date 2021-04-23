using Blis.Client;
using Blis.Client.UI;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcItemBoxAdd, false)]
	public class RpcItemBoxAdd : RpcPacket
	{
		[Key(0)] public Item item;


		public override void Action(ClientService clientService)
		{
			UISystem.Action(new UpdateBox
			{
				addedItems = new[]
				{
					item
				}
			});
		}
	}
}