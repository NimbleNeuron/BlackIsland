using System;
using System.Runtime.CompilerServices;

namespace Blis.Common.Utils
{
	public class MinimumQueue<T>
	{
		private const int MinimumGrow = 4;


		private const int GrowFactor = 200;


		private T[] array;


		private int head;


		private int tail;

		public MinimumQueue(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}

			array = new T[capacity];
			head = tail = Count = 0;
		}


		public int Count {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
			private set;
		}


		public T Peek()
		{
			if (Count == 0)
			{
				ThrowForEmptyQueue();
			}

			return array[head];
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Enqueue(T item)
		{
			if (Count == array.Length)
			{
				Grow();
			}

			array[tail] = item;
			MoveNext(ref tail);
			Count++;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Dequeue()
		{
			if (Count == 0)
			{
				ThrowForEmptyQueue();
			}

			int num = head;
			T[] array = this.array;
			T result = array[num];
			array[num] = default;
			MoveNext(ref head);
			Count--;
			return result;
		}


		private void Grow()
		{
			int num = (int) (array.Length * 200L / 100L);
			if (num < array.Length + 4)
			{
				num = array.Length + 4;
			}

			SetCapacity(num);
		}


		private void SetCapacity(int capacity)
		{
			T[] destinationArray = new T[capacity];
			if (Count > 0)
			{
				if (head < tail)
				{
					Array.Copy(array, head, destinationArray, 0, Count);
				}
				else
				{
					Array.Copy(array, head, destinationArray, 0, array.Length - head);
					Array.Copy(array, 0, destinationArray, array.Length - head, tail);
				}
			}

			array = destinationArray;
			head = 0;
			tail = Count == capacity ? 0 : Count;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void MoveNext(ref int index)
		{
			int num = index + 1;
			if (num == array.Length)
			{
				num = 0;
			}

			index = num;
		}


		private void ThrowForEmptyQueue()
		{
			throw new InvalidOperationException("EmptyQueue");
		}
	}
}