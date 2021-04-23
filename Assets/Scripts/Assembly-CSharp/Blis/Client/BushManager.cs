using System.Collections.Generic;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class BushManager : MonoBehaviourInstance<BushManager>
	{
		public delegate void BushEvent(bool inBush, int objectId);


		private readonly List<BushEffect> bushs = new List<BushEffect>();


		private readonly HashSet<int> inBushCharacter = new HashSet<int>();


		private readonly HashSet<int> preInBushCharacter = new HashSet<int>();


		private BoundingSphere[] colliderBuffer;


		private CullingGroup cullingGroup;


		private void LateUpdate()
		{
			foreach (int num in inBushCharacter)
			{
				if (!preInBushCharacter.Contains(num))
				{
					OnBushEvevnt(true, num);
				}
			}

			foreach (int num2 in preInBushCharacter)
			{
				if (!inBushCharacter.Contains(num2))
				{
					OnBushEvevnt(false, num2);
				}
			}

			preInBushCharacter.Clear();
			preInBushCharacter.UnionWith(inBushCharacter);
			inBushCharacter.Clear();
		}

		
		
		public event BushEvent OnBushEvevnt = delegate { };


		protected override void _Awake()
		{
			cullingGroup = new CullingGroup();
			GetComponentsInChildren<BushEffect>(true, bushs);
			bushs.ForEach(delegate(BushEffect x) { x.OnCameraOut(); });
			colliderBuffer = new BoundingSphere[bushs.Count];
			for (int i = 0; i < bushs.Count; i++)
			{
				colliderBuffer[i].radius = 2f;
				colliderBuffer[i].position = bushs[i].transform.position;
			}

			cullingGroup.SetBoundingSpheres(colliderBuffer);
			cullingGroup.SetBoundingSphereCount(colliderBuffer.Length);
			cullingGroup.onStateChanged = StateChangedMethod;
			cullingGroup.targetCamera = Camera.main;
		}


		protected override void _OnDestroy()
		{
			base._OnDestroy();
			cullingGroup.Dispose();
			cullingGroup = null;
		}


		private void StateChangedMethod(CullingGroupEvent evt)
		{
			if (evt.hasBecomeVisible)
			{
				bushs[evt.index].OnCameraIn();
				return;
			}

			if (evt.hasBecomeInvisible)
			{
				bushs[evt.index].OnCameraOut();
			}
		}


		public void RegisterInBush(int objectId)
		{
			inBushCharacter.Add(objectId);
		}
	}
}