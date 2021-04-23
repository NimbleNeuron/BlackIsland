using System;

namespace Neptune.Log
{
	
	[Flags]
	public enum LogLevel
	{
		None = 0,
		Log = 1,
		Warning = 2,
		Error = 4,
		Exception = 8,
		All = 15
	}
}
