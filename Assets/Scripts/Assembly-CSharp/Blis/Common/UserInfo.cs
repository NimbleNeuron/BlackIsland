using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class UserInfo
	{
		[Key(0)] public SnapshotWrapper characterSnapshot;


		[Key(2)] public string nickname;


		[Key(4)] public byte[] playerSnapshot;


		[Key(3)] public int startingWeaponCode;


		[Key(1)] public long userId;
	}
}