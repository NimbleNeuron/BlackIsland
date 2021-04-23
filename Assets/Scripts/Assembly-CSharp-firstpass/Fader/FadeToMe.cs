using UnityEngine;

public class FadeToMe : MonoBehaviour
{
	private void Start()
	{
		FadeObstructionsManager.Instance.RegisterShouldBeVisible(gameObject);
	}
}