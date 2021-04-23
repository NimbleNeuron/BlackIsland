using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class NoiseEffect : MonoBehaviour
	{
		public float duration;


		public float warnTime;


		[SerializeField] private AnimationCurve curve = default;


		private readonly Vector2 fromSize = new Vector2(10f, 10f);


		private readonly Vector2 toSize = new Vector2(100f, 100f);


		private Coroutine animationRoutine;


		private Action<bool> callback;


		private Color defaultNoiseColor;


		private Color defaultRippleColor;


		private Image noise;


		private Image ripple;

		private void Awake()
		{
			ripple = GameUtil.Bind<Image>(gameObject, "Ripple");
			noise = GetComponent<Image>();
			defaultRippleColor = new Color(0.937f, 0.251f, 0.251f, 1f);
			defaultNoiseColor = new Color(0.937f, 0.251f, 0.251f, 1f);
		}


		private void OnEnable()
		{
			ripple.rectTransform.sizeDelta = new Vector2(10f, 10f);
		}


		public void PlayNoise(Action<bool> callback)
		{
			if (animationRoutine != null)
			{
				StopCoroutine(animationRoutine);
				animationRoutine = null;
				Action<bool> action = this.callback;
				if (action != null)
				{
					action(false);
				}
			}

			this.callback = callback;
			animationRoutine = this.StartThrowingCoroutine(Play(duration, warnTime),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][PlayNoise] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private IEnumerator Play(float duration, float warnTime)
		{
			Color color = ripple.color;
			float time = 0f;
			for (;;)
			{
				time += Time.deltaTime;
				if (time > duration)
				{
					break;
				}

				float num = curve.Evaluate(time / duration);
				ripple.rectTransform.sizeDelta = Vector2.Lerp(fromSize, toSize, num);
				color.a = 1f - num;
				ripple.color = color;
				yield return null;
			}

			yield return new WaitForSeconds(warnTime);
			Action<bool> action = callback;
			if (action != null)
			{
				action(true);
			}

			animationRoutine = null;
		}


		public void SetDefaultColor()
		{
			ripple.color = defaultRippleColor;
			noise.color = defaultNoiseColor;
		}


		public void SetCreatorColor(int creatorObjectId)
		{
			SetDefaultColor();
			if (MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				LocalPlayerCharacter localPlayerCharacter = null;
				if (MonoBehaviourInstance<ClientService>.inst.World.TryFind<LocalPlayerCharacter>(creatorObjectId,
					ref localPlayerCharacter) && MonoBehaviourInstance<ClientService>.inst.IsAlly(localPlayerCharacter))
				{
					ripple.color = GameConstants.TeamMode.GetTeamColor(localPlayerCharacter.TeamSlot);
					noise.color = GameConstants.TeamMode.GetTeamColor(localPlayerCharacter.TeamSlot);
				}
			}
		}


		public void SetColor(Color rippleColor, Color noiseColor)
		{
			ripple.color = rippleColor;
			noise.color = noiseColor;
		}


		public void SetActiveNoise(bool active)
		{
			noise.enabled = active;
		}
	}
}