using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqEmotionIcon, false)]
	public class ReqEmotionIcon : ReqPacket
	{
		[Key(0)] public EmotionPlateType emotionPlateType;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			playerSession.Character.SendEmotionIcon(emotionPlateType);
		}
	}
}