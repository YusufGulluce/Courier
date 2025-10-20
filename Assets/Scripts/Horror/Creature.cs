using UnityEngine;
using System.Collections;

public class Creature : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 30f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            WindowController.main.MarkWindow();
            Destroy(gameObject);
            SoundController.main.Hit();
        }
    }
}
