using UnityEngine;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Controls/Better Raw Image", 30)]
	public class BetterRawImage : RawImage, IImageAppearanceProvider
	{
		[SerializeField] private ColorMode colorMode;


		[SerializeField] private Color secondColor = Color.white;


		[SerializeField] private VertexMaterialData materialProperties = new VertexMaterialData();


		[SerializeField] private string materialType;


		[SerializeField] private MaterialEffect materialEffect;


		[SerializeField] private float materialProperty1;


		[SerializeField] private float materialProperty2;


		[SerializeField] private float materialProperty3;


		protected override void OnEnable()
		{
			base.OnEnable();
			if (MaterialProperties.FloatProperties != null)
			{
				if (MaterialProperties.FloatProperties.Length != 0)
				{
					materialProperty1 = MaterialProperties.FloatProperties[0].Value;
				}

				if (MaterialProperties.FloatProperties.Length > 1)
				{
					materialProperty2 = MaterialProperties.FloatProperties[1].Value;
				}

				if (MaterialProperties.FloatProperties.Length > 2)
				{
					materialProperty3 = MaterialProperties.FloatProperties[2].Value;
				}
			}
		}


		
		public string MaterialType {
			get => materialType;
			set => ImageAppearanceProviderHelper.SetMaterialType(value, this, materialProperties, ref materialEffect,
				ref materialType);
		}


		
		public MaterialEffect MaterialEffect {
			get => materialEffect;
			set => ImageAppearanceProviderHelper.SetMaterialEffect(value, this, materialProperties, ref materialEffect,
				ref materialType);
		}


		public VertexMaterialData MaterialProperties => materialProperties;


		
		public ColorMode ColoringMode {
			get => colorMode;
			set => colorMode = value;
		}


		
		public Color SecondColor {
			get => secondColor;
			set => secondColor = value;
		}


		public float GetMaterialPropertyValue(int propertyIndex)
		{
			return ImageAppearanceProviderHelper.GetMaterialPropertyValue(propertyIndex, ref materialProperty1,
				ref materialProperty2, ref materialProperty3);
		}


		public void SetMaterialProperty(int propertyIndex, float value)
		{
			ImageAppearanceProviderHelper.SetMaterialProperty(propertyIndex, value, this, materialProperties,
				ref materialProperty1, ref materialProperty2, ref materialProperty3);
		}


		protected override void OnPopulateMesh(VertexHelper vh)
		{
			Rect pixelAdjustedRect = GetPixelAdjustedRect();
			Vector2 posMin = new Vector2(pixelAdjustedRect.x, pixelAdjustedRect.y);
			Vector2 posMax = new Vector2(pixelAdjustedRect.x + pixelAdjustedRect.width,
				pixelAdjustedRect.y + pixelAdjustedRect.height);
			float num = texture != null ? texture.width * texture.texelSize.x : 1f;
			float num2 = texture != null ? texture.height * texture.texelSize.y : 1f;
			Vector2 uvMin = new Vector2(uvRect.xMin * num, uvRect.yMin * num2);
			Vector2 uvMax = new Vector2(uvRect.xMax * num, uvRect.yMax * num2);
			vh.Clear();
			ImageAppearanceProviderHelper.AddQuad(vh, pixelAdjustedRect, posMin, posMax, colorMode, color, secondColor,
				uvMin, uvMax, materialProperties);
		}
	}
}