using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class DummyRpcPacketForEnumTypeFormatter : IPacket
	{
		
		[IgnoreMember] protected const int LAST_KEY_IDX = -1;

		
		[Key(0)] public AreaRestrictionAnnounceType areaRestrictionAnnounceType;

		
		[Key(1)] public LastSafeConsoleAnnounceType lastSafeConsoleAnnounceType;
	}
}