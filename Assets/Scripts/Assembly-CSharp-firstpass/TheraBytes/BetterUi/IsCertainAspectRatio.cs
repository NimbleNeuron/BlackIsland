using System;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class IsCertainAspectRatio : IScreenTypeCheck, IIsActive
	{
		[SerializeField] private float minAspect = 0.66f;


		[SerializeField] private float maxAspect = 1.5f;


		[SerializeField] private bool inverse;


		[SerializeField] private bool isActive;


		
		public float MinAspect {
			get => minAspect;
			set => minAspect = value;
		}


		
		public float MaxAspect {
			get => maxAspect;
			set => maxAspect = value;
		}


		
		public bool Inverse {
			get => inverse;
			set => inverse = value;
		}


		
		public bool IsActive {
			get => isActive;
			set => isActive = value;
		}


		public bool IsScreenType()
		{
			float num = Screen.width / (float) Screen.height;
			return !inverse && num >= minAspect && num <= maxAspect || inverse && num < minAspect && num > maxAspect;
		}
	}
}