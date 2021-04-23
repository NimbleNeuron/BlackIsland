using Knife.Tools;
using UnityEngine;

namespace Knife.DeferredDecals.Spawn
{
    public class DecalReceiverRaycaster : MonoBehaviour
    {
        public Camera RaycastCamera;
        public bool UseGPURaycast = false;

        GPURaycastDecalsTargetInfo hitInfo;

        private void Update()
        {
            bool shift = Input.GetKey(KeyCode.LeftShift);
            if ((Input.GetMouseButtonDown(0) && !shift) || (Input.GetMouseButton(0) && shift))
            {
                if (UseGPURaycast)
                {
                    Vector2 uv = Vector2.zero;

                    uv.x = Input.mousePosition.x / RaycastCamera.pixelWidth;
                    uv.y = Input.mousePosition.y / RaycastCamera.pixelHeight;

                    bool isHitted = GPURaycast.RaycastToRegisteredTargets(RaycastCamera, uv, out hitInfo);

                    if (isHitted)
                    {
                        DecalReceiverHelper.SendGPURaycastInfo(hitInfo);
                    }
                }
                else
                {
                    Ray r = RaycastCamera.ScreenPointToRay(Input.mousePosition);

                    RaycastHit hitInfo;
                    if (Physics.Raycast(r, out hitInfo))
                    {
                        DecalReceiverHelper.SendPhysicsRaycastInfo(hitInfo);
                    }
                }
            }
        }
    }
}