using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheel : Interactionable
{
    [SerializeField]
    private Sprite[] handSprites;
    [SerializeField]
    private Material handMaterial;
    private Vector3 wheelUp;


    private Plane plane;
    private bool isGhost;
    private float angle;
    private float startAngle;
    private Transform handTR;
    private SpriteRenderer handSR;
    [SerializeField]
    private Transform camTR;
    [SerializeField]
    private float handDistance;

    private float startRotationZ;
    [SerializeField]
    private float totalAngle;

    private void Awake()
    {
        plane = new(transform.forward, transform.position);
        isGhost = true;

        wheelUp = transform.up;
        startRotationZ = 0f;
    }

    public override void Free(int index)
    {
        PlayerLook.main.handLocked[0] = false;
        PlayerLook.main.handLocked[1] = false;

        isGhost = true;

        handSR.color = new(1, 1, 1, 0.5f);
    }

    public override void Grab(int index)
    {

        PlayerLook.main.handLocked[0] = true;
        PlayerLook.main.handLocked[1] = true;

        isGhost = false;

        handSR.color = new(1, 1, 1, 1f);
        startRotationZ = transform.localRotation.eulerAngles.z;
    }

    public override void Enter(int index)
    {

        enabled = true;
        isGhost = true;

        GameObject hand = new("hand");
        handTR = hand.transform;
        handSR = hand.AddComponent<SpriteRenderer>();
        
        handTR.SetParent(transform);
        handSR.color = new(1, 1, 1, 0.5f);
        handTR.localScale = Vector3.one * 1f;
        handSR.sortingOrder = 10;
        handSR.material = handMaterial;

    }

    public override void Exit(int index)
    {
        Destroy(handTR.gameObject);
        enabled = false;
    }

    private void Update()
    {
        if (isGhost)
        {
            UpdateHandPosition();
            startAngle = UpdateRotation();
            UpdateHandSprite(startAngle);
        }
        else
        {
            float nextAngle = UpdateRotation() - startAngle;
                angle = nextAngle;
                UpdateHandSprite(angle + startAngle);
            UpdateWheelRotation(angle);
        }

    }

    private void UpdateHandPosition()
    {
        Ray ray = new(camTR.position, camTR.forward);

        plane.Raycast(ray, out float d);
        Vector3 point = ray.GetPoint(d);
        Vector3 vect = point - transform.position;
        vect.Normalize();

        handTR.position = transform.position + vect * handDistance;
        handTR.localRotation = Quaternion.Euler(handTR.rotation.x, handTR.rotation.y, startAngle - transform.localRotation.eulerAngles.z);
    }

    private float UpdateRotation()
    {
        Ray ray = new (camTR.position, camTR.forward);

        plane.Raycast(ray, out float d);
        Vector3 point = ray.GetPoint(d);
        Vector3 vect =  point - transform.position;

        float angle = Vector3.Angle(wheelUp, vect) * (vect.x > 0 ? -1 : 1);
        return angle;
    }

    private void UpdateHandSprite(float angle)
    {

        if(angle >= -45f && angle <= 45f)
        {
            if (handSR.sprite != handSprites[0])
                handSR.sprite = handSprites[0];
        }
        else if (angle < -45f && angle > -135f)
        {
            if (handSR.sprite != handSprites[1])
                handSR.sprite = handSprites[1];
        }
        else if (angle > 45f && angle < 135f)
        {
            if (handSR.sprite != handSprites[2])
                handSR.sprite = handSprites[2];
        }
        else
        {
            if (handSR.sprite != handSprites[3])
                handSR.sprite = handSprites[3];
        }

    }


    private void UpdateWheelRotation(float angle)
    {
        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, angle + startRotationZ);
    }
}
