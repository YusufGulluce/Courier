using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour
{
    public static SoundController main;

    [Header("Motor")]
    [SerializeField]
    private AudioSource motorSource;
    [SerializeField]
    private float minPitch;
    [SerializeField]
    private float accPitch;
    [SerializeField]
    private float velPitch;
    [SerializeField]
    private Car car;
    private float motorPitch = .8f;

    [Header("Rain")]
    [SerializeField]
    private AudioSource rainSource;
    [SerializeField]
    private WeatherController weatherController;
    [SerializeField]
    private float accRainVol;

    [Header("Thunder")]
    [SerializeField]
    private AudioSource[] thunderSources;


    [Header("Interior")]
    [SerializeField]
    private AudioSource interiorSource;
    [SerializeField]
    private AudioClip[] gearAudios;
    [SerializeField]
    private AudioClip[] armAudios;
    [SerializeField]
    private AudioClip[] buttonAudios;


    [Header("Hand")]
    [SerializeField]
    private AudioSource handSource;

    [Header("Door")]
    [SerializeField]
    private AudioSource doorSource;
    private float doorTimer = 0f;

    [Header("Whistle")]
    [SerializeField]
    private AudioSource whistleSource;

    [Header("Radio")]
    [SerializeField]
    private AudioSource radioSource;
    [SerializeField]
    private AudioSource radioWhistleSource;

    [Header("Phone")]
    [SerializeField]
    private AudioSource phoneSource;
    [SerializeField]
    private AudioClip[] phoneClips;
    private int phoneIndex = 0;
    private bool wasRadio;

    [Header("Crash")]
    [SerializeField]
    private AudioSource crashSource;
    [SerializeField]
    private AudioClip[] crashClips;


    [Header("Hit")]
    [SerializeField]
    private AudioSource hitSource;
    [SerializeField]
    private AudioClip[] hitClips;


    [Header("Breath")]
    [SerializeField]
    private AudioSource breathSource;


    private void Start()
    {
        main = this;
    }
    private void Update()
    {
        MotorSoundUpdate();
        RainSoundUpdate();

        if(doorTimer > 0f)
        {
            doorTimer -= Time.deltaTime;
            if (doorTimer <= 0f)
                rainSource.pitch = .77f;
            else if (doorTimer <= 1f)
            {
                float sin = Mathf.Sin(doorTimer * Mathf.PI);
                rainSource.pitch = .77f + sin * sin;
            }
        }
    }

    private void MotorSoundUpdate()
    {
        motorPitch = minPitch + Mathf.Abs(car.mapRB.velocity.z) * velPitch + accPitch * car.velocity.z;
        if(car.velocity.z > 0f)
            car.velocity.z -= 0.01f * Time.deltaTime;
        motorSource.pitch = motorPitch;
    }

    private void RainSoundUpdate()
    {
        rainSource.volume = Mathf.Min(1f, weatherController.rain * accRainVol);
    }


    public void Thunder()
    {
        AudioSource s = thunderSources[Random.Range(0, 2)];
        s.PlayDelayed(Random.Range(.3f, 7.8f));
    }
    public void ImmThunder()
    {
        AudioSource s = thunderSources[Random.Range(0, 2)];
        s.Play();
    }

    public void Gear()
    {
        interiorSource.clip = gearAudios[Random.Range(0, gearAudios.Length)];
        interiorSource.Play();
    }

    public void Button()
    {
        interiorSource.clip = buttonAudios[Random.Range(0, buttonAudios.Length)];
        interiorSource.Play();
    }

    public void Arm()
    {
        interiorSource.clip = armAudios[Random.Range(0, armAudios.Length)];
        interiorSource.Play();
    }


    public void Whistle()
    {
        if (!whistleSource.isPlaying)
            whistleSource.Play();
        else
            whistleSource.Stop();
    }

    public void Radio()
    {
        if(!phoneSource.isPlaying)
        {
            if (!radioWhistleSource.isPlaying && !radioSource.isPlaying && Random.Range(0, 5) == 0)
                radioWhistleSource.Play();
            else if (radioWhistleSource.isPlaying)
                radioWhistleSource.Pause();

            else if (!radioSource.isPlaying)
                radioSource.PlayDelayed(.5f);
            else
                radioSource.Pause();
        }
    }

    public void StartHand()
    {
        handSource.Play();
    }
    public void HandVolume(float volume)
    {
        handSource.volume = volume;
    }
    public void StopHand()
    {
        handSource.Stop();
    }

    public void DoorOpen()
    {
        doorTimer = 1.9f;
        doorSource.Play();
    }


    public void Walkie_Talkie()
    {
        phoneSource.clip = phoneClips[phoneIndex];
        phoneSource.Play();

        phoneIndex++;

        wasRadio = radioWhistleSource.isPlaying | radioSource.isPlaying;
        if (radioWhistleSource.isPlaying)
            radioWhistleSource.Pause();
        else if (radioSource.isPlaying)
            radioSource.Pause();

    }

    public void Crash(float volume)
    {
        crashSource.clip = crashClips[Random.Range(0, crashClips.Length)];
        crashSource.volume = volume;
        crashSource.Play();
    }

    public void Hit()
    {
        hitSource.clip = hitClips[Random.Range(0, hitClips.Length)];
        hitSource.Play();
    }

    public void NoBreath()
    {
        breathSource.Play();
    }

    public void RadioBack()
    {
        if(wasRadio)
            radioSource.Play();
        phoneSource.Stop();
    }
}
