using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
	public float speed = 15f;


	public float hitOffset;


	public bool UseFirePointRotation;


	public GameObject hit;


	public GameObject flash;


	private void Start()
	{
		if (flash != null)
		{
			GameObject gameObject = Instantiate<GameObject>(flash, transform.position, Quaternion.identity);
			gameObject.transform.forward = this.gameObject.transform.forward;
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			if (component == null)
			{
				Destroy(gameObject, component.main.duration);
				return;
			}

			ParticleSystem component2 = gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
			Destroy(gameObject, component2.main.duration);
		}
	}


	private void FixedUpdate()
	{
		if (speed != 0f)
		{
			transform.position += transform.forward * (speed * Time.deltaTime);
		}
	}


	private void OnCollisionEnter(Collision collision)
	{
		speed = 0f;
		ContactPoint contactPoint = collision.contacts[0];
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contactPoint.normal);
		Vector3 position = contactPoint.point + contactPoint.normal * hitOffset;
		if (hit != null)
		{
			GameObject gameObject = Instantiate<GameObject>(hit, position, rotation);
			if (UseFirePointRotation)
			{
				gameObject.transform.rotation = this.gameObject.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
			}
			else
			{
				gameObject.transform.LookAt(contactPoint.point + contactPoint.normal);
			}

			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			if (component == null)
			{
				Destroy(gameObject, component.main.duration);
			}
			else
			{
				ParticleSystem component2 = gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
				Destroy(gameObject, component2.main.duration);
			}
		}

		Destroy(this.gameObject);
	}
}