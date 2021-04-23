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
	[Description("BOT 캐릭터가 캐릭터 스폰 포인트로 이동한다.")]
	public class AiMoveToNext : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			LevelData currentLevel = MonoBehaviourInstance<GameService>.inst.CurrentLevel;
			AreaData currentAreaData = base.agent.GetCurrentAreaData(currentLevel);
			List<CharacterSpawnPoint> spawnPointsByAreaCode = currentLevel.GetSpawnPointsByAreaCode(currentAreaData.code);
			if (spawnPointsByAreaCode.Count > 0)
			{
				Vector3 position = spawnPointsByAreaCode.ElementAt(UnityEngine.Random.Range(0, spawnPointsByAreaCode.Count)).transform.position;
				base.agent.Controller.MoveTo(position, false);
				this.moveTarget.value = position;
				base.EndAction(true);
				return;
			}
			base.EndAction(false);
		}

		
		public BBParameter<Vector3> moveTarget;
	}
}
