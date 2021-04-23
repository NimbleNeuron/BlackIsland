using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("캐릭터가 바라보는 방향에서 왼쪽, 오른쪽으로 10미터 떨어진 지점에서 가장 가까운 네비게이션 메시를 패트롤 포인트로 저장")]
	public class AiSelectPatrolPoint : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			Vector3 position = base.agent.GetPosition();
			Vector3 right = base.agent.transform.right;
			NavMeshHit navMeshHit;
			if (NavMesh.FindClosestEdge(position + right * 10f, out navMeshHit, 2147483640))
			{
				this.rightPosition.value = navMeshHit.position;
			}
			if (NavMesh.FindClosestEdge(position + right * -10f, out navMeshHit, 2147483640))
			{
				this.leftPosition.value = navMeshHit.position;
			}
			base.EndAction(true);
		}

		
		public int distanceToPoint;

		
		public BBParameter<Vector3> rightPosition;

		
		public BBParameter<Vector3> leftPosition;
	}
}
