#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ACTk.Editor")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ACTk.Examples.Runtime")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ACTk.Examples.Genuine.Runtime")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ACTk.Examples.Genuine.Editor")]

namespace CodeStage.AntiCheat.Common
{
	public static class ACTkConstants
	{
		public const string Version = "2.1.3";
		
		internal const string LogPrefix = "[ACTk] ";
		internal const string DocsRootUrl = "http://codestage.net/uas_files/actk/api/";
		internal static readonly char[] StringKey = {'\x69', '\x108', '\x105', '\x110', '\x97'};
	}
}