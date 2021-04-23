using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqEvolutionSkill, false)]
	public class ReqEvolutionSkill : ReqPacket
	{
		[Key(0)] public SkillSlotIndex skillSlotIndex;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			if (playerSession.Character.CanSkillEvolution(skillSlotIndex))
			{
				playerSession.Character.EvolutionSkill(skillSlotIndex);
			}
		}
	}
}