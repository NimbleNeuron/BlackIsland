using UnityEngine;

namespace Blis.Client
{
	public abstract class LobbyCharacterCard : MonoBehaviour, ILnEventHander
	{
		public abstract void OnLnDataChange();

		public abstract void SetCharacterCode(int characterCode, bool have, bool freeRotation);
	}
}