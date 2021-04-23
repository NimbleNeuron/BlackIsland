using System;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class IsScreenOfCertainSize : IScreenTypeCheck, IIsActive
	{
		public enum ScreenMeasure
		{
			Width,


			Height,


			Diagonal
		}


		public enum UnitType
		{
			Inches,


			Centimeters
		}


		[SerializeField] private ScreenMeasure measureType = ScreenMeasure.Height;


		[SerializeField] private UnitType unitType;


		[SerializeField] private float minSizeInInches = 4.7f;


		[SerializeField] private float maxSizeInInches = 7.6f;


		[SerializeField] private bool isActive;


		
		public ScreenMeasure MeasureType {
			get => measureType;
			set => measureType = value;
		}


		
		public UnitType Units {
			get => unitType;
			set => unitType = value;
		}


		
		public float MinSize {
			get
			{
				if (unitType != UnitType.Centimeters)
				{
					return minSizeInInches;
				}

				return 2.54f * minSizeInInches;
			}
			set => minSizeInInches = unitType == UnitType.Centimeters ? value / 2.54f : value;
		}


		
		public float MaxSize {
			get
			{
				if (unitType != UnitType.Centimeters)
				{
					return maxSizeInInches;
				}

				return 2.54f * maxSizeInInches;
			}
			set => maxSizeInInches = unitType == UnitType.Centimeters ? value / 2.54f : value;
		}


		
		public bool IsActive {
			get => isActive;
			set => isActive = value;
		}


		public bool IsScreenType()
		{
			Vector2 currentResolution = ResolutionMonitor.CurrentResolution;
			float currentDpi = ResolutionMonitor.CurrentDpi;
			float num;
			switch (measureType)
			{
				case ScreenMeasure.Width:
					num = currentResolution.x / currentDpi;
					break;
				case ScreenMeasure.Height:
					num = currentResolution.y / currentDpi;
					break;
				case ScreenMeasure.Diagonal:
					num = Mathf.Sqrt(currentResolution.x * currentResolution.x +
					                 currentResolution.y * currentResolution.y) / currentDpi;
					break;
				default:
					throw new NotImplementedException();
			}

			return num >= minSizeInInches && num < maxSizeInInches;
		}
	}
}