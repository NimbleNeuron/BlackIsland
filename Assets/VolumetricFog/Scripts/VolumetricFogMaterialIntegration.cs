using UnityEngine;

namespace VolumetricFogAndMist
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(VolumetricFog))]
	public class VolumetricFogMaterialIntegration : MonoBehaviour
	{
		private static readonly Properties[] props =
		{
			new Properties {name = "_NoiseTex", type = PropertyType.Texture2D},
			new Properties {name = "_FogAlpha", type = PropertyType.Float},
			new Properties {name = "_Color", type = PropertyType.Color},
			new Properties {name = "_FogDistance", type = PropertyType.Float4},
			new Properties {name = "_FogData", type = PropertyType.Float4},
			new Properties {name = "_FogWindDir", type = PropertyType.Float3},
			new Properties {name = "_FogStepping", type = PropertyType.Float4},
			new Properties {name = "_BlurTex", type = PropertyType.Texture2D},
			new Properties {name = "_FogVoidPosition", type = PropertyType.Float3},
			new Properties {name = "_FogVoidData", type = PropertyType.Float4},
			new Properties {name = "_FogAreaPosition", type = PropertyType.Float3},
			new Properties {name = "_FogAreaData", type = PropertyType.Float4},
			new Properties {name = "_FogOfWar", type = PropertyType.Texture2D},
			new Properties {name = "_FogOfWarCenter", type = PropertyType.Float3},
			new Properties {name = "_FogOfWarSize", type = PropertyType.Float3},
			new Properties {name = "_FogOfWarCenterAdjusted", type = PropertyType.Float3},
			new Properties {name = "_FogPointLightPosition", type = PropertyType.Float4Array},
			new Properties {name = "_FogPointLightColor", type = PropertyType.ColorArray},
			new Properties {name = "_SunPosition", type = PropertyType.Float3},
			new Properties {name = "_SunDir", type = PropertyType.Float3},
			new Properties {name = "_SunColor", type = PropertyType.Float3},
			new Properties {name = "_FogScatteringData", type = PropertyType.Float4},
			new Properties {name = "_FogScatteringData2", type = PropertyType.Float4},
			new Properties {name = "_VolumetricFogSunDepthTexture", type = PropertyType.Texture2D},
			new Properties {name = "_VolumetricFogSunDepthTexture_TexelSize", type = PropertyType.Float4},
			new Properties {name = "_VolumetricFogSunProj", type = PropertyType.Matrix4x4},
			new Properties {name = "_VolumetricFogSunWorldPos", type = PropertyType.Float4},
			new Properties {name = "_VolumetricFogSunShadowsData", type = PropertyType.Float4},
			new Properties {name = "_Jitter", type = PropertyType.Float}
		};

		private static readonly string[] keywords =
		{
			"FOG_DISTANCE_ON", "FOG_AREA_SPHERE", "FOG_AREA_BOX", "FOG_VOID_SPHERE", "FOG_VOID_BOX", "FOG_OF_WAR_ON",
			"FOG_SCATTERING_ON", "FOG_BLUR_ON", "FOG_POINT_LIGHTS", "FOG_SUN_SHADOWS_ON"
		};

		public VolumetricFog fog;
		public Renderer[] materials;

		private void OnEnable()
		{
			fog = GetComponent<VolumetricFog>();
		}


		private void OnPreRender()
		{
			if (fog == null)
			{
				return;
			}

			Material fogMat = fog.fogMat;
			if (fogMat == null || materials == null || materials.Length == 0)
			{
				return;
			}

			// sync uniforms
			for (int k = 0; k < props.Length; k++)
			{
				if (!fogMat.HasProperty(props[k].name))
				{
					continue;
				}

				switch (props[k].type)
				{
					case PropertyType.Color:
						Color color = fogMat.GetColor(props[k].name);
						for (int m = 0; m < materials.Length; m++)
						{
							if (materials[m] != null && materials[m].sharedMaterial != null)
							{
								materials[m].sharedMaterial.SetColor(props[k].name, color);
							}
						}

						break;
					case PropertyType.ColorArray:
						Color[] colors = fogMat.GetColorArray(props[k].name);
						if (colors != null)
						{
							for (int m = 0; m < materials.Length; m++)
							{
								if (materials[m] != null && materials[m].sharedMaterial != null)
								{
									materials[m].sharedMaterial.SetColorArray(props[k].name, colors);
								}
							}
						}

						break;
					case PropertyType.FloatArray:
						float[] floats = fogMat.GetFloatArray(props[k].name);
						if (floats != null)
						{
							for (int m = 0; m < materials.Length; m++)
							{
								if (materials[m] != null && materials[m].sharedMaterial != null)
								{
									materials[m].sharedMaterial.SetFloatArray(props[k].name, floats);
								}
							}
						}

						break;
					case PropertyType.Float4Array:
						Vector4[] vectors = fogMat.GetVectorArray(props[k].name);
						if (vectors != null)
						{
							for (int m = 0; m < materials.Length; m++)
							{
								if (materials[m] != null && materials[m].sharedMaterial != null)
								{
									materials[m].sharedMaterial.SetVectorArray(props[k].name, vectors);
								}
							}
						}

						break;
					case PropertyType.Float:
						for (int m = 0; m < materials.Length; m++)
						{
							if (materials[m] != null && materials[m].sharedMaterial != null)
							{
								materials[m].sharedMaterial.SetFloat(props[k].name, fogMat.GetFloat(props[k].name));
							}
						}

						break;
					case PropertyType.Float3:
					case PropertyType.Float4:
						for (int m = 0; m < materials.Length; m++)
						{
							if (materials[m] != null && materials[m].sharedMaterial != null)
							{
								materials[m].sharedMaterial.SetVector(props[k].name, fogMat.GetVector(props[k].name));
							}
						}

						break;
					case PropertyType.Matrix4x4:
						for (int m = 0; m < materials.Length; m++)
						{
							if (materials[m] != null && materials[m].sharedMaterial != null)
							{
								materials[m].sharedMaterial.SetMatrix(props[k].name, fogMat.GetMatrix(props[k].name));
							}
						}

						break;
					case PropertyType.Texture2D:
						for (int m = 0; m < materials.Length; m++)
						{
							if (materials[m] != null && materials[m].sharedMaterial != null)
							{
								materials[m].sharedMaterial.SetTexture(props[k].name, fogMat.GetTexture(props[k].name));
							}
						}

						break;
				}
			}

			// sync shader keywords
			for (int k = 0; k < keywords.Length; k++)
			{
				if (fogMat.IsKeywordEnabled(keywords[k]))
				{
					for (int m = 0; m < materials.Length; m++)
					{
						if (materials[m] != null && materials[m].sharedMaterial != null)
						{
							materials[m].sharedMaterial.EnableKeyword(keywords[k]);
						}
					}
				}
				else
				{
					for (int m = 0; m < materials.Length; m++)
					{
						if (materials[m] != null && materials[m].sharedMaterial != null)
						{
							materials[m].sharedMaterial.DisableKeyword(keywords[k]);
						}
					}
				}
			}
		}

		private enum PropertyType
		{
			Float,
			Float3,
			Float4,
			Color,
			Texture2D,
			FloatArray,
			Float4Array,
			ColorArray,
			Matrix4x4
		}

		private struct Properties
		{
			public string name;
			public PropertyType type;
		}
	}
}