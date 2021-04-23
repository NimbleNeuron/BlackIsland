using BIOutline;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.Hyperloop)]
	public class LocalHyperloop : LocalObject
	{
		private const string HYPER_LOOP_WORK = "tHiperLoopWork";


		private const string CANCEL = "Cancel";


		private Animator animator;


		private CubeColliderAgent colliderAgent;


		private SelectionRenderer selectionRenderer;

		protected override ObjectType GetObjectType()
		{
			return ObjectType.Hyperloop;
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
			return Ln.Get("TutorialMessageBox/PowerUp/Step2/Title");
		}


		public void Init()
		{
			GameUtil.Bind<SelectionRenderer>(gameObject, ref selectionRenderer);
			selectionRenderer.SetColor(SingletonMonoBehaviour<PlayerController>.inst.MouseOverManager.InSightColor);
			selectionRenderer.SetUntouched(true);
			GameUtil.BindOrAdd<CubeColliderAgent>(gameObject, ref colliderAgent);
			colliderAgent.Init(1f);
			animator = GetComponentInChildren<Animator>();
			AttachPickable(gameObject).Init(this);
			MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.CreateStaticObject(objectId, GetPosition(),
				SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Map_HyperLoop"),
				MiniMapIconType.System);
		}


		protected override bool GetIsOutSight()
		{
			return false;
		}


		public override void Init(byte[] snapshotData) { }


		public void PlayHyperLoopAnimation()
		{
			animator.ResetTrigger("tHiperLoopWork");
			animator.SetTrigger("tHiperLoopWork");
		}


		public void StopHyperLoopAnimation()
		{
			animator.ResetTrigger("Cancel");
			animator.SetTrigger("Cancel");
		}


		public override bool IsMouseHitPossible(LocalSightAgent targetSightAgent, bool isInvisible)
		{
			return true;
		}
	}
}