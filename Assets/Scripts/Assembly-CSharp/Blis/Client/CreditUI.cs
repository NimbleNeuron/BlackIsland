using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class CreditUI : BaseUI
	{
		private const float OrigianlVelocity = 140f;


		private const string CREDIT_DATA_BASE_PATH = "LocalizationDB/CreditData";


		[SerializeField] private GameObject creditSlotPrefab = default;


		private readonly Vector2 initialContentListRectTransformPosition = default;


		private RectTransform backgroundRectTransform;


		private RectTransform contentListRectTransform;


		private CreditData creditData;


		private RectTransform creditListRectTransform;


		private GameObject list_0;


		private GameObject list_1;


		private GameObject list_3;


		private float scrollMoveAmount;


		private LnText subjectAlphaTester;


		private LnText subjectCommunityFoundes;


		private LnText subjectLocalizationHelpingTeam;


		private LnText subjectTitleAlphaTestersCBT;


		private LnText subjectTitleAlphaTestersFifth;


		private LnText subjectTitleAlphaTestersFirst;


		private LnText subjectTitleAlphaTestersFourth;


		private LnText subjectTitleAlphaTestersSecond;


		private LnText subjectTitleAlphaTestersThird;


		private LnText subjectTitleLocalizationHelpingTeamEnglish;


		private LnText subjectTitleLocalizationHelpingTeamFrench;


		private LnText subjectTitleLocalizationHelpingTeamGerman;


		private LnText subjectTitleLocalizationHelpingTeamIndonesian;


		private LnText subjectTitleLocalizationHelpingTeamPortuguese;


		private LnText subjectTitleLocalizationHelpingTeamRussian;


		private LnText subjectTitleLocalizationHelpingTeamSimplifiedChinese;


		private LnText subjectTitleLocalizationHelpingTeamSpanish;


		private LnText subjectTitleLocalizationHelpingTeamThai;


		private LnText subjectTitleLocalizationHelpingTeamTraditionalChinese;


		private LnText subjectTitleLocalizationHelpingTeamVietnamese;


		private LnText thank;


		private Transform transformAlphaTestersCBT;


		private Transform transformAlphaTestersFifth;


		private Transform transformAlphaTestersFirst;


		private Transform transformAlphaTestersFourth;


		private Transform transformAlphaTestersSecond;


		private Transform transformAlphaTestersThird;


		private Transform transformCommunityFoundes;


		private Transform transformLocalizationHelpingTeamEnglish;


		private Transform transformLocalizationHelpingTeamFrench;


		private Transform transformLocalizationHelpingTeamGerman;


		private Transform transformLocalizationHelpingTeamIndonesian;


		private Transform transformLocalizationHelpingTeamPortuguese;


		private Transform transformLocalizationHelpingTeamRussian;


		private Transform transformLocalizationHelpingTeamSimplifiedChinese;


		private Transform transformLocalizationHelpingTeamSpanish;


		private Transform transformLocalizationHelpingTeamThai;


		private Transform transformLocalizationHelpingTeamTraditionalChinese;


		private Transform transformLocalizationHelpingTeamVietnamese;


		private float velocity = 140f;

		protected override void Start()
		{
			base.Start();
			BindGameObject();
			LoadCreditData();
			SetLnText();
			SetInitialData();
		}


		private void Update()
		{
			if (backgroundRectTransform == null)
			{
				return;
			}

			if (contentListRectTransform == null)
			{
				return;
			}

			if (creditListRectTransform == null)
			{
				return;
			}

			if (scrollMoveAmount < creditListRectTransform.rect.height + backgroundRectTransform.rect.height * 3f / 2f)
			{
				scrollMoveAmount += velocity * Time.deltaTime;
				contentListRectTransform.anchoredPosition = new Vector2(contentListRectTransform.anchoredPosition.x,
					contentListRectTransform.anchoredPosition.y + velocity * Time.deltaTime);
			}

			if (GetMouseButtonDown())
			{
				velocity = 420f;
			}

			if (GetMouseButtonUp())
			{
				velocity = 140f;
			}
		}


		private void BindGameObject()
		{
			backgroundRectTransform = GameUtil.Bind<RectTransform>(gameObject, "Bg");
			contentListRectTransform = GameUtil.Bind<RectTransform>(backgroundRectTransform.gameObject, "Content");
			creditListRectTransform = GameUtil.Bind<RectTransform>(contentListRectTransform.gameObject, "CreditList");
			thank = GameUtil.Bind<LnText>(contentListRectTransform.gameObject, "BI/Txt_Thank");
			list_0 = GameUtil.Bind<Transform>(creditListRectTransform.gameObject, "List_0").gameObject;
			subjectCommunityFoundes = GameUtil.Bind<LnText>(list_0, "Subject/Text");
			transformCommunityFoundes = GameUtil.Bind<Transform>(list_0, "Content");
			list_1 = GameUtil.Bind<Transform>(creditListRectTransform.gameObject, "List_1").gameObject;
			subjectAlphaTester = GameUtil.Bind<LnText>(list_1, "Subject/Text");
			subjectTitleAlphaTestersFirst = GameUtil.Bind<LnText>(list_1, "Content/Content1/ContentSubjectText_1");
			transformAlphaTestersFirst = GameUtil.Bind<Transform>(list_1, "Content/Content1/SubjectContent");
			subjectTitleAlphaTestersSecond = GameUtil.Bind<LnText>(list_1, "Content/Content2/ContentSubjectText_2");
			transformAlphaTestersSecond = GameUtil.Bind<Transform>(list_1, "Content/Content2/SubjectContent");
			subjectTitleAlphaTestersThird = GameUtil.Bind<LnText>(list_1, "Content/Content3/ContentSubjectText_3");
			transformAlphaTestersThird = GameUtil.Bind<Transform>(list_1, "Content/Content3/SubjectContent");
			subjectTitleAlphaTestersFourth = GameUtil.Bind<LnText>(list_1, "Content/Content4/ContentSubjectText_4");
			transformAlphaTestersFourth = GameUtil.Bind<Transform>(list_1, "Content/Content4/SubjectContent");
			subjectTitleAlphaTestersFifth = GameUtil.Bind<LnText>(list_1, "Content/Content5/ContentSubjectText_5");
			transformAlphaTestersFifth = GameUtil.Bind<Transform>(list_1, "Content/Content5/SubjectContent");
			subjectTitleAlphaTestersCBT = GameUtil.Bind<LnText>(list_1, "Content/Content6/ContentSubjectText_6");
			transformAlphaTestersCBT = GameUtil.Bind<Transform>(list_1, "Content/Content6/SubjectContent");
			list_3 = GameUtil.Bind<Transform>(creditListRectTransform.gameObject, "List_3").gameObject;
			subjectLocalizationHelpingTeam = GameUtil.Bind<LnText>(list_3, "Subject/Text");
			subjectTitleLocalizationHelpingTeamEnglish =
				GameUtil.Bind<LnText>(list_3, "Content/Content1/ContentSubjectText_English");
			transformLocalizationHelpingTeamEnglish =
				GameUtil.Bind<Transform>(list_3, "Content/Content1/SubjectContent");
			subjectTitleLocalizationHelpingTeamRussian =
				GameUtil.Bind<LnText>(list_3, "Content/Content2/ContentSubjectText_Russian");
			transformLocalizationHelpingTeamRussian =
				GameUtil.Bind<Transform>(list_3, "Content/Content2/SubjectContent");
			subjectTitleLocalizationHelpingTeamFrench =
				GameUtil.Bind<LnText>(list_3, "Content/Content3/ContentSubjectText_French");
			transformLocalizationHelpingTeamFrench =
				GameUtil.Bind<Transform>(list_3, "Content/Content3/SubjectContent");
			subjectTitleLocalizationHelpingTeamSpanish =
				GameUtil.Bind<LnText>(list_3, "Content/Content4/ContentSubjectText_Spanish");
			transformLocalizationHelpingTeamSpanish =
				GameUtil.Bind<Transform>(list_3, "Content/Content4/SubjectContent");
			subjectTitleLocalizationHelpingTeamSimplifiedChinese =
				GameUtil.Bind<LnText>(list_3, "Content/Content5/ContentSubjectText_SimplifiedChinese");
			transformLocalizationHelpingTeamSimplifiedChinese =
				GameUtil.Bind<Transform>(list_3, "Content/Content5/SubjectContent");
			subjectTitleLocalizationHelpingTeamTraditionalChinese =
				GameUtil.Bind<LnText>(list_3, "Content/Content6/ContentSubjectText_TraditionalChinese");
			transformLocalizationHelpingTeamTraditionalChinese =
				GameUtil.Bind<Transform>(list_3, "Content/Content6/SubjectContent");
			subjectTitleLocalizationHelpingTeamThai =
				GameUtil.Bind<LnText>(list_3, "Content/Content7/ContentSubjectText_Thai");
			transformLocalizationHelpingTeamThai = GameUtil.Bind<Transform>(list_3, "Content/Content7/SubjectContent");
			subjectTitleLocalizationHelpingTeamIndonesian =
				GameUtil.Bind<LnText>(list_3, "Content/Content8/ContentSubjectText_Indonesian");
			transformLocalizationHelpingTeamIndonesian =
				GameUtil.Bind<Transform>(list_3, "Content/Content8/SubjectContent");
			subjectTitleLocalizationHelpingTeamPortuguese =
				GameUtil.Bind<LnText>(list_3, "Content/Content9/ContentSubjectText_Portuguese");
			transformLocalizationHelpingTeamPortuguese =
				GameUtil.Bind<Transform>(list_3, "Content/Content9/SubjectContent");
			subjectTitleLocalizationHelpingTeamGerman =
				GameUtil.Bind<LnText>(list_3, "Content/Content10/ContentSubjectText_German");
			transformLocalizationHelpingTeamGerman =
				GameUtil.Bind<Transform>(list_3, "Content/Content10/SubjectContent");
			subjectTitleLocalizationHelpingTeamVietnamese =
				GameUtil.Bind<LnText>(list_3, "Content/Content11/ContentSubjectText_Vietnamese");
			transformLocalizationHelpingTeamVietnamese =
				GameUtil.Bind<Transform>(list_3, "Content/Content11/SubjectContent");
		}


		private void LoadCreditData()
		{
			creditData = Resources.Load<CreditData>("LocalizationDB/CreditData");
			if (creditData == null)
			{
				Log.E("creditData is null");
			}
		}


		private void SetLnText()
		{
			thank.text = Ln.Get("CreditThanks");
			subjectCommunityFoundes.text = Ln.Get("SubjectCommunityFoundes");
			subjectAlphaTester.text = Ln.Get("SubjectAlphaTester");
			subjectTitleAlphaTestersFirst.text = Ln.Get("SubjectTitleAlphaTesterFirst");
			subjectTitleAlphaTestersSecond.text = Ln.Get("SubjectTitleAlphaTesterSecond");
			subjectTitleAlphaTestersThird.text = Ln.Get("SubjectTitleAlphaTesterThird");
			subjectTitleAlphaTestersFourth.text = Ln.Get("SubjectTitleAlphaTesterFourth");
			subjectTitleAlphaTestersFifth.text = Ln.Get("SubjectTitleAlphaTesterFifth");
			subjectTitleAlphaTestersCBT.text = Ln.Get("SubjectTitleAlphaTesterCBT");
			subjectLocalizationHelpingTeam.text = Ln.Get("SubjectLocalizationHelpingTeam");
			subjectTitleLocalizationHelpingTeamEnglish.text = Ln.Get("SubjectTitleLocalizationHelpingTeamEnglish");
			subjectTitleLocalizationHelpingTeamRussian.text = Ln.Get("SubjectTitleLocalizationHelpingTeamRussian");
			subjectTitleLocalizationHelpingTeamFrench.text = Ln.Get("SubjectTitleLocalizationHelpingTeamFrench");
			subjectTitleLocalizationHelpingTeamSpanish.text = Ln.Get("SubjectTitleLocalizationHelpingTeamSpanish");
			subjectTitleLocalizationHelpingTeamSimplifiedChinese.text =
				Ln.Get("SubjectTitleLocalizationHelpingTeamSimplified Chinese");
			subjectTitleLocalizationHelpingTeamTraditionalChinese.text =
				Ln.Get("SubjectTitleLocalizationHelpingTeamTraditional Chinese");
			subjectTitleLocalizationHelpingTeamThai.text = Ln.Get("SubjectTitleLocalizationHelpingTeamThai");
			subjectTitleLocalizationHelpingTeamIndonesian.text =
				Ln.Get("SubjectTitleLocalizationHelpingTeamIndonesian");
			subjectTitleLocalizationHelpingTeamPortuguese.text =
				Ln.Get("SubjectTitleLocalizationHelpingTeamPortuguese");
			subjectTitleLocalizationHelpingTeamGerman.text = Ln.Get("SubjectTitleLocalizationHelpingTeamGerman");
			subjectTitleLocalizationHelpingTeamVietnamese.text =
				Ln.Get("SubjectTitleLocalizationHelpingTeamVietnamese");
			if (creditSlotPrefab == null)
			{
				Log.E("creditSlotPrefab is null!!");
				return;
			}

			if (creditData == null)
			{
				Log.E("creditData is null!!");
				return;
			}

			AddCreditSlotAndSetLnData(transformCommunityFoundes, creditData.communityFounders);
			AddCreditSlotAndSetLnData(transformAlphaTestersFirst, creditData.alphaTestersFirst);
			AddCreditSlotAndSetLnData(transformAlphaTestersSecond, creditData.alphaTestersSecond);
			AddCreditSlotAndSetLnData(transformAlphaTestersThird, creditData.alphaTestersThird);
			AddCreditSlotAndSetLnData(transformAlphaTestersFourth, creditData.alphaTestersFourth);
			AddCreditSlotAndSetLnData(transformAlphaTestersFifth, creditData.alphaTestersFifth);
			AddCreditSlotAndSetLnData(transformAlphaTestersCBT, creditData.alphaTestersCBT);
			AddCreditSlotAndSetLnData(transformLocalizationHelpingTeamEnglish,
				creditData.localizationHelpingTeamEnglish);
			AddCreditSlotAndSetLnData(transformLocalizationHelpingTeamRussian,
				creditData.localizationHelpingTeamRussian);
			AddCreditSlotAndSetLnData(transformLocalizationHelpingTeamFrench, creditData.localizationHelpingTeamFrench);
			AddCreditSlotAndSetLnData(transformLocalizationHelpingTeamSpanish,
				creditData.localizationHelpingTeamSpanish);
			AddCreditSlotAndSetLnData(transformLocalizationHelpingTeamSimplifiedChinese,
				creditData.localizationHelpingTeamSimplifiedChinese);
			AddCreditSlotAndSetLnData(transformLocalizationHelpingTeamTraditionalChinese,
				creditData.localizationHelpingTeamTraditionalChinese);
			AddCreditSlotAndSetLnData(transformLocalizationHelpingTeamThai, creditData.localizationHelpingTeamThai);
			AddCreditSlotAndSetLnData(transformLocalizationHelpingTeamIndonesian,
				creditData.localizationHelpingTeamIndonesian);
			AddCreditSlotAndSetLnData(transformLocalizationHelpingTeamPortuguese,
				creditData.localizationHelpingTeamPortuguese);
			AddCreditSlotAndSetLnData(transformLocalizationHelpingTeamGerman, creditData.localizationHelpingTeamGerman);
			AddCreditSlotAndSetLnData(transformLocalizationHelpingTeamVietnamese,
				creditData.localizationHelpingTeamVietnamese);
		}


		private void AddCreditSlotAndSetLnData(Transform parent, string[] strData)
		{
			if (parent == null)
			{
				Log.E(parent.name + " is null!!");
				return;
			}

			if (strData == null)
			{
				Log.E(string.Format("{0} is null!!", strData));
				return;
			}

			for (int i = 0; i < strData.Length; i++)
			{
				LnText component = Instantiate<GameObject>(creditSlotPrefab, parent).GetComponent<LnText>();
				if (component != null)
				{
					component.text = strData[i];
				}
			}
		}


		private void SetInitialData()
		{
			scrollMoveAmount = 0f;
			contentListRectTransform.anchoredPosition = initialContentListRectTransformPosition;
		}


		public void Active(bool active)
		{
			if (active)
			{
				Singleton<SoundControl>.inst.PlayBGM("Eternal world", true);
				return;
			}

			Singleton<SoundControl>.inst.PlayBGM("BGM_Lobby", true);
		}


		private bool GetMouseButtonDown()
		{
			return Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2);
		}


		private bool GetMouseButtonUp()
		{
			return Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2);
		}
	}
}