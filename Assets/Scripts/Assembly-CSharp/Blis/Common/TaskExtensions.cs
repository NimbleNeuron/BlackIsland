using System.Collections;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Blis.Common
{
	public static class TaskExtensions
	{
		public static IEnumerator AsCoroutine(this Task task)
		{
			while (!task.IsCompleted)
			{
				yield return null;
			}

			if (!task.IsFaulted)
			{
				yield break;
			}

			ExceptionDispatchInfo.Capture(task.Exception).Throw();
		}


		public static void Forget(this Task task)
		{
			task.ContinueWith(
				delegate(Task t) { Log.E("Exception occurred while Forget Async Task", t.Exception.Message); },
				TaskContinuationOptions.OnlyOnFaulted);
		}
	}
}