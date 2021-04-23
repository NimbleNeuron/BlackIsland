using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.ReqEmotionCharacaterVoice, false)]
	public class ReqEmotionCharacterVoice : ReqPacket
	{
		
		[Key(0)] public int characterObjectId;

		
		[Key(1)] public int emotionCharVoiceType;

		
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			playerSession.Character.EmotionCharacterVoice(characterObjectId, emotionCharVoiceType);
		}
	}
}