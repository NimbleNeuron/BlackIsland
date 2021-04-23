using UnityEngine;

public class HeadLookController : MonoBehaviour
{
	public Transform rootNode;


	public BendingSegment[] segments;


	public NonAffectedJoints[] nonAffectedJoints;


	public Vector3 headLookVector = Vector3.forward;


	public Vector3 headUpVector = Vector3.up;


	public Vector3 target = Vector3.zero;


	public float effect = 1f;


	public bool overrideAnimation;


	public bool IsLobbyCharacter = true;


	private void Start()
	{
		if (rootNode == null)
		{
			rootNode = transform;
		}

		if (IsLobbyCharacter)
		{
			target = Camera.main.transform.position;
		}
		else
		{
			target = transform.position + transform.forward;
		}

		foreach (BendingSegment bendingSegment in segments)
		{
			Quaternion lhs = Quaternion.Inverse(bendingSegment.firstTransform.parent.rotation);
			bendingSegment.referenceLookDir = lhs * rootNode.rotation * headLookVector.normalized;
			bendingSegment.referenceUpDir = lhs * rootNode.rotation * headUpVector.normalized;
			bendingSegment.angleH = 0f;
			bendingSegment.angleV = 0f;
			bendingSegment.dirUp = bendingSegment.referenceUpDir;
			bendingSegment.chainLength = 1;
			Transform transform = bendingSegment.lastTransform;
			while (transform != bendingSegment.firstTransform && transform != transform.root)
			{
				bendingSegment.chainLength++;
				transform = transform.parent;
			}

			bendingSegment.origRotations = new Quaternion[bendingSegment.chainLength];
			transform = bendingSegment.lastTransform;
			for (int j = bendingSegment.chainLength - 1; j >= 0; j--)
			{
				bendingSegment.origRotations[j] = transform.localRotation;
				transform = transform.parent;
			}
		}
	}


	private void LateUpdate()
	{
		if (Time.deltaTime == 0f)
		{
			return;
		}

		Vector3[] array = new Vector3[nonAffectedJoints.Length];
		for (int i = 0; i < nonAffectedJoints.Length; i++)
		{
			foreach (Transform tf in nonAffectedJoints[i].joint)
			{
				array[i] = tf.position - nonAffectedJoints[i].joint.position;
			}

			// co: convert to foreach from enumerator
			// using (IEnumerator enumerator = this.nonAffectedJoints[i].joint.GetEnumerator())
			// {
			// 	if (enumerator.MoveNext())
			// 	{
			// 		Transform transform = (Transform)enumerator.Current;
			// 		array[i] = transform.position - this.nonAffectedJoints[i].joint.position;
			// 	}
			// }
		}

		foreach (BendingSegment bendingSegment in segments)
		{
			Transform transform2 = bendingSegment.lastTransform;
			if (overrideAnimation)
			{
				for (int k = bendingSegment.chainLength - 1; k >= 0; k--)
				{
					transform2.localRotation = bendingSegment.origRotations[k];
					transform2 = transform2.parent;
				}
			}

			Quaternion rotation = bendingSegment.firstTransform.parent.rotation;
			Quaternion rotation2 = Quaternion.Inverse(rotation);
			Vector3 normalized = (target - bendingSegment.lastTransform.position).normalized;
			Vector3 vector = rotation2 * normalized;
			float num = AngleAroundAxis(bendingSegment.referenceLookDir, vector, bendingSegment.referenceUpDir);
			Vector3 axis = Vector3.Cross(bendingSegment.referenceUpDir, vector);
			float num2 = AngleAroundAxis(vector - Vector3.Project(vector, bendingSegment.referenceUpDir), vector, axis);
			float f = Mathf.Max(0f, Mathf.Abs(num) - bendingSegment.thresholdAngleDifference) * Mathf.Sign(num);
			float f2 = Mathf.Max(0f, Mathf.Abs(num2) - bendingSegment.thresholdAngleDifference) * Mathf.Sign(num2);
			num = Mathf.Max(Mathf.Abs(f) * Mathf.Abs(bendingSegment.bendingMultiplier),
				      Mathf.Abs(num) - bendingSegment.maxAngleDifference) * Mathf.Sign(num) *
			      Mathf.Sign(bendingSegment.bendingMultiplier);
			num2 = Mathf.Max(Mathf.Abs(f2) * Mathf.Abs(bendingSegment.bendingMultiplier),
				       Mathf.Abs(num2) - bendingSegment.maxAngleDifference) * Mathf.Sign(num2) *
			       Mathf.Sign(bendingSegment.bendingMultiplier);
			num = Mathf.Clamp(num, -bendingSegment.maxBendingAngle, bendingSegment.maxBendingAngle);
			num2 = Mathf.Clamp(num2, -bendingSegment.maxBendingAngle, bendingSegment.maxBendingAngle);
			Vector3 axis2 = Vector3.Cross(bendingSegment.referenceUpDir, bendingSegment.referenceLookDir);
			bendingSegment.angleH =
				Mathf.Lerp(bendingSegment.angleH, num, Time.deltaTime * bendingSegment.responsiveness);
			bendingSegment.angleV =
				Mathf.Lerp(bendingSegment.angleV, num2, Time.deltaTime * bendingSegment.responsiveness);
			vector = Quaternion.AngleAxis(bendingSegment.angleH, bendingSegment.referenceUpDir) *
			         Quaternion.AngleAxis(bendingSegment.angleV, axis2) * bendingSegment.referenceLookDir;
			Vector3 referenceUpDir = bendingSegment.referenceUpDir;
			Vector3.OrthoNormalize(ref vector, ref referenceUpDir);
			Vector3 forward = vector;
			bendingSegment.dirUp = Vector3.Slerp(bendingSegment.dirUp, referenceUpDir, Time.deltaTime * 5f);
			Vector3.OrthoNormalize(ref forward, ref bendingSegment.dirUp);
			Quaternion b = rotation * Quaternion.LookRotation(forward, bendingSegment.dirUp) *
			               Quaternion.Inverse(rotation * Quaternion.LookRotation(bendingSegment.referenceLookDir,
				               bendingSegment.referenceUpDir));
			Quaternion lhs = Quaternion.Slerp(Quaternion.identity, b, effect / bendingSegment.chainLength);
			transform2 = bendingSegment.lastTransform;
			for (int l = 0; l < bendingSegment.chainLength; l++)
			{
				transform2.rotation = lhs * transform2.rotation;
				transform2 = transform2.parent;
			}
		}

		for (int m = 0; m < nonAffectedJoints.Length; m++)
		{
			Vector3 vector2 = Vector3.zero;

			foreach (Transform tf in nonAffectedJoints[m].joint)
			{
				vector2 = tf.position - nonAffectedJoints[m].joint.position;
			}

			// co: convert to foreach from enumerator
			// using (IEnumerator enumerator = this.nonAffectedJoints[m].joint.GetEnumerator())
			// {
			// 	if (enumerator.MoveNext())
			// 	{
			// 		vector2 = ((Transform)enumerator.Current).position - this.nonAffectedJoints[m].joint.position;
			// 	}
			// }

			Vector3 toDirection = Vector3.Slerp(array[m], vector2, nonAffectedJoints[m].effect);
			nonAffectedJoints[m].joint.rotation =
				Quaternion.FromToRotation(vector2, toDirection) * nonAffectedJoints[m].joint.rotation;
		}
	}


	public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
	{
		dirA -= Vector3.Project(dirA, axis);
		dirB -= Vector3.Project(dirB, axis);
		return Vector3.Angle(dirA, dirB) * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0f ? -1 : 1);
	}
}