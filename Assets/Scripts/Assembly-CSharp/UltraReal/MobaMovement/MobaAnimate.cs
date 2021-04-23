using UnityEngine;

namespace UltraReal.MobaMovement
{
	
	public class MobaAnimate : MonoBehaviour
	{
		
		private void Start()
		{
			if (!this._animator)
			{
				this._animator = base.GetComponent<Animator>();
			}
			this._lastPos = base.transform.position;
		}

		
		private void Update()
		{
			Vector3 vector = (base.transform.position - this._lastPos) / Time.deltaTime;
			this._forwardSpeedFactor = Mathf.Lerp(this._forwardSpeedFactor, vector.magnitude / this._strideDistance, Time.deltaTime * 10f);
			this._lastPos = base.transform.position;
			if (this._animator != null)
			{
				this._animator.SetFloat(this._forwardName, this._forwardSpeedFactor);
			}
		}

		
		protected Vector3 _lastPos;

		
		protected float _forwardSpeedFactor;

		
		[SerializeField]
		private Animator _animator;

		
		[SerializeField]
		private float _strideDistance = 1f;

		
		[SerializeField]
		private string _forwardName = "Forward";
	}
}
