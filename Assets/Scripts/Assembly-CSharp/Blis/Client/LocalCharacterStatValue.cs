using Blis.Common;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Blis.Client
{
	public class LocalCharacterStatValue : CharacterStatValue
	{
		private ObscuredInt value;

		public LocalCharacterStatValue(StatType statType, int value) : base(statType)
		{
			this.value = value;
		}


		public override void Set(float v)
		{
			value = Mathf.RoundToInt(v * 100f);
		}


		public override void Set(int v)
		{
			value = Mathf.RoundToInt(v * 100f);
		}


		public override int GetIntValue()
		{
			int result = Mathf.RoundToInt(value / 100f);
			value.RandomizeCryptoKey();
			return result;
		}


		public override int GetIntValue(bool shuffle)
		{
			return Mathf.RoundToInt(value / 100f);
		}


		public override float GetValue()
		{
			float result = value / 100f;
			value.RandomizeCryptoKey();
			return result;
		}


		public override float GetValue(bool shuffle)
		{
			if (shuffle)
			{
				return GetValue();
			}

			return value / 100f;
		}
	}
}