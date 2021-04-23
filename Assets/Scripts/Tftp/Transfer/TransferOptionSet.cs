using System.Collections.Generic;

namespace Tftp.Net.Transfer
{
	
	internal class TransferOptionSet
	{
		
		public static TransferOptionSet NewDefaultSet()
		{
			return new TransferOptionSet
			{
				IncludesBlockSizeOption = true,
				IncludesTimeoutOption = true,
				IncludesTransferSizeOption = true
			};
		}

		
		public static TransferOptionSet NewEmptySet()
		{
			return new TransferOptionSet
			{
				IncludesBlockSizeOption = false,
				IncludesTimeoutOption = false,
				IncludesTransferSizeOption = false
			};
		}

		
		private TransferOptionSet()
		{
		}

		
		public TransferOptionSet(IEnumerable<TransferOption> options)
		{
			this.IncludesBlockSizeOption = (this.IncludesTimeoutOption = (this.IncludesTransferSizeOption = false));
			foreach (TransferOption option in options)
			{
				this.Parse(option);
			}
		}

		
		private void Parse(TransferOption option)
		{
			string name = option.Name;
			if (name == "blksize")
			{
				this.IncludesBlockSizeOption = this.ParseBlockSizeOption(option.Value);
				return;
			}
			if (name == "timeout")
			{
				this.IncludesTimeoutOption = this.ParseTimeoutOption(option.Value);
				return;
			}
			if (!(name == "tsize"))
			{
				return;
			}
			this.IncludesTransferSizeOption = this.ParseTransferSizeOption(option.Value);
		}

		
		public List<TransferOption> ToOptionList()
		{
			List<TransferOption> list = new List<TransferOption>();
			if (this.IncludesBlockSizeOption)
			{
				list.Add(new TransferOption("blksize", this.BlockSize.ToString()));
			}
			if (this.IncludesTimeoutOption)
			{
				list.Add(new TransferOption("timeout", this.Timeout.ToString()));
			}
			if (this.IncludesTransferSizeOption)
			{
				list.Add(new TransferOption("tsize", this.TransferSize.ToString()));
			}
			return list;
		}

		
		private bool ParseTransferSizeOption(string value)
		{
			return long.TryParse(value, out this.TransferSize) && this.TransferSize >= 0L;
		}

		
		private bool ParseTimeoutOption(string value)
		{
			int num;
			if (!int.TryParse(value, out num))
			{
				return false;
			}
			if (num < 1 || num > 255)
			{
				return false;
			}
			this.Timeout = num;
			return true;
		}

		
		private bool ParseBlockSizeOption(string value)
		{
			int num;
			if (!int.TryParse(value, out num))
			{
				return false;
			}
			if (num < 8 || num > 65464)
			{
				return false;
			}
			this.BlockSize = num;
			return true;
		}

		
		public const int DEFAULT_BLOCKSIZE = 512;

		
		public const int DEFAULT_TIMEOUT_SECS = 5;

		
		public bool IncludesBlockSizeOption;

		
		public int BlockSize = 512;

		
		public bool IncludesTimeoutOption;

		
		public int Timeout = 5;

		
		public bool IncludesTransferSizeOption;

		
		public long TransferSize;
	}
}
