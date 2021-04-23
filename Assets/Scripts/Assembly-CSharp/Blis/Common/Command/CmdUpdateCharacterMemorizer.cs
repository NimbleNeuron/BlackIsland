using System.Collections.Generic;
using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdUpdateCharacterMemorizer, false)]
	public class CmdUpdateCharacterMemorizer : CommandPacket
	{
		
		[Key(0)] public HashSet<int> memorizedTargets;

		
		public override void Action(ClientService service)
		{
			foreach (int objectId in memorizedTargets)
			{
				service.World.Find<LocalCharacter>(objectId).SightAgent.AddMemorizerPlayer(service.MyObjectId);
			}
		}
	}
}