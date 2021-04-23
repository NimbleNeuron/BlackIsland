using UnityEngine;

namespace BIOutline
{
	[RequireComponent(typeof(Renderer))]
	public class OutlineRendererOrganizeDrawCall : MonoBehaviour
	{
		private Outliner outliner;


		private void Awake()
		{
			outliner = transform.GetComponentInParent<Outliner>();
		}


		private void OnEnable()
		{
			if (outliner == null)
			{
				outliner = transform.GetComponentInParent<Outliner>();
			}

			if (outliner != null)
			{
				outliner.OrganizeDrawCall();
			}
		}


		private void OnDisable()
		{
			if (outliner == null)
			{
				outliner = transform.GetComponentInParent<Outliner>();
			}

			if (outliner != null)
			{
				outliner.OrganizeDrawCall();
			}
		}
	}
}