using UnityEngine;
using System.Collections;

public class InteractionController : MonoBehaviour
{
    [Header("Interior Light")]
    [SerializeField]
    private Light interiorLight;
    [SerializeField]
    private Material interiorLightMat;
    [SerializeField]
    [ColorUsageAttribute(true, true)]
    private Color interiorLightColor;

    [Header("Front Lights")]
    [SerializeField]
    private Light[] frontLights;
    private bool far = false;

    [Header("Cleaner")]
    [SerializeField]
    private Material window;
    private bool cleanerOpen = false;


    [Header("Gear")]
    [SerializeField]
    private Car car;


    public void InteriorLight()
    {
        interiorLight.enabled = !interiorLight.enabled;
        interiorLightMat.SetColor("_EmissionColor", interiorLight.enabled ? interiorLightColor : Color.black);

        SoundController.main.Button();
        HorrorController.main.interiorLight = interiorLight.enabled;
    }

    public void GearShift()
    {
        car.gear *= -1;
        SoundController.main.Gear();
    }

    public void FrontLights()
    {
        far = !far;
        if (far)
            foreach (Light l in frontLights)
                l.intensity = 1000f;
        else
            foreach (Light l in frontLights)
                l.intensity = 150f;

        HorrorController.main.frontLight = far;
        SoundController.main.Arm();

    }

    public void CleanerSwitch()
    {
        cleanerOpen = !cleanerOpen;
        if (cleanerOpen)
            window.SetFloat("_Clean", 1f);
        else
            window.SetFloat("_Clean", 0f);


        SoundController.main.Arm();

    }

    public void Radio()
    {
        SoundController.main.Button();
        SoundController.main.Radio();
    }

}
