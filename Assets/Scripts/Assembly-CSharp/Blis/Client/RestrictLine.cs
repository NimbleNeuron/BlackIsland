using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class RestrictLine : RestrictedEffect
	{
		public Material lineNormal;


		public Material lineReserved;


		public Material lineRestricted;


		public Renderer gradient;


		public Material gradientReserved;


		public Material gradientRestricted;


		private MeshRenderer meshRenderer;

		protected void Awake()
		{
			meshRenderer = GetComponent<MeshRenderer>();
		}


		public override void SetState(AreaRestrictionState areaState, float time)
		{
			if (areaState == AreaRestrictionState.Reserved)
			{
				gradient.gameObject.SetActive(true);
				gradient.material = gradientReserved;
				meshRenderer.material = lineReserved;
				return;
			}

			if (areaState != AreaRestrictionState.Restricted)
			{
				meshRenderer.material = lineNormal;
				gradient.gameObject.SetActive(false);
				return;
			}

			gradient.gameObject.SetActive(true);
			gradient.material = gradientRestricted;
			meshRenderer.material = lineRestricted;
		}
	}
}