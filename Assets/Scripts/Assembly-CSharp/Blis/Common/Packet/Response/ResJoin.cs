using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResJoin, false)]
	public class ResJoin : ResPacket
	{
		[Key(6)] public SnapshotWrapper character;


		[Key(9)] public long gameId;


		[Key(3)] public bool isObserver;


		[Key(2)] public string nickname;


		[Key(8)] public byte[] playerSnapshot;


		[Key(7)] public List<SnapshotWrapper> snapshot;


		[Key(4)] public int startingWeaponCode;


		[Key(1)] public long userId;


		[Key(5)] public List<UserInfo> userList;
	}
}