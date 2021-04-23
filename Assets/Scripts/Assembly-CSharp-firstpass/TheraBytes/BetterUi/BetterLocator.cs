using System;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("Better UI/Layout/Better Locator", 30)]
	public class BetterLocator : MonoBehaviour, IResolutionDependency
	{
		[SerializeField] private RectTransformData transformFallback = default;


		[SerializeField] private RectTransformDataConfigCollection transformConfigs = default;


		public RectTransformData CurrentTransformData => transformConfigs.GetCurrentItem(transformFallback);


		private RectTransform rectTransform => transform as RectTransform;


		private void OnEnable()
		{
			CurrentTransformData.PushToTransform(rectTransform);
		}


		public void OnResolutionChanged()
		{
			CurrentTransformData.PushToTransform(rectTransform);
		}


		[Serializable]
		public class RectTransformDataConfigCollection : SizeConfigCollection<RectTransformData> { }
	}
}