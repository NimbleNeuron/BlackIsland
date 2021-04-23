using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqUseItem, false)]
	public class ReqUseItem : ReqPacketForResponse
	{
		[Key(1)] public int itemId;


		[Key(2)] public ItemMadeType madeType;


		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			Item item = playerSession.Character.FindInventoryItemById(itemId, madeType);
			if (item == null)
			{
				return new ResUseItem
				{
					success = false
				};
			}

			if (!playerSession.Character.CanAnyAction(ActionType.ItemUse) && !item.IsRecoveryItem())
			{
				return new ResUseItem
				{
					success = false
				};
			}

			if (!playerSession.Character.UseItem(itemId, madeType))
			{
				return new ResUseItem
				{
					success = false
				};
			}

			return new ResUseItem
			{
				success = true
			};
		}
	}
}