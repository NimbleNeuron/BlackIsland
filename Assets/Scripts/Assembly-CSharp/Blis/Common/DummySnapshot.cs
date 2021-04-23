using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class DummySnapshot : CharacterSnapshot
	{
		[Key(7)] public int prefabNo;
	}
}