using System;
using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class Siren : RestrictedEffect
	{
		private const string SoundTag = "Siren";


		private Animator animator;

		private void Awake()
		{
			GameUtil.Bind<Animator>(gameObject, ref animator);
		}


		public override void SetState(AreaRestrictionState areaState, float time)
		{
			if (animator != null)
			{
				animator.SetInteger("areaRestrictionState", (int) areaState);
				if (areaState == AreaRestrictionState.Reserved)
				{
					this.StartThrowingCoroutine(PlayWarningSound(Mathf.Max(time - 20f, 0f)),
						delegate(Exception exception)
						{
							Log.E("[EXCEPTION][PlayWarningSound] Message:" + exception.Message + ", StackTrace:" +
							      exception.StackTrace);
						});
				}
			}
		}


		private IEnumerator PlayWarningSound(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			Singleton<SoundControl>.inst.PlayFXSound("effect_siren", "Siren", 10, gameObject.transform.position, false);
		}
	}
}