using UnityEngine;


public class BlinkColor : MonoBehaviour
{
	
	private void Start()
	{
		this.colorOri = base.GetComponent<Renderer>().material.GetColor(this.colorName);
	}

	
	private void LateUpdate()
	{
		float t = Mathf.PingPong(Time.time, this.duration) / this.duration;
		base.GetComponent<Renderer>().material.SetColor(this.colorName, Color.Lerp(this.colorOri, this.toColor, t));
	}

	
	public string colorName = "_Color";

	
	public Color toColor = Color.black;

	
	public float duration = 1f;

	
	private Color colorOri = Color.white;
}
