using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdCancelHyperLoopExit, false)]
	public class CmdCancelHyperLoopExit : CommandPacket
	{
		
		[Key(0)] public BlisVector destination;

		
		public override void Action(ClientService service)
		{
			MonoBehaviourInstance<EnvironmentEffectManager>.inst.InvokeEvent<WhiteHoleEffect>(destination.ToVector3(),
				1f, "Cancel");
		}
	}
}