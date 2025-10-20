using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ChunkTrigger : MonoBehaviour
{
    private static ChunkLoader staticLoader;
    [SerializeField]
    private ChunkLoader chunkLoader;
    [SerializeField]
    private GameObject[] loads;
    [SerializeField]
    private GameObject[] removes;
    [SerializeField]
    private UnityEvent action;

    private void Start()
    {
        if (staticLoader == null)
            staticLoader = chunkLoader;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            staticLoader.LoadAndRemove(removes, loads);
            action.Invoke();
        }
    }
}
