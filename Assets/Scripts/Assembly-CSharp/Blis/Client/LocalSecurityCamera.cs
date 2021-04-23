using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.SecurityCamera)]
	public class LocalSecurityCamera : LocalObject
	{
		private const float HEAD_AX = 0.05f;


		[SerializeField] private Transform headPivot = default;


		[SerializeField] private Transform cameraPivot = default;


		[SerializeField] private RotationTweener tweener = default;


		[SerializeField] private RotationTweener cameraTweener = default;


		private readonly Vector3 g = default;


		private int areaCode = default;


		private DefaultColliderAgent colliderAgent = default;


		private LocalSecurityConsole ownerConsole = default;


		private LocalSightAgent sightAgent = default;


		public int AreaCode => areaCode;


		private void Update()
		{
			if (ownerConsole != null && MonoBehaviourInstance<ClientService>.inst.MyPlayer != null)
			{
				LocalPlayerCharacter character = MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character;
				if (ownerConsole.GetCheckoutCount() > 0 && !ownerConsole.IsCheckoutPlayer(character.ObjectId) &&
				    sightAgent.IsInSight(character.SightAgent, character.GetPosition(), 0f, character.IsInvisible))
				{
					tweener.StopAnimation();
					cameraTweener.StopAnimation();
					Vector3 vector = character.GetPosition() - headPivot.transform.position;
					vector.y = 0f;
					Quaternion b = GameUtil.LookRotation(-vector.normalized);
					headPivot.transform.rotation = Quaternion.Slerp(headPivot.transform.rotation, b, 0.05f);
					float x = Mathf.Lerp(-40f, 0f, vector.magnitude / 10f);
					cameraPivot.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
					return;
				}
			}

			if (!tweener.IsPlaying)
			{
				tweener.from = headPivot.eulerAngles + new Vector3(0f, -180f, 0f);
				tweener.to = tweener.from + new Vector3(0f, 360f, 0f);
				tweener.PlayAnimation();
				Vector3 localEulerAngles = cameraPivot.localEulerAngles;
				localEulerAngles.x -= 360f;
				cameraTweener.from = localEulerAngles;
				cameraTweener.PlayAnimation();
			}
		}


		private void OnDrawGizmos()
		{
			Gizmos.DrawLine(cameraPivot.transform.position, cameraPivot.transform.position + g);
		}

		protected override ObjectType GetObjectType()
		{
			return ObjectType.SecurityCamera;
		}


		protected override int GetTeamNumber()
		{
			return 0;
		}


		protected override ColliderAgent GetColliderAgent()
		{
			return colliderAgent;
		}


		public void Init(int areaCode)
		{
			this.areaCode = areaCode;
			GameUtil.BindOrAdd<DefaultColliderAgent>(gameObject, ref colliderAgent);
			GameUtil.BindOrAdd<LocalSightAgent>(gameObject, ref sightAgent);
			sightAgent.UpdateSightRange(10f);
			ConnectConsole(MonoBehaviourInstance<ClientService>.inst.World.FindAll<LocalSecurityConsole>()
				.FirstOrDefault(x => x.AreaCode == areaCode));
		}


		protected override bool GetIsOutSight()
		{
			return false;
		}


		public override string GetLocalizedName(bool includeColor)
		{
			return Ln.Get("보안 카메라");
		}


		public override void Init(byte[] snapshotData) { }


		public void ConnectConsole(LocalSecurityConsole console)
		{
			ownerConsole = console;
		}
	}
}