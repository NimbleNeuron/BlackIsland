using Blis.Common.Utils;
using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqOpenItemBox, false)]
	public class ReqOpenItemBox : ReqPacket
	{
		[Key(0)] public int targetId;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			WorldItemBox worldItemBox = null;
			if (!gameService.World.TryFind<WorldItemBox>(targetId, ref worldItemBox))
			{
				throw new GameException(ErrorType.ObjectNotFound);
			}

			if (playerSession.Character.SkillController.OnlyMoveInputWhileSkillPlaying())
			{
				playerSession.Character.Controller.MoveTo(worldItemBox.GetPosition(), false);
				return;
			}

			if (!playerSession.Character.IsInInteractableDistance(worldItemBox))
			{
				return;
			}

			bool flag = worldItemBox.ObjectType == ObjectType.AirSupplyItemBox;
			if (!playerSession.Character.CanAnyAction(flag ? ActionType.AirSupplyOpen : ActionType.OpenNoCastBox))
			{
				return;
			}

			bool flag2 = playerSession.Character.IsInteractedObject(worldItemBox);
			if (flag && flag2)
			{
				OpenItemBoxAction(gameServer, gameService, playerSession, targetId);
				return;
			}

			int extraParam = 0;
			if (worldItemBox.ObjectType == ObjectType.ResourceItemBox)
			{
				extraParam = ((WorldResourceItemBox) worldItemBox).ResourceDataCode;
			}
			else if (worldItemBox.ObjectType == ObjectType.AirSupplyItemBox)
			{
				extraParam = targetId;
			}

			CastingActionType type = CastingActionType.None;
			switch (worldItemBox.ObjectType)
			{
				case ObjectType.StaticItemBox:
					type = CastingActionType.BoxOpen;
					break;
				case ObjectType.ResourceItemBox:
				{
					CollectibleData collectibleData =
						MonoBehaviourInstance<GameService>.inst.CurrentLevel.GetCollectibleData(
							((WorldResourceItemBox) worldItemBox).ResourceDataCode);
					if (collectibleData != null)
					{
						type = collectibleData.castingActionType;
					}

					break;
				}
				case ObjectType.AirSupplyItemBox:
					type = CastingActionType.AirSupplyOpen;
					break;
			}

			playerSession.Character.StartActionCasting(type, true, null,
				delegate { OpenItemBoxAction(gameServer, gameService, playerSession, targetId); }, null, extraParam);
		}


		private void OpenItemBoxAction(GameServer gameServer, GameService gameService, PlayerSession playerSession,
			int targetId)
		{
			WorldItemBox worldItemBox = null;
			if (!gameService.World.TryFind<WorldItemBox>(targetId, ref worldItemBox))
			{
				throw new GameException(ErrorType.ObjectNotFound);
			}

			if (playerSession.Character.SkillController.OnlyMoveInputWhileSkillPlaying())
			{
				playerSession.Character.Controller.MoveTo(worldItemBox.GetPosition(), false);
				return;
			}

			if (!playerSession.Character.IsInInteractableDistance(worldItemBox))
			{
				return;
			}

			bool flag = worldItemBox.ObjectType == ObjectType.AirSupplyItemBox;
			if (!playerSession.Character.CanAnyAction(flag ? ActionType.AirSupplyOpen : ActionType.OpenNoCastBox))
			{
				return;
			}

			bool flag2 = false;
			if (worldItemBox.ObjectType == ObjectType.ResourceItemBox)
			{
				if (playerSession.Character.CollectResource((WorldResourceItemBox) worldItemBox))
				{
					playerSession.Character.SendInventoryUpdate(UpdateInventoryType.ResourceGather);
					flag2 = true;
				}
			}
			else
			{
				if (playerSession.OpenBoxId != 0)
				{
					WorldObject worldObject = null;
					if (gameService.World.TryFind<WorldObject>(playerSession.OpenBoxId, ref worldObject))
					{
						worldObject.ItemBox.Close(playerSession);
						Singleton<ItemRoutingRecoder>.inst.CloseBox(playerSession.userId, worldObject.ObjectId);
					}
					else
					{
						playerSession.SetOpenBoxId(0);
					}
				}

				gameServer.Send(playerSession, new RpcOpenItemBox
				{
					targetId = targetId,
					items = worldItemBox.ItemBox.Open(playerSession)
				});
				if (!playerSession.Character.IsInteractedObject(worldItemBox))
				{
					MasteryConditionType type = worldItemBox.ObjectType == ObjectType.StaticItemBox
						? MasteryConditionType.FixedBoxOpen
						: MasteryConditionType.AirSupplyOpen;
					playerSession.Character.AddMasteryConditionExp(type, 1);
					playerSession.Character.AddInteractedObject(worldItemBox.ObjectId);
				}

				Singleton<ItemRoutingRecoder>.inst.OpenBox(playerSession.userId, targetId);
				flag2 = true;
			}

			if (flag2)
			{
				NoiseType noiseType = NoiseType.None;
				switch (worldItemBox.ObjectType)
				{
					case ObjectType.StaticItemBox:
						noiseType = NoiseType.FixedBoxOpen;
						break;
					case ObjectType.ResourceItemBox:
						noiseType = NoiseType.CollectibleOpen;
						break;
					case ObjectType.AirSupplyItemBox:
						noiseType = NoiseType.AirSupplyOpen;
						break;
				}

				if (noiseType != NoiseType.None)
				{
					gameService.Announce.MakeNoise(playerSession.Character, null, noiseType);
				}
			}

			SingletonMonoBehaviour<BattleEventCollector>.inst.OnAfterOepnItemBox(playerSession.Character, worldItemBox);
		}
	}
}