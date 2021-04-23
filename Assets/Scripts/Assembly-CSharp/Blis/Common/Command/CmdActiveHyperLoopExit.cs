using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdActiveHyperLoopExit, false)]
	public class CmdActiveHyperLoopExit : CommandPacket
	{
		
		[Key(0)] public BlisVector destination;

		
		public override void Action(ClientService service)
		{
			MonoBehaviourInstance<EnvironmentEffectManager>.inst.InvokeEvent<WhiteHoleEffect>(destination.ToVector3(),
				1f, "tHyperLoopDoor_Work");
		}
	}
}