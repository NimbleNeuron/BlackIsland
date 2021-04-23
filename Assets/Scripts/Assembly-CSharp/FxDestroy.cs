using UnityEngine;


public class FxDestroy : MonoBehaviour
{
	
	private void Start()
	{
		UnityEngine.Object.Destroy(base.gameObject, this.time);
	}

	
	public float time;
}
