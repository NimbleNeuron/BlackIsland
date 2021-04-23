using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[RequireComponent(typeof(LineRenderer))]
	public class BulletLine : MonoBehaviour
	{
		private Transform from;


		private LineRenderer lineRenderer;


		private Transform to;

		private void Awake()
		{
			lineRenderer = GetComponent<LineRenderer>();
		}


		private void Update()
		{
			if (from != null && to != null)
			{
				lineRenderer.SetPosition(0, from.transform.position);
				lineRenderer.SetPosition(1, to.transform.position);
				return;
			}

			lineRenderer.enabled = false;
		}


		public void Link(Transform from, Transform to)
		{
			this.from = from;
			this.to = to;
			lineRenderer.enabled = true;
		}


		public void SetMaterial(string resourceName)
		{
			Material effectMaterial = SingletonMonoBehaviour<ResourceManager>.inst.GetEffectMaterial(resourceName);
			lineRenderer.material = effectMaterial;
		}


		public void SetWidth(float startWidth, float endWidth)
		{
			lineRenderer.startWidth = startWidth;
			lineRenderer.endWidth = endWidth;
		}


		public void Unlink()
		{
			from = null;
			to = null;
			lineRenderer.enabled = false;
		}
	}
}