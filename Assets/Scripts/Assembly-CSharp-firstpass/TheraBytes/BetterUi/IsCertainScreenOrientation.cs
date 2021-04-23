using System;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class IsCertainScreenOrientation : IScreenTypeCheck, IIsActive
	{
		public enum Orientation
		{
			Portrait,


			Landscape
		}


		[SerializeField] private Orientation expectedOrientation;


		[SerializeField] private bool isActive;


		public IsCertainScreenOrientation(Orientation expectedOrientation)
		{
			this.expectedOrientation = expectedOrientation;
		}


		
		public Orientation ExpectedOrientation {
			get => expectedOrientation;
			set => expectedOrientation = value;
		}


		
		public bool IsActive {
			get => isActive;
			set => isActive = value;
		}


		public bool IsScreenType()
		{
			Vector2 currentResolution = ResolutionMonitor.CurrentResolution;
			Orientation orientation = expectedOrientation;
			if (orientation == Orientation.Portrait)
			{
				return currentResolution.x < currentResolution.y;
			}

			if (orientation != Orientation.Landscape)
			{
				throw new NotImplementedException();
			}

			return currentResolution.x >= currentResolution.y;
		}
	}
}