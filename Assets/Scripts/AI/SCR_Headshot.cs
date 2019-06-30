using UnityEngine;
using Valve.VR.InteractionSystem;

public class SCR_Headshot : MonoBehaviour
{
    private SCR_Guest stateMachine;


    private void Start()
    {
        stateMachine = transform.root.GetComponent<SCR_Guest>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            stateMachine.SetInteractable(other.transform.parent.GetComponent<Interactable>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            if (!other.gameObject.transform.parent.GetComponent<Interactable>().isPenetrable)
            {
                stateMachine.SpawnBlood(other.ClosestPointOnBounds(transform.position));
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            stateMachine.ClearWeapon();
        }
    }
}
