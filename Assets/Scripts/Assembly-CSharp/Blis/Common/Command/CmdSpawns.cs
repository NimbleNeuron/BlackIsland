using System.Collections.Generic;
using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdSpawns, false)]
	public class CmdSpawns : CommandPacket
	{
		[Key(0)] public List<SnapshotWrapper> snapshots;


		public override void Action(ClientService clientService)
		{
			using (List<SnapshotWrapper>.Enumerator enumerator = snapshots.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SnapshotWrapper snapshotWrapper = enumerator.Current;
					LocalObject localObject = clientService.World.SpawnSnapshot(snapshotWrapper);
					if (!(localObject == null))
					{
						localObject.IfTypeOf<LocalCharacter>(delegate(LocalCharacter localCharacter)
						{
							CharacterSnapshot characterSnapshot =
								Serializer.Default.Deserialize<CharacterSnapshot>(snapshotWrapper.snapshot);
							localCharacter.InitStateEffect(characterSnapshot.initialStateEffect);
						});
					}
				}
			}
		}
	}
}