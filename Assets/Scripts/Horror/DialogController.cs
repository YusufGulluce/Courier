using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public static DialogController main;
    private string[] sentences;
    private float[] timings;
    private int index;
    private float timer;

    [SerializeField]
    private Text textBox;

    private void Start()
    {
        main = this;
    }

    private void Update()
    {
        if(timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                NextSentence();
            }
        }
    }

    public void Read(string text, float[] timings)
    {
        sentences = text.Split('.');
        this.timings = timings;

        index = 0;
        timer = .01f;
    }

    public void NextSentence()
    {
        if (index < sentences.Length)
        {
            SetText(sentences[index]);
            timer = timings[index];
            index++;
        }
        else
        {
            timer = 0f;
            SetText("");
            SoundController.main.RadioBack();
        }
    }


    public void SetText(string text)
    {
        textBox.text = text;
    }

}
