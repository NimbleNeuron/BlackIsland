using System;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public class LastSafeConsoleAnnounce : GameAnnounce
	{
		public LastSafeConsoleAnnounceType announceType;

		public override void Init(byte[] announceInfo)
		{
			announceType = Serializer.Default.Deserialize<LastSafeConsoleAnnounceType>(announceInfo);
		}


		public override void ShowAnnounce()
		{
			switch (announceType)
			{
				case LastSafeConsoleAnnounceType.Active:
					MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.ShowMessage(AnnounceMessageType.Notice,
						Ln.Get("최종안전지대활성화안내"),
						delegate
						{
							Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.FinalSafeActive);
						});
					MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(Ln.Get("최종안전지대활성화안내"), true);
					return;
				case LastSafeConsoleAnnounceType.PreDeactive:
					MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.ShowMessage(AnnounceMessageType.Notice,
						Ln.Get("최종안전지대해제예고안내"),
						delegate
						{
							Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.FinalSafePreDeactive);
						});
					MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(Ln.Get("최종안전지대해제예고안내"), true);
					return;
				case LastSafeConsoleAnnounceType.Deactive:
					MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.ShowMessage(AnnounceMessageType.Notice,
						Ln.Get("최종안전지대해제안내"),
						delegate
						{
							Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.FinalSafeDeactive);
						});
					MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(Ln.Get("최종안전지대해제안내"), true);
					return;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}