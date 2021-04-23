using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[Union(0, typeof(MoveCommandPacket))]
	[Union(1, typeof(CmdMoveStraightWithoutNav))]
	[Union(2, typeof(CmdMoveToTargetWithoutNav))]
	[Union(3, typeof(CmdBroadcastUpdateStat))]
	[Union(4, typeof(CmdHeal))]
	[Union(5, typeof(CmdHealEffectCode))]
	[Union(6, typeof(CmdHealHp))]
	[Union(7, typeof(CmdHealSp))]
	[Union(8, typeof(CmdHealHpEffectCode))]
	[Union(9, typeof(CmdHealSpEffectCode))]
	[Union(10, typeof(CmdSetExtraPoint))]
	[Union(11, typeof(CmdDamage))]
	[Union(12, typeof(CmdSpDamage))]
	[Union(13, typeof(CmdSkillDamage))]
	[Union(14, typeof(CmdBlock))]
	[Union(15, typeof(CmdEvasion))]
	[Union(16, typeof(CmdKill))]
	[Union(17, typeof(CmdDead))]
	[Union(18, typeof(CmdAddState))]
	[Union(19, typeof(CmdUpdateState))]
	[Union(20, typeof(CmdResetCreateTimeState))]
	[Union(21, typeof(CmdPauseState))]
	[Union(22, typeof(CmdRemoveState))]
	[Union(23, typeof(CmdAirborne))]
	[Union(24, typeof(CmdUpdateCharacterInvisible))]
	[Union(25, typeof(CmdUpdateInCombat))]
	[Union(26, typeof(CmdAddSight))]
	[Union(27, typeof(CmdRemoveSight))]
	[Union(28, typeof(CmdInSight))]
	[Union(29, typeof(CmdOutSight))]
	[MessagePackObject()]
	public abstract class LocalCharacterCommandPacket : ObjectCommandPacket
	{
		
		[IgnoreMember] protected new const int LAST_KEY_IDX = 0;

		
		public override void Action(ClientService service)
		{
			Action(service, service.World.Find<LocalCharacter>(objectId));
		}

		
		public abstract void Action(ClientService service, LocalCharacter self);
	}
}