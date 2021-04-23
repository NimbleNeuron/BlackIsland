using System;

namespace Blis.Common
{
	public class RequestHolder
	{
		public readonly ReqPacketForResponse req;


		public readonly uint reqId;


		private Action<ServerPacketWrapper> callback;

		public RequestHolder(ReqPacketForResponse req, Action<ServerPacketWrapper> callback)
		{
			this.callback = callback;
			reqId = req.reqId;
			this.req = req;
		}


		public void OnResponse(ServerPacketWrapper packetWrapper)
		{
			Action<ServerPacketWrapper> action = callback;
			if (action != null)
			{
				action(packetWrapper);
			}

			callback = null;
		}
	}
}