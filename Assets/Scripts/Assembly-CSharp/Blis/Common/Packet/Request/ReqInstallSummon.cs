using System.Collections.Generic;
using Blis.Server;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqInstallSummon, false)]
	public class ReqInstallSummon : ReqPacket
	{
		[Key(0)] public int itemId;


		[Key(2)] public ItemMadeType madeType;


		[Key(1)] public Vector3 position;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			if (playerSession.Character.SkillController.OnlyMoveInputWhileSkillPlaying())
			{
				playerSession.Character.Controller.MoveTo(position, false);
			}
			else
			{
				if (playerSession.Character.IsRest)
				{
					return;
				}

				Item inventoryItemById = playerSession.Character.FindInventoryItemById(itemId, madeType);
				SummonData summonData = GameDB.character.GetSummonData(
					((inventoryItemById != null
						 ? inventoryItemById.ItemData.GetSubTypeData<ItemSpecialData>()
						 : throw new GameException(ErrorType.ItemNotFound)) ??
					 throw new GameException(ErrorType.ItemNotFound)).summonCode);
				if (summonData == null)
				{
					throw new GameException(ErrorType.ItemNotFound);
				}

				if (!playerSession.Character.CanAnyAction(summonData.castingActionType == CastingActionType.None
					? ActionType.NoCastInstallSummon
					: ActionType.CastInstallSummon))
				{
					throw new GameException(ErrorType.InvalidAction);
				}

				if (playerSession.Character.IsRunningCastingAction())
				{
					playerSession.Character.CancelActionCasting(CastingCancelType.Action);
				}

				Vector3 sampledPosition;
				MoveAgent.SamplePosition(position, playerSession.Character.WalkableNavMask, out sampledPosition);
				List<WorldPlayerCharacter> teamMembers = playerSession.GetTeamCharacters();
				teamMembers.Add(playerSession.Character);
				for (int i = 0; i < teamMembers.Count; ++i)
				{
					foreach (WorldSummonBase worldSummonBase in gameService.World.FindAll<WorldSummonBase>(
						s => s.IsOwner(teamMembers[i].ObjectId)))
					{
						if (worldSummonBase.SummonData.pileRange > 0.0)
						{
							float magnitude = Vector3.Scale(sampledPosition - worldSummonBase.GetPosition(),
								new Vector3(1f, 0.0f, 1f)).magnitude;
							if (magnitude <=
							    summonData.radius + (double) worldSummonBase.SummonData.radius)
							{
								playerSession.Character.SendToastMessage(ToastMessageType.IsImpossibleSummonPosition);
								return;
							}

							if (summonData.objectType == worldSummonBase.SummonData.objectType &&
							    magnitude <= (double) worldSummonBase.SummonData.pileRange)
							{
								playerSession.Character.SendToastMessage(ToastMessageType.IsImpossibleSummonPosition);
								return;
							}
						}
					}
				}

				playerSession.Character.Controller.InstallSummon(inventoryItemById, sampledPosition);
			}

			// co: dotPeek
			// ReqInstallSummon.<>c__DisplayClass3_0 CS$<>8__locals1 = new ReqInstallSummon.<>c__DisplayClass3_0();
			// if (playerSession.Character.SkillController.OnlyMoveInputWhileSkillPlaying())
			// {
			// 	playerSession.Character.Controller.MoveTo(this.position, false);
			// 	return;
			// }
			// if (playerSession.Character.IsRest)
			// {
			// 	return;
			// }
			// Item item = playerSession.Character.FindInventoryItemById(this.itemId, this.madeType);
			// if (item == null)
			// {
			// 	throw new GameException(ErrorType.ItemNotFound);
			// }
			// ItemSpecialData subTypeData = item.ItemData.GetSubTypeData<ItemSpecialData>();
			// if (subTypeData == null)
			// {
			// 	throw new GameException(ErrorType.ItemNotFound);
			// }
			// SummonData summonData = GameDB.character.GetSummonData(subTypeData.summonCode);
			// if (summonData == null)
			// {
			// 	throw new GameException(ErrorType.ItemNotFound);
			// }
			// if (!playerSession.Character.CanAnyAction((summonData.castingActionType == CastingActionType.None) ? ActionType.NoCastInstallSummon : ActionType.CastInstallSummon))
			// {
			// 	throw new GameException(ErrorType.InvalidAction);
			// }
			// if (playerSession.Character.IsRunningCastingAction())
			// {
			// 	playerSession.Character.CancelActionCasting(CastingCancelType.Action);
			// }
			// Vector3 a;
			// MoveAgent.SamplePosition(this.position, playerSession.Character.WalkableNavMask, out a);
			// CS$<>8__locals1.teamMembers = playerSession.GetTeamCharacters();
			// CS$<>8__locals1.teamMembers.Add(playerSession.Character);
			// int i;
			// Func<WorldSummonBase, bool> <>9__0;
			// int j;
			// for (i = 0; i < CS$<>8__locals1.teamMembers.Count; i = j)
			// {
			// 	WorldBase<WorldObject> world = gameService.World;
			// 	Func<WorldSummonBase, bool> checkCondition;
			// 	if ((checkCondition = <>9__0) == null)
			// 	{
			// 		checkCondition = (<>9__0 = ((WorldSummonBase s) => s.IsOwner(CS$<>8__locals1.teamMembers[i].ObjectId)));
			// 	}
			// 	foreach (WorldSummonBase worldSummonBase in world.FindAll<WorldSummonBase>(checkCondition))
			// 	{
			// 		if (worldSummonBase.SummonData.pileRange > 0f)
			// 		{
			// 			float magnitude = Vector3.Scale(a - worldSummonBase.GetPosition(), new Vector3(1f, 0f, 1f)).magnitude;
			// 			if (magnitude <= summonData.radius + worldSummonBase.SummonData.radius)
			// 			{
			// 				playerSession.Character.SendToastMessage(ToastMessageType.IsImpossibleSummonPosition);
			// 				return;
			// 			}
			// 			if (summonData.objectType == worldSummonBase.SummonData.objectType && magnitude <= worldSummonBase.SummonData.pileRange)
			// 			{
			// 				playerSession.Character.SendToastMessage(ToastMessageType.IsImpossibleSummonPosition);
			// 				return;
			// 			}
			// 		}
			// 	}
			// 	j = i + 1;
			// }
			// playerSession.Character.Controller.InstallSummon(item, a);
		}
	}
}