using UnityEngine;

namespace Ara
{
	[RequireComponent(typeof(AraTrail))]
	public class ElectricalArc : MonoBehaviour
	{
		public Transform source;


		public Transform target;


		public int points = 20;


		public float burstInterval = 0.5f;


		public float burstRandom = 0.2f;


		public float speedRandom = 2f;


		public float positionRandom = 0.1f;


		private float accum;


		private AraTrail trail;


		private void Update()
		{
			accum += Time.deltaTime;
			if (accum >= burstInterval)
			{
				ChangeArc();
				accum = -burstInterval * Random.value * burstRandom;
			}
		}


		private void OnEnable()
		{
			trail = GetComponent<AraTrail>();
			trail.emit = false;
		}


		private void ChangeArc()
		{
			trail.points.Clear();
			if (source != null && target != null)
			{
				for (int i = 0; i < points; i++)
				{
					float num = i / (float) (points - 1);
					float d = Mathf.Sin(num * 3.1415927f);
					Vector3 a = Vector3.Lerp(source.position, target.position, num);
					trail.points.Add(new AraTrail.Point(a + Random.onUnitSphere * positionRandom * d,
						Random.onUnitSphere * speedRandom * d, Vector3.up, Vector3.forward, Color.white, 1f,
						burstInterval * 2f));
				}
			}
		}
	}
}