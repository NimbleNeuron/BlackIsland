using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqEquipItem, false)]
	public class ReqEquipItem : ReqPacketForResponse
	{
		[Key(1)] public int itemId;


		[Key(2)] public ItemMadeType madeType;


		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			if (!playerSession.Character.CanAnyAction(ActionType.ItemEquipOrUnequip))
			{
				return new ResEquipItem();
			}

			if (playerSession.Character.SkillController.AnyPlayingSkill())
			{
				return new ResEquipItem();
			}

			Item item = playerSession.Character.RemoveInventoryItem(itemId, madeType);
			if (playerSession.Character.EquipItem(item))
			{
				playerSession.Character.SendInventoryUpdate(UpdateInventoryType.EquipItem);
				playerSession.Character.SendEquipmentUpdate();
			}
			else
			{
				playerSession.Character.ForceAddInventoryItem(item);
				playerSession.Character.SendInventoryUpdate(UpdateInventoryType.EquipItem);
			}

			return new ResEquipItem
			{
				errorCode = 0
			};
		}
	}
}