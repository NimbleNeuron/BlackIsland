using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tftp.Net
{
	
	internal class CommandParser
	{
		
		public ITftpCommand Parse(byte[] message)
		{
			ITftpCommand result;
			try
			{
				result = this.ParseInternal(message);
			}
			catch (TftpParserException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new TftpParserException(e);
			}
			return result;
		}

		
		private ITftpCommand ParseInternal(byte[] message)
		{
			TftpStreamReader tftpStreamReader = new TftpStreamReader(new MemoryStream(message));
			switch (tftpStreamReader.ReadUInt16())
			{
			case 1:
				return this.ParseReadRequest(tftpStreamReader);
			case 2:
				return this.ParseWriteRequest(tftpStreamReader);
			case 3:
				return this.ParseData(tftpStreamReader);
			case 4:
				return this.ParseAcknowledgement(tftpStreamReader);
			case 5:
				return this.ParseError(tftpStreamReader);
			case 6:
				return this.ParseOptionAcknowledgement(tftpStreamReader);
			default:
				throw new TftpParserException("Invalid opcode");
			}
		}

		
		private OptionAcknowledgement ParseOptionAcknowledgement(TftpStreamReader reader)
		{
			return new OptionAcknowledgement(this.ParseTransferOptions(reader));
		}

		
		private Error ParseError(TftpStreamReader reader)
		{
			ushort errorCode = reader.ReadUInt16();
			string message = this.ParseNullTerminatedString(reader);
			return new Error(errorCode, message);
		}

		
		private Acknowledgement ParseAcknowledgement(TftpStreamReader reader)
		{
			return new Acknowledgement(reader.ReadUInt16());
		}

		
		private Data ParseData(TftpStreamReader reader)
		{
			ushort blockNumber = reader.ReadUInt16();
			byte[] data = reader.ReadBytes(10000);
			return new Data(blockNumber, data);
		}

		
		private WriteRequest ParseWriteRequest(TftpStreamReader reader)
		{
			string filename = this.ParseNullTerminatedString(reader);
			TftpTransferMode mode = this.ParseModeType(this.ParseNullTerminatedString(reader));
			IEnumerable<TransferOption> options = this.ParseTransferOptions(reader);
			return new WriteRequest(filename, mode, options);
		}

		
		private ReadRequest ParseReadRequest(TftpStreamReader reader)
		{
			string filename = this.ParseNullTerminatedString(reader);
			TftpTransferMode mode = this.ParseModeType(this.ParseNullTerminatedString(reader));
			IEnumerable<TransferOption> options = this.ParseTransferOptions(reader);
			return new ReadRequest(filename, mode, options);
		}

		
		private List<TransferOption> ParseTransferOptions(TftpStreamReader reader)
		{
			List<TransferOption> list = new List<TransferOption>();
			for (;;)
			{
				string text;
				try
				{
					text = this.ParseNullTerminatedString(reader);
				}
				catch (IOException)
				{
					text = "";
				}
				if (text.Length == 0)
				{
					break;
				}
				string value = this.ParseNullTerminatedString(reader);
				list.Add(new TransferOption(text, value));
			}
			return list;
		}

		
		private string ParseNullTerminatedString(TftpStreamReader reader)
		{
			StringBuilder stringBuilder = new StringBuilder();
			byte value;
			while ((value = reader.ReadByte()) > 0)
			{
				stringBuilder.Append((char)value);
			}
			return stringBuilder.ToString();
		}

		
		private TftpTransferMode ParseModeType(string mode)
		{
			mode = mode.ToLowerInvariant();
			if (mode == "netascii")
			{
				return TftpTransferMode.netascii;
			}
			if (mode == "mail")
			{
				return TftpTransferMode.mail;
			}
			if (mode == "octet")
			{
				return TftpTransferMode.octet;
			}
			throw new TftpParserException("Unknown mode type: " + mode);
		}
	}
}
