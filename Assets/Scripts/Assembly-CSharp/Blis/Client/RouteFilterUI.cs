using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine.UI;

namespace Blis.Client
{
	public class RouteFilterUI : BaseUI
	{
		private readonly List<Button> buttons = new List<Button>();


		private readonly List<RouteFilter> routeFilters = new List<RouteFilter>();


		private bool ignoreClickEvent;


		private RouteFilterType prevRouteFilterType;


		private RouteFilterType routeFilterType;


		private LnText txtTitle;

		
		
		public event Action<RouteFilterType> changeFilterType;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			transform.GetComponentsInChildren<Button>(false, buttons);
			for (int i = 0; i < buttons.Count; i++)
			{
				RouteFilter routeFilter = new RouteFilter(buttons[i]);
				routeFilter.clickFilterType += ClickFilterType;
				routeFilters.Add(routeFilter);
			}
		}


		public void Show(bool show)
		{
			gameObject.SetActive(show);
		}


		public void IgnoreClickEvent(bool ignore)
		{
			ignoreClickEvent = ignore;
		}


		private void ClickFilterType()
		{
			if (ignoreClickEvent)
			{
				return;
			}

			ChangeFilterType();
			ChangeFilterUI(routeFilterType);
			if (changeFilterType != null)
			{
				changeFilterType(routeFilterType);
			}
		}


		private void ChangeFilterType()
		{
			prevRouteFilterType = routeFilterType;
			int num = 0;
			num += routeFilters[0].GetIsActive() ? 1 : 0;
			num += routeFilters[1].GetIsActive() ? 2 : 0;
			switch (num + (routeFilters[2].GetIsActive() ? 4 : 0))
			{
				case 1:
					routeFilterType = RouteFilterType.SOLO;
					break;
				case 2:
					routeFilterType = RouteFilterType.DUO;
					break;
				case 3:
					routeFilterType = RouteFilterType.SOLO_DUO;
					break;
				case 4:
					routeFilterType = RouteFilterType.SQUAD;
					break;
				case 5:
					routeFilterType = RouteFilterType.SOLO_SQUAD;
					break;
				case 6:
					routeFilterType = RouteFilterType.DUO_SQUAD;
					break;
				case 7:
					routeFilterType = RouteFilterType.ALL;
					break;
				default:
					routeFilterType = RouteFilterType.ALL;
					break;
			}

			if (routeFilterType == RouteFilterType.ALL)
			{
				int num2 = 0;
				using (List<RouteFilter>.Enumerator enumerator = routeFilters.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.GetIsActive())
						{
							num2++;
						}
					}
				}

				if (num2 == 0)
				{
					routeFilterType = prevRouteFilterType;
				}
			}
		}


		public void ChangeFilterType(RouteFilterType routeFilterType)
		{
			this.routeFilterType = routeFilterType;
			ChangeFilterUI(routeFilterType);
		}


		private void ChangeFilterUI(RouteFilterType routeFilterType)
		{
			foreach (RouteFilter routeFilter in routeFilters)
			{
				routeFilter.SetIsActive(false);
			}

			string text = routeFilterType.ToString();
			if (text.Equals("ALL"))
			{
				foreach (RouteFilter routeFilter2 in routeFilters)
				{
					routeFilter2.SetIsActive(true);
				}

				return;
			}

			if (text.Contains("SOLO"))
			{
				routeFilters[0].SetIsActive(true);
			}

			if (text.Contains("DUO"))
			{
				routeFilters[1].SetIsActive(true);
			}

			if (text.Contains("SQUAD"))
			{
				routeFilters[2].SetIsActive(true);
			}
		}


		public class RouteFilter
		{
			private readonly Image checkOn;


			private readonly Image iconOn;


			private bool isActive;


			public RouteFilter(Button button)
			{
				checkOn = GameUtil.Bind<Image>(button.gameObject, "CheckOff/CheckOn");
				iconOn = GameUtil.Bind<Image>(button.gameObject, "IconOff/IconOn");
				button.onClick.AddListener(delegate
				{
					isActive = !isActive;
					clickFilterType();
				});
			}

			
			
			public event Action clickFilterType;


			public bool GetIsActive()
			{
				return isActive;
			}


			public void SetIsActive(bool isActive)
			{
				this.isActive = isActive;
				checkOn.enabled = isActive;
				iconOn.enabled = isActive;
			}
		}
	}
}