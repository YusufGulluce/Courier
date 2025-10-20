using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayColldier : MonoBehaviour
{

    [SerializeField]
    public Rigidbody mapRB;
    private void OnCollisionEnter(Collision collision)
    {
        SoundController.main.Crash(Mathf.Abs(mapRB.velocity.z * .5f));
        if(Mathf.Abs(mapRB.velocity.z) > 1.5f)
            HorrorController.main.Crush();
    }
}
