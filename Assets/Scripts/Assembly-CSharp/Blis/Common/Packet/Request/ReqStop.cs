using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqStop, true)]
	public class ReqStop : ReqPacketForResponse
	{
		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			if (playerSession.Character.IsRest)
			{
				return new ResStop();
			}

			if (!playerSession.Character.CanMove())
			{
				return new ResStop();
			}

			if (!playerSession.Character.CanStop())
			{
				return new ResStop();
			}

			bool cancelNormalAttack = playerSession.Character.SkillController.CancelNormalAttack();
			playerSession.Character.Controller.Stop();
			return new ResStop
			{
				cancelNormalAttack = cancelNormalAttack
			};
		}
	}
}