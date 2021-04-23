using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class TutorialController : MonoBehaviourInstance<TutorialController>
	{
		public bool successDiscaradFlag;


		public bool fromInputReload;


		public bool haveAllOwnItemFlag;


		private readonly Queue<int> dialogueQueue = new Queue<int>();


		private readonly List<TutorialQuestType> finalSurvivalGainType = new List<TutorialQuestType>();


		private readonly List<int> playedAnnounce = new List<int>();


		private bool arrowFlag;


		private bool clickNoNeed;


		private bool combineBoxElectronic;


		private bool combineBoxKnife;


		private TutorialQuestType currentTutorialQuestType;


		private bool enableHyperloop;


		private bool existHovudSources;


		private bool finalMoveToChapel;


		private bool finalMoveToFactory;


		private int highlightItemCode;


		private bool leatherFlag;


		private bool makedGatlingGun;


		private bool makedStg44;


		private PositionTweener mobaCameraPivotTP;


		private PositionTweener mobaCameraTP;


		private int monsterKillStack;


		private bool once;


		private bool openMonsterItembox;


		private Vector3 originMovaPosition;


		private bool powerUpHyperLoop;


		private List<int> powerUpItems;


		private int powerUpItemStack;


		private bool readyMotor;


		private bool restrictAreaFlag;


		private bool showAllPlayer;


		private Tutorial tutorial;


		private bool tutorialClear;


		private bool tutorialStart;


		private TutorialType tutorialType;


		private bool yelloMarkFlag;


		public TutorialType TutorialType => tutorialType;


		public List<TutorialQuestInfo> AlreadySuccessQuestInfos { get; } = new List<TutorialQuestInfo>();


		public bool MakedGatlingGun => makedGatlingGun;


		public bool ShowAllPlayer => showAllPlayer;


		public void Init(TutorialType tutorialType)
		{
			this.tutorialType = tutorialType;
			highlightItemCode = 0;
			monsterKillStack = 0;
			powerUpItemStack = 0;
			tutorialClear = false;
			tutorialStart = false;
			arrowFlag = false;
			successDiscaradFlag = false;
			fromInputReload = false;
			haveAllOwnItemFlag = false;
			restrictAreaFlag = false;
			openMonsterItembox = false;
			combineBoxKnife = false;
			combineBoxElectronic = false;
			readyMotor = false;
			existHovudSources = false;
			leatherFlag = false;
			powerUpHyperLoop = false;
			yelloMarkFlag = false;
			makedStg44 = false;
			makedGatlingGun = false;
			clickNoNeed = false;
			finalMoveToChapel = false;
			finalMoveToFactory = false;
			finalSurvivalGainType.Clear();
			enableHyperloop = GameDB.tutorial.GetTutorialSettingData(tutorialType).enableHyperloop;
		}


		public void SetTutorial(Tutorial tutorial)
		{
			this.tutorial = tutorial;
		}


		public void SetCurrentTutorialQuestType(TutorialQuestType currentTutorialQuestType)
		{
			this.currentTutorialQuestType = currentTutorialQuestType;
		}


		public void ShowMessageBoxTutorial(int group, Action closeCallback)
		{
			tutorial.ShowMessageBox(group, closeCallback);
		}


		public void AddMainQuestTutorial(int code, Action mainQuestCompletedAction)
		{
			tutorial.AddMainQuest(code, mainQuestCompletedAction);
		}


		public void AddSubQuestTutorial(int code, Action completedAction = null)
		{
			tutorial.AddSubQuest(code, completedAction);
		}


		public void AddDialogueTutorial(int code)
		{
			dialogueQueue.Enqueue(code);
		}


		public IEnumerator ShowRecursiveDialogueTutorial(Action finishCallback)
		{
			if (dialogueQueue.Count == 0)
			{
				tutorial.HideDialogue();
				if (finishCallback != null)
				{
					finishCallback();
				}

				yield break;
			}

			int code = dialogueQueue.Dequeue();
			yield return this.StartThrowingCoroutine(tutorial.ShowDialogue(code), null);
			yield return this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(finishCallback), null);
		}


		private TutorialQuestType GetTutorialQuestTypeByItemCode(int itemCode)
		{
			if (itemCode <= 117403)
			{
				if (itemCode <= 101404)
				{
					if (itemCode == 101201)
					{
						return TutorialQuestType.GainArmyKnife;
					}

					if (itemCode == 101404)
					{
						return TutorialQuestType.GainVibroblade;
					}
				}
				else
				{
					if (itemCode == 108101)
					{
						return TutorialQuestType.GainBranch;
					}

					if (itemCode == 117403)
					{
						return TutorialQuestType.GainGatlingGun;
					}
				}
			}
			else if (itemCode <= 401110)
			{
				if (itemCode == 401106)
				{
					return TutorialQuestType.GainScrapMetal;
				}

				if (itemCode == 401110)
				{
					return TutorialQuestType.GainBettery;
				}
			}
			else
			{
				if (itemCode == 401112)
				{
					return TutorialQuestType.GainOil;
				}

				if (itemCode == 401121)
				{
					return TutorialQuestType.GainGunpowder;
				}

				if (itemCode == 502104)
				{
					return TutorialQuestType.GainPianoWire;
				}
			}

			return TutorialQuestType.None;
		}


		public void UpdateInventoryTutorial(List<Item> items)
		{
			UpdateUserItems(items);
			PowerUpItemTutorial(items);
		}


		public void ShowShortCutHighlight(int targetItemCode)
		{
			if (!tutorialStart)
			{
				return;
			}

			TutorialType tutorialType = TutorialType;
			if (tutorialType != TutorialType.PowerUp)
			{
				if (tutorialType != TutorialType.FinalSurvival)
				{
					return;
				}

				if (makedStg44)
				{
					return;
				}

				if (targetItemCode == 117201)
				{
					MonoBehaviourInstance<GameUI>.inst.ShortCutCraftHud.ShowHighlightBox(true, targetItemCode);
				}
			}
			else
			{
				if (yelloMarkFlag)
				{
					MonoBehaviourInstance<GameUI>.inst.ShortCutCraftHud.ShowHighlightBox(true, targetItemCode);
					return;
				}

				yelloMarkFlag = true;
				SingletonMonoBehaviour<PlayerController>.inst.LockInput(true);
				MonoBehaviourInstance<GameUI>.inst.ShortCutCraftHud.ShowHighlightBox(true, targetItemCode);
				UISystem.Action(new CloseBox());
				AddDialogueTutorial(52);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
				{
					SingletonMonoBehaviour<PlayerController>.inst.LockInput(false);
					MonoBehaviourInstance<GameUI>.inst.ShortCutCraftHud.ShowHighlightBox(false);
				}), null);
			}
		}


		public void ClickCombineableNoNeed()
		{
			if (clickNoNeed)
			{
				return;
			}

			TutorialType tutorialType = this.tutorialType;
			if (tutorialType == TutorialType.BasicGuide || tutorialType - TutorialType.PowerUp <= 1)
			{
				clickNoNeed = true;
				int code = this.tutorialType == TutorialType.BasicGuide || this.tutorialType == TutorialType.PowerUp
					? 53
					: 68;
				AddDialogueTutorial(code);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate { clickNoNeed = false; }), null);
			}
		}


		public void UpdateCombineableTutorialArrow(List<ItemData> combineableItems)
		{
			TutorialType tutorialType = TutorialType;
			if (tutorialType == TutorialType.BasicGuide)
			{
				if (!arrowFlag)
				{
					highlightItemCode = 101201;
					return;
				}

				int currentItemCode = -1;
				if (currentTutorialQuestType == TutorialQuestType.None)
				{
					highlightItemCode = 101201;
					if (MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.GetEquipments()
						.Exists(x => x.itemCode == highlightItemCode))
					{
						AddQuestTutorialType();
						return;
					}
				}
				else if (currentTutorialQuestType == TutorialQuestType.GainArmyKnife)
				{
					currentItemCode = 101201;
					highlightItemCode = 401211;
				}
				else if (currentTutorialQuestType == TutorialQuestType.GainElectronicParts)
				{
					currentItemCode = 401211;
					highlightItemCode = 401303;
				}
				else if (currentTutorialQuestType == TutorialQuestType.GainMotor)
				{
					currentItemCode = 401303;
					highlightItemCode = 101404;
				}
				else if (currentTutorialQuestType == TutorialQuestType.GainVibroblade)
				{
					currentItemCode = 101404;
				}

				bool flag = combineableItems.Exists(x => x.code == highlightItemCode);
				MonoBehaviourInstance<GameUI>.inst.ShortCutCraftHud.ShowHighlight(flag, highlightItemCode);
				if (flag)
				{
					AddQuestTutorialType();
					return;
				}

				bool flag2 = combineableItems.Exists(x => x.code == currentItemCode);
				MonoBehaviourInstance<GameUI>.inst.ShortCutCraftHud.ShowHighlight(flag2, currentItemCode);
				if (flag2)
				{
					highlightItemCode = currentItemCode;
				}
			}
		}


		private void AddQuestTutorialType()
		{
			TutorialQuestType tutorialQuestType = currentTutorialQuestType;
			if (tutorialQuestType != TutorialQuestType.None)
			{
				switch (tutorialQuestType)
				{
					case TutorialQuestType.GainArmyKnife:
						currentTutorialQuestType = TutorialQuestType.GainElectronicParts;
						return;
					case TutorialQuestType.GainPianoWire:
						break;
					case TutorialQuestType.GainElectronicParts:
						currentTutorialQuestType = TutorialQuestType.GainMotor;
						return;
					case TutorialQuestType.GainMotor:
						currentTutorialQuestType = TutorialQuestType.GainVibroblade;
						break;
					default:
						return;
				}

				return;
			}

			currentTutorialQuestType = TutorialQuestType.GainArmyKnife;
		}


		public void UpdateEquipTutorial(List<EquipItem> equipItems)
		{
			List<Item> list = new List<Item>();
			foreach (EquipItem equipItem in equipItems)
			{
				if (equipItem.item != null)
				{
					list.Add(equipItem.item);
				}
			}

			UpdateUserItems(list);
			PowerUpItemTutorial(list);
		}


		private void UpdateUserItems(List<Item> items)
		{
			TutorialQuestType tutorialQuestType = TutorialQuestType.None;
			TutorialType tutorialType = this.tutorialType;
			if (tutorialType != TutorialType.BasicGuide)
			{
				if (tutorialType != TutorialType.FinalSurvival)
				{
					goto IL_217;
				}
			}
			else
			{
				using (List<Item>.Enumerator enumerator = items.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Item item = enumerator.Current;
						tutorialQuestType = GetTutorialQuestTypeByItemCode(item.itemCode);
						if (tutorialQuestType != TutorialQuestType.None)
						{
							if (tutorialQuestType == TutorialQuestType.GainVibroblade)
							{
								tutorial.SuccessMainQuest();
								return;
							}

							tutorial.SuccessSubQuest(tutorialQuestType);
						}
					}

					goto IL_217;
				}
			}

			foreach (Item item2 in items)
			{
				if (item2.itemCode == 117201)
				{
					if (makedStg44)
					{
						return;
					}

					makedStg44 = true;
					MonoBehaviourInstance<GameUI>.inst.ShortCutCraftHud.ShowHighlightBox(false, 117201);
					AddDialogueTutorial(60);
					this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
					{
						SingletonMonoBehaviour<PlayerController>.inst.AddWalkableAreas(14);
						MonoBehaviourInstance<GameUI>.inst.ShortCutCraftHud.ShowHighlightBox(false, 117201);
						tutorial.CreateTutorialArrowDir(new Vector3(-70.267f, 1.304f, -4.683f));
					}), null);
					return;
				}

				tutorialQuestType = GetTutorialQuestTypeByItemCode(item2.itemCode);
				if (tutorialQuestType != TutorialQuestType.None)
				{
					if (tutorialQuestType == TutorialQuestType.GainBettery ||
					    tutorialQuestType == TutorialQuestType.GainScrapMetal ||
					    tutorialQuestType == TutorialQuestType.GainOil)
					{
						if (!finalSurvivalGainType.Contains(tutorialQuestType))
						{
							finalSurvivalGainType.Add(tutorialQuestType);
							int count = finalSurvivalGainType.Count;
							if (count != 1)
							{
								if (count == 2)
								{
									AddDialogueTutorial(65);
									this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(null), null);
								}
							}
							else
							{
								AddDialogueTutorial(64);
								this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(null), null);
							}
						}
					}
					else if (tutorialQuestType == TutorialQuestType.GainGatlingGun)
					{
						if (makedGatlingGun)
						{
							return;
						}

						makedGatlingGun = true;
						enableHyperloop = true;
						List<int> list = new List<int>
						{
							11,
							13,
							14
						};
						for (int i = 1; i < 16; i++)
						{
							if (!list.Contains(i))
							{
								SingletonMonoBehaviour<PlayerController>.inst.AddWalkableAreas(i);
							}
						}

						SingletonMonoBehaviour<PlayerController>.inst.NextTutorialSequence();
					}

					tutorial.SuccessSubQuest(tutorialQuestType);
				}
			}

			IL_217:
			AlreadySuccessQuest(tutorialQuestType);
		}


		private void AlreadySuccessQuest(TutorialQuestType tutorialQuestType)
		{
			if (tutorialQuestType == TutorialQuestType.None)
			{
				return;
			}

			TutorialQuestInfo item = new TutorialQuestInfo(tutorialQuestType, true);
			if (!AlreadySuccessQuestInfos.Exists(x => x.TutorialQuestType == tutorialQuestType))
			{
				AlreadySuccessQuestInfos.Add(item);
			}
		}


		private bool CheckSuccessQuest(TutorialQuestType tutorialQuestType)
		{
			return AlreadySuccessQuestInfos.Exists(x => x.TutorialQuestType == tutorialQuestType);
		}


		public void ChangeAreaTutorial(int areaCode)
		{
			TutorialType tutorialType = this.tutorialType;
			if (tutorialType != TutorialType.BasicGuide)
			{
				if (tutorialType != TutorialType.FinalSurvival)
				{
					return;
				}

				if (areaCode == 13)
				{
					if (finalMoveToFactory)
					{
						return;
					}

					finalMoveToFactory = true;
					tutorial.HideTutorialArrowDir();
					AddDialogueTutorial(63);
					this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
					{
						AddSubQuestTutorial(11, delegate
						{
							AddDialogueTutorial(66);
							this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(null), null);
						});
					}), null);
				}
				else if (areaCode == 14)
				{
					if (finalMoveToChapel)
					{
						return;
					}

					finalMoveToChapel = true;
					tutorial.HideTutorialArrowDir();
					AddDialogueTutorial(61);
					this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
					{
						AddSubQuestTutorial(10, delegate
						{
							AddDialogueTutorial(62);
							this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
							{
								SingletonMonoBehaviour<PlayerController>.inst.AddWalkableAreas(13);
								tutorial.CreateTutorialArrowDir(new Vector3(-121.73f, -0.931f, -10.091f));
							}), null);
						});
					}), null);
				}
			}
			else if (areaCode == 14)
			{
				tutorial.SuccessSubQuest(TutorialQuestType.MoveToUptown);
			}
		}


		public void OpenBoxTutorial(List<Item> items)
		{
			TutorialType tutorialType = TutorialType;
			if (tutorialType == TutorialType.BasicGuide)
			{
				foreach (Item item in items) { }
			}
		}


		public void RunCCTVTutorial()
		{
			TutorialQuestType tutorialQuestType = TutorialQuestType.None;
			TutorialType tutorialType = TutorialType;
			if (tutorialType == TutorialType.Trace)
			{
				tutorialQuestType = TutorialQuestType.RunCCTV;
				if (CheckSuccessQuest(tutorialQuestType))
				{
					return;
				}

				tutorial.SuccessSubQuest(tutorialQuestType);
				SingletonMonoBehaviour<PlayerController>.inst.LockInput(true);
			}

			AlreadySuccessQuest(tutorialQuestType);
		}


		public void CharacterKillTutorial()
		{
			TutorialType tutorialType = TutorialType;
		}


		public void MonsterKillTutorial()
		{
			TutorialType tutorialType = TutorialType;
			if (tutorialType == TutorialType.Hunt)
			{
				monsterKillStack++;
				tutorial.AddMainQuestStack();
				if (monsterKillStack == 1)
				{
					ShowAnnounce(7, true, Color.black, false, true);
					AddSubQuestTutorial(3);
					AddDialogueTutorial(23);
					this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(null), null);
					return;
				}

				if (monsterKillStack == 4)
				{
					tutorial.SuccessMainQuest();
				}
			}
		}


		public void SuccessOpenMonsterItemBox()
		{
			if (openMonsterItembox)
			{
				return;
			}

			TutorialType tutorialType = TutorialType;
			if (tutorialType == TutorialType.Hunt)
			{
				openMonsterItembox = true;
				tutorial.SuccessSubQuest(TutorialQuestType.OpenMonsterItemBox);
			}
		}


		public void CreateReloadTutorial()
		{
			TutorialType tutorialType = this.tutorialType;
			if (tutorialType == TutorialType.Hunt)
			{
				ShowAnnounce(8, true, Color.black, false, true);
				AddSubQuestTutorial(1);
				return;
			}

			if (tutorialType != TutorialType.FinalSurvival)
			{
				return;
			}

			ShowAnnounce(8, true, Color.black, false, true);
			AddSubQuestTutorial(7);
		}


		public void SuccessReloadTutorial()
		{
			TutorialType tutorialType = this.tutorialType;
			if ((tutorialType == TutorialType.Hunt || tutorialType == TutorialType.FinalSurvival) && fromInputReload)
			{
				tutorial.SuccessSubQuest(TutorialQuestType.ReloadWeapon);
			}
		}


		public void CreateRestingTutorial()
		{
			switch (tutorialType)
			{
				case TutorialType.Hunt:
					ShowAnnounce(6, true, Color.black, false, true);
					AddSubQuestTutorial(2);
					return;
				case TutorialType.PowerUp:
					ShowAnnounce(6, true, Color.black, false, true);
					AddSubQuestTutorial(3);
					return;
				case TutorialType.FinalSurvival:
					ShowAnnounce(6, true, Color.black, false, true);
					AddSubQuestTutorial(4);
					return;
				default:
					return;
			}
		}


		public void SuccessRestingTutorial()
		{
			TutorialType tutorialType = this.tutorialType;
			if (tutorialType - TutorialType.Hunt <= 2)
			{
				tutorial.SuccessSubQuest(TutorialQuestType.Resting);
			}
		}


		public void CreateHaveAllOwnItemTutorial()
		{
			TutorialType tutorialType = this.tutorialType;
			if (tutorialType != TutorialType.PowerUp)
			{
				if (tutorialType != TutorialType.FinalSurvival)
				{
					return;
				}

				if (haveAllOwnItemFlag)
				{
					return;
				}

				haveAllOwnItemFlag = true;
				tutorial.ShowTutorialSquareNaviArea();
				AddDialogueTutorial(49);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate { tutorial.HideTutorialSquare(); }),
					null);
			}
			else
			{
				if (powerUpHyperLoop)
				{
					return;
				}

				UISystem.Action(new CloseBox());
				MonoBehaviourInstance<MobaCamera>.inst.ResetCameraPosition();
				SingletonMonoBehaviour<PlayerController>.inst.LockInput(true);
				enableHyperloop = true;
				AddDialogueTutorial(51);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
				{
					powerUpHyperLoop = true;
					if (this.tutorialType == TutorialType.PowerUp)
					{
						ShowAnnounce(10, true, Color.black);
						AddSubQuestTutorial(1);
					}
					else
					{
						AddSubQuestTutorial(8);
					}

					SingletonMonoBehaviour<PlayerController>.inst.LockInput(false);
					mobaCameraTP = MonoBehaviourInstance<MobaCamera>.inst.GetComponent<PositionTweener>();
					MovaCameraGoPowerupTutorial();
				}), null);
			}
		}


		public void CreateRestricAreaTutorial()
		{
			if (restrictAreaFlag)
			{
				return;
			}

			TutorialType tutorialType = this.tutorialType;
			if (tutorialType == TutorialType.FinalSurvival)
			{
				restrictAreaFlag = true;
				ShowAnnounce(12, true, Color.black);
				AddDialogueTutorial(34);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(null), null);
			}
		}


		private void CreatePowerUpMainQuestItems()
		{
			powerUpItems = (from x in MonoBehaviourInstance<GameUI>.inst.NavigationHud.TargetItemList
				select x.code).ToList<int>();
		}


		public void PowerUpItemTutorial(List<Item> items)
		{
			if (powerUpItems == null)
			{
				return;
			}

			TutorialType tutorialType = TutorialType;
			if (tutorialType == TutorialType.PowerUp)
			{
				using (List<Item>.Enumerator enumerator = items.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Item item = enumerator.Current;
						if (item != null && powerUpItems.Exists(x => x == item.itemCode))
						{
							powerUpItemStack++;
							tutorial.AddMainQuestStack();
							powerUpItems.Remove(item.itemCode);
							if (powerUpItemStack == 1)
							{
								tutorial.ShowTutorialSquareNavi();
								AddDialogueTutorial(54);
								this.StartThrowingCoroutine(
									ShowRecursiveDialogueTutorial(delegate { tutorial.HideTutorialSquare(); }), null);
							}
							else if (powerUpItemStack == 2)
							{
								tutorial.SuccessMainQuest();
							}
						}
					}
				}
			}
		}


		public void CreateDiscardItemTutorial(int itemCount)
		{
			if (!tutorialStart)
			{
				return;
			}

			if (successDiscaradFlag)
			{
				return;
			}

			TutorialType tutorialType = this.tutorialType;
			if (tutorialType != TutorialType.BasicGuide)
			{
				if (tutorialType - TutorialType.PowerUp > 1)
				{
					return;
				}

				int code = this.tutorialType == TutorialType.PowerUp ? 56 : 48;
				if (itemCount < 10)
				{
					tutorial.HideTutorialSquare();
					return;
				}

				AddDialogueTutorial(code);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(null), null);
				tutorial.ShowTutorialSquareInven();
				int code2 = this.tutorialType == TutorialType.PowerUp ? 2 : 5;
				AddSubQuestTutorial(code2, delegate { tutorial.HideTutorialSquare(); });
			}
			else
			{
				if (itemCount < 10)
				{
					MonoBehaviourInstance<GameUI>.inst.InventoryHud.ShowTutorialSquareInven(false);
					return;
				}

				AddDialogueTutorial(56);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
				{
					MonoBehaviourInstance<GameUI>.inst.InventoryHud.ShowTutorialSquareInven(false);
					successDiscaradFlag = true;
				}), null);
				MonoBehaviourInstance<GameUI>.inst.InventoryHud.ShowTutorialSquareInven(true);
			}
		}


		public void SuccessDiscardItemTutorial()
		{
			TutorialType tutorialType = this.tutorialType;
			if (tutorialType - TutorialType.PowerUp <= 1)
			{
				tutorial.SuccessSubQuest(TutorialQuestType.DiscardItem);
				successDiscaradFlag = true;
			}
		}


		public void SuccessHyperloopTutorial()
		{
			TutorialType tutorialType = this.tutorialType;
			if (tutorialType - TutorialType.PowerUp <= 1)
			{
				tutorial.SuccessSubQuest(TutorialQuestType.UseHyperloop);
			}
		}


		public void SuccessOpenMasteryWindowTutorial()
		{
			TutorialType tutorialType = this.tutorialType;
			if (tutorialType == TutorialType.FinalSurvival)
			{
				MonoBehaviourInstance<GameUI>.inst.StatusHud.ShowTutorialSquareWeaponType(false);
				tutorial.SuccessSubQuest(TutorialQuestType.OpenMasteryWindow);
			}
		}


		public void SuccessOpenMapWindowTutorial()
		{
			TutorialType tutorialType = TutorialType;
			if (tutorialType == TutorialType.FinalSurvival)
			{
				tutorial.SuccessSubQuest(TutorialQuestType.OpenMapWindow);
			}
		}


		public void SuccessRifleWeaponMasteryLevelSevenTutorial()
		{
			TutorialType tutorialType = this.tutorialType;
			if (tutorialType == TutorialType.FinalSurvival)
			{
				tutorial.SuccessSubQuest(TutorialQuestType.RifleWeaponMasteryLevelSeven);
			}
		}


		public void SuccessLearnWeaponSkill()
		{
			TutorialType tutorialType = this.tutorialType;
			if (tutorialType == TutorialType.FinalSurvival)
			{
				tutorial.SuccessSubQuest(TutorialQuestType.LearnWeaponSkill);
			}
		}


		public void ShowTutorialBoxCombine(int itemCode)
		{
			if (itemCode == 101201)
			{
				ShowTutorialBoxCombineKnife();
				return;
			}

			if (itemCode == 401211)
			{
				ShowTutorialBoxCombineElectronic();
			}
		}


		public void ShowTutorialBoxCombineKnife()
		{
			if (combineBoxKnife)
			{
				return;
			}

			TutorialType tutorialType = this.tutorialType;
			if (tutorialType == TutorialType.BasicGuide)
			{
				AddDialogueTutorial(35);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(null), null);
				MonoBehaviourInstance<GameUI>.inst.CombineWindow.ShowTutorialBoxCombine(true);
				MonoBehaviourInstance<GameUI>.inst.NavigationHud.ShowTutorialBoxNavi(false, 101201);
			}
		}


		public void FinishTutorialBoxCombine(int itemCode)
		{
			TutorialType tutorialType = TutorialType;
			if (tutorialType == TutorialType.BasicGuide)
			{
				if (itemCode == 101201)
				{
					combineBoxKnife = true;
					MonoBehaviourInstance<GameUI>.inst.CombineWindow.ShowTutorialBoxCombine(false);
					return;
				}

				if (itemCode == 401211)
				{
					combineBoxElectronic = true;
					MonoBehaviourInstance<GameUI>.inst.CombineWindow.ShowTutorialBoxCombine(false);
				}
			}
		}


		public void ShowTutorialBoxCombineElectronic()
		{
			if (combineBoxElectronic)
			{
				return;
			}

			TutorialType tutorialType = this.tutorialType;
			if (tutorialType == TutorialType.BasicGuide)
			{
				MonoBehaviourInstance<GameUI>.inst.CombineWindow.ShowTutorialBoxCombine(true);
				MonoBehaviourInstance<GameUI>.inst.NavigationHud.ShowTutorialBoxNavi(false, 401211);
			}
		}


		public void ShowTutorialMotor()
		{
			if (readyMotor)
			{
				return;
			}

			TutorialType tutorialType = this.tutorialType;
			if (tutorialType == TutorialType.BasicGuide)
			{
				readyMotor = true;
				AddDialogueTutorial(36);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(null), null);
			}
		}


		public bool DontThrowItems(int itemCode)
		{
			List<int> list = new List<int>
			{
				101104,
				101201
			};
			TutorialType tutorialType = this.tutorialType;
			return tutorialType == TutorialType.BasicGuide && list.Contains(itemCode);
		}


		public void OnGameStarted()
		{
			if (once)
			{
				return;
			}

			once = true;
			tutorialStart = true;
			switch (TutorialType)
			{
				case TutorialType.BasicGuide:
					ScenarioBasicGuide();
					return;
				case TutorialType.Trace:
					ScenarioTrace();
					return;
				case TutorialType.Hunt:
					ScenarioHunt();
					return;
				case TutorialType.PowerUp:
					ScenarioPowerUp();
					return;
				case TutorialType.FinalSurvival:
					ScenarioFinalSurvival();
					return;
				default:
					return;
			}
		}


		public bool CheckTutorialClear()
		{
			switch (tutorialType)
			{
				case TutorialType.BasicGuide:
				case TutorialType.Hunt:
				case TutorialType.PowerUp:
					return tutorialClear;
				case TutorialType.Trace:
				case TutorialType.FinalSurvival:
					return true;
				default:
					return false;
			}
		}


		private void ScenarioBasicGuide()
		{
			GameDB.tutorial.InitQuestDataListBasicGuide();
			ShowMessageBoxTutorial(11, delegate
			{
				SingletonMonoBehaviour<PlayerController>.inst.LockInput(true);
				AddDialogueTutorial(1);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
				{
					tutorial.ShowTutorialSquareEquip();
					AddDialogueTutorial(2);
					this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
					{
						tutorial.HideTutorialSquare();
						tutorial.ShowTutorialSquareNavi();
						AddDialogueTutorial(3);
						this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
						{
							tutorial.HideTutorialSquare();
							ShowAnnounce(1, true, Color.black, true);
							ShowMessageBoxTutorial(1, delegate
							{
								SingletonMonoBehaviour<PlayerController>.inst.LockInput(false);
								tutorial.ShowTutorialSquareNaviArea();
								AddDialogueTutorial(4);
								this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
								{
									tutorial.HideTutorialSquare();
									AddDialogueTutorial(5);
									this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
									{
										ShowMessageBoxTutorial(2, delegate
										{
											AddMainQuestTutorial(1, delegate
											{
												AddDialogueTutorial(14);
												this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
												{
													tutorialClear = true;
													MonoBehaviourInstance<ClientService>.inst.EndTutorial(1,
														ObjectType.None,
														MonoBehaviourInstance<ClientService>.inst.MyObjectId, 0,
														string.Empty);
												}), null);
											});
											AddSubQuestTutorial(1, delegate
											{
												AddDialogueTutorial(6);
												this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
												{
													ShowAnnounce(2, true, Color.black, true);
													ShowMessageBoxTutorial(3, delegate
													{
														AddDialogueTutorial(7);
														this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(null),
															null);
														AddSubQuestTutorial(2, delegate
														{
															AddDialogueTutorial(8);
															this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(
																delegate
																{
																	arrowFlag = true;
																	UpdateCombineableTutorialArrow(
																		MonoBehaviourInstance<GameUI>.inst
																			.ShortCutCraftHud.CombineableItems);
																	AddSubQuestTutorial(3, delegate
																	{
																		tutorial.ShowTutorialSquareEquip();
																		AddDialogueTutorial(9);
																		this.StartThrowingCoroutine(
																			ShowRecursiveDialogueTutorial(delegate
																			{
																				tutorial.HideTutorialSquare();
																				AddDialogueTutorial(10);
																				AddDialogueTutorial(11);
																				this.StartThrowingCoroutine(
																					ShowRecursiveDialogueTutorial(
																						delegate
																						{
																							tutorial
																								.CreateTutorialArrowDir(
																									new Vector3(-113.2f,
																										0.9f, -10.7f));
																							SingletonMonoBehaviour<
																									PlayerController>
																								.inst
																								.AddWalkableAreas(14);
																							AddSubQuestTutorial(4,
																								delegate
																								{
																									tutorial
																										.HideTutorialArrowDir();
																									tutorial
																										.HideTutorialSquare();
																									AddDialogueTutorial(
																										12);
																									this
																										.StartThrowingCoroutine(
																											ShowRecursiveDialogueTutorial(
																												delegate
																												{
																													AddSubQuestTutorial(
																														5,
																														delegate
																														{
																															AddDialogueTutorial(
																																13);
																															ShowAnnounce(
																																3,
																																true,
																																Color
																																	.black);
																															this
																																.StartThrowingCoroutine(
																																	ShowRecursiveDialogueTutorial(
																																		null),
																																	null);
																														});
																												}),
																											null);
																								});
																						}), null);
																			}), null);
																	});
																}), null);
														});
													});
												}), null);
											});
										});
									}), null);
								}), null);
							});
						}), null);
					}), null);
				}), null);
			});
		}


		private void ScenarioTrace()
		{
			GameDB.tutorial.InitQuestDataListTrace();
			ShowMessageBoxTutorial(4, delegate
			{
				MonoBehaviourInstance<MobaCamera>.inst.ResetCameraPosition();
				SingletonMonoBehaviour<PlayerController>.inst.LockInput(true);
				AddDialogueTutorial(15);
				AddDialogueTutorial(16);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
				{
					SingletonMonoBehaviour<PlayerController>.inst.LockInput(false);
					ShowAnnounce(5, true, Color.black, true);
					AddDialogueTutorial(18);
					this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(null), null);
					AddMainQuestTutorial(2, null);
					AddSubQuestTutorial(1, delegate
					{
						mobaCameraPivotTP =
							MonoBehaviourInstance<MobaCamera>.inst.Pivot.GetComponent<PositionTweener>();
						MovaCameraGoTraceTutorial();
					});
				}), null);
			});
		}


		private void ScenarioHunt()
		{
			GameDB.tutorial.InitQuestDataListHunt();
			tutorial.InitMainQuestStack();
			ShowMessageBoxTutorial(5, delegate
			{
				AddDialogueTutorial(22);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
				{
					AddMainQuestTutorial(3, delegate
					{
						tutorialClear = true;
						MonoBehaviourInstance<ClientService>.inst.EndTutorial(1, ObjectType.None,
							MonoBehaviourInstance<ClientService>.inst.MyObjectId, 0, string.Empty);
					});
				}), null);
			});
		}


		private void ScenarioPowerUp()
		{
			GameDB.tutorial.InitQuestDataListPowerUp();
			tutorial.InitMainQuestStack();
			ShowMessageBoxTutorial(8, delegate
			{
				SingletonMonoBehaviour<PlayerController>.inst.LockInput(true);
				tutorial.ShowTutorialSquareNaviArea();
				AddDialogueTutorial(50);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
				{
					tutorial.HideTutorialSquare();
					AddDialogueTutorial(70);
					this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
					{
						SingletonMonoBehaviour<PlayerController>.inst.LockInput(false);
						CreatePowerUpMainQuestItems();
						AddMainQuestTutorial(4, delegate
						{
							AddDialogueTutorial(55);
							this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
							{
								tutorialClear = true;
								MonoBehaviourInstance<ClientService>.inst.EndTutorial(1, ObjectType.None,
									MonoBehaviourInstance<ClientService>.inst.MyObjectId, 0, string.Empty);
							}), null);
						});
					}), null);
				}), null);
			});
		}


		private void ScenarioFinalSurvival()
		{
			GameDB.tutorial.InitQuestDataListFinalSurvival();
			ShowMessageBoxTutorial(10, delegate
			{
				SingletonMonoBehaviour<PlayerController>.inst.LockInput(true);
				MonoBehaviourInstance<GameUI>.inst.ProductionGoalWindow.FinalSurvivalOpen();
				AddDialogueTutorial(57);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
				{
					ShowAnnounce(13, true, Color.black);
					SingletonMonoBehaviour<PlayerController>.inst.LockInput(false);
					MonoBehaviourInstance<GameUI>.inst.ProductionGoalWindow.FinalSurvivalClose();
					AddDialogueTutorial(58);
					this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(null), null);
					AddMainQuestTutorial(5, null);
					AddSubQuestTutorial(12, delegate
					{
						ShowAnnounce(14, true, Color.black);
						AddDialogueTutorial(67);
						this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
						{
							showAllPlayer = true;
							foreach (PlayerContext playerContext in
								MonoBehaviourInstance<ClientService>.inst.GetPlayers())
							{
								playerContext.Character.ShowMapIcon(MiniMapIconType.Sight);
								playerContext.Character.ShowMiniMapIcon(MiniMapIconType.Sight);
							}
						}), null);
					});
					AddSubQuestTutorial(9, delegate
					{
						MonoBehaviourInstance<GameUI>.inst.ShortCutCraftHud.ShowHighlightBox(true, 117201);
						AddDialogueTutorial(59);
						this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(null), null);
					});
				}), null);
			});
		}


		private void MovaCameraGoTraceTutorial()
		{
			MonoBehaviourInstance<MobaCamera>.inst.ResetCameraPosition();
			SingletonMonoBehaviour<PlayerController>.inst.LockInput(true);
			mobaCameraPivotTP.enabled = false;
			mobaCameraPivotTP.from = Vector3.zero;
			mobaCameraPivotTP.to = new Vector3(-3.47f, 0f, 32f);
			mobaCameraPivotTP.speed = 2f;
			mobaCameraPivotTP.PlayAnimation();
			mobaCameraPivotTP.enabled = true;
			mobaCameraPivotTP.OnAnimationFinish += MoveFinishedTraceTutorial;
		}


		private void MovaCameraBackTraceTutorial()
		{
			MonoBehaviourInstance<MobaCamera>.inst.ResetCameraPosition();
			mobaCameraPivotTP.enabled = false;
			mobaCameraPivotTP.from = new Vector3(-3.47f, 0f, 32f);
			mobaCameraPivotTP.to = Vector3.zero;
			mobaCameraPivotTP.speed = 2f;
			mobaCameraPivotTP.PlayAnimation();
			mobaCameraPivotTP.enabled = true;
			mobaCameraPivotTP.OnAnimationFinish -= MoveFinishedTraceTutorial;
			mobaCameraPivotTP.OnAnimationFinish += delegate
			{
				SingletonMonoBehaviour<PlayerController>.inst.LockInput(false);
				AddDialogueTutorial(20);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(delegate
				{
					tutorial.ShowTutorialSquareSkill();
					AddDialogueTutorial(21);
					this.StartThrowingCoroutine(
						ShowRecursiveDialogueTutorial(delegate { tutorial.HideTutorialSquare(); }), null);
				}), null);
			};
		}


		private void MoveFinishedTraceTutorial()
		{
			this.StartThrowingCoroutine(CorFinishedTraceTutorial(), null);
		}


		private IEnumerator CorFinishedTraceTutorial()
		{
			SingletonMonoBehaviour<PlayerController>.inst.NextTutorialSequence();
			yield return new WaitForSeconds(tutorial.waitNadineInHunt);
			AddDialogueTutorial(19);
			this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(null), null);
			yield return new WaitForSeconds(tutorial.waitMagHuynwooInHunt);
			MovaCameraBackTraceTutorial();
		}


		private void MovaCameraGoPowerupTutorial()
		{
			MonoBehaviourInstance<MobaCamera>.inst.ResetCameraPosition();
			originMovaPosition = MonoBehaviourInstance<MobaCamera>.inst.transform.position;
			SingletonMonoBehaviour<PlayerController>.inst.LockInput(true);
			mobaCameraTP.enabled = false;
			mobaCameraTP.from = originMovaPosition;
			mobaCameraTP.to = new Vector3(16.025f, 0.211f, -103.326f);
			mobaCameraTP.speed = 2f;
			mobaCameraTP.PlayAnimation();
			mobaCameraTP.enabled = true;
			mobaCameraTP.OnAnimationFinish += MoveFinishedPowerUpTutorial;
		}


		private void MovaCameraBackPowerUpTutorial()
		{
			mobaCameraTP.enabled = false;
			mobaCameraTP.from = new Vector3(16.025f, 0.211f, -103.326f);
			mobaCameraTP.to = originMovaPosition;
			mobaCameraTP.speed = 2f;
			mobaCameraTP.PlayAnimation();
			mobaCameraTP.enabled = true;
			mobaCameraTP.OnAnimationFinish -= MoveFinishedPowerUpTutorial;
			mobaCameraTP.OnAnimationFinish += delegate
			{
				SingletonMonoBehaviour<PlayerController>.inst.LockInput(false);
			};
		}


		private void MoveFinishedPowerUpTutorial()
		{
			this.StartThrowingCoroutine(CorFinishedPowerUpTutorial(), null);
		}


		private IEnumerator CorFinishedPowerUpTutorial()
		{
			yield return new WaitForSeconds(2f);
			MovaCameraBackPowerUpTutorial();
		}


		public bool EnableHyperloop()
		{
			if (enableHyperloop)
			{
				return true;
			}

			TutorialType tutorialType = this.tutorialType;
			if (tutorialType != TutorialType.PowerUp)
			{
				if (tutorialType == TutorialType.FinalSurvival)
				{
					AddDialogueTutorial(68);
					this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(null), null);
				}
			}
			else
			{
				tutorial.ShowTutorialSquareNaviArea();
				AddDialogueTutorial(69);
				this.StartThrowingCoroutine(ShowRecursiveDialogueTutorial(null), null);
			}

			return false;
		}


		private void ShowAnnounce(int num, bool showAnnounceMessage, Color color, bool isTogetherMessageBox = false,
			bool dontSkipAndEnqueue = false)
		{
			dontSkipAndEnqueue = !isTogetherMessageBox && dontSkipAndEnqueue;
			if (playedAnnounce.Contains(num))
			{
				return;
			}

			float delay = 0f;
			if (!dontSkipAndEnqueue && Singleton<SoundControl>.inst.CheckAnnounceSoundPlaying())
			{
				Singleton<SoundControl>.inst.StopAnnounce();
				delay = isTogetherMessageBox ? 0f : 0.2f;
			}

			if (!dontSkipAndEnqueue & showAnnounceMessage &&
			    MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.IsShowing())
			{
				MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.SkipCurrentShowingMessage(isTogetherMessageBox);
			}

			this.StartThrowingCoroutine(CoroutineUtil.DelayedAction(delay, delegate
				{
					playedAnnounce.Add(num);
					AudioClip audioClip = Singleton<SoundControl>.inst.PlayAnnounceSoundTutorial(num);
					if ((audioClip != null) & showAnnounceMessage)
					{
						MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.ShowMessageTutorial(
							Ln.Get(string.Format("Tutorial/Announce_Tutorial_{0:D2}", num)), audioClip.length, color,
							isTogetherMessageBox);
					}
				}),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][ShowAnnounce] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		public void StartAnnounce()
		{
			switch (TutorialType)
			{
				case TutorialType.BasicGuide:
				case TutorialType.Hunt:
				case TutorialType.PowerUp:
				case TutorialType.FinalSurvival:
					Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.StartGame);
					break;
				case TutorialType.Trace:
					break;
				default:
					return;
			}
		}

		private void Ref()
		{
			Reference.Use(leatherFlag);
			Reference.Use(existHovudSources);
		}
	}
}