using System.Collections.Generic;
using UnityEngine;

namespace Knife.DeferredDecals.Spawn
{
    [RequireComponent(typeof(ParticleSystem))]
    public class SpawnDecalOnParticle : MonoBehaviour
    {
        public SpawnControl SpawnControl = SpawnControl.ByReceiver;
        public DecalSpawnController SpawnController;
        public float Offset = 0.1f;
        public bool ParentOnCollide;
        [Range(0f, 1f)]
        public float RotationJitter;

        ParticleSystem attachedParticleSystem;
        List<ParticleCollisionEvent> collisions;

        private void Awake()
        {
            attachedParticleSystem = GetComponent<ParticleSystem>();
            collisions = new List<ParticleCollisionEvent>();
        }

        private void OnParticleCollision(GameObject other)
        {
            collisions.Clear();
            int count = attachedParticleSystem.GetCollisionEvents(other, collisions);

            if(SpawnControl == SpawnControl.ByReceiver)
            {
                for (int i = 0; i < count; i++)
                {
                    DecalReceiverHelper.SendParticleCollisionEvent(collisions[i]);
                }
            }
            else if(SpawnControl == SpawnControl.BySpawnController)
            {
                for (int i = 0; i < count; i++)
                {
                    var point = collisions[i].intersection;
                    var normal = collisions[i].normal;

                    var decal = SpawnController.SpawnDecal();
                    decal.transform.position = point + normal * Offset;
                    decal.transform.up = normal;
                    decal.transform.rotation *= Quaternion.AngleAxis(Random.value * 360f * RotationJitter, Vector3.up);

                    if (ParentOnCollide)
                    {
                        decal.transform.SetParent(collisions[i].colliderComponent.transform);
                    }
                }
            }
        }
    }
}