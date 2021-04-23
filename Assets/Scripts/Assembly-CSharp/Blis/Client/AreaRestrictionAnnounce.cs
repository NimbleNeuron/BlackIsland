using System;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public class AreaRestrictionAnnounce : GameAnnounce
	{
		private AreaRestrictionAnnounceType announceType;

		public override void Init(byte[] announceInfo)
		{
			announceType = Serializer.Default.Deserialize<AreaRestrictionAnnounceType>(announceInfo);
		}


		public override void ShowAnnounce()
		{
			switch (announceType)
			{
				case AreaRestrictionAnnounceType.Normal:
					MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.ShowMessage(AnnounceMessageType.Notice,
						Ln.Get("금지구역지정안내"),
						delegate { Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.Restrict); });
					MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(Ln.Get("금지구역지정안내"), true);
					return;
				case AreaRestrictionAnnounceType.Accelerater:
					MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.ShowMessage(AnnounceMessageType.Notice,
						Ln.Get("금지구역지정안내"), delegate
						{
							Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.Restrict);
							MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(Ln.Get("금지구역지정안내"), true);
						});
					MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.ShowMessage(AnnounceMessageType.Notice,
						Ln.Get("가속금지구역지정안내"), delegate
						{
							Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.RestrictAccel);
							MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(Ln.Get("가속금지구역지정안내"), true);
						});
					return;
				case AreaRestrictionAnnounceType.Last:
					MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.ShowMessage(AnnounceMessageType.Notice,
						Ln.Get("마지막금지구역안내"), delegate
						{
							Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.FinalRestrict);
							MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(Ln.Get("마지막금지구역안내"), true);
						});
					return;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}