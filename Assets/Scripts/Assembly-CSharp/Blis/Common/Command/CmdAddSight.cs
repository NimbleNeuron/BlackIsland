using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdAddSight, false)]
	public class CmdAddSight : LocalCharacterCommandPacket
	{
		[Key(1)] public int attachSightId;


		[Key(3)] public BlisFixedPoint sightRange;


		[Key(2)] public int targetId;


		public override void Action(ClientService service, LocalCharacter self)
		{
			LocalObject localObject = service.World.Find<LocalObject>(targetId);
			LocalSightAgent localSightAgent = localObject.gameObject.AddComponent<LocalSightAgent>();
			localSightAgent.InitAttachSight(localObject, attachSightId);
			localSightAgent.SetOwner(self.SightAgent);
			localSightAgent.UpdateSightRange(sightRange.Value);
			localSightAgent.UpdateSightAngle(360);
		}
	}
}