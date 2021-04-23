using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ShortcutButton : BaseControl
	{
		[SerializeField] private ScrollRect scrollRect = default;


		[SerializeField] private Button button = default;


		[SerializeField] private Text txtTitle = default;


		[SerializeField] private Text txtKey = default;


		[SerializeField] private Text txtCombinations = default;


		private GameInputEvent gameInputEvent;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			if (scrollRect == null)
			{
				return;
			}

			OnBeginDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnBeginDrag(eventData);
			};
			OnEndDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnEndDrag(eventData);
			};
			OnDragEvent += delegate(BaseControl control, PointerEventData eventData) { scrollRect.OnDrag(eventData); };
			OnScrollEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnScroll(eventData);
			};
		}


		public override void OnPointerClick(PointerEventData eventData) { }


		public Button GetButton()
		{
			return button;
		}


		public void SetInputEvent(GameInputEvent gameInputEvent)
		{
			this.gameInputEvent = gameInputEvent;
		}


		public void SetTitle(GameInputEvent gameInputEvent)
		{
			if (txtTitle == null)
			{
				return;
			}

			txtTitle.text = Ln.Get(string.Format("Shortcut/{0}", gameInputEvent));
		}


		public void SetTextStyleNormal(string combi, string key)
		{
			txtCombinations.text = combi;
			txtKey.text = key;
		}


		public void SetTextStyleMore(string combi, string key)
		{
			if (!combi.Equals(""))
			{
				txtCombinations.text = combi + " + ";
				txtCombinations.gameObject.SetActive(true);
			}
			else
			{
				txtCombinations.text = "";
				txtCombinations.gameObject.SetActive(false);
			}

			txtKey.text = StringUtil.KeycodeToString(key);
		}


		public void OnPointerEnter(BaseEventData eventData)
		{
			if (gameInputEvent >= GameInputEvent.Active1 && gameInputEvent <= GameInputEvent.WeaponSkill ||
			    gameInputEvent >= GameInputEvent.Alpha1 && gameInputEvent <= GameInputEvent.Alpha0)
			{
				return;
			}

			MonoBehaviourInstance<Tooltip>.inst.SetLabel(Ln.Get(string.Format("{0}", gameInputEvent)));
			Vector2 vector = button.transform.position;
			vector += GameUtil.ConvertPositionOnScreenResolution(-663f, -14f);
			MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, vector, Tooltip.Pivot.LeftTop);
		}


		public void OnPointerExit(BaseEventData eventData)
		{
			MonoBehaviourInstance<Tooltip>.inst.Hide();
		}
	}
}