using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	[Serializable]
	public class RuntimeGrowthParameters
	{
		public float growthSpeed;


		public float lifetime;


		public bool speedOverLifetimeEnabled;


		public AnimationCurve speedOverLifetimeCurve;


		public float delay;


		public bool startGrowthOnAwake;


		public RuntimeGrowthParameters()
		{
			growthSpeed = 25f;
			lifetime = 5f;
			speedOverLifetimeEnabled = false;
			speedOverLifetimeCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.2f, 1f),
				new Keyframe(0.8f, 1f), new Keyframe(1f, 0f));
			delay = 0f;
			startGrowthOnAwake = true;
		}
	}
}