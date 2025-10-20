using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningController : MonoBehaviour
{
    public static LightningController main;
    [SerializeField]
    private Light[] lights;

    [SerializeField]
    private float maxPower;
    [SerializeField]
    private float[] lightTime;
    [SerializeField]
    private int[] lightningCount;

    [SerializeField]
    private bool callLightning;

    private List<int> lightningLine = new();
    private List<float> timeLine = new();
    private List<float> startLine = new();


    private float timer;

    private void Start()
    {
        main = this;
    }

    public void CallThuner_()
    {
        CallThunder();
    }

    public float CallImmThunder()
    {
        int lC = Random.Range(lightningCount[0], lightningCount[1]);
        for (int i = 0; i < lC; ++i)
        {
            lightningLine.Add(Random.Range(0, lights.Length));
            timeLine.Add(Random.Range(lightTime[0], lightTime[1]));
            startLine.Add(Random.Range(0f, lightTime[1] * 10f));
        }
        timer = 0f;

        SoundController.main.ImmThunder();
        return startLine[1];
    }

    public float CallThunder()
    {
        int lC = Random.Range(lightningCount[0], lightningCount[1]);
        for (int i = 0; i < lC; ++i)
        {
            lightningLine.Add(Random.Range(0, lights.Length));
            timeLine.Add(Random.Range(lightTime[0], lightTime[1]));
            startLine.Add(Random.Range(0f, lightTime[1] * 10f));
        }
        timer = 0f;

        SoundController.main.Thunder();
        return startLine[1];
    }

    private void CallLightning(int index, float time, float startTime)
    {
        lights[index].intensity = Mathf.Pow(Mathf.Sin(Mathf.PI * (timer - startTime) / time), 4) * maxPower;
    }

    void Update()
    {
        if(callLightning)
        {
            CallThunder();
            callLightning = false;
        }
        if(lightningLine.Count > 0)
        {
            for(int i = 0; i < startLine.Count; ++i)
            {
                if (timer >= startLine[i])
                {
                    CallLightning(lightningLine[i], timeLine[i], startLine[i]);
                    if (timer >= timeLine[i] + startLine[i])
                    {
                        lights[lightningLine[i]].intensity = 0f;
                        lightningLine.RemoveAt(i);
                        timeLine.RemoveAt(i);
                        startLine.RemoveAt(i);
                    }
                }
            }
            timer += Time.deltaTime;
        }
    }
}
