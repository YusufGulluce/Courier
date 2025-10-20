using UnityEngine;
using System.Collections;

public class TireSphere : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    private void OnCollisionEnter(Collision collision)
    {
    }

    private void OnCollisionExit(Collision collision)
    {
        rb.velocity = Vector3.zero;
    }
}
