using Knife.Tools;
using UnityEngine;

namespace Knife.DeferredDecals.Spawn
{
    public class SimpleDecalReceiver : MonoBehaviour, IDecalReceiver
    {
        public DecalSpawnController SpawnController;
        
        public bool ParentOnHit;
        public float Offset = 0.15f;
        [Range(0f, 1f)]
        public float RotationJitter;

        public void HittedByGPURaycast(GPURaycastDecalsTargetInfo hitInfo)
        {
            var decalInstance = SpawnController.SpawnDecal();
            var decalTransform = decalInstance.transform;

            ApplyDecalTransform(decalTransform, hitInfo);

            OnHitted(decalInstance, hitInfo);
        }

        public void HittedByPhysicsRaycast(RaycastHit hitInfo)
        {
            var decalInstance = SpawnController.SpawnDecal();
            var decalTransform = decalInstance.transform;

            ApplyDecalTransform(decalTransform, hitInfo);

            OnHitted(decalInstance, hitInfo);
        }

        public void HittedByParticle(ParticleCollisionEvent collisionEvent)
        {
            var decalInstance = SpawnController.SpawnDecal();
            var decalTransform = decalInstance.transform;

            ApplyDecalTransform(decalTransform, collisionEvent);

            OnHitted(decalInstance, collisionEvent);
        }

        public void HittedByPhysicsCollision(Collision collision)
        {
            var decalInstance = SpawnController.SpawnDecal();
            var decalTransform = decalInstance.transform;

            ApplyDecalTransform(decalTransform, collision);

            OnHitted(decalInstance, collision);
        }

        void ApplyDecalTransform(Transform decalTransform, GPURaycastDecalsTargetInfo hitInfo)
        {
            decalTransform.position = hitInfo.position + hitInfo.normal * Offset;
            ApplyDecalRotationByNormal(decalTransform, hitInfo.normal);

            if (ParentOnHit)
            {
                decalTransform.SetParent(hitInfo.hittedRenderer.transform);
            }
        }

        void ApplyDecalTransform(Transform decalTransform, RaycastHit hitInfo)
        {
            decalTransform.position = hitInfo.point + hitInfo.normal * Offset;
            ApplyDecalRotationByNormal(decalTransform, hitInfo.normal);

            if (ParentOnHit)
            {
                decalTransform.SetParent(hitInfo.transform);
            }
        }

        void ApplyDecalTransform(Transform decalTransform, ParticleCollisionEvent collisionEvent)
        {
            decalTransform.position = collisionEvent.intersection + collisionEvent.normal * Offset;
            ApplyDecalRotationByNormal(decalTransform, collisionEvent.normal);

            if (ParentOnHit)
            {
                decalTransform.SetParent(collisionEvent.colliderComponent.transform);
            }
        }

        void ApplyDecalTransform(Transform decalTransform, Collision collision)
        {
            var contact = collision.contacts[0];
            decalTransform.position = contact.point + contact.normal * Offset;
            ApplyDecalRotationByNormal(decalTransform, contact.normal);

            if (ParentOnHit)
            {
                decalTransform.SetParent(collision.transform);
            }
        }

        void ApplyDecalRotationByNormal(Transform decalTransform, Vector3 normal)
        {
            decalTransform.up = normal;
            decalTransform.rotation *= Quaternion.AngleAxis(Random.value * 360f * RotationJitter, Vector3.up);
        }

        protected virtual void OnHitted(Decal decalInstance, GPURaycastDecalsTargetInfo hitInfo)
        {

        }

        protected virtual void OnHitted(Decal decalInstance, RaycastHit hitInfo)
        {

        }

        protected virtual void OnHitted(Decal decalInstance, ParticleCollisionEvent collisionEvent)
        {

        }

        protected virtual void OnHitted(Decal decalInstance, Collision collision)
        {

        }
    }
}