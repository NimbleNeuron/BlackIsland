using UnityEngine;
using UnityEngine.AI;

namespace MLSpace
{
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(ThirdPersonCharacter))]
	public class AICharacterControlCustom : MonoBehaviour
	{
		public Transform[] targets;


		public float m_Speed = 60f;


		private bool m_Crouch;


		private int m_CurrentTargetIndex;


		private bool m_Jump;


		
		public NavMeshAgent agent { get; private set; }


		
		public ThirdPersonCharacter character { get; private set; }


		private void Start()
		{
			agent = GetComponentInChildren<NavMeshAgent>();
			character = GetComponent<ThirdPersonCharacter>();
			agent.updateRotation = false;
			agent.updatePosition = false;
			if (targets.Length == 0)
			{
				Debug.LogError("No waypoints exists.");
				return;
			}

			agent.SetDestination(targets[m_CurrentTargetIndex].position);
		}


		private void Update()
		{
			if (!agent.enabled)
			{
				character.Move(Vector3.zero, false, false);
				return;
			}

			if (Input.GetKeyDown(KeyCode.J))
			{
				m_Jump = true;
			}

			if (Input.GetKey(KeyCode.C))
			{
				m_Crouch = true;
			}

			float num = Vector3.Distance(transform.position, targets[m_CurrentTargetIndex].position);
			Vector3 vector = targets[m_CurrentTargetIndex].position - transform.position;
			vector.y = 0f;
			vector.Normalize();
			vector = vector * Time.deltaTime * m_Speed;
			Debug.DrawLine(targets[m_CurrentTargetIndex].position,
				targets[m_CurrentTargetIndex].position + Vector3.up * 6f, Color.blue);
			Debug.DrawLine(transform.position, transform.position + agent.desiredVelocity, Color.yellow);
			Debug.DrawLine(transform.position, transform.position + vector, Color.red);
			if (num < 1f)
			{
				m_CurrentTargetIndex++;
				if (m_CurrentTargetIndex >= targets.Length)
				{
					m_CurrentTargetIndex = 0;
				}
			}

			character.Move(vector, m_Crouch, m_Jump);
			m_Jump = false;
		}


		public void SetTargets(Transform[] _targets)
		{
			targets = _targets;
		}


		public void SetJump(bool _jump)
		{
			m_Jump = _jump;
		}


		public void SetCrouch(bool _crouch)
		{
			m_Crouch = _crouch;
		}
	}
}