using UnityEngine;

namespace Blis.Client
{
	public class MinimapIndicator : MonoBehaviour
	{
		private bool isActive;


		private RectTransform rectTransform;


		public bool IsActive => isActive;


		
		public float Range { get; set; }

		private void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
		}


		public void ShowIndicator()
		{
			isActive = true;
			gameObject.SetActive(true);
		}


		public void HideIndicator()
		{
			isActive = false;
			gameObject.SetActive(false);
		}


		public void SetPosition(Vector3 position)
		{
			transform.localPosition = position;
		}


		public void SetScale(float minimapSize)
		{
			rectTransform.sizeDelta = new Vector2(minimapSize, minimapSize);
		}
	}
}