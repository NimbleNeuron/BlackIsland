using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class OneMoreAskPopup : BasePopup
	{
		[SerializeField] private Text txtExistKey = default;


		[SerializeField] private Text txtDesc = default;


		[SerializeField] private Text txtOk = default;


		private Action NoCallback;


		private Action OkCallback;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			enableBackShadeEvent = false;
		}


		public void Open(ShortcutInputEvent sci, Action okCallback, Action noCallBack)
		{
			OkCallback = okCallback;
			NoCallback = noCallBack;
			if (sci == null)
			{
				txtExistKey.gameObject.SetActive(false);
				txtDesc.text = Ln.Get("Shortcut/Popup/Reset");
				txtOk.text = Ln.Get("확인");
			}
			else
			{
				txtExistKey.gameObject.SetActive(true);
				List<KeyCode> keyCodeList = Singleton<LocalSetting>.inst.GetKeyCodeList(sci.gameInputEvent);
				txtExistKey.text = Singleton<LocalSetting>.inst.ConvertKeyCodeListToString(keyCodeList);
				txtDesc.text = Ln.Get("Shortcut/Popup/ExistKey");
				txtOk.text = Ln.Get("저장");
			}

			base.Open();
		}


		protected override void OnClose()
		{
			base.OnClose();
			if (NoCallback != null)
			{
				NoCallback();
			}
		}


		public void ClickOkay()
		{
			if (OkCallback != null)
			{
				OkCallback();
			}

			Close();
		}


		public void ClickNo()
		{
			Close();
		}
	}
}