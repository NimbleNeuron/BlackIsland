using BIFog;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.Item)]
	public class LocalItem : LocalObject
	{
		private ItemColliderAgent colliderAgent;


		private FogHiderOnCenter fogHiderOnCenter;


		private Item item;


		private ItemFloatingUI itemFloatingUI;


		public FogHiderOnCenter FogHiderOnCenter => fogHiderOnCenter;


		protected override ObjectType GetObjectType()
		{
			return ObjectType.Item;
		}


		protected override int GetTeamNumber()
		{
			return 0;
		}


		protected override ColliderAgent GetColliderAgent()
		{
			return colliderAgent;
		}


		public override string GetLocalizedName(bool includeColor)
		{
			if (includeColor)
			{
				return StringUtil.Coloring(item.ItemData.itemGrade.GetColor(), LnUtil.GetItemName(item.itemCode));
			}

			return LnUtil.GetItemName(item.itemCode);
		}


		protected override bool GetIsOutSight()
		{
			return false;
		}


		public override void Init(byte[] snapshotData)
		{
			ItemSnapshot itemSnapshot = serializer.Deserialize<ItemSnapshot>(snapshotData);
			item = itemSnapshot.item;
			GameUtil.BindOrAdd<ItemColliderAgent>(this.gameObject, ref colliderAgent);
			colliderAgent.Init();
			GameObject gameObject =
				Resources.Load<GameObject>("WorldObjects/Client/Item/Item_" + item.itemCode.ToString().PadLeft(5, '0'));
			if (gameObject == null)
			{
				GameObject gameObject2 = new GameObject();
				gameObject2.transform.parent = transform;
				Pickable pickable = AttachPickable(gameObject2);
				pickable.Init(this);
				pickable.gameObject.AddComponent<BoxCollider>().size = Vector3.one;
				gameObject2.AddComponent<SpriteRenderer>().sprite = item.ItemData.GetSprite();
				Log.E("No Prefab For Item: [{0}]", item.ItemData.name);
			}
			else
			{
				GameObject gameObject2 = Instantiate<GameObject>(gameObject, transform);
				AttachPickable(gameObject2).Init(this);
			}

			ItemMadeType madeType = item.madeType;
			GameUtil.BindOrAdd<FogHiderOnCenter>(this.gameObject, ref fogHiderOnCenter);
			fogHiderOnCenter.Init(GetObjectType());
			fogHiderOnCenter.RebuildRendererList();
			GameUtil.BindOrAdd<ItemFloatingUI>(this.gameObject, ref itemFloatingUI);
			itemFloatingUI.Init(item);
			if (fogHiderOnCenter.IsInSight)
			{
				itemFloatingUI.OnSight();
			}
		}


		public override ObjectOrder GetObjectOrder()
		{
			return ObjectOrder.Item;
		}


		public override bool IsMouseHitPossible(LocalSightAgent targetSightAgent, bool isInvisible)
		{
			return true;
		}


		public override bool SetCursor(LocalPlayerCharacter myPlayer)
		{
			MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.Item);
			return true;
		}
	}
}