using UnityEngine;

namespace SuperScrollView
{
	public class LoopGridViewItem : MonoBehaviour
	{
		private RectTransform mCachedRectTransform;


		
		public object UserObjectData { get; set; }


		
		public int UserIntData1 { get; set; }


		
		public int UserIntData2 { get; set; }


		
		public string UserStringData1 { get; set; }


		
		public string UserStringData2 { get; set; }


		
		public int ItemCreatedCheckFrameCount { get; set; }


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


		
		public int Row { get; set; } = -1;


		
		public int Column { get; set; } = -1;


		
		public int ItemIndex { get; set; } = -1;


		
		public int ItemId { get; set; } = -1;


		
		public bool IsInitHandlerCalled { get; set; }


		
		public LoopGridView ParentGridView { get; set; }


		
		public LoopGridViewItem PrevItem { get; set; }


		
		public LoopGridViewItem NextItem { get; set; }
	}
}