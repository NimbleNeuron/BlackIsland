using System;
using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class LocalDrone : MonoBehaviour
	{
		private const string boxAnchor = "Anchor";


		private LocalAirSupplyItemBox airSupplyItemBox;


		private Transform anchorTransform;


		private Action<LocalDrone> DestroySelf;


		public Transform Anchor => anchorTransform;


		public void Init(Action<LocalDrone> destroyAction)
		{
			anchorTransform = transform.FindRecursively("Anchor");
			DestroySelf = destroyAction;
		}


		public void SetAirSupplyItemBox(LocalAirSupplyItemBox itemBox)
		{
			airSupplyItemBox = itemBox;
			LocalAirSupplyItemBox localAirSupplyItemBox = airSupplyItemBox;
			if (localAirSupplyItemBox != null)
			{
				localAirSupplyItemBox.SetUnopenable();
			}

			AnimatorStateInfo currentAnimatorStateInfo =
				transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
			Singleton<SoundControl>.inst.PlayFXSoundChild("AirdropDown", "AirdropBox", 10, false, transform, false);
			this.StartThrowingCoroutine(DestroyDrone(currentAnimatorStateInfo.length),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][DestroyDrone] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
			this.StartThrowingCoroutine(BoxDrop(),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][BoxDrop] Message:" + exception.Message + ", StackTrace:" + exception.StackTrace);
				});
			this.StartThrowingCoroutine(SpawnDustEffect(),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][SpawnDustEffect] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private IEnumerator DestroyDrone(float waitSec)
		{
			yield return new WaitForSeconds(waitSec);
			Action<LocalDrone> destroySelf = DestroySelf;
			if (destroySelf != null)
			{
				destroySelf(this);
			}
		}


		private IEnumerator BoxDrop()
		{
			yield return new WaitForSeconds(3.5f);
			if (airSupplyItemBox != null)
			{
				airSupplyItemBox.transform.parent = transform.parent;
				airSupplyItemBox.DropOnGround();
			}
		}


		private IEnumerator SpawnDustEffect()
		{
			yield return new WaitForSeconds(3f);
			Instantiate<GameObject>(SingletonMonoBehaviour<ResourceManager>.inst.LoadEffect("FX_BI_SupplyBoxDust_01"),
				transform.localPosition, Quaternion.identity);
		}
	}
}