using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class TutorialArrowDir : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer spriteRenderer = default;


		[SerializeField] private Sprite[] sprites = default;


		private int index = default;


		private Vector3 targetPos = default;

		private void Start()
		{
			transform.localPosition = MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.GetPosition();
			this.StartThrowingCoroutine(CorSpriteAnimation(), null);
		}


		private void Update()
		{
			Vector3 position = MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.GetPosition();
			Vector3 direction = GameUtil.DirectionOnPlane(position, targetPos);
			transform.localRotation = GameUtil.LookRotation(direction);
			transform.localPosition = position;
		}


		public void SetTargetPos(Vector3 targetPos)
		{
			this.targetPos = targetPos;
		}


		private IEnumerator CorSpriteAnimation()
		{
			spriteRenderer.sprite = sprites[index];
			yield return new WaitForSeconds(0.14f);
			index++;
			if (index == sprites.Length)
			{
				index = 0;
			}

			this.StartThrowingCoroutine(CorSpriteAnimation(), null);
		}
	}
}