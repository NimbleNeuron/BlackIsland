using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqOpenCorpse, true)]
	public class ReqOpenCorpse : ReqPacketForResponse
	{
		[Key(1)] public int targetId;


		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			WorldCharacter worldCharacter = null;
			if (!gameService.World.TryFind<WorldCharacter>(targetId, ref worldCharacter))
			{
				return new ResOpenCorpse
				{
					success = false
				};
			}

			if (playerSession.Character.SkillController.OnlyMoveInputWhileSkillPlaying())
			{
				playerSession.Character.Controller.MoveTo(worldCharacter.GetPosition(), false);
				return new ResOpenCorpse
				{
					success = false
				};
			}

			if (!playerSession.Character.CanAnyAction(ActionType.OpenCorpse))
			{
				return new ResOpenCorpse
				{
					success = false
				};
			}

			if (!playerSession.Character.IsInInteractableDistance(worldCharacter))
			{
				return new ResOpenCorpse
				{
					success = false
				};
			}

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

			SingletonMonoBehaviour<BattleEventCollector>.inst.OnBeforeOpenCorpse(playerSession.Character,
				worldCharacter);
			Singleton<ItemRoutingRecoder>.inst.OpenBox(playerSession.userId, targetId);
			return new ResOpenCorpse
			{
				success = true,
				items = worldCharacter.ItemBox.Open(playerSession)
			};
		}
	}
}