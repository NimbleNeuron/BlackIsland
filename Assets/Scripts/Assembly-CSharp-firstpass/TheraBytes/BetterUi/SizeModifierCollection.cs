using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class SizeModifierCollection
	{
		public float ExponentialScale = 1f;


		public List<SizeModifier> SizeModifiers = new List<SizeModifier>
		{
			new SizeModifier()
		};


		public SizeModifierCollection(params SizeModifier[] mods) : this(1f, mods) { }


		public SizeModifierCollection(float exponentialScale, params SizeModifier[] mods)
		{
			ExponentialScale = exponentialScale;
			SizeModifiers = mods.ToList<SizeModifier>();
		}


		public float CalculateFactor(Component caller, string screenConfig)
		{
			OverrideScreenProperties componentInParent = caller.GetComponentInParent<OverrideScreenProperties>();
			ScreenInfo screenInfo = componentInParent != null
				? componentInParent.OptimizedOverride
				: ResolutionMonitor.GetOpimizedScreenInfo(screenConfig);
			Vector2 resolution = screenInfo.Resolution;
			float dpi = screenInfo.Dpi;
			Vector2 actualResolution = componentInParent != null
				? componentInParent.CurrentSize.Resolution
				: ResolutionMonitor.CurrentResolution;
			float actualDpi = componentInParent != null
				? componentInParent.CurrentSize.Dpi
				: ResolutionMonitor.CurrentDpi;
			float num = 0f;
			float num2;
			if (SizeModifiers.Count <= 0)
			{
				num2 = 1f;
			}
			else
			{
				num2 = SizeModifiers.Max(o => o.Impact);
			}

			float num3 = num2;
			float num4 = 0f;
			foreach (SizeModifier sizeModifier in SizeModifiers)
			{
				num += sizeModifier.GetFactor(caller, resolution, actualResolution, dpi, actualDpi);
				num4 += sizeModifier.Impact / num3;
			}

			if (num == 0f)
			{
				num = 1f;
			}
			else
			{
				num /= num4;
				num = 1f - num;
			}

			if (ExponentialScale != 1f)
			{
				num = Mathf.Pow(num, ExponentialScale);
			}

			return num;
		}


		public SizeModifierCollection Clone()
		{
			SizeModifierCollection other = new SizeModifierCollection(Array.Empty<SizeModifier>());
			CopyTo(other);
			return Clone();
		}


		public void CopyTo(SizeModifierCollection other)
		{
			other.SizeModifiers.Clear();
			other.ExponentialScale = ExponentialScale;
			foreach (SizeModifier sizeModifier in SizeModifiers)
			{
				other.SizeModifiers.Add(new SizeModifier(sizeModifier.Mode, sizeModifier.Impact));
			}
		}


		[Serializable]
		public class SizeModifier
		{
			public ImpactMode Mode;


			[Range(0f, 1f)] public float Impact = 1f;


			public SizeModifier() { }


			public SizeModifier(ImpactMode mode, float impact)
			{
				Mode = mode;
				Impact = impact;
			}


			internal float GetFactor(Component caller, Vector2 optimizedResolution, Vector2 actualResolution,
				float optimizedDpi, float actualDpi)
			{
				float result;
				switch (Mode)
				{
					case ImpactMode.PixelHeight:
						result = CalculateSize(optimizedResolution.y, actualResolution.y, Impact);
						break;
					case ImpactMode.PixelWidth:
						result = CalculateSize(optimizedResolution.x, actualResolution.x, Impact);
						break;
					case ImpactMode.AspectRatio:
					{
						float optimizedValue = optimizedResolution.x / optimizedResolution.y;
						float actualValue = actualResolution.x / actualResolution.y;
						result = CalculateSize(optimizedValue, actualValue, Impact);
						break;
					}
					case ImpactMode.InverseAspectRatio:
					{
						float optimizedValue2 = optimizedResolution.y / optimizedResolution.x;
						float actualValue2 = actualResolution.y / actualResolution.x;
						result = CalculateSize(optimizedValue2, actualValue2, Impact);
						break;
					}
					case ImpactMode.Dpi:
						result = CalculateSize(optimizedDpi, actualDpi, Impact);
						break;
					case ImpactMode.StaticMethod1:
					case ImpactMode.StaticMethod2:
					case ImpactMode.StaticMethod3:
					case ImpactMode.StaticMethod4:
					case ImpactMode.StaticMethod5:
						result = 1f - ResolutionMonitor.InvokeStaticMethod(Mode, caller, optimizedResolution,
							actualResolution, optimizedDpi, actualDpi);
						break;
					default:
						result = 0f;
						break;
				}

				return result;
			}


			private float CalculateSize(float optimizedValue, float actualValue, float impact)
			{
				if (impact == 0f || optimizedValue == actualValue)
				{
					return 0f;
				}

				float b = actualValue / optimizedValue;
				return 1f - Mathf.Lerp(1f, b, impact);
			}
		}
	}
}