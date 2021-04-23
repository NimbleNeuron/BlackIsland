namespace Blis.Client
{
	public class AnimationParam
	{
		public string animatorName;


		public int eventIndex;


		public int layer;


		public object param;


		public int[] syncLayerIndices;


		public string tag;

		public AnimationParam(int layer, int[] syncLayerIndices, string tag, object param, int eventIndex,
			string animatorName)
		{
			this.layer = layer;
			this.syncLayerIndices = syncLayerIndices;
			this.tag = tag;
			this.param = param;
			this.eventIndex = eventIndex;
			this.animatorName = animatorName;
		}


		public AnimationParam() { }


		public override int GetHashCode()
		{
			return (int) new StringTo64bitHashCodeBuilderKnuth().AddClassType(layer).AddString(tag)
				.AddString(param.GetType().Name).AddClassType(param).AddString(animatorName).AddClassType(eventIndex)
				.Build();
		}


		public override string ToString()
		{
			return layer + tag + param;
		}


		public static object Clone(object instance)
		{
			if (instance is BaseParam)
			{
				return ((BaseParam) instance).Clone();
			}

			return instance;
		}
	}
}