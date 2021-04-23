using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdSpawn, false)]
	public class CmdSpawn : CommandPacket
	{
		[Key(0)] public SnapshotWrapper snapshot;


		public override void Action(ClientService clientService)
		{
			LocalObject localObject = clientService.World.SpawnSnapshot(snapshot);
			if (localObject == null)
			{
				return;
			}

			localObject.IfTypeOf<LocalCharacter>(delegate(LocalCharacter localCharacter)
			{
				CharacterSnapshot characterSnapshot =
					Serializer.Default.Deserialize<CharacterSnapshot>(snapshot.snapshot);
				localCharacter.InitStateEffect(characterSnapshot.initialStateEffect);
			});
			
			localObject.IfTypeOf<LocalSummonBase>(delegate(LocalSummonBase summon)
			{
				if (!clientService.IsPlayer)
				{
					summon.InSight();
					summon.OnVisible();
				}

				if (summon.SummonData.castingActionType == CastingActionType.InstallTrap && summon.Owner != null)
				{
					summon.Owner.CharacterVoiceControl.PlayCharacterVoice(CharacterVoiceType.InstallTrap, 15,
						summon.Owner.GetPosition());
				}

				if (!string.IsNullOrEmpty(summon.SummonData.createSound))
				{
					Singleton<SoundControl>.inst.PlayFXSound(summon.SummonData.createSound, "InstallSummon", 8,
						summon.GetPosition(), false);
				}
			});
		}
	}
}