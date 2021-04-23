using UnityEngine;

namespace Knife.ImplementationExample
{
    public class Hands : MonoBehaviour
    {
        public Weapon[] Weapons;
        public Camera Cam;
        public KeyCode[] Keys;

        float startFov;

        void Start()
        {
            startFov = Cam.fieldOfView;
            for (int i = 0; i < Weapons.Length; i++)
            {
                Weapons[i].gameObject.SetActive(false);
            }
        }

        void Update()
        {
            for (int i = 0; i < Keys.Length; i++)
            {
                if (Input.GetKeyDown(Keys[i]))
                {
                    Weapons[i].gameObject.SetActive(!Weapons[i].gameObject.activeSelf);
                    for (int j = 0; j < Weapons.Length; j++)
                    {
                        if (i == j)
                            continue;

                        Weapons[j].gameObject.SetActive(false);
                    }
                    break;
                }
            }

            foreach (Weapon weapon in Weapons)
            {
                if (weapon.gameObject.activeSelf)
                {
                    Cam.fieldOfView = weapon.CurrentFov;
                    return;
                }
            }
            Cam.fieldOfView = startFov;
        }
    }
}