using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.ResourceItemBox)]
	public class WorldResourceItemBox : WorldItemBox
	{
		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.ResourceItemBox;
		}

		
		
		public bool IsCollected
		{
			get
			{
				return this.collected;
			}
		}

		
		
		public bool IsCooldown
		{
			get
			{
				return MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime < this.cooldownUntil;
			}
		}

		
		
		public int ResourceDataCode
		{
			get
			{
				return this.resourceDataCode;
			}
		}

		
		public void Init(int itemSpawnPointCode, int resourceDataCode, int areaCode, float initSpawnTime)
		{
			base.Init(itemSpawnPointCode, 0);
			this.resourceDataCode = resourceDataCode;
			this.collected = false;
			this.cooldownUntil = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			this.initSpawnTime = initSpawnTime;
			this.areaCode = areaCode;
			if (initSpawnTime > 0f)
			{
				MonoBehaviourInstance<GameService>.inst.Spawn.listLifeoftrees.Add(this);
			}
		}

		
		public override byte[] CreateSnapshot()
		{
			return WorldObject.serializer.Serialize<ResourceItemBoxSnapshot>(new ResourceItemBoxSnapshot
			{
				cooldownUntil = new BlisFixedPoint(this.cooldownUntil)
			});
		}

		
		public void StartCooldown()
		{
			this.StartCooldown(-1f);
		}

		
		public void StartCooldown(float cooldown)
		{
			if (cooldown == -1f)
			{
				this.cooldownUntil = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime + this.initSpawnTime;
				cooldown = this.initSpawnTime;
			}
			else
			{
				this.cooldownUntil = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime + cooldown;
			}
			base.EnqueueCommand(new CmdUpdateResourceBoxCooldown
			{
				cooldown = new BlisFixedPoint(cooldown)
			});
			if (this.ResourceDataCode == 7)
			{
				float num = 60f;
				float num2 = cooldown - num;
				base.StartCoroutine(CoroutineUtil.DelayedAction(num2 - 0.5f, delegate()
				{
					base.EnqueueCommand(new CmdResourceBoxChildReady());
				}));
				base.StartCoroutine(CoroutineUtil.DelayedAction(cooldown - 0.5f, delegate()
				{
					base.EnqueueCommand(new CmdResourceBoxChildActive());
				}));
			}
		}

		
		public void Collected()
		{
			this.collected = true;
		}

		
		public override Vector3 InteractDirection(Vector3 position)
		{
			CastingActionType castingActionType = MonoBehaviourInstance<GameService>.inst.CurrentLevel.GetCollectibleData(this.ResourceDataCode).castingActionType;
			if (castingActionType - CastingActionType.CollectibleOpenSeaFish <= 1)
			{
				return this.GetColliderAgent().transform.forward;
			}
			return base.InteractDirection(position);
		}

		
		private bool collected;

		
		private float cooldownUntil;

		
		private float initSpawnTime;

		
		private int resourceDataCode;

		
		private int areaCode;
	}
}
