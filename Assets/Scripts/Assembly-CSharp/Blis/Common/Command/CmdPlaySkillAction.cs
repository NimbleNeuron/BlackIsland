using System.Collections.Generic;
using Blis.Client;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdPlaySkillAction, false)]
	public class CmdPlaySkillAction : LocalObjectCommandPacket
	{
		[Key(2)] public int actionNo;


		[Key(1)] public SkillId skillId;


		[Key(3)] public List<SkillActionTarget> targets;


		public override void Action(ClientService service, LocalObject self)
		{
			if (targets == null)
			{
				return;
			}

			foreach (SkillActionTarget skillActionTarget in targets)
			{
				Vector3? targetPosition = null;
				if (skillActionTarget.targetPos != null)
				{
					targetPosition = skillActionTarget.targetPos.ToVector3();
				}

				PlaySkill(skillActionTarget.targetId, targetPosition, service, self);
			}
		}


		private void PlaySkill(int pTargetId, Vector3? targetPosition, ClientService service, LocalObject self)
		{
			LocalObject target = null;
			if (pTargetId != 0)
			{
				target = service.World.Find<LocalObject>(pTargetId);
			}

			self.PlaySkill(skillId, actionNo, target, targetPosition);
		}
	}
}