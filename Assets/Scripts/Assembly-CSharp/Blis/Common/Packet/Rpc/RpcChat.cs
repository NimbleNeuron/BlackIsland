using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcChat, false)]
	public class RpcChat : RpcPacket
	{
		[Key(1)] public int characterCode;


		[Key(2)] public string chatContent;


		[Key(3)] public bool IsAll;


		[Key(4)] public bool IsNotice;


		[Key(0)] public int senderObjectId;


		[Key(5)] public bool showTime;


		public override void Action(ClientService service)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.IsIgnoreUser(IgnoreType.Chat,
				senderObjectId))
			{
				return;
			}

			chatContent = SingletonMonoBehaviour<SwearWordManager>.inst.CheckAndReplaceChat(chatContent);
			string characterName = LnUtil.GetCharacterName(characterCode);
			if (service.IsGameStarted)
			{
				MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddChatting(senderObjectId, characterName, chatContent,
					IsAll, IsNotice, showTime, false, "inGameChatting");
				return;
			}

			if (MonoBehaviourInstance<GameUI>.inst.StartingView.IsOpen)
			{
				MonoBehaviourInstance<GameUI>.inst.StartingView.AddChatting(senderObjectId, characterName, chatContent,
					IsAll, IsNotice, false);
			}
		}
	}
}