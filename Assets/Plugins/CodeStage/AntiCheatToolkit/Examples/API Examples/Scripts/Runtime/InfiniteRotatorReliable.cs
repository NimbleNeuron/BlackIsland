#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Examples
{
	using Time;
	using UnityEngine;

	// speed-hack resistant analogue of the InfiniteRotator.cs
	[AddComponentMenu("")]
	internal class InfiniteRotatorReliable : MonoBehaviour
	{
		[Range(1f, 100f)]
		public float speed = 5f;

		private void Awake()
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				Destroy(gameObject);
			}
		}
		
		private void Update()
		{
			// just use SpeedHackProofTime.* instead of Time.* to reach speed hacks resistant time
			transform.Rotate(0, speed * SpeedHackProofTime.deltaTime, 0);
		}
	}
}