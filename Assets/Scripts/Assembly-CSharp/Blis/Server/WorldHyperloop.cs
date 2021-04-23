using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.Hyperloop)]
	public class WorldHyperloop : WorldObject
	{
		
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
			return this.colliderAgent;
		}

		
		public void Init()
		{
			GameUtil.BindOrAdd<CubeColliderAgent>(base.gameObject, ref this.colliderAgent);
			this.colliderAgent.Init(1f);
		}

		
		public void SetUseFlag()
		{
			this.lastUsedTime = Time.realtimeSinceStartup;
		}

		
		public bool IsReadyToHyperLoop()
		{
			return true;
		}

		
		public void CastingHyperLoopAction()
		{
			base.EnqueueCommand(new CmdHyperLoopAction());
		}

		
		public void CancelHyperLoopAction()
		{
			base.EnqueueCommand(new CmdCancelHyperLoopAction());
		}

		
		protected override SkillAgent GetSkillAgent()
		{
			return null;
		}

		
		public override byte[] CreateSnapshot()
		{
			return null;
		}

		
		private CubeColliderAgent colliderAgent;

		
		private float lastUsedTime = -1f;
	}
}
