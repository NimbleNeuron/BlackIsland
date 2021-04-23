using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blis.Client
{
	[Serializable]
	public class ExtraPointDisplayData
	{
		[SerializeField] private int characterCode = -1;


		[SerializeField] private ExtraPointDisplayType extraPointDisplayType = ExtraPointDisplayType.Gauge;


		[SerializeField] private Sprite gaugeIcon = default;


		[SerializeField] private Sprite stackIconEmpty = default;


		[SerializeField] private Sprite stackIconFill = default;


		[SerializeField] private float stackIconInterval = default;


		[SerializeField] private List<float> dots = default;


		[SerializeField] private List<float> colorStarts = default;


		[SerializeField] private List<Color> colors = default;


		public int CharacterCode => characterCode;


		public ExtraPointDisplayType ExtraPointDisplayType => extraPointDisplayType;


		public Sprite GaugeIcon => gaugeIcon;


		public Sprite StackIconEmpty => stackIconEmpty;


		public Sprite StackIconFill => stackIconFill;


		public float StackIconInterval => stackIconInterval;


		public List<float> Dots => dots;


		public List<float> ColorStarts => colorStarts;


		public List<Color> Colors => colors;
	}
}