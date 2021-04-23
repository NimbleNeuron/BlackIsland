using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{
	internal class MenuSceneScript : MonoBehaviour
	{
		public Transform mButtonPanelTf = default;


		private readonly SceneNameInfo[] mSceneNameArray =
		{
			new SceneNameInfo("Staggered GridView1", "StaggeredGridView_TopToBottom"),
			new SceneNameInfo("Staggered GridView2", "StaggeredGridView_LeftToRight"),
			new SceneNameInfo("Chat Message List", "ChatMsgListViewDemo"),
			new SceneNameInfo("Horizontal Gallery", "HorizontalGalleryDemo"),
			new SceneNameInfo("Vertical Gallery", "VerticalGalleryDemo"),
			new SceneNameInfo("GridView", "GridView_TopLeftToBottomRight"),
			new SceneNameInfo("PageView", "PageViewDemo"),
			new SceneNameInfo("TreeView", "TreeViewDemo"),
			new SceneNameInfo("Spin Date Picker", "SpinDatePickerDemo"),
			new SceneNameInfo("Pull Down To Refresh", "PullAndRefreshDemo"),
			new SceneNameInfo("TreeView\nWith Sticky Head", "TreeViewWithStickyHeadDemo"),
			new SceneNameInfo("Change Item Height", "ChangeItemHeightDemo"),
			new SceneNameInfo("Pull Up To Load More", "PullAndLoadMoreDemo"),
			new SceneNameInfo("Click Load More", "ClickAndLoadMoreDemo"),
			new SceneNameInfo("Select And Delete", "DeleteItemDemo"),
			new SceneNameInfo("GridView Select Delete ", "GridViewDeleteItemDemo"),
			new SceneNameInfo("Responsive GridView", "ResponsiveGridViewDemo"),
			new SceneNameInfo("TreeView\nWith Children Indent", "TreeViewWithChildrenIndentDemo")
		};


		private void Start()
		{
			CreateFpsDisplyObj();
			int childCount = mButtonPanelTf.childCount;
			int num = mSceneNameArray.Length;
			for (int i = 0; i < childCount; i++)
			{
				if (i >= num)
				{
					mButtonPanelTf.GetChild(i).gameObject.SetActive(false);
				}
				else
				{
					mButtonPanelTf.GetChild(i).gameObject.SetActive(true);
					SceneNameInfo info = mSceneNameArray[i];
					Button component = mButtonPanelTf.GetChild(i).GetComponent<Button>();
					component.onClick.AddListener(delegate { SceneManager.LoadScene(info.mSceneName); });
					component.transform.Find("Text").GetComponent<Text>().text = info.mName;
				}
			}
		}


		private void CreateFpsDisplyObj()
		{
			if (FindObjectOfType<FPSDisplay>() != null)
			{
				return;
			}

			GameObject gameObject = new GameObject();
			gameObject.name = "FPSDisplay";
			gameObject.AddComponent<FPSDisplay>();
			DontDestroyOnLoad(gameObject);
		}
	}
}