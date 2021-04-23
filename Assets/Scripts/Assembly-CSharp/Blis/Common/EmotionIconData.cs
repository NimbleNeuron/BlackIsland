using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class EmotionIconData
	{
		public readonly int code;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly EmotionGrade grade;


		public readonly float inputDelayTime;


		public readonly string name;


		public readonly string prefab;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly EmotionPurchaseType purchaseType;


		public readonly string sound;


		public readonly string sprite;


		[JsonConstructor]
		public EmotionIconData(int code, EmotionGrade grade, EmotionPurchaseType purchaseType, string name,
			string sprite, string prefab, string sound, float inputDelayTime)
		{
			this.code = code;
			this.grade = grade;
			this.purchaseType = purchaseType;
			this.name = name;
			this.sprite = sprite;
			this.prefab = prefab;
			this.sound = sound;
			this.inputDelayTime = inputDelayTime;
		}


		public EmotionIconData Clone()
		{
			return (EmotionIconData) MemberwiseClone();
		}


		public static EmotionIconData CloneEmpty()
		{
			return new EmotionIconData(int.MinValue, EmotionGrade.None, EmotionPurchaseType.NONE, null, null, null,
				null, 0f);
		}
	}
}