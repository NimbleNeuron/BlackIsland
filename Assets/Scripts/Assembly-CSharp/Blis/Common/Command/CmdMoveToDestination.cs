using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject]
	[PacketAttr(PacketType.CmdMoveToDestination, false)]
	public class CmdMoveToDestination : MoveCommandPacket
	{
		
		[Key(3)] public BlisVector[] corners;

		
		[Key(2)] public BlisVector destination;

		
		protected override void MoveAction(ClientService service, ILocalMoveAgentOwner iMoveAgent, LocalCharacter self)
		{
			iMoveAgent.MoveToDestination(position, destination, corners);
			LocalPlayerCharacter localPlayerCharacter = self as LocalPlayerCharacter;
			if (localPlayerCharacter == null)
			{
				return;
			}

			if (!localPlayerCharacter.IsFirstMove)
			{
				localPlayerCharacter.FirstMove();
				localPlayerCharacter.CharacterVoiceControl.PlayCharacterVoice(CharacterVoiceType.FirstMove, 15,
					localPlayerCharacter.GetPosition());
				return;
			}

			int code = localPlayerCharacter.GetCurrentAreaData(service.CurrentLevel).code;
			if (code != 0)
			{
				if (service.CheckCurrentAreaRestrict(code))
				{
					localPlayerCharacter.CharacterVoiceControl.PlayCharacterVoice(
						CharacterVoiceType.MoveInRestrictedArea, 15, localPlayerCharacter.GetPosition());
					if (MonoBehaviourInstance<GameClient>.inst.IsTutorial && service.MyObjectId == self.ObjectId)
					{
						MonoBehaviourInstance<TutorialController>.inst.CreateRestricAreaTutorial();
					}
				}
				else
				{
					CharacterVoiceType charVoiceType = CharacterVoiceUtil.AreaConvertToCharacterVoiceType(code);
					localPlayerCharacter.CharacterVoiceControl.PlayCharacterVoice(charVoiceType, 15,
						localPlayerCharacter.GetPosition());
				}
			}
		}
	}
}