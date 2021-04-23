using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class Tutorial : BaseUI
	{
		[SerializeField] private TutorialMessageBox messageBox = default;


		[SerializeField] private TutorialQuest quest = default;


		[SerializeField] private TutorialDialogue dialogue = default;


		[SerializeField] private TutorialArrowDir tutorialArrowDir = default;


		public float waitNadineInHunt = 3f;


		public float waitMagHuynwooInHunt = 11f;

		public void CreateTutorialArrowDir(Vector3 targetPos)
		{
			GameObject gameObject = Instantiate<GameObject>(
				SingletonMonoBehaviour<ResourceManager>.inst.LoadTutorialPrefab("TutorialArrowDirNew"),
				MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.transform.parent);
			tutorialArrowDir = gameObject.GetComponent<TutorialArrowDir>();
			tutorialArrowDir.SetTargetPos(targetPos);
		}


		public void HideTutorialArrowDir()
		{
			tutorialArrowDir.gameObject.SetActive(false);
		}


		public void InitMainQuestStack()
		{
			quest.InitMainQuestStack();
		}


		public void ShowMessageBox(int group, Action closeCallback)
		{
			messageBox.Open();
			messageBox.Show(group, closeCallback);
		}


		public void AddMainQuest(int code, Action mainQuestCompletedAction)
		{
			quest.AddMainQuest(code, mainQuestCompletedAction);
			TutorialQuest tutorialQuest = quest;
			tutorialQuest.questDetailButtonClick = (TutorialQuest.DetailButtonClick) Delegate.Combine(
				tutorialQuest.questDetailButtonClick,
				new TutorialQuest.DetailButtonClick(delegate(int id) { ShowMessageBox(id, null); }));
		}


		public void AddMainQuestStack()
		{
			quest.AddMainQuestStack();
		}


		public void SuccessMainQuest()
		{
			quest.SuccessMainQuest();
		}


		public void HideMainQuest()
		{
			quest.Hide();
		}


		public void AddSubQuest(int code, Action completedAction = null)
		{
			quest.AddSubQuest(code, completedAction);
		}


		public void SuccessSubQuest(TutorialQuestType tutorialQuestType)
		{
			quest.SuccessSubQuest(tutorialQuestType);
		}


		public IEnumerator ShowDialogue(int code)
		{
			yield return this.StartThrowingCoroutine(dialogue.Show(code), null);
		}


		public void HideDialogue()
		{
			dialogue.Hide();
		}


		public void ShowTutorialSquareEquip()
		{
			MonoBehaviourInstance<GameUI>.inst.StatusHud.ShowTutorialSquareEquip(true);
		}


		public void ShowTutorialSquareNavi()
		{
			MonoBehaviourInstance<GameUI>.inst.NavigationHud.ShowTutorialSquareNavi(true);
		}


		public void ShowTutorialSquareNaviArea()
		{
			MonoBehaviourInstance<GameUI>.inst.NavigationHud.ShowTutorialSquareNaviArea(true);
		}


		public void ShowTutorialSquareInven()
		{
			MonoBehaviourInstance<GameUI>.inst.InventoryHud.ShowTutorialSquareInven(true);
		}


		public void ShowTutorialSquareSkill()
		{
			MonoBehaviourInstance<GameUI>.inst.SkillHud.ShowTutorialSquareSkill(true);
		}


		public void HideTutorialSquare()
		{
			MonoBehaviourInstance<GameUI>.inst.StatusHud.ShowTutorialSquareEquip(false);
			MonoBehaviourInstance<GameUI>.inst.InventoryHud.ShowTutorialSquareInven(false);
			MonoBehaviourInstance<GameUI>.inst.NavigationHud.ShowTutorialSquareNavi(false);
			MonoBehaviourInstance<GameUI>.inst.NavigationHud.ShowTutorialSquareNaviArea(false);
			MonoBehaviourInstance<GameUI>.inst.SkillHud.ShowTutorialSquareSkill(false);
		}
	}
}