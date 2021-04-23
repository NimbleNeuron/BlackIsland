using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class DeadPlayerAnnounce : GameAnnounce
	{
		private PlayerKillAnnounceInfo info;


		public override void Init(byte[] announceInfo)
		{
			info = Serializer.Default.Deserialize<PlayerKillAnnounceInfo>(announceInfo);
		}


		public override void ShowAnnounce()
		{
			Log.V("[DeadPlayerAnnounce.ShowAnnounce] info.killCount : {0}", info.killCount);
			Math.Min(info.killCount, 8);
			MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.ShowMessage(AnnounceMessageType.Kill,
				Ln.Format("플레이어처치안내1",
					MonoBehaviourInstance<ClientService>.inst.GetPlayerNickName(info.killPlayerObjectId),
					MonoBehaviourInstance<ClientService>.inst.GetPlayerNickName(info.deadPlayerObjectId)), 5f,
				Color.red,
				delegate { Singleton<SoundControl>.inst.PlayAnnounceSoundDeadPlayer(info.killCount, info.aliveCount); },
				info.killWeaponType, info.killCharacterCode, info.deadCharacterCode, info.trapKill,
				info.killPlayerObjectId, info.assistants);
		}
	}
}