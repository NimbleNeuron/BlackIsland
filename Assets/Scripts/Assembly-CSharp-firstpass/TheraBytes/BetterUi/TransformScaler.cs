using UnityEngine;
using UnityEngine.Serialization;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Helpers/Transform Scaler", 30)]
	public class TransformScaler : ResolutionSizer<Vector3>
	{
		[FormerlySerializedAs("scaleSizer")] [SerializeField]
		private Vector3SizeModifier scaleSizerFallback =
			new Vector3SizeModifier(Vector3.one, Vector3.zero, 4f * Vector3.one);


		[SerializeField] private Vector3SizeConfigCollection customScaleSizers = new Vector3SizeConfigCollection();


		public Vector3SizeModifier ScaleSizer => customScaleSizers.GetCurrentItem(scaleSizerFallback);


		protected override ScreenDependentSize<Vector3> sizer => customScaleSizers.GetCurrentItem(scaleSizerFallback);


		protected override void ApplySize(Vector3 newSize)
		{
			transform.localScale = newSize;
		}
	}
}