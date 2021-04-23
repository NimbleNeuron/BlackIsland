using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.RpcSkillReserveCancel, false)]
	public class RpcSkillReserveCancel : RpcPacket
	{
		
		[Key(0)] public SkillSlotSet skillSlotSet;

		
		public override void Action(ClientService service)
		{
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.OnSkillReserveCancel(skillSlotSet);
		}
	}
}