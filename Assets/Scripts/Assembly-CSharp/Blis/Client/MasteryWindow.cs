using System.Collections.Generic;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class MasteryWindow : BaseWindow
	{
		[SerializeField] private Text level = default;


		[SerializeField] private Text nickname = default;


		[SerializeField] private Text exp = default;


		[SerializeField] private UIProgress expBar = default;


		[SerializeField] private List<UIMasteryItem> combatMasteries = default;


		[SerializeField] private List<UIMasteryItem> searchMasteries = default;


		[SerializeField] private List<UIMasteryItem> growthMasteries = default;


		private Dictionary<MasteryType, MasteryStore.Mastery> updateMasteries = default;


		protected override void OnDestroy()
		{
			base.OnDestroy();
			UISystem.RemoveListener<MasteryStore>(OnUpdateMasteryStore);
		}

		protected override void OnStartUI()
		{
			base.OnStartUI();
			UISystem.AddListener<MasteryStore>(OnUpdateMasteryStore);
		}


		protected override void OnClose()
		{
			base.OnClose();
			MonoBehaviourInstance<Tooltip>.inst.Hide(this);
		}


		protected override void OnOpen()
		{
			base.OnOpen();
			RenderView();
			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				MonoBehaviourInstance<TutorialController>.inst.SuccessOpenMasteryWindowTutorial();
			}
		}


		public void SetNickname(string nickname)
		{
			this.nickname.text = nickname;
		}


		private void OnUpdateMasteryStore(MasteryStore masteryStore)
		{
			if (updateMasteries == null)
			{
				updateMasteries =
					new Dictionary<MasteryType, MasteryStore.Mastery>(
						SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);
				foreach (MasteryStore.Mastery mastery in masteryStore.GetMasteries)
				{
					updateMasteries[mastery.masteryType] = mastery;
				}

				RenderView();
			}
			else
			{
				foreach (MasteryStore.Mastery mastery2 in masteryStore.GetUpdatedMasteries)
				{
					updateMasteries[mastery2.masteryType] = mastery2;
				}
			}

			if (IsOpen)
			{
				RenderView();
			}

			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				foreach (KeyValuePair<MasteryType, MasteryStore.Mastery> keyValuePair in updateMasteries)
				{
					if (keyValuePair.Key == MasteryType.AssaultRifle && keyValuePair.Value.level >= 7)
					{
						MonoBehaviourInstance<TutorialController>.inst.SuccessRifleWeaponMasteryLevelSevenTutorial();
					}
				}
			}
		}


		private void RenderView()
		{
			if (updateMasteries == null)
			{
				MasteryStore store = UISystem.GetStore<MasteryStore>();
				if (store != null)
				{
					updateMasteries =
						new Dictionary<MasteryType, MasteryStore.Mastery>(
							SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance);
					using (Dictionary<MasteryType, MasteryStore.Mastery>.ValueCollection.Enumerator enumerator =
						store.GetMasteries.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							MasteryStore.Mastery mastery = enumerator.Current;
							updateMasteries[mastery.masteryType] = mastery;
						}

						goto IL_63;
					}
				}

				return;
			}

			IL_63:
			foreach (MasteryStore.Mastery mastery2 in updateMasteries.Values)
			{
				UIMasteryItem uimasteryItem = FindMasteryItem(mastery2.masteryType);
				if (uimasteryItem != null)
				{
					uimasteryItem.SetMasteryData(mastery2.masteryType, mastery2.level, mastery2.exp, mastery2.maxExp);
				}
			}

			updateMasteries.Clear();
		}


		public void OnLevelUpdate(int level, int exp)
		{
			CharacterExpData expData = GameDB.character.GetExpData(level);
			this.level.text = level.ToString();
			this.exp.text = string.Format("{0}/{1}", exp, expData.levelUpExp);
			expBar.SetValue(exp, expData.levelUpExp);
		}


		private UIMasteryItem FindMasteryItem(MasteryType masteryType)
		{
			UIMasteryItem uimasteryItem = null;
			List<UIMasteryItem> list = null;
			switch (masteryType.GetCategory())
			{
				case MasteryCategory.Combat:
					list = combatMasteries;
					break;
				case MasteryCategory.Search:
					list = searchMasteries;
					break;
				case MasteryCategory.Growth:
					list = growthMasteries;
					break;
			}

			if (list != null)
			{
				uimasteryItem = list.Find(x => x.GetMasteryType() == masteryType);
				if (uimasteryItem == null)
				{
					uimasteryItem = list.Find(x => x.GetMasteryType() == MasteryType.None);
				}
			}

			return uimasteryItem;
		}
	}
}