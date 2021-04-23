using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    public float Speed = 3f;

    private void OnEnable()
    {
        GetComponent<Rigidbody>().velocity = transform.up * Speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
