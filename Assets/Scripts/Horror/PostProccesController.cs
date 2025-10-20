using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PostProccesController : MonoBehaviour
{
    public static PostProccesController main;
    [SerializeField]
    private VolumeProfile[] profiles;
    [SerializeField]
    private Volume localVolume;
    [SerializeField]
    private Volume lifeVolume;

    [SerializeField]
    public Text uiText;
    [SerializeField]
    private Image uiPanel;

    private float lifeTimer = 4f;
    private float lifeAcc = -0.25f;
    private float weightAim = 0f;
    private bool canRestart = false;

    private float lerpAmount;
    private float weight;
    private bool doJob;
    /*
     * 0 - Human
     * 1 - Animal
     * 2 - Hand
     * 3 - Co-Pilot
     * 4 - Radio
     */

    private void Awake()
    {
        main = this;
        lifeTimer = 4f;
        lifeAcc = -0.25f;
        weightAim = 0f;
        canRestart = false;
        doJob = false;
    }

    public void SetHumanPP()
    {
        localVolume.profile = profiles[0];
    }
    public void SetAnimalPP()
    {
        localVolume.profile = profiles[1];
    }
    public void SetHandPP()
    {
        localVolume.profile = profiles[2];
    }
    public void SetCoPilotPP()
    {
        localVolume.profile = profiles[3];
    }
    public void SetRadioPP()
    {
        localVolume.profile = profiles[4];
    }

    public void SetLocalVolume(float weight, float lerpAmount)
    {
        if (lerpAmount <= 0f)
            localVolume.weight = weight;
        else
            doJob = true;

        this.weight = weight;
        this.lerpAmount = lerpAmount;

    }
    private void Update()
    {
        if(doJob)
        {
            localVolume.weight = Mathf.Lerp(localVolume.weight, weight, lerpAmount);
            if (localVolume.weight >= weight - .02f)
                doJob = false;
        }

        if(lifeTimer > 0f)
        {
            lifeTimer -= Time.deltaTime;
            lifeVolume.weight += lifeAcc * Time.deltaTime;

            if(weightAim > 0f)
                uiPanel.color = new Color(0,0,0,lifeVolume.weight);
            
            if (lifeTimer <= 0f)
            {
                if (weightAim > 0f)
                {
                    uiText.enabled = true;
                    canRestart = true;
                }
                lifeVolume.enabled = false;
                lifeAcc = 0f;
            }
        }

        if(canRestart && Input.GetMouseButtonDown(0))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void SetDiePP(float weight)
    {
        lifeVolume.enabled = true;
        weightAim = weight;
        lifeTimer = 5f;
        lifeAcc = (weightAim - lifeVolume.weight) / lifeTimer;
    }

}
