using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CarCam : MonoBehaviour
{
    [SerializeField]
    private RenderTexture rt;
    [SerializeField]
    private Camera cam;
    void Start()
    {
        cam.targetTexture = rt;
    }
}
