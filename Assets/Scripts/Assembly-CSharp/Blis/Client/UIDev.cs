using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIDev : MonoBehaviour
	{
		public InputField inputMinLatency;


		public InputField inputMaxLatency;


		public InputField inputPacketLoss;

		public void OnApply()
		{
			try
			{
				int minLatency = 0;
				if (!int.TryParse(inputMinLatency.text, out minLatency))
				{
					Log.E("Value minLatency \"{0}\" isn't number.", inputMinLatency.text);
				}

				int maxLatency = 0;
				if (!int.TryParse(inputMaxLatency.text, out maxLatency))
				{
					Log.E("Value maxLatency \"{0}\" isn't number.", inputMaxLatency.text);
				}

				int packetLoss = 0;
				if (!int.TryParse(inputPacketLoss.text, out packetLoss))
				{
					Log.E("Value packetLoss \"{0}\" isn't number.", inputPacketLoss.text);
				}

				MonoBehaviourInstance<GameClient>.inst.SetSimulation(minLatency, maxLatency, packetLoss);
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
		}
	}
}