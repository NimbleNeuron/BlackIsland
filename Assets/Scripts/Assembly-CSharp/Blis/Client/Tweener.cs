using UnityEngine;

namespace Blis.Client
{
	public abstract class Tweener : MonoBehaviour
	{
		public delegate void AnimationFinishEvent();


		public enum AnimationType
		{
			PingPong,

			Loop,

			Once
		}


		[SerializeField] protected AnimationType animationType;


		public float speed = 0.5f;


		[SerializeField] private AnimationCurve curve;


		[SerializeField] private bool playOnEnable = true;


		private bool isPlaying;


		private float time;


		public bool IsPlaying => isPlaying;


		private void Update()
		{
			if (isPlaying)
			{
				if (animationType == AnimationType.Once && time > 1f)
				{
					OnUpdateAnimationValue(curve.Evaluate(1f));
					StopAnimation();
					return;
				}

				float value = curve.Evaluate(time);
				OnUpdateAnimationValue(value);
				time += Time.deltaTime * (1f / speed);
			}
		}


		private void OnEnable()
		{
			if (playOnEnable)
			{
				PlayAnimation();
			}

			UpdateAnimationType();
			OnInit();
		}


		private void OnDisable()
		{
			StopAnimation();
		}

		
		
		public event AnimationFinishEvent OnAnimationFinish;


		public void SetAnimationType(AnimationType animationType)
		{
			this.animationType = animationType;
		}


		protected abstract void OnInit();


		protected abstract void OnRelease();


		protected abstract void OnUpdateAnimationValue(float value);


		private void UpdateAnimationType()
		{
			switch (animationType)
			{
				case AnimationType.PingPong:
					curve.postWrapMode = WrapMode.PingPong;
					return;
				case AnimationType.Loop:
					curve.postWrapMode = WrapMode.Loop;
					return;
				case AnimationType.Once:
					curve.postWrapMode = WrapMode.Once;
					return;
				default:
					return;
			}
		}


		public void SetCurve(AnimationCurve curve)
		{
			this.curve = curve;
		}


		public void PlayAnimation()
		{
			if (!isPlaying)
			{
				if (enabled)
				{
					isPlaying = true;
					ResetAnimation();
					return;
				}

				enabled = true;
			}
		}


		public void StopAnimation()
		{
			if (isPlaying)
			{
				isPlaying = false;
				AnimationFinishEvent onAnimationFinish = OnAnimationFinish;
				if (onAnimationFinish != null)
				{
					onAnimationFinish();
				}

				OnRelease();
			}
		}


		private void ResetAnimation()
		{
			time = 0f;
		}
	}
}