using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class MoveAgentSnapshot
	{
		[Key(8)] public bool isInBush;


		[Key(2)] public int lockRotation;


		[Key(1)] public byte[] moveStrategySnapshot;


		[Key(0)] public MoveStrategyType moveStrategyType;


		[Key(3)] public BlisVector startRotation;


		[Key(4)] public BlisVector targetRotation;


		[Key(5)] public BlisFixedPoint targetRotationPeriod;


		[Key(6)] public BlisFixedPoint targetRotationTimeStack;


		[Key(7)] public int walkableNavMask;
	}
}