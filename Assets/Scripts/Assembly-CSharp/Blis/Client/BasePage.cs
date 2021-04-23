namespace Blis.Client
{
	public class BasePage : BaseUI
	{
		protected bool isOpen;


		public bool IsOpen => isOpen;


		public void OpenPage()
		{
			gameObject.SetActive(true);
			isOpen = true;
			OnOpenPage();
		}


		public void ClosePage()
		{
			OnClosePage();
			isOpen = false;
			gameObject.SetActive(false);
		}


		protected virtual void OnOpenPage() { }


		protected virtual void OnClosePage() { }
	}
}