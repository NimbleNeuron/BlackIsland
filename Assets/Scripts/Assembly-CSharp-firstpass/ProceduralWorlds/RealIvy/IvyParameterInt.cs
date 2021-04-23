using System;

namespace ProceduralWorlds.RealIvy
{
	[Serializable]
	public class IvyParameterInt : IvyParameter
	{
		public IvyParameterInt(int value)
		{
			this.value = value;
		}


		public override void UpdateValue(float value)
		{
			this.value = (int) value;
		}


		public static implicit operator int(IvyParameterInt intParameter)
		{
			return (int) intParameter.value;
		}


		public static implicit operator IvyParameterInt(int intValue)
		{
			return new IvyParameterInt(intValue);
		}
	}
}