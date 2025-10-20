using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Car : MonoBehaviour
{
    public int gear = 1;

    [SerializeField]
    public Vector3 velocity;
    private float oldVelocity = 0f;

    [SerializeField]
    private float direction;

    [SerializeField]
    private Transform steeringWheel;

    [SerializeField]
    private Transform map;
    [SerializeField]
    public Rigidbody mapRB;

    [SerializeField]
    private Transform cam;
    private Vector3 realPos;

    private void Start()
    {
        realPos = cam.position;
    }

    private void Update()
    {
        //HeadMovement();
        SetDirection();
        LimitVelocity();
        if (Input.GetKey(KeyCode.W))
            Gas();
        else if (Input.GetKey(KeyCode.Space))
            Brake();
    }

    private void SetDirection()
    {
        direction = steeringWheel.localRotation.eulerAngles.z;
        if (direction > 180f)
            direction -= 360f;
        direction *= -10f * Mathf.Sin(mapRB.velocity.z * .5f * Mathf.PI / 180f);

        map.RotateAround(steeringWheel.position, Vector3.up, direction * Time.deltaTime);
        map.localEulerAngles = new Vector3(50f, map.localEulerAngles.y, 0f);
    }

    private void LimitVelocity()
    {
        float power = mapRB.velocity.z * mapRB.velocity.z * .01f * Time.deltaTime;
        mapRB.velocity -= mapRB.velocity.normalized * power;
        map.position = new(map.position.x, 3f, map.position.z);
    }

    private void HeadMovement()
    {
        float acc = mapRB.velocity.z - oldVelocity;
        if (acc > 0.001f || acc < -0.001f)
        {
            cam.position = realPos + acc * Vector3.forward;
            oldVelocity = mapRB.velocity.z;
        }
        //a.format = RenderTextureFormat.


    }

    public void Gas()
    {
        velocity.z = gear * .5f * Time.deltaTime;
        mapRB.velocity -= velocity;
    }

    public void Brake()
    {
        mapRB.velocity -= mapRB.velocity.normalized * Time.deltaTime;

    }

}
