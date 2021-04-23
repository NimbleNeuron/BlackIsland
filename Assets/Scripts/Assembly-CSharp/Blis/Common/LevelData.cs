using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blis.Common
{
	public class LevelData
	{
		private readonly Dictionary<int, List<CharacterSpawnPoint>> areaCodeCharacterSpawnPoints =
			new Dictionary<int, List<CharacterSpawnPoint>>();


		public readonly Dictionary<int, AreaData> areaDataMap;


		public readonly Dictionary<int, AreaData> areaMaskCodeMap;


		public readonly List<CharacterSpawnPoint> characterSpawnPoints;


		public readonly List<CollectibleData> collectibleDataList;


		public readonly List<HyperloopSpawnPoint> hyperloopSpawnPoints;


		public readonly List<ItemSpawnPoint> itemSpawnPoints;


		public readonly List<ItemSpawnData> itemSpawns;


		public readonly List<MonsterSpawnPoint> monsterSpawnPoints;


		public readonly Dictionary<int, List<int>> nearByAreaMap;


		public readonly List<SecurityCameraSpawnPoint> securityCameraSpawnPoints;


		public readonly List<SecurityConsoleSpawnPoint> securityConsoleSpawnPoints;


		private AreaData laboratoryArea;


		public LevelData()
		{
			areaDataMap = new Dictionary<int, AreaData>();
			areaMaskCodeMap = new Dictionary<int, AreaData>();
			itemSpawns = new List<ItemSpawnData>();
			LoadDummy();
			LoadDummySpawnData();
		}


		public LevelData(List<AreaData> areas, List<ItemSpawnData> itemSpawns,
			List<CollectibleData> collectibleDataList, List<NearByAreaData> nearByAreaDatas,
			List<CharacterSpawnPoint> characterSpawnPoints, List<ItemSpawnPoint> itemSpawnPoints,
			List<MonsterSpawnPoint> monsterSpawnPoints, List<HyperloopSpawnPoint> hyperloopSpawnPoints,
			List<SecurityConsoleSpawnPoint> consoleSpawnPoints, List<SecurityCameraSpawnPoint> cameraSpawnPoints)
		{
			areaDataMap = new Dictionary<int, AreaData>();
			areaMaskCodeMap = new Dictionary<int, AreaData>();
			nearByAreaMap = new Dictionary<int, List<int>>();
			using (List<AreaData>.Enumerator enumerator = areas.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AreaData areaData = enumerator.Current;
					areaDataMap.Add(areaData.code, areaData);
					areaMaskCodeMap.Add(areaData.maskCode, areaData);
					nearByAreaMap.Add(areaData.code, (from x in nearByAreaDatas
						where x.areaCode == areaData.code
						select x.nearByAreaCode).ToList<int>());
				}
			}

			areaDataMap.TryGetValue(16, out laboratoryArea);
			for (int i = 0; i < characterSpawnPoints.Count; i++)
			{
				if (!characterSpawnPoints[i].UseCharacterSpawn)
				{
					characterSpawnPoints.RemoveAt(i--);
				}
			}

			this.characterSpawnPoints = characterSpawnPoints;
			this.itemSpawns = itemSpawns;
			this.collectibleDataList = collectibleDataList;
			this.itemSpawnPoints = itemSpawnPoints;
			this.monsterSpawnPoints = monsterSpawnPoints;
			this.hyperloopSpawnPoints = hyperloopSpawnPoints;
			securityConsoleSpawnPoints = consoleSpawnPoints;
			securityCameraSpawnPoints = cameraSpawnPoints;
			areaCodeCharacterSpawnPoints.Clear();
			foreach (CharacterSpawnPoint characterSpawnPoint in this.characterSpawnPoints)
			{
				List<CharacterSpawnPoint> list;
				if (areaCodeCharacterSpawnPoints.TryGetValue(characterSpawnPoint.AreaCode, out list))
				{
					areaCodeCharacterSpawnPoints[characterSpawnPoint.AreaCode].Add(characterSpawnPoint);
				}
				else
				{
					areaCodeCharacterSpawnPoints.Add(characterSpawnPoint.AreaCode, new List<CharacterSpawnPoint>
					{
						characterSpawnPoint
					});
				}
			}
		}


		public AreaData LaboratoryArea => laboratoryArea;


		public List<CharacterSpawnPoint> GetSpawnPointsByAreaCode(int areaCode)
		{
			List<CharacterSpawnPoint> result = null;
			if (!areaCodeCharacterSpawnPoints.TryGetValue(areaCode, out result))
			{
				result = new List<CharacterSpawnPoint>();
			}

			return result;
		}


		public CollectibleData GetCollectibleData(int code)
		{
			return collectibleDataList.Find(data => data.code == code);
		}


		public ItemSpawnPoint GetItemSpawnPointByCode(int code)
		{
			return itemSpawnPoints.FirstOrDefault(x => x.code == code);
		}


		public List<Vector3> GetResourcePositionList(ItemData itemData)
		{
			List<Vector3> list = new List<Vector3>();
			foreach (ItemSpawnPoint itemSpawnPoint in itemSpawnPoints)
			{
				if (itemSpawnPoint.resource)
				{
					CollectibleData collectibleData = GetCollectibleData(itemSpawnPoint.resourceDataCode);
					if (collectibleData != null && collectibleData.itemCode == itemData.code)
					{
						list.Add(itemSpawnPoint.transform.position);
					}
				}
			}

			return list;
		}


		public List<Vector3> GetMonsterPositionList(int monsterCode)
		{
			List<Vector3> list = new List<Vector3>();
			foreach (MonsterSpawnPoint monsterSpawnPoint in monsterSpawnPoints)
			{
				if (monsterSpawnPoint.monsterCode == monsterCode)
				{
					list.Add(monsterSpawnPoint.transform.position);
				}
			}

			return list;
		}


		private void AddSpawnData(int code, DefaultMapAreaType areaCode, int itemCode, int initialCount, int dropCount)
		{
			itemSpawns.Add(new ItemSpawnData(code, (int) areaCode, 0, itemCode, DropPoint.Fixed, dropCount));
		}


		private void AddRandomData(int code, DefaultMapAreaType areaCode, int itemCode, int initialCount, int dropCount)
		{
			itemSpawns.Add(new ItemSpawnData(code, (int) areaCode, 0, itemCode, DropPoint.RandomBox, dropCount));
		}


		private void AddAreaData(int code, string name, int maskCode)
		{
			AreaData value = new AreaData(code, name, maskCode);
			areaDataMap.Add(code, value);
			areaMaskCodeMap.Add(maskCode, value);
		}


		private void LoadDummy()
		{
			AddAreaData(1, "항구", 1024);
			AddAreaData(2, "연못", 2048);
			AddAreaData(3, "모래사장", 4096);
			AddAreaData(4, "고급주택가", 8192);
			AddAreaData(5, "골목길", 16384);
			AddAreaData(6, "호텔", 32768);
			AddAreaData(7, "번화가", 65536);
			AddAreaData(8, "병원", 131072);
			AddAreaData(9, "절", 262144);
			AddAreaData(10, "양궁장", 524288);
			AddAreaData(11, "묘지", 1048576);
			AddAreaData(12, "숲", 2097152);
			AddAreaData(13, "공장", 4194304);
			AddAreaData(14, "성당", 8388608);
			AddAreaData(15, "학교", 16777216);
			AddAreaData(16, "연구소", 33554432);
			laboratoryArea = areaDataMap[16];
		}


		private void LoadDummySpawnData()
		{
			AddSpawnData(1, DefaultMapAreaType.Uptown, 118, 1, 3);
			AddSpawnData(2, DefaultMapAreaType.Uptown, 180, 1, 2);
			AddSpawnData(3, DefaultMapAreaType.Uptown, 245, 1, 1);
			AddSpawnData(4, DefaultMapAreaType.Uptown, 3, 1, 3);
			AddSpawnData(5, DefaultMapAreaType.Uptown, 96, 1, 3);
			AddSpawnData(6, DefaultMapAreaType.Uptown, 395, 1, 3);
			AddSpawnData(7, DefaultMapAreaType.Uptown, 396, 1, 3);
			AddSpawnData(8, DefaultMapAreaType.Uptown, 116, 1, 5);
			AddSpawnData(9, DefaultMapAreaType.Uptown, 123, 5, 4);
			AddSpawnData(10, DefaultMapAreaType.Uptown, 181, 2, 3);
			AddSpawnData(11, DefaultMapAreaType.Uptown, 79, 1, 3);
			AddSpawnData(12, DefaultMapAreaType.Uptown, 126, 1, 5);
			AddSpawnData(13, DefaultMapAreaType.Uptown, 367, 1, 2);
			AddSpawnData(14, DefaultMapAreaType.Uptown, 397, 1, 4);
			AddSpawnData(15, DefaultMapAreaType.Uptown, 6, 1, 2);
			AddSpawnData(16, DefaultMapAreaType.Uptown, 381, 1, 1);
			AddSpawnData(17, DefaultMapAreaType.Uptown, 136, 1, 4);
			AddSpawnData(18, DefaultMapAreaType.Uptown, 137, 1, 3);
			AddSpawnData(19, DefaultMapAreaType.Uptown, 183, 1, 3);
			AddSpawnData(20, DefaultMapAreaType.Uptown, 5, 1, 3);
			AddSpawnData(21, DefaultMapAreaType.Uptown, 21, 1, 4);
			AddSpawnData(22, DefaultMapAreaType.Uptown, 462, 1, 5);
			AddSpawnData(23, DefaultMapAreaType.Uptown, 184, 1, 4);
			AddSpawnData(24, DefaultMapAreaType.Uptown, 95, 1, 4);
			AddSpawnData(25, DefaultMapAreaType.Alley, 396, 1, 5);
			AddSpawnData(26, DefaultMapAreaType.Alley, 118, 1, 3);
			AddSpawnData(27, DefaultMapAreaType.Alley, 208, 1, 2);
			AddSpawnData(28, DefaultMapAreaType.Alley, 210, 3, 4);
			AddSpawnData(29, DefaultMapAreaType.Alley, 4, 1, 3);
			AddSpawnData(30, DefaultMapAreaType.Alley, 124, 1, 4);
			AddSpawnData(31, DefaultMapAreaType.Alley, 351, 1, 7);
			AddSpawnData(32, DefaultMapAreaType.Alley, 102, 1, 3);
			AddSpawnData(33, DefaultMapAreaType.Alley, 79, 1, 3);
			AddSpawnData(34, DefaultMapAreaType.Alley, 394, 1, 4);
			AddSpawnData(35, DefaultMapAreaType.Alley, 380, 1, 4);
			AddSpawnData(36, DefaultMapAreaType.Alley, 60, 1, 4);
			AddSpawnData(37, DefaultMapAreaType.Alley, 129, 1, 4);
			AddSpawnData(38, DefaultMapAreaType.Alley, 367, 1, 2);
			AddSpawnData(39, DefaultMapAreaType.Alley, 397, 1, 3);
			AddSpawnData(40, DefaultMapAreaType.Alley, 217, 1, 5);
			AddSpawnData(41, DefaultMapAreaType.Alley, 6, 1, 3);
			AddSpawnData(42, DefaultMapAreaType.Alley, 381, 1, 2);
			AddSpawnData(43, DefaultMapAreaType.Alley, 462, 1, 3);
			AddSpawnData(44, DefaultMapAreaType.Alley, 179, 1, 4);
			AddSpawnData(45, DefaultMapAreaType.Alley, 25, 1, 3);
			AddSpawnData(46, DefaultMapAreaType.Alley, 126, 1, 3);
			AddSpawnData(47, DefaultMapAreaType.Factory, 208, 1, 8);
			AddSpawnData(48, DefaultMapAreaType.Factory, 182, 1, 3);
			AddSpawnData(49, DefaultMapAreaType.Factory, 21, 1, 4);
			AddSpawnData(50, DefaultMapAreaType.Factory, 379, 1, 3);
			AddSpawnData(51, DefaultMapAreaType.Factory, 79, 1, 3);
			AddSpawnData(52, DefaultMapAreaType.Factory, 211, 1, 5);
			AddSpawnData(53, DefaultMapAreaType.Factory, 378, 1, 3);
			AddSpawnData(54, DefaultMapAreaType.Factory, 102, 1, 4);
			AddSpawnData(55, DefaultMapAreaType.Factory, 351, 1, 4);
			AddSpawnData(56, DefaultMapAreaType.Factory, 210, 3, 5);
			AddSpawnData(57, DefaultMapAreaType.Factory, 115, 1, 5);
			AddSpawnData(58, DefaultMapAreaType.Factory, 216, 1, 3);
			AddSpawnData(59, DefaultMapAreaType.Factory, 215, 1, 5);
			AddSpawnData(60, DefaultMapAreaType.Factory, 315, 1, 4);
			AddSpawnData(61, DefaultMapAreaType.Factory, 60, 1, 4);
			AddSpawnData(62, DefaultMapAreaType.Factory, 213, 2, 5);
			AddSpawnData(63, DefaultMapAreaType.Factory, 4, 1, 3);
			AddSpawnData(64, DefaultMapAreaType.Factory, 221, 1, 5);
			AddSpawnData(65, DefaultMapAreaType.Factory, 118, 1, 3);
			AddSpawnData(66, DefaultMapAreaType.Factory, 224, 1, 5);
			AddSpawnData(67, DefaultMapAreaType.Factory, 220, 1, 3);
			AddSpawnData(68, DefaultMapAreaType.SandyBeach, 135, 1, 3);
			AddSpawnData(69, DefaultMapAreaType.SandyBeach, 206, 1, 4);
			AddSpawnData(70, DefaultMapAreaType.SandyBeach, 207, 1, 5);
			AddSpawnData(71, DefaultMapAreaType.SandyBeach, 212, 1, 3);
			AddSpawnData(72, DefaultMapAreaType.SandyBeach, 179, 3, 8);
			AddSpawnData(73, DefaultMapAreaType.SandyBeach, 61, 1, 4);
			AddSpawnData(74, DefaultMapAreaType.SandyBeach, 60, 1, 4);
			AddSpawnData(75, DefaultMapAreaType.SandyBeach, 130, 1, 2);
			AddSpawnData(76, DefaultMapAreaType.SandyBeach, 225, 1, 4);
			AddSpawnData(77, DefaultMapAreaType.SandyBeach, 254, 1, 4);
			AddSpawnData(78, DefaultMapAreaType.SandyBeach, 253, 2, 2);
			AddSpawnData(79, DefaultMapAreaType.SandyBeach, 25, 1, 5);
			AddSpawnData(80, DefaultMapAreaType.SandyBeach, 226, 1, 4);
			AddSpawnData(81, DefaultMapAreaType.SandyBeach, 275, 1, 5);
			AddSpawnData(82, DefaultMapAreaType.SandyBeach, 1, 20, 8);
			AddSpawnData(83, DefaultMapAreaType.SandyBeach, 122, 1, 4);
			AddSpawnData(84, DefaultMapAreaType.SandyBeach, 102, 1, 4);
			AddSpawnData(85, DefaultMapAreaType.SandyBeach, 405, 1, 4);
			AddSpawnData(86, DefaultMapAreaType.Cemetery, 422, 1, 5);
			AddSpawnData(87, DefaultMapAreaType.Cemetery, 183, 1, 3);
			AddSpawnData(88, DefaultMapAreaType.Cemetery, 124, 1, 4);
			AddSpawnData(89, DefaultMapAreaType.Cemetery, 226, 1, 4);
			AddSpawnData(90, DefaultMapAreaType.Cemetery, 121, 1, 5);
			AddSpawnData(91, DefaultMapAreaType.Cemetery, 133, 2, 3);
			AddSpawnData(92, DefaultMapAreaType.Cemetery, 463, 1, 4);
			AddSpawnData(93, DefaultMapAreaType.Cemetery, 26, 1, 4);
			AddSpawnData(94, DefaultMapAreaType.Cemetery, 179, 3, 4);
			AddSpawnData(95, DefaultMapAreaType.Cemetery, 283, 1, 6);
			AddSpawnData(96, DefaultMapAreaType.Cemetery, 209, 1, 4);
			AddSpawnData(97, DefaultMapAreaType.Cemetery, 420, 1, 5);
			AddSpawnData(98, DefaultMapAreaType.Cemetery, 116, 1, 4);
			AddSpawnData(99, DefaultMapAreaType.Cemetery, 284, 1, 5);
			AddSpawnData(100, DefaultMapAreaType.Cemetery, 377, 3, 4);
			AddSpawnData(101, DefaultMapAreaType.Cemetery, 119, 1, 5);
			AddSpawnData(102, DefaultMapAreaType.Cemetery, 58, 1, 4);
			AddSpawnData(103, DefaultMapAreaType.Cemetery, 449, 1, 4);
			AddSpawnData(104, DefaultMapAreaType.Cemetery, 95, 1, 2);
			AddSpawnData(105, DefaultMapAreaType.Cemetery, 223, 3, 5);
			AddSpawnData(106, DefaultMapAreaType.Cemetery, 2, 20, 1);
			AddSpawnData(107, DefaultMapAreaType.Cemetery, 128, 2, 4);
			AddSpawnData(108, DefaultMapAreaType.Downtown, 368, 1, 6);
			AddSpawnData(109, DefaultMapAreaType.Downtown, 382, 1, 3);
			AddSpawnData(110, DefaultMapAreaType.Downtown, 224, 1, 5);
			AddSpawnData(111, DefaultMapAreaType.Downtown, 134, 1, 4);
			AddSpawnData(112, DefaultMapAreaType.Downtown, 213, 1, 4);
			AddSpawnData(113, DefaultMapAreaType.Downtown, 115, 1, 5);
			AddSpawnData(114, DefaultMapAreaType.Downtown, 3, 1, 3);
			AddSpawnData(115, DefaultMapAreaType.Downtown, 225, 1, 4);
			AddSpawnData(116, DefaultMapAreaType.Downtown, 178, 1, 6);
			AddSpawnData(117, DefaultMapAreaType.Downtown, 182, 1, 5);
			AddSpawnData(118, DefaultMapAreaType.Downtown, 255, 1, 4);
			AddSpawnData(119, DefaultMapAreaType.Downtown, 217, 1, 5);
			AddSpawnData(120, DefaultMapAreaType.Downtown, 136, 1, 4);
			AddSpawnData(121, DefaultMapAreaType.Downtown, 78, 1, 3);
			AddSpawnData(122, DefaultMapAreaType.Downtown, 58, 1, 4);
			AddSpawnData(123, DefaultMapAreaType.Downtown, 80, 1, 4);
			AddSpawnData(124, DefaultMapAreaType.Downtown, 381, 1, 3);
			AddSpawnData(125, DefaultMapAreaType.Downtown, 2, 20, 5);
			AddSpawnData(126, DefaultMapAreaType.Downtown, 327, 52, 3);
			AddSpawnData(127, DefaultMapAreaType.Downtown, 216, 1, 5);
			AddSpawnData(128, DefaultMapAreaType.Downtown, 98, 1, 4);
			AddSpawnData(129, DefaultMapAreaType.Hospital, 123, 5, 4);
			AddSpawnData(130, DefaultMapAreaType.Hospital, 394, 1, 5);
			AddSpawnData(131, DefaultMapAreaType.Hospital, 78, 1, 4);
			AddSpawnData(132, DefaultMapAreaType.Hospital, 211, 1, 5);
			AddSpawnData(133, DefaultMapAreaType.Hospital, 396, 1, 4);
			AddSpawnData(134, DefaultMapAreaType.Hospital, 5, 1, 3);
			AddSpawnData(135, DefaultMapAreaType.Hospital, 180, 1, 4);
			AddSpawnData(136, DefaultMapAreaType.Hospital, 80, 1, 4);
			AddSpawnData(137, DefaultMapAreaType.Hospital, 123, 5, 4);
			AddSpawnData(138, DefaultMapAreaType.Hospital, 171, 1, 5);
			AddSpawnData(139, DefaultMapAreaType.Hospital, 172, 2, 5);
			AddSpawnData(140, DefaultMapAreaType.Hospital, 215, 1, 5);
			AddSpawnData(141, DefaultMapAreaType.Hospital, 174, 1, 5);
			AddSpawnData(142, DefaultMapAreaType.Hospital, 173, 1, 4);
			AddSpawnData(143, DefaultMapAreaType.Hospital, 395, 1, 2);
			AddSpawnData(144, DefaultMapAreaType.Hospital, 24, 1, 8);
			AddSpawnData(145, DefaultMapAreaType.Hospital, 221, 1, 5);
			AddSpawnData(146, DefaultMapAreaType.Hospital, 368, 1, 4);
			AddSpawnData(147, DefaultMapAreaType.Hospital, 423, 1, 3);
			AddSpawnData(148, DefaultMapAreaType.Church, 126, 1, 4);
			AddSpawnData(149, DefaultMapAreaType.Church, 327, 52, 3);
			AddSpawnData(150, DefaultMapAreaType.Church, 328, 1, 3);
			AddSpawnData(151, DefaultMapAreaType.Church, 134, 3, 4);
			AddSpawnData(152, DefaultMapAreaType.Church, 98, 1, 3);
			AddSpawnData(153, DefaultMapAreaType.Church, 222, 1, 4);
			AddSpawnData(154, DefaultMapAreaType.Church, 125, 1, 3);
			AddSpawnData(155, DefaultMapAreaType.Church, 380, 1, 4);
			AddSpawnData(156, DefaultMapAreaType.Church, 179, 1, 4);
			AddSpawnData(157, DefaultMapAreaType.Church, 296, 1, 4);
			AddSpawnData(158, DefaultMapAreaType.Church, 22, 1, 8);
			AddSpawnData(159, DefaultMapAreaType.Church, 315, 1, 3);
			AddSpawnData(160, DefaultMapAreaType.Church, 253, 2, 3);
			AddSpawnData(161, DefaultMapAreaType.Church, 101, 1, 8);
			AddSpawnData(162, DefaultMapAreaType.Church, 449, 1, 4);
			AddSpawnData(163, DefaultMapAreaType.Church, 100, 1, 5);
			AddSpawnData(164, DefaultMapAreaType.Church, 275, 1, 4);
			AddSpawnData(165, DefaultMapAreaType.Church, 421, 1, 4);
			AddSpawnData(166, DefaultMapAreaType.Church, 255, 1, 5);
			AddSpawnData(167, DefaultMapAreaType.Church, 61, 1, 4);
			AddSpawnData(168, DefaultMapAreaType.Forest, 422, 1, 6);
			AddSpawnData(169, DefaultMapAreaType.Forest, 128, 2, 4);
			AddSpawnData(170, DefaultMapAreaType.Forest, 219, 1, 4);
			AddSpawnData(171, DefaultMapAreaType.Forest, 205, 1, 6);
			AddSpawnData(172, DefaultMapAreaType.Forest, 283, 1, 5);
			AddSpawnData(173, DefaultMapAreaType.Forest, 209, 1, 3);
			AddSpawnData(174, DefaultMapAreaType.Forest, 98, 1, 5);
			AddSpawnData(175, DefaultMapAreaType.Forest, 80, 1, 3);
			AddSpawnData(176, DefaultMapAreaType.Forest, 440, 1, 4);
			AddSpawnData(177, DefaultMapAreaType.Forest, 253, 2, 2);
			AddSpawnData(178, DefaultMapAreaType.Forest, 179, 3, 7);
			AddSpawnData(179, DefaultMapAreaType.Forest, 420, 1, 5);
			AddSpawnData(180, DefaultMapAreaType.Forest, 132, 1, 7);
			AddSpawnData(181, DefaultMapAreaType.Forest, 284, 1, 3);
			AddSpawnData(182, DefaultMapAreaType.Forest, 285, 1, 4);
			AddSpawnData(183, DefaultMapAreaType.Forest, 223, 3, 4);
			AddSpawnData(184, DefaultMapAreaType.Forest, 95, 1, 3);
			AddSpawnData(185, DefaultMapAreaType.Forest, 310, 1, 4);
			AddSpawnData(186, DefaultMapAreaType.Archery, 216, 1, 3);
			AddSpawnData(187, DefaultMapAreaType.Archery, 212, 1, 3);
			AddSpawnData(188, DefaultMapAreaType.Archery, 133, 2, 3);
			AddSpawnData(189, DefaultMapAreaType.Archery, 420, 1, 3);
			AddSpawnData(190, DefaultMapAreaType.Archery, 224, 1, 5);
			AddSpawnData(191, DefaultMapAreaType.Archery, 137, 1, 3);
			AddSpawnData(192, DefaultMapAreaType.Archery, 205, 1, 3);
			AddSpawnData(193, DefaultMapAreaType.Archery, 127, 1, 5);
			AddSpawnData(194, DefaultMapAreaType.Archery, 26, 1, 4);
			AddSpawnData(195, DefaultMapAreaType.Archery, 128, 2, 4);
			AddSpawnData(196, DefaultMapAreaType.Archery, 115, 1, 5);
			AddSpawnData(197, DefaultMapAreaType.Archery, 222, 1, 4);
			AddSpawnData(198, DefaultMapAreaType.Archery, 296, 1, 5);
			AddSpawnData(199, DefaultMapAreaType.Archery, 209, 1, 3);
			AddSpawnData(200, DefaultMapAreaType.Archery, 310, 1, 3);
			AddSpawnData(201, DefaultMapAreaType.Archery, 59, 1, 3);
			AddSpawnData(202, DefaultMapAreaType.Archery, 182, 1, 5);
			AddSpawnData(203, DefaultMapAreaType.Archery, 253, 2, 4);
			AddSpawnData(204, DefaultMapAreaType.Archery, 379, 1, 3);
			AddSpawnData(205, DefaultMapAreaType.Archery, 121, 1, 4);
			AddSpawnData(206, DefaultMapAreaType.Archery, 2, 20, 10);
			AddSpawnData(207, DefaultMapAreaType.Archery, 226, 1, 4);
			AddSpawnData(208, DefaultMapAreaType.Pond, 59, 1, 4);
			AddSpawnData(209, DefaultMapAreaType.Pond, 127, 1, 5);
			AddSpawnData(210, DefaultMapAreaType.Pond, 421, 1, 4);
			AddSpawnData(211, DefaultMapAreaType.Pond, 206, 1, 5);
			AddSpawnData(212, DefaultMapAreaType.Pond, 382, 1, 5);
			AddSpawnData(213, DefaultMapAreaType.Pond, 132, 1, 5);
			AddSpawnData(214, DefaultMapAreaType.Pond, 179, 3, 17);
			AddSpawnData(215, DefaultMapAreaType.Pond, 96, 1, 3);
			AddSpawnData(216, DefaultMapAreaType.Pond, 253, 2, 4);
			AddSpawnData(217, DefaultMapAreaType.Pond, 95, 1, 4);
			AddSpawnData(218, DefaultMapAreaType.Pond, 99, 1, 4);
			AddSpawnData(219, DefaultMapAreaType.Pond, 254, 1, 6);
			AddSpawnData(220, DefaultMapAreaType.Pond, 125, 1, 6);
			AddSpawnData(221, DefaultMapAreaType.Pond, 219, 1, 4);
			AddSpawnData(222, DefaultMapAreaType.Pond, 6, 1, 4);
			AddSpawnData(223, DefaultMapAreaType.Temple, 379, 1, 4);
			AddSpawnData(224, DefaultMapAreaType.Temple, 132, 1, 4);
			AddSpawnData(225, DefaultMapAreaType.Temple, 119, 1, 5);
			AddSpawnData(226, DefaultMapAreaType.Temple, 223, 3, 4);
			AddSpawnData(227, DefaultMapAreaType.Temple, 205, 1, 4);
			AddSpawnData(228, DefaultMapAreaType.Temple, 214, 1, 5);
			AddSpawnData(229, DefaultMapAreaType.Temple, 440, 1, 6);
			AddSpawnData(230, DefaultMapAreaType.Temple, 378, 1, 3);
			AddSpawnData(231, DefaultMapAreaType.Temple, 377, 3, 5);
			AddSpawnData(232, DefaultMapAreaType.Temple, 121, 1, 4);
			AddSpawnData(233, DefaultMapAreaType.Temple, 222, 1, 4);
			AddSpawnData(234, DefaultMapAreaType.Temple, 350, 1, 4);
			AddSpawnData(235, DefaultMapAreaType.Temple, 58, 1, 2);
			AddSpawnData(236, DefaultMapAreaType.Temple, 23, 1, 8);
			AddSpawnData(237, DefaultMapAreaType.Temple, 219, 1, 5);
			AddSpawnData(238, DefaultMapAreaType.Temple, 217, 1, 4);
			AddSpawnData(239, DefaultMapAreaType.Temple, 97, 1, 6);
			AddSpawnData(240, DefaultMapAreaType.Temple, 209, 1, 3);
			AddSpawnData(241, DefaultMapAreaType.Temple, 26, 1, 4);
			AddSpawnData(242, DefaultMapAreaType.Temple, 212, 1, 3);
			AddSpawnData(243, DefaultMapAreaType.School, 218, 1, 1);
			AddSpawnData(244, DefaultMapAreaType.School, 328, 1, 3);
			AddSpawnData(245, DefaultMapAreaType.School, 225, 1, 4);
			AddSpawnData(246, DefaultMapAreaType.School, 207, 1, 3);
			AddSpawnData(247, DefaultMapAreaType.School, 129, 1, 4);
			AddSpawnData(248, DefaultMapAreaType.School, 423, 1, 2);
			AddSpawnData(249, DefaultMapAreaType.School, 395, 1, 4);
			AddSpawnData(250, DefaultMapAreaType.School, 211, 1, 3);
			AddSpawnData(251, DefaultMapAreaType.School, 394, 1, 5);
			AddSpawnData(252, DefaultMapAreaType.School, 99, 1, 3);
			AddSpawnData(253, DefaultMapAreaType.School, 59, 1, 4);
			AddSpawnData(254, DefaultMapAreaType.School, 126, 1, 5);
			AddSpawnData(255, DefaultMapAreaType.School, 210, 3, 6);
			AddSpawnData(256, DefaultMapAreaType.School, 96, 1, 2);
			AddSpawnData(257, DefaultMapAreaType.School, 326, 1, 6);
			AddSpawnData(258, DefaultMapAreaType.School, 21, 1, 6);
			AddSpawnData(259, DefaultMapAreaType.School, 78, 1, 4);
			AddSpawnData(260, DefaultMapAreaType.School, 285, 12, 3);
			AddSpawnData(261, DefaultMapAreaType.School, 61, 1, 4);
			AddSpawnData(262, DefaultMapAreaType.School, 215, 1, 5);
			AddSpawnData(263, DefaultMapAreaType.School, 4, 1, 4);
			AddSpawnData(264, DefaultMapAreaType.School, 378, 1, 4);
			AddSpawnData(265, DefaultMapAreaType.School, 5, 1, 3);
			AddSpawnData(266, DefaultMapAreaType.Harbor, 206, 1, 3);
			AddSpawnData(267, DefaultMapAreaType.Harbor, 253, 2, 3);
			AddSpawnData(268, DefaultMapAreaType.Harbor, 122, 1, 4);
			AddSpawnData(269, DefaultMapAreaType.Harbor, 127, 1, 4);
			AddSpawnData(270, DefaultMapAreaType.Harbor, 213, 1, 4);
			AddSpawnData(271, DefaultMapAreaType.Harbor, 25, 1, 4);
			AddSpawnData(272, DefaultMapAreaType.Harbor, 210, 3, 5);
			AddSpawnData(273, DefaultMapAreaType.Harbor, 179, 3, 6);
			AddSpawnData(274, DefaultMapAreaType.Harbor, 129, 1, 4);
			AddSpawnData(275, DefaultMapAreaType.Harbor, 99, 1, 3);
			AddSpawnData(276, DefaultMapAreaType.Harbor, 130, 1, 1);
			AddSpawnData(277, DefaultMapAreaType.Harbor, 422, 1, 5);
			AddSpawnData(278, DefaultMapAreaType.Harbor, 116, 1, 4);
			AddSpawnData(279, DefaultMapAreaType.Harbor, 350, 1, 2);
			AddSpawnData(280, DefaultMapAreaType.Harbor, 405, 1, 3);
			AddSpawnData(281, DefaultMapAreaType.Harbor, 135, 1, 4);
			AddSpawnData(282, DefaultMapAreaType.Harbor, 1, 20, 9);
			AddSpawnData(283, DefaultMapAreaType.Harbor, 368, 1, 5);
			AddSpawnData(284, DefaultMapAreaType.Harbor, 207, 1, 3);
			AddSpawnData(285, DefaultMapAreaType.Harbor, 208, 1, 6);
			AddSpawnData(286, DefaultMapAreaType.Hotel, 184, 1, 4);
			AddSpawnData(287, DefaultMapAreaType.Hotel, 134, 1, 4);
			AddSpawnData(288, DefaultMapAreaType.Hotel, 462, 1, 4);
			AddSpawnData(289, DefaultMapAreaType.Hotel, 135, 1, 4);
			AddSpawnData(290, DefaultMapAreaType.Hotel, 369, 1, 2);
			AddSpawnData(291, DefaultMapAreaType.Hotel, 119, 1, 4);
			AddSpawnData(292, DefaultMapAreaType.Hotel, 181, 1, 4);
			AddSpawnData(293, DefaultMapAreaType.Hotel, 24, 1, 3);
			AddSpawnData(294, DefaultMapAreaType.Hotel, 397, 1, 3);
			AddSpawnData(295, DefaultMapAreaType.Hotel, 3, 1, 3);
			AddSpawnData(296, DefaultMapAreaType.Hotel, 178, 1, 3);
			AddSpawnData(297, DefaultMapAreaType.Hotel, 124, 1, 4);
			AddSpawnData(298, DefaultMapAreaType.Hotel, 380, 1, 4);
			AddSpawnData(299, DefaultMapAreaType.Hotel, 463, 1, 4);
			AddSpawnData(300, DefaultMapAreaType.Hotel, 377, 3, 3);
			AddSpawnData(301, DefaultMapAreaType.Hotel, 116, 1, 5);
			AddSpawnData(302, DefaultMapAreaType.Hotel, 180, 1, 3);
			AddSpawnData(303, DefaultMapAreaType.Hotel, 133, 1, 4);
			AddSpawnData(304, DefaultMapAreaType.Hotel, 367, 1, 2);
			AddSpawnData(305, DefaultMapAreaType.Hotel, 326, 1, 4);
			AddSpawnData(306, DefaultMapAreaType.Hotel, 122, 1, 4);
			AddSpawnData(307, DefaultMapAreaType.Hotel, 255, 1, 6);
		}
	}
}