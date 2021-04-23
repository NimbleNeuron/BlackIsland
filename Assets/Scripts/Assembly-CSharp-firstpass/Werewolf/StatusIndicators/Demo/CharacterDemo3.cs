using UnityEngine;
using Werewolf.StatusIndicators.Components;

namespace Werewolf.StatusIndicators.Demo
{
	public class CharacterDemo3 : MonoBehaviour
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
				Splats.CancelSpellIndicator();
				Splats.CancelRangeIndicator();
				Splats.CancelStatusIndicator();
			}

			if (Input.GetKeyDown(KeyCode.Q))
			{
				Splats.SelectSpellIndicator("Point");
			}

			if (Input.GetKeyDown(KeyCode.W))
			{
				Splats.SelectSpellIndicator("Cone");
			}

			if (Input.GetKeyDown(KeyCode.E))
			{
				Splats.SelectSpellIndicator("Direction");
			}

			if (Input.GetKeyDown(KeyCode.R))
			{
				Splats.SelectSpellIndicator("Line");
			}

			if (Input.GetKeyDown(KeyCode.S))
			{
				Splats.SelectStatusIndicator("Status");
			}

			if (Input.GetKeyDown(KeyCode.D))
			{
				Splats.SelectRangeIndicator("Range");
			}
		}
	}
}