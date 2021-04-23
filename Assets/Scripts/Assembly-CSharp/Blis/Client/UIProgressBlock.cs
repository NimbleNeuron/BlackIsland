using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIProgressBlock : BaseControl
	{
		private List<Block> blocks;


		private int curBlock;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			Transform transform = GameUtil.Bind<Transform>(gameObject, "Grid");
			blocks = new List<Block>();
			for (int i = 0; i < transform.childCount; i++)
			{
				blocks.Add(new Block(transform.GetChild(i)));
			}
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			ClearBlock();
		}


		public void ClearBlock()
		{
			blocks.ForEach(delegate(Block x) { x.SetClear(); });
		}


		private void ActiveBlock(int count)
		{
			for (int i = 0; i < blocks.Count; i++)
			{
				if (i < count)
				{
					blocks[i].SetFill(1f);
				}
				else
				{
					blocks[i].SetClear();
				}
			}
		}


		public void FillBlock(int index, float value)
		{
			ActiveBlock(index);
			if (index < blocks.Count)
			{
				blocks[index].SetFill(value);
			}
		}


		private class Block
		{
			private readonly RectTransform bg;


			private readonly Image fill;


			private readonly Image glow;


			private readonly Image on;

			public Block(Transform t)
			{
				bg = GameUtil.Bind<RectTransform>(t.gameObject, "Bg");
				fill = GameUtil.Bind<Image>(t.gameObject, "Fill");
				on = GameUtil.Bind<Image>(t.gameObject, "On");
				glow = GameUtil.Bind<Image>(t.gameObject, "Bg/Glow");
			}


			private void SetAlpha(Image img, float alpha)
			{
				Color color = img.color;
				color.a = alpha;
				img.color = color;
			}


			public void SetClear()
			{
				fill.fillAmount = 0f;
				bg.offsetMax = -Vector2.one;
				bg.offsetMin = Vector2.one;
				SetAlpha(on, 0f);
				SetAlpha(glow, 0f);
			}


			public void SetFill(float value)
			{
				bg.offsetMax = Vector2.zero;
				bg.offsetMin = Vector2.zero;
				fill.fillAmount = value;
				if (value < 1f)
				{
					SetAlpha(on, 0f);
					return;
				}

				SetAlpha(on, 1f);
			}


			public void SetGlow(Color color)
			{
				glow.color = color;
			}
		}
	}
}