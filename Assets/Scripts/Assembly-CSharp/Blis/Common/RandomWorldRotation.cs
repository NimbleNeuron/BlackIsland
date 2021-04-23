using UnityEngine;

namespace Blis.Common
{
	
	public class RandomWorldRotation : MonoBehaviour
	{
		
		private void Start()
		{
			base.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(this.xRange.x, this.xRange.y), UnityEngine.Random.Range(this.yRange.x, this.yRange.y), UnityEngine.Random.Range(this.zRange.x, this.zRange.y)));
		}

		
		[SerializeField]
		private Vector2 xRange = new Vector2(0f, 360f);

		
		[SerializeField]
		private Vector2 yRange = new Vector2(0f, 360f);

		
		[SerializeField]
		private Vector2 zRange = new Vector2(0f, 360f);
	}
}
