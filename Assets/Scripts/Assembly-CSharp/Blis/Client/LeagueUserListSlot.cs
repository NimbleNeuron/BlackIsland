using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class LeagueUserListSlot : BaseUI
	{
		private GameObject myInfo;


		private Text myLP;


		private Text myName;


		private GameObject userInfo;


		private Text userLP;


		private Text userName;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			myInfo = transform.Find("MyInfo").gameObject;
			userInfo = transform.Find("UserInfo").gameObject;
			myName = GameUtil.Bind<Text>(gameObject, "MyInfo/Txt_Username");
			myLP = GameUtil.Bind<Text>(gameObject, "MyInfo/Txt_Lp");
			userName = GameUtil.Bind<Text>(gameObject, "UserInfo/Txt_Username");
			userLP = GameUtil.Bind<Text>(gameObject, "UserInfo/Txt_Lp");
		}


		public void SetMyInfo(string nickName, int mmr)
		{
			myInfo.SetActive(true);
			userInfo.SetActive(false);
			myName.text = nickName;
			myLP.text = mmr.ToString();
		}


		public void SetUserInfo(string nickName, int mmr)
		{
			myInfo.SetActive(false);
			userInfo.SetActive(true);
			userName.text = nickName;
			userLP.text = mmr.ToString();
		}
	}
}