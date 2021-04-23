using System;

namespace ProceduralWorlds.RealIvy
{
	[Serializable]
	public class IvyParameterFloat : IvyParameter
	{
		public IvyParameterFloat(float value)
		{
			this.value = value;
		}


		public override void UpdateValue(float value)
		{
			this.value = value;
		}


		public static implicit operator float(IvyParameterFloat floatParameter)
		{
			return floatParameter.value;
		}


		public static implicit operator IvyParameterFloat(float floatValue)
		{
			return new IvyParameterFloat(floatValue);
		}
	}
}