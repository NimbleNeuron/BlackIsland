#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Examples
{
	using UnityEngine;

	// dummy code, just to add some rotation to the cube from example scene
	[AddComponentMenu("")]
	internal class InfiniteRotator : MonoBehaviour
	{
		[Range(1f, 100f)]
		public float speed = 5f;

		private void Update()
		{
			transform.Rotate(0, speed * Time.deltaTime, 0);
		}
	}
}