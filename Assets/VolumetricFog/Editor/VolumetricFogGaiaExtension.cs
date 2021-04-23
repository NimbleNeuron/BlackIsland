#if GAIA_PRESENT && UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using VolumetricFogAndMist;
using UnityEditor;

namespace Gaia.GX.Kronnect
{
    /// <summary>
    /// Volumetric Fog & Mist extension for Gaia.
    /// </summary>
	public class VolumetricFogGaiaExtension : MonoBehaviour
    {
#region Generic informational methods

        /// <summary>
        /// Returns the publisher name if provided. 
        /// This will override the publisher name in the namespace ie Gaia.GX.PublisherName
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "Kronnect";
        }

        /// <summary>
        /// Returns the package name if provided
        /// This will override the package name in the class name ie public class PackageName.
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "Volumetric Fog & Mist";
        }

#endregion

#region Methods exposed by Gaia as buttons must be prefixed with GX_

        public static void GX_About()
        {
			EditorUtility.DisplayDialog("About Volumetric Fog & Mist", "Volumetric Fog & Mist is a full-screen image effect that adds realistic, live, moving fog, mist, dust, clouds and sky haze to your scenes making them less dull and boring.", "OK");
        }

		static VolumetricFog CheckFog() {
			VolumetricFog fog = VolumetricFog.instance;
			// Checks Volumetric Fog image effect is attached
			if (fog==null) {
				Camera camera = Camera.main;
				if (camera == null)
				{
					camera = FindObjectOfType<Camera>();
				}
				if (camera == null)
				{
					Debug.LogError("Could not find camera to add camera effects to. Please add a camera to your scene.");
					return null;
				}
				fog = camera.gameObject.AddComponent<VolumetricFog>();
			}
			// Finds a suitable Sun light
			if (fog.sun==null) {
				Light[] lights = FindObjectsOfType<Light>();
				for (int k=0;k<lights.Length;k++) {
					Light light = lights[k];
					if (light.name.Equals("Sun")) {
						fog.sun = light.gameObject;
						break;
					}
				}
				if (fog.sun==null) {
					for (int k=0;k<lights.Length;k++) {
						Light light = lights[k];
						if (light.type == LightType.Directional) {
							fog.sun = light.gameObject;
							break;
						}
					}
				}
			}
			return fog;
		}

		public static void GX_WorldEdge() {
			VolumetricFog fog = CheckFog();
			if (fog==null) return;
			fog.preset = FOG_PRESET.WorldEdge;
			fog.preset = FOG_PRESET.Custom; // enable to make modifications and preserve them
		}

		public static void GX_WindyMist() {
			VolumetricFog fog = CheckFog();
			if (fog==null) return;
			fog.preset = FOG_PRESET.WindyMist;
			fog.preset = FOG_PRESET.Custom; // enable to make modifications and preserve them
		}

		public static void GX_SeaOfClouds() {
			VolumetricFog fog = CheckFog();
			if (fog==null) return;
			fog.preset = FOG_PRESET.SeaClouds;
			fog.preset = FOG_PRESET.Custom; // enable to make modifications and preserve them
		}

		public static void GX_Fog() {
			VolumetricFog fog = CheckFog();
			if (fog==null) return;
			fog.preset = FOG_PRESET.Fog;
			fog.preset = FOG_PRESET.Custom; // enable to make modifications and preserve them
		}

		public static void GX_HeavyFog() {
			VolumetricFog fog = CheckFog();
			if (fog==null) return;
			fog.preset = FOG_PRESET.HeavyFog;
			fog.preset = FOG_PRESET.Custom; // enable to make modifications and preserve them
		}
		
		public static void GX_FrostedWater() {
			VolumetricFog fog = CheckFog();
			if (fog==null) return;
			fog.preset = FOG_PRESET.FrostedGround;
			fog.preset = FOG_PRESET.Custom; // enable to make modifications and preserve them
			fog.color = Color.white;
			fog.height = 1.5f;
		}

		public static void GX_GroundFog() {
			VolumetricFog fog = CheckFog();
			if (fog==null) return;
			fog.preset = FOG_PRESET.GroundFog;
			fog.preset = FOG_PRESET.Custom; // enable to make modifications and preserve them
		}

		public static void GX_ToxicSwamps() {
			VolumetricFog fog = CheckFog();
			if (fog==null) return;
			fog.preset = FOG_PRESET.ToxicSwamp;
			fog.preset = FOG_PRESET.Custom; // enable to make modifications and preserve them
		}

		public static void GX_FoggyLakes() {
			VolumetricFog fog = CheckFog();
			if (fog==null) return;
			fog.preset = FOG_PRESET.FoggyLake;
			fog.preset = FOG_PRESET.Custom; // enable to make modifications and preserve them
		}
		
		public static void GX_SandStorm1() {
			VolumetricFog fog = CheckFog();
			if (fog==null) return;
			fog.preset = FOG_PRESET.SandStorm1;
			fog.preset = FOG_PRESET.Custom; // enable to make modifications and preserve them
		}

		public static void GX_SandStorm2() {
			VolumetricFog fog = CheckFog();
			if (fog==null) return;
			fog.preset = FOG_PRESET.SandStorm2;
			fog.preset = FOG_PRESET.Custom; // enable to make modifications and preserve them
		}


#endregion
     
    }
}

#endif