using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class SkillEvolutionLevel : MonoBehaviour
	{
		private Image onImg;

		private void Awake()
		{
			onImg = gameObject.transform.GetChild(0).GetComponent<Image>();
		}


		public void Active(bool active)
		{
			gameObject.SetActive(active);
		}


		public void Enable(bool enable)
		{
			onImg.enabled = enable;
		}
	}
}