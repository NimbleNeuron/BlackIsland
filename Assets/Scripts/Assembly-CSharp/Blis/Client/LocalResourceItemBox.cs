using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.ResourceItemBox)]
	public class LocalResourceItemBox : LocalItemBox
	{
		private int areaCode;
		private GameObject childObject;
		private CollectibleData collectibleData;
		private float cooldownUntil;
		private ItemBoxFloatingUI floatingUi;
		private int resourceDataCode;
		public float CooldownUntil => cooldownUntil;
		public CollectibleData CollectibleData => collectibleData;

		protected override ObjectType GetObjectType()
		{
			return ObjectType.ResourceItemBox;
		}

		public override void OpenBox() { }

		public void Init(int itemSpawnPointCode, int resourceDataCode, int areaCode, float initSpawnTime)
		{
			base.Init(itemSpawnPointCode);
			GameUtil.Bind<ItemBoxFloatingUI>(gameObject, ref floatingUi);
			floatingUi.Init(this);
			cooldownUntil = 0f;
			this.resourceDataCode = resourceDataCode;
			this.areaCode = areaCode;
			collectibleData = GameDB.level.GetCollectibleData(resourceDataCode);
			if (initSpawnTime > 0f)
			{
				childObject = transform.FindRecursively("ChildObject").gameObject;
				childObject.SetActive(false);
			}
		}


		public override string GetLocalizedName(bool includeColor)
		{
			return LnUtil.GetItemName(collectibleData.itemCode);
		}


		protected override bool GetIsOutSight()
		{
			return false;
		}


		public override void Init(byte[] snapshotData)
		{
			ResourceItemBoxSnapshot resourceItemBoxSnapshot =
				serializer.Deserialize<ResourceItemBoxSnapshot>(snapshotData);
			cooldownUntil = resourceItemBoxSnapshot.cooldownUntil.Value -
				MonoBehaviourInstance<ClientService>.inst.CurrentServerFrameTime + Time.time;
		}


		public void StartCooldown(float cooldown)
		{
			ActiveChildObject(false);
			cooldownUntil = Time.time + cooldown;
		}


		public void ReadyChildObject()
		{
			string key = string.Empty;
			CastingActionType castingActionType = collectibleData.castingActionType;
			if (castingActionType == CastingActionType.CollectibleOpenTreeOfLife)
			{
				key = "생명의나무생성";
			}

			int param_ = 1;
			string text = Ln.Format(key, param_, LnUtil.GetAreaName(areaCode));
			if (collectibleData.castingActionType == CastingActionType.CollectibleOpenTreeOfLife)
			{
				MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(text, true);
				MonoBehaviourInstance<GameUI>.inst.SpecialAnnounceUI.ShowMessage(AnnounceType.TreeOfLife_CreateExpected,
					text);
				return;
			}

			MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddStrategySystemChatting(text);
		}


		public void ActiveChildObject(bool active)
		{
			if (childObject != null)
			{
				childObject.SetActive(active);
			}

			if (active && collectibleData.castingActionType == CastingActionType.CollectibleOpenTreeOfLife)
			{
				string text = Ln.Format("생명의나무생성완료", LnUtil.GetAreaName(areaCode));
				MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(text, true);
				MonoBehaviourInstance<GameUI>.inst.SpecialAnnounceUI.ShowMessage(AnnounceType.TreeOfLife_Appear, text);
			}
		}


		public override bool SetCursor(LocalPlayerCharacter myPlayer)
		{
			MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.ResourceBox);
			if (Time.time < CooldownUntil)
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorOption(CursorOption.Restrict);
			}
			else
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorOption(CursorOption.Timer);
			}

			return true;
		}
	}
}