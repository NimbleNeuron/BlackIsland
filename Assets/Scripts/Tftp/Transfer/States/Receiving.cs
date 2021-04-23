namespace Tftp.Net.Transfer.States
{
	
	internal class Receiving : StateThatExpectsMessagesFromDefaultEndPoint
	{
		
		public override void OnData(Data command)
		{
			if (command.BlockNumber != this.nextBlockNumber)
			{
				if (command.BlockNumber == this.lastBlockNumber)
				{
					this.SendAcknowledgement(command.BlockNumber);
				}
				return;
			}
			base.Context.InputOutputStream.Write(command.Bytes, 0, command.Bytes.Length);
			this.SendAcknowledgement(command.BlockNumber);
			if (command.Bytes.Length < base.Context.BlockSize)
			{
				base.Context.RaiseOnFinished();
				base.Context.SetState(new Closed());
				return;
			}
			this.lastBlockNumber = command.BlockNumber;
			this.nextBlockNumber = base.Context.BlockCounterWrapping.CalculateNextBlockNumber(command.BlockNumber);
			this.bytesReceived += (long)command.Bytes.Length;
			base.Context.RaiseOnProgress(this.bytesReceived);
		}

		
		public override void OnCancel(TftpErrorPacket reason)
		{
			base.Context.SetState(new CancelledByUser(reason));
		}

		
		public override void OnError(Error command)
		{
			base.Context.SetState(new ReceivedError(command));
		}

		
		private void SendAcknowledgement(ushort blockNumber)
		{
			Acknowledgement command = new Acknowledgement(blockNumber);
			base.Context.GetConnection().Send(command);
			base.ResetTimeout();
		}

		
		private ushort lastBlockNumber;

		
		private ushort nextBlockNumber = 1;

		
		private long bytesReceived;
	}
}
