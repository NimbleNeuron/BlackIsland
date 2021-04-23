using Blis.Common;
using UnityEngine;


[AddComponentMenu("Fx/AnimateScroll2")]
public class AnimateScroll2 : MonoBehaviour
{
	
	private void Start()
	{
		this.render = base.GetComponent<Renderer>();
		if (this.render != null)
		{
			this.renderMaterials = this.render.materials;
			this.targetMaterial = this.renderMaterials[this.materialIndex];
			this.isInitSuccess = true;
			return;
		}
		Log.E("[AnimateScroll2] no render component");
	}

	
	private void LateUpdate()
	{
		if (!this.isInitSuccess)
		{
			return;
		}
		this.uvOffset1 += this.uvAnimationRate1 * Time.deltaTime;
		if (this.uvOffset1.x >= 1f || this.uvOffset1.x <= -1f)
		{
			this.uvOffset1.x = 0f;
		}
		if (this.uvOffset1.y >= 1f || this.uvOffset1.y <= -1f)
		{
			this.uvOffset1.y = 0f;
		}
		this.uvOffset2 += this.uvAnimationRate2 * Time.deltaTime;
		if (this.uvOffset2.x >= 1f || this.uvOffset2.x <= -1f)
		{
			this.uvOffset2.x = 0f;
		}
		if (this.uvOffset2.y >= 1f || this.uvOffset2.y <= -1f)
		{
			this.uvOffset2.y = 0f;
		}
		if (this.render.enabled)
		{
			if (this.targetMaterial.HasProperty(this.textureName1))
			{
				this.targetMaterial.SetTextureOffset(this.textureName1, this.uvOffset1);
			}
			if (!string.IsNullOrEmpty(this.textureName2) && !this.textureName1.Equals(this.textureName2) && this.targetMaterial.HasProperty(this.textureName2))
			{
				this.targetMaterial.SetTextureOffset(this.textureName2, this.uvOffset2);
			}
		}
	}

	
	private void OnDestroy()
	{
		if (!this.isInitSuccess)
		{
			return;
		}
		UnityEngine.Object.Destroy(this.targetMaterial);
		Material[] array = this.renderMaterials;
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i]);
		}
	}

	
	public int materialIndex;

	
	public Vector2 uvAnimationRate1 = new Vector2(1f, 0f);

	
	public Vector2 uvAnimationRate2 = new Vector2(1f, 0f);

	
	public string textureName1 = "_MainTex";

	
	public string textureName2;

	
	private Vector2 uvOffset1 = Vector2.zero;

	
	private Vector2 uvOffset2 = Vector2.zero;

	
	private bool isInitSuccess;

	
	private Renderer render;

	
	private Material targetMaterial;

	
	private Material[] renderMaterials;
}
