using System.Collections.Generic;
using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.RpcBroadCastingUpdateBeforeStart, false)]
	public class RpcBroadCastingUpdateBeforeStart : LocalPlayerCharacterRpcPacket
	{
		
		[Key(1)] public List<EquipItem> equips;

		
		[Key(4)] public BlisVector position;

		
		[Key(2)] public List<CharacterStatValue> stats;

		
		[Key(3)] public byte[] statusSnapshot;

		
		[Key(5)] public int walkableNavMask;

		
		public override void Action(ClientService clientService, LocalPlayerCharacter self)
		{
			self.OnUpdateEquipment(equips);
			self.InitStat(stats, statusSnapshot);
			self.SetPosition(position.ToVector3());
			self.MoveAgent.SetWalkableNavMask(walkableNavMask);
			if (clientService.MyObjectId == self.ObjectId)
			{
				MonoBehaviourInstance<MobaCamera>.inst.SetCameraPosition(position.ToVector3(), 0f);
			}
		}
	}
}