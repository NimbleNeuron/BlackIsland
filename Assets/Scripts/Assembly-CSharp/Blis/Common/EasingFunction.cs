using UnityEngine;

namespace Blis.Common
{
	public static class EasingFunction
	{
		public delegate float Function(float s, float e, float v);


		public enum Ease
		{
			EaseInQuad,
			EaseOutQuad,
			EaseInOutQuad,
			EaseInCubic,
			EaseOutCubic,
			EaseInOutCubic,
			EaseInQuart,
			EaseOutQuart,
			EaseInOutQuart,
			EaseInQuint,
			EaseOutQuint,
			EaseInOutQuint,
			EaseInSine,
			EaseOutSine,
			EaseInOutSine,
			EaseInExpo,
			EaseOutExpo,
			EaseInOutExpo,
			EaseInCirc,
			EaseOutCirc,
			EaseInOutCirc,
			Linear,
			Spring,
			EaseInBounce,
			EaseOutBounce,
			EaseInOutBounce,
			EaseInBack,
			EaseOutBack,
			EaseInOutBack,
			EaseInElastic,
			EaseOutElastic,
			EaseInOutElastic
		}

		private const float NATURAL_LOG_OF_2 = 0.6931472f;
		public static float Linear(float start, float end, float value)
		{
			return Mathf.Lerp(start, end, value);
		}

		public static float Spring(float start, float end, float value)
		{
			value = Mathf.Clamp01(value);
			value =
				(Mathf.Sin(value * 3.1415927f * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) +
				 value) * (1f + 1.2f * (1f - value));
			return start + (end - start) * value;
		}


		public static float EaseInQuad(float start, float end, float value)
		{
			end -= start;
			return end * value * value + start;
		}


		public static float EaseOutQuad(float start, float end, float value)
		{
			end -= start;
			return -end * value * (value - 2f) + start;
		}


		public static float EaseInOutQuad(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * value * value + start;
			}

			value -= 1f;
			return -end * 0.5f * (value * (value - 2f) - 1f) + start;
		}


		public static float EaseInCubic(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value + start;
		}


		public static float EaseOutCubic(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * (value * value * value + 1f) + start;
		}


		public static float EaseInOutCubic(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * value * value * value + start;
			}

			value -= 2f;
			return end * 0.5f * (value * value * value + 2f) + start;
		}


		public static float EaseInQuart(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value + start;
		}


		public static float EaseOutQuart(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return -end * (value * value * value * value - 1f) + start;
		}


		public static float EaseInOutQuart(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * value * value * value * value + start;
			}

			value -= 2f;
			return -end * 0.5f * (value * value * value * value - 2f) + start;
		}


		public static float EaseInQuint(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value * value + start;
		}


		public static float EaseOutQuint(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * (value * value * value * value * value + 1f) + start;
		}


		public static float EaseInOutQuint(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * value * value * value * value * value + start;
			}

			value -= 2f;
			return end * 0.5f * (value * value * value * value * value + 2f) + start;
		}


		public static float EaseInSine(float start, float end, float value)
		{
			end -= start;
			return -end * Mathf.Cos(value * 1.5707964f) + end + start;
		}


		public static float EaseOutSine(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Sin(value * 1.5707964f) + start;
		}


		public static float EaseInOutSine(float start, float end, float value)
		{
			end -= start;
			return -end * 0.5f * (Mathf.Cos(3.1415927f * value) - 1f) + start;
		}


		public static float EaseInExpo(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Pow(2f, 10f * (value - 1f)) + start;
		}


		public static float EaseOutExpo(float start, float end, float value)
		{
			end -= start;
			return end * (-Mathf.Pow(2f, -10f * value) + 1f) + start;
		}


		public static float EaseInOutExpo(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * Mathf.Pow(2f, 10f * (value - 1f)) + start;
			}

			value -= 1f;
			return end * 0.5f * (-Mathf.Pow(2f, -10f * value) + 2f) + start;
		}


		public static float EaseInCirc(float start, float end, float value)
		{
			end -= start;
			return -end * (Mathf.Sqrt(1f - value * value) - 1f) + start;
		}


		public static float EaseOutCirc(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * Mathf.Sqrt(1f - value * value) + start;
		}


		public static float EaseInOutCirc(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return -end * 0.5f * (Mathf.Sqrt(1f - value * value) - 1f) + start;
			}

			value -= 2f;
			return end * 0.5f * (Mathf.Sqrt(1f - value * value) + 1f) + start;
		}


		public static float EaseInBounce(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			return end - EaseOutBounce(0f, end, num - value) + start;
		}


		public static float EaseOutBounce(float start, float end, float value)
		{
			value /= 1f;
			end -= start;
			if (value < 0.36363637f)
			{
				return end * (7.5625f * value * value) + start;
			}

			if (value < 0.72727275f)
			{
				value -= 0.54545456f;
				return end * (7.5625f * value * value + 0.75f) + start;
			}

			if (value < 0.9090909090909091)
			{
				value -= 0.8181818f;
				return end * (7.5625f * value * value + 0.9375f) + start;
			}

			value -= 0.95454544f;
			return end * (7.5625f * value * value + 0.984375f) + start;
		}


		public static float EaseInOutBounce(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			if (value < num * 0.5f)
			{
				return EaseInBounce(0f, end, value * 2f) * 0.5f + start;
			}

			return EaseOutBounce(0f, end, value * 2f - num) * 0.5f + end * 0.5f + start;
		}


		public static float EaseInBack(float start, float end, float value)
		{
			end -= start;
			value /= 1f;
			float num = 1.70158f;
			return end * value * value * ((num + 1f) * value - num) + start;
		}


		public static float EaseOutBack(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value -= 1f;
			return end * (value * value * ((num + 1f) * value + num) + 1f) + start;
		}


		public static float EaseInOutBack(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value /= 0.5f;
			if (value < 1f)
			{
				num *= 1.525f;
				return end * 0.5f * (value * value * ((num + 1f) * value - num)) + start;
			}

			value -= 2f;
			num *= 1.525f;
			return end * 0.5f * (value * value * ((num + 1f) * value + num) + 2f) + start;
		}


		public static float EaseInElastic(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			if (value == 0f)
			{
				return start;
			}

			if ((value /= num) == 1f)
			{
				return start + end;
			}

			float num4;
			if (num3 == 0f || num3 < Mathf.Abs(end))
			{
				num3 = end;
				num4 = num2 / 4f;
			}
			else
			{
				num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
			}

			return -(num3 * Mathf.Pow(2f, 10f * (value -= 1f)) * Mathf.Sin((value * num - num4) * 6.2831855f / num2)) +
			       start;
		}


		public static float EaseOutElastic(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			if (value == 0f)
			{
				return start;
			}

			if ((value /= num) == 1f)
			{
				return start + end;
			}

			float num4;
			if (num3 == 0f || num3 < Mathf.Abs(end))
			{
				num3 = end;
				num4 = num2 * 0.25f;
			}
			else
			{
				num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
			}

			return num3 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * num - num4) * 6.2831855f / num2) + end +
			       start;
		}


		public static float EaseInOutElastic(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			if (value == 0f)
			{
				return start;
			}

			if ((value /= num * 0.5f) == 2f)
			{
				return start + end;
			}

			float num4;
			if (num3 == 0f || num3 < Mathf.Abs(end))
			{
				num3 = end;
				num4 = num2 / 4f;
			}
			else
			{
				num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
			}

			if (value < 1f)
			{
				return -0.5f * (num3 * Mathf.Pow(2f, 10f * (value -= 1f)) *
				                Mathf.Sin((value * num - num4) * 6.2831855f / num2)) + start;
			}

			return num3 * Mathf.Pow(2f, -10f * (value -= 1f)) * Mathf.Sin((value * num - num4) * 6.2831855f / num2) *
				0.5f + end + start;
		}


		public static float LinearD(float start, float end, float value)
		{
			return end - start;
		}


		public static float EaseInQuadD(float start, float end, float value)
		{
			return 2f * (end - start) * value;
		}


		public static float EaseOutQuadD(float start, float end, float value)
		{
			end -= start;
			return -end * value - end * (value - 2f);
		}


		public static float EaseInOutQuadD(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * value;
			}

			value -= 1f;
			return end * (1f - value);
		}


		public static float EaseInCubicD(float start, float end, float value)
		{
			return 3f * (end - start) * value * value;
		}


		public static float EaseOutCubicD(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return 3f * end * value * value;
		}


		public static float EaseInOutCubicD(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return 1.5f * end * value * value;
			}

			value -= 2f;
			return 1.5f * end * value * value;
		}


		public static float EaseInQuartD(float start, float end, float value)
		{
			return 4f * (end - start) * value * value * value;
		}


		public static float EaseOutQuartD(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return -4f * end * value * value * value;
		}


		public static float EaseInOutQuartD(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return 2f * end * value * value * value;
			}

			value -= 2f;
			return -2f * end * value * value * value;
		}


		public static float EaseInQuintD(float start, float end, float value)
		{
			return 5f * (end - start) * value * value * value * value;
		}


		public static float EaseOutQuintD(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return 5f * end * value * value * value * value;
		}


		public static float EaseInOutQuintD(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return 2.5f * end * value * value * value * value;
			}

			value -= 2f;
			return 2.5f * end * value * value * value * value;
		}


		public static float EaseInSineD(float start, float end, float value)
		{
			return (end - start) * 0.5f * 3.1415927f * Mathf.Sin(1.5707964f * value);
		}


		public static float EaseOutSineD(float start, float end, float value)
		{
			end -= start;
			return 1.5707964f * end * Mathf.Cos(value * 1.5707964f);
		}


		public static float EaseInOutSineD(float start, float end, float value)
		{
			end -= start;
			return end * 0.5f * 3.1415927f * Mathf.Sin(3.1415927f * value);
		}


		public static float EaseInExpoD(float start, float end, float value)
		{
			return 6.931472f * (end - start) * Mathf.Pow(2f, 10f * (value - 1f));
		}


		public static float EaseOutExpoD(float start, float end, float value)
		{
			end -= start;
			return 3.465736f * end * Mathf.Pow(2f, 1f - 10f * value);
		}


		public static float EaseInOutExpoD(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return 3.465736f * end * Mathf.Pow(2f, 10f * (value - 1f));
			}

			value -= 1f;
			return 3.465736f * end / Mathf.Pow(2f, 10f * value);
		}


		public static float EaseInCircD(float start, float end, float value)
		{
			return (end - start) * value / Mathf.Sqrt(1f - value * value);
		}


		public static float EaseOutCircD(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return -end * value / Mathf.Sqrt(1f - value * value);
		}


		public static float EaseInOutCircD(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * value / (2f * Mathf.Sqrt(1f - value * value));
			}

			value -= 2f;
			return -end * value / (2f * Mathf.Sqrt(1f - value * value));
		}


		public static float EaseInBounceD(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			return EaseOutBounceD(0f, end, num - value);
		}


		public static float EaseOutBounceD(float start, float end, float value)
		{
			value /= 1f;
			end -= start;
			if (value < 0.36363637f)
			{
				return 2f * end * 7.5625f * value;
			}

			if (value < 0.72727275f)
			{
				value -= 0.54545456f;
				return 2f * end * 7.5625f * value;
			}

			if (value < 0.9090909090909091)
			{
				value -= 0.8181818f;
				return 2f * end * 7.5625f * value;
			}

			value -= 0.95454544f;
			return 2f * end * 7.5625f * value;
		}


		public static float EaseInOutBounceD(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			if (value < num * 0.5f)
			{
				return EaseInBounceD(0f, end, value * 2f) * 0.5f;
			}

			return EaseOutBounceD(0f, end, value * 2f - num) * 0.5f;
		}


		public static float EaseInBackD(float start, float end, float value)
		{
			float num = 1.70158f;
			return 3f * (num + 1f) * (end - start) * value * value - 2f * num * (end - start) * value;
		}


		public static float EaseOutBackD(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value -= 1f;
			return end * ((num + 1f) * value * value + 2f * value * ((num + 1f) * value + num));
		}


		public static float EaseInOutBackD(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value /= 0.5f;
			if (value < 1f)
			{
				num *= 1.525f;
				return 0.5f * end * (num + 1f) * value * value + end * value * ((num + 1f) * value - num);
			}

			value -= 2f;
			num *= 1.525f;
			return 0.5f * end * ((num + 1f) * value * value + 2f * value * ((num + 1f) * value + num));
		}


		public static float EaseInElasticD(float start, float end, float value)
		{
			return EaseOutElasticD(start, end, 1f - value);
		}


		public static float EaseOutElasticD(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			float num4;
			if (num3 == 0f || num3 < Mathf.Abs(end))
			{
				num3 = end;
				num4 = num2 * 0.25f;
			}
			else
			{
				num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
			}

			return num3 * 3.1415927f * num * Mathf.Pow(2f, 1f - 10f * value) *
				Mathf.Cos(6.2831855f * (num * value - num4) / num2) / num2 - 3.465736f * num3 *
				Mathf.Pow(2f, 1f - 10f * value) * Mathf.Sin(6.2831855f * (num * value - num4) / num2);
		}


		public static float EaseInOutElasticD(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			float num4;
			if (num3 == 0f || num3 < Mathf.Abs(end))
			{
				num3 = end;
				num4 = num2 / 4f;
			}
			else
			{
				num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
			}

			if (value < 1f)
			{
				value -= 1f;
				return -3.465736f * num3 * Mathf.Pow(2f, 10f * value) *
					Mathf.Sin(6.2831855f * (num * value - 2f) / num2) - num3 * 3.1415927f * num *
					Mathf.Pow(2f, 10f * value) * Mathf.Cos(6.2831855f * (num * value - num4) / num2) / num2;
			}

			value -= 1f;
			return num3 * 3.1415927f * num * Mathf.Cos(6.2831855f * (num * value - num4) / num2) /
				(num2 * Mathf.Pow(2f, 10f * value)) - 3.465736f * num3 *
				Mathf.Sin(6.2831855f * (num * value - num4) / num2) / Mathf.Pow(2f, 10f * value);
		}


		public static float SpringD(float start, float end, float value)
		{
			value = Mathf.Clamp01(value);
			end -= start;
			return end * (6f * (1f - value) / 5f + 1f) *
				(-2.2f * Mathf.Pow(1f - value, 1.2f) *
				 Mathf.Sin(3.1415927f * value * (2.5f * value * value * value + 0.2f)) +
				 Mathf.Pow(1f - value, 2.2f) *
				 (3.1415927f * (2.5f * value * value * value + 0.2f) + 23.561945f * value * value * value) *
				 Mathf.Cos(3.1415927f * value * (2.5f * value * value * value + 0.2f)) + 1f) - 6f * end *
				(Mathf.Pow(1f - value, 2.2f) * Mathf.Sin(3.1415927f * value * (2.5f * value * value * value + 0.2f)) +
				 value / 5f);
		}


		public static Function GetEasingFunction(Ease easingFunction)
		{
			if (easingFunction == Ease.EaseInQuad)
			{
				return EaseInQuad;
			}

			if (easingFunction == Ease.EaseOutQuad)
			{
				return EaseOutQuad;
			}

			if (easingFunction == Ease.EaseInOutQuad)
			{
				return EaseInOutQuad;
			}

			if (easingFunction == Ease.EaseInCubic)
			{
				return EaseInCubic;
			}

			if (easingFunction == Ease.EaseOutCubic)
			{
				return EaseOutCubic;
			}

			if (easingFunction == Ease.EaseInOutCubic)
			{
				return EaseInOutCubic;
			}

			if (easingFunction == Ease.EaseInQuart)
			{
				return EaseInQuart;
			}

			if (easingFunction == Ease.EaseOutQuart)
			{
				return EaseOutQuart;
			}

			if (easingFunction == Ease.EaseInOutQuart)
			{
				return EaseInOutQuart;
			}

			if (easingFunction == Ease.EaseInQuint)
			{
				return EaseInQuint;
			}

			if (easingFunction == Ease.EaseOutQuint)
			{
				return EaseOutQuint;
			}

			if (easingFunction == Ease.EaseInOutQuint)
			{
				return EaseInOutQuint;
			}

			if (easingFunction == Ease.EaseInSine)
			{
				return EaseInSine;
			}

			if (easingFunction == Ease.EaseOutSine)
			{
				return EaseOutSine;
			}

			if (easingFunction == Ease.EaseInOutSine)
			{
				return EaseInOutSine;
			}

			if (easingFunction == Ease.EaseInExpo)
			{
				return EaseInExpo;
			}

			if (easingFunction == Ease.EaseOutExpo)
			{
				return EaseOutExpo;
			}

			if (easingFunction == Ease.EaseInOutExpo)
			{
				return EaseInOutExpo;
			}

			if (easingFunction == Ease.EaseInCirc)
			{
				return EaseInCirc;
			}

			if (easingFunction == Ease.EaseOutCirc)
			{
				return EaseOutCirc;
			}

			if (easingFunction == Ease.EaseInOutCirc)
			{
				return EaseInOutCirc;
			}

			if (easingFunction == Ease.Linear)
			{
				return Linear;
			}

			if (easingFunction == Ease.Spring)
			{
				return Spring;
			}

			if (easingFunction == Ease.EaseInBounce)
			{
				return EaseInBounce;
			}

			if (easingFunction == Ease.EaseOutBounce)
			{
				return EaseOutBounce;
			}

			if (easingFunction == Ease.EaseInOutBounce)
			{
				return EaseInOutBounce;
			}

			if (easingFunction == Ease.EaseInBack)
			{
				return EaseInBack;
			}

			if (easingFunction == Ease.EaseOutBack)
			{
				return EaseOutBack;
			}

			if (easingFunction == Ease.EaseInOutBack)
			{
				return EaseInOutBack;
			}

			if (easingFunction == Ease.EaseInElastic)
			{
				return EaseInElastic;
			}

			if (easingFunction == Ease.EaseOutElastic)
			{
				return EaseOutElastic;
			}

			if (easingFunction == Ease.EaseInOutElastic)
			{
				return EaseInOutElastic;
			}

			return null;
		}


		public static Function GetEasingFunctionDerivative(Ease easingFunction)
		{
			if (easingFunction == Ease.EaseInQuad)
			{
				return EaseInQuadD;
			}

			if (easingFunction == Ease.EaseOutQuad)
			{
				return EaseOutQuadD;
			}

			if (easingFunction == Ease.EaseInOutQuad)
			{
				return EaseInOutQuadD;
			}

			if (easingFunction == Ease.EaseInCubic)
			{
				return EaseInCubicD;
			}

			if (easingFunction == Ease.EaseOutCubic)
			{
				return EaseOutCubicD;
			}

			if (easingFunction == Ease.EaseInOutCubic)
			{
				return EaseInOutCubicD;
			}

			if (easingFunction == Ease.EaseInQuart)
			{
				return EaseInQuartD;
			}

			if (easingFunction == Ease.EaseOutQuart)
			{
				return EaseOutQuartD;
			}

			if (easingFunction == Ease.EaseInOutQuart)
			{
				return EaseInOutQuartD;
			}

			if (easingFunction == Ease.EaseInQuint)
			{
				return EaseInQuintD;
			}

			if (easingFunction == Ease.EaseOutQuint)
			{
				return EaseOutQuintD;
			}

			if (easingFunction == Ease.EaseInOutQuint)
			{
				return EaseInOutQuintD;
			}

			if (easingFunction == Ease.EaseInSine)
			{
				return EaseInSineD;
			}

			if (easingFunction == Ease.EaseOutSine)
			{
				return EaseOutSineD;
			}

			if (easingFunction == Ease.EaseInOutSine)
			{
				return EaseInOutSineD;
			}

			if (easingFunction == Ease.EaseInExpo)
			{
				return EaseInExpoD;
			}

			if (easingFunction == Ease.EaseOutExpo)
			{
				return EaseOutExpoD;
			}

			if (easingFunction == Ease.EaseInOutExpo)
			{
				return EaseInOutExpoD;
			}

			if (easingFunction == Ease.EaseInCirc)
			{
				return EaseInCircD;
			}

			if (easingFunction == Ease.EaseOutCirc)
			{
				return EaseOutCircD;
			}

			if (easingFunction == Ease.EaseInOutCirc)
			{
				return EaseInOutCircD;
			}

			if (easingFunction == Ease.Linear)
			{
				return LinearD;
			}

			if (easingFunction == Ease.Spring)
			{
				return SpringD;
			}

			if (easingFunction == Ease.EaseInBounce)
			{
				return EaseInBounceD;
			}

			if (easingFunction == Ease.EaseOutBounce)
			{
				return EaseOutBounceD;
			}

			if (easingFunction == Ease.EaseInOutBounce)
			{
				return EaseInOutBounceD;
			}

			if (easingFunction == Ease.EaseInBack)
			{
				return EaseInBackD;
			}

			if (easingFunction == Ease.EaseOutBack)
			{
				return EaseOutBackD;
			}

			if (easingFunction == Ease.EaseInOutBack)
			{
				return EaseInOutBackD;
			}

			if (easingFunction == Ease.EaseInElastic)
			{
				return EaseInElasticD;
			}

			if (easingFunction == Ease.EaseOutElastic)
			{
				return EaseOutElasticD;
			}

			if (easingFunction == Ease.EaseInOutElastic)
			{
				return EaseInOutElasticD;
			}

			return null;
		}
	}
}