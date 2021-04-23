using System;
using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqChat, true)]
	public class ReqChat : ReqPacket
	{
		[Key(0)] public string chatContent;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			DateTime now = DateTime.Now;
			bool flag = false;
			RpcChat packet = new RpcChat
			{
				characterCode = playerSession.Character.CharacterCode,
				senderObjectId = playerSession.ObjectId,
				chatContent = chatContent,
				IsAll = flag,
				IsNotice = false,
				showTime = true
			};
			if (flag)
			{
				gameServer.Broadcast(packet);
			}
			else
			{
				playerSession.BroadcastTeamMember(packet);
			}

			string targetType = flag ? "ALL" : "TEAM";
			BattleChatMessage context = new BattleChatMessage
			{
				dtm = now,
				source = playerSession.userId,
				targetType = targetType,
				target = playerSession.TeamNumber,
				message = chatContent
			};
			gameService.PutChatMessage(context);
		}


		public override void Action(GameServer gameServer, GameService gameService, ObserverSession observerSession) { }
	}
}