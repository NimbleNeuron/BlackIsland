using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blis.Client
{
	public class StateSlot : Slot
	{
		[SerializeField] private SlotType slotType;


		private GameObject cacheGo;


		private CharacterStateValue characterState;


		public new GameObject gameObject {
			get
			{
				if (cacheGo == null)
				{
					cacheGo = base.gameObject;
				}

				return cacheGo;
			}
		}


		public CharacterStateValue CharacterState => characterState;


		public SlotType GetSlotType()
		{
			return slotType;
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
		}


		public void SetState(CharacterStateValue characterState)
		{
			this.characterState = characterState;
		}


		public void SetSlotType(SlotType slotType)
		{
			this.slotType = slotType;
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			if (!eventData.dragging)
			{
				MonoBehaviourInstance<Tooltip>.inst.ShowStateEffectTooltip(characterState, slotType);
			}
		}
	}
}