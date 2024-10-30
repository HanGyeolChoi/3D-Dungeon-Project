using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    public float power = 200f;
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * power, ForceMode.Impulse);
    }
}
