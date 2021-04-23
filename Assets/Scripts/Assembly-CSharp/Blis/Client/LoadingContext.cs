using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class LoadingContext
	{
		public enum Phase
		{
			None,


			SceneLoad = 20,


			Connect = 22,


			Handshake = 24,


			Join = 26,


			LoadCharacter = 44,


			LoadEffect = 50,


			LoadLevel = 99,


			Done
		}


		public Phase currentPhase;


		private string loadingTip;


		public int phaseMaxProgress;


		public int totalProgress;


		public LoadingContext()
		{
			List<Phase> list = EnumUtil.GetValues<Phase>().ToList<Phase>();
			loadingTip = Ln.Get(LnType.LoadingText, list.ElementAt(Random.Range(1, list.Count)).ToString());
		}


		public void SetProgress(Phase phase, float progress)
		{
			if (currentPhase != phase)
			{
				totalProgress = (int) currentPhase;
				phaseMaxProgress = phase - currentPhase;
				currentPhase = phase;
			}

			if (phase == Phase.Done)
			{
				loadingTip = Ln.Get(LnType.LoadingText, phase.ToString());
			}

			LoadingView.inst.UpdateLoading(loadingTip, (totalProgress + phaseMaxProgress * progress) / 100f);
		}
	}
}