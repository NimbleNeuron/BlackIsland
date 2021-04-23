using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdStopAndInteract, false)]
	public class CmdInteract : CommandPacket
	{
		[Key(0)] public int targetId;


		public override void Action(ClientService service)
		{
			LocalObject localObject = service.World.Find<LocalObject>(targetId);
			SingletonMonoBehaviour<PlayerController>.inst.OnInteract(localObject);
			if (localObject.ObjectType == ObjectType.PlayerCharacter ||
			    localObject.ObjectType == ObjectType.BotPlayerCharacter)
			{
				localObject.IfTypeOf<LocalPlayerCharacter>(delegate(LocalPlayerCharacter character)
				{
					LocalPlayerCharacter character2 = service.MyPlayer.Character;
					CharacterVoiceType charVoiceType = character2.ObjectId == character.WhoKilledMe
						? CharacterVoiceType.OpenSelfKillPlayer
						: CharacterVoiceType.OpenEnemyKillPlayer;
					character2.CharacterVoiceControl.PlayCharacterVoice(charVoiceType, 15, character2.GetPosition());
				});
				return;
			}

			if (localObject.ObjectType == ObjectType.Monster && MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				MonoBehaviourInstance<TutorialController>.inst.SuccessOpenMonsterItemBox();
			}
		}
	}
}