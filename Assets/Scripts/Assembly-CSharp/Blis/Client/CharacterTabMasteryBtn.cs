using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterTabMasteryBtn : BaseControl
	{
		[SerializeField] private Image icon = default;


		[SerializeField] private Text text = default;


		[SerializeField] private Image selected = default;


		private WeaponType weaponType;


		public WeaponType WeaponType => weaponType;


		public void SetMastery(WeaponType weaponType)
		{
			this.weaponType = weaponType;
			if (weaponType == WeaponType.None)
			{
				icon.gameObject.SetActive(false);
				text.gameObject.SetActive(true);
			}
			else
			{
				SetIcon();
			}

			transform.name = string.Format("CharacterTabMasteryBtn_{0}", weaponType);
		}


		private void SetIcon()
		{
			icon.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetWeaponMasterySprite(weaponType);
		}


		public void SetSelected(bool active)
		{
			selected.enabled = active;
		}


		public bool IsActiveSelected()
		{
			return selected.enabled;
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			if (weaponType != WeaponType.None)
			{
				MonoBehaviourInstance<Tooltip>.inst.SetLabel(Ln.Get(string.Format("WeaponType/{0}",
					weaponType.GetWeaponMasteryType())));
				Vector2 vector = transform.position;
				vector += GameUtil.ConvertPositionOnScreenResolution(-23f, 66f);
				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, vector, Tooltip.Pivot.LeftTop);
			}
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			MonoBehaviourInstance<Tooltip>.inst.Hide();
		}
	}
}