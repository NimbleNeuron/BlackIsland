using System.IO;
using System.Text;

namespace Tftp.Net
{
	
	internal class CommandSerializer
	{
		
		public void Serialize(ITftpCommand command, Stream stream)
		{
			CommandSerializer.CommandComposerVisitor visitor = new CommandSerializer.CommandComposerVisitor(stream);
			command.Visit(visitor);
		}

		
		private class CommandComposerVisitor : ITftpCommandVisitor
		{
			
			public CommandComposerVisitor(Stream stream)
			{
				this.writer = new TftpStreamWriter(stream);
			}

			
			private void OnReadOrWriteRequest(ReadOrWriteRequest command)
			{
				this.writer.WriteBytes(Encoding.ASCII.GetBytes(command.Filename));
				this.writer.WriteByte(0);
				this.writer.WriteBytes(Encoding.ASCII.GetBytes(command.Mode.ToString()));
				this.writer.WriteByte(0);
				if (command.Options != null)
				{
					foreach (TransferOption transferOption in command.Options)
					{
						this.writer.WriteBytes(Encoding.ASCII.GetBytes(transferOption.Name));
						this.writer.WriteByte(0);
						this.writer.WriteBytes(Encoding.ASCII.GetBytes(transferOption.Value));
						this.writer.WriteByte(0);
					}
				}
			}

			
			public void OnReadRequest(ReadRequest command)
			{
				this.writer.WriteUInt16(1);
				this.OnReadOrWriteRequest(command);
			}

			
			public void OnWriteRequest(WriteRequest command)
			{
				this.writer.WriteUInt16(2);
				this.OnReadOrWriteRequest(command);
			}

			
			public void OnData(Data command)
			{
				this.writer.WriteUInt16(3);
				this.writer.WriteUInt16(command.BlockNumber);
				this.writer.WriteBytes(command.Bytes);
			}

			
			public void OnAcknowledgement(Acknowledgement command)
			{
				this.writer.WriteUInt16(4);
				this.writer.WriteUInt16(command.BlockNumber);
			}

			
			public void OnError(Error command)
			{
				this.writer.WriteUInt16(5);
				this.writer.WriteUInt16(command.ErrorCode);
				this.writer.WriteBytes(Encoding.ASCII.GetBytes(command.Message));
				this.writer.WriteByte(0);
			}

			
			public void OnOptionAcknowledgement(OptionAcknowledgement command)
			{
				this.writer.WriteUInt16(6);
				foreach (TransferOption transferOption in command.Options)
				{
					this.writer.WriteBytes(Encoding.ASCII.GetBytes(transferOption.Name));
					this.writer.WriteByte(0);
					this.writer.WriteBytes(Encoding.ASCII.GetBytes(transferOption.Value));
					this.writer.WriteByte(0);
				}
			}

			
			private readonly TftpStreamWriter writer;
		}
	}
}
