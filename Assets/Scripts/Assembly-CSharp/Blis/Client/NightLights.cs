using System.Collections.Generic;
using UnityEngine;

namespace Blis.Client
{
	public class NightLights : MonoBehaviour
	{
		private readonly List<Animator> animators = new List<Animator>();


		private readonly List<Light> lights = new List<Light>();


		private readonly List<Renderer> renderers = new List<Renderer>();

		public void Awake()
		{
			gameObject.GetComponentsInChildren<Animator>(animators);
			gameObject.GetComponentsInChildren<Light>(lights);
			gameObject.GetComponentsInChildren<Renderer>(renderers);
			Disable();
		}


		public void Enable()
		{
			foreach (Animator animator in animators)
			{
				animator.enabled = true;
			}

			foreach (Light light in lights)
			{
				light.enabled = true;
			}

			foreach (Renderer renderer in renderers)
			{
				renderer.enabled = true;
			}
		}


		public void Disable()
		{
			foreach (Animator animator in animators)
			{
				animator.enabled = false;
			}

			foreach (Light light in lights)
			{
				light.enabled = false;
			}

			foreach (Renderer renderer in renderers)
			{
				renderer.enabled = false;
			}
		}
	}
}