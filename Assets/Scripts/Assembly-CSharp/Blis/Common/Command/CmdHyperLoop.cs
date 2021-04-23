using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdHyperLoop, false)]
	public class CmdHyperLoop : LocalPlayerCharacterCommandPacket
	{
		[Key(1)] public BlisVector destination;


		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.WarpTo(destination);
			self.SetCharacterAnimatorTrigger("tTeleportArrive");
			if (service.MyObjectId == self.ObjectId)
			{
				MonoBehaviourInstance<MobaCamera>.inst.SetCameraPosition(self.GetPosition(), 0f);
			}
		}
	}
}