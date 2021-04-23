using UnityEngine;
using Werewolf.StatusIndicators.Components;

namespace Werewolf.StatusIndicators.Demo
{
	public class DemoProgressLoop : MonoBehaviour
	{
		private Splat splat;


		private void Start()
		{
			splat = GetComponent<Splat>();
		}


		private void Update()
		{
			splat.Progress = Mathf.PingPong(Time.time * 0.5f, 1f);
		}
	}
}