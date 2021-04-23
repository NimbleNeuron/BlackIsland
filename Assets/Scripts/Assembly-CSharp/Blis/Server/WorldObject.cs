using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public abstract class WorldObject : ObjectBase
	{
		
		
		public SkillAgent SkillAgent
		{
			get
			{
				return this.GetSkillAgent();
			}
		}

		
		protected abstract SkillAgent GetSkillAgent();

		
		
		public virtual int WalkableNavMask
		{
			get
			{
				return 2147483640;
			}
		}

		
		protected virtual void OnPreFrameUpdate()
		{
		}

		
		protected virtual void OnFrameUpdate()
		{
		}

		
		public void InitObject(int objectId, GameWorld world)
		{
			base.InitObject(objectId);
			this.interactionPointAgent = base.GetComponent<InteractionPointAgent>();
			this.world = world;
		}

		
		protected virtual void OnDestroy()
		{
			GameWorld gameWorld = this.world;
			if (gameWorld != null)
			{
				gameWorld.RemoveOnDestroy(this);
			}
			this.RemoveAllSightRangeLink();
		}

		
		public virtual bool IsAttackable(WorldCharacter target)
		{
			return false;
		}

		
		public Vector3 GetInteractionPoint(Vector3 currentPosition)
		{
			Vector3 result;
			MoveAgent.SamplePosition((this.interactionPointAgent != null) ? this.interactionPointAgent.GetInteractionPoint() : this.GetColliderAgent().ClosestPointOnBounds(currentPosition), this.WalkableNavMask, out result);
			return result;
		}

		
		protected void EnqueueCommand(CommandPacket commandPacket)
		{
			if (commandPacket == null)
			{
				return;
			}
			if (commandPacket is ObjectCommandPacket)
			{
				(commandPacket as ObjectCommandPacket).objectId = this.objectId;
			}
			if (commandPacket is MoveCommandPacket)
			{
				(commandPacket as MoveCommandPacket).position = new BlisVector(base.GetPosition());
			}
			MonoBehaviourInstance<GameServer>.inst.EnqueueCommand(commandPacket);
		}

		
		public void PreFrameUpdate()
		{
			this.OnPreFrameUpdate();
		}

		
		public void FrameUpdate()
		{
			this.OnFrameUpdate();
		}

		
		public virtual void DestroySelf()
		{
			UnityEngine.Object.Destroy(this);
			this.EnqueueCommand(new CmdDestroy());
		}

		
		
		public virtual IItemBox ItemBox
		{
			get
			{
				return this.GetItemBox();
			}
		}

		
		protected virtual IItemBox GetItemBox()
		{
			return null;
		}

		
		public abstract byte[] CreateSnapshot();

		
		public SnapshotWrapper CreateSnapshotWrapper()
		{
			return new SnapshotWrapper
			{
				objectType = this.GetObjectType(),
				objectId = this.objectId,
				position = base.GetPosition(),
				rotation = base.GetRotation(),
				snapshot = this.CreateSnapshot()
			};
		}

		
		public virtual void LookAt(Vector3 lookTo, float duration = 0f, bool isServerRotateInstant = false)
		{
			if (1E-45f < Vector3.Cross(base.transform.forward, lookTo).sqrMagnitude)
			{
				Quaternion rotation = GameUtil.LookRotation(lookTo);
				this.EnqueueCommand(new CmdLookAt
				{
					lookAtFromY = new BlisFixedPoint(base.transform.eulerAngles.y),
					lookAtToY = new BlisFixedPoint(rotation.eulerAngles.y),
					duration = new BlisFixedPoint(duration)
				});
				base.SetRotation(rotation);
			}
		}

		
		public void AddAttachedSight(ServerSightAgent sightAgent)
		{
			if (this.attachedSights == null)
			{
				this.attachedSights = new List<ServerSightAgent>();
			}
			this.attachedSights.Add(sightAgent);
		}

		
		public void RemoveAttachedSight(ServerSightAgent sightAgent)
		{
			if (this.attachedSights == null)
			{
				return;
			}
			this.attachedSights.Remove(sightAgent);
		}

		
		public override void RemoveAllAttachedSight()
		{
			if (this.attachedSights == null)
			{
				return;
			}
			for (int i = 0; i < this.attachedSights.Count; i++)
			{
				if (!(this.attachedSights[i] == null))
				{
					SightAgent owner = this.attachedSights[i].GetOwner();
					if (owner != null)
					{
						owner.RemoveAttachSight(this.attachedSights[i]);
					}
					UnityEngine.Object.DestroyImmediate(this.attachedSights[i]);
				}
			}
			this.attachedSights.Clear();
		}

		
		public virtual Vector3 InteractDirection(Vector3 position)
		{
			return GameUtil.DirectionOnPlane(position, base.GetPosition());
		}

		
		public void AddSightRangeLink(SightRangeLink addSightRangeLink)
		{
			if (this.sightRangeLinks == null)
			{
				this.sightRangeLinks = new List<SightRangeLink>();
			}
			this.sightRangeLinks.Add(addSightRangeLink);
			this.isDirty = true;
		}

		
		public void RemoveSightRangeLink(SightRangeLink removeSightRangeLink)
		{
			if (this.sightRangeLinks == null)
			{
				return;
			}
			this.sightRangeLinks.Remove(removeSightRangeLink);
			this.isDirty = true;
		}

		
		public List<SightRangeLink> GetSightRangeLinks()
		{
			if (this.getSightRangeLinks == null)
			{
				this.getSightRangeLinks = new List<SightRangeLink>();
			}
			if (this.sightRangeLinks == null)
			{
				return this.getSightRangeLinks;
			}
			if (this.isDirty)
			{
				this.getSightRangeLinks.Clear();
				foreach (SightRangeLink item in this.sightRangeLinks)
				{
					this.getSightRangeLinks.Add(item);
				}
				this.isDirty = false;
			}
			return this.getSightRangeLinks;
		}

		
		protected void RemoveAllSightRangeLink()
		{
			if (this.sightRangeLinks != null)
			{
				for (int i = this.sightRangeLinks.Count - 1; i >= 0; i--)
				{
					this.sightRangeLinks[i].RemoveSightRangeLink();
				}
				this.isDirty = true;
			}
		}

		
		protected static ISerializer serializer = Serializer.Default;

		
		protected List<SightRangeLink> sightRangeLinks;

		
		private GameWorld world;

		
		private InteractionPointAgent interactionPointAgent;

		
		[NonSerialized]
		public List<ServerSightAgent> attachedSights;

		
		private List<SightRangeLink> getSightRangeLinks;

		
		private bool isDirty;
	}
}
