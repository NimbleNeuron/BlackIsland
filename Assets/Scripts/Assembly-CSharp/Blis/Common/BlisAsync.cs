using System;

namespace Blis.Common
{
	public class BlisAsync
	{
		private readonly AsyncType asyncType;


		private Action failCallback;


		private Action<bool> finishedCallback;


		private Action successCallback;


		private readonly Action<Action<bool>>[] taskList;


		private BlisAsync(AsyncType asyncType, Action<Action<bool>>[] tasks)
		{
			this.asyncType = asyncType;
			taskList = tasks;
		}

		public static BlisAsync Parallel(params Action<Action<bool>>[] tasks)
		{
			return new BlisAsync(AsyncType.Parallel, tasks);
		}


		public void Execute(Action<bool> finishedCallback = null)
		{
			if (asyncType == AsyncType.Parallel)
			{
				this.finishedCallback = finishedCallback;
				ParallelProcess();
			}
		}


		public BlisAsync OnSuccess(Action callback)
		{
			successCallback = callback;
			return this;
		}


		public BlisAsync OnFail(Action callback)
		{
			failCallback = callback;
			return this;
		}


		private void ParallelProcess()
		{
			if (taskList == null || taskList.Length == 0)
			{
				return;
			}

			int workCount = taskList.Length;
			bool isSuccess = true;
			Action<bool> obj = delegate(bool result)
			{
				isSuccess = isSuccess && result;
				int num = workCount - 1;
				workCount = num;
				if (num != 0)
				{
					if (workCount < 0)
					{
						Log.E("[BlisAsync] 누군가 한 번 불러야하는 콜백을 여러번 호출하고 있음.");
					}

					return;
				}

				if (isSuccess)
				{
					Action action = successCallback;
					if (action != null)
					{
						action();
					}
				}
				else
				{
					Action action2 = failCallback;
					if (action2 != null)
					{
						action2();
					}
				}

				Action<bool> action3 = finishedCallback;
				if (action3 == null)
				{
					return;
				}

				action3(isSuccess);
			};
			Action<Action<bool>>[] array = taskList;
			for (int i = 0; i < array.Length; i++)
			{
				array[i](obj);
			}
		}


		private enum AsyncType
		{
			Parallel
		}
	}
}