using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	[ObjectAttr(ObjectType.BotPlayerCharacter)]
	public class LocalBotPlayerCharacter : LocalPlayerCharacter
	{
		public override void Init(byte[] snapshotData)
		{
			base.Init(snapshotData);
			if (MonoBehaviourInstance<GameClient>.inst.MatchingMode.Equals(MatchingMode.Tutorial5))
			{
				OnUpdateSurvivableTime(120f);
			}
		}


		public override void OnDead(LocalCharacter attacker)
		{
			base.OnDead(attacker);
			bool flag = !MonoBehaviourInstance<ClientService>.inst.IsPlayer;
			if (!flag)
			{
				flag = MonoBehaviourInstance<GameClient>.inst.IsTutorial &&
				       MonoBehaviourInstance<TutorialController>.inst.ShowAllPlayer;
			}

			if (flag)
			{
				if (characterRenderer.IsRendering)
				{
					ShowMapIcon(MiniMapIconType.Sight);
					return;
				}

				ShowMapIcon(MiniMapIconType.Sight);
				ShowMiniMapIcon(MiniMapIconType.Sight);
			}
		}


		public override void HideMiniMapIcon(MiniMapIconType miniMapIconType)
		{
			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial &&
			    MonoBehaviourInstance<TutorialController>.inst.ShowAllPlayer)
			{
				return;
			}

			base.HideMiniMapIcon(miniMapIconType);
		}


		public override void HideMapIcon(MiniMapIconType miniMapIconType)
		{
			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial &&
			    MonoBehaviourInstance<TutorialController>.inst.ShowAllPlayer)
			{
				return;
			}

			base.HideMapIcon(miniMapIconType);
		}


		protected override void UpdateMapIconPos()
		{
			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst == null)
			{
				return;
			}

			GameClient inst2 = MonoBehaviourInstance<GameClient>.inst;
			if (inst2 == null)
			{
				return;
			}

			GameUI inst3 = MonoBehaviourInstance<GameUI>.inst;
			if (inst3 == null)
			{
				return;
			}

			bool flag = !inst.IsPlayer;
			if (!flag && inst2.IsTutorial)
			{
				TutorialController inst4 = MonoBehaviourInstance<TutorialController>.inst;
				if (inst4 != null)
				{
					flag = inst4.ShowAllPlayer;
				}
			}

			if (!flag)
			{
				flag = characterRenderer != null && characterRenderer.IsRendering;
			}

			if (!flag)
			{
				return;
			}

			bool isAlly = inst.IsAlly(this);
			inst3.CombineWindow.UpdateMapPlayerPosition(ObjectId, isAlly, GetPosition());
			inst3.MapWindow.UpdateMapPlayerPosition(ObjectId, isAlly, GetPosition());
			inst3.Minimap.UIMap.UpdatePlayerPosition(ObjectId, isAlly, GetPosition());
			inst3.HyperloopWindow.UpdateMapPlayerPosition(ObjectId, isAlly, GetPosition());
		}
	}
}