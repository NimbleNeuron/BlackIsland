using Blis.Server;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.ReqDropItemFromEquipment, false)]
	public class ReqDropItemFromEquipment : ReqPacket
	{
		
		[Key(0)] public int itemId;

		
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			if (!playerSession.Character.CanAnyAction(ActionType.ItemEquipOrUnequip))
			{
				return;
			}

			if (playerSession.Character.SkillController.AnyPlayingSkill())
			{
				return;
			}

			Item item = playerSession.Character.FindEquipById(itemId);
			if (item == null)
			{
				throw new GameException(ErrorType.ItemNotFound);
			}

			playerSession.Character.UnequipItem(item);
			playerSession.Character.FinishBulletCooldown(itemId);
			playerSession.Character.SendEquipmentUpdate();
			Vector3 position = playerSession.Character.GetPosition();
			gameService.Spawn.SpawnItem(position, item, gameService.GetDropItemPosition(position));
		}
	}
}