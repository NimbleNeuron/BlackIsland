using System;
using UnityEngine;

namespace Blis.Client
{
	public class EffectSound : MonoBehaviour
	{
		[Flags]
		public enum Property
		{
			None = 0,

			Loop = 1,

			InChild = 2,

			IsStopParentIsNull = 4
		}


		public const string SoundTag = "EffectSound";


		public string audioClip;


		public int maxDistance = 20;


		public Property property;

		public void Awake()
		{
			if (property.HasFlag(Property.InChild))
			{
				Singleton<SoundControl>.inst.PlayFXSoundChild(audioClip, "EffectSound", maxDistance,
					property.HasFlag(Property.Loop), transform, property.HasFlag(Property.IsStopParentIsNull));
				return;
			}

			Singleton<SoundControl>.inst.PlayFXSound(audioClip, "EffectSound", maxDistance, transform.position,
				property.HasFlag(Property.Loop));
		}
	}
}