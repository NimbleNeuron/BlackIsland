using Blis.Common;
using UnityEngine;


public class ShaderOffset : MonoBehaviour
{
	
	
	public float SetOffset
	{
		set
		{
			this.offset = value;
			this.ApplyOffset();
		}
	}

	
	private void Awake()
	{
		Renderer component = base.GetComponent<Renderer>();
		if (component != null)
		{
			this.mat = component.material;
			if (this.mat != null)
			{
				this.isInitSuccess = true;
			}
		}
		else
		{
			Log.E("[ShaderOffset] no render component");
		}
	}

	
	private void OnEnable()
	{
		this.ApplyOffset();
	}

	
	private void ApplyOffset()
	{
		if (!this.isInitSuccess)
		{
			return;
		}
		this.mat.SetFloat("_Offset", this.offset);
	}

	
	private void OnDestroy()
	{
		if (!this.isInitSuccess)
		{
			return;
		}
		UnityEngine.Object.Destroy(this.mat);
	}

	
	[SerializeField]
	private float offset = -3000000f;

	
	private Material mat;

	
	private bool isInitSuccess;
}
