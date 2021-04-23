using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class ActiveOnHostileType : MonoBehaviour
	{
		[SerializeField] private GameObject allyObject = default;


		[SerializeField] private GameObject enemyObject = default;


		private bool? pastViewIsAlly;


		private LocalCharacter targetCharacter;


		private LocalProjectile targetProjectile;


		private void OnEnable() { }


		private void OnDisable() { }

		public void Init(LocalSummonBase summonBase)
		{
			targetProjectile = null;
			targetCharacter = summonBase;
			Refresh();
		}


		public void Init(LocalProjectile projectile)
		{
			targetProjectile = projectile;
			targetCharacter = null;
			Refresh();
		}


		public void Init(LocalCharacter character)
		{
			targetProjectile = null;
			targetCharacter = character;
			Refresh();
		}


		public void Refresh()
		{
			if (targetProjectile == null && targetCharacter == null)
			{
				return;
			}

			bool flag = false;
			if (targetProjectile != null)
			{
				flag = MonoBehaviourInstance<ClientService>.inst.IsAlly(targetProjectile.Owner);
			}
			else if (targetCharacter != null)
			{
				flag = MonoBehaviourInstance<ClientService>.inst.IsAlly(targetCharacter);
			}

			bool? flag2 = pastViewIsAlly;
			bool flag3 = flag;
			if ((flag2.GetValueOrDefault() == flag3) & (flag2 != null))
			{
				return;
			}

			pastViewIsAlly = flag;
			GameObject gameObject = allyObject;
			if (gameObject != null)
			{
				gameObject.SetActive(flag);
			}

			GameObject gameObject2 = enemyObject;
			if (gameObject2 == null)
			{
				return;
			}

			gameObject2.SetActive(!flag);
		}
	}
}