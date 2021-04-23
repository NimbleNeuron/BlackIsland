using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UiEffect
{
	public class UiEffectObjectPool<T> where T : new()
	{
		private readonly UnityAction<T> m_ActionOnGet;


		private readonly UnityAction<T> m_ActionOnRelease;


		private readonly Stack<T> m_Stack = new Stack<T>();


		public UiEffectObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
		{
			m_ActionOnGet = actionOnGet;
			m_ActionOnRelease = actionOnRelease;
		}


		
		public int countAll { get; private set; }


		public int countActive => countAll - countInactive;


		public int countInactive => m_Stack.Count;


		public T Get()
		{
			T t;
			if (m_Stack.Count == 0)
			{
				t = Activator.CreateInstance<T>();
				int countAll = this.countAll;
				this.countAll = countAll + 1;
			}
			else
			{
				t = m_Stack.Pop();
			}

			if (m_ActionOnGet != null)
			{
				m_ActionOnGet(t);
			}

			return t;
		}


		public void Release(T element)
		{
			// co: replace equality comparing method
			if (m_Stack.Count > 0 && Equals(m_Stack.Peek(), element))
			{
				Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
			}

			if (m_ActionOnRelease != null)
			{
				m_ActionOnRelease(element);
			}

			m_Stack.Push(element);
		}
	}
}