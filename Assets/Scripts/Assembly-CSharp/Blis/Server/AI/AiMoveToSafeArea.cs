using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("가장 가까운 안전 구역으로 이동한다.")]
	public class AiMoveToSafeArea : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			LevelData currentLevel = MonoBehaviourInstance<GameService>.inst.CurrentLevel;
			AreaData currentAreaData = base.agent.GetCurrentAreaData(currentLevel);
			int code = currentAreaData.code;
			if (!currentLevel.nearByAreaMap.ContainsKey(code))
			{
				base.EndAction(false);
				return;
			}
			IEnumerable<AreaData> areaDataListByState = MonoBehaviourInstance<GameService>.inst.Area.getAreaDataListByState(AreaRestrictionState.Normal);
			AreaData areaData = areaDataListByState.FirstOrDefault((AreaData area) => currentLevel.nearByAreaMap[code].Exists((int nearbyCode) => area.code == nearbyCode));
			if (areaData == null)
			{
				areaData = areaDataListByState.ElementAtOrDefault(UnityEngine.Random.Range(0, areaDataListByState.Count<AreaData>()));
			}
			Vector3 vector = Vector3.zero;
			if (areaData != null)
			{
				List<CharacterSpawnPoint> spawnPointsByAreaCode = currentLevel.GetSpawnPointsByAreaCode(areaData.code);
				if (spawnPointsByAreaCode.Count > 0)
				{
					vector = spawnPointsByAreaCode[UnityEngine.Random.Range(0, spawnPointsByAreaCode.Count)].transform.position;
				}
			}
			else
			{
				WorldSecurityConsole lastSafeConsole = MonoBehaviourInstance<GameService>.inst.Area.LastSafeConsole;
				if (lastSafeConsole == null)
				{
					base.EndAction(false);
					return;
				}
				MoveAgent.SamplePosition(lastSafeConsole.GetPosition(), base.agent.WalkableNavMask, out vector);
			}
			base.agent.Controller.MoveTo(vector, false);
			this.moveTarget.value = vector;
			base.EndAction(true);
		}

		
		public BBParameter<Vector3> moveTarget;
	}
}
