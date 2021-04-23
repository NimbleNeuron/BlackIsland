using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[RequireComponent(typeof(CharacterFloatingUI))]
	[ObjectAttr(ObjectType.Dummy)]
	public class LocalDummy : LocalMovableCharacter
	{
		private ClientCharacter clientCharacter;


		private DummyHostileAgent hostileAgent;


		private int prefabNo;

		public override void Init(byte[] snapshotData)
		{
			DummySnapshot dummySnapshot = serializer.Deserialize<DummySnapshot>(snapshotData);
			prefabNo = dummySnapshot.prefabNo;
			hostileAgent = new DummyHostileAgent(this);
			base.Init(snapshotData);
			if (characterAnimator != null)
			{
				characterAnimator.logWarnings = false;
			}

			clientCharacter = characterObject.GetComponentInChildren<ClientCharacter>();
			if (clientCharacter != null)
			{
				clientCharacter.Init(this, objectId);
			}
		}


		protected override ObjectType GetObjectType()
		{
			return ObjectType.Dummy;
		}


		protected override int GetTeamNumber()
		{
			return 0;
		}


		protected override int GetCharacterCode()
		{
			return 0;
		}


		protected override string GetNickname()
		{
			return "Dummy";
		}


		protected override HostileAgent GetHostileAgent()
		{
			return hostileAgent;
		}


		protected override GameObject GetCharacterPrefab()
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.LoadObject(string.Format("Ward_{0}", prefabNo));
		}


		public override void OnSight()
		{
			base.OnSight();
		}


		public override ObjectOrder GetObjectOrder()
		{
			if (IsAlive)
			{
				if (!MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
				{
					return ObjectOrder.AliveEnemy;
				}

				return ObjectOrder.AliveAlly;
			}

			if (!MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
			{
				return ObjectOrder.DeadEnemy;
			}

			return ObjectOrder.DeadAlly;
		}
	}
}