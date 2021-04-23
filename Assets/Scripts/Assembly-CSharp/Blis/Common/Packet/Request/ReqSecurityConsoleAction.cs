using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.ReqSecurityConsoleAction, false)]
	public class ReqSecurityConsoleAction : ReqPacket
	{
		
		[Key(1)] public SecurityConsoleEvent eventType;

		
		[Key(0)] public int targetId;

		
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			WorldSecurityConsole console = null;
			if (!gameService.World.TryFind<WorldSecurityConsole>(targetId, ref console))
			{
				throw new GameException(ErrorType.ObjectNotFound);
			}

			if (playerSession.Character.SkillController.OnlyMoveInputWhileSkillPlaying())
			{
				playerSession.Character.Controller.MoveTo(console.GetPosition(), false);
				return;
			}

			if (!playerSession.Character.CanAnyAction(ActionType.SecurityCamera))
			{
				return;
			}

			if (!playerSession.Character.IsInInteractableDistance(console))
			{
				return;
			}

			console.CastingConsoleAction(eventType);
			if (eventType == SecurityConsoleEvent.AreaSecurityCameraSight)
			{
				playerSession.Character.StartActionCasting(CastingActionType.AreaSecurityCameraSight, true, null,
					delegate
					{
						console.ExecuteConsoleAction(playerSession.Character, eventType);
						console.CancelConsoleAction();
						playerSession.Character.AddMasteryConditionExp(MasteryConditionType.AreaSecurityCameraSight, 1);
					}, delegate { console.CancelConsoleAction(); });
			}
		}
	}
}