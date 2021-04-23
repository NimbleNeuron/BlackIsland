using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class MonsterKillAnnounce : GameAnnounce
	{
		private MonsterKillAnnounceInfo info;

		public override void Init(byte[] announceInfo)
		{
			info = Serializer.Default.Deserialize<MonsterKillAnnounceInfo>(announceInfo);
		}


		public override void ShowAnnounce()
		{
			if (GameDB.monster.GetMonsterData(info.killMonsterCode).monster == MonsterType.Wickline)
			{
				MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.ShowMessage(AnnounceMessageType.General,
					Ln.Format("위클라인사안내",
						MonoBehaviourInstance<ClientService>.inst.GetPlayerNickName(info.deadPlayerObjectId)), 5f,
					Color.red,
					delegate { Singleton<SoundControl>.inst.PlayAnnounceSoundDeadPlayer(0, info.aliveCount); });
				return;
			}

			MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.ShowMessage(AnnounceMessageType.General,
				Ln.Format("야생동물사안내",
					MonoBehaviourInstance<ClientService>.inst.GetPlayerNickName(info.deadPlayerObjectId)), 5f,
				Color.red, delegate { Singleton<SoundControl>.inst.PlayAnnounceSoundDeadPlayer(0, info.aliveCount); });
		}
	}
}