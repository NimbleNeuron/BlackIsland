namespace Blis.Client
{
	public class AnimationStateKey
	{
		public int layer;


		public string stateName;


		public override bool Equals(object obj)
		{
			return obj is AnimationStateKey && Equals(obj as AnimationStateKey);
		}


		protected bool Equals(AnimationStateKey other)
		{
			return layer == other.layer && string.Equals(stateName, other.stateName);
		}


		public override int GetHashCode()
		{
			return (layer * 397) ^ (stateName != null ? stateName.GetHashCode() : 0);
		}
	}
}