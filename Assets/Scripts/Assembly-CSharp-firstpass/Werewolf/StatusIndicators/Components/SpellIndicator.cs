using UnityEngine;

namespace Werewolf.StatusIndicators.Components
{
	public abstract class SpellIndicator : Splat
	{
		public RangeIndicator RangeIndicator;


		[SerializeField] protected float range = 5f;


		
		public float Range {
			get => range;
			set => SetRange(value);
		}


		public override void OnShow()
		{
			UpdateRangeIndicatorSize();
		}


		public void SetRange(float range)
		{
			this.range = range;
			UpdateRangeIndicatorSize();
		}


		protected Vector3 FlattenVector(Vector3 target)
		{
			return new Vector3(target.x, Manager.transform.position.y, target.z);
		}


		private void UpdateRangeIndicatorSize()
		{
			if (RangeIndicator != null)
			{
				RangeIndicator.Scale = range * 2.1f;
			}
		}
	}
}