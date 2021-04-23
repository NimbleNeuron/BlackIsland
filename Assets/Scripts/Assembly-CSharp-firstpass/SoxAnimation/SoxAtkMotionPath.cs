using UnityEngine;

public class SoxAtkMotionPath : MonoBehaviour
{
	[HideInInspector] public float m_version = 1.101f;


	[HideInInspector] public bool m_initialized;


	public Animator m_animator;


	public Transform m_motionPathObject;


	[HideInInspector] public int m_animClipIndex;


	public bool m_autoUpdate = true;


	public Color m_pathColor = Color.green;


	public int m_pathStep = 60;


	[HideInInspector] public Vector3[] m_pathPositions = new Vector3[60];


	[HideInInspector] public float m_timeStart;


	[HideInInspector] public float m_timeEnd = 1f;


	[HideInInspector] public float m_animationSlider;


	private Matrix4x4 meMatrix;


	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			return;
		}

		if (!m_initialized)
		{
			return;
		}

		Gizmos.color = m_pathColor;
		if (m_animator == null)
		{
			meMatrix = transform.localToWorldMatrix;
		}
		else
		{
			meMatrix = m_animator.transform.localToWorldMatrix;
		}

		for (int i = 1; i < m_pathPositions.Length; i++)
		{
			Gizmos.DrawLine(meMatrix.MultiplyPoint3x4(m_pathPositions[i - 1]),
				meMatrix.MultiplyPoint3x4(m_pathPositions[i]));
		}
	}


	private void OnValidate()
	{
		m_pathStep = Mathf.Max(2, m_pathStep);
		m_timeStart = Mathf.Min(m_timeStart, m_timeEnd);
	}
}