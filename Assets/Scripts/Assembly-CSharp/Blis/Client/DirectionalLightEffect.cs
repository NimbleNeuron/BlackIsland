using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class DirectionalLightEffect : MonoBehaviourInstance<DirectionalLightEffect>
	{
		public Color day;


		public Color night;


		private Light _light;


		private LightTweener lightTweener;

		protected override void _Awake()
		{
			base._Awake();
			_light = GameUtil.Bind<Light>(gameObject, ref _light);
			lightTweener = GameUtil.Bind<LightTweener>(gameObject, ref lightTweener);
		}


		public void SetDayNight(DayNight dayNight)
		{
			if (dayNight == DayNight.Day)
			{
				lightTweener.from = _light.color;
				lightTweener.to = day;
			}
			else
			{
				lightTweener.from = _light.color;
				lightTweener.to = night;
			}

			lightTweener.PlayAnimation();
		}
	}
}