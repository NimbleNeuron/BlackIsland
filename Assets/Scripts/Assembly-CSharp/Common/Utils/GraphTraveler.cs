using System.Collections.Generic;

namespace Common.Utils
{
	
	public class GraphTraveler<T>
	{
		
		
		
		private event GraphTraveler<T>.GetNextNodeEvent getNext;

		
		public GraphTraveler(GraphTraveler<T>.GetNextNodeEvent getNextNode)
		{
			this.getNext = getNextNode;
		}

		
		public void TravelBFS(T head, GraphTraveler<T>.VisitEvent onVisit)
		{
			Queue<T> queue = new Queue<T>();
			queue.Enqueue(head);
			HashSet<T> hashSet = new HashSet<T>();
			while (queue.Count > 0)
			{
				T t = queue.Dequeue();
				if (!hashSet.Contains(t))
				{
					hashSet.Add(t);
					if (onVisit != null)
					{
						onVisit(t);
					}
					List<T> list = this.getNext(t);
					for (int i = 0; i < list.Count; i++)
					{
						queue.Enqueue(list[i]);
					}
				}
			}
		}

		
		public delegate List<T> GetNextNodeEvent(T node);

		
		public delegate void VisitEvent(T node);
	}
}
