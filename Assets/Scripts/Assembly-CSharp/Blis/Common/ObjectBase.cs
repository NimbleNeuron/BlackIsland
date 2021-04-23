using System;
using UnityEngine;

namespace Blis.Common
{
	public abstract class ObjectBase : MonoBehaviour
	{
		protected int objectId { get; set; }


		public int ObjectId => objectId;


		public ObjectType ObjectType => GetObjectType();


		public int TeamNumber => GetTeamNumber();


		public ColliderAgent ColliderAgent => GetColliderAgent();


		protected abstract ObjectType GetObjectType();


		protected abstract int GetTeamNumber();


		protected abstract ColliderAgent GetColliderAgent();


		public Vector3 GetPosition()
		{
			return transform.position;
		}


		public virtual void SetPosition(Vector3 position)
		{
			transform.position = position;
		}


		public Quaternion GetRotation()
		{
			return transform.rotation;
		}


		public void SetRotation(Quaternion rot)
		{
			transform.rotation = rot;
		}


		public void InitObject(int objectId)
		{
			this.objectId = objectId;
		}


		public bool IsTypeOf<T>() where T : ObjectBase
		{
			return this is T || GetType().IsSubclassOf(typeof(T));
		}


		public void IfTypeOf<T>(Action<T> callback) where T : ObjectBase
		{
			if (IsTypeOf<T>())
			{
				callback((T) this);
			}
		}


		public virtual void RemoveAllAttachedSight() { }
	}
}