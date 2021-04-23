using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqTarget, false)]
	public class ReqTarget : ReqPacket
	{
		[Key(0)] public int targetId;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			WorldObject worldObject = null;
			if (!gameService.World.TryFind<WorldObject>(targetId, ref worldObject))
			{
				throw new GameException(ErrorType.ObjectNotFound);
			}

			if (playerSession.Character.IsOutSight(targetId))
			{
				return;
			}

			if (playerSession.Character.SkillController.OnlyMoveInputWhileSkillPlaying())
			{
				playerSession.Character.Controller.MoveTo(worldObject.GetPosition(), false);
				return;
			}

			if (playerSession.Character.IsRest)
			{
				return;
			}

			if (playerSession.Character.IsRunningCastingAction())
			{
				playerSession.Character.CancelActionCasting(CastingCancelType.Action);
			}

			playerSession.Character.Controller.TargetOn(worldObject);
		}
	}
}