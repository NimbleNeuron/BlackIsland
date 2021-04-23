using System.Text.RegularExpressions;
using BIOutline;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.ItemBox)]
	public abstract class LocalItemBox : LocalObject
	{
		private ItemBoxColliderAgent colliderAgent;


		protected string firstChildBoxName;


		private int lastInitObjectId;


		protected SelectionRenderer selectionRenderer;

		protected override ObjectType GetObjectType()
		{
			return ObjectType.ItemBox;
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
			return Ln.Get("상자");
		}


		protected void Init(int itemSpawnPointCode)
		{
			GameUtil.Bind<SelectionRenderer>(gameObject, ref selectionRenderer);
			selectionRenderer.SetColor(SingletonMonoBehaviour<PlayerController>.inst.MouseOverManager.InSightColor);
			selectionRenderer.SetUntouched(true);
			GameUtil.BindOrAdd<ItemBoxColliderAgent>(gameObject, ref colliderAgent);
			if (lastInitObjectId != ObjectId)
			{
				lastInitObjectId = ObjectId;
				if (itemSpawnPointCode > 0)
				{
					ItemSpawnPoint itemSpawnPointByCode =
						MonoBehaviourInstance<ClientService>.inst.CurrentLevel.GetItemSpawnPointByCode(
							itemSpawnPointCode);
					colliderAgent.Init(ObjectType, itemSpawnPointByCode.shape, itemSpawnPointByCode.GetCollider(),
						!itemSpawnPointByCode.airSupply);
					string text = Regex.Replace(itemSpawnPointByCode.transform.GetChild(0).name, "\\([0-9]\\)", "");
					firstChildBoxName = text.Trim();
					ApplyBoxGraphic(itemSpawnPointByCode);
				}
			}

			AttachPickable(colliderAgent.gameObject).Init(this);
		}


		protected virtual void ApplyBoxGraphic(ItemSpawnPoint spawnPoint)
		{
			GameObject gameObject = Instantiate<GameObject>(spawnPoint.gameObject, transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.SetActive(true);
			Outliner component = GetComponent<Outliner>();
			if (component == null)
			{
				return;
			}

			component.OrganizeDrawCall();
		}


		public abstract void OpenBox();


		public override bool IsMouseHitPossible(LocalSightAgent targetSightAgent, bool isInvisible)
		{
			return true;
		}


		public override bool SetCursor(LocalPlayerCharacter myPlayer)
		{
			MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.ItemBox);
			if (MonoBehaviourInstance<ClientService>.inst.MyPlayer.IsInteractedObject(this))
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorOption(CursorOption.Opened);
				return true;
			}

			return false;
		}
	}
}