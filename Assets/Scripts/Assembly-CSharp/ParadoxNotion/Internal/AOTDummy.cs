using Blis.Common;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using NodeCanvas.Tasks.Actions;
using NodeCanvas.Tasks.Conditions;
using UnityEngine;

namespace ParadoxNotion.Internal
{
	
	internal class AOTDummy
	{
		
		private BBParameter<AIState> NodeCanvas_Framework_BBParameter_Blis_Common_AIState;

		
		private BBParameter<BotDifficulty> NodeCanvas_Framework_BBParameter_Blis_Common_BotDifficulty;

		
		private BBParameter<DayNight> NodeCanvas_Framework_BBParameter_Blis_Common_DayNight;

		
		private BBParameter<MonsterType> NodeCanvas_Framework_BBParameter_Blis_Common_MonsterType;

		
		private BBParameter<bool> NodeCanvas_Framework_BBParameter_System_Boolean;

		
		private BBParameter<int> NodeCanvas_Framework_BBParameter_System_Int32;

		
		private BBParameter<float> NodeCanvas_Framework_BBParameter_System_Single;

		
		private BBParameter<Bounds> NodeCanvas_Framework_BBParameter_UnityEngine_Bounds;

		
		private BBParameter<Collision> NodeCanvas_Framework_BBParameter_UnityEngine_Collision;

		
		private BBParameter<Collision2D> NodeCanvas_Framework_BBParameter_UnityEngine_Collision2D;

		
		private BBParameter<Color> NodeCanvas_Framework_BBParameter_UnityEngine_Color;

		
		private BBParameter<ContactPoint> NodeCanvas_Framework_BBParameter_UnityEngine_ContactPoint;

		
		private BBParameter<ContactPoint2D> NodeCanvas_Framework_BBParameter_UnityEngine_ContactPoint2D;

		
		private BBParameter<Keyframe> NodeCanvas_Framework_BBParameter_UnityEngine_Keyframe;

		
		private BBParameter<LayerMask> NodeCanvas_Framework_BBParameter_UnityEngine_LayerMask;

		
		private BBParameter<Quaternion> NodeCanvas_Framework_BBParameter_UnityEngine_Quaternion;

		
		private BBParameter<Ray> NodeCanvas_Framework_BBParameter_UnityEngine_Ray;

		
		private BBParameter<RaycastHit> NodeCanvas_Framework_BBParameter_UnityEngine_RaycastHit;

		
		private BBParameter<RaycastHit2D> NodeCanvas_Framework_BBParameter_UnityEngine_RaycastHit2D;

		
		private BBParameter<Rect> NodeCanvas_Framework_BBParameter_UnityEngine_Rect;

		
		private BBParameter<Space> NodeCanvas_Framework_BBParameter_UnityEngine_Space;

		
		private BBParameter<Vector2> NodeCanvas_Framework_BBParameter_UnityEngine_Vector2;

		
		private BBParameter<Vector3> NodeCanvas_Framework_BBParameter_UnityEngine_Vector3;

		
		private BBParameter<Vector4> NodeCanvas_Framework_BBParameter_UnityEngine_Vector4;

		
		private ExposedParameter<AIState> NodeCanvas_Framework_ExposedParameter_Blis_Common_AIState;

		
		private ExposedParameter<BotDifficulty> NodeCanvas_Framework_ExposedParameter_Blis_Common_BotDifficulty;

		
		private ExposedParameter<DayNight> NodeCanvas_Framework_ExposedParameter_Blis_Common_DayNight;

		
		private ExposedParameter<MonsterType> NodeCanvas_Framework_ExposedParameter_Blis_Common_MonsterType;

		
		private ExposedParameter<bool> NodeCanvas_Framework_ExposedParameter_System_Boolean;

		
		private ExposedParameter<int> NodeCanvas_Framework_ExposedParameter_System_Int32;

		
		private ExposedParameter<float> NodeCanvas_Framework_ExposedParameter_System_Single;

		
		private ExposedParameter<Bounds> NodeCanvas_Framework_ExposedParameter_UnityEngine_Bounds;

		
		private ExposedParameter<Collision> NodeCanvas_Framework_ExposedParameter_UnityEngine_Collision;

		
		private ExposedParameter<Collision2D> NodeCanvas_Framework_ExposedParameter_UnityEngine_Collision2D;

		
		private ExposedParameter<Color> NodeCanvas_Framework_ExposedParameter_UnityEngine_Color;

		
		private ExposedParameter<ContactPoint> NodeCanvas_Framework_ExposedParameter_UnityEngine_ContactPoint;

		
		private ExposedParameter<ContactPoint2D> NodeCanvas_Framework_ExposedParameter_UnityEngine_ContactPoint2D;

		
		private ExposedParameter<Keyframe> NodeCanvas_Framework_ExposedParameter_UnityEngine_Keyframe;

		
		private ExposedParameter<LayerMask> NodeCanvas_Framework_ExposedParameter_UnityEngine_LayerMask;

		
		private ExposedParameter<Quaternion> NodeCanvas_Framework_ExposedParameter_UnityEngine_Quaternion;

		
		private ExposedParameter<Ray> NodeCanvas_Framework_ExposedParameter_UnityEngine_Ray;

		
		private ExposedParameter<RaycastHit> NodeCanvas_Framework_ExposedParameter_UnityEngine_RaycastHit;

		
		private ExposedParameter<RaycastHit2D> NodeCanvas_Framework_ExposedParameter_UnityEngine_RaycastHit2D;

		
		private ExposedParameter<Rect> NodeCanvas_Framework_ExposedParameter_UnityEngine_Rect;

		
		private ExposedParameter<Space> NodeCanvas_Framework_ExposedParameter_UnityEngine_Space;

		
		private ExposedParameter<Vector2> NodeCanvas_Framework_ExposedParameter_UnityEngine_Vector2;

		
		private ExposedParameter<Vector3> NodeCanvas_Framework_ExposedParameter_UnityEngine_Vector3;

		
		private ExposedParameter<Vector4> NodeCanvas_Framework_ExposedParameter_UnityEngine_Vector4;

		
		private ReflectedAction<AIState> NodeCanvas_Framework_Internal_ReflectedAction_Blis_Common_AIState;

		
		private ReflectedAction<BotDifficulty> NodeCanvas_Framework_Internal_ReflectedAction_Blis_Common_BotDifficulty;

		
		private ReflectedAction<DayNight> NodeCanvas_Framework_Internal_ReflectedAction_Blis_Common_DayNight;

		
		private ReflectedAction<MonsterType> NodeCanvas_Framework_Internal_ReflectedAction_Blis_Common_MonsterType;

		
		private ReflectedAction<bool> NodeCanvas_Framework_Internal_ReflectedAction_System_Boolean;

		
		private ReflectedAction<int> NodeCanvas_Framework_Internal_ReflectedAction_System_Int32;

		
		private ReflectedAction<float> NodeCanvas_Framework_Internal_ReflectedAction_System_Single;

		
		private ReflectedAction<Bounds> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_Bounds;

		
		private ReflectedAction<Collision> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_Collision;

		
		private ReflectedAction<Collision2D> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_Collision2D;

		
		private ReflectedAction<Color> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_Color;

		
		private ReflectedAction<ContactPoint> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_ContactPoint;

		
		private ReflectedAction<ContactPoint2D>
			NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_ContactPoint2D;

		
		private ReflectedAction<Keyframe> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_Keyframe;

		
		private ReflectedAction<LayerMask> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_LayerMask;

		
		private ReflectedAction<Quaternion> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_Quaternion;

		
		private ReflectedAction<Ray> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_Ray;

		
		private ReflectedAction<RaycastHit> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_RaycastHit;

		
		private ReflectedAction<RaycastHit2D> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_RaycastHit2D;

		
		private ReflectedAction<Rect> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_Rect;

		
		private ReflectedAction<Space> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_Space;

		
		private ReflectedAction<Vector2> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_Vector2;

		
		private ReflectedAction<Vector3> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_Vector3;

		
		private ReflectedAction<Vector4> NodeCanvas_Framework_Internal_ReflectedAction_UnityEngine_Vector4;

		
		private ReflectedFunction<AIState> NodeCanvas_Framework_Internal_ReflectedFunction_Blis_Common_AIState;

		
		private ReflectedFunction<BotDifficulty>
			NodeCanvas_Framework_Internal_ReflectedFunction_Blis_Common_BotDifficulty;

		
		private ReflectedFunction<DayNight> NodeCanvas_Framework_Internal_ReflectedFunction_Blis_Common_DayNight;

		
		private ReflectedFunction<MonsterType> NodeCanvas_Framework_Internal_ReflectedFunction_Blis_Common_MonsterType;

		
		private ReflectedFunction<bool> NodeCanvas_Framework_Internal_ReflectedFunction_System_Boolean;

		
		private ReflectedFunction<int> NodeCanvas_Framework_Internal_ReflectedFunction_System_Int32;

		
		private ReflectedFunction<float> NodeCanvas_Framework_Internal_ReflectedFunction_System_Single;

		
		private ReflectedFunction<Bounds> NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_Bounds;

		
		private ReflectedFunction<Collision> NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_Collision;

		
		private ReflectedFunction<Collision2D> NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_Collision2D;

		
		private ReflectedFunction<Color> NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_Color;

		
		private ReflectedFunction<ContactPoint>
			NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_ContactPoint;

		
		private ReflectedFunction<ContactPoint2D>
			NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_ContactPoint2D;

		
		private ReflectedFunction<Keyframe> NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_Keyframe;

		
		private ReflectedFunction<LayerMask> NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_LayerMask;

		
		private ReflectedFunction<Quaternion> NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_Quaternion;

		
		private ReflectedFunction<Ray> NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_Ray;

		
		private ReflectedFunction<RaycastHit> NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_RaycastHit;

		
		private ReflectedFunction<RaycastHit2D>
			NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_RaycastHit2D;

		
		private ReflectedFunction<Rect> NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_Rect;

		
		private ReflectedFunction<Space> NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_Space;

		
		private ReflectedFunction<Vector2> NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_Vector2;

		
		private ReflectedFunction<Vector3> NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_Vector3;

		
		private ReflectedFunction<Vector4> NodeCanvas_Framework_Internal_ReflectedFunction_UnityEngine_Vector4;

		
		private Variable<AIState> NodeCanvas_Framework_Variable_Blis_Common_AIState;

		
		private Variable<BotDifficulty> NodeCanvas_Framework_Variable_Blis_Common_BotDifficulty;

		
		private Variable<DayNight> NodeCanvas_Framework_Variable_Blis_Common_DayNight;

		
		private Variable<MonsterType> NodeCanvas_Framework_Variable_Blis_Common_MonsterType;

		
		private Variable<bool> NodeCanvas_Framework_Variable_System_Boolean;

		
		private Variable<int> NodeCanvas_Framework_Variable_System_Int32;

		
		private Variable<float> NodeCanvas_Framework_Variable_System_Single;

		
		private Variable<Bounds> NodeCanvas_Framework_Variable_UnityEngine_Bounds;

		
		private Variable<Collision> NodeCanvas_Framework_Variable_UnityEngine_Collision;

		
		private Variable<Collision2D> NodeCanvas_Framework_Variable_UnityEngine_Collision2D;

		
		private Variable<Color> NodeCanvas_Framework_Variable_UnityEngine_Color;

		
		private Variable<ContactPoint> NodeCanvas_Framework_Variable_UnityEngine_ContactPoint;

		
		private Variable<ContactPoint2D> NodeCanvas_Framework_Variable_UnityEngine_ContactPoint2D;

		
		private Variable<Keyframe> NodeCanvas_Framework_Variable_UnityEngine_Keyframe;

		
		private Variable<LayerMask> NodeCanvas_Framework_Variable_UnityEngine_LayerMask;

		
		private Variable<Quaternion> NodeCanvas_Framework_Variable_UnityEngine_Quaternion;

		
		private Variable<Ray> NodeCanvas_Framework_Variable_UnityEngine_Ray;

		
		private Variable<RaycastHit> NodeCanvas_Framework_Variable_UnityEngine_RaycastHit;

		
		private Variable<RaycastHit2D> NodeCanvas_Framework_Variable_UnityEngine_RaycastHit2D;

		
		private Variable<Rect> NodeCanvas_Framework_Variable_UnityEngine_Rect;

		
		private Variable<Space> NodeCanvas_Framework_Variable_UnityEngine_Space;

		
		private Variable<Vector2> NodeCanvas_Framework_Variable_UnityEngine_Vector2;

		
		private Variable<Vector3> NodeCanvas_Framework_Variable_UnityEngine_Vector3;

		
		private Variable<Vector4> NodeCanvas_Framework_Variable_UnityEngine_Vector4;

		
		private AddElementToDictionary<AIState> NodeCanvas_Tasks_Actions_AddElementToDictionary_Blis_Common_AIState;

		
		private AddElementToDictionary<BotDifficulty>
			NodeCanvas_Tasks_Actions_AddElementToDictionary_Blis_Common_BotDifficulty;

		
		private AddElementToDictionary<DayNight> NodeCanvas_Tasks_Actions_AddElementToDictionary_Blis_Common_DayNight;

		
		private AddElementToDictionary<MonsterType>
			NodeCanvas_Tasks_Actions_AddElementToDictionary_Blis_Common_MonsterType;

		
		private AddElementToDictionary<bool> NodeCanvas_Tasks_Actions_AddElementToDictionary_System_Boolean;

		
		private AddElementToDictionary<int> NodeCanvas_Tasks_Actions_AddElementToDictionary_System_Int32;

		
		private AddElementToDictionary<float> NodeCanvas_Tasks_Actions_AddElementToDictionary_System_Single;

		
		private AddElementToDictionary<Bounds> NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_Bounds;

		
		private AddElementToDictionary<Collision> NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_Collision;

		
		private AddElementToDictionary<Collision2D>
			NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_Collision2D;

		
		private AddElementToDictionary<Color> NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_Color;

		
		private AddElementToDictionary<ContactPoint>
			NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_ContactPoint;

		
		private AddElementToDictionary<ContactPoint2D>
			NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_ContactPoint2D;

		
		private AddElementToDictionary<Keyframe> NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_Keyframe;

		
		private AddElementToDictionary<LayerMask> NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_LayerMask;

		
		private AddElementToDictionary<Quaternion>
			NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_Quaternion;

		
		private AddElementToDictionary<Ray> NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_Ray;

		
		private AddElementToDictionary<RaycastHit>
			NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_RaycastHit;

		
		private AddElementToDictionary<RaycastHit2D>
			NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_RaycastHit2D;

		
		private AddElementToDictionary<Rect> NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_Rect;

		
		private AddElementToDictionary<Space> NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_Space;

		
		private AddElementToDictionary<Vector2> NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_Vector2;

		
		private AddElementToDictionary<Vector3> NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_Vector3;

		
		private AddElementToDictionary<Vector4> NodeCanvas_Tasks_Actions_AddElementToDictionary_UnityEngine_Vector4;

		
		private AddElementToList<AIState> NodeCanvas_Tasks_Actions_AddElementToList_Blis_Common_AIState;

		
		private AddElementToList<BotDifficulty> NodeCanvas_Tasks_Actions_AddElementToList_Blis_Common_BotDifficulty;

		
		private AddElementToList<DayNight> NodeCanvas_Tasks_Actions_AddElementToList_Blis_Common_DayNight;

		
		private AddElementToList<MonsterType> NodeCanvas_Tasks_Actions_AddElementToList_Blis_Common_MonsterType;

		
		private AddElementToList<bool> NodeCanvas_Tasks_Actions_AddElementToList_System_Boolean;

		
		private AddElementToList<int> NodeCanvas_Tasks_Actions_AddElementToList_System_Int32;

		
		private AddElementToList<float> NodeCanvas_Tasks_Actions_AddElementToList_System_Single;

		
		private AddElementToList<Bounds> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_Bounds;

		
		private AddElementToList<Collision> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_Collision;

		
		private AddElementToList<Collision2D> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_Collision2D;

		
		private AddElementToList<Color> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_Color;

		
		private AddElementToList<ContactPoint> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_ContactPoint;

		
		private AddElementToList<ContactPoint2D> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_ContactPoint2D;

		
		private AddElementToList<Keyframe> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_Keyframe;

		
		private AddElementToList<LayerMask> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_LayerMask;

		
		private AddElementToList<Quaternion> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_Quaternion;

		
		private AddElementToList<Ray> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_Ray;

		
		private AddElementToList<RaycastHit> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_RaycastHit;

		
		private AddElementToList<RaycastHit2D> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_RaycastHit2D;

		
		private AddElementToList<Rect> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_Rect;

		
		private AddElementToList<Space> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_Space;

		
		private AddElementToList<Vector2> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_Vector2;

		
		private AddElementToList<Vector3> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_Vector3;

		
		private AddElementToList<Vector4> NodeCanvas_Tasks_Actions_AddElementToList_UnityEngine_Vector4;

		
		private GetDictionaryElement<AIState> NodeCanvas_Tasks_Actions_GetDictionaryElement_Blis_Common_AIState;

		
		private GetDictionaryElement<BotDifficulty>
			NodeCanvas_Tasks_Actions_GetDictionaryElement_Blis_Common_BotDifficulty;

		
		private GetDictionaryElement<DayNight> NodeCanvas_Tasks_Actions_GetDictionaryElement_Blis_Common_DayNight;

		
		private GetDictionaryElement<MonsterType> NodeCanvas_Tasks_Actions_GetDictionaryElement_Blis_Common_MonsterType;

		
		private GetDictionaryElement<bool> NodeCanvas_Tasks_Actions_GetDictionaryElement_System_Boolean;

		
		private GetDictionaryElement<int> NodeCanvas_Tasks_Actions_GetDictionaryElement_System_Int32;

		
		private GetDictionaryElement<float> NodeCanvas_Tasks_Actions_GetDictionaryElement_System_Single;

		
		private GetDictionaryElement<Bounds> NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_Bounds;

		
		private GetDictionaryElement<Collision> NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_Collision;

		
		private GetDictionaryElement<Collision2D> NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_Collision2D;

		
		private GetDictionaryElement<Color> NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_Color;

		
		private GetDictionaryElement<ContactPoint>
			NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_ContactPoint;

		
		private GetDictionaryElement<ContactPoint2D>
			NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_ContactPoint2D;

		
		private GetDictionaryElement<Keyframe> NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_Keyframe;

		
		private GetDictionaryElement<LayerMask> NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_LayerMask;

		
		private GetDictionaryElement<Quaternion> NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_Quaternion;

		
		private GetDictionaryElement<Ray> NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_Ray;

		
		private GetDictionaryElement<RaycastHit> NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_RaycastHit;

		
		private GetDictionaryElement<RaycastHit2D>
			NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_RaycastHit2D;

		
		private GetDictionaryElement<Rect> NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_Rect;

		
		private GetDictionaryElement<Space> NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_Space;

		
		private GetDictionaryElement<Vector2> NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_Vector2;

		
		private GetDictionaryElement<Vector3> NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_Vector3;

		
		private GetDictionaryElement<Vector4> NodeCanvas_Tasks_Actions_GetDictionaryElement_UnityEngine_Vector4;

		
		private GetIndexOfElement<AIState> NodeCanvas_Tasks_Actions_GetIndexOfElement_Blis_Common_AIState;

		
		private GetIndexOfElement<BotDifficulty> NodeCanvas_Tasks_Actions_GetIndexOfElement_Blis_Common_BotDifficulty;

		
		private GetIndexOfElement<DayNight> NodeCanvas_Tasks_Actions_GetIndexOfElement_Blis_Common_DayNight;

		
		private GetIndexOfElement<MonsterType> NodeCanvas_Tasks_Actions_GetIndexOfElement_Blis_Common_MonsterType;

		
		private GetIndexOfElement<bool> NodeCanvas_Tasks_Actions_GetIndexOfElement_System_Boolean;

		
		private GetIndexOfElement<int> NodeCanvas_Tasks_Actions_GetIndexOfElement_System_Int32;

		
		private GetIndexOfElement<float> NodeCanvas_Tasks_Actions_GetIndexOfElement_System_Single;

		
		private GetIndexOfElement<Bounds> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_Bounds;

		
		private GetIndexOfElement<Collision> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_Collision;

		
		private GetIndexOfElement<Collision2D> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_Collision2D;

		
		private GetIndexOfElement<Color> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_Color;

		
		private GetIndexOfElement<ContactPoint> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_ContactPoint;

		
		private GetIndexOfElement<ContactPoint2D> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_ContactPoint2D;

		
		private GetIndexOfElement<Keyframe> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_Keyframe;

		
		private GetIndexOfElement<LayerMask> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_LayerMask;

		
		private GetIndexOfElement<Quaternion> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_Quaternion;

		
		private GetIndexOfElement<Ray> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_Ray;

		
		private GetIndexOfElement<RaycastHit> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_RaycastHit;

		
		private GetIndexOfElement<RaycastHit2D> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_RaycastHit2D;

		
		private GetIndexOfElement<Rect> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_Rect;

		
		private GetIndexOfElement<Space> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_Space;

		
		private GetIndexOfElement<Vector2> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_Vector2;

		
		private GetIndexOfElement<Vector3> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_Vector3;

		
		private GetIndexOfElement<Vector4> NodeCanvas_Tasks_Actions_GetIndexOfElement_UnityEngine_Vector4;

		
		private InsertElementToList<AIState> NodeCanvas_Tasks_Actions_InsertElementToList_Blis_Common_AIState;

		
		private InsertElementToList<BotDifficulty>
			NodeCanvas_Tasks_Actions_InsertElementToList_Blis_Common_BotDifficulty;

		
		private InsertElementToList<DayNight> NodeCanvas_Tasks_Actions_InsertElementToList_Blis_Common_DayNight;

		
		private InsertElementToList<MonsterType> NodeCanvas_Tasks_Actions_InsertElementToList_Blis_Common_MonsterType;

		
		private InsertElementToList<bool> NodeCanvas_Tasks_Actions_InsertElementToList_System_Boolean;

		
		private InsertElementToList<int> NodeCanvas_Tasks_Actions_InsertElementToList_System_Int32;

		
		private InsertElementToList<float> NodeCanvas_Tasks_Actions_InsertElementToList_System_Single;

		
		private InsertElementToList<Bounds> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_Bounds;

		
		private InsertElementToList<Collision> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_Collision;

		
		private InsertElementToList<Collision2D> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_Collision2D;

		
		private InsertElementToList<Color> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_Color;

		
		private InsertElementToList<ContactPoint> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_ContactPoint;

		
		private InsertElementToList<ContactPoint2D>
			NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_ContactPoint2D;

		
		private InsertElementToList<Keyframe> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_Keyframe;

		
		private InsertElementToList<LayerMask> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_LayerMask;

		
		private InsertElementToList<Quaternion> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_Quaternion;

		
		private InsertElementToList<Ray> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_Ray;

		
		private InsertElementToList<RaycastHit> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_RaycastHit;

		
		private InsertElementToList<RaycastHit2D> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_RaycastHit2D;

		
		private InsertElementToList<Rect> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_Rect;

		
		private InsertElementToList<Space> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_Space;

		
		private InsertElementToList<Vector2> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_Vector2;

		
		private InsertElementToList<Vector3> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_Vector3;

		
		private InsertElementToList<Vector4> NodeCanvas_Tasks_Actions_InsertElementToList_UnityEngine_Vector4;

		
		private PickListElement<AIState> NodeCanvas_Tasks_Actions_PickListElement_Blis_Common_AIState;

		
		private PickListElement<BotDifficulty> NodeCanvas_Tasks_Actions_PickListElement_Blis_Common_BotDifficulty;

		
		private PickListElement<DayNight> NodeCanvas_Tasks_Actions_PickListElement_Blis_Common_DayNight;

		
		private PickListElement<MonsterType> NodeCanvas_Tasks_Actions_PickListElement_Blis_Common_MonsterType;

		
		private PickListElement<bool> NodeCanvas_Tasks_Actions_PickListElement_System_Boolean;

		
		private PickListElement<int> NodeCanvas_Tasks_Actions_PickListElement_System_Int32;

		
		private PickListElement<float> NodeCanvas_Tasks_Actions_PickListElement_System_Single;

		
		private PickListElement<Bounds> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_Bounds;

		
		private PickListElement<Collision> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_Collision;

		
		private PickListElement<Collision2D> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_Collision2D;

		
		private PickListElement<Color> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_Color;

		
		private PickListElement<ContactPoint> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_ContactPoint;

		
		private PickListElement<ContactPoint2D> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_ContactPoint2D;

		
		private PickListElement<Keyframe> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_Keyframe;

		
		private PickListElement<LayerMask> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_LayerMask;

		
		private PickListElement<Quaternion> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_Quaternion;

		
		private PickListElement<Ray> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_Ray;

		
		private PickListElement<RaycastHit> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_RaycastHit;

		
		private PickListElement<RaycastHit2D> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_RaycastHit2D;

		
		private PickListElement<Rect> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_Rect;

		
		private PickListElement<Space> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_Space;

		
		private PickListElement<Vector2> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_Vector2;

		
		private PickListElement<Vector3> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_Vector3;

		
		private PickListElement<Vector4> NodeCanvas_Tasks_Actions_PickListElement_UnityEngine_Vector4;

		
		private PickRandomListElement<AIState> NodeCanvas_Tasks_Actions_PickRandomListElement_Blis_Common_AIState;

		
		private PickRandomListElement<BotDifficulty>
			NodeCanvas_Tasks_Actions_PickRandomListElement_Blis_Common_BotDifficulty;

		
		private PickRandomListElement<DayNight> NodeCanvas_Tasks_Actions_PickRandomListElement_Blis_Common_DayNight;

		
		private PickRandomListElement<MonsterType>
			NodeCanvas_Tasks_Actions_PickRandomListElement_Blis_Common_MonsterType;

		
		private PickRandomListElement<bool> NodeCanvas_Tasks_Actions_PickRandomListElement_System_Boolean;

		
		private PickRandomListElement<int> NodeCanvas_Tasks_Actions_PickRandomListElement_System_Int32;

		
		private PickRandomListElement<float> NodeCanvas_Tasks_Actions_PickRandomListElement_System_Single;

		
		private PickRandomListElement<Bounds> NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_Bounds;

		
		private PickRandomListElement<Collision> NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_Collision;

		
		private PickRandomListElement<Collision2D>
			NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_Collision2D;

		
		private PickRandomListElement<Color> NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_Color;

		
		private PickRandomListElement<ContactPoint>
			NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_ContactPoint;

		
		private PickRandomListElement<ContactPoint2D>
			NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_ContactPoint2D;

		
		private PickRandomListElement<Keyframe> NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_Keyframe;

		
		private PickRandomListElement<LayerMask> NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_LayerMask;

		
		private PickRandomListElement<Quaternion> NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_Quaternion;

		
		private PickRandomListElement<Ray> NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_Ray;

		
		private PickRandomListElement<RaycastHit> NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_RaycastHit;

		
		private PickRandomListElement<RaycastHit2D>
			NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_RaycastHit2D;

		
		private PickRandomListElement<Rect> NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_Rect;

		
		private PickRandomListElement<Space> NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_Space;

		
		private PickRandomListElement<Vector2> NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_Vector2;

		
		private PickRandomListElement<Vector3> NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_Vector3;

		
		private PickRandomListElement<Vector4> NodeCanvas_Tasks_Actions_PickRandomListElement_UnityEngine_Vector4;

		
		private RemoveElementFromList<AIState> NodeCanvas_Tasks_Actions_RemoveElementFromList_Blis_Common_AIState;

		
		private RemoveElementFromList<BotDifficulty>
			NodeCanvas_Tasks_Actions_RemoveElementFromList_Blis_Common_BotDifficulty;

		
		private RemoveElementFromList<DayNight> NodeCanvas_Tasks_Actions_RemoveElementFromList_Blis_Common_DayNight;

		
		private RemoveElementFromList<MonsterType>
			NodeCanvas_Tasks_Actions_RemoveElementFromList_Blis_Common_MonsterType;

		
		private RemoveElementFromList<bool> NodeCanvas_Tasks_Actions_RemoveElementFromList_System_Boolean;

		
		private RemoveElementFromList<int> NodeCanvas_Tasks_Actions_RemoveElementFromList_System_Int32;

		
		private RemoveElementFromList<float> NodeCanvas_Tasks_Actions_RemoveElementFromList_System_Single;

		
		private RemoveElementFromList<Bounds> NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_Bounds;

		
		private RemoveElementFromList<Collision> NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_Collision;

		
		private RemoveElementFromList<Collision2D>
			NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_Collision2D;

		
		private RemoveElementFromList<Color> NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_Color;

		
		private RemoveElementFromList<ContactPoint>
			NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_ContactPoint;

		
		private RemoveElementFromList<ContactPoint2D>
			NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_ContactPoint2D;

		
		private RemoveElementFromList<Keyframe> NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_Keyframe;

		
		private RemoveElementFromList<LayerMask> NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_LayerMask;

		
		private RemoveElementFromList<Quaternion> NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_Quaternion;

		
		private RemoveElementFromList<Ray> NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_Ray;

		
		private RemoveElementFromList<RaycastHit> NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_RaycastHit;

		
		private RemoveElementFromList<RaycastHit2D>
			NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_RaycastHit2D;

		
		private RemoveElementFromList<Rect> NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_Rect;

		
		private RemoveElementFromList<Space> NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_Space;

		
		private RemoveElementFromList<Vector2> NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_Vector2;

		
		private RemoveElementFromList<Vector3> NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_Vector3;

		
		private RemoveElementFromList<Vector4> NodeCanvas_Tasks_Actions_RemoveElementFromList_UnityEngine_Vector4;

		
		private SendEvent<AIState> NodeCanvas_Tasks_Actions_SendEvent_Blis_Common_AIState;

		
		private SendEvent<BotDifficulty> NodeCanvas_Tasks_Actions_SendEvent_Blis_Common_BotDifficulty;

		
		private SendEvent<DayNight> NodeCanvas_Tasks_Actions_SendEvent_Blis_Common_DayNight;

		
		private SendEvent<MonsterType> NodeCanvas_Tasks_Actions_SendEvent_Blis_Common_MonsterType;

		
		private SendEvent<bool> NodeCanvas_Tasks_Actions_SendEvent_System_Boolean;

		
		private SendEvent<int> NodeCanvas_Tasks_Actions_SendEvent_System_Int32;

		
		private SendEvent<float> NodeCanvas_Tasks_Actions_SendEvent_System_Single;

		
		private SendEvent<Bounds> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_Bounds;

		
		private SendEvent<Collision> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_Collision;

		
		private SendEvent<Collision2D> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_Collision2D;

		
		private SendEvent<Color> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_Color;

		
		private SendEvent<ContactPoint> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_ContactPoint;

		
		private SendEvent<ContactPoint2D> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_ContactPoint2D;

		
		private SendEvent<Keyframe> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_Keyframe;

		
		private SendEvent<LayerMask> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_LayerMask;

		
		private SendEvent<Quaternion> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_Quaternion;

		
		private SendEvent<Ray> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_Ray;

		
		private SendEvent<RaycastHit> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_RaycastHit;

		
		private SendEvent<RaycastHit2D> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_RaycastHit2D;

		
		private SendEvent<Rect> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_Rect;

		
		private SendEvent<Space> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_Space;

		
		private SendEvent<Vector2> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_Vector2;

		
		private SendEvent<Vector3> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_Vector3;

		
		private SendEvent<Vector4> NodeCanvas_Tasks_Actions_SendEvent_UnityEngine_Vector4;

		
		private SendEventToObjects<AIState> NodeCanvas_Tasks_Actions_SendEventToObjects_Blis_Common_AIState;

		
		private SendEventToObjects<BotDifficulty> NodeCanvas_Tasks_Actions_SendEventToObjects_Blis_Common_BotDifficulty;

		
		private SendEventToObjects<DayNight> NodeCanvas_Tasks_Actions_SendEventToObjects_Blis_Common_DayNight;

		
		private SendEventToObjects<MonsterType> NodeCanvas_Tasks_Actions_SendEventToObjects_Blis_Common_MonsterType;

		
		private SendEventToObjects<bool> NodeCanvas_Tasks_Actions_SendEventToObjects_System_Boolean;

		
		private SendEventToObjects<int> NodeCanvas_Tasks_Actions_SendEventToObjects_System_Int32;

		
		private SendEventToObjects<float> NodeCanvas_Tasks_Actions_SendEventToObjects_System_Single;

		
		private SendEventToObjects<Bounds> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_Bounds;

		
		private SendEventToObjects<Collision> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_Collision;

		
		private SendEventToObjects<Collision2D> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_Collision2D;

		
		private SendEventToObjects<Color> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_Color;

		
		private SendEventToObjects<ContactPoint> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_ContactPoint;

		
		private SendEventToObjects<ContactPoint2D>
			NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_ContactPoint2D;

		
		private SendEventToObjects<Keyframe> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_Keyframe;

		
		private SendEventToObjects<LayerMask> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_LayerMask;

		
		private SendEventToObjects<Quaternion> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_Quaternion;

		
		private SendEventToObjects<Ray> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_Ray;

		
		private SendEventToObjects<RaycastHit> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_RaycastHit;

		
		private SendEventToObjects<RaycastHit2D> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_RaycastHit2D;

		
		private SendEventToObjects<Rect> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_Rect;

		
		private SendEventToObjects<Space> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_Space;

		
		private SendEventToObjects<Vector2> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_Vector2;

		
		private SendEventToObjects<Vector3> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_Vector3;

		
		private SendEventToObjects<Vector4> NodeCanvas_Tasks_Actions_SendEventToObjects_UnityEngine_Vector4;

		
		private SendMessage<AIState> NodeCanvas_Tasks_Actions_SendMessage_Blis_Common_AIState;

		
		private SendMessage<BotDifficulty> NodeCanvas_Tasks_Actions_SendMessage_Blis_Common_BotDifficulty;

		
		private SendMessage<DayNight> NodeCanvas_Tasks_Actions_SendMessage_Blis_Common_DayNight;

		
		private SendMessage<MonsterType> NodeCanvas_Tasks_Actions_SendMessage_Blis_Common_MonsterType;

		
		private SendMessage<bool> NodeCanvas_Tasks_Actions_SendMessage_System_Boolean;

		
		private SendMessage<int> NodeCanvas_Tasks_Actions_SendMessage_System_Int32;

		
		private SendMessage<float> NodeCanvas_Tasks_Actions_SendMessage_System_Single;

		
		private SendMessage<Bounds> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_Bounds;

		
		private SendMessage<Collision> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_Collision;

		
		private SendMessage<Collision2D> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_Collision2D;

		
		private SendMessage<Color> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_Color;

		
		private SendMessage<ContactPoint> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_ContactPoint;

		
		private SendMessage<ContactPoint2D> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_ContactPoint2D;

		
		private SendMessage<Keyframe> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_Keyframe;

		
		private SendMessage<LayerMask> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_LayerMask;

		
		private SendMessage<Quaternion> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_Quaternion;

		
		private SendMessage<Ray> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_Ray;

		
		private SendMessage<RaycastHit> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_RaycastHit;

		
		private SendMessage<RaycastHit2D> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_RaycastHit2D;

		
		private SendMessage<Rect> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_Rect;

		
		private SendMessage<Space> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_Space;

		
		private SendMessage<Vector2> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_Vector2;

		
		private SendMessage<Vector3> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_Vector3;

		
		private SendMessage<Vector4> NodeCanvas_Tasks_Actions_SendMessage_UnityEngine_Vector4;

		
		private SetListElement<AIState> NodeCanvas_Tasks_Actions_SetListElement_Blis_Common_AIState;

		
		private SetListElement<BotDifficulty> NodeCanvas_Tasks_Actions_SetListElement_Blis_Common_BotDifficulty;

		
		private SetListElement<DayNight> NodeCanvas_Tasks_Actions_SetListElement_Blis_Common_DayNight;

		
		private SetListElement<MonsterType> NodeCanvas_Tasks_Actions_SetListElement_Blis_Common_MonsterType;

		
		private SetListElement<bool> NodeCanvas_Tasks_Actions_SetListElement_System_Boolean;

		
		private SetListElement<int> NodeCanvas_Tasks_Actions_SetListElement_System_Int32;

		
		private SetListElement<float> NodeCanvas_Tasks_Actions_SetListElement_System_Single;

		
		private SetListElement<Bounds> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_Bounds;

		
		private SetListElement<Collision> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_Collision;

		
		private SetListElement<Collision2D> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_Collision2D;

		
		private SetListElement<Color> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_Color;

		
		private SetListElement<ContactPoint> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_ContactPoint;

		
		private SetListElement<ContactPoint2D> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_ContactPoint2D;

		
		private SetListElement<Keyframe> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_Keyframe;

		
		private SetListElement<LayerMask> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_LayerMask;

		
		private SetListElement<Quaternion> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_Quaternion;

		
		private SetListElement<Ray> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_Ray;

		
		private SetListElement<RaycastHit> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_RaycastHit;

		
		private SetListElement<RaycastHit2D> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_RaycastHit2D;

		
		private SetListElement<Rect> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_Rect;

		
		private SetListElement<Space> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_Space;

		
		private SetListElement<Vector2> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_Vector2;

		
		private SetListElement<Vector3> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_Vector3;

		
		private SetListElement<Vector4> NodeCanvas_Tasks_Actions_SetListElement_UnityEngine_Vector4;

		
		private SetVariable<AIState> NodeCanvas_Tasks_Actions_SetVariable_Blis_Common_AIState;

		
		private SetVariable<BotDifficulty> NodeCanvas_Tasks_Actions_SetVariable_Blis_Common_BotDifficulty;

		
		private SetVariable<DayNight> NodeCanvas_Tasks_Actions_SetVariable_Blis_Common_DayNight;

		
		private SetVariable<MonsterType> NodeCanvas_Tasks_Actions_SetVariable_Blis_Common_MonsterType;

		
		private SetVariable<bool> NodeCanvas_Tasks_Actions_SetVariable_System_Boolean;

		
		private SetVariable<int> NodeCanvas_Tasks_Actions_SetVariable_System_Int32;

		
		private SetVariable<float> NodeCanvas_Tasks_Actions_SetVariable_System_Single;

		
		private SetVariable<Bounds> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_Bounds;

		
		private SetVariable<Collision> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_Collision;

		
		private SetVariable<Collision2D> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_Collision2D;

		
		private SetVariable<Color> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_Color;

		
		private SetVariable<ContactPoint> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_ContactPoint;

		
		private SetVariable<ContactPoint2D> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_ContactPoint2D;

		
		private SetVariable<Keyframe> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_Keyframe;

		
		private SetVariable<LayerMask> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_LayerMask;

		
		private SetVariable<Quaternion> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_Quaternion;

		
		private SetVariable<Ray> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_Ray;

		
		private SetVariable<RaycastHit> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_RaycastHit;

		
		private SetVariable<RaycastHit2D> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_RaycastHit2D;

		
		private SetVariable<Rect> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_Rect;

		
		private SetVariable<Space> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_Space;

		
		private SetVariable<Vector2> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_Vector2;

		
		private SetVariable<Vector3> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_Vector3;

		
		private SetVariable<Vector4> NodeCanvas_Tasks_Actions_SetVariable_UnityEngine_Vector4;

		
		private CheckCSharpEvent<AIState> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_Blis_Common_AIState;

		
		private CheckCSharpEvent<BotDifficulty> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_Blis_Common_BotDifficulty;

		
		private CheckCSharpEvent<DayNight> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_Blis_Common_DayNight;

		
		private CheckCSharpEvent<MonsterType> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_Blis_Common_MonsterType;

		
		private CheckCSharpEvent<bool> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_System_Boolean;

		
		private CheckCSharpEvent<int> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_System_Int32;

		
		private CheckCSharpEvent<float> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_System_Single;

		
		private CheckCSharpEvent<Bounds> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_Bounds;

		
		private CheckCSharpEvent<Collision> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_Collision;

		
		private CheckCSharpEvent<Collision2D> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_Collision2D;

		
		private CheckCSharpEvent<Color> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_Color;

		
		private CheckCSharpEvent<ContactPoint> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_ContactPoint;

		
		private CheckCSharpEvent<ContactPoint2D>
			NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_ContactPoint2D;

		
		private CheckCSharpEvent<Keyframe> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_Keyframe;

		
		private CheckCSharpEvent<LayerMask> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_LayerMask;

		
		private CheckCSharpEvent<Quaternion> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_Quaternion;

		
		private CheckCSharpEvent<Ray> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_Ray;

		
		private CheckCSharpEvent<RaycastHit> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_RaycastHit;

		
		private CheckCSharpEvent<RaycastHit2D> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_RaycastHit2D;

		
		private CheckCSharpEvent<Rect> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_Rect;

		
		private CheckCSharpEvent<Space> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_Space;

		
		private CheckCSharpEvent<Vector2> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_Vector2;

		
		private CheckCSharpEvent<Vector3> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_Vector3;

		
		private CheckCSharpEvent<Vector4> NodeCanvas_Tasks_Conditions_CheckCSharpEvent_UnityEngine_Vector4;

		
		private CheckCSharpEventValue<AIState> NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_Blis_Common_AIState;

		
		private CheckCSharpEventValue<BotDifficulty>
			NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_Blis_Common_BotDifficulty;

		
		private CheckCSharpEventValue<DayNight> NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_Blis_Common_DayNight;

		
		private CheckCSharpEventValue<MonsterType>
			NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_Blis_Common_MonsterType;

		
		private CheckCSharpEventValue<bool> NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_System_Boolean;

		
		private CheckCSharpEventValue<int> NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_System_Int32;

		
		private CheckCSharpEventValue<float> NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_System_Single;

		
		private CheckCSharpEventValue<Bounds> NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_Bounds;

		
		private CheckCSharpEventValue<Collision>
			NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_Collision;

		
		private CheckCSharpEventValue<Collision2D>
			NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_Collision2D;

		
		private CheckCSharpEventValue<Color> NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_Color;

		
		private CheckCSharpEventValue<ContactPoint>
			NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_ContactPoint;

		
		private CheckCSharpEventValue<ContactPoint2D>
			NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_ContactPoint2D;

		
		private CheckCSharpEventValue<Keyframe> NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_Keyframe;

		
		private CheckCSharpEventValue<LayerMask>
			NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_LayerMask;

		
		private CheckCSharpEventValue<Quaternion>
			NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_Quaternion;

		
		private CheckCSharpEventValue<Ray> NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_Ray;

		
		private CheckCSharpEventValue<RaycastHit>
			NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_RaycastHit;

		
		private CheckCSharpEventValue<RaycastHit2D>
			NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_RaycastHit2D;

		
		private CheckCSharpEventValue<Rect> NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_Rect;

		
		private CheckCSharpEventValue<Space> NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_Space;

		
		private CheckCSharpEventValue<Vector2> NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_Vector2;

		
		private CheckCSharpEventValue<Vector3> NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_Vector3;

		
		private CheckCSharpEventValue<Vector4> NodeCanvas_Tasks_Conditions_CheckCSharpEventValue_UnityEngine_Vector4;

		
		private CheckEvent<AIState> NodeCanvas_Tasks_Conditions_CheckEvent_Blis_Common_AIState;

		
		private CheckEvent<BotDifficulty> NodeCanvas_Tasks_Conditions_CheckEvent_Blis_Common_BotDifficulty;

		
		private CheckEvent<DayNight> NodeCanvas_Tasks_Conditions_CheckEvent_Blis_Common_DayNight;

		
		private CheckEvent<MonsterType> NodeCanvas_Tasks_Conditions_CheckEvent_Blis_Common_MonsterType;

		
		private CheckEvent<bool> NodeCanvas_Tasks_Conditions_CheckEvent_System_Boolean;

		
		private CheckEvent<int> NodeCanvas_Tasks_Conditions_CheckEvent_System_Int32;

		
		private CheckEvent<float> NodeCanvas_Tasks_Conditions_CheckEvent_System_Single;

		
		private CheckEvent<Bounds> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_Bounds;

		
		private CheckEvent<Collision> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_Collision;

		
		private CheckEvent<Collision2D> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_Collision2D;

		
		private CheckEvent<Color> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_Color;

		
		private CheckEvent<ContactPoint> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_ContactPoint;

		
		private CheckEvent<ContactPoint2D> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_ContactPoint2D;

		
		private CheckEvent<Keyframe> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_Keyframe;

		
		private CheckEvent<LayerMask> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_LayerMask;

		
		private CheckEvent<Quaternion> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_Quaternion;

		
		private CheckEvent<Ray> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_Ray;

		
		private CheckEvent<RaycastHit> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_RaycastHit;

		
		private CheckEvent<RaycastHit2D> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_RaycastHit2D;

		
		private CheckEvent<Rect> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_Rect;

		
		private CheckEvent<Space> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_Space;

		
		private CheckEvent<Vector2> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_Vector2;

		
		private CheckEvent<Vector3> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_Vector3;

		
		private CheckEvent<Vector4> NodeCanvas_Tasks_Conditions_CheckEvent_UnityEngine_Vector4;

		
		private CheckEventValue<AIState> NodeCanvas_Tasks_Conditions_CheckEventValue_Blis_Common_AIState;

		
		private CheckEventValue<BotDifficulty> NodeCanvas_Tasks_Conditions_CheckEventValue_Blis_Common_BotDifficulty;

		
		private CheckEventValue<DayNight> NodeCanvas_Tasks_Conditions_CheckEventValue_Blis_Common_DayNight;

		
		private CheckEventValue<MonsterType> NodeCanvas_Tasks_Conditions_CheckEventValue_Blis_Common_MonsterType;

		
		private CheckEventValue<bool> NodeCanvas_Tasks_Conditions_CheckEventValue_System_Boolean;

		
		private CheckEventValue<int> NodeCanvas_Tasks_Conditions_CheckEventValue_System_Int32;

		
		private CheckEventValue<float> NodeCanvas_Tasks_Conditions_CheckEventValue_System_Single;

		
		private CheckEventValue<Bounds> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_Bounds;

		
		private CheckEventValue<Collision> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_Collision;

		
		private CheckEventValue<Collision2D> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_Collision2D;

		
		private CheckEventValue<Color> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_Color;

		
		private CheckEventValue<ContactPoint> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_ContactPoint;

		
		private CheckEventValue<ContactPoint2D> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_ContactPoint2D;

		
		private CheckEventValue<Keyframe> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_Keyframe;

		
		private CheckEventValue<LayerMask> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_LayerMask;

		
		private CheckEventValue<Quaternion> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_Quaternion;

		
		private CheckEventValue<Ray> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_Ray;

		
		private CheckEventValue<RaycastHit> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_RaycastHit;

		
		private CheckEventValue<RaycastHit2D> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_RaycastHit2D;

		
		private CheckEventValue<Rect> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_Rect;

		
		private CheckEventValue<Space> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_Space;

		
		private CheckEventValue<Vector2> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_Vector2;

		
		private CheckEventValue<Vector3> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_Vector3;

		
		private CheckEventValue<Vector4> NodeCanvas_Tasks_Conditions_CheckEventValue_UnityEngine_Vector4;

		
		private CheckUnityEvent<AIState> NodeCanvas_Tasks_Conditions_CheckUnityEvent_Blis_Common_AIState;

		
		private CheckUnityEvent<BotDifficulty> NodeCanvas_Tasks_Conditions_CheckUnityEvent_Blis_Common_BotDifficulty;

		
		private CheckUnityEvent<DayNight> NodeCanvas_Tasks_Conditions_CheckUnityEvent_Blis_Common_DayNight;

		
		private CheckUnityEvent<MonsterType> NodeCanvas_Tasks_Conditions_CheckUnityEvent_Blis_Common_MonsterType;

		
		private CheckUnityEvent<bool> NodeCanvas_Tasks_Conditions_CheckUnityEvent_System_Boolean;

		
		private CheckUnityEvent<int> NodeCanvas_Tasks_Conditions_CheckUnityEvent_System_Int32;

		
		private CheckUnityEvent<float> NodeCanvas_Tasks_Conditions_CheckUnityEvent_System_Single;

		
		private CheckUnityEvent<Bounds> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_Bounds;

		
		private CheckUnityEvent<Collision> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_Collision;

		
		private CheckUnityEvent<Collision2D> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_Collision2D;

		
		private CheckUnityEvent<Color> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_Color;

		
		private CheckUnityEvent<ContactPoint> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_ContactPoint;

		
		private CheckUnityEvent<ContactPoint2D> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_ContactPoint2D;

		
		private CheckUnityEvent<Keyframe> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_Keyframe;

		
		private CheckUnityEvent<LayerMask> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_LayerMask;

		
		private CheckUnityEvent<Quaternion> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_Quaternion;

		
		private CheckUnityEvent<Ray> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_Ray;

		
		private CheckUnityEvent<RaycastHit> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_RaycastHit;

		
		private CheckUnityEvent<RaycastHit2D> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_RaycastHit2D;

		
		private CheckUnityEvent<Rect> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_Rect;

		
		private CheckUnityEvent<Space> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_Space;

		
		private CheckUnityEvent<Vector2> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_Vector2;

		
		private CheckUnityEvent<Vector3> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_Vector3;

		
		private CheckUnityEvent<Vector4> NodeCanvas_Tasks_Conditions_CheckUnityEvent_UnityEngine_Vector4;

		
		private CheckUnityEventValue<AIState> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_Blis_Common_AIState;

		
		private CheckUnityEventValue<BotDifficulty>
			NodeCanvas_Tasks_Conditions_CheckUnityEventValue_Blis_Common_BotDifficulty;

		
		private CheckUnityEventValue<DayNight> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_Blis_Common_DayNight;

		
		private CheckUnityEventValue<MonsterType>
			NodeCanvas_Tasks_Conditions_CheckUnityEventValue_Blis_Common_MonsterType;

		
		private CheckUnityEventValue<bool> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_System_Boolean;

		
		private CheckUnityEventValue<int> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_System_Int32;

		
		private CheckUnityEventValue<float> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_System_Single;

		
		private CheckUnityEventValue<Bounds> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_Bounds;

		
		private CheckUnityEventValue<Collision> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_Collision;

		
		private CheckUnityEventValue<Collision2D>
			NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_Collision2D;

		
		private CheckUnityEventValue<Color> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_Color;

		
		private CheckUnityEventValue<ContactPoint>
			NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_ContactPoint;

		
		private CheckUnityEventValue<ContactPoint2D>
			NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_ContactPoint2D;

		
		private CheckUnityEventValue<Keyframe> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_Keyframe;

		
		private CheckUnityEventValue<LayerMask> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_LayerMask;

		
		private CheckUnityEventValue<Quaternion>
			NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_Quaternion;

		
		private CheckUnityEventValue<Ray> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_Ray;

		
		private CheckUnityEventValue<RaycastHit>
			NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_RaycastHit;

		
		private CheckUnityEventValue<RaycastHit2D>
			NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_RaycastHit2D;

		
		private CheckUnityEventValue<Rect> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_Rect;

		
		private CheckUnityEventValue<Space> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_Space;

		
		private CheckUnityEventValue<Vector2> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_Vector2;

		
		private CheckUnityEventValue<Vector3> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_Vector3;

		
		private CheckUnityEventValue<Vector4> NodeCanvas_Tasks_Conditions_CheckUnityEventValue_UnityEngine_Vector4;

		
		private CheckVariable<AIState> NodeCanvas_Tasks_Conditions_CheckVariable_Blis_Common_AIState;

		
		private CheckVariable<BotDifficulty> NodeCanvas_Tasks_Conditions_CheckVariable_Blis_Common_BotDifficulty;

		
		private CheckVariable<DayNight> NodeCanvas_Tasks_Conditions_CheckVariable_Blis_Common_DayNight;

		
		private CheckVariable<MonsterType> NodeCanvas_Tasks_Conditions_CheckVariable_Blis_Common_MonsterType;

		
		private CheckVariable<bool> NodeCanvas_Tasks_Conditions_CheckVariable_System_Boolean;

		
		private CheckVariable<int> NodeCanvas_Tasks_Conditions_CheckVariable_System_Int32;

		
		private CheckVariable<float> NodeCanvas_Tasks_Conditions_CheckVariable_System_Single;

		
		private CheckVariable<Bounds> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_Bounds;

		
		private CheckVariable<Collision> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_Collision;

		
		private CheckVariable<Collision2D> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_Collision2D;

		
		private CheckVariable<Color> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_Color;

		
		private CheckVariable<ContactPoint> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_ContactPoint;

		
		private CheckVariable<ContactPoint2D> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_ContactPoint2D;

		
		private CheckVariable<Keyframe> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_Keyframe;

		
		private CheckVariable<LayerMask> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_LayerMask;

		
		private CheckVariable<Quaternion> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_Quaternion;

		
		private CheckVariable<Ray> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_Ray;

		
		private CheckVariable<RaycastHit> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_RaycastHit;

		
		private CheckVariable<RaycastHit2D> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_RaycastHit2D;

		
		private CheckVariable<Rect> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_Rect;

		
		private CheckVariable<Space> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_Space;

		
		private CheckVariable<Vector2> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_Vector2;

		
		private CheckVariable<Vector3> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_Vector3;

		
		private CheckVariable<Vector4> NodeCanvas_Tasks_Conditions_CheckVariable_UnityEngine_Vector4;

		
		private ListContainsElement<AIState> NodeCanvas_Tasks_Conditions_ListContainsElement_Blis_Common_AIState;

		
		private ListContainsElement<BotDifficulty>
			NodeCanvas_Tasks_Conditions_ListContainsElement_Blis_Common_BotDifficulty;

		
		private ListContainsElement<DayNight> NodeCanvas_Tasks_Conditions_ListContainsElement_Blis_Common_DayNight;

		
		private ListContainsElement<MonsterType>
			NodeCanvas_Tasks_Conditions_ListContainsElement_Blis_Common_MonsterType;

		
		private ListContainsElement<bool> NodeCanvas_Tasks_Conditions_ListContainsElement_System_Boolean;

		
		private ListContainsElement<int> NodeCanvas_Tasks_Conditions_ListContainsElement_System_Int32;

		
		private ListContainsElement<float> NodeCanvas_Tasks_Conditions_ListContainsElement_System_Single;

		
		private ListContainsElement<Bounds> NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_Bounds;

		
		private ListContainsElement<Collision> NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_Collision;

		
		private ListContainsElement<Collision2D>
			NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_Collision2D;

		
		private ListContainsElement<Color> NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_Color;

		
		private ListContainsElement<ContactPoint>
			NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_ContactPoint;

		
		private ListContainsElement<ContactPoint2D>
			NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_ContactPoint2D;

		
		private ListContainsElement<Keyframe> NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_Keyframe;

		
		private ListContainsElement<LayerMask> NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_LayerMask;

		
		private ListContainsElement<Quaternion> NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_Quaternion;

		
		private ListContainsElement<Ray> NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_Ray;

		
		private ListContainsElement<RaycastHit> NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_RaycastHit;

		
		private ListContainsElement<RaycastHit2D>
			NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_RaycastHit2D;

		
		private ListContainsElement<Rect> NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_Rect;

		
		private ListContainsElement<Space> NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_Space;

		
		private ListContainsElement<Vector2> NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_Vector2;

		
		private ListContainsElement<Vector3> NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_Vector3;

		
		private ListContainsElement<Vector4> NodeCanvas_Tasks_Conditions_ListContainsElement_UnityEngine_Vector4;

		
		private TryGetValue<AIState> NodeCanvas_Tasks_Conditions_TryGetValue_Blis_Common_AIState;

		
		private TryGetValue<BotDifficulty> NodeCanvas_Tasks_Conditions_TryGetValue_Blis_Common_BotDifficulty;

		
		private TryGetValue<DayNight> NodeCanvas_Tasks_Conditions_TryGetValue_Blis_Common_DayNight;

		
		private TryGetValue<MonsterType> NodeCanvas_Tasks_Conditions_TryGetValue_Blis_Common_MonsterType;

		
		private TryGetValue<bool> NodeCanvas_Tasks_Conditions_TryGetValue_System_Boolean;

		
		private TryGetValue<int> NodeCanvas_Tasks_Conditions_TryGetValue_System_Int32;

		
		private TryGetValue<float> NodeCanvas_Tasks_Conditions_TryGetValue_System_Single;

		
		private TryGetValue<Bounds> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_Bounds;

		
		private TryGetValue<Collision> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_Collision;

		
		private TryGetValue<Collision2D> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_Collision2D;

		
		private TryGetValue<Color> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_Color;

		
		private TryGetValue<ContactPoint> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_ContactPoint;

		
		private TryGetValue<ContactPoint2D> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_ContactPoint2D;

		
		private TryGetValue<Keyframe> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_Keyframe;

		
		private TryGetValue<LayerMask> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_LayerMask;

		
		private TryGetValue<Quaternion> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_Quaternion;

		
		private TryGetValue<Ray> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_Ray;

		
		private TryGetValue<RaycastHit> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_RaycastHit;

		
		private TryGetValue<RaycastHit2D> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_RaycastHit2D;

		
		private TryGetValue<Rect> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_Rect;

		
		private TryGetValue<Space> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_Space;

		
		private TryGetValue<Vector2> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_Vector2;

		
		private TryGetValue<Vector3> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_Vector3;

		
		private TryGetValue<Vector4> NodeCanvas_Tasks_Conditions_TryGetValue_UnityEngine_Vector4;

		
		private object o = default;

		
		private void NodeCanvas_Framework_Blackboard_GetVariable_1()
		{
			Blackboard obj = null;
			obj.GetVariable<bool>((string) o);
			obj.GetVariable<float>((string) o);
			obj.GetVariable<int>((string) o);
			obj.GetVariable<Vector2>((string) o);
			obj.GetVariable<Vector3>((string) o);
			obj.GetVariable<Vector4>((string) o);
			obj.GetVariable<Quaternion>((string) o);
			obj.GetVariable<Keyframe>((string) o);
			obj.GetVariable<Bounds>((string) o);
			obj.GetVariable<Color>((string) o);
			obj.GetVariable<Rect>((string) o);
			obj.GetVariable<ContactPoint>((string) o);
			obj.GetVariable<ContactPoint2D>((string) o);
			obj.GetVariable<Collision>((string) o);
			obj.GetVariable<Collision2D>((string) o);
			obj.GetVariable<RaycastHit>((string) o);
			obj.GetVariable<RaycastHit2D>((string) o);
			obj.GetVariable<Ray>((string) o);
			obj.GetVariable<Space>((string) o);
			obj.GetVariable<AIState>((string) o);
			obj.GetVariable<BotDifficulty>((string) o);
			obj.GetVariable<DayNight>((string) o);
			obj.GetVariable<MonsterType>((string) o);
			obj.GetVariable<LayerMask>((string) o);
		}

		
		private void NodeCanvas_Framework_Blackboard_GetVariableValue_2()
		{
			Blackboard obj = null;
			obj.GetVariableValue<bool>((string) o);
			obj.GetVariableValue<float>((string) o);
			obj.GetVariableValue<int>((string) o);
			obj.GetVariableValue<Vector2>((string) o);
			obj.GetVariableValue<Vector3>((string) o);
			obj.GetVariableValue<Vector4>((string) o);
			obj.GetVariableValue<Quaternion>((string) o);
			obj.GetVariableValue<Keyframe>((string) o);
			obj.GetVariableValue<Bounds>((string) o);
			obj.GetVariableValue<Color>((string) o);
			obj.GetVariableValue<Rect>((string) o);
			obj.GetVariableValue<ContactPoint>((string) o);
			obj.GetVariableValue<ContactPoint2D>((string) o);
			obj.GetVariableValue<Collision>((string) o);
			obj.GetVariableValue<Collision2D>((string) o);
			obj.GetVariableValue<RaycastHit>((string) o);
			obj.GetVariableValue<RaycastHit2D>((string) o);
			obj.GetVariableValue<Ray>((string) o);
			obj.GetVariableValue<Space>((string) o);
			obj.GetVariableValue<AIState>((string) o);
			obj.GetVariableValue<BotDifficulty>((string) o);
			obj.GetVariableValue<DayNight>((string) o);
			obj.GetVariableValue<MonsterType>((string) o);
			obj.GetVariableValue<LayerMask>((string) o);
		}

		
		private void NodeCanvas_Framework_IBlackboardExtensions_AddVariable_1()
		{
			((IBlackboard) o).AddVariable((string) o, (bool) o);
			((IBlackboard) o).AddVariable((string) o, (float) o);
			((IBlackboard) o).AddVariable((string) o, (int) o);
			((IBlackboard) o).AddVariable((string) o, (Vector2) o);
			((IBlackboard) o).AddVariable((string) o, (Vector3) o);
			((IBlackboard) o).AddVariable((string) o, (Vector4) o);
			((IBlackboard) o).AddVariable((string) o, (Quaternion) o);
			((IBlackboard) o).AddVariable((string) o, (Keyframe) o);
			((IBlackboard) o).AddVariable((string) o, (Bounds) o);
			((IBlackboard) o).AddVariable((string) o, (Color) o);
			((IBlackboard) o).AddVariable((string) o, (Rect) o);
			((IBlackboard) o).AddVariable((string) o, (ContactPoint) o);
			((IBlackboard) o).AddVariable((string) o, (ContactPoint2D) o);
			((IBlackboard) o).AddVariable((string) o, (Collision) o);
			((IBlackboard) o).AddVariable((string) o, (Collision2D) o);
			((IBlackboard) o).AddVariable((string) o, (RaycastHit) o);
			((IBlackboard) o).AddVariable((string) o, (RaycastHit2D) o);
			((IBlackboard) o).AddVariable((string) o, (Ray) o);
			((IBlackboard) o).AddVariable((string) o, (Space) o);
			((IBlackboard) o).AddVariable((string) o, (AIState) o);
			((IBlackboard) o).AddVariable((string) o, (BotDifficulty) o);
			((IBlackboard) o).AddVariable((string) o, (DayNight) o);
			((IBlackboard) o).AddVariable((string) o, (MonsterType) o);
			((IBlackboard) o).AddVariable((string) o, (LayerMask) o);
		}

		
		private void NodeCanvas_Framework_IBlackboardExtensions_AddVariable_2()
		{
			((IBlackboard) o).AddVariable<bool>((string) o);
			((IBlackboard) o).AddVariable<float>((string) o);
			((IBlackboard) o).AddVariable<int>((string) o);
			((IBlackboard) o).AddVariable<Vector2>((string) o);
			((IBlackboard) o).AddVariable<Vector3>((string) o);
			((IBlackboard) o).AddVariable<Vector4>((string) o);
			((IBlackboard) o).AddVariable<Quaternion>((string) o);
			((IBlackboard) o).AddVariable<Keyframe>((string) o);
			((IBlackboard) o).AddVariable<Bounds>((string) o);
			((IBlackboard) o).AddVariable<Color>((string) o);
			((IBlackboard) o).AddVariable<Rect>((string) o);
			((IBlackboard) o).AddVariable<ContactPoint>((string) o);
			((IBlackboard) o).AddVariable<ContactPoint2D>((string) o);
			((IBlackboard) o).AddVariable<Collision>((string) o);
			((IBlackboard) o).AddVariable<Collision2D>((string) o);
			((IBlackboard) o).AddVariable<RaycastHit>((string) o);
			((IBlackboard) o).AddVariable<RaycastHit2D>((string) o);
			((IBlackboard) o).AddVariable<Ray>((string) o);
			((IBlackboard) o).AddVariable<Space>((string) o);
			((IBlackboard) o).AddVariable<AIState>((string) o);
			((IBlackboard) o).AddVariable<BotDifficulty>((string) o);
			((IBlackboard) o).AddVariable<DayNight>((string) o);
			((IBlackboard) o).AddVariable<MonsterType>((string) o);
			((IBlackboard) o).AddVariable<LayerMask>((string) o);
		}

		
		private void NodeCanvas_Framework_IBlackboardExtensions_GetVariableValue_3()
		{
			((IBlackboard) o).GetVariableValue<bool>((string) o);
			double variableValue1 = ((IBlackboard) o).GetVariableValue<float>((string) o);
			((IBlackboard) o).GetVariableValue<int>((string) o);
			((IBlackboard) o).GetVariableValue<Vector2>((string) o);
			((IBlackboard) o).GetVariableValue<Vector3>((string) o);
			((IBlackboard) o).GetVariableValue<Vector4>((string) o);
			((IBlackboard) o).GetVariableValue<Quaternion>((string) o);
			((IBlackboard) o).GetVariableValue<Keyframe>((string) o);
			((IBlackboard) o).GetVariableValue<Bounds>((string) o);
			((IBlackboard) o).GetVariableValue<Color>((string) o);
			((IBlackboard) o).GetVariableValue<Rect>((string) o);
			((IBlackboard) o).GetVariableValue<ContactPoint>((string) o);
			((IBlackboard) o).GetVariableValue<ContactPoint2D>((string) o);
			((IBlackboard) o).GetVariableValue<Collision>((string) o);
			((IBlackboard) o).GetVariableValue<Collision2D>((string) o);
			((IBlackboard) o).GetVariableValue<RaycastHit>((string) o);
			((IBlackboard) o).GetVariableValue<RaycastHit2D>((string) o);
			((IBlackboard) o).GetVariableValue<Ray>((string) o);
			int variableValue2 = (int) ((IBlackboard) o).GetVariableValue<Space>((string) o);
			int variableValue3 = (int) ((IBlackboard) o).GetVariableValue<AIState>((string) o);
			int variableValue4 = (int) ((IBlackboard) o).GetVariableValue<BotDifficulty>((string) o);
			int variableValue5 = (int) ((IBlackboard) o).GetVariableValue<DayNight>((string) o);
			int variableValue6 = (int) ((IBlackboard) o).GetVariableValue<MonsterType>((string) o);
			((IBlackboard) o).GetVariableValue<LayerMask>((string) o);
		}

		
		private void NodeCanvas_Framework_IBlackboardExtensions_GetVariable_4()
		{
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
			((IBlackboard) o).GetVariable((string) o);
		}

		
		private void CustomSpoof() { }
	}
}