using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace TheraBytes.BetterUi
{
	public class Materials : SingletonScriptableObject<Materials>
	{
		private const string STANDARD = "Standard";


		private const string GRAYSCALE = "Grayscale";


		private const string HUE_SATURATION_BRIGHTNESS = "Hue Saturation Brightness";


		private static readonly List<string> materialOrder = new List<string>
		{
			"Standard",
			"Grayscale",
			"Hue Saturation Brightness"
		};


		[SerializeField] private List<MaterialInfo> materials = new List<MaterialInfo>();


		private static string FilePath => "Standard Assets/TheraBytes/Resources/Materials";


		private void OnEnable()
		{
			EnsurePredefinedMaterials();
		}


		private void EnsurePredefinedMaterials()
		{
			if (materials.Count > 0)
			{
				return;
			}

			AddIfNotPresent("Standard", e => new MaterialInfo
			{
				Material = Resources.Load<Material>("Materials/Standard_" + e)
			}, Array.Empty<MaterialEffect>());
			AddIfNotPresent("Grayscale", e => new MaterialInfo
			{
				Material = Resources.Load<Material>("Materials/Grayscale_" + e)
			}, Array.Empty<MaterialEffect>());
			AddIfNotPresent("Hue Saturation Brightness", e => new MaterialInfo
			{
				Material = Resources.Load<Material>("Materials/HueSaturationBrightness_" + e),
				Properties = new VertexMaterialData
				{
					FloatProperties = new[]
					{
						new VertexMaterialData.FloatProperty
						{
							Name = "Hue",
							Min = 0f,
							Max = 1f,
							Value = 0f,
							PropertyMap = VertexMaterialData.FloatProperty.Mapping.TexcoordX
						},
						new VertexMaterialData.FloatProperty
						{
							Name = "Saturation",
							Value = 1f,
							PropertyMap = VertexMaterialData.FloatProperty.Mapping.TexcoordY
						},
						new VertexMaterialData.FloatProperty
						{
							Name = "Brightness",
							Value = 1f,
							PropertyMap = VertexMaterialData.FloatProperty.Mapping.TangentW
						}
					}
				}
			}, Array.Empty<MaterialEffect>());
			AddIfNotPresent("Color Overlay", e => new MaterialInfo
			{
				Material = Resources.Load<Material>("Materials/ColorOverlay_" + e),
				Properties = new VertexMaterialData
				{
					FloatProperties = new[]
					{
						new VertexMaterialData.FloatProperty
						{
							Name = "Opacity",
							Min = 0f,
							Max = 1f,
							Value = 0f,
							PropertyMap = VertexMaterialData.FloatProperty.Mapping.TexcoordX
						}
					}
				}
			}, Array.Empty<MaterialEffect>());
		}


		private void AddIfNotPresent(string name, Func<MaterialEffect, MaterialInfo> CreateMaterial,
			params MaterialEffect[] preservedLayerEffects)
		{
			foreach (object obj in Enum.GetValues(typeof(MaterialEffect)))
			{
				MaterialEffect materialEffect = (MaterialEffect) obj;
				MaterialInfo materialInfo = GetMaterialInfo(name, materialEffect);
				if (materialInfo == null)
				{
					materialInfo = CreateMaterial(materialEffect);
					if (materialInfo.Material == null)
					{
						continue;
					}

					materialInfo.Name = name;
					materialInfo.Effect = materialEffect;
					materials.Add(materialInfo);
				}

				float value = 0.001f;
				bool flag = false;
				bool flag2 = false;
				BlendMode value2;
				BlendMode value3;
				switch (materialEffect)
				{
					case MaterialEffect.Normal:
						value2 = BlendMode.SrcAlpha;
						value3 = BlendMode.OneMinusSrcAlpha;
						break;
					case MaterialEffect.Additive:
						value2 = BlendMode.OneMinusDstColor;
						value3 = BlendMode.One;
						flag2 = true;
						break;
					case MaterialEffect.LinearDodge:
						value2 = BlendMode.SrcAlpha;
						value3 = BlendMode.One;
						flag2 = true;
						flag = true;
						break;
					case MaterialEffect.Multiply:
						value2 = BlendMode.DstColor;
						value3 = BlendMode.Zero;
						flag = true;
						value = 0.5f;
						break;
					case MaterialEffect.Overlay:
						value2 = BlendMode.DstAlpha;
						value3 = BlendMode.OneMinusDstColor;
						flag = true;
						value = 0.5f;
						break;
					default:
						throw new ArgumentException();
				}

				materialInfo.Material.SetInt("SrcBlendMode", (int) value2);
				materialInfo.Material.SetInt("DstBlendMode", (int) value3);
				materialInfo.Material.SetFloat("ClipThreshold", value);
				if (flag2)
				{
					materialInfo.Material.EnableKeyword("COMBINE_ALPHA");
				}
				else
				{
					materialInfo.Material.DisableKeyword("COMBINE_ALPHA");
				}

				if (flag)
				{
					materialInfo.Material.EnableKeyword("FORCE_CLIP");
				}
				else
				{
					materialInfo.Material.DisableKeyword("FORCE_CLIP");
				}
			}
		}


		private IEnumerator SetTogglePropertyDelayed(Material material, string toggleName, bool toggle)
		{
			yield return null;
			material.SetInt(toggleName, toggle ? 1 : 0);
		}


		public MaterialInfo GetMaterialInfo(string name, MaterialEffect e)
		{
			return materials.FirstOrDefault(o => o.Name == name && o.Effect == e);
		}


		public Material GetMaterial(string name)
		{
			MaterialInfo materialInfo = materials.FirstOrDefault(o => o.Name == name);
			if (materialInfo != null)
			{
				return materialInfo.Material;
			}

			return null;
		}


		public List<string> GetAllMaterialNames()
		{
			EnsurePredefinedMaterials();
			List<string> list = new HashSet<string>(from o in materials
				select o.Name).ToList<string>();
			list.Sort(delegate(string a, string b)
			{
				if (!materialOrder.Contains(a))
				{
					return 1;
				}

				if (materialOrder.Contains(b))
				{
					return materialOrder.IndexOf(a).CompareTo(materialOrder.IndexOf(b));
				}

				return -1;
			});
			return list;
		}


		public HashSet<MaterialEffect> GetAllMaterialEffects(string name)
		{
			return new HashSet<MaterialEffect>(from o in materials
				where o.Name == name
				select o.Effect);
		}


		[Serializable]
		public class MaterialInfo
		{
			public string Name;


			public Material Material;


			public VertexMaterialData Properties = new VertexMaterialData();


			public MaterialEffect Effect;


			public override string ToString()
			{
				return string.Format("{0} ({1})", Name, Effect);
			}
		}
	}
}