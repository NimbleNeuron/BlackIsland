using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdConsoleCheckOut, false)]
	public class CmdConsoleCheckOut : LocalSecurityConsoleCommandPacket
	{
		[Key(1)] public int playerCharacterId;


		public override void Action(ClientService service, LocalSecurityConsole self)
		{
			self.CheckoutConsole(playerCharacterId);
			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				MonoBehaviourInstance<TutorialController>.inst.RunCCTVTutorial();
			}
		}
	}
}