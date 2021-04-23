using UnityEngine;

namespace Blis.Client
{
	public class ShaderDeltatime : MonoBehaviour
	{
		private const string Dtime = "_Dtime";


		public GameObject targetObject;


		public float speed;


		public float startValue;


		public float maxValue = 1f;


		private Material targetMaterial;


		private Renderer targetRenderer;


		private float value;

		private void Awake()
		{
			targetRenderer = targetObject.GetComponent<Renderer>();
			targetMaterial = targetRenderer.material;
			Init();
		}


		private void LateUpdate()
		{
			if (targetRenderer.enabled)
			{
				value += Time.deltaTime * speed;
				if (maxValue <= value)
				{
					value = maxValue;
				}

				targetMaterial.SetFloat("_Dtime", value);
			}
		}


		private void OnEnable()
		{
			Init();
		}


		private void Init()
		{
			targetMaterial.SetFloat("_Dtime", startValue);
			value = startValue;
		}
	}
}