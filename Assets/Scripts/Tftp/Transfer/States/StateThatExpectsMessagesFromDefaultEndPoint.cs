using System;
using System.Net;

namespace Tftp.Net.Transfer.States
{
	
	internal class StateThatExpectsMessagesFromDefaultEndPoint : StateWithNetworkTimeout, ITftpCommandVisitor
	{
		
		public override void OnCommand(ITftpCommand command, EndPoint endpoint)
		{
			if (!endpoint.Equals(base.Context.GetConnection().RemoteEndpoint))
			{
				throw new Exception(string.Concat(new object[]
				{
					"Received message from illegal endpoint. Actual: ",
					endpoint,
					". Expected: ",
					base.Context.GetConnection().RemoteEndpoint
				}));
			}
			command.Visit(this);
		}

		
		public virtual void OnReadRequest(ReadRequest command)
		{
			throw new NotImplementedException();
		}

		
		public virtual void OnWriteRequest(WriteRequest command)
		{
			throw new NotImplementedException();
		}

		
		public virtual void OnData(Data command)
		{
			throw new NotImplementedException();
		}

		
		public virtual void OnAcknowledgement(Acknowledgement command)
		{
			throw new NotImplementedException();
		}

		
		public virtual void OnError(Error command)
		{
			throw new NotImplementedException();
		}

		
		public virtual void OnOptionAcknowledgement(OptionAcknowledgement command)
		{
			throw new NotImplementedException();
		}
	}
}
