using System.Collections.Generic;
using Blis.Common;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CustomGroup : BaseUI
	{
		private Text txtTeamNumber;


		public List<CustomUserSlot> CustomUserSlots { get; } = new List<CustomUserSlot>();


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			txtTeamNumber = GameUtil.Bind<Text>(gameObject, "TXT_TeamNumber");
			transform.GetComponentsInChildren<CustomUserSlot>(true, CustomUserSlots);
		}


		public void SetTeamNumber(int teamNumber)
		{
			txtTeamNumber.text = teamNumber.ToString();
		}
	}
}