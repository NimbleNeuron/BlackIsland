using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class BloodEffect : BaseUI
	{
		public enum AnimationType
		{
			None,

			Restricted,

			Damaged,

			HeavyDamaged
		}


		private Animator animator;


		private AnimationType currentAnimation;


		public AnimationType CurrentAnimation => currentAnimation;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<Animator>(gameObject, ref animator);
		}


		public void Play(AnimationType animationType)
		{
			if (MonoBehaviourInstance<ClientService>.inst.CurGamePlayMode == ClientService.GamePlayMode.ObserveTeam)
			{
				return;
			}

			if (currentAnimation != animationType)
			{
				animator.Play(animationType.ToString());
				currentAnimation = animationType;
			}
		}


		public void Stop()
		{
			animator.Play("None");
			currentAnimation = AnimationType.None;
		}


		public void OnFinish()
		{
			currentAnimation = AnimationType.None;
		}
	}
}