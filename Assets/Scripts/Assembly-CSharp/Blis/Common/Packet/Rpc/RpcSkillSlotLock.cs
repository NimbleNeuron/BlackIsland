using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcSkillSlotLock, false)]
	public class RpcSkillSlotLock : RpcPacket
	{
		[Key(1)] public bool isLock;


		[Key(0)] public SkillSlotSet skillSlotSetFlag;


		public override void Action(ClientService clientService)
		{
			foreach (SkillSlotSet skillSlotSet in GameDB.skill.allSkillSlotSet)
			{
				if (skillSlotSet != SkillSlotSet.None && skillSlotSetFlag.HasFlag(skillSlotSet))
				{
					SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.LockSkillSlot(skillSlotSet, isLock);
				}
			}
		}
	}
}