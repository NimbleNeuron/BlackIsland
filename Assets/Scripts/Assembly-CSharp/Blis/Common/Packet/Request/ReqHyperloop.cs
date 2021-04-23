using Blis.Common.Utils;
using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqHyperloop, false)]
	public class ReqHyperloop : ReqPacket
	{
		[Key(1)] public int areaCode;


		[Key(0)] public int objectId;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			WorldHyperloop hyperloop = null;
			if (!gameService.World.TryFind<WorldHyperloop>(objectId, ref hyperloop))
			{
				throw new GameException(ErrorType.ObjectNotFound);
			}

			if (!hyperloop.IsReadyToHyperLoop())
			{
				throw new GameException(ErrorType.NotAvailableYet);
			}

			if (playerSession.Character.IsRest ||
			    !playerSession.Character.IsInInteractableDistance(hyperloop) ||
			    !playerSession.Character.CanAnyAction(ActionType.Hyperloop))
			{
				return;
			}

			ActionCostData actionCost = GameDB.character.GetActionCost(CastingActionType.Hyperloop);
			if (actionCost == null)
			{
				throw new GameException(
					string.Format("Invalid Action Type : {0}", CastingActionType.Hyperloop));
			}

			if (playerSession.Character.Status.Sp < actionCost.sp)
			{
				throw new GameException(ErrorType.NotEnoughSp);
			}

			if (playerSession.Character.IsRunningCastingAction())
			{
				playerSession.Character.CancelActionCasting(CastingCancelType.Action);
			}

			CharacterSpawnPoint destHyperLoop =
				MonoBehaviourInstance<GameService>.inst.Level.AllocHyperLoopExit(areaCode);
			if (destHyperLoop == null)
			{
				throw new GameException("Hyperloop spawn point is not enough");
			}

			bool startCasting = false;
			playerSession.Character.StartActionCasting(CastingActionType.Hyperloop, true,
				StartCallback, CompleteCallback,
				CancelCallback);
			if (startCasting)
			{
				return;
			}

			MonoBehaviourInstance<GameService>.inst.Level.FreeHyperLoopExit(destHyperLoop);

			void StartCallback()
			{
				startCasting = true;
				hyperloop.CastingHyperLoopAction();
				gameServer.EnqueueCommand(new CmdActiveHyperLoopExit
				{
					destination = new BlisVector(destHyperLoop.GetPosition())
				});
			}

			void CompleteCallback()
			{
				MonoBehaviourInstance<GameService>.inst.Level.FreeHyperLoopExit(destHyperLoop);
				if (!hyperloop.IsReadyToHyperLoop())
				{
					throw new GameException(ErrorType.NotAvailableYet);
				}

				hyperloop.SetUseFlag();
				playerSession.Character.HyperLoop(destHyperLoop.GetPosition());
			}

			void CancelCallback()
			{
				MonoBehaviourInstance<GameService>.inst.Level.FreeHyperLoopExit(destHyperLoop);
				hyperloop.CancelHyperLoopAction();
				gameServer.EnqueueCommand(new CmdCancelHyperLoopExit
				{
					destination = new BlisVector(destHyperLoop.GetPosition())
				});
			}

			// co: dotPeek    
			// ReqHyperloop.<>c__DisplayClass2_0 CS$<>8__locals1 = new ReqHyperloop.<>c__DisplayClass2_0();
			// CS$<>8__locals1.gameServer = gameServer;
			// CS$<>8__locals1.playerSession = playerSession;
			// CS$<>8__locals1.hyperloop = null;
			// if (!gameService.World.TryFind<WorldHyperloop>(this.objectId, ref CS$<>8__locals1.hyperloop))
			// {
			// 	throw new GameException(ErrorType.ObjectNotFound);
			// }
			// if (!CS$<>8__locals1.hyperloop.IsReadyToHyperLoop())
			// {
			// 	throw new GameException(ErrorType.NotAvailableYet);
			// }
			// if (CS$<>8__locals1.playerSession.Character.IsRest)
			// {
			// 	return;
			// }
			// if (!CS$<>8__locals1.playerSession.Character.IsInInteractableDistance(CS$<>8__locals1.hyperloop))
			// {
			// 	return;
			// }
			// if (!CS$<>8__locals1.playerSession.Character.CanAnyAction(ActionType.Hyperloop))
			// {
			// 	return;
			// }
			// ActionCostData actionCost = GameDB.character.GetActionCost(CastingActionType.Hyperloop);
			// if (actionCost == null)
			// {
			// 	throw new GameException(string.Format("Invalid Action Type : {0}", CastingActionType.Hyperloop));
			// }
			// if (CS$<>8__locals1.playerSession.Character.Status.Sp < actionCost.sp)
			// {
			// 	throw new GameException(ErrorType.NotEnoughSp);
			// }
			// if (CS$<>8__locals1.playerSession.Character.IsRunningCastingAction())
			// {
			// 	CS$<>8__locals1.playerSession.Character.CancelActionCasting(CastingCancelType.Action);
			// }
			// CS$<>8__locals1.destHyperLoop = MonoBehaviourInstance<GameService>.inst.Level.AllocHyperLoopExit(this.areaCode);
			// if (CS$<>8__locals1.destHyperLoop == null)
			// {
			// 	throw new GameException("Hyperloop spawn point is not enough");
			// }
			// CS$<>8__locals1.startCasting = false;
			// CS$<>8__locals1.playerSession.Character.StartActionCasting(CastingActionType.Hyperloop, true, new Action(CS$<>8__locals1.<Action>g__StartCallback|0), new Action(CS$<>8__locals1.<Action>g__CompleteCallback|1), new Action(CS$<>8__locals1.<Action>g__CancelCallback|2), 0);
			// if (!CS$<>8__locals1.startCasting)
			// {
			// 	MonoBehaviourInstance<GameService>.inst.Level.FreeHyperLoopExit(CS$<>8__locals1.destHyperLoop);
			// }
		}
	}
}