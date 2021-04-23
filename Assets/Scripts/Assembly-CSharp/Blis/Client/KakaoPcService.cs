using System;
using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class KakaoPcService : SingletonMonoBehaviour<KakaoPcService>
	{
		private bool benefitByKakaoPcCafe;
		public bool BenefitByKakaoPcCafe => benefitByKakaoPcCafe;

		protected override void OnAwakeSingleton()
		{
			DontDestroyOnLoad(this);
		}

		public void SetBenefitByKakaoPcCafe(bool benefitByKakaoPcCafe)
		{
			if (this.benefitByKakaoPcCafe != benefitByKakaoPcCafe)
			{
				this.benefitByKakaoPcCafe = benefitByKakaoPcCafe;
				OnChangeBenefitByKakaoPcCafe();
			}
		}

		public void OnChangeBenefitByKakaoPcCafe()
		{
			if (benefitByKakaoPcCafe)
			{
				this.StartThrowingCoroutine(RequestKakaoPc(),
					delegate(Exception exception)
					{
						Log.E("[EXCEPTION][KakaoPc] Message:" + exception.Message + ", StackTrace:" +
						      exception.StackTrace);
					});
			}
		}

		private IEnumerator RequestKakaoPc()
		{
			while (benefitByKakaoPcCafe)
			{
				RequestDelegate.requestCoroutine<KakaoPcApi.KakaoApiResult>(KakaoPcApi.RequestKakaoPc(), null);
				yield return new WaitForSecondsRealtime(300f);
			}
		}
	}
}