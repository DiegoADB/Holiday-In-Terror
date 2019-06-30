using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_StolenThing : MonoBehaviour
{
    public int id;
    public int value;
    public string name;

    //Agregar objetos a la lista del manager y destruirse para despues ser generados en la pantalla dewin/loose
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Stealy"))
        {
            SCR_GameManager.thingsStolen.Add(new StolenThing(id,value,name));
            Destroy(gameObject);
        }
    }

}
