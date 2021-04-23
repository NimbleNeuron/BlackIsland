using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blis.Client
{
	public class UISearch : BaseUI
	{
		public delegate void OnClickItemEvent(ItemData itemData);


		public enum Mode
		{
			ItemType,

			ItemOption,

			Area
		}


		[SerializeField] private UISearchCategoryList uiSearchCategoryList = default;


		[SerializeField] private UIItemScrollView uiItemScrollView = default;


		[SerializeField] private InputFieldExtension inputField = default;


		private CanvasGroup canvasGroup;


		private Mode currentMode;


		private string editString = string.Empty;


		private List<ItemData> itemDataList;


		private SimpleSearchNode root;


		private WeaponType[] weaponTypes = new WeaponType[0];

		
		
		public event OnClickItemEvent OnClickListItem = delegate { };


		
		
		public event OnClickItemEvent OnRightClickItem = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			uiItemScrollView.OnClickItemHandler += OnClickItem;
			inputField.onValueChanged.AddListener(OnInputValueChange);
			inputField.onEndEdit.AddListener(OnEndEdit);
			GameUtil.BindOrAdd<CanvasGroup>(gameObject, ref canvasGroup);
		}


		public void Show()
		{
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}


		public void Hide()
		{
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}


		private void OnEndEdit(string input)
		{
			if (MonoBehaviourInstance<GameInput>.inst != null)
			{
				MonoBehaviourInstance<GameInput>.inst.SetLockInputSearch(false);
			}
		}


		private void OnInputValueChange(string input)
		{
			if (editString != input)
			{
				editString = LnUtil.GetSearchName(input);
				SetScrollViewData();
			}
		}


		public void ClearInputField()
		{
			if (MonoBehaviourInstance<GameInput>.inst != null)
			{
				MonoBehaviourInstance<GameInput>.inst.SetLockInputSearch(false);
			}

			inputField.text = string.Empty;
			inputField.textComponent.text = string.Empty;
			SetScrollViewData();
		}


		public void SetLockInputSearch(bool isLock)
		{
			inputField.SetLockInputSearch(isLock);
		}


		public void OnUpdateInventory(List<Item> items)
		{
			uiItemScrollView.OnUpdateInventory(items);
		}


		public void OnUpdateEquipment(List<Item> items)
		{
			uiItemScrollView.OnUpdateEquipment(items);
		}


		public void SetDraggable(bool draggable)
		{
			uiItemScrollView.SetDraggable(draggable);
		}


		public void UpdateCategoryUI()
		{
			int num = 0;
			uiSearchCategoryList.ClearCategory();
			if (currentMode == Mode.ItemType)
			{
				root = new SimpleSearchNode(num++, null);
				uiSearchCategoryList.AddCategoryHeader("모든 아이템", Color.white, OnClickedCategory);
				SimpleSearchNode parent = RenderCategories<WeaponType>(root, "무기", weaponTypes, ref num);
				RenderSubCategory<WeaponType>(parent, "그 외", WeaponType.None, ref num);
				RenderCategories<ArmorType>(root, "방어구", new[]
				{
					ArmorType.Chest,
					ArmorType.Head,
					ArmorType.Arm,
					ArmorType.Leg,
					ArmorType.Trinket
				}, ref num);
				RenderCategories<ItemConsumableType>(root, "음식", new[]
				{
					ItemConsumableType.Food,
					ItemConsumableType.Beverage
				}, ref num);
				RenderCategories<SpecialItemType>(root, "특수", new[]
				{
					SpecialItemType.Summon
				}, ref num);
				RenderCategories<MiscItemType>(root, "재료", new[]
				{
					MiscItemType.Material
				}, ref num);
			}
			else if (currentMode == Mode.ItemOption)
			{
				root = new SimpleSearchNode(num++, null);
				uiSearchCategoryList.AddCategoryHeader("모든 아이템", Color.white, OnClickedCategory);
				RenderCategories<ItemOptionCategory>(root, "기본공격", new[]
				{
					ItemOptionCategory.AttackPower,
					ItemOptionCategory.AttackSpeed,
					ItemOptionCategory.CriticalStrike,
					ItemOptionCategory.LifeSteal,
					ItemOptionCategory.NormalAttackIncrease
				}, ref num);
				RenderCategories<ItemOptionCategory>(root, "스킬", new[]
				{
					ItemOptionCategory.SkillAttackIncrease,
					ItemOptionCategory.CooldownReduction,
					ItemOptionCategory.MaxSp,
					ItemOptionCategory.SpRegen
				}, ref num);
				RenderCategories<ItemOptionCategory>(root, "방어", new[]
				{
					ItemOptionCategory.Defense,
					ItemOptionCategory.MaxHp,
					ItemOptionCategory.HpRegen,
					ItemOptionCategory.DamageReduction
				}, ref num);
				RenderCategories<ItemOptionCategory>(root, "기타", new[]
				{
					ItemOptionCategory.MoveSpeed,
					ItemOptionCategory.SightRange
				}, ref num);
			}
			else if (currentMode == Mode.Area)
			{
				root = RenderCategories<int>(null, "모든 지역", new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				}, ref num);
			}

			uiSearchCategoryList.SelectCategory(root.Index);
		}


		private SimpleSearchNode RenderCategories<T>(SimpleSearchNode parent, string headerName,
			IEnumerable<T> categories, ref int index)
		{
			Type typeFromHandle = typeof(T);
			int num = index;
			index = num + 1;
			SimpleSearchNode simpleSearchNode = new SimpleSearchNode(num, typeof(T));
			if (parent != null)
			{
				parent.Add(simpleSearchNode);
			}

			uiSearchCategoryList.AddCategoryHeader(headerName, new Color32(168, 110, 21, byte.MaxValue),
				OnClickedCategory);
			foreach (T t in categories)
			{
				if (typeFromHandle == typeof(int))
				{
					RenderSubCategory<T>(simpleSearchNode, string.Format("Area/Name/{0}", t), t, ref index);
				}
				else
				{
					RenderSubCategory<T>(simpleSearchNode, string.Format("{0}/{1}", typeFromHandle.Name, t), t,
						ref index);
				}
			}

			return simpleSearchNode;
		}


		private SimpleSearchNode RenderSubCategory<T>(SimpleSearchNode parent, string headerName, T category,
			ref int index)
		{
			Type typeFromHandle = typeof(T);
			SimpleSearchNode simpleSearchNode;
			if (category is Enum)
			{
				int num = index;
				index = num + 1;
				simpleSearchNode = new SimpleSearchNode(num, Enum.ToObject(typeFromHandle, category));
				uiSearchCategoryList.AddCategoryItem(headerName, Color.white, OnClickedCategory);
			}
			else
			{
				int num = index;
				index = num + 1;
				simpleSearchNode = new SimpleSearchNode(num, category);
				uiSearchCategoryList.AddCategoryItem(headerName, Color.white, OnClickedCategory);
			}

			parent.Add(simpleSearchNode);
			return simpleSearchNode;
		}


		private void OnClickedCategory(int index)
		{
			SimpleSearchNode simpleSearchNode = root.Find(index);
			if (currentMode == Mode.Area)
			{
				if (simpleSearchNode.subType is int)
				{
					uiSearchCategoryList.ClearAllDotMark();
					uiSearchCategoryList.SetDotMark(simpleSearchNode.Index, true);
					goto IL_15B;
				}

				uiSearchCategoryList.ClearAllDotMark();
				using (IEnumerator<SimpleSearchNode> enumerator = simpleSearchNode.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SimpleSearchNode simpleSearchNode2 = enumerator.Current;
						if (simpleSearchNode2.subType is int)
						{
							uiSearchCategoryList.SetDotMark(simpleSearchNode2.Index, true);
						}
					}

					goto IL_15B;
				}
			}

			if (currentMode == Mode.ItemOption)
			{
				uiSearchCategoryList.ClearAllDotMark();
				using (IEnumerator<SimpleSearchNode> enumerator = simpleSearchNode.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SimpleSearchNode simpleSearchNode3 = enumerator.Current;
						if (simpleSearchNode3.subType is Enum && (!(simpleSearchNode3.subType is WeaponType) ||
						                                          (WeaponType) simpleSearchNode3.subType !=
						                                          WeaponType.None))
						{
							uiSearchCategoryList.SetDotMark(simpleSearchNode3.Index, true);
						}
					}

					goto IL_15B;
				}
			}

			uiSearchCategoryList.ClearAllDotMark();
			foreach (SimpleSearchNode simpleSearchNode4 in simpleSearchNode)
			{
				if (simpleSearchNode4.subType is Enum)
				{
					uiSearchCategoryList.SetDotMark(simpleSearchNode4.Index, true);
				}
			}

			IL_15B:
			UpdateSearchListUI(index);
		}


		public void UpdateSearchListUI(int index)
		{
			List<object> list = new List<object>();
			if (currentMode == Mode.ItemType)
			{
				if (index != 0)
				{
					using (IEnumerator<SimpleSearchNode> enumerator = root.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SimpleSearchNode simpleSearchNode = enumerator.Current;
							if (simpleSearchNode.subType is WeaponType &&
							    (WeaponType) simpleSearchNode.subType == WeaponType.None)
							{
								uiSearchCategoryList.SetDotMark(simpleSearchNode.Index, false);
							}
							else if (uiSearchCategoryList.GetDotMark(simpleSearchNode.Index))
							{
								list.Add(simpleSearchNode.subType);
							}
						}

						goto IL_120;
					}
				}

				using (IEnumerator<SimpleSearchNode> enumerator = root.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SimpleSearchNode simpleSearchNode2 = enumerator.Current;
						if (uiSearchCategoryList.GetDotMark(simpleSearchNode2.Index))
						{
							list.Add(simpleSearchNode2.subType);
						}
					}

					goto IL_120;
				}
			}

			foreach (SimpleSearchNode simpleSearchNode3 in root)
			{
				if (uiSearchCategoryList.GetDotMark(simpleSearchNode3.Index))
				{
					list.Add(simpleSearchNode3.subType);
				}
			}

			IL_120:
			itemDataList = Filtering(list, currentMode);
			itemDataList.Sort(new ItemDB.BookItemComparer());
			SetScrollViewData();
		}


		private void SetScrollViewData()
		{
			List<ItemData> list = new List<ItemData>();
			for (int i = 0; i < itemDataList.Count; i++)
			{
				if (LnUtil.GetSearchName(LnUtil.GetItemName(itemDataList[i].code)).Contains(editString))
				{
					list.Add(itemDataList[i]);
				}
			}

			uiItemScrollView.SetDataSource(list);
		}


		private List<ItemData> Filtering(List<object> subTypeList, Mode mode)
		{
			HashSet<ItemData> itemDataSet = new HashSet<ItemData>();
			switch (mode)
			{
				case Mode.ItemType:
					List<Func<ItemData, bool>> matchFuncList = new List<Func<ItemData, bool>>();
					for (int index = 0; index < subTypeList.Count; ++index)
					{
						Func<ItemData, bool> matchFunc = GetMatchFunc(subTypeList[index]);
						if (matchFunc != null)
						{
							matchFuncList.Add(matchFunc);
						}
					}

					object obj;
					if (subTypeList.Count <= 0 ||
					    subTypeList.Find(n =>
						    (obj = n) is WeaponType && (WeaponType) obj == WeaponType.None) != null)
					{
						itemDataSet.UnionWith(GameDB.item.GetAllItems()
							.Where<ItemData>(x => x.itemType == ItemType.Weapon)
							.Where<ItemData>(x =>
							{
								ItemWeaponData subTypeData = x.GetSubTypeData<ItemWeaponData>();
								for (int index = 0; index < weaponTypes.Length; ++index)
								{
									if (weaponTypes[index] == subTypeData.weaponType)
									{
										return false;
									}
								}

								return true;
							}));
					}

					itemDataSet.UnionWith(GameDB.item.GetAllItems().Where<ItemData>(x =>
					{
						for (int index = 0; index < matchFuncList.Count; ++index)
						{
							if (matchFuncList[index](x))
							{
								return true;
							}
						}

						return false;
					}));
					break;
				case Mode.ItemOption:
					for (int index = 0; index < subTypeList.Count; ++index)
					{
						itemDataSet.UnionWith(GameDB.item.FindItemsByCategory((ItemOptionCategory) subTypeList[index])
							.Select<int, ItemData>(x => GameDB.item.FindItemByCode(x)));
					}

					RemoveOtherWeapon(itemDataSet);
					break;
				case Mode.Area:
					for (int i = 0; i < subTypeList.Count; i++)
					{
						List<ItemData> list = GameDB.level.DefaultLevel.itemSpawns
							.Where<ItemSpawnData>(x => x.areaCode == (int) subTypeList[i])
							.Select<ItemSpawnData, ItemData>(
								x => GameDB.item.FindItemByCode(x.itemCode))
							.ToList<ItemData>();
						itemDataSet.UnionWith(list);
					}

					break;
			}

			return itemDataSet.ToList<ItemData>();

			// co: dotPeek
			//    HashSet<ItemData> hashSet = new HashSet<ItemData>();
			// if (mode == UISearch.Mode.ItemOption)
			// {
			// 	for (int k = 0; k < subTypeList.Count; k++)
			// 	{
			// 		hashSet.UnionWith(from x in GameDB.item.FindItemsByCategory((ItemOptionCategory)subTypeList[k])
			// 		select GameDB.item.FindItemByCode(x));
			// 	}
			// 	this.RemoveOtherWeapon(hashSet);
			// }
			// else if (mode == UISearch.Mode.ItemType)
			// {
			// 	List<Func<ItemData, bool>> matchFuncList = new List<Func<ItemData, bool>>();
			// 	for (int j = 0; j < subTypeList.Count; j++)
			// 	{
			// 		Func<ItemData, bool> matchFunc = this.GetMatchFunc(subTypeList[j]);
			// 		if (matchFunc != null)
			// 		{
			// 			matchFuncList.Add(matchFunc);
			// 		}
			// 	}
			// 	if (subTypeList.Count > 0)
			// 	{
			// 		if (subTypeList.Find(delegate(object n)
			// 		{
			// 			if (n is WeaponType)
			// 			{
			// 				WeaponType weaponType = (WeaponType)n;
			// 				return weaponType == WeaponType.None;
			// 			}
			// 			return false;
			// 		}) == null)
			// 		{
			// 			goto IL_15B;
			// 		}
			// 	}
			// 	int k;
			// 	hashSet.UnionWith((from x in GameDB.item.GetAllItems()
			// 	where x.itemType == ItemType.Weapon
			// 	select x).Where(delegate(ItemData x)
			// 	{
			// 		ItemWeaponData subTypeData = x.GetSubTypeData<ItemWeaponData>();
			// 		for (int k = 0; k < this.weaponTypes.Length; k++)
			// 		{
			// 			if (this.weaponTypes[k] == subTypeData.weaponType)
			// 			{
			// 				return false;
			// 			}
			// 		}
			// 		return true;
			// 	}));
			// 	IL_15B:
			// 	hashSet.UnionWith(GameDB.item.GetAllItems().Where(delegate(ItemData x)
			// 	{
			// 		for (int k = 0; k < matchFuncList.Count; k++)
			// 		{
			// 			if (matchFuncList[k](x))
			// 			{
			// 				return true;
			// 			}
			// 		}
			// 		return false;
			// 	}));
			// }
			// else if (mode == UISearch.Mode.Area)
			// {
			// 	int i2;
			// 	int i;
			// 	for (i = 0; i < subTypeList.Count; i = i2 + 1)
			// 	{
			// 		List<ItemData> other = (from x in GameDB.level.DefaultLevel.itemSpawns
			// 		where x.areaCode == (int)subTypeList[i]
			// 		select GameDB.item.FindItemByCode(x.itemCode)).ToList<ItemData>();
			// 		hashSet.UnionWith(other);
			// 		i2 = i;
			// 	}
			// }
			// return hashSet.ToList<ItemData>();
		}


		private void RemoveOtherWeapon(HashSet<ItemData> itemSet)
		{
			itemSet.RemoveWhere(delegate(ItemData x)
			{
				if (x.itemType == ItemType.Weapon)
				{
					ItemWeaponData subTypeData = x.GetSubTypeData<ItemWeaponData>();
					for (int i = 0; i < weaponTypes.Length; i++)
					{
						if (weaponTypes[i] == subTypeData.weaponType)
						{
							return false;
						}
					}

					return true;
				}

				return false;
			});
		}


		private void OnClickItem(ItemData itemData, MouseButton mouseButton)
		{
			if (mouseButton == MouseButton.LeftMouse)
			{
				OnClickListItem(itemData);
				return;
			}

			if (mouseButton == MouseButton.RightMouse)
			{
				OnRightClickItem(itemData);
			}
		}


		public void SetMode(Mode mode)
		{
			currentMode = mode;
			UpdateCategoryUI();
		}


		public void SetWeaponTypes(WeaponType[] weaponTypes)
		{
			this.weaponTypes = weaponTypes;
			UpdateCategoryUI();
		}


		public void SetPreferItems(List<ItemData> items)
		{
			uiItemScrollView.SetPreferItems(items);
		}


		public void SetFocusItem(ItemData itemData)
		{
			uiItemScrollView.SetFocusItemCode(itemData.code);
		}


		public Func<ItemData, bool> GetMatchFunc(object subType)
		{
			if (subType is WeaponType)
			{
				return delegate(ItemData item)
				{
					bool result = false;
					try
					{
						if (item.itemType == ItemType.Weapon)
						{
							result = item.GetSubTypeData<ItemWeaponData>().weaponType == (WeaponType) subType;
						}
					}
					catch (Exception e)
					{
						Log.Exception(e);
					}

					return result;
				};
			}

			if (subType is ArmorType)
			{
				return delegate(ItemData item)
				{
					bool result = false;
					try
					{
						if (item.itemType == ItemType.Armor)
						{
							result = item.GetSubTypeData<ItemArmorData>().armorType == (ArmorType) subType;
						}
					}
					catch (Exception e)
					{
						Log.Exception(e);
					}

					return result;
				};
			}

			if (subType is ItemConsumableType)
			{
				return delegate(ItemData item)
				{
					bool result = false;
					try
					{
						if (item.itemType == ItemType.Consume)
						{
							result = item.GetSubTypeData<ItemConsumableData>().consumableType ==
							         (ItemConsumableType) subType;
						}
					}
					catch (Exception e)
					{
						Log.Exception(e);
					}

					return result;
				};
			}

			if (subType is SpecialItemType)
			{
				return delegate(ItemData item)
				{
					bool result = false;
					try
					{
						if (item.itemType == ItemType.Special)
						{
							result = item.GetSubTypeData<ItemSpecialData>().specialItemType ==
							         (SpecialItemType) subType;
						}
					}
					catch (Exception e)
					{
						Log.Exception(e);
					}

					return result;
				};
			}

			if (subType is MiscItemType)
			{
				return delegate(ItemData item)
				{
					bool result = false;
					try
					{
						if (item.itemType == ItemType.Misc)
						{
							result = item.GetSubTypeData<ItemMiscData>().miscItemType == (MiscItemType) subType;
						}
					}
					catch (Exception e)
					{
						Log.Exception(e);
					}

					return result;
				};
			}

			return null;
		}


		private class SimpleSearchNode : IEnumerable<SimpleSearchNode>, IEnumerable
		{
			private readonly List<SimpleSearchNode> childNodes = new List<SimpleSearchNode>();


			public readonly object subType;


			private SimpleSearchNode parent;


			public SimpleSearchNode(int index, object subType)
			{
				Index = index;
				this.subType = subType;
			}


			public int Index { get; }


			public SimpleSearchNode Parent => parent;


			public int ChildCount => childNodes.Count;


			public IEnumerator<SimpleSearchNode> GetEnumerator()
			{
				yield return this;
				int num;
				for (int i = 0; i < childNodes.Count; i = num + 1)
				{
					foreach (SimpleSearchNode simpleSearchNode in childNodes[i])
					{
						yield return simpleSearchNode;
					}

					num = i;
				}
			}


			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}


			public void Add(SimpleSearchNode node)
			{
				childNodes.Add(node);
				node.parent = this;
			}


			public void Clear()
			{
				childNodes.Clear();
			}


			public SimpleSearchNode Find(int index)
			{
				if (Index == index)
				{
					return this;
				}

				for (int i = 0; i < childNodes.Count; i++)
				{
					SimpleSearchNode simpleSearchNode = childNodes[i].Find(index);
					if (simpleSearchNode != null)
					{
						return simpleSearchNode;
					}
				}

				return null;
			}


			public SimpleSearchNode GetChild(int i)
			{
				if (i < childNodes.Count)
				{
					return childNodes[i];
				}

				return null;
			}
		}
	}
}