using System.Collections.Generic;
using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdKill, false)]
	public class CmdKill : LocalCharacterCommandPacket
	{
		[Key(2)] public List<int> assistCharacterObjectIds;


		[Key(1)] public int deadCharacterObjectId;


		public override void Action(ClientService service, LocalCharacter self)
		{
			LocalCharacter deadCharacter = deadCharacterObjectId > 0
				? service.World.Find<LocalCharacter>(deadCharacterObjectId)
				: null;
			self.IfTypeOf<LocalPlayerCharacter>(delegate(LocalPlayerCharacter attackerPlayer)
			{
				attackerPlayer.OnKill(deadCharacter);
				if (deadCharacter.ObjectType == ObjectType.Monster)
				{
					attackerPlayer.CharacterVoiceControl.PlayCharacterVoice(CharacterVoiceType.MonsterKill, 15,
						self.GetPosition());
					return;
				}

				if (deadCharacter.ObjectType == ObjectType.PlayerCharacter ||
				    deadCharacter.ObjectType == ObjectType.BotPlayerCharacter)
				{
					CharacterVoiceType charVoiceType =
						CharacterVoiceUtil.KillConvertToCharacterVoiceType(self.Status.PlayerKill);
					attackerPlayer.CharacterVoiceControl.PlayCharacterVoice(charVoiceType, 15, self.GetPosition());
				}
			});
			self.IfTypeOf<LocalSummonTrap>(delegate(LocalSummonTrap attackerSummon)
			{
				LocalPlayerCharacter owner = attackerSummon.Owner;
				if (owner != null)
				{
					owner.OnKill(deadCharacter);
					if (deadCharacter.ObjectType == ObjectType.PlayerCharacter ||
					    deadCharacter.ObjectType == ObjectType.BotPlayerCharacter)
					{
						CharacterVoiceType charVoiceType =
							CharacterVoiceUtil.KillConvertToCharacterVoiceType(owner.Status.PlayerKill);
						owner.CharacterVoiceControl.PlayCharacterVoice(charVoiceType, 15, owner.GetPosition());
					}
				}
			});
			foreach (int objectId in assistCharacterObjectIds)
			{
				if (MonoBehaviourInstance<ClientService>.inst.World.HasObjectId(objectId))
				{
					MonoBehaviourInstance<ClientService>.inst.World.Find<LocalPlayerCharacter>(objectId)
						.OnKillAssist(deadCharacter);
				}
			}
		}
	}
}