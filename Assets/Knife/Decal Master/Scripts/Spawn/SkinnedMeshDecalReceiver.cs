using Knife.Tools;
using UnityEngine;

namespace Knife.DeferredDecals.Spawn
{
    public class SkinnedMeshDecalReceiver : SimpleDecalReceiver
    {
        SkinnedMeshRenderer skinnedMeshRenderer;

        BoneWeight[] boneWeights;
        Mesh mesh;

        protected override void OnHitted(Decal decalInstance, GPURaycastDecalsTargetInfo hitInfo)
        {
            skinnedMeshRenderer = hitInfo.hittedRenderer as SkinnedMeshRenderer;
            if (skinnedMeshRenderer == null)
                return;

            mesh = skinnedMeshRenderer.sharedMesh;
            boneWeights = mesh.boneWeights;
            if (ParentOnHit)
            {
                var boneWeight = boneWeights[hitInfo.VertexIndex];

                int index0 = boneWeight.boneIndex0;
                int index1 = boneWeight.boneIndex1;
                int index2 = boneWeight.boneIndex2;
                int index3 = boneWeight.boneIndex3;

                float w0 = boneWeight.weight0;
                float w1 = boneWeight.weight1;
                float w2 = boneWeight.weight2;
                float w3 = boneWeight.weight3;

                float maxWeight = Mathf.Max(w0, w1, w2, w3);
                int maxBoneIndex = -1;

                if (maxWeight == w0)
                {
                    maxBoneIndex = boneWeight.boneIndex0;
                }
                if (maxWeight == w1)
                {
                    maxBoneIndex = boneWeight.boneIndex1;
                }
                if (maxWeight == w2)
                {
                    maxBoneIndex = boneWeight.boneIndex2;
                }
                if (maxWeight == w3)
                {
                    maxBoneIndex = boneWeight.boneIndex3;
                }

                decalInstance.transform.SetParent(skinnedMeshRenderer.bones[maxBoneIndex]);
            }
        }
    }
}