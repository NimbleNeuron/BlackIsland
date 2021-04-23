using Blis.Server;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.ReqPlayingSkillOnSelect, false)]
	public class ReqPlayingSkillOnSelect : ReqPacket
	{
		
		[Key(1)] public Vector3 hitPosition;

		
		[Key(2)] public Vector3 releasePosition;

		
		[Key(0)] public int targetId;

		
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			WorldCharacter hitTarget = null;
			gameService.World.TryFind<WorldCharacter>(targetId, ref hitTarget);
			playerSession.Character.PlayingSkillOnSelect(hitTarget, hitPosition, releasePosition);
		}
	}
}