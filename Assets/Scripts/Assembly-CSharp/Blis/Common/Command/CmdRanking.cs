using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdRanking, false)]
	public class CmdRanking : LocalPlayerCharacterCommandPacket
	{
		[Key(2)] public int rank;


		[Key(1)] public int teamNumber;


		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			service.AddUserRank(objectId, teamNumber, rank);
		}
	}
}