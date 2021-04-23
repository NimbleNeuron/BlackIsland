using System;

namespace Tftp.Net.Transfer.States
{
	
	internal class Sending : StateThatExpectsMessagesFromDefaultEndPoint
	{
		
		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.lastData = new byte[base.Context.BlockSize];
			this.SendNextPacket(1);
		}

		
		public override void OnAcknowledgement(Acknowledgement command)
		{
			if (command.BlockNumber != this.lastBlockNumber)
			{
				return;
			}
			this.bytesSent += (long)this.lastData.Length;
			base.Context.RaiseOnProgress(this.bytesSent);
			if (this.lastPacketWasSent)
			{
				base.Context.RaiseOnFinished();
				base.Context.SetState(new Closed());
				return;
			}
			this.SendNextPacket(base.Context.BlockCounterWrapping.CalculateNextBlockNumber(this.lastBlockNumber));
		}

		
		public override void OnError(Error command)
		{
			base.Context.SetState(new ReceivedError(command));
		}

		
		public override void OnCancel(TftpErrorPacket reason)
		{
			base.Context.SetState(new CancelledByUser(reason));
		}

		
		private void SendNextPacket(ushort blockNumber)
		{
			if (base.Context.InputOutputStream == null)
			{
				return;
			}
			int num = base.Context.InputOutputStream.Read(this.lastData, 0, this.lastData.Length);
			this.lastBlockNumber = blockNumber;
			if (num != this.lastData.Length)
			{
				this.lastPacketWasSent = true;
				Array.Resize<byte>(ref this.lastData, num);
			}
			ITftpCommand command = new Data(blockNumber, this.lastData);
			base.SendAndRepeat(command);
		}

		
		private byte[] lastData;

		
		private ushort lastBlockNumber;

		
		private long bytesSent;

		
		private bool lastPacketWasSent;
	}
}
