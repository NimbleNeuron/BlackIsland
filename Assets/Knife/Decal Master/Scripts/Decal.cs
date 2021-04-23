#if UNITY_EDITOR
using System.Reflection;
#endif
using UnityEngine;

namespace Knife.DeferredDecals
{
    [ExecuteInEditMode]
    public class Decal : MonoBehaviour
    {

        [SerializeField]
        private Material m_Material;
        public Material DecalMaterial
        {
            get
            {
                if (m_Material == null)
                    return DeferredDecalsSystem.DefaultDecalMaterial;

                return m_Material;
            }
            set
            {
                m_Material = value;
            }
        }
        public int SortingOrder;
        public Color InstancedColor = Color.white;
        [Range(0f, 1f)]
        public float Fade = 1f;
        public Vector2 UVTiling = new Vector2(1, 1);
        public Vector2 UVOffset = new Vector2(0, 0);
        public Vector4 UV
        {
            get
            {
                return new Vector4(UVTiling.x, UVTiling.y, UVOffset.x, UVOffset.y);
            }
            set
            {
                UVTiling.x = value.x;
                UVTiling.y = value.y;
                UVOffset.x = value.z;
                UVOffset.y = value.w;
            }
        }

        [HideInInspector]
        public float DistanceFade;

        public bool NeedDrawGizmos = true;

        public MaterialPropertyBlock PropertyBlock;

        public Bounds Bounds
        {
            get
            {
                return decalBounds;
            }
            set
            {
                decalBounds = value;
            }
        }

        public Transform CachedTransform
        {
            get
            {
                return cachedTransform;
            }
        }

        Bounds decalBounds;

        Vector3 cachedPosition;
        Vector3 cachedSize;
        Quaternion cachedRotation;

        bool needRebuildBounds = false;

        Transform cachedTransform;

        GameObject decalGameObject;

        public GameObject DecalGameObject
        {
            get
            {
                if (decalGameObject == null)
                    decalGameObject = gameObject;

                return decalGameObject;
            }

            private set
            {
                decalGameObject = value;
            }
        }

        bool IsEnabledAndActive
        {
            get
            {
                return enabled && DecalGameObject.activeInHierarchy;
            }
        }

        public void OnEnable()
        {
            cachedTransform = transform;
            if (IsEnabledAndActive)
                DeferredDecalsManager.instance.AddOrUpdateDecal(this);

#if UNITY_EDITOR
            var egu = typeof(UnityEditor.EditorGUIUtility);
            var flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            var args = new object[] { gameObject, Icon };
            var setIcon = egu.GetMethod("SetIconForObject", flags, null, new System.Type[] { typeof(UnityEngine.Object), typeof(Texture2D) }, null);
            setIcon.Invoke(null, args);
#endif
            PropertyBlock = new MaterialPropertyBlock();
        }

        public void Start()
        {
            if(IsEnabledAndActive)
                DeferredDecalsManager.instance.AddOrUpdateDecal(this);
        }

        public void OnDisable()
        {
            DeferredDecalsManager.instance.RemoveDecal(this);
        }

        private void OnValidate()
        {
            if(IsEnabledAndActive)
                UpdateSortingOrder(SortingOrder);
        }

        public void UpdateSortingOrder(int order)
        {
            SortingOrder = order;
            DeferredDecalsManager.instance.AddOrUpdateDecal(this);
        }

        public void SetupBounds()
        {
            if (!needRebuildBounds)
                return;
            
            float sideX = 0.5f;
            float sideY = 0.5f;
            float sideZ = 0.5f;

            Vector3 p1 = new Vector3(-sideX, -sideY * 2, -sideZ);
            Vector3 p2 = new Vector3(-sideX, -sideY * 2, sideZ);
            Vector3 p3 = new Vector3(sideX, -sideY * 2, sideZ);
            Vector3 p4 = new Vector3(sideX, -sideY * 2, -sideZ);
            Vector3 p5 = new Vector3(-sideX, 0, -sideZ);
            Vector3 p6 = new Vector3(-sideX, 0, sideZ);
            Vector3 p7 = new Vector3(sideX, 0, sideZ);
            Vector3 p8 = new Vector3(sideX, 0, sideZ);

            p1 = CachedTransform.TransformPoint(p1);
            p2 = CachedTransform.TransformPoint(p2);
            p3 = CachedTransform.TransformPoint(p3);
            p4 = CachedTransform.TransformPoint(p4);
            p5 = CachedTransform.TransformPoint(p5);
            p6 = CachedTransform.TransformPoint(p6);
            p7 = CachedTransform.TransformPoint(p7);
            p8 = CachedTransform.TransformPoint(p8);

            float minX = Mathf.Min(p1.x, p2.x, p3.x, p4.x, p5.x, p6.x, p7.x, p8.x);
            float minY = Mathf.Min(p1.y, p2.y, p3.y, p4.y, p5.y, p6.y, p7.y, p8.y);
            float minZ = Mathf.Min(p1.z, p2.z, p3.z, p4.z, p5.z, p6.z, p7.z, p8.z);

            float maxX = Mathf.Max(p1.x, p2.x, p3.x, p4.x, p5.x, p6.x, p7.x, p8.x);
            float maxY = Mathf.Max(p1.y, p2.y, p3.y, p4.y, p5.y, p6.y, p7.y, p8.y);
            float maxZ = Mathf.Max(p1.z, p2.z, p3.z, p4.z, p5.z, p6.z, p7.z, p8.z);

            Vector3 min = new Vector3(minX, minY, minZ);
            Vector3 max = new Vector3(maxX, maxY, maxZ);

            var bounds = Bounds;
            bounds.SetMinMax(min, max);
            Bounds = bounds;

            needRebuildBounds = false;
        }

        public void UpdateBoundsCenter()
        {
            var bounds = Bounds;
            bounds.center = CachedTransform.position - CachedTransform.up * 0.5f * cachedSize.y;
            Bounds = bounds;
        }

        private void DrawGizmo(bool selected)
        {
            // for bounds rendering
            var oldMatrix = Gizmos.matrix;
            if (NeedDrawGizmos || !selected)
            {
                var col = new Color(0.0f, 0.7f, 1f, 1.0f);
                col.a = selected ? 0.1f : 0.05f;
                if(!DeferredDecalsSystem.DrawDecalGizmos && !selected)
                {
                    col.a = 0;
                }
                Gizmos.color = col;
                Gizmos.matrix = CachedTransform.localToWorldMatrix;
                Gizmos.DrawCube(Vector3.zero - Vector3.up * 0.5f, Vector3.one);

                col.a = selected ? 0.5f : 0.25f;
                if (!DeferredDecalsSystem.DrawDecalGizmos && !selected)
                {
                    col.a = 0;
                }
                Gizmos.color = col;
                Gizmos.DrawWireCube(Vector3.zero - Vector3.up * 0.5f, Vector3.one);
            }
            // bounds rendering
            /*Gizmos.matrix = oldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Bounds.center, Bounds.size);*/
        }

        private void Update()
        {
            if(Vector3.Distance(cachedPosition, CachedTransform.position) >= 0.001f)
            {
                cachedPosition = CachedTransform.position;
                UpdateBoundsCenter();
            }

            if(Vector3.Distance(cachedSize, CachedTransform.localScale) >= 0.001f)
            {
                needRebuildBounds = true;
                cachedSize = CachedTransform.localScale;
            }

            if(Quaternion.Angle(cachedRotation, CachedTransform.rotation) >= 0.1f)
            {
                needRebuildBounds = true;
                cachedRotation = CachedTransform.rotation;
            }
        }

        public void OnDrawGizmos()
        {
            DrawGizmo(false);
        }
        public void OnDrawGizmosSelected()
        {
            DrawGizmo(true);
        }

#if UNITY_EDITOR
        static Texture2D icon;
        public static Texture2D Icon
        {
            get
            {
                if (icon == null)
                    icon = Resources.Load<Texture2D>("Knife.DecalIcon");

                return icon;
            }
        }
#endif
    }
}