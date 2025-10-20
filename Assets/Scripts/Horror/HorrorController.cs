using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HorrorController : MonoBehaviour
{
    [Header("General")]
    public static HorrorController main;
    public Action currAction;

    [SerializeField]
    private Transform map;
    [SerializeField]
    private Transform cam;

    private float lookAngle;
    private float deltaTime;

    [Header("Creatures")]
    [SerializeField]
    private Material creatureMat;
    [SerializeField]
    private Material eyeMat;

    // Human In front
    [Header("Human Infront")]
    [SerializeField]
    private Sprite[] humanSprites;
    [SerializeField]
    private float humanSpawnRate;
    [NonReorderable]
    public bool interiorLight = false;
    private float humanTimer;

    // Animal In front
    [Header("Animal Infront")]
    [SerializeField]
    private Sprite[] animalSprites;
    [SerializeField]
    private Sprite[] eyeSprites;
    [SerializeField]
    private float animalSpawnRate;
    [NonReorderable]
    public bool frontLight = false;
    private float animalTimer;

    [Header("Hand")]
    [SerializeField]
    private Transform hand;
    [SerializeField]
    private Transform closeHand;
    private float handTimer;

    [Header("Copilot")]
    [SerializeField]
    private SpriteRenderer copilot;
    [SerializeField]
    private Sprite[] coSprites;

    [Header("Statue")]
    [SerializeField]
    private SpriteRenderer statue;
    [SerializeField]
    private Sprite[] statueSprites;
    [SerializeField]
    private Material statueMat;
    [SerializeField]
    [ColorUsageAttribute(true, true)]
    private Color statueColor;
    private float statueBrightness = 0f;
    private int statueIndex = 1;

    [Header("Phone")]
    [SerializeField]
    private string[] phoneLogs;
    [SerializeField]
    private float[] logTime0;
    [SerializeField]
    private float[] logTime1;
    [SerializeField]
    private float[] logTime2;
    private float[][] logTimes;
    private int logIndex = 0;

    [SerializeField]
    private bool testPlay;

    private void Start()
    {
        logTimes = new float[][]
        {
            logTime0,
            logTime1,
            logTime2
        };
        main = this;
        currAction = null;

        interiorLight = true;
    }

    private void Update()
    {
        lookAngle = cam.rotation.eulerAngles.y;
        deltaTime = Time.deltaTime;

        currAction?.Invoke();

        if(testPlay)
        {
            Phone();
            testPlay = false;
        }

        if(statueBrightness > 0f)
        {
            statueBrightness -= Time.deltaTime;
            statueMat.SetColor("_EmissionColor", Color.Lerp(Color.black, statueColor, statueBrightness));
        }
    }


    public void HumanAnimalDeactive()
    {
        currAction = null;
        PostProccesController.main.SetLocalVolume(0f, -1f);
    }
    public void HumanActive()
    {
        currAction = HumanInfront;
        PostProccesController.main.SetHumanPP();
    }
    public void HumanInfront()
    {
        if(!interiorLight)
        {
            if(humanTimer > 0f)
                humanTimer -= deltaTime;
        }
        else
        {
            humanTimer += deltaTime;
            if (humanTimer > 1 + humanSpawnRate)
            {
                SpawnCreature(humanSprites[UnityEngine.Random.Range(0, humanSprites.Length)]);
                humanTimer = 1f;
            }
        }

        if (humanTimer > 0f)
            SetHumanEffects(humanTimer);

    }
    private void SetHumanEffects(float volume)
    {
        volume = Mathf.Min(1f, volume);
        PostProccesController.main.SetLocalVolume(volume, -1f);
    }

    public void AnimalActive()
    {
        currAction = AnimalInfront;
        PostProccesController.main.SetAnimalPP();
    }
    public void AnimalInfront()
    {
        if (frontLight)
        {
            if (animalTimer > 0f)
                animalTimer -= deltaTime;

        }
        else
        {
            animalTimer += deltaTime;
            if (animalTimer > 5 + animalSpawnRate)
            {
                int rand = UnityEngine.Random.Range(0, animalSprites.Length);
                AddEyes(eyeSprites[rand], SpawnCreature(animalSprites[rand]));
                animalTimer = 5f;
            }
        }

        if (animalTimer > 0f)
            SetAnimalEffects(animalTimer * .2f);

    }
    private void SetAnimalEffects(float volume)
    {
        volume = Mathf.Min(1f, volume);
        PostProccesController.main.SetLocalVolume(volume, -1f);
    }

    private Transform SpawnCreature(Sprite sprite)
    {
        Transform creature = new GameObject("creature").transform;
        creature.position = cam.position + Vector3.forward * UnityEngine.Random.Range(45f, 65f) + Vector3.right * UnityEngine.Random.Range(-12f, 12f);
        if (creature.position.x < 0) creature.localScale = new Vector3(-1f, 1f, 1f);
        creature.SetParent(map, true);

        creature.gameObject.AddComponent<SpriteRenderer>().sprite = sprite;
        creature.GetComponent<SpriteRenderer>().material = creatureMat;
        creature.gameObject.AddComponent<Creature>();
        creature.gameObject.AddComponent<BoxCollider>().size = Vector3.one;
        creature.gameObject.GetComponent<BoxCollider>().isTrigger = true;

        return creature;
    }

    private void AddEyes(Sprite sprite, Transform parent)
    {
        Transform eyes = new GameObject("eyes").transform;
        eyes.SetParent(parent, true);
        eyes.localPosition = new();
        eyes.localRotation = new();
        eyes.localScale = Vector3.one;

        eyes.gameObject.AddComponent<SpriteRenderer>().sprite = sprite;
        eyes.GetComponent<SpriteRenderer>().material = eyeMat;
    }

    public void HandActivated()
    {
        currAction = HandActive;
        SoundController.main.StartHand();
    }
    public void HandActive()
    {
        float looking = Vector3.Dot(cam.forward, Vector3.left);
        if (looking < .5f)
            handTimer += Time.deltaTime;
        else if (handTimer > 0f)
            handTimer -= Time.deltaTime * 20f;
        else if (handTimer > -3f)
            handTimer -= Time.deltaTime;
        else
        {
            HandDeactive();
        }
        hand.localPosition = new Vector3(-1f, 1.1f + handTimer * .015f, 0.6f);


        SoundController.main.HandVolume(Mathf.Max(.5f, handTimer));
        if (handTimer > .6f)
        {

            if(closeHand.localPosition.x > -.44f)
            {
                PostProccesController.main.uiText.text = "You died. Check left window next time.\n(Click to Restart)";
                PostProccesController.main.SetDiePP(1f);
                SoundController.main.NoBreath();

            }
            else
            {
                SoundController.main.HandVolume(0.5f);
                if (!closeHand.gameObject.active)
                    closeHand.gameObject.SetActive(true);
                closeHand.localPosition = new(-0.62f + (handTimer - 30f) * .07f, 1.5909f, 0.0406f);
            }
        }
        else if(closeHand.gameObject.active)
            closeHand.gameObject.SetActive(false);

        float volume = (looking + 1) * handTimer * .02f;
        volume = Mathf.Min(1f, volume);
        PostProccesController.main.SetLocalVolume(volume, -1f);

    }
    public void HandDeactive()
    {
        handTimer = -5f;
        currAction = null;
        SoundController.main.StopHand();
    }

    public void CopilotActivate()
    {
        copilot.enabled = true;
        copilot.sprite = coSprites[0];
        SoundController.main.DoorOpen();
    }
    public void CopilotDe()
    {
        float looking = Vector3.Dot(cam.forward, Vector3.right);
        if(looking > .6f)
        {
            Destroy(copilot.gameObject, LightningController.main.CallImmThunder());
            PostProccesController.main.SetLocalVolume(0f, -1f);
            currAction = null;
        }
    }
    public void CopilotDeactivate()
    {
        currAction = CopilotDe;
        copilot.sprite = coSprites[1];

        PostProccesController.main.SetCoPilotPP();
        PostProccesController.main.SetLocalVolume(1f, .01f);
    }


    public void Phone()
    {
        SoundController.main.Walkie_Talkie();
        DialogController.main.Read(phoneLogs[logIndex], logTimes[logIndex]);
        logIndex++;
    }

    public void Statue()
    {
        statue.sprite = statueSprites[statueIndex];
        statueBrightness = 1f;
        statueIndex++;
    }

    public void Crush()
    {
        PostProccesController.main.SetDiePP(1f);
    }



    public void Finish()
    {
        PostProccesController.main.uiText.text = "Thank you for playing the demo :)" +
            "" +
            "" +
            "" +
            "" +
            "" +
            "" +
            "" +
            "\n(Click to restart)";
        PostProccesController.main.SetDiePP(1f);
    }

}
