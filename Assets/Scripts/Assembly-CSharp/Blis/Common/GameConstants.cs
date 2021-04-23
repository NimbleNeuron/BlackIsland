using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Blis.Common
{
	public static class GameConstants
	{
		public static string Version => BSERVersion.VERSION;
		public const string PlayerPrefKey_KeySettingVersion = "KeySettingVersion";
		public const int keySettingVersion = 16;
		public const string PlayerPrefKey_AccessTermsVersion = "AccessTerms";
		public const int AccessTermsVersion = 2;
		public const float DefaultFrameUpdateRate = 0.033333335f;
		public const float FrameUpdateMargin = 0.001f;
		public const float DefaultServerUpdateRate = 0.016666668f;
		public const int MaxCommandListOffset = 3;
		public const string PlayerPrefKey_Nickname = "_dev_nickname";
		public const int MAX_TEAM_MEMBER_COUNT = 3;
		public const int MAX_MATCHING_USER_COUNT = 18;
		public const int MaxChatContentsLength = 100;
		public const float PlayerSetTargetWhenInToCombat = 40f;
		public const int WALKABLE_NAVMASK = 2147483640;
		public const float HANDLE_COMMAND_TIME_OUT_WARNING = 5f;
		public const float HANDLE_COMMAND_TIME_OUT = 10f;
		public const float NAV_STOPPING_DISTANCE = 0.5f;
		public const float MOVE_CORRECTION_DISTANCE = 1f;
		public const float ServerAngularPeriod = 0f;
		public const float LocalAngularPeriod = 0.19f;
		public const float LocalNormalAttackAngularPeriod = 0.1f;
		public const float SurvivableTime = 30f;
		public const int MAX_STACK_CONSUMABLE_ITEM = 3;
		public const int CONSUMABLE_ITEM_PERIOD = 15;
		public const string GAAP_VIP = "49.234.245.133";
		private const string SKILL_COOLDOWN_SOUND_FOLDER = "SkillCooldownSound/";
		public static readonly int ServerTargetFrame = Mathf.CeilToInt(59.999996f);
		public static readonly float StanceChangeCooldown = 0.5f;
		public static readonly int GameStandbyTime = 35;
		public static readonly int MatchingAcceptWaitTime = 10;
		public static readonly int MatchingCharacterSelectTime = 30;
		public static readonly int GameStartStandByTime = 5;
		public static readonly Vector3 CharacterOutSightPosition = new Vector3(-500f, -500f, 500f);
		public static readonly float MonsterWaitForAttackTargetTime = 5f;
		public static readonly int DefaultPort = 36947;
		public static float DefaultObjectY = 0f;
		public static float DEFAULT_WORLDOBJECT_COLLIDER_HEIGHT = 4f;
		public static double SERVER_IDLE_TIMEOUT = 20.0;
		public static int LobbyInitialCharacter = 1;
		public static readonly int AIR_SUPPLY_DROP_GROUP = 201;
		public static float USER_READY_TIMEOUT = 99999f;
		public static float AGENT_BOX_RADIUS = 0.65f;
		public static readonly Vector2 SCREEN_DRAG_MARGIN = new Vector2(100f, 100f);
		public static int STANDALONE_DEFAULT_CHARACTER = 4;
		public static int STANDALONE_DEFAULT_WEAPON = 104101;
		public static float IN_COMBAT_TIME = 5f;
		public static float MONSTER_SPAWN_WAIT_TIME = 1.5f;
		public static readonly Dictionary<SkillId, string> SKILL_COOLDOWN_SOUND =
			new Dictionary<SkillId, string>(SingletonComparerEnum<SkillIdComparer, SkillId>.Instance)
			{
				{
					SkillId.HyejinActive1,
					"SkillCooldownSound/Hyejin_Skill01_Charge"
				}
			};

		public static string GetDataCacheFilePath()
		{
#if UNITY_EDITOR
			return Path.Combine(Application.dataPath, "Cache").Replace("Assets/","");
#else
			return Path.Combine(Application.dataPath, "Cache");
#endif
		}


		public static string GetBuildName()
		{
			return "BUILD_RELEASE";
		}


		public static string GetDataName()
		{
			return "DATA_RELEASE";
		}


		public static string GetServerName()
		{
			return "SERVER_RELEASE";
		}


		public static string GetESIndexName()
		{
			return "game_play_release";
		}


		public static class Inventory
		{
			public const int MAX_INVENTORY_SPACE = 10;
		}


		public static class LayerNames
		{
			public const string IGNORE_RAYCAST = "Ignore Raycast";


			public const string UI = "UI";


			public const string GROUND = "Ground";


			public const string FX_OBJECT = "Fx";


			public const string WORLD_OBJECT = "WorldObject";


			public const string PICKABLE_OBJECT = "PickableObject";


			public const string FOG_OF_WAR = "FogOfWar";


			public const string BUSH = "Bush";


			public const string CAMERA_BOUND = "CameraBound";
		}


		public static class LayerNumber
		{
			public static readonly int IGNORE_RAYCAST = UnityEngine.LayerMask.NameToLayer("Ignore Raycast");


			public static readonly int UI = UnityEngine.LayerMask.NameToLayer("UI");


			public static readonly int GROUND = UnityEngine.LayerMask.NameToLayer("Ground");


			public static readonly int WORLD_OBJECT = UnityEngine.LayerMask.NameToLayer("WorldObject");


			public static readonly int PICKABLE_OBJECT = UnityEngine.LayerMask.NameToLayer("PickableObject");


			public static readonly int FOG_OF_WAR = UnityEngine.LayerMask.NameToLayer("FogOfWar");


			public static readonly int BUSH = UnityEngine.LayerMask.NameToLayer("Bush");

			public static bool IsObastacleLayer(int layer)
			{
				return layer == FOG_OF_WAR || layer == BUSH;
			}
		}


		public static class LayerMask
		{
			public static readonly int PICKABLE_LAYER = UnityEngine.LayerMask.GetMask("UI", "Ground", "PickableObject");


			public static readonly int PICKABLE_OBJECT_LAYER = UnityEngine.LayerMask.GetMask("PickableObject");


			public static readonly int WORLD_OBJECT_LAYER = UnityEngine.LayerMask.GetMask("WorldObject");


			public static readonly int GROUND_LAYER = UnityEngine.LayerMask.GetMask("Ground");


			public static readonly int FOG_LAYER = UnityEngine.LayerMask.GetMask("FogOfWar");


			public static readonly int SIGHT_COLLIDER_LAYER =
				UnityEngine.LayerMask.GetMask("PickableObject", "Fx", "FogOfWar", "Bush");


			public static readonly int SIGHT_OBSTACLE_LAYER = UnityEngine.LayerMask.GetMask("FogOfWar", "Bush");


			public static readonly int BUSH_LAYER = UnityEngine.LayerMask.GetMask("Bush");


			public static readonly int CAMERA_BOUND_LAYER = UnityEngine.LayerMask.GetMask("CameraBound");
		}


		public static class FieldObject
		{
			public const int SYSTEM_OWN = 10000;


			public const float DROP_ITEM_Y_POS = 0.5f;


			public const int DEFAULT_FIELD_BOX_ITEM_COUNT = 5;


			public const int DEFAULT_FIELD_DROP_ITEM_COUNT = 1;
		}


		public static class PrefabPath
		{
			public const string CONNETION = "Network/Connection";
		}


		public static class UIColor
		{
			public static readonly Color enableButtonTextColor = new Color(1f, 0.6f, 0.2f, 1f);


			public static readonly Color disableButtonTextColor = new Color(0.564f, 0.564f, 0.564f, 1f);


			public static readonly Color uiLineRendererFavorite = new Color32(149, 249, byte.MaxValue, byte.MaxValue);
		}


		public static class Community
		{
			public const int PersonaStateOnline = 0;


			public const int PersonaStateAway = 1;


			public const int PersonaStateOffline = 2;


			public const string STATE_LUMIA = "LumiaIsland";


			public const string STATE_TUTORIAL_GAME = "InTutorialGame";


			public const string STATE_LOBBY = "InLobby";


			public const string STATE_MATCHING = "InMatching";


			public const string STATE_MATCH_COMPLETED = "MatchCompleted";


			public const string STATE_GAME = "InGame";


			public const string STATE_CUSTOM_LOBBY = "InCustomLobby";


			public const string STATE_CUSTOM_GAME = "InCustomGame";


			public const string STATE_BRIEFING = "InBriefing";


			public const string STATE_CHARACTER_SELECT = "InCharacterSelect";


			public static readonly Color BgWaitingColor = new Color(0.17f, 0.21f, 0.22f);


			public static readonly Color BgInviteColor = new Color(0.18f, 0.23f, 0.17f);


			public static readonly Color OnlineColor = new Color(0.48f, 0.89f, 0f);


			public static readonly Color NotPlayingColor = new Color(0.1f, 0.5f, 0f);


			public static readonly Color OfflineColor = new Color(0.45f, 0.07f, 0.07f);


			public static readonly Color OfflineTextColor = new Color(0.4f, 0.4f, 0.4f);


			public static readonly Color AwayColor = new Color(0.62f, 0.62f, 0.62f);


			public static readonly Color PlayingColor = new Color(0f, 0.9f, 0.9f);


			public static readonly Color WaitingColor = new Color(1f, 0.84f, 0.35f);
		}


		public static class Stat
		{
			public const int DEFAULT_SIGHT_ANGLE = 360;


			public const float DEFAULT_ATTACK_SPEED_LIMIT = 2.5f;

			public static readonly int STAT_LEN = Enum.GetNames(typeof(StatType)).Length;
		}


		public static class InGame
		{
			public const float ATTACK_SUPPORT_RANGE = 3f;


			public const float ATTACK_SUPPORT_RANGE_2 = 5f;


			public const float ATTACK_SIGHT_SHARE_DURATION = 4f;


			public const float ATTACK_SIGHT_SHARE_RANGE = 3f;


			public const float SECURITY_CAMERA_SIGHT_DURATION = 180f;


			public const float SECURITY_CAMERA_SIGHT_RANGE = 10f;


			public const float HYPERLOOP_COOLDOWN = 1f;


			public const float EYE_OFFSET = 1.5f;


			public const float ObserverSightRange = 15f;


			public static readonly Vector3 LAST_SAFE_ZONE_SIZE = new Vector3(10f, 2f, 10f);
		}


		public static class TeamMode
		{
			public const int DYINGCONDITION_MAXHP = 500;


			public const int AddMasteryExpIncrementPercent = 15;


			public const int AddMasteryExpIncrementPercent2 = 15;


			public const float PingDuringTime = 6f;


			public const int MaxPingNumber = 10;


			public const float PingCheckTime = 3f;


			public const float PingForbidTime = 5f;


			public const int TEAMSLOT_1 = 1;


			public const int TEAMSLOT_2 = 2;


			public const int TEAMSLOT_3 = 3;


			public const float MarkRemoveDistance = 0.4f;


			public static readonly Color ENEMY_COLOR = new Color32(243, 41, 0, byte.MaxValue);


			public static readonly Color ALLY_COLOR = new Color32(0, 142, 237, byte.MaxValue);


			public static readonly Color MY_COLOR = new Color32(50, 215, 0, byte.MaxValue);


			public static readonly Color DYINGCONDITION_COLOR = new Color32(150, 0, 0, byte.MaxValue);


			private static readonly Color TEAMCOLOR_1 = new Color(0.09f, 0.73f, 1f);


			private static readonly Color TEAMCOLOR_2 = new Color(0.27f, 0.83f, 0.11f);


			private static readonly Color TEAMCOLOR_3 = new Color(0.95f, 0.82f, 0.12f);

			public static Color GetTeamColor(int teamSlot)
			{
				switch (teamSlot)
				{
					case 1:
						return TEAMCOLOR_1;
					case 2:
						return TEAMCOLOR_2;
					case 3:
						return TEAMCOLOR_3;
					default:
						return TEAMCOLOR_1;
				}
			}
		}


		public static class AnimationKey
		{
			public const string T_TELEPORT_ARRIVE = "tTeleportArrive";


			public static string ANIMATION_CANCEL_TRIGGER = "cancel";


			public static int ANIMATION_CANCEL_TRIGGER_KEY = Animator.StringToHash(ANIMATION_CANCEL_TRIGGER);
		}


		public static class LOGIN_KEY
		{
			public const string ID_TOKEN_KEY = "_dev_id_token";
		}
	}
}