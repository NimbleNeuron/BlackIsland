using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcJoinUser, false)]
	public class RpcJoinUser : RpcPacket
	{
		[Key(4)] public SnapshotWrapper characterSnapshot;


		[Key(2)] public bool isObserver;


		[Key(1)] public string nickname;


		[Key(5)] public byte[] playerSnapshot;


		[Key(3)] public int startingWeaponCode;


		[Key(0)] public long userId;


		public override void Action(ClientService clientService)
		{
			if (isObserver)
			{
				clientService.CreateObserver(userId, nickname, playerSnapshot, characterSnapshot);
				return;
			}

			clientService.CreatePlayerCharacter(userId, nickname, startingWeaponCode, playerSnapshot,
				characterSnapshot);
		}
	}
}