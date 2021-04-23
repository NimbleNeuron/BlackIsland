using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollContentWrapper : UIBehaviour
	{
		public delegate void ContentAppearEvent(GameObject gameObject, object data);


		public delegate void ContentDisappearEvent(GameObject gameObject);


		public RectOffset padding;


		public Vector2 cellSize;


		public Vector2 spacing;


		public int groupSize = 1;


		private readonly List<GameObject> childrenList = new List<GameObject>();


		private ArrayList dataList;


		private float? prePos;


		private ScrollRect scrollRect;


		protected override void Awake()
		{
			base.Awake();
			scrollRect = GetComponent<ScrollRect>();
			for (int i = 0; i < scrollRect.content.childCount; i++)
			{
				RectTransform rectTransform = scrollRect.content.GetChild(i) as RectTransform;
				rectTransform.sizeDelta = cellSize;
				childrenList.Add(rectTransform.gameObject);
			}
		}


		protected override void Start()
		{
			base.Start();
			for (int i = 0; i < scrollRect.content.childCount; i++)
			{
				OnDisappear(scrollRect.content.GetChild(i).gameObject);
			}
		}


		protected override void OnEnable()
		{
			base.OnEnable();
			prePos = null;
			if (!CheckValidation())
			{
				enabled = false;
				return;
			}

			scrollRect.onValueChanged.AddListener(OnValueChange);
		}


		protected override void OnDisable()
		{
			base.OnDisable();
			scrollRect.onValueChanged.RemoveListener(OnValueChange);
		}

		
		
		public event ContentAppearEvent OnAppear = delegate { };


		
		
		public event ContentDisappearEvent OnDisappear = delegate { };


		private bool CheckValidation()
		{
			if (scrollRect.content == null)
			{
				Log.E("ScrollRect don't have content");
				return false;
			}

			if (scrollRect.horizontal ^ !scrollRect.vertical)
			{
				Log.E("Only horizontal or vertical ScrollRect can have ScrollContentWrapper.");
				return false;
			}

			return childrenList.Count > 0;
		}


		public void SetDataList(IList list)
		{
			dataList = new ArrayList(list);
			int num = Mathf.CeilToInt(list.Count / (float) groupSize);
			if (scrollRect.vertical)
			{
				scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x,
					num * cellSize.y + padding.vertical + Mathf.Max(0, num - 1) * spacing.y -
					scrollRect.viewport.rect.size.y);
			}
			else
			{
				scrollRect.content.sizeDelta =
					new Vector2(
						num * cellSize.x + padding.horizontal + Mathf.Max(0, num - 1) * spacing.x -
						scrollRect.viewport.rect.size.x, scrollRect.content.sizeDelta.y);
			}

			Refresh();
		}


		private void OnValueChange(Vector2 vector2)
		{
			if (!CheckValidation())
			{
				enabled = false;
				return;
			}

			UpdateChildPos();
		}


		public void Refresh()
		{
			prePos = null;
			scrollRect.content.anchoredPosition = Vector2.zero;
			UpdateChildPos();
		}


		private int GetScrollAxis()
		{
			if (!scrollRect.vertical)
			{
				return 1;
			}

			return 0;
		}


		private void UpdateChildPos()
		{
			float num = scrollRect.vertical
				? scrollRect.viewport.rect.height - padding.vertical
				: scrollRect.viewport.rect.width - padding.horizontal;
			float num2 = scrollRect.vertical
				? scrollRect.content.anchoredPosition.y
				: scrollRect.content.anchoredPosition.x;
			float num3 = scrollRect.vertical ? cellSize.y + spacing.y : cellSize.x + spacing.x;
			int num4 = Mathf.Min(childrenList.Count, (Mathf.CeilToInt(num / num3) + 1) * groupSize);
			if (prePos != null)
			{
				int num5 = Mathf.FloorToInt(num2 / num3);
				int num6 = Mathf.FloorToInt(prePos.Value / num3);
				int num7 = Mathf.Abs(num5 - num6) * groupSize;
				if (num5 > num6)
				{
					for (int i = 0; i < num7; i++)
					{
						UpdateItem(num6 * groupSize + num4 + i);
					}
				}
				else if (num5 < num6)
				{
					for (int j = 1; j <= num7; j++)
					{
						UpdateItem(num6 * groupSize - j);
					}
				}
			}
			else
			{
				for (int k = 0; k < num4; k++)
				{
					UpdateItem(k);
				}
			}

			prePos = num2;
		}


		private void UpdateItem(int index)
		{
			int num = index % childrenList.Count;
			if (num < 0)
			{
				num += childrenList.Count;
			}

			GameObject gameObject = childrenList[num];
			if (gameObject.activeSelf)
			{
				OnDisappear(gameObject);
			}

			if (dataList != null && 0 <= index && index < dataList.Count)
			{
				gameObject.SetActive(true);
				if (groupSize <= 0)
				{
					groupSize = 1;
				}

				int num2 = index / groupSize;
				if (scrollRect.vertical)
				{
					Vector2 v = new Vector2(index % groupSize, -(float) num2) * cellSize;
					v.x += index % groupSize * spacing.x;
					v.x += padding.left;
					v.y -= padding.top;
					v.y -= spacing.y * num2;
					gameObject.transform.localPosition = v;
				}
				else
				{
					Vector2 v2 = new Vector2(num2, index % groupSize) * cellSize;
					v2.y -= index % groupSize * spacing.y;
					v2.y -= padding.top;
					v2.x += padding.left;
					v2.x += spacing.x * num2;
					gameObject.transform.localPosition = v2;
				}

				OnAppear(gameObject, dataList[index]);
				return;
			}

			OnDisappear(gameObject);
		}
	}
}