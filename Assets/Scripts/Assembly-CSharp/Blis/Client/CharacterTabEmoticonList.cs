using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterTabEmoticonList : BasePage
	{
		[SerializeField] private GameObject cloneTarget = default;


		[SerializeField] private List<EmotionEquipSlot> equipSlot = default;


		// [TupleElementNames(new string[]
		// {
		// 	"EnableColor",
		// 	"DisableColor"
		// })]
		// private ValueTuple<Color, Color> buttonColor = new ValueTuple<Color, Color>(Color.white, new Color32(118, 118, 118, byte.MaxValue));

		// co: Tuple
		private readonly (Color EnableColor, Color DisableColor) buttonColor = (Color.white,
			new Color32(118, 118, 118, byte.MaxValue));


		private readonly List<EmotionPlateType> enumData = new List<EmotionPlateType>();


		private readonly Dictionary<EmotionPlateType, EmotionIconData> fakeEquipSlotData =
			new Dictionary<EmotionPlateType, EmotionIconData>(
				SingletonComparerEnum<EmotionslotTypeComparer, EmotionPlateType>.Instance);


		private readonly List<EmotionSelectSlot> slot = new List<EmotionSelectSlot>();


		private Button cancelButton = default;


		private Image cancelLock = default;


		private Text cancelText = default;


		private Toggle hasEmotionToggle = default;


		private bool isHasEmotion;


		private bool isInit;


		private Button resetButton = default;


		private Button saveButton = default;


		private Image saveLock = default;


		private Text saveText = default;


		private RectTransform scrollViewParent = default;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			Bind();
			AddEvent();
			InitSlot();
		}


		private void Bind()
		{
			scrollViewParent = GameUtil.Bind<RectTransform>(gameObject, "EmotionScrollView/Viewport/Content");
			saveButton = GameUtil.Bind<Button>(gameObject, "EmotionEquipment/BtnList/Btn_Save");
			resetButton = GameUtil.Bind<Button>(gameObject, "EmotionEquipment/BtnList/Btn_Reset");
			cancelButton = GameUtil.Bind<Button>(gameObject, "EmotionEquipment/BtnList/Btn_Cancel");
			saveLock = GameUtil.Bind<Image>(gameObject, "EmotionEquipment/BtnList/Btn_Save/Lock");
			cancelLock = GameUtil.Bind<Image>(gameObject, "EmotionEquipment/BtnList/Btn_Cancel/Lock");
			saveText = GameUtil.Bind<Text>(gameObject, "EmotionEquipment/BtnList/Btn_Save/Txt");
			cancelText = GameUtil.Bind<Text>(gameObject, "EmotionEquipment/BtnList/Btn_Cancel/Txt");
			hasEmotionToggle = GameUtil.Bind<Toggle>(gameObject, "HaveItemCheck/CheckBox");
		}


		private void AddEvent()
		{
			hasEmotionToggle.isOn = isHasEmotion = true;
			hasEmotionToggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				isHasEmotion = isOn;
				Refresh();
			});
			saveButton.onClick.AddListener(OnClickSave);
			resetButton.onClick.AddListener(OnClickReset);
			cancelButton.onClick.AddListener(OnClickCancel);
		}


		private void InitSlot()
		{
			for (EmotionPlateType emotionPlateType = EmotionPlateType.Center;
				emotionPlateType <= EmotionPlateType.DEAD;
				emotionPlateType++)
			{
				enumData.Add(emotionPlateType);
			}

			for (int i = 0; i < equipSlot.Count; i++)
			{
				EmotionEquipSlot emotionEquipSlot = equipSlot[i];
				emotionEquipSlot.SetIconParent(transform);
				emotionEquipSlot.SetAction(Equip, UnEquip);
				emotionEquipSlot.SetInfo(enumData[i]);
			}
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			if (isInit)
			{
				Refresh();
				return;
			}

			RequestDelegate.request<InventoryApi.EmotionResult>(InventoryApi.GetInventoryEmotion(),
				delegate(RequestDelegateError err, InventoryApi.EmotionResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("ServerError/" + err.message), delegate { },
							new Popup.Button
							{
								text = Ln.Get("확인")
							});
						return;
					}

					Lobby.inst.SetEquipEmotionList(res.userEmotionSlots);
					fakeEquipSlotData.Clear();
					Refresh();
					isInit = true;
				});
		}


		protected override void OnClosePage()
		{
			base.OnClosePage();
		}


		private void Refresh()
		{
			RefreshCell();
			RefreshEquipSlot();
			RefreshButton();
		}


		private void RefreshCell()
		{
			List<EmotionIconData> emotionIconData = GameDB.emotionIcon.GetEmotionIconData(isHasEmotion);
			if (emotionIconData.Count > slot.Count)
			{
				int num = emotionIconData.Count - slot.Count;
				for (int i = 0; i < num; i++)
				{
					GameObject gameObject = Instantiate<GameObject>(cloneTarget, scrollViewParent);
					EmotionSelectSlot emotionSelectSlot =
						gameObject != null ? gameObject.GetComponent<EmotionSelectSlot>() : null;
					if (emotionSelectSlot != null)
					{
						emotionSelectSlot.SetIconParent(transform);
						emotionSelectSlot.SetEquipAction(Equip);
						slot.Add(emotionSelectSlot);
					}
				}
			}

			for (int j = 0; j < slot.Count; j++)
			{
				bool flag = j < emotionIconData.Count;
				slot[j].gameObject.SetActive(flag);
				if (flag)
				{
					slot[j].SetSlot(emotionIconData[j]);
				}
			}
		}


		private void RefreshEquipSlot()
		{
			for (int i = 0; i < equipSlot.Count; i++)
			{
				EmotionIconData emotionIconData;
				fakeEquipSlotData.TryGetValue(enumData[i], out emotionIconData);
				if (emotionIconData != null)
				{
					int num = emotionIconData != null ? emotionIconData.code : int.MinValue;
					equipSlot[i].SetSlot(num);
				}
				else
				{
					InventoryApi.UserEmoticonSlot equipEmotionList = Lobby.inst.GetEquipEmotionList(equipSlot[i].Type);
					int num2 = equipEmotionList != null && equipEmotionList.emotionCode != 0
						? equipEmotionList.emotionCode
						: int.MinValue;
					equipSlot[i].SetSlot(num2);
				}
			}
		}


		private void RefreshButton()
		{
			bool flag = IsChangeDB();
			saveButton.interactable = flag;
			saveLock.gameObject.SetActive(!flag);
			saveText.color = flag ? buttonColor.Item1 : buttonColor.Item2;
			bool flag2 = IsChangeFakeData();
			cancelButton.interactable = flag2;
			cancelLock.gameObject.SetActive(!flag2);
			cancelText.color = flag2 ? buttonColor.Item1 : buttonColor.Item2;
		}


		private void Equip(EmotionIconData data, Vector2 position, EmotionPlateType equipSlotType, bool isForce)
		{
			for (int i = 0; i < equipSlot.Count; i++)
			{
				if (isForce)
				{
					if (equipSlot[i].Data == null)
					{
						Singleton<SoundControl>.inst.PlayUISound("oui_emotion_v2");
						fakeEquipSlotData[equipSlot[i].Type] = data.Clone();
						Refresh();
						return;
					}
				}
				else if (equipSlot[i].Collider.OverlapPoint(position))
				{
					if (equipSlotType != EmotionPlateType.None)
					{
						EmotionIconData value = data.Clone();
						if (equipSlot[i].Data != null)
						{
							EmotionIconData value2 = equipSlot[i].Data.Clone();
							fakeEquipSlotData[equipSlot[i].Type] = value;
							fakeEquipSlotData[equipSlotType] = value2;
						}
						else
						{
							fakeEquipSlotData[equipSlot[i].Type] = value;
							fakeEquipSlotData[equipSlotType] = EmotionIconData.CloneEmpty();
						}
					}
					else
					{
						fakeEquipSlotData[equipSlot[i].Type] = data.Clone();
					}

					Singleton<SoundControl>.inst.PlayUISound("oui_emotion_v2");
					Refresh();
					return;
				}
			}
		}


		private void UnEquip(EmotionPlateType type, Vector2 position)
		{
			if (position != Vector2.zero)
			{
				for (int i = 0; i < equipSlot.Count; i++)
				{
					if (equipSlot[i].Collider.OverlapPoint(position))
					{
						return;
					}
				}
			}

			Singleton<SoundControl>.inst.PlayUISound("oui_emotion_v3");
			fakeEquipSlotData[type] = EmotionIconData.CloneEmpty();
			Refresh();
		}


		private void OnClickSave()
		{
			if (fakeEquipSlotData.Count <= 0)
			{
				return;
			}

			OnSave(OnClickCancel);
		}


		private void OnClickReset()
		{
			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("설정된 이모티콘을 초기화하시겠습니까?"), delegate { }, new Popup.Button
			{
				text = Ln.Get("확인"),
				callback = delegate
				{
					List<InventoryApi.UserEmoticonSlot> resultParam = new List<InventoryApi.UserEmoticonSlot>();
					equipSlot.ForEach(delegate(EmotionEquipSlot x)
					{
						resultParam.Add(new InventoryApi.UserEmoticonSlot(x.Type, 0));
					});
					SendApiData(resultParam);
				}
			}, new Popup.Button
			{
				text = Ln.Get("취소")
			});
		}


		public void OnClickCancel()
		{
			fakeEquipSlotData.Clear();
			Refresh();
		}


		public void OnSave(Action callback)
		{
			List<InventoryApi.UserEmoticonSlot> resultParam = new List<InventoryApi.UserEmoticonSlot>();
			equipSlot.ForEach(delegate(EmotionEquipSlot x)
			{
				resultParam.Add(new InventoryApi.UserEmoticonSlot(x.Type, x.Data != null ? x.Data.code : 0));
			});
			SendApiData(resultParam, callback);
		}


		private void SendApiData(List<InventoryApi.UserEmoticonSlot> resultParam, Action callback = null)
		{
			RequestDelegate.request<InventoryApi.EmotionResult>(InventoryApi.SetInventoryEmotionSlot(resultParam),
				delegate(RequestDelegateError err, InventoryApi.EmotionResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("ServerError/" + err.message), delegate { },
							new Popup.Button
							{
								text = Ln.Get("확인")
							});
						return;
					}

					Lobby.inst.SetEquipEmotionList(res.userEmotionSlots);
					fakeEquipSlotData.Clear();
					Refresh();
					Action callback2 = callback;
					if (callback2 == null)
					{
						return;
					}

					callback2();
				});
		}


		public bool IsChangeDB()
		{
			if (!isInit)
			{
				return false;
			}

			bool result = false;
			for (int i = 0; i < equipSlot.Count; i++)
			{
				InventoryApi.UserEmoticonSlot equipEmotionList = Lobby.inst.GetEquipEmotionList(equipSlot[i].Type);
				int num = equipEmotionList != null && equipEmotionList.emotionCode != 0
					? equipEmotionList.emotionCode
					: int.MinValue;
				int num2 = equipSlot[i].Data != null ? equipSlot[i].Data.code : int.MinValue;
				if (num != num2)
				{
					result = true;
					break;
				}
			}

			return result;
		}


		private bool IsChangeFakeData()
		{
			return fakeEquipSlotData.Count > 0;
		}
	}
}