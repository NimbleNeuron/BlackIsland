﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coffee.UIParticleExtensions
{
    internal static class SpriteExtensions
    {
#if UNITY_EDITOR
        private static Type tSpriteEditorExtension = Type.GetType("UnityEditor.Experimental.U2D.SpriteEditorExtension, UnityEditor")
                                                     ?? Type.GetType("UnityEditor.U2D.SpriteEditorExtension, UnityEditor");

        private static MethodInfo miGetActiveAtlasTexture = tSpriteEditorExtension
            .GetMethod("GetActiveAtlasTexture", BindingFlags.Static | BindingFlags.NonPublic);

        public static Texture2D GetActualTexture(this Sprite self)
        {
            if (!self) return null;

            if (Application.isPlaying) return self.texture;
            var ret = miGetActiveAtlasTexture.Invoke(null, new[] {self}) as Texture2D;
            return ret ? ret : self.texture;
        }
#else
        internal static Texture2D GetActualTexture(this Sprite self)
        {
            return self ? self.texture : null;
        }
#endif
    }

    internal static class ListExtensions
    {
        public static bool SequenceEqualFast(this List<bool> self, List<bool> value)
        {
            if (self.Count != value.Count) return false;
            for (var i = 0; i < self.Count; ++i)
            {
                if (self[i] != value[i]) return false;
            }

            return true;
        }

        public static int CountFast(this List<bool> self)
        {
            var count = 0;
            for (var i = 0; i < self.Count; ++i)
            {
                if (self[i]) count++;
            }

            return count;
        }

        public static bool AnyFast<T>(this List<T> self) where T : Object
        {
            for (var i = 0; i < self.Count; ++i)
            {
                if (self[i]) return true;
            }

            return false;
        }
    }

    internal static class MeshExtensions
    {
        static readonly List<Color32> s_Colors = new List<Color32>();

        public static void ModifyColorSpaceToLinear(this Mesh self)
        {
            self.GetColors(s_Colors);

            for (var i = 0; i < s_Colors.Count; i++)
                s_Colors[i] = ((Color) s_Colors[i]).gamma;

            self.SetColors(s_Colors);
            s_Colors.Clear();
        }

        public static void Clear(this CombineInstance[] self)
        {
            for (var i = 0; i < self.Length; i++)
            {
                MeshPool.Return(self[i].mesh);
                self[i].mesh = null;
            }
        }
    }

    internal static class MeshPool
    {
        private static readonly Stack<Mesh> s_Pool = new Stack<Mesh>();

        public static void Init()
        {
        }

        static MeshPool()
        {
            for (var i = 0; i < 32; i++)
            {
                var m = new Mesh();
                m.MarkDynamic();
                s_Pool.Push(m);
            }
        }

        public static Mesh Rent()
        {
            Mesh m;
            while (0 < s_Pool.Count)
            {
                m = s_Pool.Pop();
                if (m) return m;
            }

            m = new Mesh();
            m.MarkDynamic();
            return m;
        }

        public static void Return(Mesh mesh)
        {
            if (!mesh || s_Pool.Contains(mesh)) return;
            mesh.Clear(false);
            s_Pool.Push(mesh);
        }
    }

    internal static class CombineInstanceArrayPool
    {
        private static readonly Dictionary<int, CombineInstance[]> s_Pool;

        public static void Init()
        {
            s_Pool.Clear();
        }

        static CombineInstanceArrayPool()
        {
            s_Pool = new Dictionary<int, CombineInstance[]>();
        }

        public static CombineInstance[] Get(List<CombineInstance> src)
        {
            CombineInstance[] dst;
            var count = src.Count;
            if (!s_Pool.TryGetValue(count, out dst))
            {
                dst = new CombineInstance[count];
                s_Pool.Add(count, dst);
            }

            for (var i = 0; i < src.Count; i++)
            {
                dst[i].mesh = src[i].mesh;
                dst[i].transform = src[i].transform;
            }

            return dst;
        }

        public static CombineInstance[] Get(List<CombineInstanceEx> src, int count)
        {
            CombineInstance[] dst;
            if (!s_Pool.TryGetValue(count, out dst))
            {
                dst = new CombineInstance[count];
                s_Pool.Add(count, dst);
            }

            for (var i = 0; i < count; i++)
            {
                dst[i].mesh = src[i].mesh;
                dst[i].transform = src[i].transform;
            }

            return dst;
        }
    }

    internal static class ParticleSystemExtensions
    {
        public static void SortForRendering(this List<ParticleSystem> self, Transform transform)
        {
            self.Sort((a, b) =>
            {
                var tr = transform;
                var ra = a.GetComponent<ParticleSystemRenderer>();
                var rb = b.GetComponent<ParticleSystemRenderer>();

                if (!Mathf.Approximately(ra.sortingFudge, rb.sortingFudge))
                    return ra.sortingFudge < rb.sortingFudge ? 1 : -1;

                var pa = tr.InverseTransformPoint(a.transform.position).z;
                var pb = tr.InverseTransformPoint(b.transform.position).z;

                if (!Mathf.Approximately(pa, pb))
                    return pa < pb ? 1 : -1;

                var aQueue = ra.sharedMaterial.renderQueue;
                var bQueue = rb.sharedMaterial.renderQueue;
                if (aQueue != bQueue)
                    return aQueue < bQueue ? 1 : -1;

                var aHash = ra.sharedMaterial.GetHashCode();
                var bHash = rb.sharedMaterial.GetHashCode();
                if (aHash != bHash)
                    return aHash < bHash ? 1 : -1;

                return 0;
            });
        }

        public static long GetMaterialHash(this ParticleSystem self, bool trail)
        {
            if (!self) return 0;

            var r = self.GetComponent<ParticleSystemRenderer>();
            var mat = trail ? r.trailMaterial : r.sharedMaterial;

            if (!mat) return 0;

            var tex = trail ? null : self.GetTextureForSprite();
            return ((long) mat.GetHashCode() << 32) + (tex ? tex.GetHashCode() : 0);
        }

        public static Texture2D GetTextureForSprite(this ParticleSystem self)
        {
            if (!self) return null;

            // Get sprite's texture.
            var tsaModule = self.textureSheetAnimation;
            if (!tsaModule.enabled || tsaModule.mode != ParticleSystemAnimationMode.Sprites) return null;

            for (var i = 0; i < tsaModule.spriteCount; i++)
            {
                var sprite = tsaModule.GetSprite(i);
                if (!sprite) continue;

                return sprite.GetActualTexture();
            }

            return null;
        }

        public static void Exec(this List<ParticleSystem> self, Action<ParticleSystem> action)
        {
            self.RemoveAll(p => !p);
            self.ForEach(action);
        }
    }
}
