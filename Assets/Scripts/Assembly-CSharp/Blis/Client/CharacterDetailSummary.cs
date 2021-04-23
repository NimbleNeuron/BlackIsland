using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterDetailSummary : BasePage
	{
		private readonly MasteryType currentMasteryType = default;


		private readonly Image[] difficulty = new Image[3];


		private readonly List<CharacterTabCharacterDetail.MasteryIcon> masteryIcons =
			new List<CharacterTabCharacterDetail.MasteryIcon>();

		private int controlLevel;


		private DrawGraph drawGraph;


		private Transform graph;


		private LayoutGroup masterysGroup;


		private int selectedCharacterCode;


		private Text story;


		private Text storyTitle;


		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			masterysGroup = GameUtil.Bind<LayoutGroup>(this.gameObject, "WpTypeGroup");
			for (int i = 0; i < masterysGroup.transform.childCount; i++)
			{
				GameObject gameObject = masterysGroup.transform.GetChild(i).gameObject;
				masteryIcons.Add(
					new CharacterTabCharacterDetail.MasteryIcon(masterysGroup.transform.GetChild(i).gameObject));
			}

			difficulty[0] = GameUtil.Bind<Image>(this.gameObject, "Difficulty/Tab_1/Filled");
			difficulty[1] = GameUtil.Bind<Image>(this.gameObject, "Difficulty/Tab_2/Filled");
			difficulty[2] = GameUtil.Bind<Image>(this.gameObject, "Difficulty/Tab_3/Filled");
			storyTitle = GameUtil.Bind<Text>(this.gameObject, "StoryTitle");
			story = GameUtil.Bind<Text>(this.gameObject, "StoryDesc/Viewport/Content/Desc");
			graph = GameUtil.Bind<Transform>(this.gameObject, "Graph");
			drawGraph = new DrawGraph(GameUtil.Bind<RectTransform>(graph.gameObject, "Pivot/CustomMesh").gameObject);
		}


		public void SetCharacterData(int characterCode, CharacterMasteryData masteryData)
		{
			selectedCharacterCode = characterCode;
			storyTitle.text = Ln.Get(string.Format("CharacterStoryTitle/{0}", selectedCharacterCode));
			story.text = Ln.Get(string.Format("CharacterStoryDesc/{0}", selectedCharacterCode));
			SetMastery(masteryData);
			masteryIcons[0].Toggle.isOn = true;
			SetDifficulty(masteryData.weapon1);
		}


		private void SetMastery(CharacterMasteryData masteryData)
		{
			if (masteryData != null)
			{
				masteryIcons.ForEach(delegate(CharacterTabCharacterDetail.MasteryIcon x)
				{
					x.SetMastery(MasteryType.None);
				});
				List<MasteryType> masteries = masteryData.GetMasteries();
				int index = 0;
				using (List<MasteryType>.Enumerator enumerator = masteries.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MasteryType mastery = enumerator.Current;
						masteryIcons[index].SetMastery(mastery);
						masteryIcons[index++].Toggle.onValueChanged.AddListener(delegate(bool isOn)
						{
							OnToggleChange(mastery, isOn);
						});
					}
				}
			}
		}


		private void OnToggleChange(MasteryType masteryType, bool isOn)
		{
			if (isOn && masteryType != MasteryType.None && currentMasteryType != masteryType)
			{
				SetDifficulty(masteryType);
			}
		}


		private void SetDifficulty(MasteryType masteryType)
		{
			CharacterAttributeData characterAttributeData = GameDB.characterAttibute
				.GetCharacterAttributeDatas(selectedCharacterCode).Find(x => x.mastery == masteryType);
			for (int i = 0; i < difficulty.Length; i++)
			{
				difficulty[i].enabled = false;
			}

			if (characterAttributeData != null)
			{
				int num = 0;
				while (num < difficulty.Length && num < characterAttributeData.controlDifficulty)
				{
					difficulty[num].enabled = true;
					num++;
				}

				drawGraph.SetValue(characterAttributeData.attack, characterAttributeData.assistance,
					characterAttributeData.move, characterAttributeData.disruptor, characterAttributeData.defense);
			}
		}


		private class DrawGraph
		{
			private readonly Vector3 ASSISTANCE_MAX = new Vector3(89f, 30f, 0f);


			private readonly Vector3 ATTACK_MAX = new Vector3(-0.5f, 94f, 0f);


			private readonly CanvasRenderer canvasRenderer;


			private readonly Vector3 DEFENSE_MAX = new Vector3(-89f, 30f, 0f);


			private readonly Vector3 DISRUPTOR_MAX = new Vector3(-54.7f, -74f, 0f);


			private readonly Material material;


			private readonly Mesh mesh;


			private readonly Vector3 MOVE_MAX = new Vector3(54.7f, -74f, 0f);


			private readonly int[] mTris =
			{
				0,
				1,
				2,
				0,
				2,
				3,
				0,
				3,
				4,
				0,
				4,
				5,
				0,
				5,
				1
			};


			private readonly Vector2[] mUVs = new Vector2[6];


			private readonly Vector3[] mVerts = new Vector3[6];

			public DrawGraph(GameObject gameObject)
			{
				mesh = new Mesh();
				material = gameObject.GetComponent<MeshRenderer>().material;
				GameUtil.Bind<CanvasRenderer>(gameObject, ref canvasRenderer);
				canvasRenderer.SetMaterial(material, null);
				mVerts[0] = Vector3.zero;
				mVerts[1] = Vector3.zero;
				mVerts[2] = Vector3.zero;
				mVerts[3] = Vector3.zero;
				mVerts[4] = Vector3.zero;
				mVerts[5] = Vector3.zero;
				mUVs[0] = Vector3.zero;
				mUVs[1] = ATTACK_MAX;
				mUVs[2] = ASSISTANCE_MAX;
				mUVs[3] = MOVE_MAX;
				mUVs[4] = DISRUPTOR_MAX;
				mUVs[5] = DEFENSE_MAX;
				mesh.vertices = mVerts;
				mesh.uv = mUVs;
				mesh.triangles = mTris;
			}


			public void SetValue(int attack, int assistance, int move, int disruptor, int defense)
			{
				mVerts[0] = Vector3.zero;
				mVerts[1] = ATTACK_MAX / 3f * attack;
				mVerts[2] = ASSISTANCE_MAX / 3f * assistance;
				mVerts[3] = MOVE_MAX / 3f * move;
				mVerts[4] = DISRUPTOR_MAX / 3f * disruptor;
				mVerts[5] = DEFENSE_MAX / 3f * defense;
				mesh.Clear();
				mesh.vertices = mVerts;
				mesh.uv = mUVs;
				mesh.triangles = mTris;
				canvasRenderer.SetMesh(mesh);
			}
		}
	}
}