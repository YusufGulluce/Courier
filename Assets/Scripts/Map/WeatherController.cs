using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class WeatherController : MonoBehaviour
{
    [SerializeField]
    private bool apply;
    [SerializeField]
    private float rainAcc;
    public float rain;

    [SerializeField]
    private float rainTime;

    [SerializeField]
    private Material roadMat;

    [SerializeField]
    private Material[] windows;
    [SerializeField]
    private ParticleSystem rainParticle;

    [SerializeField]
    private Rigidbody mapRb;
    [SerializeField]
    private Transform mapTr;

    private ParticleSystem.VelocityOverLifetimeModule a;

    private void Start()
    {
        a = rainParticle.velocityOverLifetime;
        rainTime = 0f;
        WeatherClear();
    }
    private void Update()
    {
        if (rainTime > 0)
        {
            float delta = Time.deltaTime;
            rainTime -= delta;
            rain += rainAcc * delta;
            Rain(rain);
            WindowController.main.scaleAcc = rain * .01f;
        }
        CarToRain();
    }
    private void Rain(float rain)
    {
        roadMat.SetFloat("_DetailAlbedoMapScale", Mathf.Min(rain * .1f, 1f));
        foreach (Material m in windows)
            m.SetFloat("_Rain", rain * 2f);
        rainParticle.emissionRate = rain * 10f;
    }
    private void CarToRain()
    {
        a = rainParticle.velocityOverLifetime;
        float degree = mapTr.rotation.eulerAngles.y * Mathf.Deg2Rad;
        float xSpeed = Mathf.Sin(degree) * mapRb.velocity.z * 10f;
        float zSpeed = Mathf.Cos(degree) * mapRb.velocity.z * 10f;
        a.x = new ParticleSystem.MinMaxCurve(xSpeed, xSpeed - 5);
        a.z = new ParticleSystem.MinMaxCurve(zSpeed, zSpeed - 5);
    }

    public void RainSlow()
    {
        rainAcc = 0.01f;
        rainTime = 50f;
    }

    public void RainFast()
    {
        rainAcc = 0.25f;
        rainTime = 50f;
    }

    public void WeatherClear()
    {
        foreach (Material m in windows)
            m.SetFloat("_Rain", 0f);
        roadMat.SetFloat("_DetailAlbedoMapScale", 0f);
        rainParticle.emissionRate = 0f;
        rain = 0f;
    }
}
