using System.Collections.Generic;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ScoreboardWindow : BaseWindow
	{
		private const float UpdateWindowDelay = 3f;


		private readonly List<PlayerInfo> allPlayerInfos = new List<PlayerInfo>();


		private readonly List<UICharacterScoreCard> allyCharacterScoreList = new List<UICharacterScoreCard>();


		private readonly List<UICharacterScoreCard> otherCharacterScoreList = new List<UICharacterScoreCard>();


		private readonly List<PlayerInfo> sightedPlayerInfos = new List<PlayerInfo>();


		private GameObject allySlotHeader;


		private GameObject enemySlotHeader;


		private bool isDirty = true;


		private float lastUpdateWindowTime;


		private PlayerCharacterStore playerCharacterStore;


		private ScrollRect scrollRect;


		private void LateUpdate()
		{
			if (IsOpen && isDirty && 3f < Time.time - lastUpdateWindowTime)
			{
				if (playerCharacterStore == null)
				{
					return;
				}

				allPlayerInfos.Clear();
				allPlayerInfos.AddRange(playerCharacterStore.PlayerInfos);
				sightedPlayerInfos.Clear();
				sightedPlayerInfos.AddRange(playerCharacterStore.LastPlayerInfos);
				SortPlayerInfos();
				isDirty = false;
				lastUpdateWindowTime = Time.time;
				UpdateScoreBoard();
			}
		}


		protected override void OnDestroy()
		{
			base.OnDestroy();
			UISystem.RemoveListener<PlayerCharacterStore>(OnPlayerStoreUpdate);
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			isDirty = true;
			Transform transform = this.transform.FindRecursively("PlayerList");
			allySlotHeader = transform.FindRecursively("AllyHeader").gameObject;
			enemySlotHeader = transform.FindRecursively("EnemyHeader").gameObject;
			allySlotHeader.SetActive(true);
			enemySlotHeader.SetActive(true);
			scrollRect = GameUtil.Bind<ScrollRect>(gameObject, "Contents/ItemScrollView");
			allyCharacterScoreList.Clear();
			for (int i = 0; i < 3; i++)
			{
				Transform transform2 = transform.Find(string.Format("AllyPlayerScore ({0})", i + 1));
				if (!(transform2 == null))
				{
					transform2.GetComponent<UICharacterScoreCard>().SetScrollRect(scrollRect);
					allyCharacterScoreList.Add(transform2.GetComponent<UICharacterScoreCard>());
				}
			}

			otherCharacterScoreList.Clear();
			for (int j = 0; j < 18; j++)
			{
				Transform transform3 = transform.Find(string.Format("PlayerScore ({0})", j + 1));
				if (!(transform3 == null))
				{
					transform3.GetComponent<UICharacterScoreCard>().SetScrollRect(scrollRect);
					otherCharacterScoreList.Add(transform3.GetComponent<UICharacterScoreCard>());
				}
			}
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			UISystem.AddListener<PlayerCharacterStore>(OnPlayerStoreUpdate);
		}


		private void OnPlayerStoreUpdate(PlayerCharacterStore playerCharacterStore)
		{
			this.playerCharacterStore = playerCharacterStore;
			isDirty = true;
		}


		private void SortPlayerInfos()
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				allySlotHeader.SetActive(false);
				enemySlotHeader.SetActive(false);
				allPlayerInfos.Sort(delegate(PlayerInfo x, PlayerInfo y)
				{
					int num = x.teamNumber.CompareTo(y.teamNumber);
					if (num == 0)
					{
						return x.teamSlot.CompareTo(y.teamSlot);
					}

					return num;
				});
				return;
			}

			sightedPlayerInfos.Sort((x, y) => y.updateDtm.CompareTo(x.updateDtm));
			if (MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				int lastSightedPlayerTeamNumber = GetLastSightedPlayerTeamNumber();
				int teamNumber = MonoBehaviourInstance<ClientService>.inst.MyTeamNumber;
				allPlayerInfos.Sort(delegate(PlayerInfo x, PlayerInfo y)
				{
					if (MonoBehaviourInstance<ClientService>.inst.MyObjectId == x.objectId)
					{
						return -1;
					}

					if (MonoBehaviourInstance<ClientService>.inst.MyObjectId == y.objectId)
					{
						return 1;
					}

					if (teamNumber == x.teamNumber && teamNumber == y.teamNumber)
					{
						return x.teamSlot.CompareTo(y.teamSlot);
					}

					if (teamNumber == x.teamNumber)
					{
						return -1;
					}

					if (teamNumber == y.teamNumber)
					{
						return 1;
					}

					int num = y.isAlive.CompareTo(x.isAlive);
					if (num != 0)
					{
						return num;
					}

					if (0 < lastSightedPlayerTeamNumber)
					{
						if (x.teamNumber != y.teamNumber)
						{
							if (lastSightedPlayerTeamNumber == x.teamNumber)
							{
								return -1;
							}

							if (lastSightedPlayerTeamNumber == y.teamNumber)
							{
								return 1;
							}
						}
						else if (lastSightedPlayerTeamNumber == x.teamNumber)
						{
							return x.teamSlot.CompareTo(y.teamSlot);
						}
					}

					int num2 = sightedPlayerInfos.FindIndex(m => m.objectId == x.objectId);
					int num3 = sightedPlayerInfos.FindIndex(m => m.objectId == y.objectId);
					if (num2 >= 0 && num3 >= 0)
					{
						return num2.CompareTo(num3);
					}

					if (num2 != num3)
					{
						return num3.CompareTo(num2);
					}

					if (x.teamNumber == y.teamNumber)
					{
						return x.teamSlot.CompareTo(y.teamSlot);
					}

					return x.teamNumber.CompareTo(y.teamNumber);
				});
				return;
			}

			allPlayerInfos.Sort(delegate(PlayerInfo x, PlayerInfo y)
			{
				int num = y.isAlive.CompareTo(x.isAlive);
				if (num != 0)
				{
					return num;
				}

				if (MonoBehaviourInstance<ClientService>.inst.MyObjectId == x.objectId)
				{
					return -1;
				}

				if (MonoBehaviourInstance<ClientService>.inst.MyObjectId == y.objectId)
				{
					return 1;
				}

				int num2 = sightedPlayerInfos.FindIndex(m => m.objectId == x.objectId);
				int num3 = sightedPlayerInfos.FindIndex(m => m.objectId == y.objectId);
				if (num2 >= 0 && num3 >= 0)
				{
					return num2.CompareTo(num3);
				}

				if (num2 != num3)
				{
					return num3.CompareTo(num2);
				}

				if (x.teamNumber == y.teamNumber)
				{
					return x.teamSlot.CompareTo(y.teamSlot);
				}

				return x.teamNumber.CompareTo(y.teamNumber);
			});
		}


		private int GetLastSightedPlayerTeamNumber()
		{
			for (int i = 0; i < sightedPlayerInfos.Count; i++)
			{
				if (sightedPlayerInfos[i].isAlive)
				{
					return sightedPlayerInfos[i].teamNumber;
				}
			}

			return 0;
		}


		protected override void OnOpen()
		{
			base.OnOpen();
			if (isDirty)
			{
				if (playerCharacterStore != null)
				{
					allPlayerInfos.Clear();
					allPlayerInfos.AddRange(playerCharacterStore.PlayerInfos);
					sightedPlayerInfos.Clear();
					sightedPlayerInfos.AddRange(playerCharacterStore.LastPlayerInfos);
				}

				SortPlayerInfos();
				isDirty = false;
				lastUpdateWindowTime = Time.time;
			}

			UpdateScoreBoard();
		}


		public void ForceRefresh()
		{
			SortPlayerInfos();
			isDirty = false;
			lastUpdateWindowTime = Time.time;
			UpdateScoreBoard();
		}


		private void UpdateScoreBoard()
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				UpdateTeamPlayerCard();
				UpdateOtherPlayerCard();
				return;
			}

			UpdateObserverPlayerCard();
		}


		private void UpdateTeamPlayerCard()
		{
			int num = 0;
			int myTeamNumber = MonoBehaviourInstance<ClientService>.inst.MyTeamNumber;
			for (int i = 0; i < allPlayerInfos.Count; i++)
			{
				PlayerInfo playerInfo = allPlayerInfos[i];
				if (myTeamNumber == playerInfo.teamNumber)
				{
					if (allyCharacterScoreList.Count <= num)
					{
						break;
					}

					UICharacterScoreCard uicharacterScoreCard = allyCharacterScoreList[num];
					int num2 = sightedPlayerInfos.FindIndex(x => x.objectId == playerInfo.objectId);
					bool flag = num2 > -1;
					if (flag)
					{
						playerInfo = sightedPlayerInfos[num2];
					}

					uicharacterScoreCard.SetScoreCardState(playerInfo.isAlive
						? flag ? ScoreCardState.Known : ScoreCardState.Unknown
						: ScoreCardState.Dead);
					if (playerInfo.isAlive)
					{
						if (playerInfo.isDyingCondition)
						{
							uicharacterScoreCard.DyingCondition();
						}
						else
						{
							uicharacterScoreCard.Alive();
						}
					}
					else
					{
						uicharacterScoreCard.Dead();
					}

					if (playerInfo.isDisconnected)
					{
						uicharacterScoreCard.Disconnected();
					}
					else if (playerInfo.isObserving)
					{
						uicharacterScoreCard.Observing();
					}
					else
					{
						uicharacterScoreCard.Connected();
					}

					uicharacterScoreCard.SetObjectId(playerInfo.objectId);
					uicharacterScoreCard.SetName(playerInfo.name);
					uicharacterScoreCard.SetCharacterImage(
						SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterScoreSprite(playerInfo.characterCode));
					uicharacterScoreCard.SetTeamColor(true, playerInfo.teamSlot);
					uicharacterScoreCard.SetCharacterLevel(playerInfo.level);
					uicharacterScoreCard.SetEquips(playerInfo.equipment);
					uicharacterScoreCard.SetCombatMasteryLevel(playerInfo.combatLevel);
					uicharacterScoreCard.SetSearchMasteryLevel(playerInfo.searchLevel);
					uicharacterScoreCard.SetGrowthMasteryLevel(playerInfo.growthLevel);
					uicharacterScoreCard.SetPlayerKillCount(playerInfo.playerKill);
					uicharacterScoreCard.SetPlayerKillAssistCount(playerInfo.playerKillAssist);
					uicharacterScoreCard.SetMonsterKillCount(playerInfo.monsterKill);
					uicharacterScoreCard.SetMySlot(MonoBehaviourInstance<ClientService>.inst.MyObjectId ==
					                               playerInfo.objectId);
					uicharacterScoreCard.Show();
					num++;
				}
			}

			for (int j = num; j < allyCharacterScoreList.Count; j++)
			{
				allyCharacterScoreList[j].Hide();
			}
		}


		private void UpdateOtherPlayerCard()
		{
			int num = 0;
			int myTeamNumber = MonoBehaviourInstance<ClientService>.inst.MyTeamNumber;
			for (int i = 0; i < allPlayerInfos.Count; i++)
			{
				PlayerInfo playerInfo = allPlayerInfos[i];
				if (myTeamNumber != playerInfo.teamNumber)
				{
					if (otherCharacterScoreList.Count <= num)
					{
						break;
					}

					UICharacterScoreCard uicharacterScoreCard = otherCharacterScoreList[num];
					int num2 = sightedPlayerInfos.FindIndex(x => x.objectId == playerInfo.objectId);
					bool flag = num2 > -1;
					if (flag)
					{
						playerInfo = sightedPlayerInfos[num2];
					}

					uicharacterScoreCard.SetScoreCardState(playerInfo.isAlive
						? flag ? ScoreCardState.Known : ScoreCardState.Unknown
						: ScoreCardState.Dead);
					uicharacterScoreCard.SetObjectId(playerInfo.objectId);
					uicharacterScoreCard.SetName(playerInfo.name);
					uicharacterScoreCard.SetCharacterImage(
						SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterScoreSprite(playerInfo.characterCode));
					uicharacterScoreCard.SetTeamColor(false, playerInfo.teamSlot);
					uicharacterScoreCard.SetCharacterLevel(playerInfo.level);
					uicharacterScoreCard.SetEquips(playerInfo.equipment);
					uicharacterScoreCard.SetCombatMasteryLevel(playerInfo.combatLevel);
					uicharacterScoreCard.SetSearchMasteryLevel(playerInfo.searchLevel);
					uicharacterScoreCard.SetGrowthMasteryLevel(playerInfo.growthLevel);
					uicharacterScoreCard.SetPlayerKillCount(playerInfo.playerKill);
					uicharacterScoreCard.SetPlayerKillAssistCount(playerInfo.playerKillAssist);
					uicharacterScoreCard.SetMonsterKillCount(playerInfo.monsterKill);
					uicharacterScoreCard.SetMySlot(MonoBehaviourInstance<ClientService>.inst.MyObjectId ==
					                               playerInfo.objectId);
					uicharacterScoreCard.Show();
					num++;
				}
			}

			for (int j = num; j < otherCharacterScoreList.Count; j++)
			{
				otherCharacterScoreList[j].Hide();
			}
		}


		private void UpdateObserverPlayerCard()
		{
			int num = 0;
			int myTeamNumber = MonoBehaviourInstance<ClientService>.inst.MyTeamNumber;
			for (int i = 0; i < allPlayerInfos.Count; i++)
			{
				PlayerInfo playerInfo = allPlayerInfos[i];
				bool isMyTeam = myTeamNumber == playerInfo.teamNumber;
				if (otherCharacterScoreList.Count <= num)
				{
					break;
				}

				UICharacterScoreCard uicharacterScoreCard = otherCharacterScoreList[num];
				int num2 = sightedPlayerInfos.FindIndex(x => x.objectId == playerInfo.objectId);
				bool flag = num2 > -1;
				if (flag)
				{
					playerInfo = sightedPlayerInfos[num2];
				}

				uicharacterScoreCard.SetScoreCardState(playerInfo.isAlive
					? flag ? ScoreCardState.Known : ScoreCardState.Unknown
					: ScoreCardState.Dead);
				if (playerInfo.isAlive)
				{
					if (playerInfo.isDyingCondition)
					{
						uicharacterScoreCard.DyingCondition();
					}
					else
					{
						uicharacterScoreCard.Alive();
					}
				}
				else
				{
					uicharacterScoreCard.Dead();
				}

				if (playerInfo.isDisconnected)
				{
					uicharacterScoreCard.Disconnected();
				}
				else if (playerInfo.isObserving)
				{
					uicharacterScoreCard.Observing();
				}
				else
				{
					uicharacterScoreCard.Connected();
				}

				uicharacterScoreCard.SetObjectId(playerInfo.objectId);
				uicharacterScoreCard.SetName(playerInfo.name);
				uicharacterScoreCard.SetCharacterImage(
					SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterScoreSprite(playerInfo.characterCode));
				uicharacterScoreCard.SetTeamColor(isMyTeam, playerInfo.teamSlot);
				uicharacterScoreCard.SetCharacterLevel(playerInfo.level);
				uicharacterScoreCard.SetEquips(playerInfo.equipment);
				uicharacterScoreCard.SetCombatMasteryLevel(playerInfo.combatLevel);
				uicharacterScoreCard.SetSearchMasteryLevel(playerInfo.searchLevel);
				uicharacterScoreCard.SetGrowthMasteryLevel(playerInfo.growthLevel);
				uicharacterScoreCard.SetPlayerKillCount(playerInfo.playerKill);
				uicharacterScoreCard.SetPlayerKillAssistCount(playerInfo.playerKillAssist);
				uicharacterScoreCard.SetMonsterKillCount(playerInfo.monsterKill);
				uicharacterScoreCard.SetMySlot(MonoBehaviourInstance<ClientService>.inst.MyObjectId ==
				                               playerInfo.objectId);
				uicharacterScoreCard.Show();
				num++;
			}

			for (int j = 0; j < allyCharacterScoreList.Count; j++)
			{
				allyCharacterScoreList[j].Hide();
			}

			for (int k = num; k < otherCharacterScoreList.Count; k++)
			{
				otherCharacterScoreList[k].Hide();
			}
		}
	}
}