using System.Text;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class WicklineAppearRemainTime : BaseControl
	{
		public enum ShowMode
		{
			None,

			RemainTime,

			Activate
		}


		[SerializeField] private LnText remainTimeText = default;


		private int showedReaminTime;


		private ShowMode showMode;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
		}


		public void SetActive(bool isActive)
		{
			if (isActive == IsActive())
			{
				return;
			}

			gameObject.SetActive(isActive);
		}


		public void ShowReaminTime()
		{
			int num = Mathf.CeilToInt(MonoBehaviourInstance<ClientService>.inst.WicklineResponRemainTime);
			if (num == showedReaminTime)
			{
				return;
			}

			showedReaminTime = num;
			if (showedReaminTime <= 0f)
			{
				remainTimeText.text = null;
				return;
			}

			int value = showedReaminTime / 60;
			int value2 = showedReaminTime % 60;
			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			stringBuilder.Append(GameUtil.IntToString(value, GameUtil.NumberOfDigits.One));
			stringBuilder.Append(" : ");
			stringBuilder.Append(GameUtil.IntToString(value2, GameUtil.NumberOfDigits.Two));
			remainTimeText.text = stringBuilder.ToString();
		}


		public void SettingShowMode(ShowMode pShowMode)
		{
			if (showMode == pShowMode)
			{
				return;
			}

			showMode = pShowMode;
			if (showMode != ShowMode.RemainTime)
			{
				if (showMode == ShowMode.Activate)
				{
					remainTimeText.text = null;
					return;
				}

				remainTimeText.text = null;
			}
		}
	}
}