using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public interface ILocalMoveAgentOwner
	{
		LocalMoveAgent MoveAgent { get; }


		void UpdateMoveSpeed(float moveSpeed);


		void MoveInCurve(Vector3 startRotation, float angularSpeed);


		void MoveInDirection(Vector3 direction);


		void MoveToDestination(BlisVector startPosition, BlisVector destination, BlisVector[] corners);


		void MoveStraight(BlisVector startPosition, BlisVector destination, float duration, EasingFunction.Ease ease,
			bool canRotate);


		void MoveStraightWithoutNav(Vector3 moveStartPos, Vector3 moveEndPos, float duration);


		void MoveToTargetWithoutNav(Vector3 moveStartPos, LocalCharacter target, float moveSpeed, float arriveRadius);


		void WarpTo(BlisVector destination);


		void StopMove();


		void PauseMove();


		void ResumeMove();


		void LockRotation(bool isLock, Quaternion rotation);


		void LookAt(Quaternion lookFrom, Quaternion lookTo, float customAngularSpeed);
	}
}