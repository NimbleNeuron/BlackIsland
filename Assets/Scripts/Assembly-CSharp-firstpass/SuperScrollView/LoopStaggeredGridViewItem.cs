using UnityEngine;

namespace SuperScrollView
{
	public class LoopStaggeredGridViewItem : MonoBehaviour
	{
		private RectTransform mCachedRectTransform;


		
		public object UserObjectData { get; set; }


		
		public int UserIntData1 { get; set; }


		
		public int UserIntData2 { get; set; }


		
		public string UserStringData1 { get; set; }


		
		public string UserStringData2 { get; set; }


		
		public float DistanceWithViewPortSnapCenter { get; set; }


		
		public float StartPosOffset { get; set; }


		
		public int ItemCreatedCheckFrameCount { get; set; }


		
		public float Padding { get; set; }


		
		public float ExtraPadding { get; set; }


		public RectTransform CachedRectTransform {
			get
			{
				if (mCachedRectTransform == null)
				{
					mCachedRectTransform = gameObject.GetComponent<RectTransform>();
				}

				return mCachedRectTransform;
			}
		}


		
		public string ItemPrefabName { get; set; }


		
		public int ItemIndexInGroup { get; set; } = -1;


		
		public int ItemIndex { get; set; } = -1;


		
		public int ItemId { get; set; } = -1;


		
		public bool IsInitHandlerCalled { get; set; }


		
		public LoopStaggeredGridView ParentListView { get; set; }


		public float TopY {
			get
			{
				ListItemArrangeType arrangeType = ParentListView.ArrangeType;
				if (arrangeType == ListItemArrangeType.TopToBottom)
				{
					return CachedRectTransform.anchoredPosition3D.y;
				}

				if (arrangeType == ListItemArrangeType.BottomToTop)
				{
					return CachedRectTransform.anchoredPosition3D.y + CachedRectTransform.rect.height;
				}

				return 0f;
			}
		}


		public float BottomY {
			get
			{
				ListItemArrangeType arrangeType = ParentListView.ArrangeType;
				if (arrangeType == ListItemArrangeType.TopToBottom)
				{
					return CachedRectTransform.anchoredPosition3D.y - CachedRectTransform.rect.height;
				}

				if (arrangeType == ListItemArrangeType.BottomToTop)
				{
					return CachedRectTransform.anchoredPosition3D.y;
				}

				return 0f;
			}
		}


		public float LeftX {
			get
			{
				ListItemArrangeType arrangeType = ParentListView.ArrangeType;
				if (arrangeType == ListItemArrangeType.LeftToRight)
				{
					return CachedRectTransform.anchoredPosition3D.x;
				}

				if (arrangeType == ListItemArrangeType.RightToLeft)
				{
					return CachedRectTransform.anchoredPosition3D.x - CachedRectTransform.rect.width;
				}

				return 0f;
			}
		}


		public float RightX {
			get
			{
				ListItemArrangeType arrangeType = ParentListView.ArrangeType;
				if (arrangeType == ListItemArrangeType.LeftToRight)
				{
					return CachedRectTransform.anchoredPosition3D.x + CachedRectTransform.rect.width;
				}

				if (arrangeType == ListItemArrangeType.RightToLeft)
				{
					return CachedRectTransform.anchoredPosition3D.x;
				}

				return 0f;
			}
		}


		public float ItemSize {
			get
			{
				if (ParentListView.IsVertList)
				{
					return CachedRectTransform.rect.height;
				}

				return CachedRectTransform.rect.width;
			}
		}


		public float ItemSizeWithPadding => ItemSize + Padding;
	}
}