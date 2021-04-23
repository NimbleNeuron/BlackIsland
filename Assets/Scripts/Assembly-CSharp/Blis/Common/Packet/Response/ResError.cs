using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResError, false)]
	public class ResError : ResPacket
	{
		[Key(1)] public ErrorType errorType;


		[Key(2)] public string msg;
	}
}