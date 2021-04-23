using UnityEngine;

namespace Ara
{
	[RequireComponent(typeof(AraTrail))]
	public class TireTrack : MonoBehaviour
	{
		public float offset = 0.05f;


		public float maxDist = 0.1f;


		private AraTrail trail;


		private void OnEnable()
		{
			trail = GetComponent<AraTrail>();
			trail.onUpdatePoints += ProjectToGround;
		}


		private void OnDisable()
		{
			trail.onUpdatePoints -= ProjectToGround;
		}


		private void ProjectToGround()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(transform.position, -Vector3.up), out raycastHit, maxDist))
			{
				if (trail.emit && trail.points.Count > 0)
				{
					AraTrail.Point point = trail.points[trail.points.Count - 1];
					if (!point.discontinuous)
					{
						point.normal = raycastHit.normal;
						point.position = raycastHit.point + raycastHit.normal * offset;
						trail.points[trail.points.Count - 1] = point;
					}
				}

				trail.emit = true;
				return;
			}

			if (trail.emit)
			{
				trail.emit = false;
				if (trail.points.Count > 0)
				{
					trail.points.RemoveAt(trail.points.Count - 1);
				}
			}
		}
	}
}