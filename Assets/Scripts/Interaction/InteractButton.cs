using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class InteractButton : Interactionable
{
    [SerializeField]
    private bool press = false;
    private int frameIndex = 0;
    [SerializeField]
    private UnityEvent action;

    [SerializeField]
    private SpriteRenderer handSR;
    [SerializeField]
    private Sprite[] handSprites;

    [SerializeField]
    private Sprite[] buttonSprites;
    private int i = 1;

    public override void Enter(int index)
    {
        handSR.enabled = true;
        if(!press)
            handSR.sprite = handSprites[0];
    }

    public override void Exit(int index)
    {
        handSR.enabled = false;
    }

    public override void Free(int index)
    {
        if(!press)
            handSR.sprite = handSprites[0];
    }

    public override void Grab(int index)
    {
        if (!press)
            handSR.sprite = handSprites[1];
        else
        {
            frameIndex++;
            handSR.sprite = handSprites[frameIndex % handSprites.Length];
        }
        action.Invoke();

        if(buttonSprites.Length > 1)
        {
            i++;
            Debug.Log("index: " + i);
            GetComponent<SpriteRenderer>().sprite = buttonSprites[i % 2];
        }
    }
}
