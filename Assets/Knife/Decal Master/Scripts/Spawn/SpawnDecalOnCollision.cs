using UnityEngine;

namespace Knife.DeferredDecals.Spawn
{
    public class SpawnDecalOnCollision : MonoBehaviour
    {
        public SpawnControl SpawnControl = SpawnControl.ByReceiver;
        public DecalSpawnController SpawnController;
        public float Offset = 0.1f;
        public bool ParentOnCollide;
        [Range(0f, 1f)]
        public float RotationJitter;

        public bool SpawnOnCollisionEnter = true;
        public bool SpawnOnCollisionStay;
        public bool SpawnOnCollisionExit;
        [Header("Leave empty if you don't want check tag")]
        public string TagCheck = "";

        private void OnCollisionEnter(Collision collision)
        {
            if (SpawnOnCollisionEnter)
            {
                SpawnOnCollision(collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (SpawnOnCollisionExit)
            {
                SpawnOnCollision(collision);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (SpawnOnCollisionStay)
            {
                SpawnOnCollision(collision);
            }
        }

        void SpawnOnCollision(Collision collision)
        {
            if (string.IsNullOrEmpty(TagCheck) || collision.gameObject.CompareTag(TagCheck))
            {
                if(SpawnControl == SpawnControl.ByReceiver)
                {
                    DecalReceiverHelper.SendCollision(collision);
                } else if(SpawnControl == SpawnControl.BySpawnController)
                {
                    var contact = collision.contacts[0];

                    var point = contact.point;
                    var normal = contact.normal;

                    var decal = SpawnController.SpawnDecal();
                    decal.transform.position = point + normal * Offset;
                    decal.transform.up = normal;
                    decal.transform.rotation *= Quaternion.AngleAxis(Random.value * 360f * RotationJitter, Vector3.up);

                    if (ParentOnCollide)
                    {
                        decal.transform.SetParent(collision.transform);
                    }
                }
            }
        }
    }
}