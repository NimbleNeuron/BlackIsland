using UnityEngine;

namespace Blis.Common
{
	public interface IMoveStrategy
	{
		MoveStrategyType GetStrategyType();


		Vector3 NextPosition(Vector3 position, float deltaTime);


		void UpdateMoveSpeed(float moveSpeed);


		bool MoveFinished();


		void SetRotateWhileMoving(bool isRotation);


		bool CanRotateWhileMoving();


		void ResetDestination(Vector3 destination);
	}
}