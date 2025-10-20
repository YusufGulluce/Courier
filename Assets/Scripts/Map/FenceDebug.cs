using UnityEngine;
using System.Collections;

public class FenceDebug : MonoBehaviour
{
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("nword: " + collision.gameObject.name);
    }
}
