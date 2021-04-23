using UnityEngine;
using Knife.Tools;

namespace Knife.ImplementationExample
{
    public class Hittable : MonoBehaviour
    {
        public float Health = 25f;

        public void TakeDamage(DamageDataPhysics damage)
        {
            Health -= damage.DamageAmount;

            if (Health <= 0)
                Destroy(gameObject);
        }

        public void TakeDamage(DamageDataGPURaycast damage)
        {
            Health -= damage.DamageAmount;

            if (Health <= 0)
                Destroy(gameObject);
        }
    }

    public struct DamageDataPhysics
    {
        public float DamageAmount;
        public RaycastHit HitInfo;
    }

    public struct DamageDataGPURaycast
    {
        public float DamageAmount;
        public GPURaycastDecalsTargetInfo HitInfo;
    }
}