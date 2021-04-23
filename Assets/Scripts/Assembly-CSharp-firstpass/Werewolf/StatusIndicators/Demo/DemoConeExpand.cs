using UnityEngine;
using Werewolf.StatusIndicators.Components;

namespace Werewolf.StatusIndicators.Demo
{
	public class DemoConeExpand : MonoBehaviour
	{
		private Cone spellIndicator;


		private void Start()
		{
			spellIndicator = GetComponent<Cone>();
		}


		private void Update()
		{
			spellIndicator.Angle = Mathf.PingPong(Time.time * 100f, 320f) + 40f;
		}
	}
}