using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public class ServerSightAgent : SightAgent
	{
		
		
		public float DestroyTime
		{
			get
			{
				return this.destroyTime;
			}
		}

		
		
		public bool IsRemoveWhenInvisibleStart
		{
			get
			{
				return this.isRemoveWhenInvisibleStart;
			}
		}

		
		
		public WorldObject Target
		{
			get
			{
				return this.target;
			}
		}

		
		public override void InitCharacterSight(ObjectBase target)
		{
			base.InitCharacterSight(target);
			this.target = (target as WorldObject);
		}

		
		public override void InitAttachSight(ObjectBase target, int attachSightId)
		{
			base.InitAttachSight(target, attachSightId);
			this.target = (target as WorldObject);
			this.target.AddAttachedSight(this);
		}

		
		protected override void OnDisable()
		{
			base.OnDisable();
			if (this.isAttachMode)
			{
				this.target.RemoveAttachedSight(this);
			}
		}

		
		public void DelayDestroy(float duration, Action callback)
		{
			this.StopDestroy();
			this.destroyTime = duration + MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			this.coroutine = base.StartCoroutine(CoroutineUtil.DelayedAction(duration, delegate()
			{
				Action callback2 = callback;
				if (callback2 != null)
				{
					callback2();
				}
				this.Destroy();
			}));
		}

		
		public void Destroy()
		{
			UnityEngine.Object.Destroy(this);
		}

		
		public void StopDestroy()
		{
			this.destroyTime = 0f;
			if (this.coroutine != null)
			{
				base.StopCoroutine(this.coroutine);
			}
		}

		
		public List<T> FindAllInAllySights<T>() where T : ObjectBase
		{
			List<T> list = this.FindAllInSight<T>();
			if (base.UseOtherSight)
			{
				for (int i = this.allySights.Count - 1; i >= 0; i--)
				{
					if (this.allySights[i] == null)
					{
						this.allySights.RemoveAt(i);
					}
					else
					{
						list.AddRange(((ServerSightAgent)this.allySights[i]).FindAllInSightForAlly<T>());
					}
				}
				for (int j = this.attachSights.Count - 1; j >= 0; j--)
				{
					if (this.attachSights[j] == null)
					{
						this.attachSights.RemoveAt(j);
					}
					else
					{
						list.AddRange(((ServerSightAgent)this.attachSights[j]).FindAllInSightForAlly<T>());
					}
				}
			}
			HashSet<T> hashSet = new HashSet<T>();
			for (int k = list.Count - 1; k >= 0; k--)
			{
				if (hashSet.Contains(list[k]))
				{
					list.RemoveAt(k);
				}
				else
				{
					hashSet.Add(list[k]);
				}
			}
			return list;
		}

		
		private List<T> FindAllInSight<T>() where T : ObjectBase
		{
			List<T> list = base.FindObjects<T>(base.Position, base.SightRange);
			for (int i = list.Count - 1; i >= 0; i--)
			{
				WorldCharacter worldCharacter = list[i] as WorldCharacter;
				if (worldCharacter != null)
				{
					if (!this.IsInSight(worldCharacter.SightAgent, worldCharacter.GetPosition(), worldCharacter.Stat.Radius, worldCharacter.SightAgent.IsInvisibleCheckWithMemorizer(base.SightOwnerId)))
					{
						list.RemoveAt(i);
					}
				}
				else
				{
					SightAgent component = list[i].GetComponent<SightAgent>();
					if (!this.IsInSight(component, list[i].GetPosition(), 0f, false))
					{
						list.RemoveAt(i);
					}
				}
			}
			return list;
		}

		
		private List<T> FindAllInSightForAlly<T>() where T : ObjectBase
		{
			List<T> list = base.FindObjects<T>(base.Position, base.SightRange);
			for (int i = list.Count - 1; i >= 0; i--)
			{
				WorldCharacter worldCharacter = list[i] as WorldCharacter;
				if (worldCharacter != null)
				{
					if (!base.IsInSightToAlly(worldCharacter.SightAgent, worldCharacter.GetPosition(), worldCharacter.Stat.Radius, worldCharacter.SightAgent.IsInvisibleCheckWithMemorizer(base.SightOwnerId)))
					{
						list.RemoveAt(i);
					}
				}
				else
				{
					SightAgent component = list[i].GetComponent<SightAgent>();
					if (!base.IsInSightToAlly(component, list[i].GetPosition(), 0f, false))
					{
						list.RemoveAt(i);
					}
				}
			}
			return list;
		}

		
		public bool AddMemorizedTarget(int objectId)
		{
			if (this.IsMemorizedTarget(objectId))
			{
				return false;
			}
			this.memorizedCharacterList.Add(objectId);
			return true;
		}

		
		public bool IsMemorizedTarget(int objectId)
		{
			return this.memorizedCharacterList.Contains(objectId);
		}

		
		public void SetIsRemoveWhenInvisibleStart(bool isRemoveWhenInvisibleStart)
		{
			this.isRemoveWhenInvisibleStart = isRemoveWhenInvisibleStart;
		}

		
		public bool IsInAllySightWithoutWall(float sightRange, SightAgent targetSightAgent, Vector3 targetPos, float radius, bool isInvisible)
		{
			if (this.IsInSightWithoutWall(sightRange, targetSightAgent, targetPos, radius, isInvisible))
			{
				return true;
			}
			if (!base.UseOtherSight)
			{
				return false;
			}
			if (base.IsInSightToAttachSights(targetSightAgent, targetPos, radius, isInvisible))
			{
				return true;
			}
			for (int i = 0; i < this.allySights.Count; i++)
			{
				SightAgent sightAgent = this.allySights[i];
				if (sightAgent == null)
				{
					this.allySights.RemoveAt(i--);
				}
				else
				{
					if (sightAgent.IsInSightToAlly(targetSightAgent, targetPos, radius, isInvisible))
					{
						return true;
					}
					if (sightAgent.IsInSightToAllySights(targetSightAgent, targetPos, radius, isInvisible))
					{
						return true;
					}
					if (sightAgent.IsInSightToAttachSights(targetSightAgent, targetPos, radius, isInvisible))
					{
						return true;
					}
				}
			}
			return false;
		}

		
		private bool IsInSightWithoutWall(float sightRange, SightAgent targetSightAgent, Vector3 targetPos, float targetRadius, bool isInvisible)
		{
			if (isInvisible && base.IsOwnAgent(targetSightAgent))
			{
				isInvisible = false;
			}
			return base.IsVisible(isInvisible) && SightAgent.IsInSightWithoutWall(base.Position, base.Forward, sightRange, base.SightAngle, targetPos, targetRadius);
		}

		
		public ServerSightAgent SightAgentPositionCheckOverlap(Vector3 position, float sightRange, float duration)
		{
			for (int i = 0; i < this.attachSights.Count; i++)
			{
				if (this.attachSights[i].SightRange == sightRange)
				{
					ServerSightAgent serverSightAgent = this.attachSights[i] as ServerSightAgent;
					if (!(serverSightAgent == null))
					{
						WorldSightObject worldSightObject = serverSightAgent.target as WorldSightObject;
						if (!(worldSightObject == null) && !(worldSightObject.GetPosition() != position) && worldSightObject.DestroyTime < duration + MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime)
						{
							return serverSightAgent;
						}
					}
				}
			}
			return null;
		}

		
		private float destroyTime;

		
		private Coroutine coroutine;

		
		private HashSet<int> memorizedCharacterList = new HashSet<int>();

		
		private bool isRemoveWhenInvisibleStart;

		
		private WorldObject target;
	}
}
