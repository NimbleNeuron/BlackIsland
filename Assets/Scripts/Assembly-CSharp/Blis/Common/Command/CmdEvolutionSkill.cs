using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdEvolutionSkill, false)]
	public class CmdEvolutionSkill : CommandPacket
	{
		[Key(2)] public int currentPoint;


		[Key(1)] public SkillEvolutionPointType pointType;


		[Key(0)] public SkillSlotIndex skillSlotIndex;


		public override void Action(ClientService service)
		{
			service.MyPlayer.UpdateSkillEvolutionPoint(pointType, currentPoint);
			service.MyPlayer.EvolutionSkill(skillSlotIndex);
		}
	}
}