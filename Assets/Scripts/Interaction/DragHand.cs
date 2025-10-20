using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragHand : MonoBehaviour
{
    private float distance;
    private float strength = 10f;

    public Rigidbody rb;
    public SpriteRenderer sr;

    private Vector3 staticAim;
    public bool isGhost;
    public bool freeze;


    private static List<DragHand> allHands;

    public void Start()
    {
        distance = (Camera.main.transform.position - transform.position).magnitude;
        rb = GetComponent<Rigidbody>();
        sr = GetComponent<SpriteRenderer>();
        staticAim = Vector3.zero;

        sr.sortingOrder = 10;
    }

    public void Joint(Rigidbody other)
    {
        if(!GetComponent<FixedJoint>())
            gameObject.AddComponent<FixedJoint>().connectedBody = other;
    }

    public void BreakJoint()
    {
        Destroy(GetComponent<FixedJoint>());
    }

    private void FixedUpdate()
    {
        Vector3 aim = PlayerLook.main.InteractionPoint();
        if (staticAim != Vector3.zero)
            aim = staticAim;
        else if(aim == Vector3.zero)
            aim = Camera.main.transform.forward * distance + Camera.main.transform.position;

        rb.velocity = (aim - transform.position) * strength;


        rb.angularDrag = .9f;
        rb.drag = .5f;
    }

    public static DragHand Create(Vector3 position)
    {
        Transform hand = new GameObject("Drag Hand").transform;
        hand.localScale = Vector3.one * 0.2f;
        hand.position = position;

        Rigidbody rb = hand.gameObject.AddComponent<Rigidbody>();
        SpriteRenderer sr = hand.gameObject.AddComponent<SpriteRenderer>();
        rb.useGravity = false;
        rb.mass = 1f;

        //rb.freezeRotation = true;
        rb.constraints = (RigidbodyConstraints)48;

        sr.enabled = false;

        DragHand h = hand.gameObject.AddComponent<DragHand>();
        h.freeze = false;

        if (allHands == null)
            allHands = new List<DragHand>();

        allHands.Add(h);
        return h;
    }

    public static void FreezeAllHands()
    {
        foreach (DragHand dh in allHands)
            dh.FreezeAim();
    }

    public static void FreeAllHands()
    {
        foreach (DragHand dh in allHands)
            dh.FreeAim();
    }



    public void FreezeAim()
    {
        if(!isGhost)
        {
            freeze = true;
            staticAim = PlayerLook.main.InteractionPoint();
        }
    }
    public void FreeAim()
    {
        if (!isGhost)
        {
            freeze = false;
            staticAim = Vector3.zero;
        }
    }

    private void OnDestroy()
    {
        if(allHands != null)
            allHands.Remove(this);
    }


}
