﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SuperScrollView
{
	public class ClickEventListener : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler,
		IPointerUpHandler
	{
		private Action<GameObject> mClickedHandler;


		private Action<GameObject> mDoubleClickedHandler;


		private bool mIsPressed;


		private Action<GameObject> mOnPointerDownHandler;


		private Action<GameObject> mOnPointerUpHandler;


		public bool IsPressd => mIsPressed;


		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.clickCount == 2)
			{
				if (mDoubleClickedHandler != null)
				{
					mDoubleClickedHandler(gameObject);
				}
			}
			else if (mClickedHandler != null)
			{
				mClickedHandler(gameObject);
			}
		}


		public void OnPointerDown(PointerEventData eventData)
		{
			mIsPressed = true;
			if (mOnPointerDownHandler != null)
			{
				mOnPointerDownHandler(gameObject);
			}
		}


		public void OnPointerUp(PointerEventData eventData)
		{
			mIsPressed = false;
			if (mOnPointerUpHandler != null)
			{
				mOnPointerUpHandler(gameObject);
			}
		}


		public static ClickEventListener Get(GameObject obj)
		{
			ClickEventListener clickEventListener = obj.GetComponent<ClickEventListener>();
			if (clickEventListener == null)
			{
				clickEventListener = obj.AddComponent<ClickEventListener>();
			}

			return clickEventListener;
		}


		public void SetClickEventHandler(Action<GameObject> handler)
		{
			mClickedHandler = handler;
		}


		public void SetDoubleClickEventHandler(Action<GameObject> handler)
		{
			mDoubleClickedHandler = handler;
		}


		public void SetPointerDownHandler(Action<GameObject> handler)
		{
			mOnPointerDownHandler = handler;
		}


		public void SetPointerUpHandler(Action<GameObject> handler)
		{
			mOnPointerUpHandler = handler;
		}
	}
}