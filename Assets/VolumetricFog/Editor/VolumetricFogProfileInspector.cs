using UnityEngine;
using UnityEditor;

namespace VolumetricFogAndMist {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(VolumetricFogProfile))]
	public class VolumetricFogProfileInspector : Editor {

		Color titleColor;

		SerializedProperty lightingModel, sunCopyColor, density, noiseStrength, height, heightFallOff, baselineHeight, distance, maxFogLength, maxFogLengthFallOff, distanceFallOff;
		SerializedProperty baselineRelativeToCamera, baselineRelativeToCameraDelay;
		SerializedProperty noiseScale, noiseSparse, noiseFinalMultiplier, alpha, color, deepObscurance, specularColor, specularThreshold, specularIntensity, lightDirection, lightIntensity, lightColor;
		SerializedProperty speed, windDirection, useRealTime;
		SerializedProperty turbulenceStrength, useXYPlane;
		SerializedProperty skyColor, skyHaze, skySpeed, skyNoiseStrength, skyNoiseScale, skyAlpha, skyDepth;
		SerializedProperty downsamplingOverride, downsampling, forceComposition, edgeImprove, edgeThreshold, stepping, steppingNear, dithering, ditherStrength;
		SerializedProperty lightScatteringOverride, lightScatteringEnabled, lightScatteringDiffusion, lightScatteringExposure, lightScatteringSpread, lightScatteringWeight, lightScatteringIllumination, lightScatteringDecay, lightScatteringSamples, lightScatteringJittering, lightScatteringBlurDownscale;
		SerializedProperty fogVoidOverride, fogVoidPosition, fogVoidTopology, fogVoidRadius, fogVoidFallOff, fogVoidHeight, fogVoidDepth;

		void OnEnable() {
			titleColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.12f, 0.16f, 0.4f);

			lightingModel = serializedObject.FindProperty("lightingModel");
			sunCopyColor = serializedObject.FindProperty("sunCopyColor");
			density = serializedObject.FindProperty("density");
			noiseStrength = serializedObject.FindProperty("noiseStrength");
			height = serializedObject.FindProperty("height");
            heightFallOff = serializedObject.FindProperty("heightFallOff");
            baselineHeight = serializedObject.FindProperty("baselineHeight");
			distance = serializedObject.FindProperty("distance");
			maxFogLength = serializedObject.FindProperty("maxFogLength");
			maxFogLengthFallOff = serializedObject.FindProperty("maxFogLengthFallOff");
			distanceFallOff = serializedObject.FindProperty("distanceFallOff");
			baselineRelativeToCamera = serializedObject.FindProperty("baselineRelativeToCamera");
			baselineRelativeToCameraDelay = serializedObject.FindProperty("baselineRelativeToCameraDelay");
			noiseScale = serializedObject.FindProperty("noiseScale");
			noiseSparse = serializedObject.FindProperty("noiseSparse");
			noiseFinalMultiplier = serializedObject.FindProperty("noiseFinalMultiplier");
			alpha = serializedObject.FindProperty("alpha");
			color = serializedObject.FindProperty("color");
            deepObscurance = serializedObject.FindProperty("deepObscurance");
            specularColor = serializedObject.FindProperty("specularColor");
			specularThreshold = serializedObject.FindProperty("specularThreshold");
			specularIntensity = serializedObject.FindProperty("specularIntensity");
			lightDirection = serializedObject.FindProperty("lightDirection");
			lightIntensity = serializedObject.FindProperty("lightIntensity");
			lightColor = serializedObject.FindProperty("lightColor");
			speed = serializedObject.FindProperty("speed");
			windDirection = serializedObject.FindProperty("windDirection");
			useRealTime = serializedObject.FindProperty("useRealTime");
			turbulenceStrength = serializedObject.FindProperty("turbulenceStrength");
			useXYPlane = serializedObject.FindProperty("useXYPlane");

			skyColor = serializedObject.FindProperty("skyColor");
			skyHaze = serializedObject.FindProperty("skyHaze");
			skySpeed = serializedObject.FindProperty("skySpeed");
			skyNoiseStrength = serializedObject.FindProperty("skyNoiseStrength");
            skyNoiseScale = serializedObject.FindProperty("skyNoiseScale");
            skyAlpha = serializedObject.FindProperty("skyAlpha");
            skyDepth = serializedObject.FindProperty("skyDepth");

            downsamplingOverride = serializedObject.FindProperty("downsamplingOverride");
			downsampling = serializedObject.FindProperty("downsampling");
            forceComposition = serializedObject.FindProperty("forceComposition");
            edgeImprove = serializedObject.FindProperty("edgeImprove");
			edgeThreshold = serializedObject.FindProperty("edgeThreshold");
			stepping = serializedObject.FindProperty("stepping");
			steppingNear = serializedObject.FindProperty("steppingNear");
			dithering = serializedObject.FindProperty("dithering");
			ditherStrength = serializedObject.FindProperty("ditherStrength");

			lightScatteringOverride = serializedObject.FindProperty("lightScatteringOverride");
			lightScatteringEnabled = serializedObject.FindProperty("lightScatteringEnabled");
			lightScatteringDiffusion = serializedObject.FindProperty("lightScatteringDiffusion");
			lightScatteringExposure = serializedObject.FindProperty("lightScatteringExposure");
			lightScatteringSpread = serializedObject.FindProperty("lightScatteringSpread");
			lightScatteringWeight = serializedObject.FindProperty("lightScatteringWeight");
			lightScatteringIllumination = serializedObject.FindProperty("lightScatteringIllumination");
			lightScatteringDecay = serializedObject.FindProperty("lightScatteringDecay");
			lightScatteringSamples = serializedObject.FindProperty("lightScatteringSamples");
			lightScatteringJittering = serializedObject.FindProperty("lightScatteringJittering");
            lightScatteringBlurDownscale = serializedObject.FindProperty("lightScatteringBlurDownscale");

            fogVoidOverride = serializedObject.FindProperty("fogVoidOverride");
			fogVoidPosition = serializedObject.FindProperty("fogVoidPosition");
			fogVoidTopology = serializedObject.FindProperty("fogVoidTopology");
			fogVoidRadius = serializedObject.FindProperty("fogVoidRadius");
			fogVoidFallOff = serializedObject.FindProperty("fogVoidFallOff");
			fogVoidHeight = serializedObject.FindProperty("fogVoidHeight");
			fogVoidDepth = serializedObject.FindProperty("fogVoidDepth");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			EditorGUILayout.Separator();
			EditorGUILayout.BeginVertical();
			
			EditorGUILayout.BeginHorizontal();
			DrawTitleLabel("Volumetric Fog Profile");
			if (GUILayout.Button("Help", GUILayout.Width(50)))
				EditorUtility.DisplayDialog("Help", "Move the mouse over each label to show a description of the parameter.", "Ok");
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Separator();
			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();

			EditorGUILayout.BeginVertical();
			DrawTitleLabel("Geometry");
			EditorGUILayout.PropertyField(distance, new GUIContent("Distance", "Distance in meters from the camera at which the fog starts. It works with Distance FallOff."));
			EditorGUILayout.PropertyField(distanceFallOff, new GUIContent("   Fall Off", "When you set a value to Distance > 0, this parameter defines the gradient of the fog to the camera. The higher the value, the shorter the gradient."));
			EditorGUILayout.PropertyField(maxFogLength, new GUIContent("Max. Distance", "Maximum distance from camera at which the fog is rendered. Decrease this value to improve performance."));
			EditorGUILayout.PropertyField(maxFogLengthFallOff, new GUIContent("   FallOff", "Blends far range with background."));
			EditorGUILayout.PropertyField(height, new GUIContent("Height", "Maximum height of the fog in meters."));
            EditorGUILayout.PropertyField(heightFallOff, new GUIContent("    FallOff", "Height density or falloff (default = 0.6)."));
            EditorGUILayout.PropertyField(useXYPlane, new GUIContent("Use XY Plane", "Switches between normal mode to XY mode."));
			EditorGUILayout.BeginHorizontal();
			if (useXYPlane.boolValue) {
				EditorGUILayout.PropertyField(baselineHeight, new GUIContent("Base Z", "Starting Z of the fog in meters."));
			} else {
				EditorGUILayout.PropertyField(baselineHeight, new GUIContent("Base Height", "Starting height of the fog in meters. You can set this value above Camera position. Try it!"));
			}
			EditorGUILayout.EndHorizontal();

			if (useXYPlane.boolValue) {
				GUI.enabled = false;
			}
			EditorGUILayout.PropertyField(baselineRelativeToCamera, new GUIContent("Relative To Camera", "If set to true, the base height will be added to the camera height. This is useful for cloud styles so they always stay over your head!"));
			if (baselineRelativeToCamera.boolValue) {
				EditorGUILayout.PropertyField(baselineRelativeToCameraDelay, new GUIContent("Delay", "Speed factor for transitioning to new camera heights."));
			}
			GUI.enabled = true;

			EditorGUILayout.Separator();

			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();

			EditorGUILayout.BeginVertical();
			DrawTitleLabel("Appearance");
			EditorGUILayout.PropertyField(density, new GUIContent("Density", "General density of the fog. Higher density fog means darker fog as well."));
			EditorGUILayout.PropertyField(noiseStrength, new GUIContent("Noise Strength", "Randomness of the fog formation. 0 means uniform fog whereas a value towards 1 will make areas of different densities and heights."));
			EditorGUILayout.PropertyField(noiseScale, new GUIContent("   Scale", "Increasing this value will expand the size of the noise."));
			EditorGUILayout.PropertyField(noiseSparse, new GUIContent("   Sparse", "Increase to make noise sparser."));
			EditorGUILayout.PropertyField(noiseFinalMultiplier, new GUIContent("   Final Multiplier", "Final noise value multiplier."));
			EditorGUILayout.PropertyField(alpha, new GUIContent("Alpha", "Transparency for the fog. You may want to reduce this value if you experiment issues with billboards."));
			EditorGUILayout.PropertyField(color, new GUIContent("Albedo", "Base color of the fog."));
            EditorGUILayout.PropertyField(deepObscurance, new GUIContent("Deep Obscurance", "Makes fog darker at the bottom."));
            EditorGUILayout.PropertyField(specularColor, new GUIContent("Specular Color", "This is the color reflected by the fog under direct light exposure (see Light parameters)"));
			EditorGUILayout.PropertyField(specularThreshold, new GUIContent("Specular Threshold", "Area of the fog subject to light reflectancy"));
			EditorGUILayout.PropertyField(specularIntensity, new GUIContent("Specular Intensity", "The intensity of the reflected light."));
			EditorGUILayout.PropertyField(lightDirection, new GUIContent("Light Direction", "The normalized direction of a simulated directional light."));
			EditorGUILayout.PropertyField(lightingModel, new GUIContent("Lighting Model", "The lighting model used to calculate fog color. 'Legacy' is provided for previous version compatibility. 'Natural' uses ambient + light color. 'Single Light' excludes ambient color."));
			EditorGUILayout.PropertyField(lightIntensity, new GUIContent("Light Intensity", "Intensity of the simulated directional light."));
			EditorGUILayout.PropertyField(lightColor, new GUIContent("Light Color", "Color of the simulated direcional light."));
			EditorGUILayout.PropertyField(sunCopyColor, new GUIContent("Copy Sun Color", "Always use Sun light color. Disable this property to allow choosing a custom light color."));

			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();

			EditorGUILayout.BeginVertical();
			DrawTitleLabel("Animation");
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(speed, new GUIContent("Wind Speed", "Speed factor for the simulated wind effect over the fog."));
			if (GUILayout.Button("Stop", GUILayout.Width(60))) {
				speed.floatValue = 0;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(windDirection, new GUIContent("Wind Direction", "Normalized direcional vector for the wind effect."));
			EditorGUILayout.PropertyField(turbulenceStrength, new GUIContent("Turbulence", "Turbulence strength. Set to zero to deactivate. Turbulence adds a render pass to compute fog animation."));
			EditorGUILayout.PropertyField(useRealTime, new GUIContent("Use Real Time", "Uses real elapsed time since last fog rendering instead of Time.deltaTime (elapsed time since last frame) to ensure continuous fog animation irrespective of camera enable state. For example, if you disable the camera and 'Use Real Time' is enabled, when you enable the camera again, the fog animation wil compute the total elapsed time so it shows as if fog continues animating while camera was disabled.  If set to false, fog animation will use Time.deltaTime (elapsed time since last frame) which will cause fog to resume previous state."));
			EditorGUILayout.EndVertical();

			EditorGUILayout.Separator();

			EditorGUILayout.BeginVertical();
			DrawTitleLabel("Sky");
			if (useXYPlane.boolValue) {
				EditorGUILayout.HelpBox("Sky haze is disabled when using XY plane.", MessageType.Info);
			}
			EditorGUILayout.PropertyField(skyHaze, new GUIContent("Haze", "Height of the sky haze in meters. Reduce this or alpha to 0 to disable sky haze."));
			EditorGUILayout.PropertyField(skyColor, new GUIContent("Color", "Sky haze color."));
            EditorGUILayout.PropertyField(skyAlpha, new GUIContent("Alpha", "Transparency of the sky haze. Reduce this or Haze value to 0 to disable sky haze."));
            EditorGUILayout.PropertyField(skySpeed, new GUIContent("Speed", "Speed of the haze animation."));
			EditorGUILayout.PropertyField(skyNoiseStrength, new GUIContent("Noise Strength", "Amount of noise for the sky haze."));
            EditorGUILayout.PropertyField(skyNoiseScale, new GUIContent("Noise Scale", "Scale of the projected noise."));
            EditorGUILayout.PropertyField(skyDepth, new GUIContent("Distance", "Distance for the sky haze effect.")); 
            EditorGUILayout.EndVertical();

			EditorGUILayout.Separator();

			EditorGUILayout.BeginVertical();
			DrawTitleLabel("Light Scattering");
			EditorGUILayout.PropertyField(lightScatteringOverride, new GUIContent("Override", "Override current light scattering fog settings."));
			if (!lightScatteringOverride.boolValue) GUI.enabled = false;
			EditorGUILayout.PropertyField(lightScatteringEnabled, new GUIContent("Enable", "Enables screen space light scattering. Simulates scattering of Sun light through atmosphere."));
			if (lightScatteringEnabled.boolValue) {
				EditorGUILayout.PropertyField(lightScatteringDiffusion, new GUIContent("Diffusion", "Spread or intensity of Sun light scattering."));
				EditorGUILayout.PropertyField(lightScatteringExposure, new GUIContent("Shafts Intensity", "Intensity for the Sun shafts effect."));
				EditorGUILayout.PropertyField(lightScatteringSpread, new GUIContent("   Spread", "Length of the Sun rays. "));
				EditorGUILayout.PropertyField(lightScatteringWeight, new GUIContent("   Sample Weight", "Strength of Sun rays."));
				EditorGUILayout.PropertyField(lightScatteringIllumination, new GUIContent("   Start Illumination", "Initial strength of each ray."));
				EditorGUILayout.PropertyField(lightScatteringDecay, new GUIContent("   Decay", "Decay multiplier applied on each step."));
				EditorGUILayout.PropertyField(lightScatteringSamples, new GUIContent("   Samples", "Number of light samples used when Light Scattering is enabled. Reduce to increse performance."));
				EditorGUILayout.PropertyField(lightScatteringJittering, new GUIContent("   Jittering", "Smooths rays removing artifacts and allowing to use less samples."));
                EditorGUILayout.PropertyField(lightScatteringBlurDownscale, new GUIContent("   Blur Downscale"));
            }
			GUI.enabled = true;
			EditorGUILayout.EndVertical();

			EditorGUILayout.Separator();

			EditorGUILayout.BeginVertical();
			DrawTitleLabel("Fog Void");
			EditorGUILayout.PropertyField(fogVoidOverride, new GUIContent("Override", "Override current fog void settings."));
			if (!fogVoidOverride.boolValue) GUI.enabled = false;
			EditorGUILayout.PropertyField(fogVoidPosition, new GUIContent("Center", "Location of the center of the fog void in world space (area where the fog disappear).\nThis option is very useful if you want a clear area around your character in 3rd person view."));
			EditorGUILayout.PropertyField(fogVoidTopology, new GUIContent("Topology", "Shape of the void area."));
			if (fogVoidTopology.intValue == (int)FOG_VOID_TOPOLOGY.Sphere) {
				EditorGUILayout.PropertyField(fogVoidRadius, new GUIContent("Radius", "Radius of the void area."));
			} else if (fogVoidTopology.intValue == (int)FOG_VOID_TOPOLOGY.Box) {
				EditorGUILayout.PropertyField(fogVoidRadius, new GUIContent("Width", "Width of the void area."));
				EditorGUILayout.PropertyField(fogVoidHeight, new GUIContent("Height", "Height of the void area."));
				EditorGUILayout.PropertyField(fogVoidDepth, new GUIContent("Depth", "Depth of the void area."));
			}
			EditorGUILayout.PropertyField(fogVoidFallOff, new GUIContent("FallOff", "Gradient of the void area effect."));
			GUI.enabled = true;
			EditorGUILayout.EndVertical();

			EditorGUILayout.Separator();

			EditorGUILayout.BeginVertical();
			DrawTitleLabel("Rendering");
			EditorGUILayout.PropertyField(downsamplingOverride, new GUIContent("Downsampling Override", "Overrides downsampling settings."));
			if (!downsamplingOverride.boolValue) GUI.enabled = false;
			EditorGUILayout.PropertyField(downsampling, new GUIContent("   Downsampling", "Reduces the size of the depth texture to improve performance."));
            if (downsampling.intValue == 1) {
                EditorGUILayout.PropertyField(forceComposition, new GUIContent("   Force Composition", "Enables final composition with optional edge filter at no downscale to improve edges when MSAA is enabled."));
            }

			if (downsampling.intValue > 1 || forceComposition.boolValue) {
                EditorGUILayout.PropertyField(edgeImprove, new GUIContent("   Improve Edges", "Check this option to reduce artifacts and halos around geometry edges when downsampling is applied. This is an option because it's faster to not take care or geometry edges, which is probably unnecesary if you use fog as elevated clouds."));
				if (edgeImprove.boolValue) {
					EditorGUILayout.PropertyField(edgeThreshold, new GUIContent("      Threshold", "Depth threshold used to detected edges."));
				}
			}
			GUI.enabled = true;

			EditorGUILayout.PropertyField(stepping, new GUIContent("Stepping", "Multiplier to the ray-marching algorithm. Values between 8-12 are good. Increasing the stepping will produce more accurate and better quality fog but performance will be reduced. The less the density of the fog the lower you can set this value."));
			EditorGUILayout.PropertyField(steppingNear, new GUIContent("Stepping Near", "Works with Stepping parameter but applies only to short distances from camera. Lowering this value can help to reduce banding effect (performance can be reduced as well)."));
			EditorGUILayout.PropertyField(dithering, new GUIContent("Dithering", "Blends final fog color with a pattern to reduce banding artifacts. Use the slider to choose the intensity of dither."));
			if (dithering.boolValue) {
				EditorGUILayout.PropertyField(ditherStrength, new GUIContent("   Strength", "Dither strength."));
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();

			if (serializedObject.ApplyModifiedProperties() || (Event.current.type == EventType.ExecuteCommand &&
			    Event.current.commandName == "UndoRedoPerformed")) {

				// Triggers profile reload on all Volumetric Fog scripts
				VolumetricFog[] fogs = FindObjectsOfType<VolumetricFog>();
				for (int t = 0; t < targets.Length; t++) {
					VolumetricFogProfile profile = (VolumetricFogProfile)targets[t];
					for (int k = 0; k < fogs.Length; k++) {
						if (fogs[k] != null && fogs[k].profile == profile && fogs[k].profileSync) {
							profile.Load(fogs[k]);
						}
					}
				}
				EditorUtility.SetDirty(target);
			}
		}

		Texture2D MakeTex(int width, int height, Color col) {
			Color[] pix = new Color[width * height];
			
			for (int i = 0; i < pix.Length; i++)
				pix[i] = col;
			
			TextureFormat tf = SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat) ? TextureFormat.RGBAFloat : TextureFormat.RGBA32;
			Texture2D result = new Texture2D(width, height, tf, false);
			result.SetPixels(pix);
			result.Apply();
			
			return result;
		}

		GUIStyle titleLabelStyle;

		void DrawTitleLabel(string s) {
			if (titleLabelStyle == null) {
				titleLabelStyle = new GUIStyle(GUI.skin.label);
			}
			titleLabelStyle.normal.textColor = titleColor;
			titleLabelStyle.fontStyle = FontStyle.Bold;
			GUILayout.Label(s, titleLabelStyle);
		}
		

	

	}

}
