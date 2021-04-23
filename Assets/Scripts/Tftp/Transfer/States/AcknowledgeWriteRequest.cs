namespace Tftp.Net.Transfer.States
{
	
	internal class AcknowledgeWriteRequest : StateThatExpectsMessagesFromDefaultEndPoint
	{
		
		public override void OnStateEnter()
		{
			base.OnStateEnter();
			base.SendAndRepeat(new Acknowledgement(0));
		}

		
		public override void OnData(Data command)
		{
			Receiving receiving = new Receiving();
			base.Context.SetState(receiving);
			receiving.OnCommand(command, base.Context.GetConnection().RemoteEndpoint);
		}

		
		public override void OnCancel(TftpErrorPacket reason)
		{
			base.Context.SetState(new CancelledByUser(reason));
		}

		
		public override void OnError(Error command)
		{
			base.Context.SetState(new ReceivedError(command));
		}
	}
}
