using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdInstallRopeTrap, false)]
	public class CmdInstallRopeTrap : LocalSummonTrapCommandPacket
	{
		[Key(2)] public SkillId skillId;


		[Key(1)] public int targetId;


		public override void Action(ClientService service, LocalSummonTrap self)
		{
			LocalObject target = service.World.Find<LocalObject>(targetId);
			self.InstallRopeTrap(target, skillId);
		}
	}
}