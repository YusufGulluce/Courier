using UnityEngine;
using System.Collections;

public class PlayerLook : MonoBehaviour
{
    [SerializeField]
    public float lookSpeed = 4;

    [SerializeField]
    private LayerMask interactables;
    private Vector2 rotation = Vector2.zero;

    public bool[] handLocked;
    private bool[] ghostHand;
    private Interactionable[] ghostInteraction;


    public static PlayerLook main;
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        ghostHand = new bool[] { false, false };
        handLocked = new bool[] { false, false };


        ghostInteraction = new Interactionable[] { null, null };

        main = this;

    }

    private void Update()
    {
        Look();

        CheckInteract(0);
        CheckInputs(0);


    }


    public void Look()
    {
        rotation.y += Input.GetAxis("Mouse X");
        rotation.x += -Input.GetAxis("Mouse Y");
        rotation.x = Mathf.Clamp(rotation.x, -15f, 15f);
        transform.eulerAngles = new Vector2(rotation.x, rotation.y) * lookSpeed;
    }

    private void CheckInteract(int index)
    {
        Ray ray = new(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 50f, interactables))
        {
            //Debug.Log("interactable: " + hit.collider.transform.name);
            if (!ghostHand[index] && !handLocked[index])
            {
                ghostHand[index] = true;
                ghostInteraction[index] = hit.collider.GetComponent<Interactionable>();
                ghostInteraction[index].Enter(index);
            }
        }
        else if (ghostHand[index] && !handLocked[index])
        {
            ghostHand[index] = false;
            ghostInteraction[index].Exit(index);
            ghostInteraction[index] = null;
        }

    }

    private void CheckInputs(int index)
    {
        if (Input.GetMouseButtonDown(index) && !handLocked[index] && ghostHand[index])
        {
            handLocked[index] = true;
            ghostInteraction[index].Grab(index);
        }
        else if (Input.GetMouseButtonUp(index) && handLocked[index])
        {
            handLocked[index] = false;
            ghostInteraction[index].Free(index);

            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 50f, interactables) && hit.transform.GetComponent<Interactionable>() != ghostInteraction[index])
            {
                ghostInteraction[index].Exit(index);
                ghostHand[index] = false;
                ghostInteraction[index] = null;
            }
        }


    }

    public Vector3 InteractionPoint()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 50f, interactables))
            return hit.point;
        return Vector3.zero;
    }

    private void LockHands()
    {

    }

}
