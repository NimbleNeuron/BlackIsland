using System.Collections.Generic;
using UnityEngine;

namespace BIOutline
{
	[DisallowMultipleComponent]
	public class Outliner : MonoBehaviour
	{
		public OutlineType outlineType;


		public Color color = Color.black;


		public readonly List<OutlineDrawCall> drawCalls = new List<OutlineDrawCall>();


		private readonly List<Renderer> organizeDrawCallRenderers = new List<Renderer>();


		private bool isNeedOrganizeDrawCall;


		private OutlineManager outlineManager;


		private Transform transformCache;


		public Transform Transform => transformCache;


		private void Awake()
		{
			transformCache = transform;
			OrganizeDrawCall();
		}


		private void Update()
		{
			if (!isNeedOrganizeDrawCall)
			{
				return;
			}

			isNeedOrganizeDrawCall = false;
			drawCalls.Clear();
			GetComponentsInChildren<Renderer>(false, organizeDrawCallRenderers);
			drawCalls.Capacity = organizeDrawCallRenderers.Count;
			foreach (Renderer renderer in organizeDrawCallRenderers)
			{
				IgnoreOutlineRenderer ignoreOutlineRenderer;
				if (!(renderer is ParticleSystemRenderer) &&
				    !renderer.TryGetComponent<IgnoreOutlineRenderer>(out ignoreOutlineRenderer))
				{
					drawCalls.Add(new OutlineDrawCall(renderer));
				}
			}
		}


		private void OnEnable()
		{
			Unregister();
			if (Camera.main != null)
			{
				outlineManager = Camera.main.GetComponent<OutlineManager>();
				if (outlineManager != null)
				{
					outlineManager.RegisterOutliner(this);
				}
			}
		}


		private void OnDisable()
		{
			Unregister();
		}


		private void OnTransformChildrenChanged()
		{
			OrganizeDrawCall();
		}


		public void OrganizeDrawCall()
		{
			isNeedOrganizeDrawCall = true;
		}


		private void Unregister()
		{
			if (outlineManager != null)
			{
				outlineManager.UnregisterOutliner(this);
			}
		}
	}
}