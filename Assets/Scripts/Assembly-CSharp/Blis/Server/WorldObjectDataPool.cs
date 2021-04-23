using System;
using System.Collections.Generic;

namespace Blis.Server
{
	
	public abstract class WorldObjectDataPool<T> where T : class, new()
	{
		
		private Queue<T> ObjectDataPool = new Queue<T>();

		
		public void AllocPool(int count)
		{
			for (int i = 0; i < count; i++)
			{
				ObjectDataPool.Enqueue(Activator.CreateInstance<T>());
			}
		}

		
		public virtual T Pop()
		{
			if (ObjectDataPool.Count <= 0)
			{
				return Activator.CreateInstance<T>();
			}

			return ObjectDataPool.Dequeue();
		}

		
		public virtual void Push(T objData)
		{
			ObjectDataPool.Enqueue(objData);
		}

		
		public void Clear()
		{
			ObjectDataPool.Clear();
			ObjectDataPool = null;
		}
	}
}