using UnityEngine;
using Knife.Tools;

namespace Knife.DeferredDecals.Spawn
{
    public static class DecalReceiverHelper
    {
        public static void SendPhysicsRaycastInfo(RaycastHit hitInfo)
        {
            IDecalReceiver decalReceiver = hitInfo.collider.GetComponent<IDecalReceiver>();
            if (decalReceiver == null)
                decalReceiver = hitInfo.collider.GetComponentInParent<IDecalReceiver>();

            if (decalReceiver != null)
                decalReceiver.HittedByPhysicsRaycast(hitInfo);
        }

        public static void SendParticleCollisionEvent(ParticleCollisionEvent collisionEvent)
        {
            IDecalReceiver decalReceiver = collisionEvent.colliderComponent.GetComponent<IDecalReceiver>();
            if (decalReceiver == null)
                decalReceiver = collisionEvent.colliderComponent.GetComponentInParent<IDecalReceiver>();

            if (decalReceiver != null)
                decalReceiver.HittedByParticle(collisionEvent);
        }

        public static void SendGPURaycastInfo(GPURaycastDecalsTargetInfo hitInfo)
        {
            IDecalReceiver decalReceiver = hitInfo.hittedRenderer.GetComponent<IDecalReceiver>();
            if (decalReceiver == null)
                decalReceiver = hitInfo.hittedRenderer.GetComponentInParent<IDecalReceiver>();

            if (decalReceiver != null)
                decalReceiver.HittedByGPURaycast(hitInfo);
        }

        public static void SendCollision(Collision collision)
        {
            IDecalReceiver decalReceiver = collision.collider.GetComponent<IDecalReceiver>();
            if (decalReceiver == null)
                decalReceiver = collision.collider.GetComponentInParent<IDecalReceiver>();

            if (decalReceiver != null)
                decalReceiver.HittedByPhysicsCollision(collision);
        }
    }
}