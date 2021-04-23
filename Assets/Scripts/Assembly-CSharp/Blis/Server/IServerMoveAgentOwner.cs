using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public interface IServerMoveAgentOwner
	{
		
		
		ServerMoveAgent MoveAgent { get; }

		
		bool IsStopped();

		
		void MoveInCurve(float angularSpeed);

		
		void MoveInDirection(Vector3 direction);

		
		void MoveToDestination(Vector3 destination);

		
		void MoveToDestination(Vector3 destination, BlisVector[] nextCorners);

		
		void MoveStraight(Vector3 destination, float duration, EasingFunction.Ease ease, bool canRotate);

		
		void MoveStraightWithoutNavSpeed(Vector3 moveStartPos, Vector3 moveEndPos, float moveSpeed);

		
		void MoveToTargetWithoutNavSpeed(Vector3 moveStartPos, WorldCharacter target, float moveSpeed,
			float arriveRadius);

		
		void WarpTo(Vector3 destination, bool needCheckNavMesh);

		
		void StopMove();

		
		void LockRotation(bool isLock);

		
		void LookAt(Vector3 lookTo, float customAngularSpeed, bool isServerRotateInstant);
	}
}