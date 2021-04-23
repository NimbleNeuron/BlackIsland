using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqUnequipItem, true)]
	public class ReqUnequipItem : ReqPacket
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

			int num;
			playerSession.Character.AddInventoryItem(item, out num);
			if (num == 0)
			{
				playerSession.Character.UnequipItem(item);
			}

			playerSession.Character.SendEquipmentUpdate();
			playerSession.Character.SendInventoryUpdate(UpdateInventoryType.UnequipItem);
		}
	}
}