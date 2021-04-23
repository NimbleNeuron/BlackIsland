using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[Union(0, typeof(CmdRest))]
	[Union(1, typeof(CmdRanking))]
	[Union(2, typeof(CmdMasteryLevelUp))]
	[Union(3, typeof(CmdStartActionCasting))]
	[Union(4, typeof(CmdCancelActionCasting))]
	[Union(5, typeof(CmdUpgradeSkill))]
	[Union(6, typeof(CmdUpdateSurvivableTime))]
	[Union(7, typeof(CmdStartGunReload))]
	[Union(8, typeof(CmdFinishGunReload))]
	[Union(9, typeof(CmdSwitchSkillSet))]
	[Union(10, typeof(CmdResetSkillCooldown))]
	[Union(11, typeof(CmdStartSkillCooldown))]
	[Union(12, typeof(CmdModifySkillCooldown))]
	[Union(13, typeof(CmdHoldSkillCooldown))]
	[Union(14, typeof(CmdEmotionCharacterVoice))]
	[Union(15, typeof(CmdEmotionIcon))]
	[Union(16, typeof(CmdUpdateExp))]
	[Union(17, typeof(CmdUpdateLevel))]
	[Union(18, typeof(CmdUpdateShield))]
	[Union(19, typeof(CmdDyingCondition))]
	[Union(20, typeof(CmdTeamRevival))]
	[Union(21, typeof(CmdConsumeBullet))]
	[Union(22, typeof(CmdConsumeSkillCost))]
	[Union(23, typeof(CmdHyperLoop))]
	[MessagePackObject()]
	public abstract class LocalPlayerCharacterCommandPacket : ObjectCommandPacket
	{
		
		[IgnoreMember] protected new const int LAST_KEY_IDX = 0;

		
		public override void Action(ClientService service)
		{
			Action(service, service.World.Find<LocalPlayerCharacter>(objectId));
		}

		
		public abstract void Action(ClientService service, LocalPlayerCharacter self);
	}
}