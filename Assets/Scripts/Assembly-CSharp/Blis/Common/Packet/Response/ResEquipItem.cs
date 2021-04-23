using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResEquipItem, false)]
	public class ResEquipItem : ResPacket
	{
		[Key(1)] public int errorCode = -1;
	}
}