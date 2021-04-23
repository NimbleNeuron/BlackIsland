using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class BulletCooldown
	{
		public delegate void AddBullet(int itemId, ItemMadeType madeType, int addBullet);


		public delegate void StopCooldown(int itemId);


		private readonly BulletCapacity bulletCapacity;


		private readonly MonoBehaviour mono;


		private Coroutine bulletCoroutine;


		private float remainCooldown;


		private Item weaponItem;


		public BulletCooldown(MonoBehaviour mono, Item weaponItem)
		{
			this.mono = mono;
			this.weaponItem = weaponItem;
			bulletCapacity = GameDB.item.GetBulletCapacityData(weaponItem.itemCode);
			remainCooldown = weaponItem.RemainCoolTime == 0f ? bulletCapacity.time : weaponItem.RemainCoolTime;
		}


		public Item WeaponItem => weaponItem;


		public float RemainCooldown => remainCooldown;


		
		
		public event StopCooldown stopCooldown;


		
		
		public event AddBullet addBullet;


		public void SetRemainCooldown(float remainCooldown)
		{
			this.remainCooldown = remainCooldown;
		}


		public void MergeItem(Item weaponItem)
		{
			this.weaponItem = weaponItem;
		}


		public void UpdateInvenItemCooldown()
		{
			InvenItemSlot invenItemSlot = MonoBehaviourInstance<GameUI>.inst.InventoryHud.GetInvenItemSlot(weaponItem);
			if (invenItemSlot != null && !weaponItem.IsFullBullet())
			{
				invenItemSlot.UpdateBulletCooldown(remainCooldown, bulletCapacity.time);
			}
		}


		public void UpdateEquipItemCooldown()
		{
			EquipItemSlot weaponEquipItemSlot = MonoBehaviourInstance<GameUI>.inst.StatusHud.GetWeaponEquipItemSlot();
			if (weaponEquipItemSlot != null && !weaponItem.IsFullBullet())
			{
				weaponEquipItemSlot.UpdateBulletCooldown(remainCooldown, bulletCapacity.time);
			}
		}


		public void FinishBulletCooldown()
		{
			mono.StopCoroutine(bulletCoroutine);
			Clear();
		}


		private void Clear()
		{
			bulletCoroutine = null;
			weaponItem = null;
		}


		public void StartBulletCooldown()
		{
			bulletCoroutine = mono.StartThrowingCoroutine(CorStartBulletCooldown(), delegate { });
		}


		private IEnumerator CorStartBulletCooldown()
		{
			if (weaponItem.IsFullBullet())
			{
				stopCooldown(weaponItem.id);
				yield break;
			}

			InvenItemSlot invenItemSlot = MonoBehaviourInstance<GameUI>.inst.InventoryHud.GetInvenItemSlot(weaponItem);
			if (invenItemSlot != null)
			{
				invenItemSlot.UpdateBulletCooldown(remainCooldown, bulletCapacity.time);
			}
			else
			{
				EquipItemSlot weaponEquipItemSlot =
					MonoBehaviourInstance<GameUI>.inst.StatusHud.GetWeaponEquipItemSlot();
				if (weaponEquipItemSlot != null)
				{
					weaponEquipItemSlot.UpdateBulletCooldown(remainCooldown, bulletCapacity.time);
				}
			}

			yield return new WaitForSeconds(remainCooldown);
			remainCooldown = bulletCapacity.time;
			addBullet(weaponItem.id, weaponItem.madeType, bulletCapacity.count);
			bulletCoroutine = mono.StartThrowingCoroutine(CorStartBulletCooldown(), delegate { });
		}
	}
}