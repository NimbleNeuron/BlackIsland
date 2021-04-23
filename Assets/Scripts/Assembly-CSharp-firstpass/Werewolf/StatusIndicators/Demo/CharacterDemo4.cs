using UnityEngine;
using Werewolf.StatusIndicators.Components;

namespace Werewolf.StatusIndicators.Demo
{
	public class CharacterDemo4 : MonoBehaviour
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
			}

			if (Input.GetKeyDown(KeyCode.Q))
			{
				Splats.SelectSpellIndicator("Fireball");
			}

			if (Input.GetKeyDown(KeyCode.W))
			{
				Splats.SelectSpellIndicator("Frost Blast");
			}

			if (Input.GetKeyDown(KeyCode.E))
			{
				Splats.SelectSpellIndicator("Frost Nova");
			}
		}
	}
}