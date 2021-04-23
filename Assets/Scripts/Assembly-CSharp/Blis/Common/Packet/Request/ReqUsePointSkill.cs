using Blis.Server;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqUsePointSkill, true)]
	public class ReqUsePointSkill : ReqPacketForResponse
	{
		[Key(2)] public Vector3 hitPosition;


		[Key(3)] public Vector3 releasePosition;


		[Key(1)] public SkillSlotSet skillSlotSet;


		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			if (playerSession.Character.IsRest)
			{
				return new ResUseSkill
				{
					errorCode = 5
				};
			}

			int errorCode = (int) playerSession.Character.UseSkill(skillSlotSet, hitPosition, releasePosition);
			return new ResUseSkill
			{
				errorCode = errorCode
			};
		}
	}
}