using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdUpdateSkillPoint, false)]
	public class CmdUpdateSkillPoint : CommandPacket
	{
		
		[Key(0)] public int skillPoint;

		
		public override void Action(ClientService service)
		{
			service.MyPlayer.UpdateSkillPoint(skillPoint);
		}
	}
}