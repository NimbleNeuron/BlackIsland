using System;
using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class BulletCooldown
	{
		
		
		public Item WeaponItem
		{
			get
			{
				return this.weaponItem;
			}
		}

		
		
		public float RemainCooldown
		{
			get
			{
				return this.remainCooldown;
			}
		}

		
		
		
		public event BulletCooldown.StopCooldown stopCooldown;

		
		
		
		public event BulletCooldown.AddBullet addBullet;

		
		public BulletCooldown(MonoBehaviour mono, Item weaponItem)
		{
			this.mono = mono;
			this.weaponItem = weaponItem;
			this.bulletCapacity = GameDB.item.GetBulletCapacityData(weaponItem.itemCode);
			this.remainCooldown = this.bulletCapacity.time;
		}

		
		public void Update()
		{
			this.remainCooldown -= 0.033333335f;
		}

		
		public void MergeItem(Item weaponItem)
		{
			this.weaponItem = weaponItem;
		}

		
		public void FinishBulletCooldown()
		{
			this.mono.StopCoroutine(this.bulletCoroutine);
			this.Clear();
		}

		
		private void Clear()
		{
			this.bulletCoroutine = null;
			this.weaponItem = null;
		}

		
		public void StartBulletCooldown()
		{
			this.bulletCoroutine = this.mono.StartThrowingCoroutine(this.CorStartBulletCooldown(), delegate(Exception exception)
			{
			});
		}

		
		private IEnumerator CorStartBulletCooldown()
		{
			if (this.weaponItem.IsFullBullet())
			{
				this.stopCooldown(this.weaponItem.id);
				yield break;
			}
			yield return new WaitForSeconds(this.remainCooldown);
			this.remainCooldown = this.bulletCapacity.time;
			this.addBullet(this.weaponItem.id, this.weaponItem.madeType, this.bulletCapacity.count);
			this.bulletCoroutine = this.mono.StartThrowingCoroutine(this.CorStartBulletCooldown(), delegate(Exception exception)
			{
			});
		}

		
		private Item weaponItem;

		
		private Coroutine bulletCoroutine;

		
		private MonoBehaviour mono;

		
		private BulletCapacity bulletCapacity;

		
		private float remainCooldown;

		
		public delegate void StopCooldown(int itemId);

		
		public delegate void AddBullet(int itemId, ItemMadeType madeType, int addBullet);
	}
}
