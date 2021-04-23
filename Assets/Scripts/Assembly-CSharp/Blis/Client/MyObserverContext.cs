using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public class MyObserverContext : ObserverContext
	{
		public readonly string nickname;

		public MyObserverContext(long userId, string nickname) : base(userId)
		{
			this.nickname = nickname;
		}


		public void Init(byte[] observerSnapshot) { }


		public HostileType GetHostileType(LocalCharacter target)
		{
			return Observer.HostileAgent.GetHostileType(target.HostileAgent);
		}


		public void OnDead() { }


		public void ShowObserverUI(LocalPlayerCharacter selectCharacter)
		{
			MonoBehaviourInstance<GameUI>.inst.GameResult.Deactive();
			MonoBehaviourInstance<GameUI>.inst.HidePlayerUI();
			MonoBehaviourInstance<GameUI>.inst.ObserverHud.Open(selectCharacter);
			MonoBehaviourInstance<GameUI>.inst.BloodFx.Stop();
		}


		public bool IsValid()
		{
			return userId > 0L && !(Observer == null) && Observer.ObjectId > 0;
		}
	}
}