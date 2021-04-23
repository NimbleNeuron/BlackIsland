//------------------------------------------------------------------------------------------------------------------
// Volumetric Fog & Mist
// Created by Ramiro Oliva (Kronnect)
//------------------------------------------------------------------------------------------------------------------

namespace VolumetricFogAndMist
{
	internal interface IVolumetricFogRenderComponent
	{
		VolumetricFog fog { get; set; }

		void DestroySelf();
	}
}