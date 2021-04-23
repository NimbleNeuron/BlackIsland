using Blis.Client;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;


public class DayNightParticleRendererMaterialSwitcher : MonoBehaviour
{
	
	private void Start()
	{
		MonoBehaviourInstance<ClientService>.inst.OnUpdateRestrictedArea += this.OnUpdateRestrictedArea;
		this.particleSystemRenderer = base.GetComponent<ParticleSystemRenderer>();
		this.SetMaterialAsDayNight(MonoBehaviourInstance<ClientService>.inst.DayNight);
	}

	
	private void OnDestroy()
	{
		ClientService inst = MonoBehaviourInstance<ClientService>.inst;
		if (inst != null)
		{
			inst.OnUpdateRestrictedArea -= this.OnUpdateRestrictedArea;
		}
	}

	
	private void OnUpdateRestrictedArea(DayNight dayNight)
	{
		this.SetMaterialAsDayNight(dayNight);
	}

	
	private void SetMaterialAsDayNight(DayNight dayNight)
	{
		if (this.particleSystemRenderer == null)
		{
			return;
		}
		if (dayNight == DayNight.Day)
		{
			this.particleSystemRenderer.material = this.dayMaterial;
			return;
		}
		this.particleSystemRenderer.material = this.nightMaterial;
	}

	
	[SerializeField]
	private Material dayMaterial = default;

	
	[SerializeField]
	private Material nightMaterial = default;

	
	private ParticleSystemRenderer particleSystemRenderer;
}
