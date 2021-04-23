using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqUpgradeSkill, false)]
	public class ReqUpgradeSkill : ReqPacket
	{
		[Key(0)] public SkillSlotIndex skillSlotIndex;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			if (playerSession.Character.CanSkillUpgrade(skillSlotIndex))
			{
				playerSession.Character.UpgradeSkill(skillSlotIndex);
			}
		}
	}
}