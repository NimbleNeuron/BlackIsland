using BIFog;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class BushEffect : MonoBehaviour
	{
		private const string tBushIn = "tBushIn";


		private const string tBushOut = "tBushOut";


		public Animator animator;


		public Renderer targetRenderer;


		public Material normalMaterial;


		public Material transferMaterial;


		private readonly Collider[] colliderBuffer = new Collider[20];


		private bool bushIn;


		private bool transparent;

		private void Awake() { }


		private void LateUpdate()
		{
			int num = Physics.OverlapSphereNonAlloc(transform.position, 0.8f, colliderBuffer,
				GameConstants.LayerMask.WORLD_OBJECT_LAYER);
			bool flag = false;
			bool inOutAni = false;
			for (int i = 0; i < num; i++)
			{
				FogHiderOnCenter component = colliderBuffer[i].GetComponent<FogHiderOnCenter>();
				if (component != null && component.IsInSight)
				{
					LocalCharacter component2 = component.GetComponent<LocalCharacter>();
					if (component2 != null && !component2.IsInvisible)
					{
						inOutAni = true;
						Vector3 position = component.transform.position;
						Vector3 start = position - new Vector3(0f, 10f, 0f);
						Vector3 end = position + new Vector3(0f, 1f, 0f);
						if (Physics.Linecast(start, end, GameConstants.LayerMask.BUSH_LAYER))
						{
							flag = true;
							MonoBehaviourInstance<BushManager>.inst.RegisterInBush(component2.ObjectId);
						}
					}
				}
			}

			SetTransparent(flag);
			SetInOutAni(inOutAni);
		}


		private void SetInOutAni(bool isIn)
		{
			if (bushIn != isIn)
			{
				animator.SetTrigger(isIn ? "tBushIn" : "tBushOut");
				bushIn = isIn;
			}
		}


		private void SetTransparent(bool enable)
		{
			if (transparent != enable)
			{
				targetRenderer.sharedMaterial = enable ? transferMaterial : normalMaterial;
				transparent = enable;
			}
		}


		public void OnCameraIn()
		{
			gameObject.SetActive(true);
		}


		public void OnCameraOut()
		{
			gameObject.SetActive(false);
		}
	}
}