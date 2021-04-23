using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[Union(0, typeof(ServerCharacterStatValue))]
	[MessagePackObject()]
	public abstract class CharacterStatValue
	{
		[IgnoreMember] public const float FixedPointBias = 100f;


		[Key(0)] public readonly StatType statType;

		[SerializationConstructor]
		public CharacterStatValue(StatType statType)
		{
			this.statType = statType;
		}


		public abstract void Set(float v);


		public abstract void Set(int v);


		public abstract int GetIntValue();


		public abstract int GetIntValue(bool shuffle);


		public abstract float GetValue();


		public abstract float GetValue(bool shuffle);
	}
}