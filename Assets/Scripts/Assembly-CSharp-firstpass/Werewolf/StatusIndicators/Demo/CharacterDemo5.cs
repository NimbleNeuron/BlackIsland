using UnityEngine;
using Werewolf.StatusIndicators.Components;

namespace Werewolf.StatusIndicators.Demo
{
	public class CharacterDemo5 : MonoBehaviour
	{
		
		public SplatManager Splats { get; set; }


		private void Start()
		{
			Splats = GetComponentInChildren<SplatManager>();
		}


		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Splats.CancelRangeIndicator();
			}

			if (Input.GetKeyDown(KeyCode.Q))
			{
				Splats.SelectRangeIndicator("Range1");
				Splats.CurrentRangeIndicator.Scale = 14f;
			}

			if (Input.GetKeyDown(KeyCode.W))
			{
				Splats.SelectRangeIndicator("Range2");
				Splats.CurrentRangeIndicator.Scale = 15f;
			}

			if (Input.GetKeyDown(KeyCode.E))
			{
				Splats.SelectRangeIndicator("Range3");
				Splats.CurrentRangeIndicator.Scale = 16f;
			}

			if (Input.GetKeyDown(KeyCode.S))
			{
				Splats.SelectRangeIndicator("RangeSmall");
				Splats.CurrentRangeIndicator.Scale = 10f;
			}

			if (Input.GetKeyDown(KeyCode.D))
			{
				Splats.SelectRangeIndicator("RangeBasic");
				Splats.CurrentRangeIndicator.Scale = 12f;
			}
		}
	}
}