using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blis.Client
{
	public abstract class Splat : MonoBehaviour
	{
		protected readonly List<IndicatorPart> Parts = new List<IndicatorPart>();
		protected float angle = -1f;
		private Coroutine checkHider;
		protected Vector3? fixedDirection;
		private bool? isHide;
		private Action<Splat> lateUpdateAction;
		protected float length = -1f;
		protected float maxRange = -1f;
		protected float progress = -1f;
		protected float range = -1f;
		protected float scale = -1f;
		private List<GameObject> selfObjects;
		protected float width = -1f;
		
		public SplatManager Manager { get; set; }
		
		public float Progress {
			get => progress;
			set
			{
				if (progress == value)
				{
					return;
				}

				progress = value;
				UpdateProgress(progress);
			}
		}
		
		public float Scale {
			get => scale;
			set
			{
				if (scale == value)
				{
					return;
				}

				scale = value;
				UpdateScale(scale);
			}
		}
		
		public float Width {
			get => width;
			set
			{
				if (width == value)
				{
					return;
				}

				width = value;
				UpdateWidth(width);
			}
		}
		
		public float Range {
			get => range;
			set
			{
				if (range == value)
				{
					return;
				}

				range = value;
				UpdateRange(range);
			}
		}
		
		public float MaxRange {
			get => maxRange;
			set
			{
				if (maxRange == value)
				{
					return;
				}

				maxRange = value;
				UpdateMaxRange(maxRange);
			}
		}
		
		public float Length {
			get => length;
			set
			{
				if (length == value)
				{
					return;
				}

				length = value;
				UpdateLength(length);
			}
		}
		
		public float Angle {
			get => angle;
			set
			{
				if (angle == value)
				{
					return;
				}

				angle = value;
				UpdateAngle(angle);
			}
		}


		
		public Vector3? Direction {
			set => fixedDirection = value;
		}


		public virtual void Awake()
		{
			GetComponentsInChildren<IndicatorPart>(Parts);
		}


		protected virtual void LateUpdate()
		{
			Action<Splat> action = lateUpdateAction;
			if (action == null)
			{
				return;
			}

			action(this);
		}


		public void SetLateUpdateAction(Action<Splat> lateUpdateAction)
		{
			this.lateUpdateAction = lateUpdateAction;
		}


		public void OnValueChanged()
		{
			UpdateProgress(progress);
			UpdateScale(scale);
			UpdateRange(range);
			UpdateLength(length);
			UpdateAngle(angle);
		}


		protected virtual void UpdateProgress(float progress)
		{
			SetShaderFloat("_Fill", progress);
		}


		protected virtual void UpdateScale(float scale) { }


		protected virtual void UpdateWidth(float width) { }


		protected virtual void UpdateRange(float skillRange) { }


		protected virtual void UpdateMaxRange(float maxRange) { }


		protected virtual void UpdateLength(float length) { }


		protected virtual void UpdateAngle(float angle) { }


		protected Vector3 FlattenVector(Vector3 target)
		{
			return new Vector3(target.x, 0f, target.z);
		}


		protected void SetShaderFloat(string property, float value)
		{
			foreach (IndicatorPart indicatorPart in Parts)
			{
				if (indicatorPart != null)
				{
					indicatorPart.SetShaderFloat(property, value);
				}
			}
		}


		public virtual void Show()
		{
			gameObject.SetActive(true);
			LateUpdate();
			isHide = false;
		}


		public void Hide()
		{
			fixedDirection = null;
			if (checkHider != null)
			{
				StopCoroutine(checkHider);
			}

			if (isHide == null)
			{
				gameObject.SetActive(false);
				return;
			}

			if (gameObject.activeInHierarchy)
			{
				checkHider = StartCoroutine(CheckHide());
			}

			isHide = true;
		}


		private IEnumerator CheckHide()
		{
			for (;;)
			{
				bool? flag = isHide;
				bool flag2 = false;
				if (!((flag.GetValueOrDefault() == flag2) & (flag != null)))
				{
					break;
				}

				yield return new WaitForEndOfFrame();
			}

			gameObject.SetActive(false);
			checkHider = null;
		}


		public void ShowSelf()
		{
			if (selfObjects == null)
			{
				return;
			}

			foreach (GameObject gameObject in selfObjects)
			{
				gameObject.SetActive(true);
			}
		}


		public void HideSelf()
		{
			if (selfObjects == null)
			{
				selfObjects = new List<GameObject>();
				Transform transform = this.transform;
				int childCount = transform.childCount;
				Splat[] componentsInChildren = transform.GetComponentsInChildren<Splat>();
				for (int i = 0; i < childCount; i++)
				{
					bool flag = false;
					Transform child = transform.GetChild(i);
					foreach (Splat splat in componentsInChildren)
					{
						if (child == splat.transform)
						{
							flag = true;
							break;
						}
					}

					if (!flag)
					{
						selfObjects.Add(child.gameObject);
					}
				}
			}

			foreach (GameObject gameObject in selfObjects)
			{
				gameObject.SetActive(false);
			}
		}
	}
}