namespace Tftp.Net
{
	
	internal static class BlockCounterWrappingHelpers
	{
		
		public static ushort CalculateNextBlockNumber(this BlockCounterWrapAround wrapping, ushort previousBlockNumber)
		{
			if (previousBlockNumber != 65535)
			{
				return (ushort) (previousBlockNumber + 1);
			}
			if (wrapping != BlockCounterWrapAround.ToZero)
			{
				return 1;
			}
			return 0;
		}

		
		private const ushort LAST_AVAILABLE_BLOCK_NUMBER = 65535;
	}
}
