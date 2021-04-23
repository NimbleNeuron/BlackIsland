using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	public class ObserverAgent
	{
		
		public void Register(PlayerSession playerSession)
		{
			this.observers[playerSession.userId] = playerSession;
		}

		
		public void Deregister(PlayerSession playerSession)
		{
			if (this.observers.ContainsKey(playerSession.userId))
			{
				this.observers.Remove(playerSession.userId);
			}
		}

		
		public void Send(RpcPacket packet)
		{
			foreach (PlayerSession session in this.observers.Values)
			{
				MonoBehaviourInstance<GameServer>.inst.Send(session, packet, NetChannel.ReliableOrdered);
			}
		}

		
		private readonly Dictionary<long, PlayerSession> observers = new Dictionary<long, PlayerSession>();
	}
}
