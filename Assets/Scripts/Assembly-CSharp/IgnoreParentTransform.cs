using UnityEngine;


public class IgnoreParentTransform : MonoBehaviour
{
	
	private void Awake()
	{
		this._transform = base.transform;
		this.originLocalPos = this._transform.localPosition;
		this.originLocalRot = this._transform.localRotation;
	}

	
	private void OnEnable()
	{
		this.spawnWorldPos = this._transform.position;
		this.spawnWorldRot = this._transform.rotation;
	}

	
	private void LateUpdate()
	{
		if (this.ignorePosition)
		{
			this._transform.position = this.spawnWorldPos;
		}
		if (this.ignoreRotation)
		{
			this._transform.rotation = this.spawnWorldRot;
		}
	}

	
	private void OnDisable()
	{
		this._transform.localPosition = this.originLocalPos;
		this._transform.localRotation = this.originLocalRot;
	}

	
	public void SetPosition(Vector3 position, Quaternion quaternion)
	{
		this.spawnWorldPos = position;
		this.spawnWorldRot = quaternion;
	}

	
	public bool ignorePosition;

	
	public bool ignoreRotation;

	
	private Vector3 originLocalPos;

	
	private Quaternion originLocalRot;

	
	private Vector3 spawnWorldPos;

	
	private Quaternion spawnWorldRot;

	
	private Transform _transform;
}
