using System.Collections.Generic;
using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcUpdateEquipment, false)]
	public class RpcUpdateEquipment : LocalPlayerCharacterRpcPacket
	{
		[Key(1)] public List<EquipItem> updates;


		public override void Action(ClientService clientService, LocalPlayerCharacter self)
		{
			MyPlayerContext myPlayer = clientService.MyPlayer;
			if (myPlayer != null)
			{
				myPlayer.StopBulletCooldownIds();
			}

			self.OnUpdateEquipment(updates);
		}
	}
}