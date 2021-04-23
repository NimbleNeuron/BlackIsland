using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UISearchCategoryList : BaseUI, ILnEventHander
	{
		[SerializeField] private ToggleGroup toggleGroup = default;


		[SerializeField] private GameObject categoryPrefab = default;


		private readonly List<CategoryInfo> categoryInfos = new List<CategoryInfo>();


		public int Count => categoryInfos.Count;


		public void OnLnDataChange()
		{
			foreach (CategoryInfo categoryInfo in categoryInfos)
			{
				categoryInfo.SetCategoryText(categoryInfo.categoryName);
			}
		}


		private CategoryInfo CreateCategory(string categoryName, bool selectionEnable)
		{
			CategoryInfo categoryInfo =
				new CategoryInfo(Instantiate<GameObject>(categoryPrefab, toggleGroup.transform));
			categoryInfo.Toggle.group = toggleGroup;
			categoryInfo.SetCategoryText(categoryName);
			categoryInfo.Toggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				if (isOn)
				{
					OnCategorySelectionChange(categoryInfo);
				}
			});
			if (!selectionEnable)
			{
				categoryInfo.DisableSelection();
			}

			return categoryInfo;
		}


		public void AddCategoryHeader(string categoryName, Color32 textColor, Action<int> callback,
			bool selectionEnable = true)
		{
			CategoryInfo categoryInfo = CreateCategory(categoryName, selectionEnable);
			categoryInfo.gameObject.name = "CategoryHeader";
			categoryInfo.SetIndent(1);
			categoryInfo.SetCategoryTextColor(textColor);
			categoryInfo.SetCallback(callback);
			categoryInfos.Add(categoryInfo);
		}


		public void AddCategoryItem(string categoryName, Color32 textColor, Action<int> callback,
			bool selectionEnable = true)
		{
			CategoryInfo categoryInfo = CreateCategory(categoryName, selectionEnable);
			categoryInfo.gameObject.name = "CategoryItem";
			categoryInfo.SetIndent(2);
			categoryInfo.SetCategoryTextColor(textColor);
			categoryInfo.SetCallback(callback);
			categoryInfos.Add(categoryInfo);
		}


		public void ClearCategory()
		{
			foreach (CategoryInfo categoryInfo in categoryInfos)
			{
				Destroy(categoryInfo.gameObject);
			}

			categoryInfos.Clear();
		}


		private void OnCategorySelectionChange(CategoryInfo categoryInfo)
		{
			categoryInfo.InvokeCallback(categoryInfos.IndexOf(categoryInfo));
		}


		public void SelectCategory(int categoryIndex)
		{
			if (categoryIndex < categoryInfos.Count)
			{
				categoryInfos[categoryIndex].Toggle.isOn = true;
			}
		}


		public void ClearAllDotMark()
		{
			categoryInfos.ForEach(delegate(CategoryInfo x) { x.SetDot(false); });
		}


		public void SetDotMark(int categoryIndex, bool enable)
		{
			if (categoryIndex < categoryInfos.Count)
			{
				categoryInfos[categoryIndex].SetDot(enable);
			}
		}


		public bool GetDotMark(int categoryIndex)
		{
			return categoryIndex < categoryInfos.Count && categoryInfos[categoryIndex].GetDot();
		}


		private class CategoryInfo
		{
			private readonly Transform dot;
			public readonly GameObject gameObject;


			private readonly Text name;


			private readonly Text pinedNmae;


			private readonly Toggle toggle;


			private readonly ToggleEvent toggleEvent;


			private Action<int> callback;


			public string categoryName;


			public CategoryInfo(GameObject gameObject)
			{
				this.gameObject = gameObject;
				GameUtil.Bind<Toggle>(gameObject, ref toggle);
				GameUtil.Bind<ToggleEvent>(gameObject, ref toggleEvent);
				dot = GameUtil.Bind<Transform>(gameObject, "SubPined");
				name = GameUtil.Bind<Text>(gameObject, "Text");
				pinedNmae = GameUtil.Bind<Text>(gameObject, "Pined/Text");
			}


			public Toggle Toggle => toggle;


			public void SetIndent(int level)
			{
				Vector2 offsetMin = name.rectTransform.offsetMin;
				offsetMin.x = 10 * level;
				name.rectTransform.offsetMin = offsetMin;
				pinedNmae.rectTransform.offsetMin = offsetMin;
			}


			public void SetCategoryText(string categoryNameKey)
			{
				categoryName = categoryNameKey;
				name.text = Ln.Get(categoryNameKey);
				pinedNmae.text = Ln.Get(categoryNameKey);
			}


			public void SetCategoryTextColor(Color32 color32)
			{
				name.color = color32;
			}


			public void SetCallback(Action<int> callback)
			{
				this.callback = callback;
			}


			public void SetDot(bool enable)
			{
				dot.gameObject.SetActive(enable);
			}


			public bool GetDot()
			{
				return dot.gameObject.activeSelf;
			}


			public void SetActive(bool active)
			{
				gameObject.gameObject.SetActive(active);
			}


			public bool GetActive()
			{
				return gameObject.gameObject.activeSelf;
			}


			public void InvokeCallback(int index)
			{
				Action<int> action = callback;
				if (action == null)
				{
					return;
				}

				action(index);
			}


			public void DisableSelection()
			{
				toggleEvent.targetObject = null;
			}
		}
	}
}