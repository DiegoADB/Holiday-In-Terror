using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaserInput : MonoBehaviour
{

    public static GameObject currentObject;
    int currentID;
    RaycastHit[] hits;

    // Start is called before the first frame update
    void Start()
    {
        currentObject = null;
        currentID = 0;
    }

    // Update is called once per frame
    void Update()
    {
        hits = Physics.RaycastAll(transform.position, transform.forward, 100.0f);

        for(int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            int id = hit.collider.gameObject.GetInstanceID();

            if(currentID != id)
            {
                currentObject = hit.collider.gameObject;

                string name = currentObject.name;

                string tag = currentObject.tag;
                if(CompareTag("Button"))
                {
                    //currentObject.GetComponent<Button>().
                }
            }
        }
    }
}
