using UnityEngine;
using System.Collections;

public class ChunkLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject[] chunks;

    public void LoadAndRemove(GameObject[] removes, GameObject[] loads)
    {
        foreach (GameObject obj in removes)
            obj.SetActive(false);
        foreach (GameObject obj in loads)
            obj.SetActive(true);
    }
}
