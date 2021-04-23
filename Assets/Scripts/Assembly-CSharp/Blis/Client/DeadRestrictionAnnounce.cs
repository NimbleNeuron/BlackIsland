using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class DeadRestrictionAnnounce : GameAnnounce
	{
		private DeadAnnounceInfo info;

		public override void Init(byte[] announceInfo)
		{
			info = Serializer.Default.Deserialize<DeadAnnounceInfo>(announceInfo);
		}


		public override void ShowAnnounce()
		{
			MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.ShowMessage(AnnounceMessageType.General,
				Ln.Format("금지구역사안내",
					MonoBehaviourInstance<ClientService>.inst.GetPlayerNickName(info.deadPlayerObjectId)), 5f,
				Color.red, delegate { Singleton<SoundControl>.inst.PlayAnnounceSoundDeadPlayer(0, info.aliveCount); });
			MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(
				Ln.Format("금지구역사망알림",
					MonoBehaviourInstance<ClientService>.inst.GetPlayerNickName(info.deadPlayerObjectId),
					LnUtil.GetCharacterName(info.deadCharacterCode)), true);
		}
	}
}