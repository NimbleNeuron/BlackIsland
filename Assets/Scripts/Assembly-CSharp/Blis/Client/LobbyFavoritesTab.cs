using System;
using Blis.Common;
using Blis.Common.Utils;
using Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class LobbyFavoritesTab : LobbyTabBaseUI, ILobbyTab
	{
		public enum View
		{
			ListPage,

			EditorPage
		}


		private const int minimumTitleLength = 2;


		private const int maximumTitleLength = 32;


		[SerializeField] private FavoritesListPage favoritesListPage = default;


		[SerializeField] private FavoritesEditorPage favoritesEditorPage = default;


		private int characterCode;


		private FavoriteEditorViewStyle editorViewStyle;


		private bool isChange;


		private bool isShare;


		private RouteFilterType routeFilterType;


		private WeaponType weaponType;


		public FavoritesEditorPage FavoriteEditorPage => favoritesEditorPage;


		public void OnOpen(LobbyTab from)
		{
			EnableCanvas(true);
			Singleton<ItemService>.inst.SetLevelData(GameDB.level.DefaultLevel);
			SetView(View.ListPage);
		}


		public TabCloseResult OnClose(LobbyTab to)
		{
			if (!isChange || favoritesEditorPage.Favorite == null ||
			    favoritesEditorPage.Favorite.weaponCodes.Count == 0)
			{
				EnableCanvas(false);
				favoritesListPage.ClosePage();
				favoritesEditorPage.ClosePage();
				return TabCloseResult.Success;
			}

			if (favoritesEditorPage.IsActive())
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("즐겨찾기에 변경 사항이 있습니다.\n저장하시겠습니까?"), new Popup.Button
				{
					text = Ln.Get("저장"),
					type = Popup.ButtonType.Confirm,
					callback = (Action) (() =>
					{
						if (isShare)
						{
							MessageDiscard(to);
						}
						else
						{
							SaveFavorites(favoritesEditorPage.Favorite, () =>
							{
								favoritesListPage.ClosePage();
								favoritesEditorPage.ClosePage();
								MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(to);
							});
						}
					})
				}, new Popup.Button
				{
					text = Ln.Get("아니오"),
					type = Popup.ButtonType.Cancel,
					callback = (Action) (() =>
					{
						favoritesListPage.ClosePage();
						favoritesEditorPage.ClosePage();
						MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(to);
					})
				});
				MonoBehaviourInstance<Popup>.inst.ShowCloseBtn();
				return TabCloseResult.Fail;
			}

			EnableCanvas(false);
			return TabCloseResult.Success;

			// co: dotPeek
			// if (!this.isChange || this.favoritesEditorPage.Favorite == null || this.favoritesEditorPage.Favorite.weaponCodes.Count == 0)
			// {
			// 	base.EnableCanvas(false);
			// 	this.favoritesListPage.ClosePage();
			// 	this.favoritesEditorPage.ClosePage();
			// 	return TabCloseResult.Success;
			// }
			// if (this.favoritesEditorPage.IsActive())
			// {
			// 	Action <>9__1;
			// 	MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("즐겨찾기에 변경 사항이 있습니다.\n저장하시겠습니까?"), new Popup.Button[]
			// 	{
			// 		new Popup.Button
			// 		{
			// 			text = Ln.Get("저장"),
			// 			type = Popup.ButtonType.Confirm,
			// 			callback = delegate()
			// 			{
			// 				if (this.isShare)
			// 				{
			// 					this.MessageDiscard(to);
			// 					return;
			// 				}
			// 				LobbyFavoritesTab <>4__this = this;
			// 				Favorite favorite = this.favoritesEditorPage.Favorite;
			// 				Action callBack;
			// 				if ((callBack = <>9__1) == null)
			// 				{
			// 					callBack = (<>9__1 = delegate()
			// 					{
			// 						this.favoritesListPage.ClosePage();
			// 						this.favoritesEditorPage.ClosePage();
			// 						MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(to);
			// 					});
			// 				}
			// 				<>4__this.SaveFavorites(favorite, callBack);
			// 			}
			// 		},
			// 		new Popup.Button
			// 		{
			// 			text = Ln.Get("아니오"),
			// 			type = Popup.ButtonType.Cancel,
			// 			callback = delegate()
			// 			{
			// 				this.favoritesListPage.ClosePage();
			// 				this.favoritesEditorPage.ClosePage();
			// 				MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(to);
			// 			}
			// 		}
			// 	});
			// 	MonoBehaviourInstance<Popup>.inst.ShowCloseBtn();
			// 	return TabCloseResult.Fail;
			// }
			// base.EnableCanvas(false);
			// return TabCloseResult.Success;
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			favoritesListPage.OnRequestOpenBringMode += OpenBringMode;
			favoritesListPage.OnRequestOpenEditor += OpenEditor;
			favoritesListPage.OnRequestReset += ResetFavorite;
			favoritesListPage.OnRequestShare += ShareFavorite;
			favoritesListPage.OnChangeCharacter += ChangeCharacter;
			favoritesEditorPage.OnRequestSave += SaveFavoriteGoToListPage;
			favoritesEditorPage.OnRequestDiscard += DiscardQuestionPopup;
			favoritesEditorPage.OnRequestCopy += CopyBringFavorite;
			favoritesEditorPage.OnRequestBringClose += BringClose;
			favoritesEditorPage.OnChangeState += SetChangeState;
		}


		private void ChangeCharacter(int characterCode)
		{
			favoritesListPage.SetCurrentCharacter(characterCode);
			favoritesListPage.OpenPage();
		}


		private void DiscardQuestionPopup()
		{
			if (!isChange || favoritesEditorPage.Favorite.weaponCodes.Count == 0)
			{
				SetView(View.ListPage);
				return;
			}

			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("즐겨찾기에 변경 사항이 있습니다.\n저장하시겠습니까?"), new Popup.Button
			{
				text = Ln.Get("저장"),
				type = Popup.ButtonType.Confirm,
				callback = delegate
				{
					if (isShare)
					{
						MessageShare();
						return;
					}

					SaveFavorites(favoritesEditorPage.Favorite, delegate { SetView(View.ListPage); });
				}
			}, new Popup.Button
			{
				text = Ln.Get("아니오"),
				type = Popup.ButtonType.Cancel,
				callback = delegate { SetView(View.ListPage); }
			});
			MonoBehaviourInstance<Popup>.inst.ShowCloseBtn();
		}


		private void MessageShare()
		{
			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("루트가 변경되어 기존 공유된 루트는 해제됩니다."), new Popup.Button
			{
				text = Ln.Get("저장"),
				type = Popup.ButtonType.Confirm,
				callback = delegate
				{
					SaveFavorites(favoritesEditorPage.Favorite, delegate { SetView(View.ListPage); });
				}
			}, new Popup.Button
			{
				text = Ln.Get("아니오"),
				type = Popup.ButtonType.Cancel,
				callback = delegate { SetView(View.ListPage); }
			});
		}


		private void CopyBringFavorite(Favorite favorite)
		{
			if (favorite.userNum == Lobby.inst.User.UserNum)
			{
				Log.V(string.Format("Impossible Copy Equal UserId -> UserNum = {0}", favorite.userNum));
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("본인 루트 복사 불가"), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return;
			}

			RouteApi.Copy(favorite, BringClose);
		}


		private void BringClose()
		{
			SetView(View.ListPage);
		}


		private void MessageDiscard(LobbyTab to)
		{
			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("루트가 변경되어 기존 공유된 루트는 해제됩니다."), new Popup.Button
			{
				text = Ln.Get("저장"),
				type = Popup.ButtonType.Confirm,
				callback = (Action) (() => SaveFavorites(favoritesEditorPage.Favorite, () =>
				{
					favoritesListPage.ClosePage();
					favoritesEditorPage.ClosePage();
					MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(to);
				}))
			}, new Popup.Button
			{
				text = Ln.Get("아니오"),
				type = Popup.ButtonType.Cancel,
				callback = (Action) (() =>
				{
					favoritesListPage.ClosePage();
					favoritesEditorPage.ClosePage();
					MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(to);
				})
			});

			// co: dotPeek
			// Action <>9__1;
			// MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("루트가 변경되어 기존 공유된 루트는 해제됩니다."), new Popup.Button[]
			// {
			// 	new Popup.Button
			// 	{
			// 		text = Ln.Get("저장"),
			// 		type = Popup.ButtonType.Confirm,
			// 		callback = delegate()
			// 		{
			// 			LobbyFavoritesTab <>4__this = this;
			// 			Favorite favorite = this.favoritesEditorPage.Favorite;
			// 			Action callBack;
			// 			if ((callBack = <>9__1) == null)
			// 			{
			// 				callBack = (<>9__1 = delegate()
			// 				{
			// 					this.favoritesListPage.ClosePage();
			// 					this.favoritesEditorPage.ClosePage();
			// 					MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(to);
			// 				});
			// 			}
			// 			<>4__this.SaveFavorites(favorite, callBack);
			// 		}
			// 	},
			// 	new Popup.Button
			// 	{
			// 		text = Ln.Get("아니오"),
			// 		type = Popup.ButtonType.Cancel,
			// 		callback = delegate()
			// 		{
			// 			this.favoritesListPage.ClosePage();
			// 			this.favoritesEditorPage.ClosePage();
			// 			MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(to);
			// 		}
			// 	}
			// });
		}


		private void SetView(View view)
		{
			isChange = false;
			if (view == View.ListPage)
			{
				editorViewStyle = FavoriteEditorViewStyle.NORMAL;
				favoritesListPage.OpenPage();
				favoritesEditorPage.ClosePage();
				return;
			}

			if (view != View.EditorPage)
			{
				throw new ArgumentOutOfRangeException("view", view, null);
			}

			favoritesListPage.ClosePage();
			favoritesEditorPage.OpenPage();
		}


		private void SetViewBringMode(Favorite favorite)
		{
			RequestDelegate.request<RouteApi.BringRouteInfo>(
				RouteApi.GetBringRoute(new RecommendSearchParam(favoritesEditorPage.GetBringSortType(),
					favorite.characterCode, favorite.weaponType, favoritesEditorPage.GetBringPageIndex(),
					favoritesEditorPage.GetBringFilterType(), favoritesEditorPage.GetRecentlyVersion())), false,
				delegate(RequestDelegateError err, RouteApi.BringRouteInfo res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
						return;
					}

					MonoBehaviourInstance<LobbyUI>.inst.ShowBringGuard(true);
					editorViewStyle = FavoriteEditorViewStyle.ROUTEBRING;
					favoritesListPage.ClosePage();
					favoritesEditorPage.Load(favorite);
					favoritesEditorPage.BringModeUI();
					favoritesEditorPage.OpenPage();
					favoritesEditorPage.BringRoute.SetBringRoutes(favorite, res.recommendWeaponRoutesCount,
						res.recommendWeaponRoutes, res.myRecommendWeaponRoutes, res.initRecommendWeaponRoute);
				});
		}


		private void OpenEditor(Favorite favorite, bool isCheckMode)
		{
			editorViewStyle = isCheckMode ? FavoriteEditorViewStyle.ROUTECHECK : FavoriteEditorViewStyle.NORMAL;
			favoritesEditorPage.Load(favorite);
			SetView(View.EditorPage);
			if (editorViewStyle == FavoriteEditorViewStyle.ROUTECHECK)
			{
				favoritesEditorPage.CheckModeUI(editorViewStyle);
			}
		}


		private void OpenBringMode(Favorite favorite)
		{
			SetViewBringMode(favorite);
		}


		private void SetChangeState(bool change, bool share)
		{
			isChange = change;
			isShare = share;
		}


		private bool ImpossibleFavoriteTitle(Favorite favorite)
		{
			return !ArchStringUtil.IsOverSizeANSI(favorite.title, 2) ||
			       ArchStringUtil.IsOverSizeANSI(favorite.title, 32);
		}


		private void SaveFavoriteGoToListPage(Favorite favorite)
		{
			if (favorite.weaponCodes.Count == 0)
			{
				if (ImpossibleFavoriteTitle(favorite))
				{
					MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("루트 이름 길이 에러"), new Popup.Button
					{
						text = Ln.Get("확인")
					});
				}
				else if (SingletonMonoBehaviour<SwearWordManager>.inst.IsSwearWordChat(favorite.title))
				{
					MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("루트 제목에 금칙어가 포함되어 있음"), new Popup.Button
					{
						text = Ln.Get("확인")
					});
				}
				else
				{
					SetView(View.ListPage);
				}
			}
			else if (!isChange)
			{
				SetView(View.ListPage);
			}
			else if (favorite.share)
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("루트가 변경되어 기존 공유된 루트는 해제됩니다."), new Popup.Button
				{
					text = Ln.Get("저장"),
					type = Popup.ButtonType.Confirm,
					callback = (Action) (() =>
						SaveFavorites(favorite, () => SetView(View.ListPage)))
				}, new Popup.Button
				{
					text = Ln.Get("아니오"),
					type = Popup.ButtonType.Cancel,
					callback = (Action) (() => SetView(View.ListPage))
				});
			}
			else
			{
				SaveFavorites(favorite, () => SetView(View.ListPage));
			}

			// dotPeek
			// if (favorite.weaponCodes.Count == 0)
			// {
			// 	if (this.ImpossibleFavoriteTitle(favorite))
			// 	{
			// 		MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("루트 이름 길이 에러"), new Popup.Button[]
			// 		{
			// 			new Popup.Button
			// 			{
			// 				text = Ln.Get("확인")
			// 			}
			// 		});
			// 		return;
			// 	}
			// 	if (SingletonMonoBehaviour<SwearWordManager>.inst.IsSwearWordChat(favorite.title))
			// 	{
			// 		MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("루트 제목에 금칙어가 포함되어 있음"), new Popup.Button[]
			// 		{
			// 			new Popup.Button
			// 			{
			// 				text = Ln.Get("확인")
			// 			}
			// 		});
			// 		return;
			// 	}
			// 	this.SetView(LobbyFavoritesTab.View.ListPage);
			// 	return;
			// }
			// else
			// {
			// 	if (!this.isChange)
			// 	{
			// 		this.SetView(LobbyFavoritesTab.View.ListPage);
			// 		return;
			// 	}
			// 	if (favorite.share)
			// 	{
			// 		Action <>9__2;
			// 		MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("루트가 변경되어 기존 공유된 루트는 해제됩니다."), new Popup.Button[]
			// 		{
			// 			new Popup.Button
			// 			{
			// 				text = Ln.Get("저장"),
			// 				type = Popup.ButtonType.Confirm,
			// 				callback = delegate()
			// 				{
			// 					LobbyFavoritesTab <>4__this = this;
			// 					Favorite favorite2 = favorite;
			// 					Action callBack;
			// 					if ((callBack = <>9__2) == null)
			// 					{
			// 						callBack = (<>9__2 = delegate()
			// 						{
			// 							this.SetView(LobbyFavoritesTab.View.ListPage);
			// 						});
			// 					}
			// 					<>4__this.SaveFavorites(favorite2, callBack);
			// 				}
			// 			},
			// 			new Popup.Button
			// 			{
			// 				text = Ln.Get("아니오"),
			// 				type = Popup.ButtonType.Cancel,
			// 				callback = delegate()
			// 				{
			// 					this.SetView(LobbyFavoritesTab.View.ListPage);
			// 				}
			// 			}
			// 		});
			// 		return;
			// 	}
			// 	this.SaveFavorites(favorite, delegate
			// 	{
			// 		this.SetView(LobbyFavoritesTab.View.ListPage);
			// 	});
			// 	return;
			// }
		}


		private void SaveFavorites(Favorite favorite, Action callBack)
		{
			if (ImpossibleFavoriteTitle(favorite))
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("루트 이름 길이 에러"), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return;
			}

			if (SingletonMonoBehaviour<SwearWordManager>.inst.IsSwearWordChat(favorite.title))
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("루트 제목에 금칙어가 포함되어 있음"), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return;
			}

			RouteApi.Save(favorite, callBack);
		}


		private void ResetFavorite(Favorite favorite, FavoriteBoard favoriteBoard)
		{
			RouteApi.Delete(favorite, delegate { SetView(View.ListPage); });
		}


		private void ShareFavorite()
		{
			SetView(View.ListPage);
		}
	}
}