using Blis.Client;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdPlayPassiveSkill, false)]
	public class CmdPlayPassiveSkill : LocalObjectCommandPacket
	{
		
		[Key(2)] public int actionNo;

		
		[Key(1)] public SkillId skillId;

		
		[Key(3)] public int targetId;

		
		[Key(4)] public BlisVector targetPos;

		
		public override void Action(ClientService service, LocalObject self)
		{
			LocalObject localObject = null;
			if (targetId != 0)
			{
				localObject = service.World.Find<LocalObject>(targetId);
			}

			self.IfTypeOf<LocalPlayerCharacter>(delegate(LocalPlayerCharacter player) { player.OnPlayPassiveSkill(); });
			SkillId skillId = this.skillId;
			int num = actionNo;
			LocalObject target = localObject;
			BlisVector blisVector = targetPos;
			self.PlaySkill(skillId, num, target, blisVector != null ? blisVector.ToVector3() : Vector3.zero);
		}
	}
}