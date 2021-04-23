using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class InputFieldExtension : InputField
	{
		private bool isCaretEdge;


		private bool isLockInputSearch;


		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (isFocused)
			{
				EventSystem current = EventSystem.current;
				if (current && current.currentInputModule &&
				    string.IsNullOrEmpty(current.currentInputModule.input.compositionString) &&
				    caretPosition != textComponent.text.Length)
				{
					textComponent.text = text;
				}
			}
		}

		public void SetLockInputSearch(bool isLockInputSearch)
		{
			this.isLockInputSearch = isLockInputSearch;
		}


		public override void OnSubmit(BaseEventData eventData)
		{
			if (!IsActive() || !IsInteractable())
			{
				return;
			}

			if (Input.GetKeyDown(KeyCode.Space))
			{
				return;
			}

			base.OnSubmit(eventData);
		}


		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			if (isLockInputSearch && MonoBehaviourInstance<GameInput>.inst != null)
			{
				MonoBehaviourInstance<GameInput>.inst.SetLockInputSearch(true);
			}
		}


		public override void OnUpdateSelected(BaseEventData eventData)
		{
			base.OnUpdateSelected(eventData);
			EventSystem current = EventSystem.current;
			if (Ln.GetCurrentLanguage() == SupportLanguage.Korean && m_CaretPosition != m_CaretSelectPosition &&
			    !string.IsNullOrEmpty(current.currentInputModule.input.compositionString))
			{
				text = "";
			}
		}


		protected override void Append(char input)
		{
			if ((input >= '가' && input <= '힯' || input >= '㄰' && input <= '㆏') && m_CaretPosition == m_DrawEnd - 1)
			{
				isCaretEdge = true;
			}

			base.Append(input);
			EventSystem current = EventSystem.current;
			if (input >= '가' && input <= '힯' || input >= '㄰' && input <= '㆏')
			{
				if (m_DrawEnd != 1 && !isCaretEdge && m_CaretPosition <= m_DrawEnd &&
				    !string.IsNullOrEmpty(current.currentInputModule.input.compositionString))
				{
					m_CaretSelectPosition = --m_CaretPosition;
				}

				isCaretEdge = false;
			}
		}
	}
}