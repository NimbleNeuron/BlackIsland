using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class MasteryExpHud : BaseUI
	{
		private readonly Color32 EXP_COLOR = new Color32(113, 233, byte.MaxValue, byte.MaxValue);


		private readonly List<LabelInfo> labelList = new List<LabelInfo>();


		private MasteryExpUIPool uiExpPool;

		protected override void OnAwakeUI()
		{
			GameUtil.BindOrAdd<MasteryExpUIPool>(gameObject, ref uiExpPool);
			uiExpPool.InitPool();
		}


		protected override void OnStartUI()
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				this.StartThrowingCoroutine(CheckQueue(),
					delegate(Exception exception)
					{
						Log.E("[EXCEPTION][MasteryExpHud] Message:" + exception.Message + ", StackTrace:" +
						      exception.StackTrace);
					});
			}
		}


		public void ShowExp(MasteryType type, int exp)
		{
			LabelInfo labelInfo = labelList.Find(label => label.masteryType == type);
			if (labelInfo != null)
			{
				labelInfo.exp = exp;
				return;
			}

			labelList.Add(new LabelInfo
			{
				masteryType = type,
				exp = exp
			});
		}


		private IEnumerator CheckQueue()
		{
			while (MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.IsAlive)
			{
				if (labelList.Count > 0)
				{
					LabelInfo labelInfo = labelList[0];
					UIExp uiExp = uiExpPool.Pop<UIExp>().GetComponent<UIExp>();
					uiExp.transform.SetParent(transform);
					uiExp.transform.localPosition = Vector3.zero;
					string text = string.Format("{0} +{1}xp", LnUtil.GetMasteryName(labelInfo.masteryType),
						labelInfo.exp);
					uiExp.SetLabel(text, 18, EXP_COLOR, Color.black, delegate { uiExpPool.Push<UIExp>(uiExp); });
					labelList.RemoveAt(0);
					yield return new WaitForSeconds(0.5f);
				}
				else
				{
					yield return null;
				}
			}
		}


		private class LabelInfo
		{
			public int exp;

			public MasteryType masteryType;
		}
	}
}