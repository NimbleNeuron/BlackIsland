using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.AirSupplyItemBox)]
	public class LocalAirSupplyItemBox : LocalItemBox
	{
		private readonly List<ParticleSystem> boxEffects = new List<ParticleSystem>();
		private Animator animator;


		private GameObject boxItemObject;


		private bool canOpen;


		private bool isOpened;


		private ItemGrade maxItemGrade;


		private AirSupplyItemBoxSnapshot snapshot;


		public bool CanOpen => canOpen;


		public ItemGrade MaxItemGrade => maxItemGrade;

		protected override ObjectType GetObjectType()
		{
			return ObjectType.AirSupplyItemBox;
		}


		protected override int GetTeamNumber()
		{
			return 0;
		}


		protected override bool GetIsOutSight()
		{
			return false;
		}


		public override void Init(byte[] snapshotData)
		{
			snapshot = serializer.Deserialize<AirSupplyItemBoxSnapshot>(snapshotData);
			canOpen = true;
			maxItemGrade = snapshot.maxItemGrade;
			transform.GetComponentsInChildren<ParticleSystem>(true, boxEffects);
			base.Init(snapshot.itemSpawnPointCode);
		}


		public override GameObject ReleaseChildren()
		{
			base.ReleaseChildren();
			GameObject result = boxItemObject;
			boxItemObject = null;
			animator = null;
			return result;
		}


		protected override void ApplyBoxGraphic(ItemSpawnPoint spawnPoint)
		{
			boxItemObject =
				MonoBehaviourInstance<ClientService>.inst.World.GetAirSupplyBoxObject(maxItemGrade, transform);
			boxItemObject.transform.localPosition = Vector3.zero;
			boxItemObject.transform.localRotation = Quaternion.identity;
			GameUtil.Bind<Animator>(boxItemObject.transform.GetChild(0).gameObject, ref animator);
		}


		public void SetUnopenable()
		{
			canOpen = false;
		}


		public void DropOnGround()
		{
			canOpen = true;
			snapshot = null;
			OnBoxEffect();
		}


		public void OnBoxEffect()
		{
			for (int i = 0; i < boxEffects.Count; i++)
			{
				if (!boxEffects[i].gameObject.activeSelf)
				{
					boxEffects[i].gameObject.SetActive(true);
				}
			}
		}


		public override void OpenBox() { }


		public void PlayOpen()
		{
			if (isOpened)
			{
				return;
			}

			Animator animator = this.animator;
			if (animator != null)
			{
				animator.SetTrigger("airDropOpen");
			}

			isOpened = true;
		}


		public void CancelOpen()
		{
			Animator animator = this.animator;
			if (animator != null)
			{
				animator.SetTrigger(GameConstants.AnimationKey.ANIMATION_CANCEL_TRIGGER);
			}

			isOpened = false;
		}


		public override bool SetCursor(LocalPlayerCharacter myPlayer)
		{
			if (base.SetCursor(myPlayer))
			{
				return true;
			}

			MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorOption(CursorOption.Timer);
			return false;
		}
	}
}