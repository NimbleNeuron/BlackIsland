using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcFinishTutorial, false)]
	public class RpcFinishTutorial : RpcPacket
	{
		[Key(3)] public int attackerDataCode;


		[Key(4)] public string attackerNickName;


		[Key(2)] public int attackerObjectId;


		[Key(1)] public ObjectType attackerObjectType;


		[Key(0)] public int rank;


		public override void Action(ClientService clientService)
		{
			MonoBehaviourInstance<ClientService>.inst.EndTutorial(rank, attackerObjectType, attackerObjectId,
				attackerDataCode, attackerNickName);
		}
	}
}