using System.Collections.Generic;
using Blis.Common;
using UnityEngine;


[ExecuteInEditMode]
[AddComponentMenu("Fx/FxScaler")]
public class FxScaler : MonoBehaviour
{
	
	private void Start()
	{
		Log.E("FxScaler can use in editor not standalone build : " + base.gameObject.name);
		UnityEngine.Object.Destroy(this);
	}

	
	private void Update()
	{
	}

	
	private void ScaleShurikenSystems(float scaleFactor)
	{
	}

	
	private void ScaleTrailRenderers(float scaleFactor)
	{
		base.GetComponentsInChildren<TrailRenderer>(this.trailRendererList);
		foreach (TrailRenderer trailRenderer in this.trailRendererList)
		{
			trailRenderer.startWidth *= scaleFactor;
			trailRenderer.endWidth *= scaleFactor;
		}
	}

	
	public float fxScale = 1f;

	
	public bool alsoScaleGameobject = true;

	
	private readonly List<ParticleSystem> particleSystemList = new List<ParticleSystem>();

	
	private readonly List<TrailRenderer> trailRendererList = new List<TrailRenderer>();

	
	private float prevScale;
}
