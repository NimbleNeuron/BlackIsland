using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class NicknamePair
	{
		[Key(0)] public string original;


		[Key(1)] public string temp;


		public NicknamePair(string original, string temp)
		{
			this.original = original;
			this.temp = temp;
		}
	}
}