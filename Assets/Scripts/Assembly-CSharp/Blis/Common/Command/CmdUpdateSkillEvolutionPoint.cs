using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdUpdateSkillEvolutionPoint, false)]
	public class CmdUpdateSkillEvolutionPoint : CommandPacket
	{
		
		[Key(1)] public int currentPoint;

		
		[Key(0)] public SkillEvolutionPointType pointType;

		
		public override void Action(ClientService service)
		{
			service.MyPlayer.UpdateSkillEvolutionPoint(pointType, currentPoint);
		}
	}
}