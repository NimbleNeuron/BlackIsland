using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionInstance : MonoBehaviour
{
	public GameObject[] EffectsOnCollision;


	public float Offset;


	public float DestroyTimeDelay = 5f;


	public bool UseWorldSpacePosition;


	public bool UseFirePointRotation;


	private readonly List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();


	private ParticleSystem part;


	private ParticleSystem ps;


	private void Start()
	{
		part = GetComponent<ParticleSystem>();
	}


	private void OnParticleCollision(GameObject other)
	{
		int num = part.GetCollisionEvents(other, collisionEvents);
		for (int i = 0; i < num; i++)
		{
			GameObject[] effectsOnCollision = EffectsOnCollision;
			for (int j = 0; j < effectsOnCollision.Length; j++)
			{
				GameObject gameObject = Instantiate<GameObject>(effectsOnCollision[j],
					collisionEvents[i].intersection + collisionEvents[i].normal * Offset, default);
				if (UseFirePointRotation)
				{
					gameObject.transform.LookAt(transform.position);
				}
				else
				{
					gameObject.transform.LookAt(collisionEvents[i].intersection + collisionEvents[i].normal);
				}

				if (!UseWorldSpacePosition)
				{
					gameObject.transform.parent = transform;
				}

				Destroy(gameObject, DestroyTimeDelay);
			}
		}

		Destroy(this.gameObject, DestroyTimeDelay + 0.5f);
	}
}