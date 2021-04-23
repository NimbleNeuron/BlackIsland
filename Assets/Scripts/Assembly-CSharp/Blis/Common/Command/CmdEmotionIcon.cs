using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdEmotionIcon, false)]
	public class CmdEmotionIcon : LocalPlayerCharacterCommandPacket
	{
		[Key(1)] public int emotionIconCode;


		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			if (Singleton<LocalSetting>.inst.setting.ignoreEmotionFromEnemy && service.MyTeamNumber != self.TeamNumber)
			{
				return;
			}

			if (MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.IsIgnoreUser(IgnoreType.Emotion,
				self.ObjectId))
			{
				return;
			}

			self.ShowEmotionIcon(emotionIconCode);
		}
	}
}