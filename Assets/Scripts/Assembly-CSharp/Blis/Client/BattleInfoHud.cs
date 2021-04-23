using System.Text;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class BattleInfoHud : BaseUI
	{
		[SerializeField] private Text aliveCount = default;


		[SerializeField] private Text playerKillCount = default;


		[SerializeField] private Text playerKillAssistCount = default;


		[SerializeField] private Text monsterKillCount = default;


		private GameObject battleScore = default;


		private float playTime = 1f;


		private bool playTimeFlag;


		private Text playTimer;


		private int viewPlayTime;


		private void Update()
		{
			if (playTimeFlag)
			{
				playTime += Time.deltaTime;
				UpdatePlayTime(Mathf.FloorToInt(playTime));
			}
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			battleScore = GameUtil.Bind<Transform>(gameObject, "Info").gameObject;
			playTimer = GameUtil.Bind<Text>(gameObject, "PlayTimer");
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			SetAliveCount(MonoBehaviourInstance<ClientService>.inst.GetAliveCount());
		}


		private void UpdatePlayTime(int viewPlayTime)
		{
			if (this.viewPlayTime == viewPlayTime)
			{
				return;
			}

			this.viewPlayTime = viewPlayTime;
			int value = viewPlayTime / 60;
			int value2 = viewPlayTime % 60;
			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			stringBuilder.Append(GameUtil.IntToString(value, GameUtil.NumberOfDigits.One));
			stringBuilder.Append(" : ");
			stringBuilder.Append(GameUtil.IntToString(value2, GameUtil.NumberOfDigits.Two));
			playTimer.text = stringBuilder.ToString();
		}


		public int GetPlayTime()
		{
			return viewPlayTime;
		}


		public void SetPlayTimeFlag(bool playTimeFlag)
		{
			this.playTimeFlag = playTimeFlag;
		}


		public void SetReconnectPlayTime(int reconnectPlayTime)
		{
			playTime = reconnectPlayTime;
		}


		public void SetAliveCount(int count)
		{
			aliveCount.text = count.ToString();
		}


		public void SetPlayerKillCount(int count)
		{
			playerKillCount.text = count.ToString();
		}


		public void SetPlayerKillAssistCount(int count)
		{
			playerKillAssistCount.text = count.ToString();
		}


		public void SetMonsterKillCount(int count)
		{
			monsterKillCount.text = count.ToString();
		}


		public void SetWatchMode()
		{
			transform.Find("Info").gameObject.SetActive(false);
		}


		public void EnableBattleScore(bool enable)
		{
			battleScore.SetActive(enable);
		}
	}
}