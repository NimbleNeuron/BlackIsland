using Blis.Common;

namespace Blis.Client
{
	public class UserService : SingletonMonoBehaviour<UserService>
	{
		
		public User User { get; private set; }


		protected override void OnAwakeSingleton()
		{
			DontDestroyOnLoad(this);
		}


		public void SetUser(User user)
		{
			User = user;
		}
	}
}