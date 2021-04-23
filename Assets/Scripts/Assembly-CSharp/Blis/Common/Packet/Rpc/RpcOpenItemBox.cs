using System.Collections.Generic;
using Blis.Client;
using Blis.Client.UI;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcOpenItemBox, false)]
	public class RpcOpenItemBox : RpcPacket
	{
		[Key(1)] public List<Item> items;


		[Key(0)] public int targetId;


		public override void Action(ClientService clientService)
		{
			LocalItemBox localItemBox = null;
			if (!clientService.World.TryFind<LocalItemBox>(targetId, ref localItemBox))
			{
				return;
			}

			if (localItemBox is LocalAirSupplyItemBox)
			{
				SingletonMonoBehaviour<PlayerController>.inst.OpenBox(BoxWindowType.AirSupply, localItemBox, items);
				return;
			}

			SingletonMonoBehaviour<PlayerController>.inst.OpenBox(BoxWindowType.Box, localItemBox, items);
		}
	}
}