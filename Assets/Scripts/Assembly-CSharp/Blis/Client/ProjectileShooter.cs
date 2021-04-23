using System.Collections.Generic;
using UnityEngine;

namespace Blis.Client
{
	public class ProjectileShooter : MonoBehaviour
	{
		public GameObject projectileSource;


		public float CreateDelay = 3f;


		public float MoveDistance = 12f;


		public float Speed = 18.75f;


		private readonly List<GameObject> projectiles = new List<GameObject>();


		private float deltaMove;


		private float deltaTime;

		public void Update()
		{
			CreateProjectile();
			UpdateProjectile();
		}


		private void CreateProjectile()
		{
			if (projectileSource == null)
			{
				return;
			}

			deltaTime += Time.deltaTime;
			if (CreateDelay < 1f)
			{
				CreateDelay = 1f;
			}

			if (deltaTime < CreateDelay)
			{
				return;
			}

			projectiles.Add(Instantiate<GameObject>(projectileSource, transform.position, Quaternion.identity));
			deltaTime = 0f;
		}


		private void UpdateProjectile()
		{
			for (int i = projectiles.Count - 1; i >= 0; i--)
			{
				if (MoveDistance <= (transform.position - projectiles[i].transform.position).magnitude)
				{
					foreach (ParticleSystem particleSystem in projectiles[i].GetComponentsInChildren<ParticleSystem>())
					{
						ParticleSystem.MainModule main = particleSystem.main;
						if (main.loop)
						{
							main.loop = false;
						}
						else
						{
							Renderer component = particleSystem.GetComponent<Renderer>();
							if (component != null)
							{
								component.enabled = false;
							}
						}
					}

					Destroy(projectiles[i], 10f);
					projectiles.RemoveAt(i);
				}
				else
				{
					projectiles[i].transform.position += Time.deltaTime * Speed * projectiles[i].transform.forward;
				}
			}
		}
	}
}