using Knife.DeferredDecals.Spawn;
using Knife.Tools;
using UnityEngine;

namespace Knife.ImplementationExample
{
    public class Weapon : MonoBehaviour
    {
        public LayerMask ShotMask;
        public GameObject ProjectilePrefab;
        public float Damage = 10f;

        public float DefaultFov = 60f;
        public float AimFov = 35f;
        public bool UseGPURaycast = false;

        public Animator handsAnimator;

        bool isAiming = false;

        float currentFov;

        public float CurrentFov
        {
            get
            {
                return currentFov;
            }
        }

        void Start()
        {
            if(handsAnimator == null)
                handsAnimator = GetComponent<Animator>();
        }

        private void OnDisable()
        {
            currentFov = DefaultFov;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                shot(0);
            }
            if (Input.GetMouseButtonDown(2))
            {
                shot(2);
            }

            if (Input.GetMouseButtonDown(1))
            {
                isAiming = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                isAiming = false;
            }

            currentFov = Mathf.Lerp(currentFov, isAiming ? AimFov : DefaultFov, Time.deltaTime * 12f);
        }

        protected virtual void shot(int button)
        {
            if (button == 0)
            {
                handsAnimator.Play("Shot", 0, 0);
                if (UseGPURaycast)
                {
                    GPURaycastDecalsTargetInfo hitInfo;
                    if(GPURaycast.RaycastToRegisteredTargets(Camera.main, Vector2.one / 2, out hitInfo))
                    {
                        var hittable = hitInfo.hittedRenderer.GetComponent<Hittable>();
                        if (hittable != null)
                        {
                            DamageDataGPURaycast damage = new DamageDataGPURaycast()
                            {
                                DamageAmount = Damage,
                                HitInfo = hitInfo
                            };

                            hittable.TakeDamage(damage);
                        }

                        DecalReceiverHelper.SendGPURaycastInfo(hitInfo);
                    }
                }
                else
                {
                    Ray r = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(r, out hitInfo, 1000, ShotMask))
                    {
                        var hittable = hitInfo.collider.GetComponent<Hittable>();
                        if (hittable != null)
                        {
                            DamageDataPhysics damage = new DamageDataPhysics()
                            {
                                DamageAmount = Damage,
                                HitInfo = hitInfo
                            };

                            hittable.TakeDamage(damage);
                        }

                        DecalReceiverHelper.SendPhysicsRaycastInfo(hitInfo);
                    }
                }
            } else if(button == 2)
            {
                handsAnimator.Play("Shot", 0, 0);
                GameObject projectileInstance = Instantiate(ProjectilePrefab, ProjectilePrefab.transform.position, ProjectilePrefab.transform.rotation);
                projectileInstance.transform.SetParent(null);
                projectileInstance.gameObject.SetActive(true);
            }
        }
    }
}