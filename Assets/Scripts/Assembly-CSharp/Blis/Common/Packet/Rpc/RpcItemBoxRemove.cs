using Blis.Client;
using Blis.Client.UI;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcItemBoxRemove, false)]
	public class RpcItemBoxRemove : RpcPacket
	{
		[Key(0)] public int itemId;


		public override void Action(ClientService clientService)
		{
			UISystem.Action(new UpdateBox
			{
				removedItemIds = new[]
				{
					itemId
				}
			});
		}
	}
}