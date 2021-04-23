using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdUpdateMark, false)]
	public class CmdUpdateMarkTarget : CommandPacket
	{
		
		[Key(0)] public BlisVector[] marks;

		
		public override void Action(ClientService service)
		{
			SingletonMonoBehaviour<PlayerController>.inst.PingTarget.UpdateMarkTarget(marks);
		}
	}
}