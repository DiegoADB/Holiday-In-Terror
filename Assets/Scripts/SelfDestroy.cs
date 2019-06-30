using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [SerializeField]
    private float bloodParticleDuration = 3.0f;
    void Start()
    {
        Invoke("Autodestruct", bloodParticleDuration);
    }

    void Autodestruct()
    {
        Destroy(this.gameObject);
    }
}
