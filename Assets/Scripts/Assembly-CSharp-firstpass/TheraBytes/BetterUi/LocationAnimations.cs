using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace TheraBytes.BetterUi
{
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("Better UI/Animation/Location Animations", 30)]
	public class LocationAnimations : MonoBehaviour
	{
		[SerializeField] private List<LocationData> locations = new List<LocationData>();


		[SerializeField] private List<Animation> animations = new List<Animation>();


		[SerializeField] private string startLocation = default;


		[SerializeField] private string startUpAnimation = default;


		[SerializeField] private LocationAnimationEvent actionOnInit = default;


		private AnimationState runningAnimation;


		public RectTransform RectTransform => transform as RectTransform;


		public List<LocationData> Locations => locations;


		public List<Animation> Animations => animations;


		
		public string StartUpAnimation {
			get => startUpAnimation;
			set => startUpAnimation = value;
		}


		public bool IsAnimating => runningAnimation != null;


		public AnimationState RunningAnimation => runningAnimation;


		private void Start()
		{
			SetToLocation(startLocation);
			actionOnInit.Invoke();
			StartAnimation(startUpAnimation);
		}


		private void Update()
		{
			UpdateCurrentAnimation(Time.deltaTime);
		}


		public void StopCurrentAnimation()
		{
			runningAnimation = null;
		}


		public void StartAnimation(string name)
		{
			StartAnimation(GetAnimation(name), null, null);
		}


		public void StartAnimation(string name, float timeScale)
		{
			StartAnimation(GetAnimation(name), timeScale, null);
		}


		public void StartAnimation(string name, LocationAnimationEvent onFinish)
		{
			StartAnimation(GetAnimation(name), null, onFinish);
		}


		public void StartAnimation(string name, float timeScale, LocationAnimationEvent onFinish)
		{
			StartAnimation(GetAnimation(name), timeScale, onFinish);
		}


		public void StartAnimation(Animation ani, float? timeScale, LocationAnimationEvent onFinish)
		{
			if (ani == null || ani.To == null || runningAnimation != null && ani == runningAnimation.Animation)
			{
				return;
			}

			if (runningAnimation != null)
			{
				StopCurrentAnimation();
			}

			if (ani.Curve == null || ani.Curve.keys.Length <= 1)
			{
				SetToLocation(ani.To);
				return;
			}

			float num = timeScale ?? ani.TimeScale;
			bool loop =
				num > 0f && (ani.Curve.postWrapMode == WrapMode.Loop || ani.Curve.postWrapMode == WrapMode.PingPong) ||
				num < 0f && (ani.Curve.preWrapMode == WrapMode.Loop || ani.Curve.preWrapMode == WrapMode.PingPong);
			runningAnimation = new AnimationState
			{
				Animation = ani,
				From = GetLocationTransformFallbackCurrent(ani.From),
				To = GetLocationTransformFallbackCurrent(ani.To),
				ActionAfterFinish = onFinish ?? ani.ActionAfterFinish,
				Duration = ani.Curve.keys[ani.Curve.keys.Length - 1].time,
				Loop = loop,
				TimeScale = num,
				Time = 0f
			};
			ani.ActionBeforeStart.Invoke();
		}


		public void UpdateCurrentAnimation(float deltaTime)
		{
			if (runningAnimation == null || runningAnimation.Animation == null ||
			    runningAnimation.Animation.Curve == null || runningAnimation.Animation.Curve.length == 0)
			{
				return;
			}

			bool flag = !runningAnimation.Loop && runningAnimation.Time >= runningAnimation.Duration;
			if (flag)
			{
				runningAnimation.Time = runningAnimation.Duration;
			}

			float amount = runningAnimation.Animation.Curve.Evaluate(runningAnimation.Time);
			RectTransformData
				.LerpUnclamped(runningAnimation.From, runningAnimation.To, amount,
					runningAnimation.Animation.AnimateWithEulerRotation).PushToTransform(RectTransform);
			runningAnimation.Time += deltaTime * runningAnimation.TimeScale;
			if (flag)
			{
				runningAnimation.ActionAfterFinish.Invoke();
				runningAnimation = null;
			}
		}


		public void SetToLocation(string name)
		{
			LocationData location = GetLocation(name);
			if (location == null)
			{
				return;
			}

			location.CurrentTransformData.PushToTransform(RectTransform);
		}


		public LocationData GetLocation(string name)
		{
			return locations.FirstOrDefault(o => o.Name == name);
		}


		private RectTransformData GetLocationTransformFallbackCurrent(string name)
		{
			LocationData locationData = locations.FirstOrDefault(o => o.Name == name);
			if (locationData == null)
			{
				return new RectTransformData(RectTransform)
				{
					SaveRotationAsEuler = true
				};
			}

			RectTransformData currentTransformData = locationData.CurrentTransformData;
			currentTransformData.SaveRotationAsEuler = true;
			return currentTransformData;
		}


		public Animation GetAnimation(string name)
		{
			return animations.FirstOrDefault(o => o.Name == name);
		}


		[Serializable]
		public class LocationAnimationEvent : UnityEvent
		{
			public LocationAnimationEvent() { }


			public LocationAnimationEvent(params UnityAction[] actions)
			{
				foreach (UnityAction call in actions)
				{
					AddListener(call);
				}
			}
		}


		[Serializable]
		public class RectTransformDataConfigCollection : SizeConfigCollection<RectTransformData> { }


		[Serializable]
		public class LocationData
		{
			[SerializeField] private string name;


			[SerializeField] private RectTransformData transformFallback = new RectTransformData();


			[SerializeField]
			private RectTransformDataConfigCollection transformConfigs = new RectTransformDataConfigCollection();


			
			public string Name {
				get => name;
				internal set => name = value;
			}


			public RectTransformData CurrentTransformData => transformConfigs.GetCurrentItem(transformFallback);
		}


		[Serializable]
		public class Animation
		{
			[SerializeField] private string name;


			[SerializeField] private string from;


			[SerializeField] private string to;


			[SerializeField] private AnimationCurve curve;


			[SerializeField] private LocationAnimationEvent actionBeforeStart = new LocationAnimationEvent();


			[SerializeField] private LocationAnimationEvent actionAfterFinish = new LocationAnimationEvent();


			[SerializeField] private bool animateWithEulerRotation = true;


			[SerializeField] private float timeScale = 1f;


			
			public string Name {
				get => name;
				internal set => name = value;
			}


			
			public string From {
				get => from;
				set => from = value;
			}


			
			public string To {
				get => to;
				set => to = value;
			}


			
			public AnimationCurve Curve {
				get => curve;
				internal set => curve = value;
			}


			
			public bool AnimateWithEulerRotation {
				get => animateWithEulerRotation;
				set => animateWithEulerRotation = value;
			}


			
			public float TimeScale {
				get => timeScale;
				set => timeScale = value;
			}


			public LocationAnimationEvent ActionBeforeStart => actionBeforeStart;


			public LocationAnimationEvent ActionAfterFinish => actionAfterFinish;
		}


		[Serializable]
		public class AnimationState
		{
			
			public Animation Animation { get; internal set; }


			
			public RectTransformData From { get; internal set; }


			
			public RectTransformData To { get; internal set; }


			
			public float Time { get; set; }


			
			public float Duration { get; set; }


			
			public bool Loop { get; internal set; }


			
			public float TimeScale { get; set; }


			
			public LocationAnimationEvent ActionAfterFinish { get; internal set; }
		}
	}
}