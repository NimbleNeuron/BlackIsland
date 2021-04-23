using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class UIItemTree : BaseControl, ISlotEventListener, IEnumerable
	{
		public delegate void ClickHistoryItemEvent(ItemData itemData);


		public delegate void RequestCombineEvent(ItemData itemData);


		public delegate void TreeSelectionChangeEvent(ItemData itemData);


		[SerializeField] private TreeItemNode root = default;


		private readonly Stack<ItemData> historyStack = new Stack<ItemData>();


		private int focusedItemCode;


		private ItemDataSlot historyItemSlot;


		private ItemData rootItemData;


		private Transform[] treeGeneration;


		public IEnumerator GetEnumerator()
		{
			Queue<TreeItemNode> nodes = new Queue<TreeItemNode>();
			nodes.Enqueue(root);
			while (nodes.Count > 0)
			{
				TreeItemNode treeItemNode = nodes.Dequeue();
				if (treeItemNode.LeftNode != null)
				{
					nodes.Enqueue(treeItemNode.LeftNode);
				}

				if (treeItemNode.RightNode != null)
				{
					nodes.Enqueue(treeItemNode.RightNode);
				}

				yield return treeItemNode;
			}
		}


		public void OnSlotLeftClick(Slot slot)
		{
			ItemDataSlot itemDataSlot = slot as ItemDataSlot;
			if (itemDataSlot != null && itemDataSlot.GetItemData() != null)
			{
				TreeSelectionChangeEvent onClickTreeItem = OnClickTreeItem;
				if (onClickTreeItem == null)
				{
					return;
				}

				onClickTreeItem(itemDataSlot.GetItemData());
			}
		}


		public void OnSlotRightClick(Slot slot)
		{
			ItemDataSlot itemDataSlot = slot as ItemDataSlot;
			if (itemDataSlot != null)
			{
				RequestCombineEvent onRightClickItem = OnRightClickItem;
				if (onRightClickItem == null)
				{
					return;
				}

				onRightClickItem(itemDataSlot.GetItemData());
			}
		}


		public void OnDropItem(Slot slot, BaseUI draggedUI) { }


		public void OnThrowItem(Slot slot) { }


		public void OnThrowItemPiece(Slot slot) { }


		public void OnPointerEnter(Slot slot) { }


		public void OnPointerExit(Slot slot) { }


		public void OnSlotDoubleClick(Slot slot)
		{
			OnSlotRightClick(slot);
		}

		
		
		public event TreeSelectionChangeEvent OnClickTreeItem;


		
		
		public event RequestCombineEvent OnRightClickItem;


		
		
		public event ClickHistoryItemEvent OnClickHistoryItem;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			treeGeneration = new Transform[4];
			treeGeneration[0] = GameUtil.Bind<Transform>(gameObject, "Contents/Items/Gen1");
			treeGeneration[1] = GameUtil.Bind<Transform>(gameObject, "Contents/Items/Gen2");
			treeGeneration[2] = GameUtil.Bind<Transform>(gameObject, "Contents/Items/Gen3");
			treeGeneration[3] = GameUtil.Bind<Transform>(gameObject, "Contents/Items/Gen4");
			historyItemSlot = GameUtil.Bind<ItemDataSlot>(gameObject, "Contents/BackItemSlot");
			historyItemSlot.OnPointerClickEvent += delegate
			{
				OnClickHistorySlot();
				MonoBehaviourInstance<Tooltip>.inst.Hide(GetParentWindow());
			};
			foreach (object obj in this)
			{
				((TreeItemNode) obj).SetEventListener(this);
			}
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			ResetSlots();
			ClampTreeLevel(0);
		}


		private void UpdateTreeUI()
		{
			ResetSlots();
			int num1 = 1;
			int num2 = 0;
			root.SetItemData(this.rootItemData);
			foreach (TreeItemNode treeItemNode in this)
			{
				TreeItemNode node = treeItemNode;
				++num2;
				if (node.GetItemData() != null)
				{
					num1 = num2;
					bool enable = false;
					if (node.GetItemData().makeMaterial1 > 0)
					{
						ItemData itemByCode = GameDB.item.FindItemByCode(node.GetItemData().makeMaterial1);
						if (itemByCode != null && itemByCode.code > 0)
						{
							enable = true;
							if (node.LeftNode != null)
							{
								node.LeftNode.SetItemData(itemByCode);
							}
						}
					}

					if (node.GetItemData().makeMaterial2 > 0)
					{
						ItemData itemByCode = GameDB.item.FindItemByCode(node.GetItemData().makeMaterial2);
						if (itemByCode != null && itemByCode.code > 0)
						{
							enable = true;
							if (node.RightNode != null)
							{
								node.RightNode.SetItemData(itemByCode);
							}
						}
					}

					node.EnableDownTreeBtn(enable, () =>
					{
						ItemData rootItemData = GetRootItemData();
						SetRootItem(node.GetItemData());
						AddItemHistory(rootItemData);
					});
					node.EnableChildLiner(enable);
					if (node.GetItemData() != null && node.GetItemData().code == focusedItemCode)
					{
						node.SetSelection(true);
					}
				}
			}

			ClampTreeLevel(num1 > 1 ? num1 > 3 ? num1 > 7 ? 4 : 3 : 2 : 1);
			UpdateHistory();

			// co: dotPeek
			// this.ResetSlots();
			// int num = 1;
			// int num2 = 0;
			// this.root.SetItemData(this.rootItemData);
			// using (IEnumerator enumerator = this.GetEnumerator())
			// {
			// 	while (enumerator.MoveNext())
			// 	{
			// 		TreeItemNode node = (TreeItemNode)enumerator.Current;
			// 		num2++;
			// 		if (node.GetItemData() != null)
			// 		{
			// 			num = num2;
			// 			bool enable = false;
			// 			if (node.GetItemData().makeMaterial1 > 0)
			// 			{
			// 				ItemData itemData = GameDB.item.FindItemByCode(node.GetItemData().makeMaterial1);
			// 				if (itemData != null && itemData.code > 0)
			// 				{
			// 					enable = true;
			// 					if (node.LeftNode != null)
			// 					{
			// 						node.LeftNode.SetItemData(itemData);
			// 					}
			// 				}
			// 			}
			// 			if (node.GetItemData().makeMaterial2 > 0)
			// 			{
			// 				ItemData itemData2 = GameDB.item.FindItemByCode(node.GetItemData().makeMaterial2);
			// 				if (itemData2 != null && itemData2.code > 0)
			// 				{
			// 					enable = true;
			// 					if (node.RightNode != null)
			// 					{
			// 						node.RightNode.SetItemData(itemData2);
			// 					}
			// 				}
			// 			}
			// 			node.EnableDownTreeBtn(enable, delegate
			// 			{
			// 				ItemData itemData3 = this.GetRootItemData();
			// 				this.SetRootItem(node.GetItemData());
			// 				this.AddItemHistory(itemData3);
			// 			});
			// 			node.EnableChildLiner(enable);
			// 			if (node.GetItemData() != null && node.GetItemData().code == this.focusedItemCode)
			// 			{
			// 				node.SetSelection(true);
			// 			}
			// 		}
			// 	}
			// }
			// int level;
			// if (num <= 1)
			// {
			// 	level = 1;
			// }
			// else if (num <= 3)
			// {
			// 	level = 2;
			// }
			// else if (num <= 7)
			// {
			// 	level = 3;
			// }
			// else
			// {
			// 	level = 4;
			// }
			// this.ClampTreeLevel(level);
			// this.UpdateHistory();
		}


		public void SetRootItem(ItemData itemData)
		{
			rootItemData = itemData;
			UpdateTreeUI();
		}


		public void SetSelection(int itemCode)
		{
			focusedItemCode = itemCode;
			UpdateTreeUI();
		}


		public void Refresh()
		{
			UpdateTreeUI();
		}


		public void ResetSlots()
		{
			foreach (object obj in this)
			{
				((TreeItemNode) obj).ResetUI();
			}

			historyItemSlot.ResetSlot();
			historyItemSlot.transform.localScale = Vector3.zero;
		}


		public void ClearHistory()
		{
			historyStack.Clear();
			UpdateHistory();
		}


		public void AddItemHistory(ItemData itemData)
		{
			if (itemData == null)
			{
				return;
			}

			if (historyStack.Count > 0 && historyStack.Peek().code == itemData.code)
			{
				return;
			}

			historyStack.Push(itemData);
			UpdateHistory();
		}


		public void OnClickHistorySlot()
		{
			if (historyStack.Count > 0)
			{
				OnClickHistoryItem(historyStack.Pop());
				UpdateHistory();
			}
		}


		private void UpdateHistory()
		{
			if (historyStack.Count > 0)
			{
				ItemData itemData = historyStack.Peek();
				historyItemSlot.transform.localScale = Vector3.one;
				historyItemSlot.SetItemData(itemData);
				historyItemSlot.SetSlotType(SlotType.None);
				historyItemSlot.SetSprite(itemData.GetSprite());
				historyItemSlot.SetBackground(itemData.GetGradeSprite());
				return;
			}

			historyItemSlot.transform.localScale = Vector3.zero;
		}


		public ItemData GetRootItemData()
		{
			return root.GetItemData();
		}


		private void ClampTreeLevel(int level)
		{
			for (int i = 0; i < treeGeneration.Length; i++)
			{
				treeGeneration[i].gameObject.SetActive(i < level);
			}
		}
	}
}