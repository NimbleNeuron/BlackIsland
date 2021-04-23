namespace Blis.Common
{
	public class GameDB
	{
		public static readonly bool useDummyData = false;

		public static PlatformDB platform = new PlatformDB();
		public static ItemDB item = new ItemDB();
		public static SkillDB skill = new SkillDB();
		public static CharacterStateDB characterState = new CharacterStateDB();
		public static CharacterDB character = new CharacterDB();
		public static LevelDB level = new LevelDB();
		public static MonsterDB monster = new MonsterDB();
		public static MasteryDB mastery = new MasteryDB();
		public static RecommendDB recommend = new RecommendDB();
		public static ProjectileDB projectile = new ProjectileDB();
		public static EffectAndSoundDB effectAndSound = new EffectAndSoundDB();
		public static BotDB bot = new BotDB();
		public static ProductDB product = new ProductDB();
		public static CharacterVoiceDB characterVoice = new CharacterVoiceDB();
		public static CrowdControlDB crowdControl = new CrowdControlDB();
		public static TutorialDB tutorial = new TutorialDB();
		public static UserDB user = new UserDB();
		public static MissionDB mission = new MissionDB();
		public static EmotionDB emotionIcon = new EmotionDB();
		public static CharacterAttributeDB characterAttibute = new CharacterAttributeDB();
		public static CharacterSkillVideoDB characterSkillVideoDB = new CharacterSkillVideoDB();

		public static void PostInitialize()
		{
			item.PostInitialize();
			level.PostInitialize();
			monster.PostInitialize();
			product.PostInitialize();
		}
	}
}