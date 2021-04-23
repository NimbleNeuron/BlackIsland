using UnityEngine;


public class LocalEmmaPigeon : MonoBehaviour
{
	
	private void Start()
	{
		this.initTime = Time.time;
		this.startPosition = new Vector3(0f, this.downHillStartPositionY, 0f);
		this.stopPosition = new Vector3(0f, this.downHillStopPositionY, 0f);
		base.transform.localPosition = this.startPosition;
	}

	
	private void Update()
	{
		if (this.animationEnd)
		{
			return;
		}
		if (base.transform.localPosition.y + Time.deltaTime < this.downHillStopPositionY)
		{
			base.transform.localPosition = this.stopPosition;
			this.animationEnd = true;
			return;
		}
		if (this.initTime == 0f)
		{
			return;
		}
		if (this.initTime + this.downHillDelay < Time.time)
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y - Time.deltaTime * this.downHillSpeed, base.transform.localPosition.z);
		}
	}

	
	[SerializeField]
	private float downHillStartPositionY = default;

	
	[SerializeField]
	private float downHillStopPositionY = default;

	
	private Vector3 startPosition = default;

	
	private Vector3 stopPosition = default;

	
	[SerializeField]
	private float downHillDelay = default;

	
	[SerializeField]
	private float downHillSpeed = default;

	
	private float initTime;

	
	private bool animationEnd;
}
