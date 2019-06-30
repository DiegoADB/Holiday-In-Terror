using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Door : MonoBehaviour
{
    public Animator anmtrDoor;

    //Animate Door
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stealy"))
        {
            anmtrDoor.SetBool("isOpen",true);
        }
    }
}
