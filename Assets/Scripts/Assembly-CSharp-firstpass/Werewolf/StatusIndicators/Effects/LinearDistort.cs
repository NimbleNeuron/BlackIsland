using UnityEngine;

namespace Werewolf.StatusIndicators.Effects
{
	public class LinearDistort : MonoBehaviour
	{
		public float XSpeed;


		public float YSpeed;


		private Material Material;


		private void Start()
		{
			Material = GetComponent<Projector>().material;
		}


		private void Update()
		{
			Material.SetFloat("_OffsetX", Mathf.Repeat(Time.time * XSpeed, 1f));
			Material.SetFloat("_OffsetY", Mathf.Repeat(Time.time * YSpeed, 1f));
		}
	}
}