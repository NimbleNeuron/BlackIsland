using UnityEngine;

namespace Knife.Tools
{
    public class DecalsTarget : MonoBehaviour
    {
        public Renderer[] Renderers
        {
            get
            {
                if (myRenderers == null)
                    Awake();

                return myRenderers;
            }
        }

        Renderer[] myRenderers;

        private void Awake()
        {
            myRenderers = GetComponentsInChildren<Renderer>();
        }

        private void OnEnable()
        {
            GPURaycast.AddDecalsTarget(this);
        }

        private void OnDestroy()
        {
            GPURaycast.RemoveDecalsTarget(this);
        }
    }
}