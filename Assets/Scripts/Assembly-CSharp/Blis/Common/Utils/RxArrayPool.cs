using System;
using System.Threading;

namespace Blis.Common.Utils
{
	public sealed class RxArrayPool<T>
	{
		private const int DefaultMaxNumberOfArraysPerBucket = 50;


		private static readonly T[] EmptyArray = new T[0];


		public static readonly RxArrayPool<T> Shared = new RxArrayPool<T>();


		private readonly MinimumQueue<T[]>[] buckets;


		private readonly SpinLock[] locks;

		private RxArrayPool()
		{
			buckets = new MinimumQueue<T[]>[18];
			locks = new SpinLock[18];
			for (int i = 0; i < buckets.Length; i++)
			{
				buckets[i] = new MinimumQueue<T[]>(4);
				locks[i] = new SpinLock(false);
			}
		}


		public T[] Rent(int minimumLength)
		{
			if (minimumLength < 0)
			{
				throw new ArgumentOutOfRangeException("minimumLength");
			}

			if (minimumLength == 0)
			{
				return EmptyArray;
			}

			int num = CalculateSize(minimumLength);
			int queueIndex = GetQueueIndex(num);
			if (queueIndex != -1)
			{
				MinimumQueue<T[]> minimumQueue = buckets[queueIndex];
				bool flag = false;
				try
				{
					locks[queueIndex].Enter(ref flag);
					if (minimumQueue.Count != 0)
					{
						return minimumQueue.Dequeue();
					}
				}
				finally
				{
					if (flag)
					{
						locks[queueIndex].Exit(false);
					}
				}
			}

			return new T[num];
		}


		public void Return(T[] array, bool clearArray = false)
		{
			if (array == null || array.Length == 0)
			{
				return;
			}

			int queueIndex = GetQueueIndex(array.Length);
			if (queueIndex != -1)
			{
				if (clearArray)
				{
					Array.Clear(array, 0, array.Length);
				}

				MinimumQueue<T[]> minimumQueue = buckets[queueIndex];
				bool flag = false;
				try
				{
					locks[queueIndex].Enter(ref flag);
					if (minimumQueue.Count <= 50)
					{
						minimumQueue.Enqueue(array);
					}
				}
				finally
				{
					if (flag)
					{
						locks[queueIndex].Exit(false);
					}
				}
			}
		}


		private static int CalculateSize(int size)
		{
			size--;
			size |= size >> 1;
			size |= size >> 2;
			size |= size >> 4;
			size |= size >> 8;
			size |= size >> 16;
			size++;
			if (size < 8)
			{
				size = 8;
			}

			return size;
		}


		private static int GetQueueIndex(int size)
		{
			if (size <= 2048)
			{
				if (size <= 64)
				{
					if (size <= 16)
					{
						if (size == 8)
						{
							return 0;
						}

						if (size == 16)
						{
							return 1;
						}
					}
					else
					{
						if (size == 32)
						{
							return 2;
						}

						if (size == 64)
						{
							return 3;
						}
					}
				}
				else if (size <= 256)
				{
					if (size == 128)
					{
						return 4;
					}

					if (size == 256)
					{
						return 5;
					}
				}
				else
				{
					if (size == 512)
					{
						return 6;
					}

					if (size == 1024)
					{
						return 7;
					}

					if (size == 2048)
					{
						return 8;
					}
				}
			}
			else if (size <= 32768)
			{
				if (size <= 8192)
				{
					if (size == 4096)
					{
						return 9;
					}

					if (size == 8192)
					{
						return 10;
					}
				}
				else
				{
					if (size == 16384)
					{
						return 11;
					}

					if (size == 32768)
					{
						return 12;
					}
				}
			}
			else if (size <= 131072)
			{
				if (size == 65536)
				{
					return 13;
				}

				if (size == 131072)
				{
					return 14;
				}
			}
			else
			{
				if (size == 262144)
				{
					return 15;
				}

				if (size == 524288)
				{
					return 16;
				}

				if (size == 1048576)
				{
					return 17;
				}
			}

			return -1;
		}
	}
}