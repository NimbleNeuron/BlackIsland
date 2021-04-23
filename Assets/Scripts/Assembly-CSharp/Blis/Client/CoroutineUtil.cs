using System;
using System.Collections;
using UnityEngine;

namespace Blis.Client
{
	public static class CoroutineUtil
	{
		public static IEnumerator DelayedAction(float delay, Action action)
		{
			yield return new WaitForSeconds(delay);
			action();
		}


		public static IEnumerator FrameDelayedAction(int frameCount, Action action)
		{
			while (0 < frameCount)
			{
				int num = frameCount;
				frameCount = num - 1;
				yield return null;
			}

			action();
		}


		public static Coroutine StartThrowingCoroutine(this MonoBehaviour monoBehaviour, IEnumerator enumerator,
			Action<Exception> done)
		{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			try
			{
#endif
				return monoBehaviour.StartCoroutine(enumerator);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			}
			catch (Exception e)
			{
				done?.Invoke(e);
			}
#endif
			return null;
		}


		private static IEnumerator RunThrowingIterator(IEnumerator enumerator, Action<Exception> done)
		{
			for (;;)
			{
				object obj;
				try
				{
					if (!enumerator.MoveNext())
					{
						yield break;
					}

					obj = enumerator.Current;
				}
				catch (Exception obj2)
				{
					if (done != null)
					{
						done(obj2);
					}

					yield break;
				}

				yield return obj;
			}
		}


		public static IEnumerator MoveTo(Transform target, Vector3 src, Vector3 dest, float duration, Space space,
			Action callback)
		{
			float startTime = Time.time;
			while (startTime > Time.time - duration)
			{
				float t = (Time.time - startTime) / duration;
				if (space == Space.World)
				{
					target.position = Vector3.Lerp(src, dest, t);
				}
				else
				{
					target.localPosition = Vector3.Lerp(src, dest, t);
				}

				yield return null;
			}

			if (callback != null)
			{
				callback();
			}
		}


		public static IEnumerator Lerp(float from, float to, float duration, Action<float> callback)
		{
			if (callback == null)
			{
				yield break;
			}

			float startTime = Time.time;
			while (startTime > Time.time - duration)
			{
				float t = (Time.time - startTime) / duration;
				callback(Mathf.Lerp(from, to, t));
				yield return null;
			}
		}
	}
}