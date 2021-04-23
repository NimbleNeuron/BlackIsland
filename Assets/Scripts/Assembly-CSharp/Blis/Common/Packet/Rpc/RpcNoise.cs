using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcNoise, false)]
	public class RpcNoise : RpcPacket
	{
		[Key(1)] public int creatorObjectId;


		[Key(0)] public BlisVector noisePos;


		public override void Action(ClientService clientService)
		{
			MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.MakeNoise(noisePos.ToVector3(), creatorObjectId);
		}
	}
}