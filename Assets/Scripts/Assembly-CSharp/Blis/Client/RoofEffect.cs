using System;
using System.Collections;
using System.Collections.Generic;
using BIFog;
using BIOutline;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.Rendering;

namespace Blis.Client
{
	public class RoofEffect : MonoBehaviour
	{
		private const float CHECK_SQR_DISTANCE = 4900f;


		private const float ShowRoofDelayTime = 0.5f;


		private const float HideRoofDelayTime = 0.2f;


		private const int CacheSize = 50;


		public List<BoxCollider> insideColiders;


		public List<Renderer> roofs;


		private readonly List<FogHiderOnCenter> doors = new List<FogHiderOnCenter>();


		private readonly List<Outliner> outliners = new List<Outliner>();


		private readonly Dictionary<Collider, FogSight> sightCache = new Dictionary<Collider, FogSight>();


		private Camera cam;


		private Collider[] cols = new Collider[50];


		private Coroutine routine;


		private bool showRoof;

		private void Awake()
		{
			cam = Camera.main;
			GetComponentsInChildren<Outliner>(outliners);
			GetComponentsInChildren<FogHiderOnCenter>(doors);
			doors.ForEach(delegate(FogHiderOnCenter x)
			{
				if (x.GetComponent<Collider>() == null)
				{
					x.gameObject.AddComponent<SphereCollider>();
				}
			});
			if (insideColiders.Exists(x => x == null))
			{
				Log.E(name + "'s RoofEffect InsideCollider Setting Error");
				enabled = false;
				return;
			}

			if (roofs.Exists(x => x == null))
			{
				Log.E(name + "'s RoofEffect Roofs Setting Error");
				enabled = false;
				return;
			}

			showRoof = true;
		}


		private void Update()
		{
			if ((cam.transform.position - this.transform.position).sqrMagnitude > 4900f)
			{
				return;
			}

			for (int i = 0; i < doors.Count; i++)
			{
				if (doors[i].IsInSight)
				{
					if (showRoof)
					{
						HideRoof();
					}

					return;
				}
			}

			for (int j = 0; j < insideColiders.Count; j++)
			{
				Transform transform = insideColiders[j].transform;
				Vector3 center = transform.TransformPoint(insideColiders[j].center);
				Vector3 halfExtents = Vector3.Scale(insideColiders[j].size, transform.lossyScale) / 2f;
				int num = Physics.OverlapBoxNonAlloc(center, halfExtents, cols, transform.rotation,
					GameConstants.LayerMask.WORLD_OBJECT_LAYER);
				while (cols.Length <= num)
				{
					cols = new Collider[cols.Length + 50];
					num = Physics.OverlapBoxNonAlloc(center, halfExtents, cols, transform.rotation,
						GameConstants.LayerMask.WORLD_OBJECT_LAYER);
				}

				for (int k = num - 1; k >= 0; k--)
				{
					FogSight fogSight = null;
					Collider collider = cols[k];
					if (!sightCache.TryGetValue(collider, out fogSight))
					{
						fogSight = collider.GetComponent<FogSight>();
						sightCache[collider] = fogSight;
					}

					if (!(fogSight == null) && fogSight.enabled)
					{
						if (showRoof)
						{
							HideRoof();
						}

						return;
					}
				}
			}

			if (!showRoof)
			{
				ShowRoof();
			}
		}


		private void HideRoof()
		{
			if (routine != null)
			{
				StopCoroutine(routine);
			}

			routine = this.StartThrowingCoroutine(DelayedShowRoof(false, 0.2f),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][HideRoof] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
			showRoof = false;
		}


		private void ShowRoof()
		{
			if (routine != null)
			{
				StopCoroutine(routine);
			}

			routine = this.StartThrowingCoroutine(DelayedShowRoof(true, 0.5f),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][ShowRoof] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
			showRoof = true;
		}


		private IEnumerator DelayedShowRoof(bool show, float delay)
		{
			yield return new WaitForSeconds(delay);
			for (int i = 0; i < roofs.Count; i++)
			{
				if (show)
				{
					if (roofs[i].shadowCastingMode != ShadowCastingMode.On)
					{
						roofs[i].shadowCastingMode = ShadowCastingMode.On;
					}
				}
				else if (roofs[i].shadowCastingMode != ShadowCastingMode.ShadowsOnly)
				{
					roofs[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
				}
			}

			EnableOutlinerRenderer(show);
			MonoBehaviourInstance<ClientService>.inst.UpdateMyPlayerInDoor();
		}


		private void EnableOutlinerRenderer(bool enable)
		{
			for (int i = 0; i < outliners.Count; i++)
			{
				outliners[i].outlineType = enable ? OutlineType.HoverLine : OutlineType.None;
				Renderer[] componentsInChildren = outliners[i].GetComponentsInChildren<Renderer>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].enabled = enable;
				}
			}
		}
	}
}