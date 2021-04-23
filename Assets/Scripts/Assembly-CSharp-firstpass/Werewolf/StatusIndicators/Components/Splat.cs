using System.Collections.Generic;
using UnityEngine;
using Werewolf.StatusIndicators.Services;

namespace Werewolf.StatusIndicators.Components
{
	public abstract class Splat : MonoBehaviour
	{
		[SerializeField] [Range(0f, 1f)] protected float progress;


		[SerializeField] protected float scale = 7f;


		[SerializeField] protected float width;


		private readonly List<Projector> projectors = new List<Projector>();


		public abstract ScalingType Scaling { get; }


		public List<Projector> Projectors {
			get
			{
				GetComponentsInChildren<Projector>(projectors);
				return projectors;
			}
		}


		
		public SplatManager Manager { get; set; }


		
		public float Progress {
			get => progress;
			set
			{
				progress = value;
				OnValueChanged();
			}
		}


		
		public float Scale {
			get => scale;
			set
			{
				scale = value;
				OnValueChanged();
			}
		}


		
		public float Width {
			get => width;
			set
			{
				width = value;
				OnValueChanged();
			}
		}


		public virtual void Update() { }


		private void OnDestroy()
		{
			if (Projectors != null)
			{
				foreach (Projector projector in Projectors)
				{
					DestroyImmediate(projector.material);
				}
			}
		}


		public virtual void Initialize()
		{
			foreach (Projector projector in Projectors)
			{
				projector.material = new Material(projector.material);
			}

			transform.localPosition = Vector3.zero;
		}


		public virtual void OnValueChanged()
		{
			ProjectorScaler.Resize(Projectors, Scaling, scale, width);
			UpdateProgress(progress);
		}


		public virtual void OnShow() { }


		public virtual void OnHide() { }


		protected void UpdateProgress(float progress)
		{
			SetShaderFloat("_Fill", progress);
		}


		protected void SetShaderFloat(string property, float value)
		{
			foreach (Projector projector in Projectors)
			{
				if (projector.material.HasProperty(property))
				{
					projector.material.SetFloat(property, value);
				}
			}
		}
	}
}