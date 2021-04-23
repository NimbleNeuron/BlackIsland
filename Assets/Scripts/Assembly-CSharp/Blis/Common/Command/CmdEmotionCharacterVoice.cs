using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject]
	[PacketAttr(PacketType.CmdEmotionCharacterVoice, false)]
	public class CmdEmotionCharacterVoice : LocalPlayerCharacterCommandPacket
	{
		
		[Key(1)] public int characterObjectId;

		
		[Key(2)] public int charVoiceType;

		
		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			LocalPlayerCharacter localPlayerCharacter = service.World.Find<LocalPlayerCharacter>(characterObjectId);
			self.CharacterVoiceControl.PlayCharacterVoice((CharacterVoiceType) charVoiceType, 15,
				localPlayerCharacter.GetPosition());
		}
	}
}