using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[RequireComponent(typeof(LocalHookLineProjectile))]
	[ObjectAttr(ObjectType.HookLineProjectile)]
	public class LocalHookLineProjectile : LocalProjectile
	{
		private HookLineProjectileData hookLineData;


		private bool isLinked;


		private LineRenderer lineRenderer;


		private Transform lineRendererFrom;


		private Transform lineRendererTo;


		private LocalCharacter linkFromCharacter;


		private LocalPlayerCharacter linkFromPlayerCharacter;


		private LocalCharacter linkToCharacter;


		private LocalPlayerCharacter linkToPlayerCharacter;


		private Vector3? linkToPoint;


		private Transform meshTrans;


		private LocalPlayerCharacter playerOwner;


		private Transform stretchTransform;


		public LocalCharacter LinkFromCharacter => linkFromCharacter;


		public LocalCharacter LinkToCharacter => linkToCharacter;


		public Vector3? LinkToPoint => linkToPoint;


		protected override bool needRebuildFogHider => false;


		protected override void Update()
		{
			if (!IsAlive)
			{
				return;
			}

			if (isLinked)
			{
				UpdateConnection();
			}
			else
			{
				base.Update();
			}

			UpdateLineRenderer();
		}

		protected override ObjectType GetObjectType()
		{
			return ObjectType.HookLineProjectile;
		}


		public override void Init(byte[] snapshotData)
		{
			HookLineProjectileSnapshot hookLineProjectileSnapshot =
				serializer.Deserialize<HookLineProjectileSnapshot>(snapshotData);
			base.Init(hookLineProjectileSnapshot);
			playerOwner = Owner as LocalPlayerCharacter;
			HookLineInfoSnapshot hookLineInfoSnapshot =
				serializer.Deserialize<HookLineInfoSnapshot>(hookLineProjectileSnapshot.hookLineInfoSnapshot);
			hookLineData = GameDB.projectile.GetHookLineData(hookLineInfoSnapshot.hookLineCode);
			linkFromCharacter = hookLineInfoSnapshot.linkFromObjectId == 0
				? Owner
				: MonoBehaviourInstance<ClientService>.inst.World.Find<LocalCharacter>(hookLineInfoSnapshot
					.linkFromObjectId);
			linkFromPlayerCharacter = linkFromCharacter as LocalPlayerCharacter;
			if (!string.IsNullOrEmpty(hookLineData.casterLineStickObjectName))
			{
				lineRendererFrom = linkFromCharacter.transform.FindRecursively(hookLineData.casterLineStickObjectName);
			}

			if (lineRendererFrom == null)
			{
				lineRendererFrom = linkFromCharacter.transform;
			}

			if (hookLineInfoSnapshot.linkToObjectId != 0)
			{
				OnCollision(
					MonoBehaviourInstance<ClientService>.inst.World.Find<LocalCharacter>(hookLineInfoSnapshot
						.linkToObjectId));
			}
			else if (hookLineInfoSnapshot.linkToPoint != null)
			{
				OnCollisionWall(hookLineInfoSnapshot.linkToPoint.Value);
			}
			else
			{
				isLinked = false;
				lineRendererTo = transform;
				SetLineRenderer();
				if (stretchTransform != null)
				{
					float y = GameUtil.DistanceOnPlane(lineRendererFrom.position, lineRendererTo.position);
					stretchTransform.localScale = new Vector3(1f, y, 1f);
				}

				createdPosition = lineRendererFrom.position;
			}

			fogHiderOnCenter.SetAnotherTargetTransform(Owner.transform, ProjectileData.collisionObjectRadius);
			Update();
		}


		public override void OnCollision(LocalCharacter target)
		{
			base.OnCollision(target);
			if (isLinked)
			{
				return;
			}

			HideProjectile();
			linkToCharacter = target;
			linkToPlayerCharacter = linkToCharacter as LocalPlayerCharacter;
			isLinked = true;
			if (!string.IsNullOrEmpty(hookLineData.targetLineStickObjectName))
			{
				lineRendererTo = linkToCharacter.transform.FindRecursively(hookLineData.targetLineStickObjectName);
			}

			if (lineRendererTo == null)
			{
				lineRendererTo = transform;
			}

			if (!string.IsNullOrEmpty(hookLineData.connectionLinePrefab))
			{
				if (lineRenderer != null)
				{
					Destroy(lineRenderer.gameObject);
				}
				else if (meshTrans != null)
				{
					Destroy(meshTrans.gameObject);
				}

				SetLineRenderer();
			}

			ShowConnectionEffect();
			Update();
		}


		public override void OnCollisionWall(Vector3 position)
		{
			base.OnCollisionWall(position);
			if (isLinked)
			{
				return;
			}

			HideProjectile();
			linkToPoint = position;
			isLinked = true;
			if (lineRendererTo == null)
			{
				lineRendererTo = transform;
			}

			if (!string.IsNullOrEmpty(hookLineData.connectionLinePrefab))
			{
				if (lineRenderer != null)
				{
					Destroy(lineRenderer.gameObject);
				}
				else if (meshTrans != null)
				{
					Destroy(meshTrans.gameObject);
				}

				SetLineRenderer();
			}

			ShowConnectionEffect();
			Update();
		}


		private void SetLineRenderer()
		{
			string projectileName = isLinked ? hookLineData.connectionLinePrefab : hookLineData.fireLinePrefab;
			GameObject gameObject = Instantiate<GameObject>(LoadProjectile(projectileName), transform);
			gameObject.SetActive(true);
			lineRenderer = gameObject.GetComponent<LineRenderer>();
			if (lineRenderer == null)
			{
				meshTrans = gameObject.transform;
			}

			if (meshTrans != null && !string.IsNullOrEmpty(hookLineData.StretchObjectName))
			{
				stretchTransform = meshTrans.FindRecursively(hookLineData.StretchObjectName);
			}

			fogHiderOnCenter.RebuildRendererList();
		}


		private void ShowConnectionEffect()
		{
			if (!string.IsNullOrEmpty(hookLineData.connectionEffectPrefab) &&
			    (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Owner.ObjectId) ||
			     SingletonMonoBehaviour<PlayerController>.inst.IsMe(linkFromCharacter.ObjectId) ||
			     SingletonMonoBehaviour<PlayerController>.inst.IsMe(linkToCharacter.ObjectId)))
			{
				GameObject gameObject = LoadEffect(hookLineData.connectionEffectPrefab);
				if (gameObject != null)
				{
					Transform transform = Instantiate<GameObject>(gameObject, this.transform).transform;
					transform.localPosition = Vector3.zero;
					transform.localScale = Vector3.one;
				}
			}

			if (!string.IsNullOrEmpty(hookLineData.connectionSound))
			{
				AudioClip audioClip = LoadFXSound(hookLineData.connectionSound);
				Singleton<SoundControl>.inst.PlayFXSoundChild(audioClip, ".connectionSound",
					Mathf.CeilToInt(hookLineData.connectionMaxRange), false, transform, true);
			}
		}


		private bool IsOwnerInvalid()
		{
			return Owner == null || !Owner.IsAlive || playerOwner != null && playerOwner.IsDyingCondition;
		}


		private bool IsLinkToCharacterInvalid()
		{
			return linkFromCharacter == null || !linkFromCharacter.IsAlive ||
			       linkFromPlayerCharacter != null && linkFromPlayerCharacter.IsDyingCondition ||
			       linkToCharacter == null || !linkToCharacter.IsAlive ||
			       linkToPlayerCharacter != null && linkToPlayerCharacter.IsDyingCondition;
		}


		public override void DestroySelf()
		{
			base.DestroySelf();
			if (!string.IsNullOrEmpty(hookLineData.connectionSound))
			{
				Singleton<SoundControl>.inst.StopFxSoundChild(transform, hookLineData.connectionSound);
			}
		}


		private void UpdateLineRenderer()
		{
			if (lineRenderer != null)
			{
				lineRenderer.SetPosition(0, lineRendererFrom.position);
				lineRenderer.SetPosition(1, lineRendererTo.position);
				return;
			}

			if (stretchTransform != null)
			{
				float num = GameUtil.Distance(lineRendererFrom.position, lineRendererTo.position);
				Vector3 one = Vector3.one;
				one.y = num;
				stretchTransform.localScale = one;
				if (num > 0f)
				{
					Vector3 direction = GameUtil.Direction(lineRendererFrom.position, lineRendererTo.position);
					meshTrans.rotation = GameUtil.LookRotation(direction);
				}
			}
		}


		private void UpdateConnection()
		{
			if (linkFromCharacter == null || !linkFromCharacter.IsAlive)
			{
				if (lineRenderer != null)
				{
					Destroy(lineRenderer.gameObject);
				}

				return;
			}

			if (linkToPoint != null)
			{
				Vector3 value = linkToPoint.Value;
				value.y = lineRendererTo.position.y;
				SetPosition(value);
				return;
			}

			if ((linkToCharacter == null || !linkToCharacter.IsAlive) && lineRenderer != null)
			{
				Destroy(lineRenderer.gameObject);
				return;
			}

			Vector3 position = linkToCharacter.GetPosition();
			position.y = lineRendererTo.position.y;
			SetPosition(position);
		}
	}
}