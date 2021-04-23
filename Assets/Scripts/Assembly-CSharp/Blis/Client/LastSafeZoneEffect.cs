namespace Blis.Client
{
	public class LastSafeZoneEffect : EnvironmentEffect
	{
		public const string SHOW = "show";


		public const string HIDE = "hide";

		private void Awake()
		{
			gameObject.SetActive(false);
		}


		public override void PlayAnimation(string eventKey)
		{
			if (eventKey == "show")
			{
				gameObject.SetActive(true);
				return;
			}

			if (!(eventKey == "hide"))
			{
				return;
			}

			gameObject.SetActive(false);
		}
	}
}