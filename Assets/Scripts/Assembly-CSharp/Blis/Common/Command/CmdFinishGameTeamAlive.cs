using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdFinishGameTeamAlive, false)]
	public class CmdFinishGameTeamAlive : CommandPacket
	{
		
		[Key(2)] public int attackerDataCode;

		
		[Key(3)] public string attackerNickName;

		
		[Key(1)] public int attackerObjectId;

		
		[Key(0)] public ObjectType attackerObjectType;

		
		[Key(4)] public string attackerTempNickname;

		
		public override void Action(ClientService clientService)
		{
			clientService.EndGameTeamAlive(attackerObjectType, attackerObjectId, attackerDataCode, attackerNickName,
				attackerTempNickname);
		}
	}
}