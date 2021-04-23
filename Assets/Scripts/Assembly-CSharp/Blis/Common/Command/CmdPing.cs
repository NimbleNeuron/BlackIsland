using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdPing, false)]
	public class CmdPing : CommandPacket
	{
		[Key(1)] public int pingObjectId;


		[Key(3)] public BlisVector pingPosition;


		[Key(2)] public int senderObjectId;


		[Key(4)] public int teamSlot;


		[Key(0)] public PingType type;


		public override void Action(ClientService service)
		{
			SingletonMonoBehaviour<PlayerController>.inst.PingTarget.AddPingTarget(type, pingObjectId,
				pingPosition.ToVector3(), senderObjectId);
		}
	}
}