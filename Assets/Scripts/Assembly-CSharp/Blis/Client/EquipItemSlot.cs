using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class EquipItemSlot : ItemSlot
	{
		private ColorTweener sourceItemFrame;


		private Image sourceItemFrameImg;


		private LnText txtBulletStack;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			txtBulletStack = GameUtil.Bind<LnText>(gameObject, "BulletStack");
			sourceItemFrameImg = GameUtil.Bind<Image>(gameObject, "SourceItemFrame");
			sourceItemFrame = GameUtil.Bind<ColorTweener>(gameObject, "SourceItemFrame");
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
		}


		public void PlaySourceItemFrame()
		{
			sourceItemFrameImg.color = new Color(0f, 0.827f, 1f, 0f);
			sourceItemFrameImg.enabled = true;
			sourceItemFrame.enabled = true;
			sourceItemFrame.PlayAnimation();
		}


		public void StopSourceItemFrame()
		{
			sourceItemFrame.StopAnimation();
			sourceItemFrameImg.enabled = false;
			sourceItemFrame.enabled = false;
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