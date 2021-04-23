using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blis.Common
{
	public class SkillDB
	{
		public readonly List<SkillSlotSet> allSkillSlotSet = new List<SkillSlotSet>();


		private readonly Dictionary<int, CharacterSkillSetData> characterSkillSetMap =
			new Dictionary<int, CharacterSkillSetData>();


		private readonly List<SkillData> findSkillDatas = new List<SkillData>();


		private readonly Dictionary<int, CharacterSkillSetData> monsterSkillSetMap =
			new Dictionary<int, CharacterSkillSetData>();


		public readonly Dictionary<int, SkillEvolutionData> skillEvolutionMap =
			new Dictionary<int, SkillEvolutionData>();


		public readonly Dictionary<int, List<SkillEvolutionPointData>> skillEvolutionPointMap =
			new Dictionary<int, List<SkillEvolutionPointData>>();


		public readonly Dictionary<int, SkillGroupData> skillGroupMap = new Dictionary<int, SkillGroupData>();


		public readonly Dictionary<int, SkillData> skillMap = new Dictionary<int, SkillData>();


		private readonly Dictionary<SpecialSkillId, SpecialSkillSetData> specialSkillSetMap =
			new Dictionary<SpecialSkillId, SpecialSkillSetData>(
				SingletonComparerEnum<SpecialSkillIdComparer, SpecialSkillId>.Instance);


		private readonly Dictionary<MasteryType, WeaponSkillSetData> weaponSkillSetMap =
			new Dictionary<MasteryType, WeaponSkillSetData>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>
				.Instance);

		public SkillDB()
		{
			allSkillSlotSet.Clear();
			foreach (object obj in Enum.GetValues(typeof(SkillSlotSet)))
			{
				SkillSlotSet item = (SkillSlotSet) obj;
				allSkillSlotSet.Add(item);
			}

			Load();
		}


		public void Load()
		{
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(1).SetActive(SkillSlotSet.Attack_1, 1, 1001001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1001101).SetActive(SkillSlotSet.Passive_1, 2, 1001102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1001103).SetActive(SkillSlotSet.Active1_1, 1, 1001201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1001202).SetActive(SkillSlotSet.Active1_1, 3, 1001203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1001204).SetActive(SkillSlotSet.Active1_1, 5, 1001205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1001301).SetActive(SkillSlotSet.Active2_1, 2, 1001302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1001303).SetActive(SkillSlotSet.Active2_1, 4, 1001304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1001305).SetActive(SkillSlotSet.Active3_1, 1, 1001401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1001402).SetActive(SkillSlotSet.Active3_1, 3, 1001403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1001404).SetActive(SkillSlotSet.Active3_1, 5, 1001405)
				.SetActive(SkillSlotSet.Active4_1, 1, 1001501).SetActive(SkillSlotSet.Active4_1, 2, 1001502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1001503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(2).SetActive(SkillSlotSet.Attack_1, 1, 1002001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1002101).SetActive(SkillSlotSet.Passive_1, 2, 1002102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1002103).SetActive(SkillSlotSet.Active1_1, 1, 1002201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1002202).SetActive(SkillSlotSet.Active1_1, 3, 1002203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1002204).SetActive(SkillSlotSet.Active1_1, 5, 1002205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1002301).SetActive(SkillSlotSet.Active2_1, 2, 1002302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1002303).SetActive(SkillSlotSet.Active2_1, 4, 1002304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1002305).SetActive(SkillSlotSet.Active3_1, 1, 1002401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1002402).SetActive(SkillSlotSet.Active3_1, 3, 1002403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1002404).SetActive(SkillSlotSet.Active3_1, 5, 1002405)
				.SetActive(SkillSlotSet.Active4_1, 1, 1002501).SetActive(SkillSlotSet.Active4_1, 2, 1002502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1002503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(3).SetActive(SkillSlotSet.Attack_1, 1, 1003001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1003101).SetActive(SkillSlotSet.Passive_1, 2, 1003102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1003103).SetActive(SkillSlotSet.Active1_1, 1, 1003201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1003202).SetActive(SkillSlotSet.Active1_1, 3, 1003203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1003204).SetActive(SkillSlotSet.Active1_1, 5, 1003205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1003301).SetActive(SkillSlotSet.Active2_1, 2, 1003302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1003303).SetActive(SkillSlotSet.Active2_1, 4, 1003304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1003305).SetActive(SkillSlotSet.Active3_1, 1, 1003401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1003402).SetActive(SkillSlotSet.Active3_1, 3, 1003403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1003404).SetActive(SkillSlotSet.Active3_1, 5, 1003405)
				.SetActive(SkillSlotSet.Active3_1, 1, 1003411).SetActive(SkillSlotSet.Active3_1, 2, 1003412)
				.SetActive(SkillSlotSet.Active3_1, 3, 1003413).SetActive(SkillSlotSet.Active3_1, 4, 1003414)
				.SetActive(SkillSlotSet.Active3_1, 5, 1003415).SetActive(SkillSlotSet.Active4_1, 1, 1003501)
				.SetActive(SkillSlotSet.Active4_1, 2, 1003502).SetActive(SkillSlotSet.Active4_1, 3, 1003503)
				.SetActive(SkillSlotSet.Active4_1, 1, 1003511).SetActive(SkillSlotSet.Active4_1, 2, 1003512)
				.SetActive(SkillSlotSet.Active4_1, 3, 1003513).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(4).SetActive(SkillSlotSet.Attack_1, 1, 1004001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1004101).SetActive(SkillSlotSet.Passive_1, 2, 1004102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1004103).SetActive(SkillSlotSet.Active1_1, 1, 1004201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1004202).SetActive(SkillSlotSet.Active1_1, 3, 1004203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1004204).SetActive(SkillSlotSet.Active1_1, 5, 1004205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1004301).SetActive(SkillSlotSet.Active2_1, 2, 1004302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1004303).SetActive(SkillSlotSet.Active2_1, 4, 1004304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1004305).SetActive(SkillSlotSet.Active3_1, 1, 1004401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1004402).SetActive(SkillSlotSet.Active3_1, 3, 1004403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1004404).SetActive(SkillSlotSet.Active3_1, 5, 1004405)
				.SetActive(SkillSlotSet.Active4_1, 1, 1004501).SetActive(SkillSlotSet.Active4_1, 2, 1004502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1004503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(5).SetActive(SkillSlotSet.Attack_1, 1, 1005001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1005101).SetActive(SkillSlotSet.Passive_1, 2, 1005102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1005103).SetActive(SkillSlotSet.Active1_1, 1, 1005201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1005202).SetActive(SkillSlotSet.Active1_1, 3, 1005203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1005204).SetActive(SkillSlotSet.Active1_1, 5, 1005205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1005301).SetActive(SkillSlotSet.Active2_1, 2, 1005302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1005303).SetActive(SkillSlotSet.Active2_1, 4, 1005304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1005305).SetActive(SkillSlotSet.Active3_1, 1, 1005401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1005402).SetActive(SkillSlotSet.Active3_1, 3, 1005403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1005404).SetActive(SkillSlotSet.Active3_1, 5, 1005405)
				.SetActive(SkillSlotSet.Active4_1, 1, 1005501).SetActive(SkillSlotSet.Active4_1, 2, 1005502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1005503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(6).SetActive(SkillSlotSet.Attack_1, 1, 1006001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1006101).SetActive(SkillSlotSet.Passive_1, 2, 1006102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1006103).SetActive(SkillSlotSet.Active1_1, 1, 1006201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1006202).SetActive(SkillSlotSet.Active1_1, 3, 1006203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1006204).SetActive(SkillSlotSet.Active1_1, 5, 1006205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1006301).SetActive(SkillSlotSet.Active2_1, 2, 1006302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1006303).SetActive(SkillSlotSet.Active2_1, 4, 1006304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1006305).SetActive(SkillSlotSet.Active2_1, 1, 1006311)
				.SetActive(SkillSlotSet.Active2_1, 2, 1006312).SetActive(SkillSlotSet.Active2_1, 3, 1006313)
				.SetActive(SkillSlotSet.Active2_1, 4, 1006314).SetActive(SkillSlotSet.Active2_1, 5, 1006315)
				.SetActive(SkillSlotSet.Active3_1, 1, 1006401).SetActive(SkillSlotSet.Active3_1, 2, 1006402)
				.SetActive(SkillSlotSet.Active3_1, 3, 1006403).SetActive(SkillSlotSet.Active3_1, 4, 1006404)
				.SetActive(SkillSlotSet.Active3_1, 5, 1006405).SetActive(SkillSlotSet.Active3_1, 1, 1006411)
				.SetActive(SkillSlotSet.Active3_1, 2, 1006412).SetActive(SkillSlotSet.Active3_1, 3, 1006413)
				.SetActive(SkillSlotSet.Active3_1, 4, 1006414).SetActive(SkillSlotSet.Active3_1, 5, 1006415)
				.SetActive(SkillSlotSet.Active4_1, 1, 1006501).SetActive(SkillSlotSet.Active4_1, 2, 1006502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1006503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(7).SetActive(SkillSlotSet.Attack_1, 1, 1007001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1007101).SetActive(SkillSlotSet.Passive_1, 2, 1007102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1007103).SetActive(SkillSlotSet.Active1_1, 1, 1007201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1007202).SetActive(SkillSlotSet.Active1_1, 3, 1007203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1007204).SetActive(SkillSlotSet.Active1_1, 5, 1007205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1007301).SetActive(SkillSlotSet.Active2_1, 2, 1007302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1007303).SetActive(SkillSlotSet.Active2_1, 4, 1007304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1007305).SetActive(SkillSlotSet.Active3_1, 1, 1007401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1007402).SetActive(SkillSlotSet.Active3_1, 3, 1007403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1007404).SetActive(SkillSlotSet.Active3_1, 5, 1007405)
				.SetActive(SkillSlotSet.Active4_1, 1, 1007501).SetActive(SkillSlotSet.Active4_1, 2, 1007502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1007503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(8).SetActive(SkillSlotSet.Attack_1, 1, 1008001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1008101).SetActive(SkillSlotSet.Passive_1, 2, 1008102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1008103).SetActive(SkillSlotSet.Active1_1, 1, 1008201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1008202).SetActive(SkillSlotSet.Active1_1, 3, 1008203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1008204).SetActive(SkillSlotSet.Active1_1, 5, 1008205)
				.SetActive(SkillSlotSet.Active1_1, 1, 1008211).SetActive(SkillSlotSet.Active1_1, 2, 1008212)
				.SetActive(SkillSlotSet.Active1_1, 3, 1008213).SetActive(SkillSlotSet.Active1_1, 4, 1008214)
				.SetActive(SkillSlotSet.Active1_1, 5, 1008215).SetActive(SkillSlotSet.Active2_1, 1, 1008301)
				.SetActive(SkillSlotSet.Active2_1, 2, 1008302).SetActive(SkillSlotSet.Active2_1, 3, 1008303)
				.SetActive(SkillSlotSet.Active2_1, 4, 1008304).SetActive(SkillSlotSet.Active2_1, 5, 1008305)
				.SetActive(SkillSlotSet.Active3_1, 1, 1008401).SetActive(SkillSlotSet.Active3_1, 2, 1008402)
				.SetActive(SkillSlotSet.Active3_1, 3, 1008403).SetActive(SkillSlotSet.Active3_1, 4, 1008404)
				.SetActive(SkillSlotSet.Active3_1, 5, 1008405).SetActive(SkillSlotSet.Active3_1, 1, 1008411)
				.SetActive(SkillSlotSet.Active3_1, 2, 1008412).SetActive(SkillSlotSet.Active3_1, 3, 1008413)
				.SetActive(SkillSlotSet.Active3_1, 4, 1008414).SetActive(SkillSlotSet.Active3_1, 5, 1008415)
				.SetActive(SkillSlotSet.Active3_1, 1, 1008421).SetActive(SkillSlotSet.Active3_1, 2, 1008422)
				.SetActive(SkillSlotSet.Active3_1, 3, 1008423).SetActive(SkillSlotSet.Active3_1, 4, 1008424)
				.SetActive(SkillSlotSet.Active3_1, 5, 1008425).SetActive(SkillSlotSet.Active4_1, 1, 1008501)
				.SetActive(SkillSlotSet.Active4_1, 2, 1008502).SetActive(SkillSlotSet.Active4_1, 3, 1008503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(9).SetActive(SkillSlotSet.Attack_1, 1, 1009001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1009101).SetActive(SkillSlotSet.Passive_1, 2, 1009102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1009103).SetActive(SkillSlotSet.Active1_1, 1, 1009201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1009202).SetActive(SkillSlotSet.Active1_1, 3, 1009203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1009204).SetActive(SkillSlotSet.Active1_1, 5, 1009205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1009301).SetActive(SkillSlotSet.Active2_1, 2, 1009302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1009303).SetActive(SkillSlotSet.Active2_1, 4, 1009304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1009305).SetActive(SkillSlotSet.Active3_1, 1, 1009401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1009402).SetActive(SkillSlotSet.Active3_1, 3, 1009403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1009404).SetActive(SkillSlotSet.Active3_1, 5, 1009405)
				.SetActive(SkillSlotSet.Active4_1, 1, 1009501).SetActive(SkillSlotSet.Active4_1, 2, 1009502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1009503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(10).SetActive(SkillSlotSet.Attack_1, 1, 1010001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1010101).SetActive(SkillSlotSet.Passive_1, 2, 1010102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1010103).SetActive(SkillSlotSet.Active1_1, 1, 1010201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1010202).SetActive(SkillSlotSet.Active1_1, 3, 1010203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1010204).SetActive(SkillSlotSet.Active1_1, 5, 1010205)
				.SetActive(SkillSlotSet.Active1_1, 1, 1010211).SetActive(SkillSlotSet.Active1_1, 2, 1010211)
				.SetActive(SkillSlotSet.Active1_1, 3, 1010211).SetActive(SkillSlotSet.Active1_1, 4, 1010211)
				.SetActive(SkillSlotSet.Active1_1, 5, 1010211).SetActive(SkillSlotSet.Active1_1, 1, 1010211)
				.SetActive(SkillSlotSet.Active1_1, 2, 1010211).SetActive(SkillSlotSet.Active1_1, 3, 1010211)
				.SetActive(SkillSlotSet.Active1_1, 4, 1010211).SetActive(SkillSlotSet.Active1_1, 5, 1010211)
				.SetActive(SkillSlotSet.Active2_1, 1, 1010301).SetActive(SkillSlotSet.Active2_1, 2, 1010302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1010303).SetActive(SkillSlotSet.Active2_1, 4, 1010304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1010305).SetActive(SkillSlotSet.Active3_1, 1, 1010401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1010402).SetActive(SkillSlotSet.Active3_1, 3, 1010403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1010404).SetActive(SkillSlotSet.Active3_1, 5, 1010405)
				.SetActive(SkillSlotSet.Active4_1, 1, 1010501).SetActive(SkillSlotSet.Active4_1, 2, 1010502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1010503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(11).SetActive(SkillSlotSet.Attack_1, 1, 1011001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1011101).SetActive(SkillSlotSet.Passive_1, 2, 1011102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1011103).SetActive(SkillSlotSet.Active1_1, 1, 1011201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1011202).SetActive(SkillSlotSet.Active1_1, 3, 1011203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1011204).SetActive(SkillSlotSet.Active1_1, 5, 1011205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1011301).SetActive(SkillSlotSet.Active2_1, 2, 1011302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1011303).SetActive(SkillSlotSet.Active2_1, 4, 1011304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1011305).SetActive(SkillSlotSet.Active3_1, 1, 1011401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1011402).SetActive(SkillSlotSet.Active3_1, 3, 1011403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1011404).SetActive(SkillSlotSet.Active3_1, 5, 1011405)
				.SetActive(SkillSlotSet.Active4_1, 1, 1011501).SetActive(SkillSlotSet.Active4_1, 2, 1011502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1011503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(12).SetActive(SkillSlotSet.Attack_1, 1, 1012001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1012101).SetActive(SkillSlotSet.Passive_1, 2, 1012102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1012103).SetActive(SkillSlotSet.Active1_1, 1, 1012201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1012202).SetActive(SkillSlotSet.Active1_1, 3, 1012203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1012204).SetActive(SkillSlotSet.Active1_1, 5, 1012205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1012301).SetActive(SkillSlotSet.Active2_1, 2, 1012302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1012303).SetActive(SkillSlotSet.Active2_1, 4, 1012304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1012305).SetActive(SkillSlotSet.Active2_1, 1, 1012311)
				.SetActive(SkillSlotSet.Active2_1, 2, 1012311).SetActive(SkillSlotSet.Active2_1, 3, 1012311)
				.SetActive(SkillSlotSet.Active2_1, 4, 1012311).SetActive(SkillSlotSet.Active2_1, 5, 1012311)
				.SetActive(SkillSlotSet.Active3_1, 1, 1012401).SetActive(SkillSlotSet.Active3_1, 2, 1012402)
				.SetActive(SkillSlotSet.Active3_1, 3, 1012403).SetActive(SkillSlotSet.Active3_1, 4, 1012404)
				.SetActive(SkillSlotSet.Active3_1, 5, 1012405).SetActive(SkillSlotSet.Active3_1, 1, 1012411)
				.SetActive(SkillSlotSet.Active3_1, 2, 1012411).SetActive(SkillSlotSet.Active3_1, 3, 1012411)
				.SetActive(SkillSlotSet.Active3_1, 4, 1012411).SetActive(SkillSlotSet.Active3_1, 5, 1012411)
				.SetActive(SkillSlotSet.Active4_1, 1, 1012501).SetActive(SkillSlotSet.Active4_1, 2, 1012502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1012503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(13).SetActive(SkillSlotSet.Attack_1, 1, 1013001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1013101).SetActive(SkillSlotSet.Passive_1, 2, 1013102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1013103).SetActive(SkillSlotSet.Active1_1, 1, 1013201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1013202).SetActive(SkillSlotSet.Active1_1, 3, 1013203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1013204).SetActive(SkillSlotSet.Active1_1, 5, 1013205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1013301).SetActive(SkillSlotSet.Active2_1, 2, 1013302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1013303).SetActive(SkillSlotSet.Active2_1, 4, 1013304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1013305).SetActive(SkillSlotSet.Active3_1, 1, 1013401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1013402).SetActive(SkillSlotSet.Active3_1, 3, 1013403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1013404).SetActive(SkillSlotSet.Active3_1, 5, 1013405)
				.SetActive(SkillSlotSet.Active4_1, 1, 1013501).SetActive(SkillSlotSet.Active4_1, 2, 1013502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1013503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(14).SetActive(SkillSlotSet.Attack_1, 1, 1014001)
				.SetActive(SkillSlotSet.Attack_2, 1, 1014011).SetActive(SkillSlotSet.Passive_1, 1, 1014101)
				.SetActive(SkillSlotSet.Passive_1, 2, 1014102).SetActive(SkillSlotSet.Passive_1, 3, 1014103)
				.SetActive(SkillSlotSet.Active1_1, 1, 1014201).SetActive(SkillSlotSet.Active1_1, 2, 1014202)
				.SetActive(SkillSlotSet.Active1_1, 3, 1014203).SetActive(SkillSlotSet.Active1_1, 4, 1014204)
				.SetActive(SkillSlotSet.Active1_1, 5, 1014205).SetActive(SkillSlotSet.Active2_1, 1, 1014301)
				.SetActive(SkillSlotSet.Active2_1, 2, 1014302).SetActive(SkillSlotSet.Active2_1, 3, 1014303)
				.SetActive(SkillSlotSet.Active2_1, 4, 1014304).SetActive(SkillSlotSet.Active2_1, 5, 1014305)
				.SetActive(SkillSlotSet.Active2_1, 1, 1014311).SetActive(SkillSlotSet.Active2_1, 2, 1014311)
				.SetActive(SkillSlotSet.Active2_1, 3, 1014311).SetActive(SkillSlotSet.Active2_1, 4, 1014311)
				.SetActive(SkillSlotSet.Active2_1, 5, 1014311).SetActive(SkillSlotSet.Active3_1, 1, 1014401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1014402).SetActive(SkillSlotSet.Active3_1, 3, 1014403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1014404).SetActive(SkillSlotSet.Active3_1, 5, 1014405)
				.SetActive(SkillSlotSet.Active4_1, 1, 1014501).SetActive(SkillSlotSet.Active4_1, 2, 1014502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1014503).SetActive(SkillSlotSet.Active4_1, 1, 1014511)
				.SetActive(SkillSlotSet.Active4_1, 2, 1014512).SetActive(SkillSlotSet.Active4_1, 3, 1014513).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(15).SetActive(SkillSlotSet.Attack_1, 1, 1015001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1015101).SetActive(SkillSlotSet.Passive_1, 2, 1015102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1015103).SetActive(SkillSlotSet.Active1_1, 1, 1015201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1015202).SetActive(SkillSlotSet.Active1_1, 3, 1015203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1015204).SetActive(SkillSlotSet.Active1_1, 5, 1015205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1015301).SetActive(SkillSlotSet.Active2_1, 2, 1015302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1015303).SetActive(SkillSlotSet.Active2_1, 4, 1015304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1015305).SetActive(SkillSlotSet.Active3_1, 1, 1015401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1015402).SetActive(SkillSlotSet.Active3_1, 3, 1015403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1015404).SetActive(SkillSlotSet.Active3_1, 5, 1015405)
				.SetActive(SkillSlotSet.Active4_1, 1, 1015501).SetActive(SkillSlotSet.Active4_1, 2, 1015502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1015503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(16).SetActive(SkillSlotSet.Passive_1, 1, 1016101)
				.SetActive(SkillSlotSet.Passive_1, 2, 1016102).SetActive(SkillSlotSet.Passive_1, 3, 1016103)
				.SetActive(SkillSlotSet.Attack_1, 1, 1016001).SetActive(SkillSlotSet.Active1_1, 1, 1016201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1016202).SetActive(SkillSlotSet.Active1_1, 3, 1016203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1016204).SetActive(SkillSlotSet.Active1_1, 5, 1016205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1016301).SetActive(SkillSlotSet.Active2_1, 2, 1016302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1016303).SetActive(SkillSlotSet.Active2_1, 4, 1016304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1016305).SetActive(SkillSlotSet.Active3_1, 1, 1016401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1016402).SetActive(SkillSlotSet.Active3_1, 3, 1016403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1016404).SetActive(SkillSlotSet.Active3_1, 5, 1016405)
				.SetActive(SkillSlotSet.Active4_1, 1, 1016501).SetActive(SkillSlotSet.Active4_1, 2, 1016502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1016503).SetActive(SkillSlotSet.Active1_2, 1, 1016601)
				.SetActive(SkillSlotSet.Active1_2, 2, 1016602).SetActive(SkillSlotSet.Active1_2, 3, 1016603)
				.SetActive(SkillSlotSet.Active1_2, 4, 1016604).SetActive(SkillSlotSet.Active1_2, 5, 1016605)
				.SetActive(SkillSlotSet.Active2_2, 1, 1016701).SetActive(SkillSlotSet.Active2_2, 2, 1016702)
				.SetActive(SkillSlotSet.Active2_2, 3, 1016703).SetActive(SkillSlotSet.Active2_2, 4, 1016704)
				.SetActive(SkillSlotSet.Active2_2, 5, 1016705).SetActive(SkillSlotSet.Active3_2, 1, 1016801)
				.SetActive(SkillSlotSet.Active3_2, 2, 1016802).SetActive(SkillSlotSet.Active3_2, 3, 1016803)
				.SetActive(SkillSlotSet.Active3_2, 4, 1016804).SetActive(SkillSlotSet.Active3_2, 5, 1016805)
				.SetActive(SkillSlotSet.Active4_2, 1, 1016901).SetActive(SkillSlotSet.Active4_2, 2, 1016902)
				.SetActive(SkillSlotSet.Active4_2, 3, 1016903).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(17).SetActive(SkillSlotSet.Attack_1, 1, 1017001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1017101).SetActive(SkillSlotSet.Passive_1, 2, 1017102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1017103).SetActive(SkillSlotSet.Passive_1, 1, 1017111)
				.SetActive(SkillSlotSet.Passive_1, 2, 1017112).SetActive(SkillSlotSet.Passive_1, 3, 1017113)
				.SetActive(SkillSlotSet.Active1_1, 1, 1017201).SetActive(SkillSlotSet.Active1_1, 2, 1017202)
				.SetActive(SkillSlotSet.Active1_1, 3, 1017203).SetActive(SkillSlotSet.Active1_1, 4, 1017204)
				.SetActive(SkillSlotSet.Active1_1, 5, 1017205).SetActive(SkillSlotSet.Active2_1, 1, 1017301)
				.SetActive(SkillSlotSet.Active2_1, 2, 1017302).SetActive(SkillSlotSet.Active2_1, 3, 1017303)
				.SetActive(SkillSlotSet.Active2_1, 4, 1017304).SetActive(SkillSlotSet.Active2_1, 5, 1017305)
				.SetActive(SkillSlotSet.Active3_1, 1, 1017401).SetActive(SkillSlotSet.Active3_1, 2, 1017402)
				.SetActive(SkillSlotSet.Active3_1, 3, 1017403).SetActive(SkillSlotSet.Active3_1, 4, 1017404)
				.SetActive(SkillSlotSet.Active3_1, 5, 1017405).SetActive(SkillSlotSet.Active4_1, 1, 1017501)
				.SetActive(SkillSlotSet.Active4_1, 2, 1017502).SetActive(SkillSlotSet.Active4_1, 3, 1017503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(18).SetActive(SkillSlotSet.Attack_1, 1, 1018001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1018101).SetActive(SkillSlotSet.Passive_1, 2, 1018102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1018103).SetActive(SkillSlotSet.Active1_1, 1, 1018201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1018202).SetActive(SkillSlotSet.Active1_1, 3, 1018203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1018204).SetActive(SkillSlotSet.Active1_1, 5, 1018205)
				.SetActive(SkillSlotSet.Active1_1, 1, 1018211).SetActive(SkillSlotSet.Active1_1, 2, 1018212)
				.SetActive(SkillSlotSet.Active1_1, 3, 1018213).SetActive(SkillSlotSet.Active1_1, 4, 1018214)
				.SetActive(SkillSlotSet.Active1_1, 5, 1018215).SetActive(SkillSlotSet.Active2_1, 1, 1018301)
				.SetActive(SkillSlotSet.Active2_1, 2, 1018302).SetActive(SkillSlotSet.Active2_1, 3, 1018303)
				.SetActive(SkillSlotSet.Active2_1, 4, 1018304).SetActive(SkillSlotSet.Active2_1, 5, 1018305)
				.SetActive(SkillSlotSet.Active3_1, 1, 1018401).SetActive(SkillSlotSet.Active3_1, 2, 1018402)
				.SetActive(SkillSlotSet.Active3_1, 3, 1018403).SetActive(SkillSlotSet.Active3_1, 4, 1018404)
				.SetActive(SkillSlotSet.Active3_1, 5, 1018405).SetActive(SkillSlotSet.Active4_1, 1, 1018501)
				.SetActive(SkillSlotSet.Active4_1, 2, 1018502).SetActive(SkillSlotSet.Active4_1, 3, 1018503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(19).SetActive(SkillSlotSet.Attack_1, 1, 1019001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1019101).SetActive(SkillSlotSet.Passive_1, 2, 1019102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1019103).SetActive(SkillSlotSet.Active1_1, 1, 1019201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1019202).SetActive(SkillSlotSet.Active1_1, 3, 1019203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1019204).SetActive(SkillSlotSet.Active1_1, 5, 1019205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1019301).SetActive(SkillSlotSet.Active2_1, 2, 1019302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1019303).SetActive(SkillSlotSet.Active2_1, 4, 1019304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1019305).SetActive(SkillSlotSet.Active3_1, 1, 1019401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1019402).SetActive(SkillSlotSet.Active3_1, 3, 1019403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1019404).SetActive(SkillSlotSet.Active3_1, 5, 1019405)
				.SetActive(SkillSlotSet.Active4_1, 1, 1019501).SetActive(SkillSlotSet.Active4_1, 2, 1019502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1019503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(20).SetActive(SkillSlotSet.Attack_1, 1, 1020001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1020101).SetActive(SkillSlotSet.Passive_1, 2, 1020102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1020103).SetActive(SkillSlotSet.Active1_1, 1, 1020201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1020202).SetActive(SkillSlotSet.Active1_1, 3, 1020203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1020204).SetActive(SkillSlotSet.Active1_1, 5, 1020205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1020301).SetActive(SkillSlotSet.Active2_1, 2, 1020302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1020303).SetActive(SkillSlotSet.Active2_1, 4, 1020304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1020305).SetActive(SkillSlotSet.Active3_1, 1, 1020401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1020402).SetActive(SkillSlotSet.Active3_1, 3, 1020403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1020404).SetActive(SkillSlotSet.Active3_1, 5, 1020405)
				.SetActive(SkillSlotSet.Active4_1, 1, 1020501).SetActive(SkillSlotSet.Active4_1, 2, 1020502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1020503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(21).SetActive(SkillSlotSet.Attack_1, 1, 1021001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1021101).SetActive(SkillSlotSet.Passive_1, 2, 1021102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1021103).SetActive(SkillSlotSet.Active1_1, 1, 1021201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1021202).SetActive(SkillSlotSet.Active1_1, 3, 1021203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1021204).SetActive(SkillSlotSet.Active1_1, 5, 1021205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1021301).SetActive(SkillSlotSet.Active2_1, 2, 1021302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1021303).SetActive(SkillSlotSet.Active2_1, 4, 1021304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1021305).SetActive(SkillSlotSet.Active3_1, 1, 1021401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1021402).SetActive(SkillSlotSet.Active3_1, 3, 1021403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1021404).SetActive(SkillSlotSet.Active3_1, 5, 1021405)
				.SetActive(SkillSlotSet.Active3_1, 1, 1021411).SetActive(SkillSlotSet.Active3_1, 2, 1021412)
				.SetActive(SkillSlotSet.Active3_1, 3, 1021413).SetActive(SkillSlotSet.Active3_1, 4, 1021414)
				.SetActive(SkillSlotSet.Active3_1, 5, 1021415).SetActive(SkillSlotSet.Active4_1, 1, 1021501)
				.SetActive(SkillSlotSet.Active4_1, 2, 1021502).SetActive(SkillSlotSet.Active4_1, 3, 1021503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(22).SetActive(SkillSlotSet.Attack_1, 1, 1022001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1022101).SetActive(SkillSlotSet.Passive_1, 2, 1022102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1022103).SetActive(SkillSlotSet.Active1_1, 1, 1022201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1022202).SetActive(SkillSlotSet.Active1_1, 3, 1022203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1022204).SetActive(SkillSlotSet.Active1_1, 5, 1022205)
				.SetActive(SkillSlotSet.Active1_2, 1, 1022211).SetActive(SkillSlotSet.Active1_2, 2, 1022212)
				.SetActive(SkillSlotSet.Active1_2, 3, 1022213).SetActive(SkillSlotSet.Active1_2, 4, 1022214)
				.SetActive(SkillSlotSet.Active1_2, 5, 1022215).SetActive(SkillSlotSet.Active2_1, 1, 1022301)
				.SetActive(SkillSlotSet.Active2_1, 2, 1022302).SetActive(SkillSlotSet.Active2_1, 3, 1022303)
				.SetActive(SkillSlotSet.Active2_1, 4, 1022304).SetActive(SkillSlotSet.Active2_1, 5, 1022305)
				.SetActive(SkillSlotSet.Active3_1, 1, 1022401).SetActive(SkillSlotSet.Active3_1, 2, 1022402)
				.SetActive(SkillSlotSet.Active3_1, 3, 1022403).SetActive(SkillSlotSet.Active3_1, 4, 1022404)
				.SetActive(SkillSlotSet.Active3_1, 5, 1022405).SetActive(SkillSlotSet.Active4_1, 1, 1022501)
				.SetActive(SkillSlotSet.Active4_1, 2, 1022502).SetActive(SkillSlotSet.Active4_1, 3, 1022503).Build());
			AddCharacterSkillSet(CharacterSkillSetData.Builder.Create(24).SetActive(SkillSlotSet.Attack_1, 1, 1024001)
				.SetActive(SkillSlotSet.Passive_1, 1, 1024101).SetActive(SkillSlotSet.Passive_1, 2, 1024102)
				.SetActive(SkillSlotSet.Passive_1, 3, 1024103).SetActive(SkillSlotSet.Active1_1, 1, 1024201)
				.SetActive(SkillSlotSet.Active1_1, 2, 1024202).SetActive(SkillSlotSet.Active1_1, 3, 1024203)
				.SetActive(SkillSlotSet.Active1_1, 4, 1024204).SetActive(SkillSlotSet.Active1_1, 5, 1024205)
				.SetActive(SkillSlotSet.Active2_1, 1, 1024301).SetActive(SkillSlotSet.Active2_1, 2, 1024302)
				.SetActive(SkillSlotSet.Active2_1, 3, 1024303).SetActive(SkillSlotSet.Active2_1, 4, 1024304)
				.SetActive(SkillSlotSet.Active2_1, 5, 1024305).SetActive(SkillSlotSet.Active3_1, 1, 1024401)
				.SetActive(SkillSlotSet.Active3_1, 2, 1024402).SetActive(SkillSlotSet.Active3_1, 3, 1024403)
				.SetActive(SkillSlotSet.Active3_1, 4, 1024404).SetActive(SkillSlotSet.Active3_1, 5, 1024405)
				.SetActive(SkillSlotSet.Active4_1, 1, 1024501).SetActive(SkillSlotSet.Active4_1, 2, 1024502)
				.SetActive(SkillSlotSet.Active4_1, 3, 1024503).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.Glove).SetActive(1, 3001001)
				.SetActive(2, 3001002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.Tonfa).SetActive(1, 3002001)
				.SetActive(2, 3002002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.Bat).SetActive(1, 3003001)
				.SetActive(2, 3003002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.Whip).SetActive(1, 3004001)
				.SetActive(2, 3004002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.HighAngleFire).SetActive(1, 3005001)
				.SetActive(2, 3005002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.DirectFire).SetActive(1, 3006001)
				.SetActive(2, 3006002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.Bow).SetActive(1, 3007001)
				.SetActive(2, 3007002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.CrossBow).SetActive(1, 3008001)
				.SetActive(2, 3008002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.Pistol).SetActive(1, 3009001)
				.SetActive(2, 3009002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.AssaultRifle).SetActive(1, 3010001)
				.SetActive(2, 3010002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.SniperRifle).SetActive(1, 3011001)
				.SetActive(2, 3011002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.Cannon).SetActive(1, 3012001)
				.SetActive(2, 3012002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.Hammer).SetActive(1, 3013001)
				.SetActive(2, 3013002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.Axe).SetActive(1, 3014001)
				.SetActive(2, 3014002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.OneHandSword).SetActive(1, 3015001)
				.SetActive(2, 3015002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.TwoHandSword).SetActive(1, 3016001)
				.SetActive(2, 3016002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.Polearm).SetActive(1, 3017001)
				.SetActive(2, 3017002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.DualSword).SetActive(1, 3018001)
				.SetActive(2, 3018002).SetActive(1, 3018011).SetActive(2, 3018012).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.Spear).SetActive(1, 3019001)
				.SetActive(2, 3019002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.Nunchaku).SetActive(1, 3020001)
				.SetActive(2, 3020002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.Rapier).SetActive(1, 3021001)
				.SetActive(2, 3021002).Build());
			AddWeaponSkillSet(WeaponSkillSetData.Builder.Create(MasteryType.Guitar).SetActive(1, 3022001)
				.SetActive(2, 3022002).Build());
			AddMonsterSkillSet(CharacterSkillSetData.Builder.Create(1).SetActive(SkillSlotSet.Attack_1, 1, 2001101)
				.Build());
			AddMonsterSkillSet(CharacterSkillSetData.Builder.Create(2).SetActive(SkillSlotSet.Attack_1, 1, 2002101)
				.Build());
			AddMonsterSkillSet(CharacterSkillSetData.Builder.Create(3).SetActive(SkillSlotSet.Attack_1, 1, 2003101)
				.SetActive(SkillSlotSet.Active1_1, 1, 2003201).Build());
			AddMonsterSkillSet(CharacterSkillSetData.Builder.Create(4).SetActive(SkillSlotSet.Attack_1, 1, 2004101)
				.Build());
			AddMonsterSkillSet(CharacterSkillSetData.Builder.Create(5).SetActive(SkillSlotSet.Attack_1, 1, 2005101)
				.SetActive(SkillSlotSet.Active1_1, 1, 2005201).Build());
			AddMonsterSkillSet(CharacterSkillSetData.Builder.Create(6).SetActive(SkillSlotSet.Attack_1, 1, 2006101)
				.SetActive(SkillSlotSet.Active1_1, 1, 2006201).Build());
			AddMonsterSkillSet(CharacterSkillSetData.Builder.Create(7).SetActive(SkillSlotSet.Attack_1, 1, 2007101)
				.SetActive(SkillSlotSet.Passive_1, 1, 2007201).SetActive(SkillSlotSet.Active1_1, 1, 2007301)
				.SetActive(SkillSlotSet.Active2_1, 1, 2007401).Build());
			AddSpecialSkillSet(
				SpecialSkillSetData.Builder.Create(SpecialSkillId.QuantumJump).SetActive(4001001).Build());
		}


		public void SetData<T>(List<T> data)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(SkillGroupData))
			{
				skillGroupMap.Clear();
				using (List<SkillGroupData>.Enumerator enumerator =
					data.Cast<SkillGroupData>().ToList<SkillGroupData>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SkillGroupData skillGroupData = enumerator.Current;
						skillGroupMap.Add(skillGroupData.group, skillGroupData);
					}

					return;
				}
			}

			if (typeFromHandle == typeof(SkillData))
			{
				skillMap.Clear();
				using (List<SkillData>.Enumerator enumerator2 =
					data.Cast<SkillData>().ToList<SkillData>().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SkillData skillData = enumerator2.Current;
						skillMap.Add(skillData.code, skillData);
					}

					return;
				}
			}

			if (typeFromHandle == typeof(SkillEvolutionData))
			{
				skillEvolutionMap.Clear();
				using (List<SkillEvolutionData>.Enumerator enumerator3 =
					data.Cast<SkillEvolutionData>().ToList<SkillEvolutionData>().GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						SkillEvolutionData skillEvolutionData = enumerator3.Current;
						skillEvolutionMap.Add(skillEvolutionData.skillGroup, skillEvolutionData);
					}

					return;
				}
			}

			if (typeFromHandle == typeof(SkillEvolutionPointData))
			{
				skillEvolutionPointMap.Clear();
				foreach (SkillEvolutionPointData skillEvolutionPointData in data.Cast<SkillEvolutionPointData>()
					.ToList<SkillEvolutionPointData>())
				{
					if (!skillEvolutionPointMap.ContainsKey(skillEvolutionPointData.characterCode))
					{
						skillEvolutionPointMap.Add(skillEvolutionPointData.characterCode,
							new List<SkillEvolutionPointData>());
					}

					SkillEvolutionConditionType conditionType = skillEvolutionPointData.conditionType;
					SkillEvolutionPointData item;
					if (conditionType != SkillEvolutionConditionType.WEAPON_CRAFT)
					{
						if (conditionType != SkillEvolutionConditionType.STATE_STACK)
						{
							throw new ArgumentOutOfRangeException();
						}

						item = new StateStackSkillEvolutionPointData(skillEvolutionPointData);
					}
					else
					{
						item = new WeaponCraftSkillEvolutionPointData(skillEvolutionPointData);
					}

					skillEvolutionPointMap[skillEvolutionPointData.characterCode].Add(item);
				}
			}
		}


		private void AddCharacterSkillSet(CharacterSkillSetData data)
		{
			if (characterSkillSetMap.ContainsKey(data.characterCode))
			{
				throw new Exception();
			}

			characterSkillSetMap.Add(data.characterCode, data);
		}


		private void AddMonsterSkillSet(CharacterSkillSetData data)
		{
			if (monsterSkillSetMap.ContainsKey(data.characterCode))
			{
				throw new Exception();
			}

			monsterSkillSetMap.Add(data.characterCode, data);
		}


		private void AddWeaponSkillSet(WeaponSkillSetData data)
		{
			if (weaponSkillSetMap.ContainsKey(data.masteryType))
			{
				throw new Exception();
			}

			weaponSkillSetMap.Add(data.masteryType, data);
		}


		private void AddSpecialSkillSet(SpecialSkillSetData data)
		{
			if (specialSkillSetMap.ContainsKey(data.specialSkillId))
			{
				throw new Exception();
			}

			specialSkillSetMap.Add(data.specialSkillId, data);
		}


		public Dictionary<SkillSlotIndex, SkillSlotSet> GetDefaultSkillSet(int characterCode, ObjectType objectType)
		{
			if (objectType == ObjectType.PlayerCharacter || objectType == ObjectType.BotPlayerCharacter)
			{
				if (!characterSkillSetMap.ContainsKey(characterCode))
				{
					return null;
				}

				return new Dictionary<SkillSlotIndex, SkillSlotSet>(characterSkillSetMap[characterCode]
					.defaultSkillSet);
			}

			if (objectType != ObjectType.Monster)
			{
				return null;
			}

			if (!monsterSkillSetMap.ContainsKey(characterCode))
			{
				return null;
			}

			return new Dictionary<SkillSlotIndex, SkillSlotSet>(monsterSkillSetMap[characterCode].defaultSkillSet);
		}


		public Dictionary<SkillSlotSet, SkillSet>.KeyCollection GetAllSkillSetKey(int characterCode,
			ObjectType objectType)
		{
			if (objectType == ObjectType.PlayerCharacter || objectType == ObjectType.BotPlayerCharacter)
			{
				if (!characterSkillSetMap.ContainsKey(characterCode))
				{
					return null;
				}

				return characterSkillSetMap[characterCode].skillCodeMap.Keys;
			}

			if (objectType != ObjectType.Monster)
			{
				return null;
			}

			if (!monsterSkillSetMap.ContainsKey(characterCode))
			{
				return null;
			}

			return monsterSkillSetMap[characterCode].skillCodeMap.Keys;
		}


		public SkillSet GetSkillSetData(int characterCode, ObjectType objectType, SkillSlotSet slotSet)
		{
			if (objectType == ObjectType.PlayerCharacter || objectType == ObjectType.BotPlayerCharacter)
			{
				if (!characterSkillSetMap.ContainsKey(characterCode))
				{
					return null;
				}

				CharacterSkillSetData characterSkillSetData = characterSkillSetMap[characterCode];
				if (characterSkillSetData.skillCodeMap.ContainsKey(slotSet))
				{
					return characterSkillSetData.skillCodeMap[slotSet];
				}
			}
			else if (objectType == ObjectType.Monster)
			{
				if (!monsterSkillSetMap.ContainsKey(characterCode))
				{
					return null;
				}

				CharacterSkillSetData characterSkillSetData2 = monsterSkillSetMap[characterCode];
				if (characterSkillSetData2.skillCodeMap.ContainsKey(slotSet))
				{
					return characterSkillSetData2.skillCodeMap[slotSet];
				}
			}

			return null;
		}


		public SkillSet GetSkillSetData(MasteryType masteryType)
		{
			if (!weaponSkillSetMap.ContainsKey(masteryType))
			{
				return null;
			}

			return weaponSkillSetMap[masteryType].skillSet;
		}


		public SkillSet GetSkillSetData(SpecialSkillId specialSkillId)
		{
			if (!specialSkillSetMap.ContainsKey(specialSkillId))
			{
				return null;
			}

			return specialSkillSetMap[specialSkillId].skillSet;
		}


		public List<SkillData> GetCharacterSkills(int characterCode, ObjectType objectType, SkillSlotSet slotSet,
			int level)
		{
			findSkillDatas.Clear();
			SkillSet skillSetData = GetSkillSetData(characterCode, objectType, slotSet);
			if (skillSetData == null)
			{
				return findSkillDatas;
			}

			List<int> skillsByLevel = skillSetData.GetSkillsByLevel(level);
			for (int i = 0; i < skillsByLevel.Count; i++)
			{
				SkillData item;
				if (skillMap.TryGetValue(skillsByLevel[i], out item))
				{
					findSkillDatas.Add(item);
				}
			}

			return findSkillDatas;
		}


		public SkillData GetSkillData(int skillDataCode)
		{
			SkillData result;
			if (!skillMap.TryGetValue(skillDataCode, out result))
			{
				return null;
			}

			return result;
		}


		public SkillData GetSkillData(int characterCode, ObjectType objectType, SkillSlotSet skillSlotSet, int level,
			int sequence)
		{
			SkillSet skillSetData = GetSkillSetData(characterCode, objectType, skillSlotSet);
			if (skillSetData == null)
			{
				return null;
			}

			SkillData result;
			skillMap.TryGetValue(skillSetData.GetSkill(level, sequence), out result);
			return result;
		}


		public List<SkillData> GetWeaponSkills(MasteryType masteryType, int level)
		{
			findSkillDatas.Clear();
			SkillSet skillSetData = GetSkillSetData(masteryType);
			if (skillSetData == null)
			{
				return findSkillDatas;
			}

			List<int> skillsByLevel = skillSetData.GetSkillsByLevel(level);
			for (int i = 0; i < skillsByLevel.Count; i++)
			{
				SkillData item;
				if (skillMap.TryGetValue(skillsByLevel[i], out item))
				{
					findSkillDatas.Add(item);
				}
			}

			return findSkillDatas;
		}


		public SkillData GetSkillData(MasteryType masteryType, int level, int sequence)
		{
			SkillSet skillSetData = GetSkillSetData(masteryType);
			if (skillSetData == null)
			{
				return null;
			}

			SkillData result;
			if (!skillMap.TryGetValue(skillSetData.GetSkill(level, sequence), out result))
			{
				return null;
			}

			return result;
		}


		public SkillData GetSkillData(SpecialSkillId specialSkillId, int sequence)
		{
			SkillSet skillSetData = GetSkillSetData(specialSkillId);
			if (skillSetData == null)
			{
				return null;
			}

			SkillData result;
			if (!skillMap.TryGetValue(skillSetData.GetSkill(1, sequence), out result))
			{
				return null;
			}

			return result;
		}


		public SkillGroupData GetSkillGroupData(int group)
		{
			SkillGroupData result;
			if (!skillGroupMap.TryGetValue(group, out result))
			{
				return null;
			}

			return result;
		}


		public SkillGroupData GetSkillGroupDataByActiveSkillId(SkillId skillId)
		{
			foreach (KeyValuePair<int, SkillGroupData> keyValuePair in skillGroupMap)
			{
				if (keyValuePair.Value.skillId == skillId)
				{
					return keyValuePair.Value;
				}
			}

			return null;
		}


		public Sprite GetSkillIcon(string skillIcon)
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.GetSkillsSprite(skillIcon);
		}


		public SkillSlotSet? FindSkillSlotSet(int characterCode, ObjectType objectType, int skillCode)
		{
			if (objectType == ObjectType.PlayerCharacter || objectType == ObjectType.BotPlayerCharacter)
			{
				if (!characterSkillSetMap.ContainsKey(characterCode))
				{
					return null;
				}

				using (Dictionary<SkillSlotSet, SkillSet>.Enumerator enumerator =
					characterSkillSetMap[characterCode].skillCodeMap.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<SkillSlotSet, SkillSet> keyValuePair = enumerator.Current;
						if (keyValuePair.Value.Any(skillCode))
						{
							return keyValuePair.Key;
						}
					}

					goto IL_F4;
				}
			}

			if (objectType == ObjectType.Monster)
			{
				if (!monsterSkillSetMap.ContainsKey(characterCode))
				{
					return null;
				}

				foreach (KeyValuePair<SkillSlotSet, SkillSet> keyValuePair2 in monsterSkillSetMap[characterCode]
					.skillCodeMap)
				{
					if (keyValuePair2.Value.Any(skillCode))
					{
						return keyValuePair2.Key;
					}
				}
			}

			IL_F4:
			return null;
		}


		public bool FindSkillSlotAndMastery(int characterCode, int skillCode, ObjectType objectType,
			ref SkillSlotSet skillSlotSet, ref MasteryType masteryType)
		{
			if (objectType == ObjectType.PlayerCharacter || objectType == ObjectType.BotPlayerCharacter)
			{
				if (!characterSkillSetMap.ContainsKey(characterCode))
				{
					return false;
				}

				foreach (KeyValuePair<SkillSlotSet, SkillSet> keyValuePair in characterSkillSetMap[characterCode]
					.skillCodeMap)
				{
					if (keyValuePair.Value.Any(skillCode))
					{
						skillSlotSet = keyValuePair.Key;
						masteryType = MasteryType.None;
						return true;
					}
				}

				using (Dictionary<MasteryType, WeaponSkillSetData>.ValueCollection.Enumerator enumerator2 =
					weaponSkillSetMap.Values.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						WeaponSkillSetData weaponSkillSetData = enumerator2.Current;
						if (weaponSkillSetData.skillSet.Any(skillCode))
						{
							skillSlotSet = SkillSlotSet.WeaponSkill;
							masteryType = weaponSkillSetData.masteryType;
							return true;
						}
					}

					return false;
				}
			}

			if (objectType == ObjectType.Monster)
			{
				if (!monsterSkillSetMap.ContainsKey(characterCode))
				{
					return false;
				}

				foreach (KeyValuePair<SkillSlotSet, SkillSet> keyValuePair2 in monsterSkillSetMap[characterCode]
					.skillCodeMap)
				{
					if (keyValuePair2.Value.Any(skillCode))
					{
						skillSlotSet = keyValuePair2.Key;
						masteryType = MasteryType.None;
						return true;
					}
				}
			}

			return false;
		}


		public int GetSkillSequence(int characterCode, ObjectType objectType, int skillCode)
		{
			SkillSlotSet? skillSlotSet = FindSkillSlotSet(characterCode, objectType, skillCode);
			if (skillSlotSet == null)
			{
				return -1;
			}

			return characterSkillSetMap[characterCode].skillCodeMap[skillSlotSet.Value].GetSequence(skillCode);
		}


		public int GetSkillMaxEvolutionLevel(int skillGroup)
		{
			if (skillEvolutionMap.ContainsKey(skillGroup))
			{
				return skillEvolutionMap[skillGroup].maxEvolutionLevel;
			}

			return 0;
		}


		public SkillEvolutionData GetEvolutionData(SkillData skillData)
		{
			if (skillEvolutionMap.ContainsKey(skillData.group))
			{
				return skillEvolutionMap[skillData.group];
			}

			return null;
		}


		public bool TryGetEvolutionPointData<T>(int characterCode, ref List<T> dataList)
			where T : SkillEvolutionPointData
		{
			if (skillEvolutionPointMap.ContainsKey(characterCode))
			{
				foreach (SkillEvolutionPointData skillEvolutionPointData in skillEvolutionPointMap[characterCode])
				{
					if (skillEvolutionPointData is T)
					{
						dataList.Add((T) skillEvolutionPointData);
					}
				}

				return true;
			}

			return false;
		}
	}
}