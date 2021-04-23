using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcUpdateStrategy, false)]
	public class RpcUpdateStrategy : RpcPacket
	{
		[Key(2)] public int startingAreaCode;


		[Key(1)] public int teamNumber;


		[Key(0)] public long userId;


		public override void Action(ClientService clientService)
		{
			int teamSlot = clientService.GetTeamSlot(userId);
			MonoBehaviourInstance<GameUI>.inst.Events.OnUpdateStrategySheets(userId, teamNumber, teamSlot,
				startingAreaCode);
		}
	}
}