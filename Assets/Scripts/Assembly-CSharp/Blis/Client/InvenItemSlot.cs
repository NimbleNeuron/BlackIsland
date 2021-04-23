using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class InvenItemSlot : ItemSlot
	{
		private static readonly Color focusColor = new Color(0f, 0.827f, 1f, 0f);


		private GameInputEvent gameInputEvent;


		private Image iconXiukai;


		private ColorTweener sourceItemFrame;


		private Image sourceItemFrameImg;


		private LnText txtBulletStack;


		private LnText txtKeyCode;


		public GameInputEvent GameInputEvent => gameInputEvent;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			sourceItemFrameImg = GameUtil.Bind<Image>(gameObject, "SourceItemFrame");
			iconXiukai = GameUtil.Bind<Image>(gameObject, "IconXiukai");
			sourceItemFrame = sourceItemFrameImg.GetComponent<ColorTweener>();
			txtKeyCode = GameUtil.Bind<LnText>(gameObject, "ShortCut/KeyCode");
			txtBulletStack = GameUtil.Bind<LnText>(gameObject, "BulletStack");
		}


		public void SetGameInputEventKey(GameInputEvent gameInputEvent)
		{
			this.gameInputEvent = gameInputEvent;
			SetInvenSlotKeyCode(Singleton<LocalSetting>.inst.GetKeyCode(gameInputEvent),
				Singleton<LocalSetting>.inst.GetCombinationKeyCode(gameInputEvent));
		}


		public override void ResetSlot()
		{
			base.ResetSlot();
			if (txtBulletStack != null)
			{
				txtBulletStack.text = "";
			}

			if (sourceItemFrameImg != null)
			{
				sourceItemFrameImg.enabled = false;
			}

			if (sourceItemFrame != null)
			{
				sourceItemFrame.enabled = false;
			}

			if (iconXiukai != null)
			{
				iconXiukai.enabled = false;
			}
		}


		public void PlaySourceItemFrame()
		{
			sourceItemFrameImg.enabled = true;
			sourceItemFrameImg.color = focusColor;
			sourceItemFrame.enabled = true;
			sourceItemFrame.PlayAnimation();
		}


		public void StopSourceItemFrame()
		{
			sourceItemFrame.StopAnimation();
			sourceItemFrameImg.enabled = false;
			sourceItemFrame.enabled = false;
		}


		public void EnableXiukaiIcon(bool enable)
		{
			iconXiukai.enabled = enable;
		}


		public void SetInvenSlotKeyCode(KeyCode keyCode, KeyCode[] combinations)
		{
			List<KeyCode> list = combinations.ToList<KeyCode>();
			list.Add(keyCode);
			string text = Singleton<LocalSetting>.inst.ConvertKeyCodeListToString(list);
			txtKeyCode.text = text;
		}


		public void SetBulletStackText(int itemCode, int bulletStack)
		{
			if (txtBulletStack != null)
			{
				int bulletCapacity = GameDB.item.GetBulletCapacity(itemCode);
				bulletStack = bulletStack >= bulletCapacity ? bulletCapacity : bulletStack;
				txtBulletStack.text = bulletStack.ToString();
			}
		}


		public void StopBulletCooldown()
		{
			float cooldown = this.cooldown.GetCooldown();
			if (cooldown > 0f)
			{
				MonoBehaviourInstance<ClientService>.inst.MyPlayer.SetBulletRemainCooldown(GetItem(), cooldown);
				this.cooldown.Init();
			}
		}


		public void UpdateBulletCooldown(float remainCooldown, float maxCooldown)
		{
			Cooldown.Init();
			Cooldown.SetCooldown(remainCooldown, maxCooldown, UICooldown.FillAmountType.BULLET_FORWARD, false);
		}
	}
}