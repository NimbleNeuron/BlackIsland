using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[Union(0, typeof(ObjectCommandPacket))]
	[Union(1, typeof(CmdUpdateStat))]
	[Union(2, typeof(CmdInteract))]
	[Union(3, typeof(CmdUpdateMastery))]
	[Union(4, typeof(CmdEvolutionSkill))]
	[Union(5, typeof(CmdUpdateSkillPoint))]
	[Union(6, typeof(CmdUpdateSkillEvolutionPoint))]
	[Union(7, typeof(CmdUpdateCharacterMemorizer))]
	[Union(8, typeof(CmdSpawn))]
	[Union(9, typeof(CmdSpawns))]
	[Union(10, typeof(CmdAddItemCooldown))]
	[Union(11, typeof(CmdRemoveItemCooldown))]
	[Union(12, typeof(CmdResetSkillSequence))]
	[Union(13, typeof(CmdSetSkillSequence))]
	[Union(14, typeof(CmdActiveHyperLoopExit))]
	[Union(15, typeof(CmdCancelHyperLoopExit))]
	[Union(16, typeof(CmdSystemChat))]
	[Union(17, typeof(CmdUpdateMarkTarget))]
	[Union(18, typeof(CmdPing))]
	[Union(19, typeof(CmdUserDisconnected))]
	[Union(20, typeof(CmdFinishGame))]
	[Union(21, typeof(CmdFinishGameTeamAlive))]
	[Union(22, typeof(CmdChangeToObserver))]
	[Union(23, typeof(CmdUserConnected))]
	[MessagePackObject()]
	public abstract class CommandPacket : IPacket
	{
		[IgnoreMember] protected const int LAST_KEY_IDX = -1;

		public abstract void Action(ClientService service);
	}
}