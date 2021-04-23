using System.Collections.Generic;
using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.RpcUpdateRestrictedArea, false)]
	public class RpcUpdateRestrictedArea : RpcPacket
	{
		
		[Key(1)] public Dictionary<int, AreaRestrictionState> areaStateMap;

		
		[Key(2)] public DayNight dayNight;

		
		[Key(0)] public int remainTime;

		
		public override void Action(ClientService clientService)
		{
			clientService.UpdateRestrictedArea(areaStateMap, remainTime, dayNight);
		}
	}
}