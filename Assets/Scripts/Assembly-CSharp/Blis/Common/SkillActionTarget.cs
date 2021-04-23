using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class SkillActionTarget
	{
		[Key(0)] public int targetId;


		[Key(1)] public BlisVector targetPos;
	}
}