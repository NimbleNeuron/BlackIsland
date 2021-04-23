using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blis.Common
{
	public abstract class SightAgent : MonoBehaviour
	{
		private readonly Collider[] colliderBuffer = new Collider[100];


		protected List<SightAgent> allySights = new List<SightAgent>();


		private int attachSightId;


		protected List<SightAgent> attachSights = new List<SightAgent>();


		private readonly Dictionary<BlockedSightType, bool> blockedSightCase =
			new Dictionary<BlockedSightType, bool>(SingletonComparerEnum<BlockedSightTypeComparer, BlockedSightType>
				.Instance);


		protected bool isAttachMode;


		private bool isDetect;


		private bool isDetectShare;


		protected bool isInvisible;


		private readonly HashSet<int> memorizerPlayerList = new HashSet<int>();


		private int objectId;


		private SightAgent ownerAgent;


		[Range(0f, 360f)] private int sightAngle = 360;


		private int sightOwnerId;


		private float sightRange;


		private readonly Dictionary<Type, float> targetTypeInSightRange = new Dictionary<Type, float>();


		public int AttachSightId => attachSightId;


		public int ObjectId => objectId;


		public int SightOwnerId {
			get
			{
				if (ownerAgent != null)
				{
					return ownerAgent.sightOwnerId;
				}

				return sightOwnerId;
			}
		}


		protected Vector3 Position => transform.position;


		protected Vector3 Forward => transform.TransformDirection(Vector3.forward);


		public bool IsInvisible => isInvisible;


		public float SightRange => sightRange;


		public int SightAngle => sightAngle;


		public bool IsDetectShare => isDetectShare;


		public bool IsDetect => isDetect;


		public bool UseOtherSight => blockedSightCase.Count == 0;


		protected virtual void OnEnable()
		{
			Register();
		}


		protected virtual void OnDisable()
		{
			Unregister();
		}


		public virtual void InitCharacterSight(ObjectBase target)
		{
			objectId = target.ObjectId;
			sightOwnerId = objectId;
			attachSightId = -1;
			isAttachMode = false;
		}


		public virtual void InitAttachSight(ObjectBase target, int attachSightId)
		{
			objectId = target.ObjectId;
			sightOwnerId = objectId;
			this.attachSightId = attachSightId;
			isAttachMode = true;
		}


		public void SetOwner(SightAgent owner)
		{
			Unregister();
			if (this == owner || owner.ownerAgent == this)
			{
				return;
			}

			ownerAgent = owner;
			Register();
		}


		public void SetIsInvisible(bool isInvisible)
		{
			this.isInvisible = isInvisible;
		}


		public SightAgent GetOwner()
		{
			return ownerAgent;
		}


		public virtual void BlockAllySight(BlockedSightType blockedSightType, bool block)
		{
			if (block)
			{
				blockedSightCase[blockedSightType] = true;
				return;
			}

			if (blockedSightCase.ContainsKey(blockedSightType))
			{
				blockedSightCase.Remove(blockedSightType);
			}
		}


		private void Register()
		{
			if (!isAttachMode)
			{
				if (ownerAgent != null)
				{
					ownerAgent.AddAllySight(this);
				}
			}
			else if (ownerAgent != null)
			{
				ownerAgent.AddAttachSight(this);
			}
		}


		private void Unregister()
		{
			if (!isAttachMode)
			{
				if (ownerAgent != null)
				{
					ownerAgent.RemoveAllySight(this);
				}
			}
			else if (ownerAgent != null)
			{
				ownerAgent.RemoveAttachSight(this);
			}
		}


		public void SetDetect(bool detectShare, bool isDetect)
		{
			isDetectShare = detectShare;
			this.isDetect = isDetect;
		}


		public virtual void UpdateSightRange(float sightRange)
		{
			this.sightRange = sightRange;
		}


		public virtual void UpdateSightAngle(int degree)
		{
			sightAngle = Mathf.Clamp(degree, 0, 360);
		}


		public void UpdateTargetTypeInSightRange(Type type, float sightRange)
		{
			targetTypeInSightRange[type] = sightRange;
		}


		public void RemoveTargetTypeInSightRange(Type type)
		{
			targetTypeInSightRange.Remove(type);
		}


		public float GetTargetTypeInSightRange(Type type)
		{
			float result;
			if (!targetTypeInSightRange.TryGetValue(type, out result))
			{
				result = sightRange;
			}

			return result;
		}


		public virtual void AddAllySight(SightAgent agent)
		{
			if (!allySights.Contains(agent))
			{
				allySights.Add(agent);
			}
		}


		public virtual void RemoveAllySight(SightAgent agent)
		{
			allySights.Remove(agent);
		}


		public virtual void AddAttachSight(SightAgent agent)
		{
			if (!attachSights.Contains(agent))
			{
				attachSights.Add(agent);
			}
		}


		public virtual void RemoveAttachSight(SightAgent agent)
		{
			attachSights.Remove(agent);
		}


		protected List<T> FindObjects<T>(Vector3 position, float range) where T : ObjectBase
		{
			int num = Physics.OverlapSphereNonAlloc(position, range, colliderBuffer,
				GameConstants.LayerMask.WORLD_OBJECT_LAYER);
			List<T> list = new List<T>(num);
			for (int i = 0; i < num; i++)
			{
				T component = colliderBuffer[i].GetComponent<T>();
				if (!(component == null))
				{
					list.Add(component);
				}
			}

			return list;
		}


		public virtual bool IsInAllySight(SightAgent targetSightAgent, Vector3 targetPos, float radius,
			bool isInvisible)
		{
			if (IsInSight(targetSightAgent, targetPos, radius, isInvisible))
			{
				return true;
			}

			if (!UseOtherSight)
			{
				return false;
			}

			for (int i = attachSights.Count - 1; i >= 0; i--)
			{
				SightAgent sightAgent = attachSights[i];
				if (sightAgent == null)
				{
					attachSights.RemoveAt(i);
				}
				else if (sightAgent.IsInSightToAlly(targetSightAgent, targetPos, radius, isInvisible))
				{
					return true;
				}
			}

			for (int j = 0; j < allySights.Count; j++)
			{
				SightAgent sightAgent2 = allySights[j];
				if (sightAgent2 == null)
				{
					allySights.RemoveAt(j--);
				}
				else
				{
					if (sightAgent2.IsInSightToAlly(targetSightAgent, targetPos, radius, isInvisible))
					{
						return true;
					}

					for (int k = 0; k < sightAgent2.allySights.Count; k++)
					{
						SightAgent sightAgent3 = sightAgent2.allySights[k];
						if (sightAgent3 == null)
						{
							sightAgent2.allySights.RemoveAt(k--);
						}
						else if (sightAgent3.IsInSightToAlly(targetSightAgent, targetPos, radius, isInvisible))
						{
							return true;
						}
					}

					for (int l = 0; l < sightAgent2.attachSights.Count; l++)
					{
						SightAgent sightAgent4 = sightAgent2.attachSights[l];
						if (sightAgent4 == null)
						{
							sightAgent2.attachSights.RemoveAt(l--);
						}
						else if (sightAgent4.IsInSightToAlly(targetSightAgent, targetPos, radius, isInvisible))
						{
							return true;
						}
					}
				}
			}

			return false;
		}


		public virtual bool IsInSight(SightAgent targetSightAgent, Vector3 targetPos, float targetRadius,
			bool isInvisible)
		{
			if (isInvisible && IsOwnAgent(targetSightAgent))
			{
				isInvisible = false;
			}

			return IsVisible(isInvisible) &&
			       IsInSight(Position, Forward, sightRange, sightAngle, targetPos, targetRadius);
		}


		public bool IsVisible(bool isInvisible)
		{
			return !isInvisible || isDetect;
		}


		public bool IsInSightToAlly(SightAgent targetSightAgent, Vector3 targetPos, float targetRadius,
			bool isInvisible)
		{
			if (isInvisible && IsOwnAgent(targetSightAgent))
			{
				isInvisible = false;
			}

			return IsVisibleToAlly(isInvisible) &&
			       IsInSight(Position, Forward, sightRange, sightAngle, targetPos, targetRadius);
		}


		protected bool IsVisibleToAlly(bool isInvisible)
		{
			return !isInvisible || isDetect && isDetectShare;
		}


		public virtual bool IsInSight(Vector3 targetPos, float targetRadius)
		{
			return IsInSight(Position, Forward, sightRange, sightAngle, targetPos, targetRadius);
		}


		public static bool IsInSight(Vector3 position, Vector3 forward, float sightRange, int sightAngle,
			Vector3 targetPos, float targetRadius)
		{
			Vector3 vector = targetPos - position;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			float num = magnitude - Mathf.Max(0f, targetRadius);
			if (sightRange - num < -Mathf.Epsilon)
			{
				return false;
			}

			Vector3 normalized = vector.normalized;
			if (sightAngle < 360 && vector.sqrMagnitude > 1f &&
			    Vector3.Dot(forward, normalized) <= Mathf.Cos(sightAngle * 0.5f * 0.017453292f))
			{
				return false;
			}

			position.y += 1.5f;
			RaycastHit raycastHit;
			return !Physics.Raycast(position, normalized, out raycastHit, magnitude,
				GameConstants.LayerMask.SIGHT_OBSTACLE_LAYER);
		}


		public static bool IsInSightWithoutWall(Vector3 position, Vector3 forward, float sightRange, int sightAngle,
			Vector3 targetPos, float targetRadius)
		{
			Vector3 vector = targetPos - position;
			vector.y = 0f;
			float num = vector.magnitude - Mathf.Max(0f, targetRadius);
			if (sightRange - num < -Mathf.Epsilon)
			{
				return false;
			}

			Vector3 normalized = vector.normalized;
			return sightAngle >= 360 || vector.sqrMagnitude <= 1f ||
			       Vector3.Dot(forward, normalized) > Mathf.Cos(sightAngle * 0.5f * 0.017453292f);
		}


		protected bool IsOwnAgent(SightAgent targetSightAgent)
		{
			return targetSightAgent != null &&
			       (targetSightAgent.ObjectId == ObjectId || targetSightAgent.SightOwnerId == SightOwnerId);
		}


		public bool IsInvisibleCheckWithMemorizer(int memorizerObjectId)
		{
			return isInvisible && !memorizerPlayerList.Contains(memorizerObjectId);
		}


		public void AddMemorizerPlayer(int objectId)
		{
			memorizerPlayerList.Add(objectId);
		}


		public bool IsAlly(int targetObjectId)
		{
			for (int i = 0; i < allySights.Count; i++)
			{
				if (allySights[i].ObjectId == targetObjectId)
				{
					return true;
				}

				for (int j = 0; j < allySights[i].allySights.Count; j++)
				{
					if (allySights[i].allySights[j].ObjectId == targetObjectId)
					{
						return true;
					}
				}
			}

			return false;
		}


		public bool IsAttachSight(int targetObjectId)
		{
			for (int i = 0; i < attachSights.Count; i++)
			{
				if (attachSights[i].ObjectId == targetObjectId)
				{
					return true;
				}
			}

			return false;
		}


		public bool IsInSightToAllySights(SightAgent targetSightAgent, Vector3 targetPos, float targetRadius,
			bool isInvisible)
		{
			for (int i = 0; i < allySights.Count; i++)
			{
				SightAgent sightAgent = allySights[i];
				if (sightAgent == null)
				{
					allySights.RemoveAt(i--);
				}
				else if (sightAgent.IsInSightToAlly(targetSightAgent, targetPos, targetRadius, isInvisible))
				{
					return true;
				}
			}

			return false;
		}


		public bool IsInSightToAttachSights(SightAgent targetSightAgent, Vector3 targetPos, float targetRadius,
			bool isInvisible)
		{
			for (int i = 0; i < attachSights.Count; i++)
			{
				SightAgent sightAgent = attachSights[i];
				if (sightAgent == null)
				{
					attachSights.RemoveAt(i--);
				}
				else if (sightAgent.IsInSightToAlly(targetSightAgent, targetPos, targetRadius, isInvisible))
				{
					return true;
				}
			}

			return false;
		}
	}
}