using UnityEngine;

namespace BIFog
{
	
	[DisallowMultipleComponent]
	public class FogHiderOnCollider : FogHiderBase
	{
		
		protected override void Awake()
		{
			this.hideRenderer = true;
			base.Awake();
		}

		
		public override void SetHideRenderer(bool hideRenderer)
		{
			this.hideRenderer = true;
		}
	}
}
