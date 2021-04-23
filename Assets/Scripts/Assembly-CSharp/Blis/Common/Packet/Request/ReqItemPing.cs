using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqItemPing, false)]
	public class ReqItemPing : ReqPacket
	{
		[Key(0)] public int itemId;


		[Key(1)] public SystemChatType type;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			AreaData currentAreaData = AreaUtil.GetCurrentAreaData(gameService.CurrentLevel,
				playerSession.Character.GetPosition(), 2147483640);
			CmdSystemChat packet = new CmdSystemChat
			{
				type = type,
				targetObjectId = itemId,
				characterCode = playerSession.Character.CharacterCode,
				senderObjectId = playerSession.ObjectId,
				areaCode = currentAreaData != null ? currentAreaData.code : 0
			};
			playerSession.EnqueueCommandTeamOnly(new PacketWrapper(packet));
		}
	}
}