using UnityEngine;
using UnityEngine.UI;

namespace Werewolf.StatusIndicators.Demo
{
	public class SplatName : MonoBehaviour
	{
		private void OnEnable()
		{
			GetComponent<Text>().text = "";
		}
	}
}