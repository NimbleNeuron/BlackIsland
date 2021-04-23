using System.Collections.Generic;
using UnityEngine;

namespace Blis.Client
{
	public class SlotTable<T, U> : BaseUI where T : class where U : Slot
	{
		[SerializeField] private GameObject slotPrefab = default;


		private readonly Dictionary<T, U> activeSlotMap = new Dictionary<T, U>();


		private readonly List<U> slots = new List<U>();


		private ISlotEventListener currentEventListener;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GetComponentsInChildren<U>(true, slots);
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			slots.ForEach(delegate(U x) { x.ResetSlot(); });
		}


		private U GetIdleSlot()
		{
			for (int i = 0; i < slots.Count; i++)
			{
				if (!activeSlotMap.ContainsValue(slots[i]))
				{
					return slots[i];
				}
			}

			if (slotPrefab != null)
			{
				return CreateSlot(slotPrefab);
			}

			return default;
		}


		public U CreateSlot(T t)
		{
			if (activeSlotMap.ContainsKey(t))
			{
				return activeSlotMap[t];
			}

			U idleSlot = GetIdleSlot();
			if (idleSlot != null)
			{
				idleSlot.ResetSlot();
				activeSlotMap.Add(t, idleSlot);
				return idleSlot;
			}

			return default;
		}


		public U FindSlot(T t)
		{
			if (activeSlotMap.ContainsKey(t))
			{
				return activeSlotMap[t];
			}

			return default;
		}


		public int FindSlotIndex(T t)
		{
			int num = 0;
			foreach (KeyValuePair<T, U> keyValuePair in activeSlotMap)
			{
				if (t == keyValuePair.Key)
				{
					return num;
				}

				num++;
			}

			return 0;
		}


		public void RemoveSlot(T t)
		{
			U u = FindSlot(t);
			if (u == null)
			{
				return;
			}

			u.ResetSlot();
			activeSlotMap.Remove(t);
		}


		public void SetSlotEventListener(ISlotEventListener eventListener)
		{
			currentEventListener = eventListener;
			slots.ForEach(delegate(U x) { x.SetEventListener(eventListener); });
		}


		private U CreateSlot(GameObject prefab)
		{
			U component = Instantiate<GameObject>(prefab, transform).GetComponent<U>();
			component.SetEventListener(currentEventListener);
			slots.Add(component);
			return component;
		}


		public int GetCount()
		{
			return activeSlotMap.Count;
		}


		public Dictionary<T, U>.KeyCollection GetKeys()
		{
			return activeSlotMap.Keys;
		}


		public void Clear()
		{
			foreach (KeyValuePair<T, U> keyValuePair in activeSlotMap)
			{
				keyValuePair.Value.ResetSlot();
			}

			activeSlotMap.Clear();
		}
	}
}