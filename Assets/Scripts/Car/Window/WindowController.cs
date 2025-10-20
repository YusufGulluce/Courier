using UnityEngine;
using System.Collections;

public class WindowController : MonoBehaviour
{
    [SerializeField]
    public static WindowController main;
    [SerializeField]
    private Transform[] windows;
    [SerializeField]
    private Camera[] windowCams;
    private Camera mainCam;

    [SerializeField]
    private Material windowMat;
    [SerializeField]
    private Material windowLeft;
    [SerializeField]
    private Material windowRight;
    [SerializeField]
    private Material windowBack;
    private float mult;
    private float scale;
    public float scaleAcc;
    private float markRise;
    private float lastTime;

    private float oldSin;
    [SerializeField]
    private bool mark;

    private bool waitForCleaner = true;
    private float cleaner;

    [SerializeField]
    private Transform stick;

    private void Start()
    {
        main = this;
        mainCam = Camera.main;
        mult = 0f;
        scale = 0f;
        scaleAcc = .05f;
        oldSin = 2f;
        cleaner = 0f;
        markRise = 1f;
        lastTime = 0f;
        SetMat(windowMat);
        SetMat(windowLeft);
        SetMat(windowRight);
        SetMat(windowBack);
    }

    private void SetMat(Material mat)
    {
        mat.SetFloat("_Size", 8f);
        mat.SetFloat("_Disortion", -4f);
        mat.SetFloat("_ChunkSizeX", 2f);
        mat.SetFloat("_ChunkSizeY", 2f);
    }

    private void Update()
    {
        if(mark)
        {
            MarkWindow();
            mark = false;
        }
        for(int i = 0; i < windows.Length; ++i)
        {
            Transform window = windows[i];
            Camera windowCam = windowCams[i];

            Plane p = new(window.forward, window.position);
            Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
            Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(windowCam.worldToCameraMatrix)) * clipPlaneWorldSpace;
            var newMatrix = mainCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
            windowCam.projectionMatrix = newMatrix;
        }
        //windowCam.nearClipPlane = .3f;

        if(cleaner > 0)
            lastTime += Time.deltaTime;
        windowMat.SetFloat("_CleanerCos", Mathf.Cos(Mathf.PI * Mathf.Sin(lastTime * .5f)));
        windowMat.SetFloat("_TimeSin", Mathf.Sin(lastTime));

        stick.localEulerAngles = new(stick.localEulerAngles.x, stick.localEulerAngles.y, 90f - Mathf.Cos(Mathf.PI * Mathf.Sin(lastTime * .5f)) * 90f);
        if(scale > 0f && cleaner > 0f)
            scale -= Time.deltaTime * scaleAcc;
        if (markRise < 1f)
        {
            markRise += Time.deltaTime * 8f;
            windowMat.SetFloat("_MarkRise", markRise);
        }
        windowMat.SetFloat("_MarkScale", scale);

        if (oldSin * Mathf.Sin(lastTime) < 0)
        {
            if (mult > 0)
            {
                mult -= .1f;
                windowMat.SetFloat("_MarkMult", mult);
            }
        }

        if (oldSin != 2f)
            oldSin = Mathf.Sin(lastTime);

        if (waitForCleaner && cleaner > 0 && Mathf.Cos(Mathf.PI * Mathf.Sin(lastTime * .5f)) < -.99f)
        {
            cleaner = Mathf.Abs(cleaner - 1f);
            windowMat.SetFloat("_Clean", cleaner);
        }

    }

    public void MarkWindow()
    {
        mult = 1f;
        scale = 1f;
        oldSin = 1f;
        markRise = 0f;
        windowMat.SetFloat("_MarkScale", scale);
        windowMat.SetFloat("_MarkMult", mult);
    }

    public void Cleaner()
    {
        SoundController.main.Arm();
        waitForCleaner = !waitForCleaner;
        if(!waitForCleaner)
        {
            cleaner = Mathf.Abs(cleaner - 1f);
            windowMat.SetFloat("_Clean", cleaner);
        }
    }
}
