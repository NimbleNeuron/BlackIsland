using System;
using System.Collections;
using UnityEngine;

namespace Blis.Server
{
	
	public static class CoroutineUtil
	{
		
		public static IEnumerator DelayedAction(float delay, Action action)
		{
			yield return new WaitForFrameUpdate().Seconds(delay);
			action();
		}

		
		public static IEnumerator DelayedAction(int frame, Action action)
		{
			yield return new WaitForFrameUpdate().Frame(frame);
			action();
		}

		
		public static Coroutine StartThrowingCoroutine(this MonoBehaviour monoBehaviour, IEnumerator enumerator, Action<Exception> done)
		{
			return monoBehaviour.StartCoroutine(CoroutineUtil.RunThrowingIterator(enumerator, done));
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
	}
}
