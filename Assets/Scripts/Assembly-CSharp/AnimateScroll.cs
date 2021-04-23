using Blis.Common;
using UnityEngine;


[AddComponentMenu("Fx/AnimateScroll")]
public class AnimateScroll : MonoBehaviour
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
		Log.E("[AnimateScroll] no render component");
	}

	
	private void LateUpdate()
	{
		if (!this.isInitSuccess)
		{
			return;
		}
		this.uvOffset += this.uvAnimationRate * Time.deltaTime;
		if (this.uvOffset.x >= 1f || this.uvOffset.x <= -1f)
		{
			this.uvOffset.x = 0f;
		}
		if (this.uvOffset.y >= 1f || this.uvOffset.y <= -1f)
		{
			this.uvOffset.y = 0f;
		}
		if (this.render.enabled)
		{
			this.targetMaterial.SetTextureOffset(this.textureName1, this.uvOffset);
			if (!string.IsNullOrEmpty(this.textureName2) && !this.textureName1.Equals(this.textureName2))
			{
				this.targetMaterial.SetTextureOffset(this.textureName2, this.uvOffset);
			}
		}
	}

	
	private void OnDestroy()
	{
		if (!this.isInitSuccess)
		{
			return;
		}
		Material[] array = this.renderMaterials;
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i]);
		}
	}

	
	public int materialIndex;

	
	public Vector2 uvAnimationRate = new Vector2(1f, 0f);

	
	public string textureName1 = "_MainTex";

	
	public string textureName2;

	
	private Vector2 uvOffset = Vector2.zero;

	
	private bool isInitSuccess;

	
	private Renderer render;

	
	private Material targetMaterial;

	
	private Material[] renderMaterials;
}
