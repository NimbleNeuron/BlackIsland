using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blis.Common
{
	
	public class CharacterSpawnPoint : MonoBehaviour
	{
		
		
		public int AreaCode
		{
			get
			{
				return this.areaCode;
			}
		}

		
		
		public bool UseCharacterSpawn
		{
			get
			{
				return this.useCharacterSpawn;
			}
		}

		
		
		public bool IsUsed
		{
			get
			{
				return this.isUsed;
			}
		}

		
		
		public IEnumerable<CharacterSpawnPoint> NotUsedChildPoints
		{
			get
			{
				return from x in this.childPoints
				where !x.isUsed
				select x;
			}
		}

		
		
		public int UsedChildCount
		{
			get
			{
				return this.childPoints.Count((CharacterSpawnPoint x) => x.isUsed);
			}
		}

		
		
		public bool AllUsedChildPoints
		{
			get
			{
				if (this.childPoints.Count > 0)
				{
					return this.childPoints.All((CharacterSpawnPoint x) => x.isUsed);
				}
				return true;
			}
		}

		
		public void Awake()
		{
			this.childPoints.Clear();
			CharacterSpawnPoint[] componentsInChildren = base.transform.GetComponentsInChildren<CharacterSpawnPoint>();
			if (componentsInChildren != null && componentsInChildren.Length != 0)
			{
				this.childPoints.AddRange(componentsInChildren);
			}
		}

		
		public void SetIsUsed(bool isUsed)
		{
			this.isUsed = isUsed;
		}

		
		public Vector3 GetPosition()
		{
			if (this.position == null)
			{
				this.position = new Vector3?(base.transform.position);
			}
			return this.position.Value;
		}

		
		private void OnDrawGizmos()
		{
			if (base.transform.parent.GetComponent<CharacterSpawnPoint>() != null)
			{
				Gizmos.color = new Color(1f, 0.3f, 0f, 1f);
				Gizmos.DrawSphere(base.transform.position, 0.3f);
				return;
			}
			if (this.useCharacterSpawn)
			{
				Gizmos.color = new Color(1f, 0f, 1f, 1f);
				Gizmos.DrawSphere(base.transform.position, 0.4f);
				return;
			}
			Gizmos.color = new Color(0f, 1f, 0f, 1f);
			Gizmos.DrawSphere(base.transform.position, 0.4f);
		}

		
		public CharacterSpawnPoint GetRandomPointNotCheckUse()
		{
			if (this.childPoints.Count <= 0)
			{
				return this;
			}
			int num = UnityEngine.Random.Range(0, this.childPoints.Count + 1);
			if (num != this.childPoints.Count + 1)
			{
				return this.childPoints[num];
			}
			return this;
		}

		
		[SerializeField]
		private int areaCode = default;

		
		[SerializeField]
		private bool useCharacterSpawn = default;

		
		private bool isUsed = default;

		
		private Vector3? position = default;

		
		private readonly List<CharacterSpawnPoint> childPoints = new List<CharacterSpawnPoint>();
	}
}
