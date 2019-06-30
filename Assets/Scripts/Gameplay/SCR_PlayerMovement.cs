using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SCR_PlayerMovement : MonoBehaviour
{
    [Header("SteamVR Stuff")]
    public SteamVR_Action_Vector2 trackpadAction;
    public Hand hand;
    public GameObject grabPosition;
    public float trackpadTolerance;
    [Header("Movement")]
    public GameObject parent;
    public GameObject vrCamera;
    public Rigidbody rBody;
    public bool useRBody;
    public float forwardSpeed;
    public float strafeSpeed;
    public float backSpeed;
    [Header("Grabs")]
    public GameObject grabCollider;
    public bool isTriggered;

    Vector2 trackpadValues;
    Vector3 speed = Vector3.zero;


    private void OnEnable()
    {
        if (hand == null)
        {
            hand = GetComponent<Hand>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    //Prende collider para poder agarrar joyas, abrir puertas, etc.
    void Grabbing()
    {
        isTriggered = SteamVR_Input.GetState("isGrabbinRight",hand.handType);

        if(isTriggered)
        {
            grabCollider.SetActive(true);
        }
        else
        {
            grabCollider.SetActive(false);

        }
    }


    //Mueve al jugador
    void Movement()
    {
        //Consigue la posicion del player
        trackpadValues = SteamVR_Input.GetVector2("dPad", hand.handType, true);

        if (trackpadValues.x > trackpadTolerance)
        {
            speed.x = strafeSpeed;
        }
        else if (trackpadValues.x < -trackpadTolerance)
        {
            speed.x = -strafeSpeed;
        }
        if (trackpadValues.y > trackpadTolerance)
        {
            speed.z = forwardSpeed;
        }
        else if (trackpadValues.y < -trackpadTolerance)
        {
            speed.z = -backSpeed;
        }
        if (trackpadValues.y <= trackpadTolerance && trackpadValues.y >= -trackpadTolerance && trackpadValues.x <= trackpadTolerance && trackpadValues.x >= -trackpadTolerance)
        {
            speed = Vector3.zero;
        }

        //Tengo retraso nocturno alguien que haga bien el movimiento xd ya recibe los valores del control xd o yo mas al rato :p
        if (useRBody)
        {
            if (speed != Vector3.zero)
            {
                rBody.AddForce(Quaternion.Euler(Vector3.up * vrCamera.transform.rotation.eulerAngles.y) * speed, ForceMode.Impulse);
                //rBody.AddForce(new Vector3(speed.x, 0, speed.z), ForceMode.Impulse);

            }
            else
            {
                rBody.velocity = Vector3.zero;
            }
        }
        else
        {
            parent.transform.position += Quaternion.Euler(Vector3.up * vrCamera.transform.rotation.eulerAngles.y) * speed;
        }
    }

    //Consigue la posicion del player - Delegate
    void GetTrackpadAxis(SteamVR_Action_Vector2 _actionIn, SteamVR_Input_Sources _inputSource, Vector2 _axis, Vector2 _delta)
    {
        trackpadValues = _axis;

        //trackpadAction.AddOnUpdateListener(GetTrackpadAxis, hand.handType);

        //trackpadAction.RemoveOnUpdateListener(GetTrackpadAxis, hand.handType);
    }

}