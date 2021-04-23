using UnityEngine;

namespace Knife.DeferredDecals
{
    [ExecuteAlways]
    [RequireComponent(typeof(Renderer))]
    public class CustomDecalExclusionMaskRenderer : MonoBehaviour
    {
        public int[] Submeshes;

        Renderer attachedRenderer;

        public Renderer AttachedRenderer
        {
            get
            {
                if (attachedRenderer == null)
                    attachedRenderer = GetComponent<Renderer>();

                return attachedRenderer;
            }
        }

        private void OnEnable()
        {
            if (Submeshes == null)
                return;

            CustomRendererManager.instance.AddCustomRendererID(AttachedRenderer, Submeshes);
        }

        private void OnDisable()
        {
            CustomRendererManager.instance.RemoveCustomRendererID(AttachedRenderer);
        }

        private void OnValidate()
        {
            CustomRendererManager.instance.RemoveCustomRendererID(AttachedRenderer);

            if (Submeshes == null)
                return;

            CustomRendererManager.instance.AddCustomRendererID(AttachedRenderer, Submeshes);
        }
    }
}