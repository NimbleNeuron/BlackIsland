using Blis.Common.Utils;

namespace Blis.Client
{
	public class AirSupplyAnnounce : GameAnnounce
	{
		public override void Init(byte[] announceInfo) { }


		public override void ShowAnnounce()
		{
			MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.ShowMessage(AnnounceMessageType.Notice, Ln.Get("항공보급안내"),
				delegate { Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.AirSupplyNotice); });
			MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(Ln.Get("항공보급안내"), true);
		}
	}
}