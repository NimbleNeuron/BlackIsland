using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[Union(0, typeof(RpcJoinUser))]
	[Union(1, typeof(RpcSetupGame))]
	[Union(2, typeof(RpcStartGame))]
	[Union(3, typeof(RpcUpdateStrategy))]
	[Union(4, typeof(RpcUpdateRestrictedArea))]
	[Union(5, typeof(RpcStopRestrictedArea))]
	[Union(6, typeof(RpcNoise))]
	[Union(7, typeof(RpcOpenItemBox))]
	[Union(8, typeof(RpcGameAnnounce))]
	[Union(9, typeof(RpcNoticeAirSupply))]
	[Union(10, typeof(RpcNoticeWicklineSpawnStart))]
	[Union(11, typeof(RpcNoticeWicklineKilled))]
	[Union(12, typeof(RpcSpawnAirSupply))]
	[Union(13, typeof(RpcBattleResultKey))]
	[Union(14, typeof(RpcExitGame))]
	[Union(15, typeof(RpcFinishTutorial))]
	[Union(16, typeof(RpcError))]
	[Union(17, typeof(RpcItemBoxAdd))]
	[Union(18, typeof(RpcItemBoxRemove))]
	[Union(19, typeof(RpcUpdateInventory))]
	[Union(20, typeof(RpcCompleteMakeItem))]
	[Union(21, typeof(RpcUpdateBeforeStart))]
	[Union(22, typeof(RpcChat))]
	[Union(23, typeof(RpcObserving))]
	[Union(24, typeof(RpcToastMessage))]
	[Union(25, typeof(RpcSkillSlotLock))]
	[Union(26, typeof(RpcSkillReserveCancel))]
	[Union(27, typeof(RpcVisitedNewArea))]
	[Union(28, typeof(RpcFinishGame))]
	[MessagePackObject()]
	public abstract class RpcPacket : IPacket
	{
		[IgnoreMember] protected const int LAST_KEY_IDX = -1;

		public abstract void Action(ClientService clientService);
	}
}